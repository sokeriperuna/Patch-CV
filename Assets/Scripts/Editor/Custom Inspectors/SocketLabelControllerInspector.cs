using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

// I should have named these classes better...

[CustomEditor(typeof(SocketLabelController))]
public class SocketLabelControllerInspector : Editor
{
    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();


        SocketLabelController label = (SocketLabelController)target;

        EditorGUILayout.BeginHorizontal();

        if (GUILayout.Button("Force update"))
            label.UpdateLabelAll();

        if (GUILayout.Button("Force update all"))
        {
            SocketLabelController[] labels;
            labels = MonoBehaviour.FindObjectsOfType<SocketLabelController>();
            foreach (SocketLabelController l in labels)
                l.UpdateLabelAll();
        }

        EditorGUILayout.EndHorizontal();

        EditorGUILayout.Space();

        EditorGUILayout.BeginHorizontal();
        if (GUILayout.Button("Reinitialize"))
            label.Start();

        if (GUILayout.Button("Reinitialize all"))
        {
            SocketLabelController[] labels;
            labels = MonoBehaviour.FindObjectsOfType<SocketLabelController>();
            foreach (SocketLabelController l in labels)
                l.Start();
        }
        EditorGUILayout.EndHorizontal();
    }
}

