using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class ToggleTestJoint
{
    public bool enabled;
    public string name;
    public ConfigurableJoint joint;

    public ToggleTestJoint(bool enabled, string name, ConfigurableJoint joint)
    {
        this.enabled = enabled;
        this.name = name;
        this.joint = joint;
    }
}

public class TestJointLimits : MonoBehaviour
{
    public Vector3 testAxis;
    public float speed = .8f;

    public Rigidbody root;
    public List<ToggleTestJoint> testJoints = new List<ToggleTestJoint>();
    private ConfigurableJoint joint;
    private float lowXLimit;
    private float highXLimit;
    private float yLimit;
    private float zLimit;

    void Start()
    {
        //freeze root
        root.constraints = RigidbodyConstraints.FreezeAll;
        //get joint and joint limits from current game object
        ConfigurableJoint[] jointsInChildren = GetComponentsInChildren<ConfigurableJoint>();
        foreach (ConfigurableJoint joint in jointsInChildren)
        {
            testJoints.Add(new ToggleTestJoint(false, joint.gameObject.name, joint));
        }
    }

    void Update()
    {
        foreach (ToggleTestJoint testJoint in testJoints)
        {
            UpdateJointRotation(testJoint);
        }
    }

    void UpdateJointRotation(ToggleTestJoint testJoint)
    {
        if (!testJoint.enabled) return;

        //get enabled axis values from input
        float enabledX = testAxis.x > 0f ? 1f : 0f;
        float enabledY = testAxis.y > 0f ? 1f : 0f;
        float enabledZ = testAxis.z > 0f ? 1f : 0f;
        //get current time * speed
        float timer = Time.time * speed;
        //ping pong between 0 and 1 depending on timer
        float currentPhase = Mathf.PingPong(timer, 1f);
        //get current joint limits
        lowXLimit = testJoint.joint.lowAngularXLimit.limit;
        highXLimit = testJoint.joint.highAngularXLimit.limit;
        yLimit = testJoint.joint.angularYLimit.limit;
        zLimit = testJoint.joint.angularZLimit.limit;
        //lerp between high and low joint limits depending on current phase
        float xRot = enabledX * Mathf.Lerp(lowXLimit, highXLimit, currentPhase);
        float yRot = enabledY * Mathf.Lerp(-yLimit, yLimit, currentPhase);
        float zRot = enabledZ * Mathf.Lerp(-zLimit, zLimit, currentPhase);
        //set new target rotation
        testJoint.joint.targetRotation = Quaternion.Euler(xRot, yRot, zRot);
    }
}
