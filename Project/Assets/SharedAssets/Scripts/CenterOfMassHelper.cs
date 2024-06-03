using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class CenterOfMassHelper : MonoBehaviour
{
    public float gizmosSize = 0.05f;
    private Rigidbody r;

    void Start()
    {
        r = GetComponent<Rigidbody>();
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.yellow;
        if (!r) return;
        Gizmos.DrawSphere(transform.position + transform.rotation * r.centerOfMass, gizmosSize);
    }
}