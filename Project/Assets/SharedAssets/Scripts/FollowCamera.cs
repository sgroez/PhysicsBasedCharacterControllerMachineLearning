using System;
using UnityEngine;

public class FollowCamera : MonoBehaviour
{
    public UserController userController;
    public float offset;

    void FixedUpdate()
    {
        Vector3 direction = Vector3.forward;
        if (userController == null)
        {
            throw new Exception("Missing User Controller");
        }
        else if (userController.worldAxisMode)
        {
            direction = Vector3.up;
        }
        else
        {
            direction = userController.rotation * -Vector3.forward;
            direction = new Vector3(direction.x, 0f, direction.z);
        }
        transform.position = userController.root.position + (direction * offset);
        transform.LookAt(userController.root);
    }
}
