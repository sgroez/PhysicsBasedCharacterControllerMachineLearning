using System;
using System.Collections;
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

[RequireComponent(typeof(Rigidbody))]
public class Bodypart : MonoBehaviour
{
    [Header("Bodypart Config")]
    public PhysicsConfig physicsConfig;
    public bool triggerTochingGroundEvent;

    [Header("Bodypart Info")]
    public float currentStrength;
    public bool isTouchingGround = false;
    public UnityEvent onTouchingGround;

    /*
    * hidden script variables and references
    */
    [HideInInspector] public Rigidbody rb;
    [HideInInspector] public ConfigurableJoint joint;

    [HideInInspector] public Vector3 startingPos;
    [HideInInspector] public Quaternion startingRot;
    [HideInInspector] public Quaternion startingRotLocal;

    [HideInInspector] public Vector3 dof; //degrees of freedom

    void Awake()
    {
        //setup component references
        rb = GetComponent<Rigidbody>();
        if (TryGetComponent(out ConfigurableJoint foundJoint))
        {
            joint = foundJoint;
        }

        //save starting position and rotations
        startingPos = transform.position;
        startingRot = transform.rotation;
        startingRotLocal = transform.localRotation;

        //setup rigidbody
        rb.maxAngularVelocity = physicsConfig.k_MaxAngularVelocity;

        //setup base degrees of freedom
        dof = new Vector3(0f, 0f, 0f);

        if (joint)
        {
            //setup joint
            var jd = new JointDrive
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
        //setup events
        onTouchingGround = new UnityEvent();
    }

    public void Reset()
    {
        Reset(startingPos, startingRot);
    }

    public void Reset(Vector3 resetPos, Quaternion resetRot)
    {
        rb.transform.position = resetPos;
        rb.transform.rotation = resetRot;
        rb.velocity = Vector3.zero;
        rb.angularVelocity = Vector3.zero;
        isTouchingGround = false;
    }

    public void SetJointTargetRotation(float x, float y, float z)
    {
        Vector3 targetRotationEuler = CalculateJointTargetRotationEuler(x, y, z);
        joint.targetRotation = Quaternion.Euler(targetRotationEuler);
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

    public Vector3 CalculateJointTargetRotationEuler(float x, float y, float z)
    {
        x = (x + 1f) * 0.5f;
        y = (y + 1f) * 0.5f;
        z = (z + 1f) * 0.5f;

        float lowAngularXLimit = joint.angularXMotion == ConfigurableJointMotion.Limited ? joint.lowAngularXLimit.limit : -180f;
        float highAngularXLimit = joint.angularXMotion == ConfigurableJointMotion.Limited ? joint.highAngularXLimit.limit : 180f;
        float angularYLimit = joint.angularYMotion == ConfigurableJointMotion.Limited ? joint.angularYLimit.limit : 180f;
        float angularZLimit = joint.angularZMotion == ConfigurableJointMotion.Limited ? joint.angularZLimit.limit : 180f;

        var xRot = Mathf.Lerp(lowAngularXLimit, highAngularXLimit, x);
        var yRot = Mathf.Lerp(-angularYLimit, angularYLimit, y);
        var zRot = Mathf.Lerp(-angularZLimit, angularZLimit, z);

        return new Vector3(xRot, yRot, zRot);
    }

    void OnCollisionEnter(Collision col)
    {
        if (col.transform.CompareTag("ground"))
        {
            isTouchingGround = true;
            if (triggerTochingGroundEvent)
            {
                onTouchingGround.Invoke();
            }
        }
    }

    void OnCollisionExit(Collision col)
    {
        if (col.transform.CompareTag("ground"))
        {
            isTouchingGround = false;
        }
    }
}
