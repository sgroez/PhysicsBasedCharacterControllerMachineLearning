using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
[RequireComponent(typeof(MovementTrajectory))]
public class RandomMoveLookDirections : MonoBehaviour
{
    public float strength = 10;
    public float torqueStrength = 10;
    public Transform moveToGoal;
    public Transform lookAtGoal;
    int updateCount = 0;
    Vector3 velocity;

    Rigidbody rb;
    MovementTrajectory moveTraj;
    void Start()
    {
        rb = GetComponent<Rigidbody>();
        moveTraj = GetComponent<MovementTrajectory>();
    }

    void FixedUpdate()
    {
        if (moveTraj.points != null)
        {
            Vector3[] currentPoints = moveTraj.GetCurrentPoints();
            Vector3 goalPosition = moveTraj.moveToGoal.position;
            if (Vector3.Distance(transform.position, goalPosition) - 0.2f < Vector3.Distance(currentPoints[0], goalPosition))
            {
                moveTraj.ReachedPoint();
                currentPoints = moveTraj.GetCurrentPoints();
            }
            rb.AddForce((currentPoints[0] - transform.position).normalized * strength);
            rb.AddTorque(Vector3.Cross((currentPoints[3] - transform.position).normalized, transform.forward) * torqueStrength);
        }
        if (updateCount % 100 == 0)
        {
            moveToGoal.position = new Vector3(Random.Range(-10, 10), transform.position.y, Random.Range(-10, 10));
            lookAtGoal.position = new Vector3(Random.Range(-10, 10), transform.position.y, Random.Range(-10, 10));
            moveTraj.UpdateCurve(moveToGoal.position, rb.velocity, transform.forward, lookAtGoal.position);
        }
        updateCount++;
    }
}
