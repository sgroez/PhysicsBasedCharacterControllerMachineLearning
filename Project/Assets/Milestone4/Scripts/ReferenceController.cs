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
    }

    public void ResetReference()
    {
        // Call the Play method of the Animator to start playing the animation at a specific point
        float randomPhase = Random.Range(phaseStartMin, phaseStartMax);
        animator.Play(animationName, -1, randomPhase);
    }

    public float GetCurrentPhase()
    {
        float phase = animator.GetCurrentAnimatorStateInfo(0).normalizedTime % 1;
        return phase;
    }
}
