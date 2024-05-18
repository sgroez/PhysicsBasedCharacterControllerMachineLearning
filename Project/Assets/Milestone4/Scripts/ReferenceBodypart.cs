using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.MLAgents;


public class ReferenceBodypart
{
    public Transform transform;
    [HideInInspector] public Rigidbody rb;
    private Vector3 previousPos;
    private Quaternion previousRot;
    private float previousTime;

    public ReferenceBodypart(Transform transform)
    {
        this.transform = transform;
        this.previousPos = transform.position;
        this.previousRot = transform.rotation;
        previousTime = Time.time;
        rb = transform.GetComponent<Rigidbody>();
    }

    public void GetVelocities(out Vector3 velocity, out Vector3 angularVelocity)
    {
        // Calculate current time
        float currentTime = Time.time;

        // Calculate change in time
        float deltaTime = currentTime - previousTime;

        // Calculate out variable velocity using finite differences
        velocity = deltaTime > 0 ? (transform.position - previousPos) / deltaTime : Vector3.zero;

        // Calculate out variable angular velocity using finite differences
        Quaternion deltaRotation = transform.rotation * Quaternion.Inverse(previousRot);
        Vector3 deltaRotationsEuler = deltaRotation.eulerAngles;
        Vector3 deltaRotationRadians = Vector3.zero;
        //convert from angles to radians
        deltaRotationRadians.x = deltaRotationsEuler.x * Mathf.Deg2Rad;
        deltaRotationRadians.y = deltaRotationsEuler.y * Mathf.Deg2Rad;
        deltaRotationRadians.z = deltaRotationsEuler.z * Mathf.Deg2Rad;
        angularVelocity = deltaTime > 0 ? deltaRotationRadians / deltaTime : Vector3.zero;

        // Update previous position, rotation, and time
        previousPos = transform.position;
        previousRot = transform.rotation;
        previousTime = currentTime;
    }
}