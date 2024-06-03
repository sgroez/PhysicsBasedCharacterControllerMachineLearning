using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class TestJoint
{
    public string name;
    public ConfigurableJoint joint;
    public Vector3 enabledAxis;
    [Range(0f, 1f)] public float extension;
    [HideInInspector] public float lastExtension;

    public TestJoint(string name, ConfigurableJoint joint)
    {
        this.name = name;
        this.joint = joint;
        enabledAxis = Vector3.zero;
        extension = 0.5f;
        lastExtension = 0f;
    }
}

public class TestJointLimits : MonoBehaviour
{
    public Rigidbody root;
    public List<TestJoint> testJoints = new List<TestJoint>();

    void Start()
    {
        //freeze root
        root.constraints = RigidbodyConstraints.FreezeAll;
        //get joint and joint limits from current game object
        ConfigurableJoint[] jointsInChildren = GetComponentsInChildren<ConfigurableJoint>();
        foreach (ConfigurableJoint joint in jointsInChildren)
        {
            testJoints.Add(new TestJoint(joint.gameObject.name, joint));
        }
    }

    void Update()
    {
        foreach (TestJoint testJoint in testJoints)
        {
            UpdateJointRotation(testJoint);
        }
    }

    void UpdateJointRotation(TestJoint testJoint)
    {
        //reduce duplicate updates
        if (testJoint.extension == testJoint.lastExtension) return;

        //get enabled axis values from input
        float enabledX = testJoint.enabledAxis.x > 0f ? 1f : 0f;
        float enabledY = testJoint.enabledAxis.y > 0f ? 1f : 0f;
        float enabledZ = testJoint.enabledAxis.z > 0f ? 1f : 0f;
        //get current joint limits
        float lowXLimit = testJoint.joint.lowAngularXLimit.limit;
        float highXLimit = testJoint.joint.highAngularXLimit.limit;
        float yLimit = testJoint.joint.angularYLimit.limit;
        float zLimit = testJoint.joint.angularZLimit.limit;
        //lerp between high and low joint limits depending on current phase
        float xRot = enabledX * Mathf.Lerp(lowXLimit, highXLimit, testJoint.extension);
        float yRot = enabledY * Mathf.Lerp(-yLimit, yLimit, testJoint.extension);
        float zRot = enabledZ * Mathf.Lerp(-zLimit, zLimit, testJoint.extension);
        //set new target rotation
        testJoint.joint.targetRotation = Quaternion.Euler(xRot, yRot, zRot);

        //set new last extension
        testJoint.lastExtension = testJoint.extension;
    }
}
