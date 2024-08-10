using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using Unity.MLAgents;
using Unity.MLAgents.Actuators;
using Unity.MLAgents.Sensors;
using Random = UnityEngine.Random;

/*
* Walker Agent 1
* brain of the walker controlling the bodyparts and implementing the rl loop
*/
public class WalkerAgent1 : Agent
{
    [Header("Walk Speed")]
    public float minWalkingSpeed = 0.1f;
    public float maxWalkingSpeed = 10f;
    public bool randomizeWalkSpeedEachEpisode;
    public float targetWalkingSpeed = 0.1f;


    [Header("Target To Walk Towards")]
    public Transform target;
    public UnityEvent onTouchedTarget = new UnityEvent();

    [Header("Bodyparts")]
    public Transform root;
    public Transform head;
    public bool randomizeRotationOnEpsiode = true;
    [HideInInspector] public List<Bodypart> bodyparts = new List<Bodypart>();

    [Header("Debug Log Stats")]
    public bool logStats = false;

    //This will be used as a stabilized model space reference point for observations
    //Because ragdolls can move erratically during training, using a stabilized reference transform improves learning
    protected OrientationCubeController1 walkOrientationCube;
    public EnvironmentParameters m_ResetParams;
    public StatsRecorder statsRecorder;

    /*
    * Environment stats
    */
    protected Vector3 previousPos;
    protected float distanceMovedInTargetDirection;
    protected float lastReachedTargetTime = 0f;
    protected int reachedTargets;

    public override void Initialize()
    {
        //init orientation object
        GameObject orientationObject = new GameObject("OrientationObject");
        orientationObject.transform.parent = transform;
        walkOrientationCube = orientationObject.AddComponent<OrientationCubeController1>();
        walkOrientationCube.root = root;
        walkOrientationCube.target = target;

        //change to auto setup each body part
        foreach (Bodypart bp in root.GetComponentsInChildren<Bodypart>())
        {
            bp.Initialize();
            bp.onTouchedGround.AddListener(OnTouchedGround);
            bp.onTouchedTarget.AddListener(OnTouchedTarget);
            bodyparts.Add(bp);
        }

        m_ResetParams = Academy.Instance.EnvironmentParameters;
        statsRecorder = Academy.Instance.StatsRecorder;
    }

    /// <summary>
    /// Loop over body parts and reset them to initial conditions.
    /// </summary>
    public override void OnEpisodeBegin()
    {
        //Reset all of the body parts
        foreach (Bodypart bp in bodyparts)
        {
            bp.ResetTransform();
        }

        //Random start rotation to help generalize
        if (randomizeRotationOnEpsiode)
        {
            root.rotation = Quaternion.Euler(0, Random.Range(0.0f, 360.0f), 0);
        }

        //Set our goal walking speed
        targetWalkingSpeed =
            randomizeWalkSpeedEachEpisode ? Random.Range(minWalkingSpeed, maxWalkingSpeed) : targetWalkingSpeed;

        //record walking speed stats
        RecordStat("Environment/WalkingSpeed", targetWalkingSpeed);

        //record then reset distance moved in target direction
        RecordStat("Environment/DistanceMovedInTargetDirection", distanceMovedInTargetDirection);
        distanceMovedInTargetDirection = 0f;
        previousPos = root.position;

        //record then reset targets reached
        RecordStat("Environment/ReachedTargets", reachedTargets);
        reachedTargets = 0;
    }

    /// <summary>
    /// Add relevant information on each body part to observations.
    /// </summary>
    public void CollectObservationBodyPart(Bodypart bp, VectorSensor sensor)
    {
        //GROUND CHECK
        sensor.AddObservation(bp.touchingGround); // Is this bp touching the ground

        //Get velocities in the context of our orientation cube's space
        //Note: You can get these velocities in world space as well but it may not train as well.
        sensor.AddObservation(walkOrientationCube.transform.InverseTransformDirection(bp.rb.velocity));
        sensor.AddObservation(walkOrientationCube.transform.InverseTransformDirection(bp.rb.angularVelocity));

        //Get position relative to root in the context of our orientation cube's space
        sensor.AddObservation(walkOrientationCube.transform.InverseTransformDirection(bp.rb.position - root.position));

        //return when bodypart joint has no free rotation axis
        if (bp.dof.sqrMagnitude <= 0) return;

        sensor.AddObservation(bp.rb.transform.localRotation);
        sensor.AddObservation(bp.currentStrength / bp.physicsConfig.maxJointForceLimit);
    }

    /// <summary>
    /// Loop over body parts to add them to observation.
    /// </summary>
    public override void CollectObservations(VectorSensor sensor)
    {
        var cubeForward = walkOrientationCube.transform.forward;

        //velocity we want to match
        var velGoal = cubeForward * targetWalkingSpeed;
        //ragdoll's avg vel
        var avgVel = GetAvgVelocity();

        //current ragdoll velocity. normalized
        sensor.AddObservation(Vector3.Distance(velGoal, avgVel));
        //avg body vel relative to cube
        sensor.AddObservation(walkOrientationCube.transform.InverseTransformDirection(avgVel));
        //vel goal relative to cube
        sensor.AddObservation(walkOrientationCube.transform.InverseTransformDirection(velGoal));

        //rotation deltas
        sensor.AddObservation(Quaternion.FromToRotation(root.forward, cubeForward));
        sensor.AddObservation(Quaternion.FromToRotation(head.forward, cubeForward));

        //Position of target position relative to cube
        sensor.AddObservation(walkOrientationCube.transform.InverseTransformPoint(target.transform.position));

        foreach (Bodypart bp in bodyparts)
        {
            CollectObservationBodyPart(bp, sensor);
        }
    }

    public override void OnActionReceived(ActionBuffers actionBuffers)
    {
        var continuousActions = actionBuffers.ContinuousActions;
        int i = -1;

        foreach (Bodypart bp in bodyparts)
        {
            if (bp.dof.sqrMagnitude <= 0) continue;
            float targetRotX = bp.dof.x == 1 ? continuousActions[++i] : 0;
            float targetRotY = bp.dof.y == 1 ? continuousActions[++i] : 0;
            float targetRotZ = bp.dof.z == 1 ? continuousActions[++i] : 0;
            float jointStrength = continuousActions[++i];
            bp.SetJointTargetRotation(targetRotX, targetRotY, targetRotZ);
            bp.SetJointStrength(jointStrength);
        }
    }

    public virtual void FixedUpdate()
    {
        distanceMovedInTargetDirection += GetDistanceMovedInTargetDirection();

        var cubeForward = walkOrientationCube.transform.forward;

        // Set reward for this step according to mixture of the following elements.
        // a. Match target speed
        //This reward will approach 1 if it matches perfectly and approach zero as it deviates
        var matchSpeedReward = GetMatchingVelocityReward(cubeForward * targetWalkingSpeed, GetAvgVelocity());
        RecordStat("Reward/MatchingVelocityReward", matchSpeedReward);

        //Check for NaNs
        if (float.IsNaN(matchSpeedReward))
        {
            throw new ArgumentException(
                "NaN in moveTowardsTargetReward.\n" +
                $" cubeForward: {cubeForward}\n" +
                $" root.velocity: {bodyparts[0].rb.velocity}\n" +
                $" maximumWalkingSpeed: {maxWalkingSpeed}"
            );
        }

        // b. Rotation alignment with target direction.
        //This reward will approach 1 if it faces the target direction perfectly and approach zero as it deviates
        var headForward = head.forward;
        headForward.y = 0;
        // var lookAtTargetReward = (Vector3.Dot(cubeForward, head.forward) + 1) * .5F;
        var lookAtTargetReward = (Vector3.Dot(cubeForward, headForward) + 1) * .5F;
        RecordStat("Reward/LookAtTargetReward", lookAtTargetReward);

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

    //Returns the average velocity of all of the body parts
    //Using the velocity of the hips only has shown to result in more erratic movement from the limbs, so...
    //...using the average helps prevent this erratic movement
    public Vector3 GetAvgVelocity()
    {
        Vector3 velSum = Vector3.zero;

        foreach (Bodypart bp in bodyparts)
        {
            velSum += bp.rb.velocity;
        }

        var avgVel = velSum / bodyparts.Count;
        return avgVel;
    }

    //normalized value of the difference in avg speed vs goal walking speed.
    public float GetMatchingVelocityReward(Vector3 velocityGoal, Vector3 actualVelocity)
    {
        //distance between our actual velocity and goal velocity
        float upperLimit = Mathf.Max(0.1f, targetWalkingSpeed);
        var velDeltaMagnitude = Mathf.Clamp(Vector3.Distance(actualVelocity, velocityGoal), 0, upperLimit);

        //return the value on a declining sigmoid shaped curve that decays from 1 to 0
        //This reward will approach 1 if it matches perfectly and approach zero as it deviates
        float matchingVelocityReward = Mathf.Pow(1 - Mathf.Pow(velDeltaMagnitude / upperLimit, 2), 2);
        return matchingVelocityReward;
    }

    protected float GetDistanceMovedInTargetDirection()
    {
        //calculate the displacement vector
        Vector3 currentPos = root.position;
        Vector3 displacement = currentPos - previousPos;

        //project the displacement vector onto the goal direction vector
        float movementInTargetDirection = Vector3.Dot(displacement, walkOrientationCube.transform.forward);

        //update the previous position for the next frame
        previousPos = currentPos;
        return movementInTargetDirection;
    }

    protected void OnTouchedTarget()
    {
        if (lastReachedTargetTime + 0.1f <= Time.time)
        {
            lastReachedTargetTime = Time.time;
            reachedTargets++;
            onTouchedTarget.Invoke();
        }
    }

    protected void OnTouchedGround()
    {
        //check that the episode did not start in the last step to remove duplicate calls
        if (Academy.Instance.StepCount < 1) return;
        SetReward(-1f);
        EndEpisode();
    }

    protected void RecordStat(string path, float value)
    {
        if (logStats) Debug.Log($"{path}: {value}");
        statsRecorder.Add(path, value);
    }
}
