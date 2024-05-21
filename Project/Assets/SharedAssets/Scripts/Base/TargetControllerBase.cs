using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.MLAgents;


public class TargetControllerBase : MonoBehaviour
{
    [Header("Collider Tag To Detect")]
    public string tagToDetect = "agent"; //collider tag to detect

    [Header("Set behaviour if touched")]
    public bool respawnIfTouched = false;
    public bool endEpisodeIfTouched = false;
    [HideInInspector] public Agent agent;

    [Header("Target Placement")]
    public float minSpawnRadius;
    public float maxSpawnRadius;
    public float spawnAngle; //The angle from the front of the agent in which the target can be randomly spawned.

    protected Vector3 startingPos; //the starting position of the target

    void OnEnable()
    {
        startingPos = transform.position;
    }

    private void OnCollisionEnter(Collision col)
    {
        if (col.transform.CompareTag(tagToDetect))
        {
            if (respawnIfTouched)
            {
                MoveTargetToRandomPosition();
            }
            if (endEpisodeIfTouched && agent != null)
            {
                agent.EndEpisode();
            }
        }
    }

    public virtual void MoveTargetToRandomPosition()
    {
        // Generate a random angle in radians
        float angleLimit = spawnAngle / 360f;
        float angle = Random.Range(-Mathf.PI * angleLimit, Mathf.PI * angleLimit);

        // Calculate a random distance within the specified radius
        float distance = Random.Range(minSpawnRadius, maxSpawnRadius);

        // Calculate the 2D offset using trigonometry
        float offsetX = Mathf.Cos(angle + Mathf.PI / 2) * distance;
        float offsetZ = Mathf.Sin(angle + Mathf.PI / 2) * distance;
        transform.position = startingPos + new Vector3(offsetX, 0f, offsetZ);
    }
}