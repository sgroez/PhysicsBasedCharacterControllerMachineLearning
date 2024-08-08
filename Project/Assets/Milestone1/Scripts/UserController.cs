using UnityEngine;

public class UserController : MonoBehaviour
{
    public Transform root;
    public Transform head;
    public Transform target;
    public bool worldAxisMode = false;
    public Quaternion rotation = Quaternion.identity;

    protected float rotAngle = 0;

    public virtual void FixedUpdate()
    {
        //Einlesen Tastatur Input
        float inputHor = Input.GetAxis("Horizontal");
        float inputVert = Input.GetAxis("Vertical");

        Vector3 position;

        if (worldAxisMode)
        {
            //Einlesen Maus Position
            Vector3 mousePos = Input.mousePosition;

            //Maus Position normalisieren relativ zu Bildschirmauflösung
            float normalizedMouseX = 2 * (mousePos.x / Screen.width) - 1;
            float normalizedMouseY = 2 * (mousePos.y / Screen.height) - 1;
            mousePos = new Vector3(normalizedMouseX, 0, normalizedMouseY);

            //Berechnung der Rotation
            rotation = Quaternion.LookRotation(mousePos, Vector3.up);

            //Position berechnen
            position = root.position + Vector3.forward * inputVert + Vector3.right * inputHor;
        }
        else
        {
            //Einlesen Maus Input
            float mouseX = Input.GetAxis("Mouse X");
            rotAngle += mouseX;

            //Berechnung der Rotation
            rotation = Quaternion.AngleAxis(rotAngle, Vector3.up);

            //Anwendung der Rotation auf Richtungsvektoren
            Vector3 directionForward = rotation * Vector3.forward;
            Vector3 directionRight = rotation * Vector3.right;

            //Position berechnen
            position = root.position + directionForward * inputVert + directionRight * inputHor;
        }

        //Setzen der Zielposition
        target.position = position;
    }

    void OnDrawGizmos()
    {
        // Visualisiert Blickrichtung mit Ray
        Gizmos.color = Color.yellow;
        if (!head) return;
        Gizmos.DrawRay(head.position, rotation * Vector3.forward);
    }
}
