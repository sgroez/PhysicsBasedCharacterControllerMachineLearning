using UnityEngine;
using Random = UnityEngine.Random;
using Unity.MLAgents;

public class LookTargetController : MonoBehaviour
{
    [Header("Reference Component to calculate position")]
    public WalkerAgent2 agent;

    [Header("Angle To Spawn Look Target At")]
    public float minAngle;
    public float maxAngle;
    private float randomAngle = 0f;
    private bool isFirstAngle = true;

    private Vector3 targetPosition;

    public void UpdateTargetPosition(Vector3 newTargetPosition)
    {
        targetPosition = newTargetPosition;
        if (!isFirstAngle)
        {
            randomAngle = Random.Range(minAngle, maxAngle);
        }
        else
        {
            isFirstAngle = false;
        }
        SetLookTarget();
    }

    void FixedUpdate()
    {
        SetLookTarget();
    }

    private void SetLookTarget()
    {
        float radius = 5f;
        Vector3 agentPosition = agent.root.position;

        float initialAngleRad = Mathf.Atan2(targetPosition.z - agentPosition.z, targetPosition.x - agentPosition.x);
        float angleRad = randomAngle * Mathf.Deg2Rad;

        float newAngleRad = initialAngleRad + angleRad;

        float x = agentPosition.x + radius * Mathf.Cos(newAngleRad);
        float z = agentPosition.z + radius * Mathf.Sin(newAngleRad);
        transform.position = new Vector3(x, agentPosition.y, z);
    }
}
