using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using Unity.MLAgents;
using Unity.MLAgents.Sensors;
using Unity.MLAgents.Actuators;

public class WalkerStanding : WalkerAgent1
{
    public override void Initialize()
    {
        base.Initialize();
        targetWalkingSpeed = 0f;
    }
}
