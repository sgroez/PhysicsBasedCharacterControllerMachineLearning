using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using Unity.MLAgents;
using Unity.MLAgents.Sensors;
using Unity.MLAgents.Actuators;
using Random = UnityEngine.Random;

public enum Direction
{
    Forward,
    Right,
    Left,
    Backward,
}

public class WalkerMultidirection : WalkerAgent1
{
    public Direction direction = Direction.Forward;
    Direction[] directions;

    public override void Initialize()
    {
        base.Initialize();
        directions = (Direction[])Enum.GetValues(typeof(Direction));
        SetRandomWalkDirection();
    }

    public override void OnEpisodeBegin()
    {
        base.OnEpisodeBegin();
        SetRandomWalkDirection();
    }

    public void SetRandomWalkDirection()
    {
        direction = directions[Random.Range(0, directions.Length)];
    }

    public override void CollectObservations(VectorSensor sensor)
    {
        base.CollectObservations(sensor);
        sensor.AddObservation((float)direction);
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
        Vector3 lookDirection = cubeForward;
        switch (direction)
        {
            case Direction.Right:
                lookDirection = walkOrientationCube.transform.right;
                break;
            case Direction.Left:
                lookDirection = -walkOrientationCube.transform.right;
                break;
            case Direction.Backward:
                lookDirection = -walkOrientationCube.transform.forward;
                break;
        }
        var lookAtTargetReward = (Vector3.Dot(lookDirection, headForward) + 1) * .5F;
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

        AddReward(matchSpeedReward * lookAtTargetReward);
    }

    void OnDrawGizmos()
    {
        Gizmos.color = Color.yellow;
        if (!walkOrientationCube) return;
        Vector3 lookDirection = walkOrientationCube.transform.forward;
        switch (direction)
        {
            case Direction.Right:
                lookDirection = walkOrientationCube.transform.right;
                break;
            case Direction.Left:
                lookDirection = -walkOrientationCube.transform.right;
                break;
            case Direction.Backward:
                lookDirection = -walkOrientationCube.transform.forward;
                break;
        }
        Gizmos.DrawRay(head.position, lookDirection);
    }
}
