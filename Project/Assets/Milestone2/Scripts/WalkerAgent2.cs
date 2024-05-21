using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.MLAgents;
using Unity.MLAgents.Actuators;
using Unity.MLAgents.Sensors;

public class WalkerAgent2 : WalkerAgent1
{
    [Header("Target To Look At")]
    [Space(10)]
    public TargetControllerBase lookAtTargetController;
    private Transform lookDirectionGoal;

    public override void InitEnvVariables()
    {
        base.InitEnvVariables();
        //init look at target transform
        GameObject lookDirectionGoalGO = new GameObject("lookDirectionGoal");
        lookDirectionGoalGO.transform.parent = transform;
        lookDirectionGoal = lookDirectionGoalGO.transform;

        //set lookAtTargetController Agent
        lookAtTargetController.agent = this;
        lookAtTargetController.spawnAngle = RotationRandAngle;
    }
    public override void UpdateEnvVariablesOnEpisode()
    {
        base.UpdateEnvVariablesOnEpisode();
        UpdateOrientationTransform(lookDirectionGoal, lookAtTargetController.transform);
    }

    public override void UpdateEnvVariablesOnFixedUpdate()
    {
        base.UpdateEnvVariablesOnFixedUpdate();
        UpdateOrientationTransform(lookDirectionGoal, lookAtTargetController.transform);
    }

    public override void RandomiseStartPositions()
    {
        base.RandomiseStartPositions();
        lookAtTargetController.MoveTargetToRandomPosition();
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

        //observe rotation deltas relative to look direction instead of walking direction
        sensor.AddObservation(Quaternion.FromToRotation(root.forward, lookDirectionGoal.forward));
        sensor.AddObservation(Quaternion.FromToRotation(head.forward, lookDirectionGoal.forward));

        //Position of target position relative to cube
        sensor.AddObservation(walkingDirectionGoal.InverseTransformPoint(targetController.transform.position));
    }

    public override float GetLookAtTargetReward()
    {
        var headForward = head.forward;
        headForward.y = 0;
        //using look direction instead of walking direction
        float lookAtTargetReward = (Vector3.Dot(lookDirectionGoal.forward, headForward) + 1) * .5F;
        statsRecorder.Add("Environment/LookAtTargetReward", lookAtTargetReward);
        return lookAtTargetReward;
    }
}
