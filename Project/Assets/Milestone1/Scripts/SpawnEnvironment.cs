using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnEnvironment : MonoBehaviour
{
    public bool hasInitialEnvironment;
    public GameObject environmentPrefab;
    public int environmentCount;
    void Start()
    {
        for (int i = 0; i < environmentCount; i++)
        {
            if (hasInitialEnvironment && i == 0) continue;
            GameObject environment = GameObject.Instantiate(environmentPrefab, new Vector3(0, 0, i * 50), Quaternion.identity);
            environment.transform.parent = transform;
        }
    }
}
