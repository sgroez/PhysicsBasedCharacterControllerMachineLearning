using UnityEngine;

public class UserController : MonoBehaviour
{
    public Transform root;
    public Transform cam;
    public float camOffset;
    public Transform target;

    Vector3 rotationAxis = Vector3.up;
    Vector3 startForward;
    Vector3 startRight;
    float rotAngle = 0;

    void Start()
    {
        //Root Position als Startposition festhalten
        startForward = root.forward;
        startRight = root.right;
    }
    void FixedUpdate()
    {
        //Einlesen Tastatur Input
        float inputHor = Input.GetAxis("Horizontal");
        float inputVert = Input.GetAxis("Vertical");

        //Einlesen Maus Input
        float mouseX = Input.GetAxis("Mouse X");
        rotAngle += mouseX;

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
