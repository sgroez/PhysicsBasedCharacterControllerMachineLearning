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
**********************************************************************************************/

public class WalkerAgentSimple : Agent
{
    [Header("Walk Speed")]
    [Range(0.1f, 10)]
    [SerializeField]
    //The walking speed to try and achieve
    private float m_TargetWalkingSpeed = 10;

    public float MTargetWalkingSpeed // property
    {
        get { return m_TargetWalkingSpeed; }
        set { m_TargetWalkingSpeed = Mathf.Clamp(value, .1f, m_maxWalkingSpeed); }
    }

    const float m_maxWalkingSpeed = 10; //The max walking speed

    //Should the agent sample a new goal velocity each episode?
    //If true, walkSpeed will be randomly set between zero and m_maxWalkingSpeed in OnEpisodeBegin()
    //If false, the goal velocity will be walkingSpeed
    public bool randomizeWalkSpeedEachEpisode;

    [Header("Target To Walk Towards")] public Transform target; //Target the agent will walk towards during training.

    public Dictionary<string, Transform> bodypartTransformsMap = new Dictionary<string, Transform>();
    [HideInInspector] public Transform hips;
    [HideInInspector] public Transform chest;
    [HideInInspector] public Transform spine;
    [HideInInspector] public Transform head;
    [HideInInspector] public Transform thighL;
    [HideInInspector] public Transform shinL;
    [HideInInspector] public Transform footL;
    [HideInInspector] public Transform thighR;
    [HideInInspector] public Transform shinR;
    [HideInInspector] public Transform footR;
    [HideInInspector] public Transform armL;
    [HideInInspector] public Transform forearmL;
    [HideInInspector] public Transform handL;
    [HideInInspector] public Transform armR;
    [HideInInspector] public Transform forearmR;
    [HideInInspector] public Transform handR;

    //This will be used as a stabilized model space reference point for observations
    //Because ragdolls can move erratically during training, using a stabilized reference transform improves learning
    OrientationCubeController m_OrientationCube;

    JointDriveController m_JdController;
    public EnvironmentParameters m_ResetParams;
    public StatsRecorder statsRecorder;

    public override void Initialize()
    {
        //init orientation object
        GameObject orientationObject = new GameObject("OrientationObject");
        orientationObject.transform.parent = transform;
        m_OrientationCube = orientationObject.AddComponent<OrientationCubeController>();

        //change to auto setup each body part
        foreach (Rigidbody rb in GetComponentsInChildren<Rigidbody>())
        {
            bodypartTransformsMap.Add(rb.transform.name, rb.transform);
        }

        //map bodyparts to variables
        hips = bodypartTransformsMap["hips"];
        chest = bodypartTransformsMap["chest"];
        spine = bodypartTransformsMap["spine"];
        head = bodypartTransformsMap["head"];
        thighL = bodypartTransformsMap["thighL"];
        shinL = bodypartTransformsMap["shinL"];
        footL = bodypartTransformsMap["footL"];
        thighR = bodypartTransformsMap["thighR"];
        shinR = bodypartTransformsMap["shinR"];
        footR = bodypartTransformsMap["footR"];
        armL = bodypartTransformsMap["upper_arm_L"];
        forearmL = bodypartTransformsMap["lower_arm_L"];
        handL = bodypartTransformsMap["hand_L"];
        armR = bodypartTransformsMap["upper_arm_R"];
        forearmR = bodypartTransformsMap["lower_arm_R"];
        handR = bodypartTransformsMap["hand_R"];

        m_JdController = GetComponent<JointDriveController>();
        m_JdController.SetupBodyPart(hips);
        m_JdController.SetupBodyPart(chest);
        m_JdController.SetupBodyPart(spine);
        m_JdController.SetupBodyPart(head);
        m_JdController.SetupBodyPart(thighL);
        m_JdController.SetupBodyPart(shinL);
        m_JdController.SetupBodyPart(footL);
        m_JdController.SetupBodyPart(thighR);
        m_JdController.SetupBodyPart(shinR);
        m_JdController.SetupBodyPart(footR);
        m_JdController.SetupBodyPart(armL);
        m_JdController.SetupBodyPart(forearmL);
        m_JdController.SetupBodyPart(handL);
        m_JdController.SetupBodyPart(armR);
        m_JdController.SetupBodyPart(forearmR);
        m_JdController.SetupBodyPart(handR);

        m_ResetParams = Academy.Instance.EnvironmentParameters;
        statsRecorder = Academy.Instance.StatsRecorder;
    }

    /// <summary>
    /// Loop over body parts and reset them to initial conditions.
    /// </summary>
    public override void OnEpisodeBegin()
    {
        //Reset all of the body parts
        foreach (var bodyPart in m_JdController.bodyPartsDict.Values)
        {
            bodyPart.Reset(bodyPart);
        }

        //Random start rotation to help generalize
        hips.rotation = Quaternion.Euler(0, Random.Range(0.0f, 360.0f), 0);

        UpdateOrientationObjects();

        //Set our goal walking speed
        MTargetWalkingSpeed =
            randomizeWalkSpeedEachEpisode ? Random.Range(0.1f, m_maxWalkingSpeed) : MTargetWalkingSpeed;
    }

    /// <summary>
    /// Add relevant information on each body part to observations.
    /// </summary>
    public void CollectObservationBodyPart(BodyPart bp, VectorSensor sensor)
    {
        //GROUND CHECK
        sensor.AddObservation(bp.groundContact.touchingGround); // Is this bp touching the ground

        //Get velocities in the context of our orientation cube's space
        //Note: You can get these velocities in world space as well but it may not train as well.
        sensor.AddObservation(m_OrientationCube.transform.InverseTransformDirection(bp.rb.velocity));
        sensor.AddObservation(m_OrientationCube.transform.InverseTransformDirection(bp.rb.angularVelocity));

        //Get position relative to hips in the context of our orientation cube's space
        sensor.AddObservation(m_OrientationCube.transform.InverseTransformDirection(bp.rb.position - hips.position));

        if (bp.rb.transform != hips && bp.rb.transform != handL && bp.rb.transform != handR)
        {
            sensor.AddObservation(bp.rb.transform.localRotation);
            sensor.AddObservation(bp.currentStrength / m_JdController.maxJointForceLimit);
        }
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
        sensor.AddObservation(Quaternion.FromToRotation(hips.forward, cubeForward));
        sensor.AddObservation(Quaternion.FromToRotation(head.forward, cubeForward));

        //Position of target position relative to cube
        sensor.AddObservation(m_OrientationCube.transform.InverseTransformPoint(target.transform.position));

        foreach (var bodyPart in m_JdController.bodyPartsList)
        {
            CollectObservationBodyPart(bodyPart, sensor);
        }
    }

    public override void OnActionReceived(ActionBuffers actionBuffers)
    {
        var continuousActions = actionBuffers.ContinuousActions;
        int i = -1;

        foreach (KeyValuePair<Transform, BodyPart> kvp in m_JdController.bodyPartsDict)
        {
            if (kvp.Key == hips || kvp.Key == handL || kvp.Key == handR) continue;
            BodyPart bp = kvp.Value;
            float targetRotX = bp.joint.angularXMotion != ConfigurableJointMotion.Locked ? continuousActions[++i] : 0;
            float targetRotY = bp.joint.angularYMotion != ConfigurableJointMotion.Locked ? continuousActions[++i] : 0;
            float targetRotZ = bp.joint.angularZMotion != ConfigurableJointMotion.Locked ? continuousActions[++i] : 0;
            float jointStrength = continuousActions[++i];
            bp.SetJointTargetRotation(targetRotX, targetRotY, targetRotZ);
            bp.SetJointStrength(jointStrength);
        }
    }

    //Update OrientationCube
    void UpdateOrientationObjects()
    {
        m_OrientationCube.UpdateOrientation(hips, target);
    }

    void FixedUpdate()
    {
        UpdateOrientationObjects();

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
                $" hips.velocity: {m_JdController.bodyPartsDict[hips].rb.velocity}\n" +
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
    Vector3 GetAvgVelocity()
    {
        Vector3 velSum = Vector3.zero;

        //ALL RBS
        int numOfRb = 0;
        foreach (var item in m_JdController.bodyPartsList)
        {
            numOfRb++;
            velSum += item.rb.velocity;
        }

        var avgVel = velSum / numOfRb;
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
}
