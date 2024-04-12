using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RemoteTrigger : MonoBehaviour
{
    private float triggeredAt;
    private void OnTriggerEnter(Collider col)
    {
        if (!col.transform.CompareTag("target")) return;
        if (Time.time > triggeredAt + 0.1f)
        {
            triggeredAt = Time.time;
        }
        col.GetComponent<TargetControllerV2>().TriggerRemote(triggeredAt);
    }

    private void OnTriggerStay(Collider col)
    {
        if (col.transform.CompareTag("target"))
        {
            col.GetComponent<TargetControllerV2>().TriggerRemote(triggeredAt);
        }
    }
}
