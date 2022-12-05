using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public class PatternSequenceEditor : EditorWindow
{
    private static PatternSequenceEditor window;
    private static Vector2 mainFormSize, snbSize;

    private PatternSequence targetSequence;

    private string sequenceName;


    [MenuItem("Tools/Pattern Sequence Editor")]
    private static void ShowWindow()
    {
        if (window != null)
        {
            window.Close();
            window = null;
        }
        window = EditorWindow.CreateInstance<PatternSequenceEditor>();

        window.ShowUtility();

        mainFormSize = new Vector2(700F, 400F);
        snbSize = new Vector2(290F, 400F);

        window.minSize = mainFormSize;

        // Center
        var mainWindow = EditorGUIUtility.GetMainWindowPosition();
        var current = window.position;

        current.x = (mainWindow.x + mainWindow.width * 0.5f) - current.width * 0.5f;
        current.y = (mainWindow.y + mainWindow.height * 0.5f) - current.height * 0.5f - 100F;

        window.position = current;

        window.titleContent = new GUIContent("");
    }


    private void OnGUI()
    {
        mainFormSize.x = window.position.width - snbSize.x - 10;
        mainFormSize.y = snbSize.y = window.position.height;

        EditorGUILayout.BeginHorizontal();

        // Main
        EditorGUILayout.BeginVertical("box", GUILayout.Width(mainFormSize.x));
        DisplayMainForm();
        EditorGUILayout.EndVertical();

        // SNB
        EditorGUILayout.BeginVertical(GUILayout.Width(snbSize.x));
        DisplaySNB();
        EditorGUILayout.EndVertical();

        Space(10);

        EditorGUILayout.EndHorizontal();
    }


    private void DisplayMainForm()
    {
        if (targetSequence == null)
        {
            AlignCenter(horizontal: false, () =>
                AlignCenter(horizontal: true, () =>
                    GUILayout.Label("No pattern sequence file was selected")
                )
            );
            return;
        }

        GUILayout.Label($"Selected sequence: \t{targetSequence.name}", EditorStyles.boldLabel);

        Space();
    }

    private void DisplaySNB()
    {
        Space(20);
        AlignCenter(horizontal: true, () => GUILayout.Label("Pattern Sequence Editor", EditorStyles.boldLabel));
        Space(20);

        Anomaly.Editor.EditorUtils.DrawHorizontalLine(Color.gray);

        Space(10);

        AlignCenter(horizontal: true, () =>
        {
            float prev = EditorGUIUtility.labelWidth;
            EditorGUIUtility.labelWidth = 60;
            targetSequence = EditorGUILayout.ObjectField("Target", targetSequence, typeof(PatternSequence), false, GUILayout.Width(snbSize.x - 30)) as PatternSequence;
            EditorGUIUtility.labelWidth = prev;
        });

        Space();

        Anomaly.Editor.EditorUtils.DrawHorizontalLine(Color.gray);

        Space(10);

        AlignLeft(horizontal: true, () =>
        {
            Space(12);
            GUILayout.Label("Create new one", EditorStyles.boldLabel);
        });

        Space(2);

        AlignRight(horizontal: true, () =>
        {
            sequenceName = EditorGUILayout.TextField(sequenceName, GUILayout.Width(200));

            bool click = GUILayout.Button("Create", GUILayout.Width(60));
            if (click)
            {
                if (string.IsNullOrEmpty(sequenceName) || string.IsNullOrWhiteSpace(sequenceName)) sequenceName = "NewPatternSequence";

                string fileName = $"Assets/{sequenceName}.asset";

                if (AssetDatabase.LoadAssetAtPath<PatternSequence>(fileName) != null)
                {
                    EditorUtility.DisplayDialog("Warning!", $"Sequence {sequenceName} is already exist.\nYou cannot create sequence when sequence's name is same", "Confirm");
                    return;
                }

                var newAsset = ScriptableObject.CreateInstance<PatternSequence>();

                AssetDatabase.CreateAsset(newAsset, $"Assets/{sequenceName}.asset");
                AssetDatabase.SaveAssets();

                targetSequence = newAsset;
            }
            Space(15);
        });

        Space(10);
    }


    #region Editor Helper
    private void Space(float pixels = -1)
    {
        if (pixels < 0F) GUILayout.FlexibleSpace();
        else GUILayout.Space(pixels);
    }

    private void AlignCenter(bool horizontal, System.Action callback, string style = "")
    {
        if (horizontal) EditorGUILayout.BeginHorizontal(style);
        else EditorGUILayout.BeginVertical(style);

        GUILayout.FlexibleSpace();
        callback?.Invoke();
        GUILayout.FlexibleSpace();

        if (horizontal) EditorGUILayout.EndHorizontal();
        else EditorGUILayout.EndVertical();
    }
    private void AlignLeft(bool horizontal, System.Action callback, string style = "")
    {
        if (horizontal) EditorGUILayout.BeginHorizontal(style);
        else EditorGUILayout.BeginVertical(style);

        callback?.Invoke();
        GUILayout.FlexibleSpace();

        if (horizontal) EditorGUILayout.EndHorizontal();
        else EditorGUILayout.EndVertical();
    }
    private void AlignRight(bool horizontal, System.Action callback, string style = "")
    {
        if (horizontal) EditorGUILayout.BeginHorizontal(style);
        else EditorGUILayout.BeginVertical(style);

        GUILayout.FlexibleSpace();
        callback?.Invoke();

        if (horizontal) EditorGUILayout.EndHorizontal();
        else EditorGUILayout.EndVertical();
    }
    #endregion
}
