using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.MLAgents;
using Unity.MLAgents.Actuators;
using Unity.MLAgents.Sensors;

public class WalkerAgentSimple2 : WalkerAgentSimple
{
    public Transform lookAtTarget;
    private Transform lookDirection;

    public override void InitOrientationGoals()
    {
        base.InitOrientationGoals();
        lookDirection = InitDirectionTransform("lookDirection");
    }

    public override void UpdateOrientationGoals()
    {
        base.UpdateOrientationGoals();
        UpdateDirectionTransform(lookDirection, lookAtTarget);
    }

    public override float GetLookAtTargetReward()
    {
        var headForward = head.forward;
        headForward.y = 0;
        //using look direction instead of walking direction
        return (Vector3.Dot(lookDirection.forward, headForward) + 1) * .5F;
    }

    public override void OnEpisodeBegin()
    {
        base.OnEpisodeBegin();
        lookAtTarget.GetComponent<TargetControllerSimple>().MoveTargetToRandomPosition();
    }

    public override void CollectObservations(VectorSensor sensor)
    {
        base.CollectObservations(sensor);
        //add observation changes here
    }

    public override void CollectRotationDeltas(VectorSensor sensor)
    {
        //observe rotation deltas relative to look direction instead of walking direction
        sensor.AddObservation(Quaternion.FromToRotation(root.forward, lookDirection.forward));
        sensor.AddObservation(Quaternion.FromToRotation(head.forward, lookDirection.forward));
    }
}
