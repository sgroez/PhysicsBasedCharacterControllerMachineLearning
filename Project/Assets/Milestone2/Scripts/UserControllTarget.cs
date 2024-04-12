using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UserControllTarget : MonoBehaviour
{
    [SerializeField] private Transform target;

    void FixedUpdate()
    {
        Vector3 hor = transform.right * Input.GetAxis("Horizontal");
        hor.y = 0f;
        Vector3 vert = transform.forward * Input.GetAxis("Vertical");
        vert.y = 0f;
        target.transform.position = transform.position + hor + vert;
    }
}
