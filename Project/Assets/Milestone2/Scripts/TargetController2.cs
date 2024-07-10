using UnityEngine;
using Random = UnityEngine.Random;
using Unity.MLAgents;
using UnityEngine.Events;

public class TargetController2 : TargetController
{
    public WalkerAgent2 agent;
    public float neededAvgLookReward = 0.2f;
    protected override void OnCollisionEnter(Collision col)
    {
        if (col.transform.CompareTag(tagToDetect))
        {
            onCollisionEnterEvent.Invoke(col);
            if (respawnIfTouched && agent.avgLookReward > neededAvgLookReward)
            {
                agent.ResetAvgLookReward();
                MoveTargetToRandomPosition();
            }
            else
            {
                agent.EndEpisode();
            }
        }
    }
}
