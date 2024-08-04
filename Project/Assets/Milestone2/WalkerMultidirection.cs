using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using Unity.MLAgents;
using Unity.MLAgents.Sensors;
using Unity.MLAgents.Actuators;
using Random = UnityEngine.Random;

public enum Orientation
{
    Forward,
    Right,
    Left,
    Backward,
}

public class WalkerMultidirection : WalkerAgent1
{
    [Header("Walk Orientation")]
    public Orientation orientation = Orientation.Forward;
    public bool randomizeWalkOrientation = true;
    Orientation[] orientations;

    public override void Initialize()
    {
        base.Initialize();
        orientations = (Orientation[])Enum.GetValues(typeof(Orientation));
        if (randomizeWalkOrientation)
        {
            SetRandomWalkOrientation();
        }
    }

    public override void OnEpisodeBegin()
    {
        base.OnEpisodeBegin();
        if (randomizeWalkOrientation)
        {
            SetRandomWalkOrientation();
        }
    }

    public void SetRandomWalkOrientation()
    {
        orientation = orientations[Random.Range(0, orientations.Length)];
    }

    public override void CollectObservations(VectorSensor sensor)
    {
        base.CollectObservations(sensor);
        sensor.AddObservation((float)orientation);
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
        switch (orientation)
        {
            case Orientation.Right:
                lookDirection = -walkOrientationCube.transform.right;
                break;
            case Orientation.Left:
                lookDirection = walkOrientationCube.transform.right;
                break;
            case Orientation.Backward:
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

        float totalPower = GetTotalPower();
        float powerSaveReward = -Mathf.Clamp(totalPower / 3000 - 0.05f, 0f, 1f);
        if (logStats) Debug.Log($"power save reward: {powerSaveReward}, total: {totalPower}");
        statsRecorder.Add("Reward/PowerSaveReward", powerSaveReward);

        AddReward(powerSaveReward);
    }

    float GetTotalPower()
    {
        float totalPower = 0f;
        foreach (Bodypart bp in bodyparts)
        {
            totalPower += bp.power;
        }
        return totalPower;
    }

    void OnDrawGizmos()
    {
        Gizmos.color = Color.yellow;
        if (!walkOrientationCube) return;
        Vector3 lookDirection = walkOrientationCube.transform.forward;
        switch (orientation)
        {
            case Orientation.Right:
                lookDirection = -walkOrientationCube.transform.right;
                break;
            case Orientation.Left:
                lookDirection = walkOrientationCube.transform.right;
                break;
            case Orientation.Backward:
                lookDirection = -walkOrientationCube.transform.forward;
                break;
        }
        Gizmos.DrawRay(head.position, lookDirection);
    }
}
