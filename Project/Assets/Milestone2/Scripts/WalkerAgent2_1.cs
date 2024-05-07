using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.MLAgents;
using Unity.MLAgents.Actuators;
using Unity.MLAgents.Sensors;

enum Direction
{
    Forward,
    Right,
    Backward,
    Left
}


public class WalkerAgent2_1 : WalkerAgentSimple
{
    private Direction lookDirection;
    public VectorSensorComponent goalSensor;

    public override void Initialize()
    {
        base.Initialize();
        goalSensor = GetComponentInChildren<VectorSensorComponent>();
        lookDirection = (Direction)resetParams.GetWithDefault("look_direction", 0);
    }

    public override void OnEpisodeBegin()
    {
        base.OnEpisodeBegin();
        lookDirection = (Direction)resetParams.GetWithDefault("look_direction", 0);
    }

    public override void CollectObservations(VectorSensor sensor)
    {
        base.CollectObservations(sensor);
        goalSensor.GetSensor().AddOneHotObservation((int)lookDirection, 4);
    }

    public override float GetLookAtTargetReward()
    {
        var headForward = head.forward;
        headForward.y = 0;
        Vector3 lookDirectionVector = walkingDirection.forward;
        switch (lookDirection)
        {
            case Direction.Backward:
                lookDirectionVector = -walkingDirection.forward;
                break;
            case Direction.Right:
                lookDirectionVector = walkingDirection.right;
                break;
            case Direction.Left:
                lookDirectionVector = -walkingDirection.right;
                break;
        }
        lookDirectionVector.y = 0;
        //using look direction instead of walking direction
        return (Vector3.Dot(lookDirectionVector, headForward) + 1) * .5F;
    }
}
