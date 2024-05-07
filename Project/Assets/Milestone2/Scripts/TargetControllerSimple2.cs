using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TargetControllerSimple2 : TargetControllerSimple
{
    public float spawnAngle;
    public override void MoveTargetToRandomPosition()
    {
        // Generate a random angle in radians
        float angleLimit = spawnAngle / 360f;
        float angle = Random.Range(-Mathf.PI * angleLimit, Mathf.PI * angleLimit);

        // Calculate a random distance within the specified radius
        float distance = Random.Range(0f, spawnRadius);

        // Calculate the 2D offset using trigonometry
        float offsetX = Mathf.Cos(angle) * distance;
        float offsetZ = Mathf.Sin(angle) * distance;
        transform.localPosition = new Vector3(offsetX, m_startingPos.y, offsetZ);
    }
}
