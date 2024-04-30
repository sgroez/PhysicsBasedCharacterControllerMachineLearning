using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.MLAgents;
using Unity.MLAgents.Actuators;
using Unity.MLAgents.Sensors;

public class WalkerAgentSimple2 : WalkerAgentSimple
{
    public override void CollectObservations(VectorSensor sensor)
    {
        sensor.AddObservation(0f);
    }
}
