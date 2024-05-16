using UnityEngine;

public class TestAnimatorPlay : MonoBehaviour
{
    public Animator animator;
    public string animationName;

    public float phaseStartMin;
    public float phaseStartMax;

    void Start()
    {
        // Get a reference to the Animator component
        animator = GetComponent<Animator>();
    }

    void Update()
    {
        // Check for input or condition to trigger the animation
        if (Input.GetKeyDown(KeyCode.Space))
        {
            // Call the Play method of the Animator to start playing the animation at a specific point
            float randomPhase = Random.Range(phaseStartMin, phaseStartMax);
            Debug.Log(randomPhase);
            animator.Play(animationName, -1, randomPhase);
        }
    }
}
