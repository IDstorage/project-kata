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

        GUILayout.Label("Offset");
        EditorGUI.indentLevel++;
        {
            for (int i = self.Lines.Count - 1; i >= 0; --i)
            {
                self.Lines[i].offset = EditorGUILayout.Vector3Field($"{i + 1}", self.Lines[i].offset);
            }
        }
        EditorGUI.indentLevel--;

        GUILayout.Space(20);

        EditorGUILayout.BeginHorizontal("box");

        if (GUILayout.Button($"{(self.showHandles ? "Hide" : "Show")} Handles", GUILayout.Width(160), GUILayout.Height(30)))
        {
            self.showHandles = !self.showHandles;
        }

        GUILayout.Space(5);

        if (GUILayout.Button($"{(self.showGizmos ? "Hide" : "Show")} Gizmos", GUILayout.Width(120), GUILayout.Height(30)))
        {
            self.showGizmos = !self.showGizmos;
        }

        GUILayout.FlexibleSpace();

        if (GUILayout.Button("Clear", GUILayout.Width(60), GUILayout.Height(30)))
        {
            self.Clear();
        }

        EditorGUILayout.EndHorizontal();
    }
}
#endif