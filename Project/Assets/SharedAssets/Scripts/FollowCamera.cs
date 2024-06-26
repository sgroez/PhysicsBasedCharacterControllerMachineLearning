using System;
using UnityEngine;

public class FollowCamera : MonoBehaviour
{
    public Transform followObject;
    public float offset;

    void FixedUpdate()
    {
        Vector3 flatForward = followObject.forward;
        flatForward.y = 0f;
        transform.position = followObject.position + (flatForward * offset);
        transform.LookAt(followObject);
    }
}
