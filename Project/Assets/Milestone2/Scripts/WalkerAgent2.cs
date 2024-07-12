using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using Unity.MLAgents;
using Unity.MLAgents.Sensors;
using Unity.MLAgents.Actuators;

public class WalkerAgent2 : WalkerAgent1
{
    [Header("Target To Look Towards")]
    public Transform lookTarget;
    public float durationLookAtTarget = 2f;
    public float lookAtTargetMargin = 1f;
    public UnityEvent onLookedAtTarget = new UnityEvent();
    OrientationCubeController1 lookOrientationCube;
    bool isLookingAtTarget = false;
    float startetLookingAtTarget = 0f;

    int reachedLookTargets = 0;

    public override void Initialize()
    {
        //init orientation object
        GameObject lookOrientationObject = new GameObject("LookOrientationObject");
        lookOrientationObject.transform.parent = transform;
        lookOrientationCube = lookOrientationObject.AddComponent<OrientationCubeController1>();
        lookOrientationCube.root = root;
        lookOrientationCube.target = lookTarget;
        base.Initialize();
    }

    public override void OnEpisodeBegin()
    {
        base.OnEpisodeBegin();

        //record then reset look targets reached
        RecordStat("Environment/ReachedLookTargets", reachedLookTargets);
        reachedLookTargets = 0;
    }

    // added look at target position
    public override void CollectObservations(VectorSensor sensor)
    {
        var cubeForward = walkOrientationCube.transform.forward;

        //velocity we want to match
        var velGoal = cubeForward * targetWalkingSpeed;
        //ragdoll's avg vel
        var avgVel = GetAvgVelocity();

        //current ragdoll velocity. normalized
        sensor.AddObservation(Vector3.Distance(velGoal, avgVel));
        //avg body vel relative to cube
        sensor.AddObservation(walkOrientationCube.transform.InverseTransformDirection(avgVel));
        //vel goal relative to cube
        sensor.AddObservation(walkOrientationCube.transform.InverseTransformDirection(velGoal));

        //rotation deltas
        sensor.AddObservation(Quaternion.FromToRotation(root.forward, cubeForward));
        sensor.AddObservation(Quaternion.FromToRotation(head.forward, cubeForward));

        //Position of target position relative to cube
        sensor.AddObservation(walkOrientationCube.transform.InverseTransformPoint(target.transform.position));

        //Position of look target position relative to cube
        sensor.AddObservation(walkOrientationCube.transform.InverseTransformPoint(lookTarget.transform.position));

        foreach (Bodypart bp in bodyparts)
        {
            CollectObservationBodyPart(bp, sensor);
        }
    }

    public override void FixedUpdate()
    {
        var cubeForward = walkOrientationCube.transform.forward;

        // Set reward for this step according to mixture of the following elements.
        // a. Match target speed
        //This reward will approach 1 if it matches perfectly and approach zero as it deviates
        var matchSpeedReward = GetMatchingVelocityReward(cubeForward * targetWalkingSpeed, GetAvgVelocity());
        RecordStat("Reward/MatchingVelocityReward", matchSpeedReward);

        //Check for NaNs
        if (float.IsNaN(matchSpeedReward))
        {
            throw new ArgumentException(
                "NaN in moveTowardsTargetReward.\n" +
                $" cubeForward: {cubeForward}\n" +
                $" root.velocity: {bodyparts[0].rb.velocity}\n" +
                $" maximumWalkingSpeed: {maxWalkingSpeed}"
            );
        }

        // b. Rotation alignment with target direction.
        //This reward will approach 1 if it faces the target direction perfectly and approach zero as it deviates
        var headForward = head.forward;
        headForward.y = 0;
        var lookCubeForward = lookOrientationCube.transform.forward;
        // var lookAtTargetReward = (Vector3.Dot(cubeForward, head.forward) + 1) * .5F;
        var lookAtTargetReward = (Vector3.Dot(lookCubeForward, headForward) + 1) * .5F;
        lookAtTargetReward = Mathf.Exp(lookAtTargetReward * 5 - 5);
        RecordStat("Reward/LookAtTargetReward", lookAtTargetReward);

        //Check for NaNs
        if (float.IsNaN(lookAtTargetReward))
        {
            throw new ArgumentException(
                "NaN in lookAtTargetReward.\n" +
                $" cubeForward: {cubeForward}\n" +
                $" head.forward: {head.forward}"
            );
        }

        //add head tilt reward to keep head level
        float headTilt = head.forward.y;
        float headTiltRewardUnclipped = 0f;
        if (headTilt > 0)
        {
            headTiltRewardUnclipped = headTilt / 0.2f - 1f;
        }
        else if (headTilt < 0)
        {
            headTiltRewardUnclipped = -headTilt / 0.2f - 1f;
        }
        float headTiltReward = -Mathf.Clamp(headTiltRewardUnclipped, 0, 1);
        RecordStat("Reward/HeadTiltReward", headTiltReward);

        AddReward(matchSpeedReward * lookAtTargetReward + headTiltReward);

        //add new value to moved distance in target direction
        distanceMovedInTargetDirection += GetDistanceMovedInTargetDirection();

        //check if agent was looking at target for duration
        CheckLookAtTarget();
    }

    void CheckLookAtTarget()
    {
        RaycastHit hit;
        if (Physics.SphereCast(head.position, lookAtTargetMargin, head.forward, out hit, 9f) && hit.collider.gameObject.CompareTag("lookTarget"))
        {
            if (isLookingAtTarget)
            {
                if (startetLookingAtTarget + durationLookAtTarget <= Time.fixedTime)
                {
                    onLookedAtTarget.Invoke();
                    isLookingAtTarget = false;
                    reachedLookTargets++;
                }
            }
            else
            {
                startetLookingAtTarget = Time.fixedTime;
                isLookingAtTarget = true;
            }
        }
        else
        {
            isLookingAtTarget = false;
        }
    }

    void OnDrawGizmos()
    {
        Gizmos.color = Color.yellow;
        Vector3 targetPosition = head.position + head.forward * 9f;
        if (isLookingAtTarget)
        {
            targetPosition = lookTarget.position;
        }
        Gizmos.DrawLine(head.position, targetPosition);
        Gizmos.DrawWireSphere(targetPosition, lookAtTargetMargin);
    }
}
