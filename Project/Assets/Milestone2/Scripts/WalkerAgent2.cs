using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using Unity.MLAgents;
using Unity.MLAgents.Sensors;

public class WalkerAgent2 : WalkerAgent1
{
    [Header("Target To Look Towards")]
    public LookTargetController lookTargetController;
    OrientationCubeController lookOrientationCube;

    [System.Serializable] public class ResetEvent : UnityEvent<Vector3> { }
    public ResetEvent onResetEvent = new ResetEvent();

    public override void Initialize()
    {
        //init orientation object
        GameObject lookOrientationObject = new GameObject("LookOrientationObject");
        lookOrientationObject.transform.parent = transform;
        lookOrientationCube = lookOrientationObject.AddComponent<OrientationCubeController>();
        base.Initialize();
        float angleLimit = Academy.Instance.EnvironmentParameters.GetWithDefault("lookAngleLimit", lookTargetController.maxAngle);
        lookTargetController.minAngle = -angleLimit;
        lookTargetController.maxAngle = angleLimit;
    }

    public override void OnEpisodeBegin()
    {
        base.OnEpisodeBegin();
        float angleLimit = Academy.Instance.EnvironmentParameters.GetWithDefault("lookAngleLimit", lookTargetController.maxAngle);
        lookTargetController.minAngle = -angleLimit;
        lookTargetController.maxAngle = angleLimit;
        onResetEvent.Invoke(root.position);
    }

    // added look at target position
    public override void CollectObservations(VectorSensor sensor)
    {
        var cubeForward = m_OrientationCube.transform.forward;

        //velocity we want to match
        var velGoal = cubeForward * MTargetWalkingSpeed;
        //ragdoll's avg vel
        var avgVel = GetAvgVelocity();

        //current ragdoll velocity. normalized
        sensor.AddObservation(Vector3.Distance(velGoal, avgVel));
        //avg body vel relative to cube
        sensor.AddObservation(m_OrientationCube.transform.InverseTransformDirection(avgVel));
        //vel goal relative to cube
        sensor.AddObservation(m_OrientationCube.transform.InverseTransformDirection(velGoal));

        //rotation deltas
        sensor.AddObservation(Quaternion.FromToRotation(root.forward, cubeForward));
        sensor.AddObservation(Quaternion.FromToRotation(head.forward, cubeForward));

        //Position of target position relative to cube
        sensor.AddObservation(m_OrientationCube.transform.InverseTransformPoint(target.transform.position));

        //Position of look target position relative to cube
        sensor.AddObservation(m_OrientationCube.transform.InverseTransformPoint(lookTargetController.transform.position));

        foreach (Bodypart bps in bodyparts)
        {
            CollectObservationBodyPart(bps, sensor);
        }
    }

    protected override void UpdateOrientationObjects()
    {
        base.UpdateOrientationObjects();
        lookOrientationCube.UpdateOrientation(root, lookTargetController.transform);
    }

    public override void FixedUpdate()
    {
        UpdateOrientationObjects();
        distanceMovedInTargetDirection += GetDistanceMovedInTargetDirection();

        var cubeForward = m_OrientationCube.transform.forward;

        // Set reward for this step according to mixture of the following elements.
        // a. Match target speed
        //This reward will approach 1 if it matches perfectly and approach zero as it deviates
        var matchSpeedReward = GetMatchingVelocityReward(cubeForward * MTargetWalkingSpeed, GetAvgVelocity());
        statsRecorder.Add("Reward/MatchingVelocityReward", matchSpeedReward);

        //Check for NaNs
        if (float.IsNaN(matchSpeedReward))
        {
            throw new ArgumentException(
                "NaN in moveTowardsTargetReward.\n" +
                $" cubeForward: {cubeForward}\n" +
                $" root.velocity: {bodyparts[0].rb.velocity}\n" +
                $" maximumWalkingSpeed: {m_maxWalkingSpeed}"
            );
        }

        // b. Rotation alignment with target direction.
        //This reward will approach 1 if it faces the target direction perfectly and approach zero as it deviates
        var headForward = head.forward;
        headForward.y = 0;
        var lookCubeForward = lookOrientationCube.transform.forward;
        // var lookAtTargetReward = (Vector3.Dot(cubeForward, head.forward) + 1) * .5F;
        var lookAtTargetReward = (Vector3.Dot(lookCubeForward, headForward) + 1) * .5F;
        statsRecorder.Add("Reward/LookAtTargetReward", lookAtTargetReward);

        //Check for NaNs
        if (float.IsNaN(lookAtTargetReward))
        {
            throw new ArgumentException(
                "NaN in lookAtTargetReward.\n" +
                $" cubeForward: {cubeForward}\n" +
                $" head.forward: {head.forward}"
            );
        }

        AddReward(matchSpeedReward * lookAtTargetReward);
    }
}
