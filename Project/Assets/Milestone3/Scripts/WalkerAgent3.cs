using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using Unity.MLAgents;
using Unity.MLAgents.Sensors;

public class WalkerAgent3 : WalkerAgent1
{
    public Transform footL;
    public Transform footR;
    private bool leftForward = false;
    private float switchTime = 0f;

    public override void OnEpisodeBegin()
    {
        base.OnEpisodeBegin();
        leftForward = false;
        switchTime = Time.fixedTime;
    }
    public override void FixedUpdate()
    {
        float leftDistance = Vector3.Distance(footL.position, target.position);
        float rightDistance = Vector3.Distance(footR.position, target.position);
        if (!leftForward && leftDistance < rightDistance)
        {
            leftForward = true;
            switchTime = Time.fixedTime;
        }
        else if (leftForward && rightDistance < leftDistance)
        {
            leftForward = false;
            switchTime = Time.fixedTime;
        }
        float switchDeltaTime = Time.fixedTime - switchTime;
        float footSwitchReward = -Mathf.Clamp((switchDeltaTime / 4) - 0.3f, 0f, 1f);
        if (logStats && footSwitchReward < 0) Debug.Log($"foot switch reward: {footSwitchReward}, switched: {switchDeltaTime} seconds ago");
        statsRecorder.Add("Reward/FootSwitchReward", footSwitchReward);
        AddReward(footSwitchReward);
        base.FixedUpdate();
    }
}
