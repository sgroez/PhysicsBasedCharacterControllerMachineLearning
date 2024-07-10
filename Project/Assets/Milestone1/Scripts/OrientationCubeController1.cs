using UnityEngine;

/// <summary>
/// Utility class to allow a stable observation platform.
/// </summary>
public class OrientationCubeController1 : MonoBehaviour
{
    public Transform root;
    public Transform target;

    //Update position and Rotation
    public void UpdateOrientation()
    {
        var dirVector = target.position - transform.position;
        dirVector.y = 0; //flatten dir on the y. this will only work on level, uneven surfaces
        var lookRot =
            dirVector == Vector3.zero
                ? Quaternion.identity
                : Quaternion.LookRotation(dirVector); //get our look rot to the target

        //UPDATE ORIENTATION CUBE POS & ROT
        transform.SetPositionAndRotation(root.position, lookRot);
    }

    void FixedUpdate()
    {
        UpdateOrientation();
    }
}
