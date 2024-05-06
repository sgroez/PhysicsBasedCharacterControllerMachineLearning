using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestJointLimits : MonoBehaviour
{
    public Vector3 testAxis;
    public float speed = .8f;
    private ConfigurableJoint joint;
    private float lowXLimit;
    private float highXLimit;
    private float yLimit;
    private float zLimit;

    void Start()
    {
        //get joint and joint limits from current game object
        joint = GetComponent<ConfigurableJoint>();
    }

    void FixedUpdate()
    {
        //get enabled axis values from input
        float enabledX = testAxis.x > 0f ? 1f : 0f;
        float enabledY = testAxis.y > 0f ? 1f : 0f;
        float enabledZ = testAxis.z > 0f ? 1f : 0f;
        //get current time * speed
        float timer = Time.fixedTime * speed;
        //ping pong between 0 and 1 depending on timer
        float currentPhase = Mathf.PingPong(timer, 1f);
        //get current joint limits
        lowXLimit = joint.lowAngularXLimit.limit;
        highXLimit = joint.highAngularXLimit.limit;
        yLimit = joint.angularYLimit.limit;
        zLimit = joint.angularZLimit.limit;
        //lerp between high and low joint limits depending on current phase
        float xRot = enabledX * Mathf.Lerp(lowXLimit, highXLimit, currentPhase);
        float yRot = enabledY * Mathf.Lerp(-yLimit, yLimit, currentPhase);
        float zRot = enabledZ * Mathf.Lerp(-zLimit, zLimit, currentPhase);
        //set new target rotation
        joint.targetRotation = Quaternion.Euler(xRot, yRot, zRot);
    }
}
