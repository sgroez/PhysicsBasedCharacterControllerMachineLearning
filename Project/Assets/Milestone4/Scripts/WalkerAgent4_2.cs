using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.MLAgents;
using Unity.MLAgents.Actuators;
using Unity.MLAgents.Sensors;
using Random = UnityEngine.Random;

public class WalkerAgent4_2 : WalkerAgentBase
{
    [Header("Reference Controller To Match Reference Motion From")]
    [Space(10)]
    public ReferenceController referenceController;

    [Header("Missing End Effectors")]
    [Space(10)]
    public Transform footL;
    public Transform footR;

    public override void InitEnvVariables() { }

    public override void UpdateEnvVariablesOnEpisode() { }

    public override void UpdateEnvVariablesOnFixedUpdate() { }

    public override void RandomiseStartPositions()
    {
        //init reference animation at random point
        referenceController.ResetReference();
        //reste bodypart position and then set it to the start pose from the reference character
        int i = 0;
        foreach (Bodypart bp in bodyPartsDict.Values)
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
        //add phase variable to observation
        sensor.AddObservation(referenceController.GetCurrentPhase());
    }

    public override void CollectObservationBodyPart(VectorSensor sensor, Bodypart bp)
    {
        //Get position relative to hips
        if (bp.rb.transform != root)
        {
            sensor.AddObservation(root.InverseTransformPoint(bp.rb.transform.position));
            sensor.AddObservation(Quaternion.Inverse(root.rotation) * bp.rb.transform.rotation);
        }
        else
        {
            //add root pos and rot
            sensor.AddObservation(root.position);
            sensor.AddObservation(root.rotation);
        }
        //Get velocities in the context of the root transform (hips)
        sensor.AddObservation(root.InverseTransformDirection(bp.rb.velocity));
        sensor.AddObservation(root.InverseTransformDirection(bp.rb.angularVelocity));
    }

    /*
    * combine rewards like defined in paper
    * https://xbpeng.github.io/projects/DeepMimic/2018_TOG_DeepMimic.pdf
    */
    public override float CalculateReward()
    {
        float poseRewardWeight = .65f;
        float velocityRewardWeight = .1f;
        float endEffectorRewardWeight = .15f;
        float centerOfMassRewardWeight = .1f;
        float poseReward = CalculatePoseReward();
        float velocityReward = CalculateVelocityReward();
        float endEffectorReward = CalculateEndEffectorReward();
        float centerOfMassReward = CalculateCenterOfMassReward();
        return poseRewardWeight * poseReward + velocityRewardWeight * velocityReward + endEffectorRewardWeight * endEffectorReward + centerOfMassRewardWeight * centerOfMassReward;
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
        foreach (Bodypart bp in bodyPartsDict.Values)
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
        //Debug.Log("poseReward: " + poseReward);
        return poseReward;
    }
    private float CalculateVelocityReward()
    {
        int i = 0;
        float sum = 0f;
        //sum over all bodyparts
        foreach (Bodypart bp in bodyPartsDict.Values)
        {
            ReferenceBodypart referenceBodypart = referenceController.referenceBodyparts[i];
            referenceBodypart.GetVelocities(out Vector3 _, out Vector3 referenceAngularVelocity);
            Vector3 difference = referenceAngularVelocity - bp.rb.angularVelocity;
            float differenceMagnitude = difference.magnitude;
            float differenceMagnitudeSquared = Mathf.Pow(differenceMagnitude, 2f);
            sum += differenceMagnitudeSquared;
            i++;
        }
        float velocityReward = Mathf.Exp(-.1f * sum);
        //Debug.Log("velocityReward: " + velocityReward);
        return velocityReward;
    }

    private float CalculateEndEffectorReward()
    {
        int i = 0;
        float sum = 0f;
        Transform[] endEffectors = { handL, handR, footL, footR };
        //sum over all bodyparts
        foreach (Bodypart bp in bodyPartsDict.Values)
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
        //Debug.Log("endEffectorReward: " + endEffectorReward);
        return endEffectorReward;
    }

    private float CalculateCenterOfMassReward()
    {
        Bodypart bp = bodyPartsDict[root];
        ReferenceBodypart referenceBodypart = referenceController.referenceBodyparts[0];
        Vector3 difference = referenceBodypart.transform.position - bp.rb.transform.position;
        float differenceMagnitude = difference.magnitude;
        float differenceMagnitudeSquared = Mathf.Pow(differenceMagnitude, 2f);
        float centerOfMassReward = Mathf.Exp(-10 * differenceMagnitudeSquared);
        //Debug.Log("centerOfMassReward: " + centerOfMassReward);
        return centerOfMassReward;
    }
}