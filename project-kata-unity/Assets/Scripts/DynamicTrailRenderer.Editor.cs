#if UNITY_EDITOR
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(DynamicTrailRenderer))]
public class DynamicTrailRendererEditor : Editor
{
    public override void OnInspectorGUI()
    {
        var self = target as DynamicTrailRenderer;
        base.OnInspectorGUI();

        GUILayout.Space(20);

        EditorGUILayout.BeginHorizontal("box");

        GUILayout.FlexibleSpace();

        if (GUILayout.Button($"{(self.showGizmos ? "Hide" : "Show")} Gizmos", GUILayout.Width(120), GUILayout.Height(30)))
        {
            self.showGizmos = !self.showGizmos;
        }

        GUILayout.Space(5);

        if (GUILayout.Button("Clear", GUILayout.Width(60), GUILayout.Height(30)))
        {
            self.Clear();
        }

        EditorGUILayout.EndHorizontal();
    }
}
#endif