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
    [Header("Walking Speed")]
    [Space(10)]
    public float maxWalkingSpeed;
    protected float walkingSpeed;

    public override void InitEnvParamCallbacks()
    {
        resetParams.RegisterCallback("maxWalkingSpeed", (float maxWalkingSpeed) => this.maxWalkingSpeed = maxWalkingSpeed);
    }

    public override void UpdateEnvVariablesOnEpisode()
    {
        walkingSpeed = Random.Range(0.1f, maxWalkingSpeed);
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

        //Position of target position relative to cube
        sensor.AddObservation(walkingDirectionGoal.InverseTransformPoint(target.transform.position));
    }

    public override void CollectObservationBodyPart(VectorSensor sensor, Bodypart bp)
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

    public override float CalculateReward()
    {
        // Set reward for this step according to mixture of the following elements.
        // a. Match target speed
        //This reward will approach 1 if it matches perfectly and approach zero as it deviates
        var matchSpeedReward = GetMatchingVelocityReward();

        //Check for NaNs
        if (float.IsNaN(matchSpeedReward))
        {
            throw new ArgumentException(
                "NaN in moveTowardsTargetReward.\n" +
                $" walkingDirectionGoal: {walkingDirectionGoal.forward}\n" +
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
        return Mathf.Pow(1 - Mathf.Pow(velDeltaMagnitude / walkingSpeed, 2), 2);
    }

    public virtual float GetLookAtTargetReward()
    {
        var headForward = head.forward;
        headForward.y = 0;
        return (Vector3.Dot(walkingDirectionGoal.forward, headForward) + 1) * .5F;
    }
}