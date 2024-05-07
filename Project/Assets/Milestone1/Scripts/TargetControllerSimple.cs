using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TargetControllerSimple : MonoBehaviour
{
    [Header("Collider Tag To Detect")]
    public string tagToDetect = "agent"; //collider tag to detect

    [Header("Target Placement")]
    public float spawnRadius; //The radius in which a target can be randomly spawned.
    public bool respawnIfTouched; //Should the target respawn to a different position when touched

    [Header("Target Fell Protection")]
    public bool respawnIfFallsOffPlatform = true; //If the target falls off the platform, reset the position.
    public float fallDistance = 5; //distance below the starting height that will trigger a respawn

    protected Vector3 m_startingPos; //the starting position of the target
    // Start is called before the first frame update
    void OnEnable()
    {
        m_startingPos = transform.position;
    }

    // Update is called once per frame
    void Update()
    {
        if (respawnIfFallsOffPlatform)
        {
            if (transform.position.y < m_startingPos.y - fallDistance)
            {
                Debug.Log($"{transform.name} Fell Off Platform");
                MoveTargetToRandomPosition();
            }
        }
    }

    /// <summary>
    /// Moves target to a random position within specified radius.
    /// </summary>
    public virtual void MoveTargetToRandomPosition()
    {
        var newTargetPos = m_startingPos + (Random.insideUnitSphere * spawnRadius);
        newTargetPos.y = m_startingPos.y;
        transform.position = newTargetPos;
    }

    private void OnCollisionEnter(Collision col)
    {
        if (col.transform.CompareTag(tagToDetect))
        {
            if (respawnIfTouched)
            {
                MoveTargetToRandomPosition();
            }
        }
    }
}
