using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.MLAgents;
using Unity.MLAgents.Actuators;
using Unity.MLAgents.Sensors;
using Random = UnityEngine.Random;

public class WalkerAgent1 : WalkerAgentBase
{
    [Header("Head to match look direction")]
    [Space(10)]
    public Transform head;

    [Header("Target To Walk Towards")]
    [Space(10)]
    public TargetControllerBase targetController;
    protected Transform walkingDirectionGoal;

    [Header("Walking Speed")]
    [Space(10)]
    public bool randomizeWalkingSpeed = false;
    public float maxWalkingSpeed;
    protected float walkingSpeed = 0.1f;

    /*
    * Environment stats
    */
    private Vector3 previousPos;
    private float distanceMovedInTargetDirection;
    [HideInInspector] public int reachedTargets;

    public override void InitEnvVariables()
    {
        //init walking direction transform
        GameObject walkingDirectionGoalGO = new GameObject("walkingDirectionGoal");
        walkingDirectionGoalGO.transform.parent = transform;
        walkingDirectionGoal = walkingDirectionGoalGO.transform;

        //set targetController Agent
        targetController.agent = this;
        targetController.spawnAngle = RotationRandAngle;
    }

    public override void InitEnvParamCallbacks()
    {
        resetParams.RegisterCallback("maxWalkingSpeed", (float maxWalkingSpeed) => this.maxWalkingSpeed = maxWalkingSpeed);
    }

    public override void UpdateEnvVariablesOnEpisode()
    {
        UpdateOrientationTransform(walkingDirectionGoal, targetController.transform);
        if (randomizeWalkingSpeed)
        {
            walkingSpeed = Random.Range(0.1f, maxWalkingSpeed);
        }
        else
        {
            walkingSpeed = maxWalkingSpeed;
        }
        //record walking speed stats
        statsRecorder.Add("Environment/WalkingSpeed", walkingSpeed);
        //record then reset distance moved in target direction
        statsRecorder.Add("Environment/DistanceMovedInTargetDirection", distanceMovedInTargetDirection);
        distanceMovedInTargetDirection = 0f;
        previousPos = root.position;
        //record then reset targets reached
        statsRecorder.Add("Environment/ReachedTargets", reachedTargets);
        reachedTargets = 0;
    }

    public override void UpdateEnvVariablesOnFixedUpdate()
    {
        UpdateOrientationTransform(walkingDirectionGoal, targetController.transform);
        distanceMovedInTargetDirection += GetDistanceMovedInTargetDirection();
    }

    public override void RandomiseStartPositions()
    {
        base.RandomiseStartPositions();
        targetController.MoveTargetToRandomPosition();
    }

    public override void CollectObservationGeneral(VectorSensor sensor)
    {
        //velocity we want to match
        var velGoal = walkingDirectionGoal.forward * walkingSpeed;
        //ragdoll's avg vel
        var avgVel = GetAvgVelocity();

        //current ragdoll velocity. normalized
        sensor.AddObservation(Vector3.Distance(velGoal, avgVel));
        //avg body vel relative to cube
        sensor.AddObservation(walkingDirectionGoal.InverseTransformDirection(avgVel));
        //vel goal relative to cube
        sensor.AddObservation(walkingDirectionGoal.InverseTransformDirection(velGoal));

        //rotation deltas
        sensor.AddObservation(Quaternion.FromToRotation(root.forward, walkingDirectionGoal.forward));
        sensor.AddObservation(Quaternion.FromToRotation(head.forward, walkingDirectionGoal.forward));

        //position of target position relative to cube
        sensor.AddObservation(walkingDirectionGoal.InverseTransformPoint(targetController.transform.position));
    }

    public override void CollectObservationBodyPart(VectorSensor sensor, Bodypart bp)
    {
        //ground check
        sensor.AddObservation(bp.groundContact.touchingGround); // Is this bp touching the ground

        //get velocities in the context of our orientation cube's space
        //note: You can get these velocities in world space as well but it may not train as well.
        sensor.AddObservation(walkingDirectionGoal.InverseTransformDirection(bp.rb.velocity));
        sensor.AddObservation(walkingDirectionGoal.InverseTransformDirection(bp.rb.angularVelocity));

        //get position relative to hips in the context of our orientation cube's space
        sensor.AddObservation(walkingDirectionGoal.InverseTransformDirection(bp.rb.position - root.position));

        if (bp.rb.transform != root && bp.dof.sqrMagnitude > 0)
        {
            sensor.AddObservation(bp.rb.transform.localRotation);
            sensor.AddObservation(bp.joint.slerpDrive.maximumForce / bp.config.maxJointForceLimit);
        }
    }

    public override float CalculateReward()
    {
        // set reward for this step according to mixture of the following elements.
        // a. Match target speed
        //this reward will approach 1 if it matches perfectly and approach zero as it deviates
        var matchSpeedReward = GetMatchingVelocityReward();

        //Check for NaNs
        if (float.IsNaN(matchSpeedReward))
        {
            throw new ArgumentException(
                "NaN in moveTowardsTargetReward.\n" +
                $" walkingDirectionGoal: {walkingDirectionGoal.forward}\n" +
                $" hips.velocity: {bodyParts[0].rb.velocity}\n" +
                $" maximumWalkingSpeed: {maxWalkingSpeed}\n" +
                $" walkingSpeed: {walkingSpeed}"
            );
        }

        // b. Rotation alignment with target direction.
        //this reward will approach 1 if it faces the target direction perfectly and approach zero as it deviates
        var lookAtTargetReward = GetLookAtTargetReward();

        //check for NaNs
        if (float.IsNaN(lookAtTargetReward))
        {
            throw new ArgumentException(
                "NaN in lookAtTargetReward.\n" +
                $" walkingDirectionGoal: {walkingDirectionGoal.forward}\n" +
                $" head.forward: {head.forward}"
            );
        }

        return matchSpeedReward * lookAtTargetReward;
    }

    public virtual float GetMatchingVelocityReward()
    {
        Vector3 velocityGoal = walkingDirectionGoal.forward * walkingSpeed;
        Vector3 actualVelocity = GetAvgVelocity();
        //distance between our actual velocity and goal velocity
        var velDeltaMagnitude = Mathf.Clamp(Vector3.Distance(actualVelocity, velocityGoal), 0, walkingSpeed);

        //return the value on a declining sigmoid shaped curve that decays from 1 to 0
        //This reward will approach 1 if it matches perfectly and approach zero as it deviates
        float matchingVelocityReward = Mathf.Pow(1 - Mathf.Pow(velDeltaMagnitude / walkingSpeed, 2), 2);
        statsRecorder.Add("Reward/MatchingVelocityReward", matchingVelocityReward);
        return matchingVelocityReward;
    }

    public virtual float GetLookAtTargetReward()
    {
        var headForward = head.forward;
        headForward.y = 0;
        float lookAtTargetReward = (Vector3.Dot(walkingDirectionGoal.forward, headForward) + 1) * .5F;
        statsRecorder.Add("Reward/LookAtTargetReward", lookAtTargetReward);
        return lookAtTargetReward;
    }

    private float GetDistanceMovedInTargetDirection()
    {
        //calculate the displacement vector
        Vector3 currentPos = root.position;
        Vector3 displacement = currentPos - previousPos;

        //project the displacement vector onto the goal direction vector
        float movementInTargetDirection = Vector3.Dot(displacement, walkingDirectionGoal.forward);

        //update the previous position for the next frame
        previousPos = currentPos;
        return movementInTargetDirection;
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

        foreach (Bodypart bp in bodyParts)
        {
            velSum += bp.rb.velocity;
        }

        var avgVel = velSum / bodyParts.Count;
        return avgVel;
    }
}