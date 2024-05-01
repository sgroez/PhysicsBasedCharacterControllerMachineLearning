using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System.Linq;

public static class FindMissingScripts
{
    [MenuItem("My Menu/Find missing scripts in project")]

    static void FindMissingScriptsInProjectMenuItem()
    {
        string[] prefabPaths = AssetDatabase.GetAllAssetPaths().Where(path => path.EndsWith(".prefab", System.StringComparison.OrdinalIgnoreCase)).ToArray();

        foreach (string path in prefabPaths)
        {
            GameObject prefab = AssetDatabase.LoadAssetAtPath<GameObject>(path);

            foreach (Component component in prefab.GetComponentsInChildren<Component>())
            {
                if (component == null)
                {
                    Debug.Log("Prefab found with missing script: " + path, prefab);
                    break;
                }
            }
        }
    }
}
