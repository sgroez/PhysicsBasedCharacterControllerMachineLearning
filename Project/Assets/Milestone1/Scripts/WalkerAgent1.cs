using System;
using System.Collections.Generic;
using UnityEngine;
using Unity.MLAgents;
using Unity.MLAgents.Actuators;
using Unity.MLAgents.Sensors;
using Random = UnityEngine.Random;

/**********************************************************************************************
* CHANGELOG
* Changed bodypart configuration to find bodypart transforms using GetComponentsInChildren<Rigidbody>
* Shortened Action Code using Loop over bodyparts (also makes it more diverse <- usable for more komplex body structures)
* Removed direction indicator
* Removed unused variable m_WorldDirToWalk
* Added automated orientationCube creation
* Changed from using JointDriveController to using Bodypart
* Changed OnActionReceived to ignore all bodyparts with dof (0,0,0)
* Added Event Listener for Bodypart touching ground
* Added Check for duplicate touching ground triggers to avoid multiple episode end calls
**********************************************************************************************/

public class WalkerAgent1 : Agent
{
    [Header("Walk Speed")]
    [Range(0.1f, 10)]
    [SerializeField]
    //The walking speed to try and achieve
    protected float m_TargetWalkingSpeed = 10;

    public float MTargetWalkingSpeed // property
    {
        get { return m_TargetWalkingSpeed; }
        set { m_TargetWalkingSpeed = Mathf.Clamp(value, .1f, m_maxWalkingSpeed); }
    }

    protected const float m_maxWalkingSpeed = 10; //The max walking speed

    //Should the agent sample a new goal velocity each episode?
    //If true, walkSpeed will be randomly set between zero and m_maxWalkingSpeed in OnEpisodeBegin()
    //If false, the goal velocity will be walkingSpeed
    public bool randomizeWalkSpeedEachEpisode;

    [Header("Target To Walk Towards")]
    public Transform target;
    public bool endEpisodeOnTargetReached = false;

    [Header("Bodyparts")]
    public Transform root;
    public Transform head;
    public bool randomizeRotationOnEpsiode = true;
    public List<Bodypart> bodyparts = new List<Bodypart>();

    [Header("Debug Log Stats")]
    public bool logStats = false;

    //This will be used as a stabilized model space reference point for observations
    //Because ragdolls can move erratically during training, using a stabilized reference transform improves learning
    protected OrientationCubeController m_OrientationCube;
    public EnvironmentParameters m_ResetParams;
    public StatsRecorder statsRecorder;

    /*
    * Environment stats
    */
    protected Vector3 previousPos;
    protected float distanceMovedInTargetDirection;
    protected int reachedTargets;
    protected float lastReachedTargetTime = 0f;

    public override void Initialize()
    {
        //init orientation object
        GameObject orientationObject = new GameObject("OrientationObject");
        orientationObject.transform.parent = transform;
        m_OrientationCube = orientationObject.AddComponent<OrientationCubeController>();
        target.GetComponent<TargetController>().onCollisionEnterEvent.AddListener(ReachedTarget);

        //change to auto setup each body part
        foreach (Bodypart bps in root.GetComponentsInChildren<Bodypart>())
        {
            bps.Initialize();
            bps.onTouchingGround.AddListener(OnTouchingGround);
            bodyparts.Add(bps);
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
        foreach (Bodypart bps in bodyparts)
        {
            bps.Reset();
        }

        //Random start rotation to help generalize
        if (randomizeRotationOnEpsiode)
        {
            root.rotation = Quaternion.Euler(0, Random.Range(0.0f, 360.0f), 0);
        }

        UpdateOrientationObjects();

        //Set our goal walking speed
        MTargetWalkingSpeed =
            randomizeWalkSpeedEachEpisode ? Random.Range(0.1f, m_maxWalkingSpeed) : MTargetWalkingSpeed;

        //record walking speed stats
        statsRecorder.Add("Environment/WalkingSpeed", MTargetWalkingSpeed);
        //record then reset distance moved in target direction
        statsRecorder.Add("Environment/DistanceMovedInTargetDirection", distanceMovedInTargetDirection);
        if (logStats) Debug.Log($"distance moved: {distanceMovedInTargetDirection}");
        distanceMovedInTargetDirection = 0f;
        previousPos = root.position;
        //record then reset targets reached
        statsRecorder.Add("Environment/ReachedTargets", reachedTargets);
        if (logStats) Debug.Log($"reached targets: {reachedTargets}");
        reachedTargets = 0;
    }

    /// <summary>
    /// Add relevant information on each body part to observations.
    /// </summary>
    public void CollectObservationBodyPart(Bodypart bps, VectorSensor sensor)
    {
        //GROUND CHECK
        sensor.AddObservation(bps.touchingGround); // Is this bps touching the ground

        //Get velocities in the context of our orientation cube's space
        //Note: You can get these velocities in world space as well but it may not train as well.
        sensor.AddObservation(m_OrientationCube.transform.InverseTransformDirection(bps.rb.velocity));
        sensor.AddObservation(m_OrientationCube.transform.InverseTransformDirection(bps.rb.angularVelocity));

        //Get position relative to root in the context of our orientation cube's space
        sensor.AddObservation(m_OrientationCube.transform.InverseTransformDirection(bps.rb.position - root.position));

        //return when bodypart joint has no free rotation axis
        if (bps.dof.sqrMagnitude <= 0) return;

        sensor.AddObservation(bps.rb.transform.localRotation);
        sensor.AddObservation(bps.currentStrength / bps.physicsConfig.maxJointForceLimit);

    }

    /// <summary>
    /// Loop over body parts to add them to observation.
    /// </summary>
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

        foreach (Bodypart bps in bodyparts)
        {
            CollectObservationBodyPart(bps, sensor);
        }
    }

    public override void OnActionReceived(ActionBuffers actionBuffers)
    {
        var continuousActions = actionBuffers.ContinuousActions;
        int i = -1;

        foreach (Bodypart bps in bodyparts)
        {
            if (bps.dof.sqrMagnitude <= 0) continue;
            float targetRotX = bps.joint.angularXMotion != ConfigurableJointMotion.Locked ? continuousActions[++i] : 0;
            float targetRotY = bps.joint.angularYMotion != ConfigurableJointMotion.Locked ? continuousActions[++i] : 0;
            float targetRotZ = bps.joint.angularZMotion != ConfigurableJointMotion.Locked ? continuousActions[++i] : 0;
            float jointStrength = continuousActions[++i];
            bps.SetJointTargetRotation(targetRotX, targetRotY, targetRotZ);
            bps.SetJointStrength(jointStrength);
        }
    }

    //Update OrientationCube
    protected virtual void UpdateOrientationObjects()
    {
        m_OrientationCube.UpdateOrientation(root, target);
    }

    public virtual void FixedUpdate()
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
        // var lookAtTargetReward = (Vector3.Dot(cubeForward, head.forward) + 1) * .5F;
        var lookAtTargetReward = (Vector3.Dot(cubeForward, headForward) + 1) * .5F;
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

    //Returns the average velocity of all of the body parts
    //Using the velocity of the hips only has shown to result in more erratic movement from the limbs, so...
    //...using the average helps prevent this erratic movement
    public Vector3 GetAvgVelocity()
    {
        Vector3 velSum = Vector3.zero;

        foreach (Bodypart bps in bodyparts)
        {
            velSum += bps.rb.velocity;
        }

        var avgVel = velSum / bodyparts.Count;
        return avgVel;
    }

    //normalized value of the difference in avg speed vs goal walking speed.
    public float GetMatchingVelocityReward(Vector3 velocityGoal, Vector3 actualVelocity)
    {
        //distance between our actual velocity and goal velocity
        var velDeltaMagnitude = Mathf.Clamp(Vector3.Distance(actualVelocity, velocityGoal), 0, MTargetWalkingSpeed);

        //return the value on a declining sigmoid shaped curve that decays from 1 to 0
        //This reward will approach 1 if it matches perfectly and approach zero as it deviates
        float matchingVelocityReward = Mathf.Pow(1 - Mathf.Pow(velDeltaMagnitude / MTargetWalkingSpeed, 2), 2);
        return matchingVelocityReward;
    }

    protected float GetDistanceMovedInTargetDirection()
    {
        //calculate the displacement vector
        Vector3 currentPos = root.position;
        Vector3 displacement = currentPos - previousPos;

        //project the displacement vector onto the goal direction vector
        float movementInTargetDirection = Vector3.Dot(displacement, m_OrientationCube.transform.forward);

        //update the previous position for the next frame
        previousPos = currentPos;
        return movementInTargetDirection;
    }

    protected void ReachedTarget(Collision collision)
    {
        if (lastReachedTargetTime + 0.1f <= Time.time)
        {
            lastReachedTargetTime = Time.time;
            reachedTargets++;
            if (endEpisodeOnTargetReached) EndEpisode();
        }
    }

    protected void OnTouchingGround()
    {
        //check that the episode did not start in the last step to remove duplicate calls
        if (Academy.Instance.StepCount < 1) return;
        SetReward(-1f);
        EndEpisode();
    }
}
