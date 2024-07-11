using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestSphereCast : MonoBehaviour
{
    public float radius = 1f;
    public float distance = 10f;
    public float duration = 1f;

    private bool isHittingTarget = false;
    private float startHittingTarget = 0f;

    private RaycastHit hit;
    void FixedUpdate()
    {
        if (Physics.SphereCast(transform.position, radius, transform.forward, out hit, distance) && hit.collider.gameObject.CompareTag("target"))
        {
            if (isHittingTarget)
            {
                if (startHittingTarget + duration <= Time.fixedTime)
                {
                    Debug.Log($"was looking at target for more than {duration} seconds.");
                }
            }
            else
            {
                startHittingTarget = Time.fixedTime;
                isHittingTarget = true;
            }
        }
        else
        {
            isHittingTarget = false;
        }
    }

    void OnDrawGizmos()
    {
        Gizmos.color = Color.yellow;
        Vector3 targetPosition = transform.position + transform.forward * distance;
        if (isHittingTarget)
        {
            targetPosition = hit.transform.position;
        }
        Gizmos.DrawLine(transform.position, targetPosition);
        Gizmos.DrawWireSphere(targetPosition, radius);
    }
}
