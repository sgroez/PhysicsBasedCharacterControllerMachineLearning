using UnityEngine;

public class TestRelativeRotation : MonoBehaviour
{
    public Transform child;
    void Start()
    {
        Debug.Log("local rotation: " + child.localRotation);
        Quaternion relativeRotation = Quaternion.Inverse(transform.rotation) * child.transform.rotation;
        Debug.Log("calculated relative rotation: " + relativeRotation);
    }
}
