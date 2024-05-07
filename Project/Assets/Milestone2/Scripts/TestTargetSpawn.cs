using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestTargetSpawn : MonoBehaviour
{
    public float spawnRadius = 5;
    public float spawnAngle;
    // Start is called before the first frame update
    void Start()
    {
        for (int i = 0; i < 1000; i++)
        {
            GameObject target = GameObject.CreatePrimitive(PrimitiveType.Cube);
            target.transform.parent = transform;
            // Generate a random angle in radians
            float angleLimit = spawnAngle / 360f;
            float angle = Random.Range(-Mathf.PI * angleLimit, Mathf.PI * angleLimit);

            // Calculate a random distance within the specified radius
            float distance = Random.Range(0f, spawnRadius);

            // Calculate the 2D offset using trigonometry
            float offsetX = Mathf.Cos(angle) * distance;
            float offsetZ = Mathf.Sin(angle) * distance;
            target.transform.localPosition = new Vector3(offsetX, 0f, offsetZ);
        }
    }
}
