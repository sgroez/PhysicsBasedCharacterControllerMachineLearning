using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.MLAgents;
using Unity.MLAgents.Actuators;
using Unity.MLAgents.Sensors;
using Random = UnityEngine.Random;

public class WalkerAgent4 : MonoBehaviour
{
    /* [Header("Reference Controller To Match Reference Motion From")]
    public ReferenceController referenceController;

    [Header("End Effectors")]
    public Transform handL;
    public Transform handR;
    public Transform footL;
    public Transform footR;

    private Vector3 previousPos;
    private float distanceMovedInTargetDirection;

    public override void UpdateEnvVariablesOnEpisode()
    {
        //record then reset distance moved in target direction
        statsRecorder.Add("Environment/DistanceMovedInTargetDirection", distanceMovedInTargetDirection);
        distanceMovedInTargetDirection = 0f;
        previousPos = root.position;
    }

    public override void UpdateEnvVariablesOnFixedUpdate()
    {
        distanceMovedInTargetDirection += GetDistanceMovedInTargetDirection();
    }

    public override void RandomiseStartPositions()
    {
        //init reference animation at random point
        referenceController.ResetReference();
        //reset bodypart position and then set it to the start pose from the reference character
        StartCoroutine(ResetBodypartsOnNextFrame());
    } */

    /*
    * implemement observations (state) like defined in paper
    * https://xbpeng.github.io/projects/DeepMimic/2018_TOG_DeepMimic.pdf
    */
    /*  public override void CollectObservationGeneral(VectorSensor sensor)
     {
         for (int i = 0; i < bodyparts.Count; i++)
         {
             Bodypart bp = bodyparts[i];
             ReferenceBodypart rbp = referenceController.referenceBodyparts[i];
             CollectBodypartObservationRelativeToReference(sensor, bp, rbp);
         }
     } */

    /* private void CollectReferenceBodypartObservation(VectorSensor sensor, ReferenceBodypart rbp)
    {
        //ground check
        sensor.AddObservation(rbp.touchingGround); // Is this rbp touching the ground

        sensor.AddObservation(rbp.velocity);
        sensor.AddObservation(rbp.angularVelocity);

        //get position relative to hips
        sensor.AddObservation(rbp.transform.position - referenceController.referenceRoot.position);

        if (rbp.transform != referenceController.referenceRoot)
        {
            sensor.AddObservation(rbp.transform.localRotation);
        }
    } */

    /* private void CollectBodypartObservationRelativeToReference(VectorSensor sensor, Bodypart bp, ReferenceBodypart rbp)
    {
        //ground check
        sensor.AddObservation(bp.touchingGround); // Is this bp touching the ground

        //get velocities relative to reference
        sensor.AddObservation(bp.rb.velocity - rbp.velocity);
        sensor.AddObservation(bp.rb.angularVelocity - rbp.angularVelocity);

        //get position relative to reference
        sensor.AddObservation(bp.rb.position - rbp.transform.position);

        if (bp.rb.transform != root)
        {
            //get local rotation relative to reference
            sensor.AddObservation(Quaternion.Inverse(rbp.transform.localRotation) * bp.rb.transform.localRotation);
            sensor.AddObservation(bp.joint.slerpDrive.maximumForce / bp.physicsConfig.maxJointForceLimit);
        }
    } */

    /*
    * combine rewards like defined in paper
    * https://xbpeng.github.io/projects/DeepMimic/2018_TOG_DeepMimic.pdf
    */
    /* public override float CalculateReward()
    {
        //set imitation reward weights
        float poseRewardWeight = .65f;
        float angularVelocityRewardWeight = .1f;
        float endEffectorRewardWeight = .15f;
        float centerOfMassRewardWeight = .1f;

        //calculate imitation rewards
        float poseReward = CalculatePoseReward();
        float angularVelocityReward = CalculateAngularVelocityReward();
        float endEffectorReward = CalculateEndEffectorReward();
        float centerOfMassReward = CalculateCenterOfMassReward();

        //Check for NaNs
        if (float.IsNaN(poseReward)) throw new ArgumentException("NaN in poseReward.");
        if (float.IsNaN(angularVelocityReward)) throw new ArgumentException("NaN in angularVelocityReward.");
        if (float.IsNaN(endEffectorReward)) throw new ArgumentException("NaN in endEffectorReward.");
        if (float.IsNaN(centerOfMassReward)) throw new ArgumentException("NaN in centerOfMassReward.");

        float imitationReward = poseRewardWeight * poseReward + angularVelocityRewardWeight * angularVelocityReward + endEffectorRewardWeight * endEffectorReward + centerOfMassRewardWeight * centerOfMassReward;

        return imitationReward;
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
            bp.Reset(referenceBone.position, referenceBone.rotation);
            i++;
        }
    } */

    /*
    * implemement rewards like defined in paper
    * https://xbpeng.github.io/projects/DeepMimic/2018_TOG_DeepMimic.pdf
    */
    /* private float CalculatePoseReward()
    {
        int i = 0;
        float sum = 0f;
        //sum over all bodyparts
        foreach (Bodypart bp in bodyparts)
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
        statsRecorder.Add("Reward/PoseReward", poseReward);
        return poseReward;
    }
    private float CalculateAngularVelocityReward()
    {
        int i = 0;
        float sum = 0f;
        //sum over all bodyparts
        foreach (Bodypart bp in bodyparts)
        {
            ReferenceBodypart referenceBodypart = referenceController.referenceBodyparts[i];
            Vector3 difference = referenceBodypart.angularVelocity - bp.rb.angularVelocity;
            float differenceMagnitude = difference.magnitude;
            float differenceMagnitudeSquared = Mathf.Pow(differenceMagnitude, 2f);
            sum += differenceMagnitudeSquared;
            i++;
        }
        float angularVelocityReward = Mathf.Exp(-.1f * sum);
        statsRecorder.Add("Reward/AngularVelocityReward", angularVelocityReward);
        return angularVelocityReward;
    }

    private float CalculateEndEffectorReward()
    {
        int i = 0;
        float sum = 0f;
        Transform[] endEffectors = { handL, handR, footL, footR };
        //sum over all bodyparts
        foreach (Bodypart bp in bodyparts)
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
        statsRecorder.Add("Reward/EndEffectorReward", endEffectorReward);
        return endEffectorReward;
    }

    //TODO change to use real center of mass instead of hips
    private float CalculateCenterOfMassReward()
    {
        Vector3 difference = referenceController.CalculateCenterOfMass() - CalculateCenterOfMass();
        float differenceMagnitude = difference.magnitude;
        float differenceMagnitudeSquared = Mathf.Pow(differenceMagnitude, 2f);
        float centerOfMassReward = Mathf.Exp(-10 * differenceMagnitudeSquared);
        statsRecorder.Add("Reward/CenterOfMassReward", centerOfMassReward);
        return centerOfMassReward;
    }

    private Vector3 CalculateCenterOfMass()
    {
        Vector3 centerOfMass = Vector3.zero;
        float totalMass = 0f;
        foreach (Bodypart bp in bodyparts)
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

    public override void OnActionReceived(ActionBuffers actionBuffers)
    {
        int index = -1;
        var continuousActions = actionBuffers.ContinuousActions;

        for (int i = 0; i < bodyparts.Count; i++)
        {
            Bodypart bp = bodyparts[i];
            ReferenceBodypart rbp = referenceController.referenceBodyparts[i];
            if (bp.rb.transform == root) continue;
            float jointStrength = bp.dof.sqrMagnitude > 0 ? continuousActions[++index] : 0;
            ConfigurableJointExtensions.SetTargetRotationLocal(bp.joint, rbp.transform.localRotation, bp.startingRotLocal);
            bp.SetJointStrength(jointStrength);
        }
    }

    private float GetDistanceMovedInTargetDirection()
    {
        //calculate the displacement vector
        Vector3 currentPos = root.position;
        Vector3 displacement = currentPos - previousPos;

        //project the displacement vector onto the goal direction vector
        float movementInTargetDirection = displacement.z;

        //update the previous position for the next frame
        previousPos = currentPos;
        return movementInTargetDirection;
    } */
}