using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.MLAgents;


[System.Serializable]
public class BodypartConfig
{
    public float maxJointSpring = 40000f;
    public float jointDampen = 5000f;
    public float maxJointForceLimit = 20000f;
    public float k_MaxAngularVelocity = 50f;

}

public class Bodypart : MonoBehaviour
{
    [Header("Body Part Info")]
    [Space(10)]
    public ConfigurableJoint joint;
    public Rigidbody rb;
    [HideInInspector] public Vector3 startingPos;
    [HideInInspector] public Quaternion startingRot;
    [HideInInspector] public Quaternion startingRotLocal;

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
        startingRotLocal = t.localRotation;
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
            if (joint.angularXMotion != ConfigurableJointMotion.Locked)
            {
                dof.x = 1;
            }
            if (joint.angularYMotion != ConfigurableJointMotion.Locked)
            {
                dof.y = 1;
            }
            if (joint.angularZMotion != ConfigurableJointMotion.Locked)
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
    public void Reset(Vector3 resetPos, Quaternion resetRot)
    {
        rb.transform.position = resetPos;
        rb.transform.rotation = resetRot;
        rb.velocity = Vector3.zero;
        rb.angularVelocity = Vector3.zero;
        if (groundContact)
        {
            groundContact.touchingGround = false;
        }
    }

    public void SetJointTargetRotation(float x, float y, float z)
    {
        Vector3 targetRotationEuler = CalculateJointTargetRotationEuler(x, y, z);
        joint.targetRotation = Quaternion.Euler(targetRotationEuler);
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
}
