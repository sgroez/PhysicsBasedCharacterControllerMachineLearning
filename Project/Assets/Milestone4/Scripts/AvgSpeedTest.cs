using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AvgSpeedTest : MonoBehaviour
{
    public int timeHorizon = 100;
    float[] previousSpeeds;
    int currentIndex;

    Vector3 previousPosition;

    void Start()
    {
        previousSpeeds = new float[timeHorizon];
        currentIndex = 0;
        previousPosition = transform.position;
    }

    void addSpeed(float currentSpeed)
    {
        previousSpeeds[currentIndex] = currentSpeed;
        currentIndex = (currentIndex + 1) % previousSpeeds.Length;
    }

    float getAvgSpeed()
    {
        float sum = 0;
        foreach (float speed in previousSpeeds)
        {
            sum += speed;
        }
        return sum / previousSpeeds.Length;
    }

    void FixedUpdate()
    {
        // Calculate the displacement vector
        Vector3 displacement = transform.position - previousPosition;

        // Calculate the speed (magnitude of the displacement divided by the time step)
        float speed = displacement.magnitude / Time.fixedDeltaTime;

        addSpeed(speed);
        float avgSpeed = getAvgSpeed();
        Debug.Log(avgSpeed);

        // Update the previous position for the next frame
        previousPosition = transform.position;
    }
}