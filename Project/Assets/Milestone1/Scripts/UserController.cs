using UnityEngine;

public class UserController : MonoBehaviour
{
    public Transform root;
    public Transform cam;
    public float camOffset;
    public Transform target;
    public bool worldAxisMode = false;

    protected Vector3 rotationAxis = Vector3.up;
    protected Vector3 startForward;
    protected Vector3 startRight;
    protected float rotAngle = 0;

    void Start()
    {
        //Root Position als Startposition festhalten
        startForward = root.forward;
        startRight = root.right;
    }
    public virtual void FixedUpdate()
    {
        //Einlesen Tastatur Input
        float inputHor = Input.GetAxis("Horizontal");
        float inputVert = Input.GetAxis("Vertical");

        //Einlesen Maus Input
        float mouseX = Input.GetAxis("Mouse X");
        rotAngle += mouseX;

        Vector3 position = root.position;

        if (worldAxisMode)
        {
            position = root.position + Vector3.forward * inputVert + Vector3.right * inputHor;
            cam.position = root.position + Vector3.up * camOffset;
            cam.LookAt(root);
        }
        else
        {
            //Berechnung der Rotation
            Quaternion rotation = Quaternion.AngleAxis(rotAngle, rotationAxis);

            //Anwendung der Rotation auf Richtungsvektoren
            Vector3 directionForward = rotation * startForward;
            Vector3 directionRight = rotation * startRight;

            position = root.position + directionForward * inputVert + directionRight * inputHor;

            //Setzen der Kamera Position
            cam.position = root.position + directionForward * camOffset;
            cam.LookAt(root);
        }

        //Setzen der Zielposition
        target.position = position;
    }
}
