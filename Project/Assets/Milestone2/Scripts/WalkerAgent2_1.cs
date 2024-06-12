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


public class WalkerAgent2_1 : Agent
{
    /* private Direction lookDirection = Direction.Forward;
    public VectorSensorComponent goalSensor;

    public override void InitEnvParamCallbacks()
    {
        resetParams.RegisterCallback("look_direction", (float directionIndex) => lookDirection = (Direction)directionIndex);
    }

    public override void InitEnvVariables()
    {
        base.InitEnvVariables();
        goalSensor = GetComponentInChildren<VectorSensorComponent>();
    }

    public override void CollectObservationGeneral(VectorSensor sensor)
    {
        base.CollectObservationGeneral(sensor);
        goalSensor.GetSensor().AddOneHotObservation((int)lookDirection, 4);
    }

    public override float GetLookAtTargetReward()
    {
        var headForward = head.forward;
        headForward.y = 0;
        Vector3 lookDirectionVector = walkingDirectionGoal.forward;
        switch (lookDirection)
        {
            case Direction.Backward:
                lookDirectionVector = -walkingDirectionGoal.forward;
                break;
            case Direction.Right:
                lookDirectionVector = walkingDirectionGoal.right;
                break;
            case Direction.Left:
                lookDirectionVector = -walkingDirectionGoal.right;
                break;
        }
        lookDirectionVector.y = 0;
        //using look direction instead of walking direction
        return (Vector3.Dot(lookDirectionVector, headForward) + 1) * .5F;
    } */
}
