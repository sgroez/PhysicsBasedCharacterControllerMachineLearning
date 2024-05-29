using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestVelocity : MonoBehaviour
{
    public Vector3 velocity;
    Rigidbody rb;
    void Start()
    {
        rb = GetComponent<Rigidbody>();
    }

    void Update()
    {
        rb.velocity = velocity;
    }
}