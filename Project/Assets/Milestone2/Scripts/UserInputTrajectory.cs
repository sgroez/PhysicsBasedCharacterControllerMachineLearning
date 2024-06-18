using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UserInputTrajectory : MonoBehaviour
{
    public float strength = 1;
    public int resolution = 10;

    Rigidbody rb;
    Vector3 start = Vector3.zero;
    Vector3 moveTowards = Vector3.zero;
    Vector3 end = Vector3.zero;
    Vector3[] points;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        points = new Vector3[resolution];
    }
    // Update is called once per frame
    void Update()
    {
        // Simple add force in input direction
        Vector3 input = new Vector3(Input.GetAxisRaw("Horizontal"), 0, Input.GetAxisRaw("Vertical"));
        rb.AddForce(input * strength);

        // Calculate points for bezier curve calculation
        start = transform.position;
        moveTowards = start + rb.velocity / 10;
        end = start + input;

        // Calculate sub curves
        Vector3[] bz0 = bezierCurve(start, moveTowards);
        Vector3[] bz1 = bezierCurve(moveTowards, end);

        // Calculate main bezier curve
        points = bezierCurve(bz0, bz1);
    }

    Vector3[] bezierCurve(Vector3 start, Vector3 end)
    {
        Vector3[] result = new Vector3[resolution];
        for (int i = 0; i < points.Length; i++)
        {
            float t = (float)i / points.Length;
            float x = (1 - t) * start.x + t * end.x;
            float z = (1 - t) * start.z + t * end.z;
            result[i] = new Vector3(x, transform.position.y, z);
        }
        return result;
    }

    Vector3[] bezierCurve(Vector3[] bz0, Vector3[] bz1)
    {
        Vector3[] result = new Vector3[resolution];
        for (int i = 0; i < points.Length; i++)
        {
            float t = (float)i / points.Length;
            float x = (1 - t) * bz0[i].x + t * bz1[i].x;
            float z = (1 - t) * bz1[i].z + t * bz1[i].z;
            result[i] = new Vector3(x, transform.position.y, z);
        }
        return result;
    }

    void OnDrawGizmos()
    {
        // Draw a yellow sphere at the transform's position
        Gizmos.color = Color.blue;

        if (points == null) return;
        for (int i = 0; i < points.Length; i++)
        {
            Gizmos.DrawSphere(points[i], 0.03f * (i + 0.5f));
        }
    }
}
