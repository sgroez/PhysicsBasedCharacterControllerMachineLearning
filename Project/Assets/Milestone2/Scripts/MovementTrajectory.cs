using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MovementTrajectory : MonoBehaviour
{
    [Header("Amount of points calculated on bezier curve")]
    public float resolution = 0.01f;
    public int walkingTrajectoryLength = 10;
    public int lookTrajectoryLength = 3;

    [Header("Directions and Velocity to calculate start moveTowards and end point from")]
    public Transform root;

    public bool autoUpdate = false;
    public Transform moveToGoal;
    public Transform lookAtGoal;

    public int reachedPoints = 1;

    // Calculated points on bezier curve
    [HideInInspector] public Vector3[,] points;

    protected void Start()
    {
        points = new Vector3[walkingTrajectoryLength, lookTrajectoryLength + 1];
    }

    void Update()
    {
        if (autoUpdate)
        {
            UpdateCurve(moveToGoal.position, root.forward, lookAtGoal.position, root.forward);
        }
    }

    public virtual void UpdateCurve(Vector3 moveToGoal, Vector3 velocity, Vector3 lookAtGoal, Vector3 lookAtDirection)
    {
        // Calculate points for bezier curve calculation
        Vector3 p0 = root.position;
        Vector3 p1 = p0 + velocity;
        Vector3 p2 = moveToGoal;

        // Calculate main bezier curve
        Vector3[] mainCurve = bezierCurve(p0, p1, p2, walkingTrajectoryLength, resolution);
        for (int i = 0; i < mainCurve.Length; i++)
        {
            //add point from main curve
            points[i, 0] = mainCurve[i];
            //calculate secondary look direction curve
            Vector3[] secondaryCurve = bezierCurve(mainCurve[i], mainCurve[i] + lookAtDirection, lookAtGoal, lookTrajectoryLength, resolution);
            //add points from secondary curve
            for (int j = 0; j < secondaryCurve.Length; j++)
            {
                points[i, j + 1] = secondaryCurve[j];
            }
        }

        //reset reached points
        reachedPoints = 1;
    }

    Vector3[] bezierCurve(Vector3 p0, Vector3 p1, Vector3 p2, int length, float resolution)
    {
        Vector3[] result = new Vector3[length];
        for (int i = 0; i < length; i++)
        {
            float t = i * resolution;
            float x = (1 - t) * ((1 - t) * p0.x + t * p1.x) + t * ((1 - t) * p1.x + t * p2.x);
            float z = (1 - t) * ((1 - t) * p0.z + t * p1.z) + t * ((1 - t) * p1.z + t * p2.z);
            result[i] = new Vector3(x, root.position.y, z);
        }
        return result;
    }

    void OnDrawGizmos()
    {
        Gizmos.color = Color.yellow;

        if (points == null) return;
        for (int i = reachedPoints; i < points.GetLength(0); i++)
        {
            Gizmos.DrawLine(i == 0 ? root.position : points[i - 1, 0], points[i, 0]);
            for (int j = 1; j < points.GetLength(1); j++)
            {
                Gizmos.DrawLine(points[i, j - 1], points[i, j]);
            }
        }
    }

    public Vector3[] GetCurrentPoints()
    {
        Vector3[] currentPoints = new Vector3[lookTrajectoryLength + 1];
        for (int j = 0; j < points.GetLength(1); j++)
        {
            currentPoints[j] = points[reachedPoints, j];
        }
        return currentPoints;
    }

    public void ReachedPoint()
    {
        reachedPoints = (int)Mathf.Clamp(reachedPoints + 1, 0f, points.GetLength(0) - 1);
    }
}
