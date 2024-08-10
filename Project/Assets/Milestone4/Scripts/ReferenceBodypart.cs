using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.MLAgents;


public class ReferenceBodypart : MonoBehaviour
{
    [HideInInspector] public Rigidbody rb;
    [HideInInspector] public Vector3 velocity = Vector3.zero;
    [HideInInspector] public Vector3 angularVelocity = Vector3.zero;

    private float previousTime;
    private Vector3 previousPos;
    private Quaternion previousRot;

    void OnEnable()
    {
        rb = GetComponent<Rigidbody>();
        ResetReferenceBodypart();
    }

    public void ResetReferenceBodypart()
    {
        previousTime = Time.fixedTime;
        previousPos = transform.position;
        previousRot = transform.rotation;
    }

    void FixedUpdate()
    {
        //calculate the time difference since last update
        float currentTime = Time.fixedTime;
        float timeDiff = currentTime - previousTime;

        //update velocities
        UpdateVelocity(timeDiff);
        UpdateAngularVelocity(timeDiff);

        //update previous time
        previousTime = currentTime;
    }

    void UpdateVelocity(float timeDiff)
    {
        //calculate the displacement vector
        Vector3 currentPos = transform.position;
        Vector3 displacement = currentPos - previousPos;

        //calculate the velocity
        velocity = Vector3.zero;
        if (timeDiff > 0)
        {
            velocity = displacement / timeDiff;
        }

        //update the previous time and previous position
        previousPos = currentPos;
    }

    void UpdateAngularVelocity(float timeDiff)
    {
        //calculate the change in rotation
        Quaternion currentRot = transform.rotation;
        Quaternion deltaRot = currentRot * Quaternion.Inverse(previousRot);

        //convert the delta rotation to axis-angle representation
        deltaRot.ToAngleAxis(out float angle, out Vector3 axis);

        //calculate the angular velocity in radians per second
        angularVelocity = Vector3.zero;
        if (timeDiff > 0)
        {
            angularVelocity = axis * angle * Mathf.Deg2Rad / timeDiff;
        }

        //update the previous rotation for the next frame
        previousRot = currentRot;
    }
}