using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

[System.Serializable]
public class PhysicsConfig
{
    public float maxJointSpring = 40000f;
    public float jointDampen = 5000f;
    public float maxJointForceLimit = 20000f;
    public float k_MaxAngularVelocity = 50f;
}

/*
* Bodypart
* implementes functions to controll the joints and holds information about bodypart
*/
[RequireComponent(typeof(Rigidbody))]
public class Bodypart : MonoBehaviour
{
    [Header("Body Part Config")]
    public PhysicsConfig physicsConfig;
    public bool triggerTouchingGroundEvent;

    [Header("Ground Contact")]
    public UnityEvent onTouchedGround;
    [HideInInspector] public bool touchingGround;

    [Header("Target Contact")]
    public UnityEvent onTouchedTarget;

    //component references
    [HideInInspector] public Rigidbody rb;
    [HideInInspector] public ConfigurableJoint joint;

    //starting pos and rot
    [HideInInspector] public Vector3 startingPos;
    [HideInInspector] public Quaternion startingRot;

    //bodypart information
    [HideInInspector] public Vector3 dof; //degrees of freedom
    [HideInInspector] public float currentStrength;
    [HideInInspector] public float power;

    void Awake()
    {
        //get component references
        rb = GetComponent<Rigidbody>();
        if (TryGetComponent(out ConfigurableJoint foundJoint))
        {
            joint = foundJoint;
        }

        //save starting pos and rot
        startingPos = transform.position;
        startingRot = transform.rotation;

        //setup rigidbody max angular velocity
        rb.maxAngularVelocity = physicsConfig.k_MaxAngularVelocity;

        //setup base degrees of freedom
        dof = new Vector3(0f, 0f, 0f);

        if (joint)
        {
            //set joint settings from physics config
            JointDrive jd = new JointDrive
            {
                positionSpring = physicsConfig.maxJointSpring,
                positionDamper = physicsConfig.jointDampen,
                maximumForce = physicsConfig.maxJointForceLimit
            };
            joint.slerpDrive = jd;

            //calculate degrees of freedom
            dof.x = joint.angularXMotion != ConfigurableJointMotion.Locked ? 1 : 0;
            dof.y = joint.angularYMotion != ConfigurableJointMotion.Locked ? 1 : 0;
            dof.z = joint.angularZMotion != ConfigurableJointMotion.Locked ? 1 : 0;
        }
    }

    public void Initialize()
    {
        //setup events
        onTouchedGround = new UnityEvent();
        onTouchedTarget = new UnityEvent();
    }

    /// <summary>
    /// Reset body part to initial configuration.
    /// </summary>
    public void ResetTransform()
    {
        rb.transform.position = startingPos;
        rb.transform.rotation = startingRot;
        rb.velocity = Vector3.zero;
        rb.angularVelocity = Vector3.zero;
        touchingGround = false;
    }

    public void ResetTransform(Vector3 position, Quaternion rotation)
    {
        rb.transform.position = position;
        rb.transform.rotation = rotation;
        rb.velocity = Vector3.zero;
        rb.angularVelocity = Vector3.zero;
        touchingGround = false;
    }

    /// <summary>
    /// Apply torque according to defined goal `x, y, z` angle and force `strength`.
    /// </summary>
    public void SetJointTargetRotation(float x, float y, float z)
    {
        x = (x + 1f) * 0.5f;
        y = (y + 1f) * 0.5f;
        z = (z + 1f) * 0.5f;

        var xRot = Mathf.Lerp(joint.lowAngularXLimit.limit, joint.highAngularXLimit.limit, x);
        var yRot = Mathf.Lerp(-joint.angularYLimit.limit, joint.angularYLimit.limit, y);
        var zRot = Mathf.Lerp(-joint.angularZLimit.limit, joint.angularZLimit.limit, z);

        joint.targetRotation = Quaternion.Euler(xRot, yRot, zRot);
    }

    public void SetJointStrength(float strength)
    {
        var rawVal = (strength + 1f) * 0.5f * physicsConfig.maxJointForceLimit;
        var jd = new JointDrive
        {
            positionSpring = physicsConfig.maxJointSpring,
            positionDamper = physicsConfig.jointDampen,
            maximumForce = rawVal
        };
        joint.slerpDrive = jd;
        currentStrength = jd.maximumForce;
    }

    void OnCollisionEnter(Collision col)
    {
        if (col.transform.CompareTag("ground"))
        {
            touchingGround = true;
            if (triggerTouchingGroundEvent)
            {
                onTouchedGround.Invoke();
            }
        }
        if (col.transform.CompareTag("target"))
        {
            onTouchedTarget.Invoke();
        }
    }

    void OnCollisionExit(Collision other)
    {
        if (other.transform.CompareTag("ground"))
        {
            touchingGround = false;
        }
    }

    void FixedUpdate()
    {
        //calculate joint power
        if (joint)
        {
            Vector3 currentTorque = joint.currentTorque;
            Vector3 angularVel = rb.angularVelocity;

            power = Mathf.Abs(Vector3.Dot(currentTorque, angularVel));
        }
    }
}