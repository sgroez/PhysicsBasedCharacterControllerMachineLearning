using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.MLAgents;
using Unity.MLAgents.Actuators;
using Unity.MLAgents.Sensors;
using Random = UnityEngine.Random;

public class WalkerAgent4 : WalkerAgent1
{
    [Header("Reference Controller To Match Reference Motion From")]
    public ReferenceController referenceController;

    public override void OnEpisodeBegin()
    {
        //Reset all of the body parts
        foreach (Bodypart bp in bodyparts)
        {
            bp.ResetTransform();
        }

        //Set our goal walking speed
        targetWalkingSpeed =
            randomizeWalkSpeedEachEpisode ? Random.Range(minWalkingSpeed, maxWalkingSpeed) : targetWalkingSpeed;

        //init reference animation at random point
        referenceController.ResetReference();

        //reset bodypart position and then set it to the start pose from the reference character
        //StartCoroutine(ResetBodypartsOnNextFrame());

        //Random start rotation to help generalize
        if (randomizeRotationOnEpsiode)
        {
            root.rotation = Quaternion.Euler(0, Random.Range(0.0f, 360.0f), 0);
        }

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

    IEnumerator ResetBodypartsOnNextFrame()
    {
        //wait for the next frame
        yield return null;

        // Code to be executed on next frame
        int i = 0;
        foreach (Bodypart bp in bodyparts)
        {
            Transform referenceBone = referenceController.referenceBodyparts[i].transform;
            bp.ResetTransform(referenceBone.position, referenceBone.rotation);
            i++;
        }
    }

    public override void CollectObservations(VectorSensor sensor)
    {
        base.CollectObservations(sensor);
        //add phase to relate movement to relative time
        sensor.AddObservation(referenceController.GetCurrentPhase());
    }

    /*
    * implemement rewards like defined in paper
    * https://xbpeng.github.io/projects/DeepMimic/2018_TOG_DeepMimic.pdf
    */
    private float CalculatePoseReward()
    {
        int i = 0;
        float sum = 0f;
        //sum over all bodyparts
        foreach (Bodypart bp in bodyparts)
        {
            float angle = Quaternion.Angle(referenceController.referenceBodyparts[i].transform.localRotation, bp.rb.transform.localRotation);
            sum += angle;
            i++;
        }
        float avg = sum / i;
        float poseReward = -(avg / 180f);
        return poseReward;
    }

    public override void FixedUpdate()
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

        //calculate imitation rewards
        float poseReward = CalculatePoseReward();
        RecordStat("Reward/PoseReward", poseReward);

        //Check for NaNs
        if (float.IsNaN(poseReward)) throw new ArgumentException("NaN in poseReward.");

        float demoReward = matchSpeedReward * lookAtTargetReward;
        AddReward(demoReward + poseReward);
    }
}