using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using Unity.MLAgents;
using Unity.MLAgents.Sensors;

public class WalkerAgent3 : WalkerAgent1
{
    public Transform handL;
    public Transform handR;
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
        float leftFootDistance = Vector3.Distance(footL.position, target.position);
        float rightFootDistance = Vector3.Distance(footR.position, target.position);
        if (!leftForward && leftFootDistance < rightFootDistance)
        {
            leftForward = true;
            switchTime = Time.fixedTime;
        }
        else if (leftForward && rightFootDistance < leftFootDistance)
        {
            leftForward = false;
            switchTime = Time.fixedTime;
        }
        float switchDeltaTime = Time.fixedTime - switchTime;
        float footSwitchReward = -Mathf.Clamp((switchDeltaTime / 4) - 0.3f, 0f, 1f);
        if (logStats && footSwitchReward < 0) Debug.Log($"foot switch reward: {footSwitchReward}, switched: {switchDeltaTime} seconds ago");
        RecordStat("Reward/FootSwitchReward", footSwitchReward);

        float leftHandDistance = Vector3.Distance(handL.position, target.position);
        float rightHandDistance = Vector3.Distance(handR.position, target.position);
        float rootDistance = Vector3.Distance(root.position, target.position);
        float pendulumSum = 0f;
        if (leftForward)
        {
            pendulumSum = (rootDistance - rightHandDistance) + (leftHandDistance - rootDistance);
        }
        else
        {
            pendulumSum = (rootDistance - leftHandDistance) + (rightHandDistance - rootDistance);
        }
        float armPendulumReward = Mathf.Clamp((pendulumSum / 2) - 0.5f, -1f, 0f);
        RecordStat("Reward/ArmPendulumReward", armPendulumReward);

        float totalPower = GetTotalPower();
        float powerSaveReward = -Mathf.Clamp(totalPower / 3000 - 0.05f, 0f, 1f);
        if (logStats) Debug.Log($"power save reward: {powerSaveReward}, total: {totalPower}");
        RecordStat("Reward/PowerSaveReward", powerSaveReward);

        AddReward(Mathf.Min(footSwitchReward, powerSaveReward, armPendulumReward));

        base.FixedUpdate();
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
}
