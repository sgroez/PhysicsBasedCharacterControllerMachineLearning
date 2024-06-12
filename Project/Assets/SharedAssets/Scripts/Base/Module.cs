using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.MLAgents;
using Unity.MLAgents.Sensors;
using Random = UnityEngine.Random;
using UnityEngine.Events;

[RequireComponent(typeof(WalkerAgentBase))]
public class Module : MonoBehaviour
{
    protected WalkerAgentBase baseAgent;
    public virtual void Initialize(WalkerAgentBase baseAgent)
    {
        this.baseAgent = baseAgent;
        baseAgent.onEpisodeBegin.AddListener(OnEpisodeBegin);
        baseAgent.onCollectObservations.AddListener(OnCollectObservations);
        baseAgent.onAgentFixedUpdate.AddListener(OnAgentFixedUpdate);
    }

    public virtual void OnEpisodeBegin() { }
    public virtual void OnCollectObservations(VectorSensor sensor) { }
    public virtual void OnAgentFixedUpdate() { }
}
