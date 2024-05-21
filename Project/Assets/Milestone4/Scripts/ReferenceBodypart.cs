using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.MLAgents;


public class ReferenceBodypart
{
    public Transform transform;
    public Rigidbody rb;
    private Vector3 previousPos;
    private Quaternion previousRot;
    public Vector3 velocity;
    public Vector3 angularVelocity;

    public ReferenceBodypart(Transform transform)
    {
        this.transform = transform;
        this.previousPos = transform.position;
        this.previousRot = transform.rotation;
        rb = transform.GetComponent<Rigidbody>();
    }

    public void Reset()
    {
        previousPos = transform.position;
        previousRot = transform.rotation;
    }

    public void Update()
    {
        // Calculate out variable velocity using finite differences
        velocity = (transform.position - previousPos) / Time.deltaTime;

        // Calculate out variable angular velocity using finite differences
        Quaternion deltaRotation = transform.rotation * Quaternion.Inverse(previousRot);
        //convert from quaternion to euler angles
        Vector3 deltaRotationsEuler = deltaRotation.eulerAngles;
        //convert from angles to radians
        Vector3 deltaRotationRadians = Vector3.zero;
        deltaRotationRadians.x = deltaRotationsEuler.x * Mathf.Deg2Rad;
        deltaRotationRadians.y = deltaRotationsEuler.y * Mathf.Deg2Rad;
        deltaRotationRadians.z = deltaRotationsEuler.z * Mathf.Deg2Rad;
        angularVelocity = deltaRotationRadians / Time.deltaTime;

        // Update previous position, rotation
        previousPos = transform.position;
        previousRot = transform.rotation;
    }
}