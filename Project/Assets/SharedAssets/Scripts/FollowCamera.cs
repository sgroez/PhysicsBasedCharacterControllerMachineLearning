using System;
using UnityEngine;

public class FollowCamera : MonoBehaviour
{
    public Transform followObject;
    public float offset;
    public Orientation orientation;

    void FixedUpdate()
    {
        Vector3 direction = followObject.forward;
        switch (orientation)
        {
            case Orientation.Right:
                direction = followObject.right;
                break;
            case Orientation.Left:
                direction = -followObject.right;
                break;
            case Orientation.Backward:
                direction = -followObject.forward;
                break;
        }
        Vector3 flatDirection = new Vector3(direction.x, 0f, direction.z);
        transform.position = followObject.position + (flatDirection * offset);
        transform.LookAt(followObject);
    }
}
