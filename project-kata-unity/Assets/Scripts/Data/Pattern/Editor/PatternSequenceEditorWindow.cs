using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public class PatternSequenceEditor : EditorWindow
{
    private static PatternSequenceEditor window;
    private static Vector2 drawableSize;

    [MenuItem("Tools/Pattern Sequence Editor")]
    private static void ShowWindow()
    {
        if (window != null)
        {
            window.Close();
            window = null;
        }
        window = EditorWindow.CreateInstance<PatternSequenceEditor>();

        window.ShowAuxWindow();

        drawableSize = new Vector2(700F, 400F);
        window.minSize = window.maxSize = drawableSize;

        // Center
        var mainWindow = EditorGUIUtility.GetMainWindowPosition();
        var current = window.position;

        current.x = (mainWindow.x + mainWindow.width * 0.5f) - current.width * 0.5f;
        current.y = (mainWindow.y + mainWindow.height * 0.5f) - current.height * 0.5f - 100F;

        window.position = current;

        window.titleContent = new GUIContent("Pattern Sequence Editor");
    }
}
