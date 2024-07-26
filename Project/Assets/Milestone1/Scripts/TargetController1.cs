using UnityEngine;
using Random = UnityEngine.Random;
using Unity.MLAgents;
using UnityEngine.Events;

public class TargetController1 : MonoBehaviour
{
    [Header("Target Config")]
    public float spawnRadius; //The radius in which a target can be randomly spawned.
    public bool setRandomStartPos = true;

    private Vector3 m_startingPos; //the starting position of the target

    void OnEnable()
    {
        m_startingPos = transform.position;
        if (setRandomStartPos)
        {
            MoveTargetToRandomPosition();
        }
    }

    /// <summary>
    /// Moves target to a random position within specified radius.
    /// </summary>
    public void MoveTargetToRandomPosition()
    {
        var newTargetPos = m_startingPos + (Random.insideUnitSphere * spawnRadius);
        newTargetPos.y = m_startingPos.y;
        transform.position = newTargetPos;
    }
}
