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
                agent.orientation = Orientation.Forward;
            }
            else
            {
                agent.orientation = Orientation.Backward;
            }
        }
        else if (inputHor != 0)
        {
            if (inputHor > 0)
            {
                agent.orientation = Orientation.Left;
            }
            else
            {
                agent.orientation = Orientation.Right;
            }
        }
        /* else
        {
            agent.targetWalkingSpeed = 0f;
            agent.SetModel("Walker", modelStanding);
        } */

        //Berechnung der Rotation
        Quaternion rotation = Quaternion.AngleAxis(rotAngle, Vector3.up);

        //Anwendung der Rotation auf Richtungsvektoren
        Vector3 directionForward = rotation * Vector3.forward;
        Vector3 directionRight = rotation * Vector3.right;

        //Setzen der Zielposition
        target.position = root.position + directionForward * inputVert + directionRight * inputHor;
    }
}
