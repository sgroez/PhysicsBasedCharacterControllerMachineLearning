using UnityEngine;

public class BodyweightMeasure : MonoBehaviour
{
    void Start()
    {
        Bodypart[] bodyparts = GetComponentsInChildren<Bodypart>();
        float weight = 0f;
        foreach (Bodypart bp in bodyparts)
        {
            weight += bp.rb.mass;
        }
        Debug.Log($"total body weight: {weight}");
    }
}
