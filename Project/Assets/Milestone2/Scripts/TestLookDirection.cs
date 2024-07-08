using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestLookDirection : MonoBehaviour
{
    void FixedUpdate()
    {
        Debug.Log($"look direction: {transform.forward}");
    }

    void OnDrawGizmos()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawRay(transform.position, transform.forward);
    }
}
