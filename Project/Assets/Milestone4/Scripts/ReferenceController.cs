using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ReferenceController : MonoBehaviour
{
    [Header("Animator Of Reference Character")]
    public string animationName;
    public Transform referenceRoot;
    private Animator animator;

    [Header("Min And Max to sample phase start from")]
    public float phaseStartMin;
    public float phaseStartMax;

    [Header("Enable debug")]
    public bool enableDebug = false;

    private Vector3 startingPos;

    /*
    * Reference Bodyparts list
    */
    [HideInInspector] public List<ReferenceBodypart> referenceBodyparts = new List<ReferenceBodypart>();

    void OnEnable()
    {
        // Get a reference to the Animator component
        animator = GetComponent<Animator>();
        startingPos = transform.position;
        foreach (ReferenceBodypart rbp in referenceRoot.GetComponentsInChildren<ReferenceBodypart>())
        {
            referenceBodyparts.Add(rbp);
        }
    }

    public void ResetReference()
    {
        transform.position = startingPos;
        //call the Play method of the Animator to start playing the animation at a specific point
        float randomPhase = Random.Range(phaseStartMin, phaseStartMax);
        animator.Play(animationName, -1, randomPhase);
        //reset reference bodyparts on next frame when animation has started from random phase
        StartCoroutine(ResetBodypartsOnNextFrame());
    }

    public IEnumerator ResetBodypartsOnNextFrame()
    {
        //wait for the next frame
        yield return null;

        //code to execute on the next frame
        foreach (ReferenceBodypart rbp in referenceBodyparts)
        {
            rbp.Reset();
        }
    }

    public float GetCurrentPhase()
    {
        float phase = animator.GetCurrentAnimatorStateInfo(0).normalizedTime % 1;
        return phase;
    }

    void Update()
    {
        if (!enableDebug) return;
        if (Input.GetKeyDown(KeyCode.Space))
        {
            ResetReference();
        }
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
