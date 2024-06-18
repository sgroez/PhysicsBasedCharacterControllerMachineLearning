using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

/**********************************************************************************************
* CHANGELOG
* Removed unused targetContact reference
* Removed jointDriveControllerReference
* Added physics config to BodypartSimple
* Changed BodypartSimple class to extend MonoBehaviour
* Moved SetupBodypart into BodypartSimple Awake function
* Removed JointDriveController
* Removed unnecessary Bodypart reference from Reset function
* Added degrees of freedom variable
* Merged Ground Contact into BodypartSimple
* Removed unused debug variables
* Added Event for touching ground
* Added Require Component Rigidbody
**********************************************************************************************/

[RequireComponent(typeof(Rigidbody))]
public class BodypartSimple : MonoBehaviour
{
    [Header("Body Part Info")]
    public PhysicsConfig physicsConfig;
    public Rigidbody rb;
    public ConfigurableJoint joint;
    [HideInInspector] public Vector3 startingPos;
    [HideInInspector] public Quaternion startingRot;

    [Header("Ground Contact")]
    public bool triggerTouchingGroundEvent;
    public UnityEvent onTouchingGround;
    public bool touchingGround;

    [HideInInspector] public float currentStrength;
    [HideInInspector] public Vector3 dof; //degrees of freedom

    void Awake()
    {
        rb = GetComponent<Rigidbody>();
        if (TryGetComponent(out ConfigurableJoint foundJoint))
        {
            joint = foundJoint;
        }
        startingPos = transform.position;
        startingRot = transform.rotation;

        rb.maxAngularVelocity = physicsConfig.k_MaxAngularVelocity;

        //setup base degrees of freedom
        dof = new Vector3(0f, 0f, 0f);

        if (joint)
        {
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
        onTouchingGround = new UnityEvent();
    }

    /// <summary>
    /// Reset body part to initial configuration.
    /// </summary>
    public void Reset()
    {
        rb.transform.position = startingPos;
        rb.transform.rotation = startingRot;
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
        if (touchingGround && triggerTouchingGroundEvent)
        {
            onTouchingGround.Invoke();
        }
    }
}