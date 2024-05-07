using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.MLAgents;
using Unity.MLAgents.Actuators;
using Unity.MLAgents.Sensors;
using Random = UnityEngine.Random;

public class WalkerAgentSimple : Agent
{
    [Header("Walking Speed")]
    public float maxWalkingSpeed;
    private float walkingSpeed;
    protected Transform walkingDirection;

    [Header("Target To Walk Towards")]
    public Transform target;

    private TargetControllerSimple targetController;

    [Header("Body Parts")]
    public Transform root;
    public Transform head;
    public Transform handL;
    public Transform handR;
    public List<Transform> bodypartTransforms = new List<Transform>();
    private Dictionary<Transform, Bodypart> bodyPartsDict = new Dictionary<Transform, Bodypart>();

    [Header("Bodypart Settings")]
    [Space(10)]
    public BodypartConfig bpConfig;

    public EnvironmentParameters resetParams;

    public override void Initialize()
    {
        targetController = target.GetComponent<TargetControllerSimple>();
        InitOrientationGoals();

        foreach (Transform t in bodypartTransforms)
        {
            var bp = new Bodypart(t, bpConfig, this);
            bodyPartsDict.Add(t, bp);
        }

        float actionCount = 0;
        foreach (Bodypart bp in bodyPartsDict.Values)
        {
            //add up x + y + z + strength if x || y || z is 1
            actionCount += bp.dof.x + bp.dof.y + bp.dof.z + (bp.dof.sqrMagnitude > 0 ? 1 : 0);
        }
        Debug.Log("Continuous actions: " + actionCount);

        resetParams = Academy.Instance.EnvironmentParameters;

        //load max walking speed from env params if available
        maxWalkingSpeed = resetParams.GetWithDefault("maxWalkingSpeed", maxWalkingSpeed);
        //Set our goal walking speed
        walkingSpeed = Random.Range(0.1f, maxWalkingSpeed);
    }

    public override void OnEpisodeBegin()
    {
        //Reset all of the body parts
        foreach (Bodypart bp in bodyPartsDict.Values)
        {
            bp.Reset();
        }

        //Random start rotation to help generalize
        root.rotation = Quaternion.Euler(0, Random.Range(0.0f, 360.0f), 0);

        targetController.MoveTargetToRandomPosition();

        UpdateOrientationGoals();

        //Set our goal walking speed
        walkingSpeed = Random.Range(0.1f, maxWalkingSpeed);
    }

    private void CollectObservationBodyPart(Bodypart bp, VectorSensor sensor)
    {
        //GROUND CHECK
        sensor.AddObservation(bp.groundContact.touchingGround); // Is this bp touching the ground

        //Get velocities in the context of our orientation cube's space
        //Note: You can get these velocities in world space as well but it may not train as well.
        sensor.AddObservation(walkingDirection.InverseTransformDirection(bp.rb.velocity));
        sensor.AddObservation(walkingDirection.InverseTransformDirection(bp.rb.angularVelocity));

        //Get position relative to hips in the context of our orientation cube's space
        sensor.AddObservation(walkingDirection.InverseTransformDirection(bp.rb.position - root.position));

        if (bp.rb.transform != root && bp.rb.transform != handL && bp.rb.transform != handR)
        {
            sensor.AddObservation(bp.rb.transform.localRotation);
            sensor.AddObservation(bp.joint.slerpDrive.maximumForce / bp.config.maxJointForceLimit);
        }
    }

    public override void CollectObservations(VectorSensor sensor)
    {
        //velocity we want to match
        var velGoal = walkingDirection.forward * walkingSpeed;
        //ragdoll's avg vel
        var avgVel = GetAvgVelocity();

        //current ragdoll velocity. normalized
        sensor.AddObservation(Vector3.Distance(velGoal, avgVel));
        //avg body vel relative to cube
        sensor.AddObservation(walkingDirection.InverseTransformDirection(avgVel));
        //vel goal relative to cube
        sensor.AddObservation(walkingDirection.InverseTransformDirection(velGoal));

        //rotation deltas
        CollectRotationDeltas(sensor);

        //Position of target position relative to cube
        sensor.AddObservation(walkingDirection.InverseTransformPoint(target.transform.position));

        foreach (Bodypart bp in bodyPartsDict.Values)
        {
            CollectObservationBodyPart(bp, sensor);
        }
    }

    public virtual void CollectRotationDeltas(VectorSensor sensor)
    {
        sensor.AddObservation(Quaternion.FromToRotation(root.forward, walkingDirection.forward));
        sensor.AddObservation(Quaternion.FromToRotation(head.forward, walkingDirection.forward));
    }

    private void OnActionReceivedBodypart(Bodypart bp, float targetRotX, float targetRotY, float targetRotZ, float jointStrength)
    {
        bp.SetJointTargetRotation(targetRotX, targetRotY, targetRotZ);
        bp.SetJointStrength(jointStrength);
    }

    public override void OnActionReceived(ActionBuffers actionBuffers)
    {
        int index = -1;
        var continuousActions = actionBuffers.ContinuousActions;

        foreach (Bodypart bp in bodyPartsDict.Values)
        {
            if (bp.rb.transform == root || bp.rb.transform == handL || bp.rb.transform == handR) continue;
            float targetRotX = bp.dof.x == 1 ? continuousActions[++index] : 0;
            float targetRotY = bp.dof.y == 1 ? continuousActions[++index] : 0;
            float targetRotZ = bp.dof.z == 1 ? continuousActions[++index] : 0;
            float jointStrength = bp.dof.sqrMagnitude > 0 ? continuousActions[++index] : 0;
            OnActionReceivedBodypart(bp, targetRotX, targetRotY, targetRotZ, jointStrength);
        }
    }

    void FixedUpdate()
    {
        UpdateOrientationGoals();

        // Set reward for this step according to mixture of the following elements.
        // a. Match target speed
        //This reward will approach 1 if it matches perfectly and approach zero as it deviates
        var matchSpeedReward = GetMatchingVelocityReward();

        //Check for NaNs
        if (float.IsNaN(matchSpeedReward))
        {
            throw new ArgumentException(
                "NaN in moveTowardsTargetReward.\n" +
                $" walkingDirection: {walkingDirection.forward}\n" +
                $" hips.velocity: {bodyPartsDict[root].rb.velocity}\n" +
                $" maximumWalkingSpeed: {maxWalkingSpeed}"
            );
        }

        // b. Rotation alignment with target direction.
        //This reward will approach 1 if it faces the target direction perfectly and approach zero as it deviates
        var lookAtTargetReward = GetLookAtTargetReward();

        //Check for NaNs
        if (float.IsNaN(lookAtTargetReward))
        {
            throw new ArgumentException(
                "NaN in lookAtTargetReward.\n" +
                $" walkingDirection: {walkingDirection.forward}\n" +
                $" head.forward: {head.forward}"
            );
        }

        AddReward(matchSpeedReward * lookAtTargetReward);
    }

    private Vector3 GetAvgVelocity()
    {
        Vector3 velSum = Vector3.zero;

        //ALL RBS
        int numOfRb = 0;
        foreach (Bodypart bp in bodyPartsDict.Values)
        {
            numOfRb++;
            velSum += bp.rb.velocity;
        }

        var avgVel = velSum / numOfRb;
        return avgVel;
    }

    private float GetMatchingVelocityReward()
    {
        Vector3 velocityGoal = walkingDirection.forward * walkingSpeed;
        Vector3 actualVelocity = GetAvgVelocity();
        //distance between our actual velocity and goal velocity
        var velDeltaMagnitude = Mathf.Clamp(Vector3.Distance(actualVelocity, velocityGoal), 0, walkingSpeed);

        //return the value on a declining sigmoid shaped curve that decays from 1 to 0
        //This reward will approach 1 if it matches perfectly and approach zero as it deviates
        return Mathf.Pow(1 - Mathf.Pow(velDeltaMagnitude / walkingSpeed, 2), 2);
    }

    public virtual float GetLookAtTargetReward()
    {
        var headForward = head.forward;
        headForward.y = 0;
        return (Vector3.Dot(walkingDirection.forward, headForward) + 1) * .5F;
    }

    public virtual void InitOrientationGoals()
    {
        walkingDirection = InitDirectionTransform("walkingDirection");
    }

    public virtual void UpdateOrientationGoals()
    {
        UpdateDirectionTransform(walkingDirection, target);
    }

    protected Transform InitDirectionTransform(string name)
    {
        //create cube
        GameObject walkingDirectionCube = GameObject.CreatePrimitive(PrimitiveType.Cube);
        //set name
        walkingDirectionCube.name = name;
        //disable rendering and collision components
        walkingDirectionCube.GetComponent<MeshRenderer>().enabled = false;
        walkingDirectionCube.GetComponent<BoxCollider>().enabled = false;
        //set parent
        walkingDirectionCube.transform.parent = transform;
        return walkingDirectionCube.transform;
    }

    protected void UpdateDirectionTransform(Transform directionTransform, Transform target)
    {
        var dirVector = target.position - root.position;
        dirVector.y = 0; //flatten dir on the y. this will only work on level, uneven surfaces
        var lookRot =
            dirVector == Vector3.zero
                ? Quaternion.identity
                : Quaternion.LookRotation(dirVector); //get our look rot to the target

        //UPDATE ORIENTATION CUBE POS & ROT
        directionTransform.SetPositionAndRotation(root.position, lookRot);
    }
}
