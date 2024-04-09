using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnEnvironment : MonoBehaviour
{
    public bool hasInitialEnvironment;
    public GameObject environmentPrefab;
    public WalkerAgent agent;
    void Start()
    {
        float environmentCount = agent.m_ResetParams.GetWithDefault("environment_count", 1f);
        for (int i = 0; i < environmentCount; i++)
        {
            if (hasInitialEnvironment && i == 0) continue;
            GameObject environment = GameObject.Instantiate(environmentPrefab, new Vector3(0, 0, i * 50), Quaternion.identity);
            environment.transform.parent = transform;
        }
    }
}
