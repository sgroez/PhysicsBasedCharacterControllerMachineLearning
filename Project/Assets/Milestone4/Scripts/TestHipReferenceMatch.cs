using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestHipReferenceMatch : MonoBehaviour
{
    public Transform t1;
    public Transform t2;

    void FixedUpdate()
    {
        Vector3 up1 = t1.up;
        Vector3 up2 = t2.up;
        up1.y = 0;
        up2.y = 0;
        up1.Normalize();
        up2.Normalize();
        float angle = Vector3.Angle(up2, up1);
        Debug.Log($"angle: {angle}");
    }
}