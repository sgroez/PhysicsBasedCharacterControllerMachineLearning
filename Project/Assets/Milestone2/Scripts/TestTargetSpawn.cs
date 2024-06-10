using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestTargetSpawn : MonoBehaviour
{
    public int algorithm;
    public float minSpawnRadius = 0;
    public float maxSpawnRadius = 5;
    public float spawnAngle;

    private Vector3 startingPos;

    void Start()
    {
        startingPos = transform.position;
        for (int i = 0; i < 1000; i++)
        {
            GameObject target = GameObject.CreatePrimitive(PrimitiveType.Cube);
            target.transform.parent = transform;
            Vector3 newTargetOffset = Vector3.zero;
            if (algorithm == 0)
            {
                newTargetOffset = RandomPositionAlgorithm0();
            }
            else
            {
                newTargetOffset = RandomPositionAlgorithm1();
            }
            target.transform.position = startingPos + newTargetOffset;
        }
    }

    Vector3 RandomPositionAlgorithm0()
    {
        Vector3 newTargetOffset = Random.insideUnitSphere * maxSpawnRadius;
        newTargetOffset.y = 0f;
        return newTargetOffset;
    }

    Vector3 RandomPositionAlgorithm1()
    {
        // Generate a random angle in radians
        float angleLimit = spawnAngle / 360f;
        float angle = Random.Range(-Mathf.PI * angleLimit, Mathf.PI * angleLimit);

        // Calculate a random distance within the specified radius
        float distance = Random.Range(minSpawnRadius, maxSpawnRadius);

        // Calculate the 2D offset using trigonometry
        float offsetX = Mathf.Cos(angle) * distance;
        float offsetZ = Mathf.Sin(angle) * distance;

        return new Vector3(offsetX, 0f, offsetZ);
    }
}
