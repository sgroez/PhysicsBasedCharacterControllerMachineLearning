using UnityEngine;
using Unity.Barracuda;

public class UserControllerMultiDirection : UserController
{
    public WalkerMultidirection agent;

    public override void FixedUpdate()
    {
        //Einlesen Tastatur Input
        float inputHor = Input.GetAxis("Horizontal");
        float inputVert = Input.GetAxis("Vertical");

        //Einlesen Maus Input
        float mouseX = Input.GetAxis("Mouse X");
        rotAngle += mouseX;

        //NN Modell wechseln
        agent.targetWalkingSpeed = 5f;
        if (inputVert != 0)
        {
            if (inputVert > 0)
            {
                agent.direction = Direction.Forward;
            }
            else
            {
                agent.direction = Direction.Backward;
            }
        }
        else if (inputHor != 0)
        {
            if (inputHor > 0)
            {
                agent.direction = Direction.Left;
            }
            else
            {
                agent.direction = Direction.Right;
            }
        }
        /* else
        {
            agent.targetWalkingSpeed = 0f;
            agent.SetModel("Walker", modelStanding);
        } */

        //Berechnung der Rotation
        Quaternion rotation = Quaternion.AngleAxis(rotAngle, rotationAxis);

        //Anwendung der Rotation auf Richtungsvektoren
        Vector3 directionForward = rotation * startForward;
        Vector3 directionRight = rotation * startRight;

        //Setzen der Zielposition
        target.position = root.position + directionForward * inputVert + directionRight * inputHor;

        //Setzen der Kamera Position
        cam.position = root.position + directionForward * camOffset;
        cam.LookAt(root);
    }
}
