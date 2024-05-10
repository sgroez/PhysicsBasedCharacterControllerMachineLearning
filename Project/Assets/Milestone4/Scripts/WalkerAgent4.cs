using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.MLAgents;
using Unity.MLAgents.Actuators;
using Unity.MLAgents.Sensors;

public class WalkerAgent4 : WalkerAgentBase
{
    public override void RandomiseStartPositions() { }

    public override void CollectObservationGeneral(VectorSensor sensor) { }

    public override float CalculateReward()
    {
        float localRotDeviationSum = 0;
        foreach (Bodypart bp in bodyPartsDict.Values)
        {
            localRotDeviationSum += 1 - (Quaternion.Angle(bp.rb.transform.localRotation, bp.startingRot) / 180);
        }
        return localRotDeviationSum / bodyPartsDict.Values.Count;
    }
}