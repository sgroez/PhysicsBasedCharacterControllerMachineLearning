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

    [HideInInspector] public Vector3 avgVelocity = Vector3.zero;
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

    void FixedUpdate()
    {
        Vector3 velSum = Vector3.zero;
        //update avg velocity
        foreach (ReferenceBodypart rbp in referenceBodyparts)
        {
            rbp.Update();
            velSum += rbp.velocity;
        }
        //avgVelocity = referenceBodyparts[0].velocity;
        avgVelocity = velSum / referenceBodyparts.Count;
        avgVelocity.y = 0f;
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
        foreach (ReferenceBodypart rbp in referenceBodyparts)
        {
            StartCoroutine(DelayResetReferenceBodypart(rbp));
        }
    }

    public float GetCurrentPhase()
    {
        float phase = animator.GetCurrentAnimatorStateInfo(0).normalizedTime % 1;
        return phase;
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

    /* void OnDrawGizmos()
    {
        // Draw a yellow sphere at the transform's position
        Gizmos.color = Color.yellow;
        if (referenceBodyparts.Count <= 0) return;
        Gizmos.DrawRay(referenceBodyparts[0].rb.position, avgVelocity);
    } */

    IEnumerator DelayResetReferenceBodypart(ReferenceBodypart rbp)
    {
        yield return new WaitForSeconds(.001f); // Wait for .001 second
        // Code to be executed after the delay
        rbp.Reset();
    }
}
