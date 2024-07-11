using UnityEngine;
using Random = UnityEngine.Random;
using Unity.MLAgents;

public class LookTargetController : MonoBehaviour
{
    [Header("Reference Transforms to calculate position")]
    public Transform root;
    public Transform head;
    public Transform target;

    [Header("Angle To Spawn Look Target At")]
    public float minAngle;
    public float maxAngle;
    private float randomAngle = 0f;

    public bool testMode = false;

    float radius = 5f;

    void OnEnable()
    {
        SetRandomLookAngle();
    }

    void FixedUpdate()
    {
        if (testMode)
        {
            transform.position = head.position + head.forward * 5;
        }
        else
        {
            SetLookTarget();
        }
    }

    public void SetRandomLookAngle()
    {
        randomAngle = Random.Range(minAngle, maxAngle);
    }

    private void SetLookTarget()
    {
        float initialAngleRad = Mathf.Atan2(root.position.z - root.position.z, target.position.x - root.position.x);
        float angleRad = randomAngle * Mathf.Deg2Rad;

        float newAngleRad = initialAngleRad + angleRad;

        float x = root.position.x + radius * Mathf.Cos(newAngleRad);
        float z = root.position.z + radius * Mathf.Sin(newAngleRad);
        transform.position = new Vector3(x, head.position.y, z);
    }
}
