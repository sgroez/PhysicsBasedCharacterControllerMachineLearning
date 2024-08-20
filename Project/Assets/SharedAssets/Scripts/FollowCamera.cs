using System;
using UnityEngine;

public class FollowCamera : MonoBehaviour
{
    public UserController userController;
    public Transform followObject;
    public Orientation orientation;
    public float offset;

    void FixedUpdate()
    {
        Vector3 direction = Vector3.forward;
        if (userController != null)
        {
            if (userController.worldAxisMode)
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
        else
        {
            direction = followObject.forward;
            switch (orientation)
            {
                case Orientation.Right:
                    direction = followObject.right;
                    break;
                case Orientation.Left:
                    direction = -followObject.right;
                    break;
                case Orientation.Backward:
                    direction = -followObject.forward;
                    break;
            }
            direction.y = 0f;
            transform.position = followObject.position + (direction * offset);
            transform.LookAt(followObject.position);
        }
    }
}
