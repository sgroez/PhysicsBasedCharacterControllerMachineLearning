using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.MLAgents;
using Unity.MLAgents.Actuators;
using Unity.MLAgents.Sensors;
using Random = UnityEngine.Random;
using UnityEngine.Events;

[System.Serializable]
public class VectorSensorEvent : UnityEvent<VectorSensor> { }

public class WalkerAgentBase : Agent
{
    [Header("Bodyparts")]
    public Transform root;
    public Transform head;
    public List<Bodypart> bodyparts = new List<Bodypart>();

    [Header("Events To Register Module Functions As Callbacks")]
    public UnityEvent onEpisodeBegin;
    public VectorSensorEvent onCollectObservations;
    public UnityEvent onAgentFixedUpdate;

    [Header("Compatibility Mode To Be Able To Use Walker Demo Training Model (Only For Walker Model)")]
    public bool useCompatibility = false;

    /*
    * hidden script variables and references
    */
    [HideInInspector] public EnvironmentParameters resetParams;
    [HideInInspector] public StatsRecorder statsRecorder;

    private List<string> bodypartsOrder = new List<string>()
    {
    "hips", //0
    "chest", //1
    "spine", //2
    "head", //3
    "thighL", //4
    "shinL", //5
    "footL", //6
    "thighR", //7
    "shinR", //8
    "footR", //9
    "upper_arm_L", //10
    "lower_arm_L", //11
    "hand_L", //12
    "upper_arm_R", //13
    "lower_arm_R", //14
    "hand_R" //15
    };

    public override void Initialize()
    {
        foreach (Bodypart bp in root.GetComponentsInChildren<Bodypart>())
        {
            bodyparts.Add(bp);
            bp.onTouchingGround.AddListener(OnTouchingGround);
        }
        //if using compatibility mode sort to match same order as the one in Walker demo
        if (useCompatibility)
        {
            bodyparts.Sort((x, y) => bodypartsOrder.IndexOf(x.name).CompareTo(bodypartsOrder.IndexOf(y.name)));
        }
        resetParams = Academy.Instance.EnvironmentParameters;
        statsRecorder = Academy.Instance.StatsRecorder;

        //setup events
        onEpisodeBegin = new UnityEvent();
        onCollectObservations = new VectorSensorEvent();
        onAgentFixedUpdate = new UnityEvent();
        foreach (Module module in GetComponentsInChildren<Module>())
        {
            module.Initialize(this);
        }
    }

    public override void OnEpisodeBegin()
    {
        onEpisodeBegin.Invoke();
    }

    public override void CollectObservations(VectorSensor sensor)
    {
        onCollectObservations.Invoke(sensor);
    }

    public override void OnActionReceived(ActionBuffers actionBuffers)
    {
        if (useCompatibility)
        {
            OnActionReceivedCompatibility(actionBuffers);
        }
        else
        {
            OnActionReceivedNew(actionBuffers);
        }
    }

    public void OnActionReceivedNew(ActionBuffers actionBuffers)
    {
        var continuousActions = actionBuffers.ContinuousActions;
        int i = -1;

        foreach (Bodypart bp in bodyparts)
        {
            if (bp.dof.sqrMagnitude <= 0) continue;
            float targetRotX = bp.dof.x == 1 ? continuousActions[++i] : 0;
            float targetRotY = bp.dof.y == 1 ? continuousActions[++i] : 0;
            float targetRotZ = bp.dof.z == 1 ? continuousActions[++i] : 0;
            float jointStrength = continuousActions[++i];
            bp.SetJointTargetRotation(targetRotX, targetRotY, targetRotZ);
            bp.SetJointStrength(jointStrength);
        }
    }

    public void OnActionReceivedCompatibility(ActionBuffers actionBuffers)
    {
        var i = -1;

        var continuousActions = actionBuffers.ContinuousActions;
        bodyparts[1].SetJointTargetRotation(continuousActions[++i], continuousActions[++i], continuousActions[++i]);
        bodyparts[2].SetJointTargetRotation(continuousActions[++i], continuousActions[++i], continuousActions[++i]);

        bodyparts[4].SetJointTargetRotation(continuousActions[++i], continuousActions[++i], 0);
        bodyparts[7].SetJointTargetRotation(continuousActions[++i], continuousActions[++i], 0);
        bodyparts[5].SetJointTargetRotation(continuousActions[++i], 0, 0);
        bodyparts[8].SetJointTargetRotation(continuousActions[++i], 0, 0);
        bodyparts[9].SetJointTargetRotation(continuousActions[++i], continuousActions[++i], continuousActions[++i]);
        bodyparts[6].SetJointTargetRotation(continuousActions[++i], continuousActions[++i], continuousActions[++i]);

        bodyparts[10].SetJointTargetRotation(continuousActions[++i], continuousActions[++i], 0);
        bodyparts[13].SetJointTargetRotation(continuousActions[++i], continuousActions[++i], 0);
        bodyparts[11].SetJointTargetRotation(continuousActions[++i], 0, 0);
        bodyparts[14].SetJointTargetRotation(continuousActions[++i], 0, 0);
        bodyparts[3].SetJointTargetRotation(continuousActions[++i], continuousActions[++i], 0);

        //update joint strength settings
        bodyparts[1].SetJointStrength(continuousActions[++i]);
        bodyparts[2].SetJointStrength(continuousActions[++i]);
        bodyparts[3].SetJointStrength(continuousActions[++i]);
        bodyparts[4].SetJointStrength(continuousActions[++i]);
        bodyparts[5].SetJointStrength(continuousActions[++i]);
        bodyparts[6].SetJointStrength(continuousActions[++i]);
        bodyparts[7].SetJointStrength(continuousActions[++i]);
        bodyparts[8].SetJointStrength(continuousActions[++i]);
        bodyparts[9].SetJointStrength(continuousActions[++i]);
        bodyparts[10].SetJointStrength(continuousActions[++i]);
        bodyparts[11].SetJointStrength(continuousActions[++i]);
        bodyparts[13].SetJointStrength(continuousActions[++i]);
        bodyparts[14].SetJointStrength(continuousActions[++i]);
    }

    void FixedUpdate()
    {
        onAgentFixedUpdate.Invoke();
    }

    void OnTouchingGround()
    {
        AddReward(-1f);
        EndEpisode();
    }
}
