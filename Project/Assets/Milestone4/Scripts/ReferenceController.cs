using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ReferenceController : MonoBehaviour
{
    [Header("Animator Of Reference Character")]
    [Space(10)]
    public Animator animator;
    public string animationName;
    public Transform referenceRoot;
    [HideInInspector] public List<ReferenceBodypart> referenceBodyparts = new List<ReferenceBodypart>();

    [Header("Reset Root transform")]
    [Space(10)]
    public bool resetRootTransform;
    private Vector3 startingPos;

    [Header("Min And Max to sample phase start from")]
    [Space(10)]
    public float phaseStartMin;
    public float phaseStartMax;

    void OnEnable()
    {
        // Get a reference to the Animator component
        animator = GetComponent<Animator>();
        foreach (Transform t in referenceRoot.GetComponentsInChildren<Transform>())
        {
            if (t.CompareTag("referenceBone"))
            {
                referenceBodyparts.Add(new ReferenceBodypart(t));
            }
        }
        startingPos = transform.position;
    }

    public void ResetReference()
    {
        if (resetRootTransform)
        {
            transform.position = startingPos;
        }
        // Call the Play method of the Animator to start playing the animation at a specific point
        float randomPhase = Random.Range(phaseStartMin, phaseStartMax);
        animator.Play(animationName, -1, randomPhase);
    }

    public float GetCurrentPhase()
    {
        float phase = animator.GetCurrentAnimatorStateInfo(0).normalizedTime % 1;
        return phase;
    }

    public Vector3 GetAvgVelocity()
    {
        Vector3 velSum = Vector3.zero;

        int numOfBp = 0;
        foreach (ReferenceBodypart rbp in referenceBodyparts)
        {
            numOfBp++;
            rbp.GetVelocities(out Vector3 velocity, out Vector3 _);
            velSum += velocity;
        }

        var avgVel = velSum / numOfBp;
        return avgVel;
    }

    public Vector3 CalculateCenterOfMass()
    {
        Vector3 centerOfMass = Vector3.zero;
        float totalMass = 0f;
        foreach (ReferenceBodypart rbp in referenceBodyparts)
        {
            centerOfMass += rbp.rb.worldCenterOfMass * rbp.rb.mass;
            totalMass += rbp.rb.mass;
        }
        if (totalMass > 0f)
        {
            centerOfMass /= totalMass; // Normalize by total mass
        }
        return centerOfMass;
    }
}
