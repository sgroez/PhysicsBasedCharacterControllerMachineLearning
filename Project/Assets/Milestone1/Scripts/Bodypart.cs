using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.MLAgents;


[System.Serializable]
public class BodypartConfig
{
    public float maxJointSpring;
    public float jointDampen;
    public float maxJointForceLimit;
    public float k_MaxAngularVelocity;

}

public class Bodypart
{
    [Header("Body Part Info")]
    [Space(10)]
    public ConfigurableJoint joint;
    public Rigidbody rb;
    [HideInInspector] public Vector3 startingPos;
    [HideInInspector] public Quaternion startingRot;
    [HideInInspector] public BodypartConfig config;
    [HideInInspector] public Vector3 dof = new Vector3(0f, 0f, 0f);

    [Header("Ground & Target Contact")]
    [Space(10)]
    public GroundContact groundContact;

    public Bodypart(Transform t, BodypartConfig config, Agent agent)
    {
        if (t.TryGetComponent(out Rigidbody foundRb))
        {
            rb = foundRb;
        }
        else
        {
            throw new Exception("Rigidbody missing on object: " + t.name);
        }
        if (t.TryGetComponent(out ConfigurableJoint foundJoint))
        {
            joint = foundJoint;
        }
        startingPos = t.position;
        startingRot = t.rotation;
        this.config = config;
        rb.maxAngularVelocity = config.k_MaxAngularVelocity;

        if (joint)
        {
            var jd = new JointDrive
            {
                positionSpring = config.maxJointSpring,
                positionDamper = config.jointDampen,
                maximumForce = config.maxJointForceLimit
            };
            joint.slerpDrive = jd;
            //calculate degrees of freedom
            if (joint.lowAngularXLimit.limit != 0 || joint.highAngularXLimit.limit != 0)
            {
                dof.x = 1;
            }
            if (joint.angularYLimit.limit != 0)
            {
                dof.y = 1;
            }
            if (joint.angularZLimit.limit != 0)
            {
                dof.z = 1;
            }
        }

        if (t.TryGetComponent(out GroundContact foundGroundContact))
        {
            groundContact = foundGroundContact;
            groundContact.agent = agent;
        }
    }

    //Reset bodypart pos, rot, velocities and contact variables
    public void Reset()
    {
        rb.transform.position = startingPos;
        rb.transform.rotation = startingRot;
        rb.velocity = Vector3.zero;
        rb.angularVelocity = Vector3.zero;
        if (groundContact)
        {
            groundContact.touchingGround = false;
        }
    }

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
        var rawVal = (strength + 1f) * 0.5f * config.maxJointForceLimit;
        var jd = new JointDrive
        {
            positionSpring = config.maxJointSpring,
            positionDamper = config.jointDampen,
            maximumForce = rawVal
        };
        joint.slerpDrive = jd;
    }
}
