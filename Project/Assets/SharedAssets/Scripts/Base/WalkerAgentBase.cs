using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.MLAgents;
using Unity.MLAgents.Actuators;
using Unity.MLAgents.Sensors;
using Random = UnityEngine.Random;

public class WalkerAgentBase : Agent
{
    [Header("Body Parts")]
    [Space(10)]
    public Transform root;
    protected List<Bodypart> bodyParts = new List<Bodypart>();

    [Header("Bodypart Settings")]
    [Space(10)]
    public BodypartConfig bpConfig;

    [Header("Enable debug")]
    [Space(10)]
    public bool enableDebug;

    protected EnvironmentParameters resetParams;
    protected StatsRecorder statsRecorder;

    /*
    * Override Agent functions to comply with ML Agents package &
    * Implement reward structure in fixed update
    */
    public override void Initialize()
    {
        InitEnvVariables();
        foreach (Rigidbody rb in root.GetComponentsInChildren<Rigidbody>())
        {
            Transform t = rb.transform;
            var bp = new Bodypart(t, bpConfig, this);
            bodyParts.Add(bp);
        }
        resetParams = Academy.Instance.EnvironmentParameters;
        statsRecorder = Academy.Instance.StatsRecorder;
        InitEnvParamCallbacks();
        if (!enableDebug) return;
        //call debug methods
        DebugActionCount();
    }
    public override void OnEpisodeBegin()
    {
        foreach (Bodypart bp in bodyParts)
        {
            bp.Reset(bp.startingPos, bp.startingRot);
        }
        UpdateEnvVariablesOnEpisode();
        RandomiseStartPositions();
    }
    public override void CollectObservations(VectorSensor sensor)
    {
        CollectObservationGeneral(sensor);
        foreach (Bodypart bp in bodyParts)
        {
            CollectObservationBodyPart(sensor, bp);
        }
    }
    public override void OnActionReceived(ActionBuffers actionBuffers)
    {
        int index = -1;
        var continuousActions = actionBuffers.ContinuousActions;

        foreach (Bodypart bp in bodyParts)
        {
            if (bp.rb.transform == root) continue;
            float targetRotX = bp.dof.x == 1 ? continuousActions[++index] : 0;
            float targetRotY = bp.dof.y == 1 ? continuousActions[++index] : 0;
            float targetRotZ = bp.dof.z == 1 ? continuousActions[++index] : 0;
            float jointStrength = bp.dof.sqrMagnitude > 0 ? continuousActions[++index] : 0;
            bp.SetJointTargetRotation(targetRotX, targetRotY, targetRotZ);
            bp.SetJointStrength(jointStrength);
        }
    }
    void FixedUpdate()
    {
        UpdateEnvVariablesOnFixedUpdate();
        AddReward(CalculateReward());
    }


    /*
    * Basic functions to override or extend for more complex walker agents
    */
    public virtual void InitEnvVariables() { }

    public virtual void InitEnvParamCallbacks() { }

    public virtual void UpdateEnvVariablesOnEpisode() { }

    public virtual void UpdateEnvVariablesOnFixedUpdate() { }

    public virtual void RandomiseStartPositions()
    {
        //random start rotation to help generalize
        bodyParts[0].rb.transform.rotation = Quaternion.Euler(0, Random.Range(0.0f, 360.0f), 0);
    }

    public virtual void CollectObservationGeneral(VectorSensor sensor) { }

    public virtual void CollectObservationBodyPart(VectorSensor sensor, Bodypart bp) { }

    public virtual float CalculateReward() { return 1f; }

    private void DebugActionCount()
    {
        int actionCount = 0;
        foreach (Bodypart bp in bodyParts)
        {
            int bpActionCount = 0;
            bpActionCount += (int)bp.dof.x;
            bpActionCount += (int)bp.dof.y;
            bpActionCount += (int)bp.dof.z;
            bpActionCount += bp.dof.sqrMagnitude > 0 ? 1 : 0;
            actionCount += bpActionCount;
        }
        Debug.Log(actionCount);
    }
}
