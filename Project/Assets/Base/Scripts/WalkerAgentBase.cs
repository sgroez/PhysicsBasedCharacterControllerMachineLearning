using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.MLAgents;
using Unity.MLAgents.Actuators;
using Unity.MLAgents.Sensors;
using Random = UnityEngine.Random;

public class WalkerAgentBase : Agent
{
    [Header("Target To Walk Towards")]
    [Space(10)]
    public Transform target;
    protected TargetControllerBase targetController;

    [Header("Body Parts")]
    [Space(10)]
    public Transform root;
    public Transform head;
    public Transform handL;
    public Transform handR;
    protected Dictionary<Transform, Bodypart> bodyPartsDict = new Dictionary<Transform, Bodypart>();

    [Header("Bodypart Settings")]
    [Space(10)]
    public BodypartConfig bpConfig;

    protected Transform walkingDirectionGoal;
    protected EnvironmentParameters resetParams;


    /*
    * Override Agent functions to comply with ML Agents package &
    * Implement reward structure in fixed update
    */
    public override void Initialize()
    {
        InitEnvVariables();
        foreach (Rigidbody rb in root.GetComponentsInChildren<Rigidbody>())
        {
            Transform t = rb.transform;
            var bp = new Bodypart(t, bpConfig, this);
            bodyPartsDict.Add(t, bp);
        }
        resetParams = Academy.Instance.EnvironmentParameters;
        InitEnvParamCallbacks();
    }
    public override void OnEpisodeBegin()
    {
        foreach (Bodypart bp in bodyPartsDict.Values)
        {
            bp.Reset(bp.startingPos, bp.startingRot);
        }
        UpdateEnvVariablesOnEpisode();
        RandomiseStartPositions();
    }
    public override void CollectObservations(VectorSensor sensor)
    {
        CollectObservationGeneral(sensor);
        foreach (Bodypart bp in bodyPartsDict.Values)
        {
            CollectObservationBodyPart(sensor, bp);
        }
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
            bp.SetJointTargetRotation(targetRotX, targetRotY, targetRotZ);
            bp.SetJointStrength(jointStrength);
        }
    }
    void FixedUpdate()
    {
        UpdateEnvVariablesOnFixedUpdate();
        AddReward(CalculateReward());
    }


    /*
    * Basic functions to override or extend for more complex walker agents
    */
    public virtual void InitEnvVariables()
    {
        //init target controller
        targetController = target.GetComponent<TargetControllerBase>();
        //init walking direction transform
        GameObject walkingDirectionGoalGO = new GameObject("walkingDirectionGoal");
        walkingDirectionGoalGO.transform.parent = transform;
        walkingDirectionGoal = walkingDirectionGoalGO.transform;
    }

    public virtual void InitEnvParamCallbacks() { }

    public virtual void UpdateEnvVariablesOnEpisode()
    {
        UpdateOrientationTransform(walkingDirectionGoal, target);
    }

    public virtual void UpdateEnvVariablesOnFixedUpdate()
    {
        UpdateOrientationTransform(walkingDirectionGoal, target);
    }

    public virtual void RandomiseStartPositions()
    {
        //random start rotation to help generalize
        root.rotation = Quaternion.Euler(0, Random.Range(0.0f, 360.0f), 0);

        targetController.MoveTargetToRandomPosition();
    }

    public virtual void CollectObservationGeneral(VectorSensor sensor)
    {
        //rotation deltas
        sensor.AddObservation(Quaternion.FromToRotation(root.forward, walkingDirectionGoal.forward));
        sensor.AddObservation(Quaternion.FromToRotation(head.forward, walkingDirectionGoal.forward));

        //Position of target position relative to cube
        sensor.AddObservation(walkingDirectionGoal.InverseTransformPoint(target.transform.position));
    }

    public virtual void CollectObservationBodyPart(VectorSensor sensor, Bodypart bp)
    {
        //GROUND CHECK
        sensor.AddObservation(bp.groundContact.touchingGround); // Is this bp touching the ground

        //Get velocities in the context of our orientation cube's space
        //Note: You can get these velocities in world space as well but it may not train as well.
        sensor.AddObservation(walkingDirectionGoal.InverseTransformDirection(bp.rb.velocity));
        sensor.AddObservation(walkingDirectionGoal.InverseTransformDirection(bp.rb.angularVelocity));

        //Get position relative to hips in the context of our orientation cube's space
        sensor.AddObservation(walkingDirectionGoal.InverseTransformDirection(bp.rb.position - root.position));

        if (bp.rb.transform != root && bp.rb.transform != handL && bp.rb.transform != handR)
        {
            sensor.AddObservation(bp.rb.transform.localRotation);
            sensor.AddObservation(bp.joint.slerpDrive.maximumForce / bp.config.maxJointForceLimit);
        }
    }

    public virtual float CalculateReward()
    {
        Vector3 velocity = GetAvgVelocity();
        float walkingDirectionReward = GetAngleDeviationNonLinear(velocity, walkingDirectionGoal.forward);

        float lookDirectionReward = GetAngleDeviationNonLinear(head.forward, walkingDirectionGoal.forward);

        float reward = -1 + (walkingDirectionReward * lookDirectionReward) * 2;
        return reward;
    }


    /*
    * Helper functions to reduce duplicate code
    */
    protected void UpdateOrientationTransform(Transform directionTransform, Transform target)
    {
        var dirVector = target.position - root.position;
        dirVector.y = 0; //flatten dir on the y. this will only work on level, uneven surfaces
        var lookRot =
            dirVector == Vector3.zero
                ? Quaternion.identity
                : Quaternion.LookRotation(dirVector); //get our look rot to the target

        //update orientation transform
        directionTransform.SetPositionAndRotation(root.position, lookRot);
    }

    protected Vector3 GetAvgVelocity()
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

    protected float GetAngleDeviationNonLinear(Vector3 a, Vector3 b)
    {
        float angleDeviation = Vector3.Angle(a, b) / 180f; //Vector3.Angle is max 180 so directionDeviation is max 1
        angleDeviation = 1 - angleDeviation; //flip range so that values increases when angle decreases
        angleDeviation = Mathf.Pow(angleDeviation, 2f); //angleDeviation^2 to make it non linear
        return angleDeviation;
    }
}
