using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using UnityEditor;
#if UNITY_EDITOR
[InitializeOnLoad]
#endif
public class VersionController : MonoBehaviour
{
    public TMP_Text VersionText;

    public float currentVersion;

    public void IncrementCurrentVersion()
    {
        currentVersion += 1;
        VersionText.text = currentVersion.ToString();
    }

    private void Start()
    {
        VersionText.text = currentVersion.ToString();
    }
}

#if UNITY_EDITOR

[CustomEditor(typeof(VersionController))]
public class VersionControllerEditor : Editor
{
    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();

        VersionController myScript = (VersionController)target;

        if (GUILayout.Button("Increment Version"))
        {
            myScript.IncrementCurrentVersion();
        }
    }
}


//[InitializeOnLoad]
//public class SceneSaveCallback : MonoBehaviour
//{
//    static SceneSaveCallback()
//    {
       
//    }

//    private static void OnPlayModeStateChanged(PlayModeStateChange state)
//    {
//        if (state == PlayModeStateChange.ExitingEditMode || state == PlayModeStateChange.EnteredEditMode)
//        {
//            // This code will be executed when entering or exiting Play mode.
//            Debug.Log("Play mode state changed.");
//        }
//    }

//    private static void OnHierarchyChanged()
//    {
//        // This code will be executed when the hierarchy changes.
//        Debug.Log("Hierarchy changed.");
//    }

//    private static void OnScriptsReloaded()
//    {
//        // This code will be executed when scripts finish reloading.
//        Debug.Log("Scripts reloaded.");
//    }
//}

#endif