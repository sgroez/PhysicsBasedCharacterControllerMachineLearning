using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MovementTrajectory : MonoBehaviour
{
    [Header("Amount of points calculated on bezier curve")]
    public int resolution = 10;

    [Header("Directions and Velocity to calculate start moveTowards and end point from")]
    public Vector3 moveDirection = Vector3.zero;
    public Vector3 velocity = Vector3.zero;
    public Vector3 lookAtDirection = Vector3.zero;

    // Calculated points on bezier curve
    Vector3[] points;

    void Start()
    {
        points = new Vector3[resolution];
    }

    void Update()
    {
        // Calculate points for bezier curve calculation
        Vector3 start = transform.position;
        Vector3 moveTowards = start + velocity;
        Vector3 end = start + moveDirection.normalized;

        // Calculate main bezier curve
        points = bezierCurve(start, moveTowards, end);
    }

    Vector3[] bezierCurve(Vector3 p0, Vector3 p1, Vector3 p2)
    {
        Vector3[] result = new Vector3[resolution];
        for (int i = 0; i < points.Length; i++)
        {
            float t = (float)i / points.Length;
            float x = (1 - t) * ((1 - t) * p0.x + t * p1.x) + t * ((1 - t) * p1.x + t * p2.x);
            float z = (1 - t) * ((1 - t) * p0.z + t * p1.z) + t * ((1 - t) * p1.z + t * p2.z);
            result[i] = new Vector3(x, transform.position.y, z);
        }
        return result;
    }

    void OnDrawGizmos()
    {
        // Draw a yellow sphere at the transform's position
        Gizmos.color = Color.blue;

        if (points == null || points.Length == 0) return;
        Gizmos.DrawLine(transform.position, points[0]);
        for (int i = 1; i < points.Length; i++)
        {
            Vector3 currentPoint = points[i];
            Gizmos.DrawLine(points[i - 1], currentPoint);
            Gizmos.DrawLine(currentPoint, currentPoint + lookAtDirection.normalized * 0.25f);
        }
    }
}
