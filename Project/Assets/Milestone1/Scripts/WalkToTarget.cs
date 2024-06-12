using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using Unity.MLAgents;
using Unity.MLAgents.Actuators;
using Unity.MLAgents.Sensors;
using Random = UnityEngine.Random;

public class WalkToTarget : Module
{
    [Header("Walkspeed")]
    public float maxWalkingSpeed = 10;
    private float targetWalkingSpeed = 10;
    public bool randomizeWalkSpeedEachEpisode;

    [Header("Target To Walk Towards")]
    public TargetControllerBase target;

    OrientationCubeController m_OrientationCube;

    /*
    * Environment stats
    */
    private Vector3 previousPos;
    private float distanceMovedInTargetDirection;
    [HideInInspector] public int reachedTargets;

    public override void Initialize(WalkerAgentBase baseAgent)
    {
        base.Initialize(baseAgent);
        m_OrientationCube = GetComponentInChildren<OrientationCubeController>();
        target.onTouchedTarget.AddListener(() => reachedTargets++);
    }

    public override void OnEpisodeBegin()
    {
        //Reset all of the body parts
        foreach (Bodypart bp in baseAgent.bodyparts)
        {
            bp.Reset();
        }

        //Random start rotation to help generalize
        baseAgent.root.rotation = Quaternion.Euler(0, Random.Range(0.0f, 360.0f), 0);

        m_OrientationCube.UpdateOrientation(baseAgent.root, target.transform);

        //Set our goal walking speed
        targetWalkingSpeed = randomizeWalkSpeedEachEpisode ? Random.Range(0.1f, maxWalkingSpeed) : maxWalkingSpeed;

        //record walking speed stats
        baseAgent.statsRecorder.Add("Environment/WalkingSpeed", targetWalkingSpeed);
        //record then reset distance moved in target direction
        baseAgent.statsRecorder.Add("Environment/DistanceMovedInTargetDirection", distanceMovedInTargetDirection);
        Debug.Log($"distance moved: {distanceMovedInTargetDirection}");
        distanceMovedInTargetDirection = 0f;
        previousPos = baseAgent.root.position;
        //record then reset targets reached
        baseAgent.statsRecorder.Add("Environment/ReachedTargets", reachedTargets);
        Debug.Log($"reached targets: {reachedTargets}");
        reachedTargets = 0;
    }

    public void CollectObservationBodyPart(Bodypart bp, VectorSensor sensor)
    {
        //GROUND CHECK
        sensor.AddObservation(bp.isTouchingGround); // Is this bp touching the ground

        //Get velocities in the context of our orientation cube's space
        //Note: You can get these velocities in world space as well but it may not train as well.
        sensor.AddObservation(m_OrientationCube.transform.InverseTransformDirection(bp.rb.velocity));
        sensor.AddObservation(m_OrientationCube.transform.InverseTransformDirection(bp.rb.angularVelocity));

        //Get position relative to hips in the context of our orientation cube's space
        sensor.AddObservation(m_OrientationCube.transform.InverseTransformDirection(bp.rb.position - baseAgent.root.position));

        if (bp.dof.sqrMagnitude > 0)
        {
            sensor.AddObservation(bp.rb.transform.localRotation);
            sensor.AddObservation(bp.currentStrength / bp.physicsConfig.maxJointForceLimit);
        }
    }

    public override void OnCollectObservations(VectorSensor sensor)
    {
        var cubeForward = m_OrientationCube.transform.forward;

        //velocity we want to match
        var velGoal = cubeForward * targetWalkingSpeed;
        //ragdoll's avg vel
        var avgVel = GetAvgVelocity();

        //current ragdoll velocity. normalized
        sensor.AddObservation(Vector3.Distance(velGoal, avgVel));
        //avg body vel relative to cube
        sensor.AddObservation(m_OrientationCube.transform.InverseTransformDirection(avgVel));
        //vel goal relative to cube
        sensor.AddObservation(m_OrientationCube.transform.InverseTransformDirection(velGoal));

        //rotation deltas
        sensor.AddObservation(Quaternion.FromToRotation(baseAgent.root.forward, cubeForward));
        sensor.AddObservation(Quaternion.FromToRotation(baseAgent.head.forward, cubeForward));

        //Position of target position relative to cube
        sensor.AddObservation(m_OrientationCube.transform.InverseTransformPoint(target.transform.position));

        foreach (Bodypart bp in baseAgent.bodyparts)
        {
            CollectObservationBodyPart(bp, sensor);
        }
    }

    public override void OnAgentFixedUpdate()
    {
        m_OrientationCube.UpdateOrientation(baseAgent.root, target.transform);
        distanceMovedInTargetDirection += GetDistanceMovedInTargetDirection();

        var cubeForward = m_OrientationCube.transform.forward;

        // Set reward for this step according to mixture of the following elements.
        // a. Match target speed
        //This reward will approach 1 if it matches perfectly and approach zero as it deviates
        Vector3 avgVelocity = GetAvgVelocity();
        var matchSpeedReward = GetMatchingVelocityReward(cubeForward * targetWalkingSpeed, avgVelocity);
        baseAgent.statsRecorder.Add("Reward/MatchingVelocityReward", matchSpeedReward);

        //Check for NaNs
        if (float.IsNaN(matchSpeedReward))
        {
            throw new ArgumentException(
                "NaN in moveTowardsTargetReward.\n" +
                $" cubeForward: {cubeForward}\n" +
                $" avgVelocity: {avgVelocity}\n" +
                $" maximumWalkingSpeed: {maxWalkingSpeed}"
            );
        }

        // b. Rotation alignment with target direction.
        //This reward will approach 1 if it faces the target direction perfectly and approach zero as it deviates
        var headForward = baseAgent.head.forward;
        headForward.y = 0;
        // var lookAtTargetReward = (Vector3.Dot(cubeForward, head.forward) + 1) * .5F;
        var lookAtTargetReward = (Vector3.Dot(cubeForward, headForward) + 1) * .5F;
        baseAgent.statsRecorder.Add("Reward/LookAtTargetReward", lookAtTargetReward);

        //Check for NaNs
        if (float.IsNaN(lookAtTargetReward))
        {
            throw new ArgumentException(
                "NaN in lookAtTargetReward.\n" +
                $" cubeForward: {cubeForward}\n" +
                $" head.forward: {baseAgent.head.forward}"
            );
        }

        baseAgent.AddReward(matchSpeedReward * lookAtTargetReward);
    }

    Vector3 GetAvgVelocity()
    {
        Vector3 velSum = Vector3.zero;

        foreach (Bodypart bp in baseAgent.bodyparts)
        {
            velSum += bp.rb.velocity;
        }

        var avgVel = velSum / baseAgent.bodyparts.Count;
        return avgVel;
    }

    public float GetMatchingVelocityReward(Vector3 velocityGoal, Vector3 actualVelocity)
    {
        //distance between our actual velocity and goal velocity
        var velDeltaMagnitude = Mathf.Clamp(Vector3.Distance(actualVelocity, velocityGoal), 0, targetWalkingSpeed);

        //return the value on a declining sigmoid shaped curve that decays from 1 to 0
        //This reward will approach 1 if it matches perfectly and approach zero as it deviates
        float matchingVelocityReward = Mathf.Pow(1 - Mathf.Pow(velDeltaMagnitude / targetWalkingSpeed, 2), 2);
        return matchingVelocityReward;
    }

    private float GetDistanceMovedInTargetDirection()
    {
        //calculate the displacement vector
        Vector3 currentPos = baseAgent.root.position;
        Vector3 displacement = currentPos - previousPos;

        //project the displacement vector onto the goal direction vector
        float movementInTargetDirection = Vector3.Dot(displacement, m_OrientationCube.transform.forward);

        //update the previous position for the next frame
        previousPos = currentPos;
        return movementInTargetDirection;
    }
}