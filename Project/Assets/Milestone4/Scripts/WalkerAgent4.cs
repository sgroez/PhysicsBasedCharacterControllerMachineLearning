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
    [Space(10)]
    public ReferenceController referenceController;

    [Header("End Effectors")]
    [Space(10)]
    public Transform handL;
    public Transform handR;
    public Transform footL;
    public Transform footR;

    public override void UpdateEnvVariablesOnEpisode()
    {
        UpdateOrientationTransform(walkingDirectionGoal, targetController.transform);
        //removed random walking speed
    }

    public override void UpdateEnvVariablesOnFixedUpdate()
    {
        base.UpdateEnvVariablesOnFixedUpdate();
        Vector3 avgRefVelocity = referenceController.avgVelocity;
        float avgRefVelocityToTarget = avgRefVelocity.z;
        walkingSpeed = avgRefVelocityToTarget > 0 ? avgRefVelocityToTarget : 0.1f;
        statsRecorder.Add("Environment/WalkingSpeed", walkingSpeed);
    }

    public override void InitEnvParamCallbacks()
    {
        //remove walking speed env parameter callback
    }

    public override void RandomiseStartPositions()
    {
        //init reference animation at random point
        referenceController.ResetReference();
        //reste bodypart position and then set it to the start pose from the reference character
        int i = 0;
        foreach (Bodypart bp in bodyParts)
        {
            StartCoroutine(DelayResetBodypart(bp, referenceController.referenceBodyparts[i].transform));
            i++;
        }
    }

    /*
    * implemement observations (state) like defined in paper
    * https://xbpeng.github.io/projects/DeepMimic/2018_TOG_DeepMimic.pdf
    */
    public override void CollectObservationGeneral(VectorSensor sensor)
    {
        base.CollectObservationGeneral(sensor);
        //add phase variable to observation
        sensor.AddObservation(referenceController.GetCurrentPhase());
    }

    /*
    * combine rewards like defined in paper
    * https://xbpeng.github.io/projects/DeepMimic/2018_TOG_DeepMimic.pdf
    */
    public override float CalculateReward()
    {
        //get goal reward (matchWalkingSpeed and LookAtTarget Reward)
        float goalRewardWeight = 0.6f;
        float goalReward = base.CalculateReward();

        //calculate imitation reward
        float poseRewardWeight = .65f;
        float velocityRewardWeight = .1f;
        float endEffectorRewardWeight = .15f;
        float centerOfMassRewardWeight = .1f;
        float poseReward = CalculatePoseReward();
        float velocityReward = CalculateVelocityReward();
        float endEffectorReward = CalculateEndEffectorReward();
        float centerOfMassReward = CalculateCenterOfMassReward();
        float imitationReward = poseRewardWeight * poseReward + velocityRewardWeight * velocityReward + endEffectorRewardWeight * endEffectorReward + centerOfMassRewardWeight * centerOfMassReward;
        float imitationRewardWeight = 0.4f;

        //combine rewards
        return imitationRewardWeight * imitationReward + goalRewardWeight * goalReward;
    }

    IEnumerator DelayResetBodypart(Bodypart bp, Transform referenceBone)
    {
        yield return new WaitForSeconds(.001f); // Wait for .001 second
        // Code to be executed after the delay
        bp.Reset(referenceBone.position, referenceBone.rotation);
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
        foreach (Bodypart bp in bodyParts)
        {
            //Quaternion difference
            Quaternion difference = referenceController.referenceBodyparts[i].transform.localRotation * Quaternion.Inverse(bp.rb.transform.localRotation);
            //Scalar of difference
            float scalarRotationRadians = 2f * Mathf.Acos(Mathf.Clamp(Mathf.Abs(difference.w), -1f, 1f));
            float scalarRotationRadiansSquared = Mathf.Pow(scalarRotationRadians, 2f);
            sum += scalarRotationRadiansSquared;
            i++;
        }
        float poseReward = Mathf.Exp(-2 * sum);
        statsRecorder.Add("Environment/PoseReward", poseReward);
        return poseReward;
    }
    private float CalculateVelocityReward()
    {
        int i = 0;
        float sum = 0f;
        //sum over all bodyparts
        foreach (Bodypart bp in bodyParts)
        {
            ReferenceBodypart referenceBodypart = referenceController.referenceBodyparts[i];
            Vector3 difference = referenceBodypart.angularVelocity - bp.rb.angularVelocity;
            float differenceMagnitude = difference.magnitude;
            float differenceMagnitudeSquared = Mathf.Pow(differenceMagnitude, 2f);
            sum += differenceMagnitudeSquared;
            i++;
        }
        float velocityReward = Mathf.Exp(-.1f * sum);
        statsRecorder.Add("Environment/VelocityReward", velocityReward);
        return velocityReward;
    }

    private float CalculateEndEffectorReward()
    {
        int i = 0;
        float sum = 0f;
        Transform[] endEffectors = { handL, handR, footL, footR };
        //sum over all bodyparts
        foreach (Bodypart bp in bodyParts)
        {
            bool isEndEffector = Array.IndexOf(endEffectors, bp.rb.transform) != -1;
            if (isEndEffector)
            {
                ReferenceBodypart referenceBodypart = referenceController.referenceBodyparts[i];
                Vector3 difference = referenceBodypart.transform.position - bp.rb.transform.position;
                float differenceMagnitude = difference.magnitude;
                float differenceMagnitudeSquared = Mathf.Pow(differenceMagnitude, 2f);
                sum += differenceMagnitudeSquared;
            }
            i++;
        }
        float endEffectorReward = Mathf.Exp(-40 * sum);
        statsRecorder.Add("Environment/EndEffectorReward", endEffectorReward);
        return endEffectorReward;
    }

    //TODO change to use real center of mass instead of hips
    private float CalculateCenterOfMassReward()
    {
        Vector3 difference = referenceController.CalculateCenterOfMass() - CalculateCenterOfMass();
        float differenceMagnitude = difference.magnitude;
        float differenceMagnitudeSquared = Mathf.Pow(differenceMagnitude, 2f);
        float centerOfMassReward = Mathf.Exp(-10 * differenceMagnitudeSquared);
        statsRecorder.Add("Environment/CenterOfMassReward", centerOfMassReward);
        return centerOfMassReward;
    }

    private Vector3 CalculateCenterOfMass()
    {
        Vector3 centerOfMass = Vector3.zero;
        float totalMass = 0f;
        foreach (Bodypart bp in bodyParts)
        {
            centerOfMass += bp.rb.worldCenterOfMass * bp.rb.mass;
            totalMass += bp.rb.mass;
        }
        if (totalMass > 0f)
        {
            centerOfMass /= totalMass; // Normalize by total mass
        }
        return centerOfMass;
    }
}