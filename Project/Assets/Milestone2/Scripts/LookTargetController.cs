using UnityEngine;
using Random = UnityEngine.Random;
using Unity.MLAgents;

public class LookTargetController : MonoBehaviour
{
    [Header("Reference Components to calculate position")]
    public TargetController targetController;
    public WalkerAgent2 agent;

    [Header("Angle To Spawn Look Target At")]
    public float minAngle;
    public float maxAngle;

    private Vector3 targetPosition;
    private Vector3 agentPosition;

    public void UpdateTargetPosition(Vector3 newTargetPosition)
    {
        targetPosition = newTargetPosition;
        SetLookTarget();
    }

    public void UpdateAgentPosition(Vector3 newAgentPosition)
    {
        agentPosition = newAgentPosition;
        SetLookTarget();
    }

    private void SetLookTarget()
    {
        float randomAngle = Random.Range(minAngle, maxAngle);

        float radius = (agentPosition - targetPosition).magnitude;
        if (radius == 0)
        {
            transform.position = agentPosition;
            return;
        }

        float initialAngleRad = Mathf.Atan2(targetPosition.z - agentPosition.z, targetPosition.x - agentPosition.x);
        float angleRad = randomAngle * Mathf.Deg2Rad;

        float newAngleRad = initialAngleRad + angleRad;

        float x = agentPosition.x + radius * Mathf.Cos(newAngleRad);
        float z = agentPosition.z + radius * Mathf.Sin(newAngleRad);
        transform.position = new Vector3(x, agentPosition.y, z);
    }
}
