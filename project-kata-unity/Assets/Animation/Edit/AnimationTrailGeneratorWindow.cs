#if UNITY_EDITOR
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEditor;
using UnityEditor.SceneManagement;
using Anomaly.Utils;

public class AnimationTrailGeneratorWindow : EditorWindow
{
    private static AnimationTrailGeneratorWindow currentWindow;
    private static AnimationTrailGenerator targetComponent;

    public static void OpenEditor(AnimationTrailGenerator caller)
    {
        if (currentWindow != null)
        {
            currentWindow.Close();
            currentWindow = null;
        }

        currentWindow = EditorWindow.CreateInstance<AnimationTrailGeneratorWindow>();
        //currentWindow.ShowUtility();

        SceneView.duringSceneGui += currentWindow.SceneViewGUI;
        if (SceneView.lastActiveSceneView) SceneView.lastActiveSceneView.Repaint();

        targetComponent = caller;
        targetComponent.UpdateTrack(0);
    }
    public static void CloseEditor()
    {
        if (currentWindow == null) return;

        SceneView.duringSceneGui -= currentWindow.SceneViewGUI;
        if (SceneView.lastActiveSceneView) SceneView.lastActiveSceneView.Repaint();
    }

    private int currentTrackIdx = 0;

    private void SceneViewGUI(SceneView sceneView)
    {
        if (Event.current.type == EventType.Repaint) return;

        int controlId = GUIUtility.GetControlID(FocusType.Passive);
        GUILayout.Window(controlId, new Rect(5F, 25F, 300F, 300F), OnDisplay, "Animation Trail Editor");

        void OnDisplay(int id)
        {
            if (targetComponent == null) return;

            EditorGUILayout.BeginVertical("box");
            ShowDataField(targetComponent);
            ShowEditor(targetComponent);
            EditorGUILayout.EndVertical();
        }
    }


    private void ShowDataField(AnimationTrailGenerator self)
    {
        self.targetAnimation = CustomObjectField<AnimationClip>("Animation", self.targetAnimation, false, (clip) => { self.UpdateAnimation(clip); });
        self.targetData = CustomObjectField<AnimationTrailData>("Data", self.targetData, false, (data) =>
        {
            currentTrackIdx = 0;
            self.UpdateTrack(0);
        });
    }

    private void ShowEditor(AnimationTrailGenerator self)
    {
        GUILayout.Space(10);

        ShowTrackBar(self);

        ShowTimeBar(self);

        ShowEditorButtons();

        void ShowEditorButtons()
        {
            GUILayout.Space(20);

            if (GUILayout.Button("Generate", GUILayout.Height(40)))
            {
                CheckTrailDataFile(self);
                self.GenerateTrail(currentTrackIdx);
                EditorUtility.SetDirty(self.targetData);
            }

            EditorGUILayout.BeginHorizontal();
            {
                GUI.enabled = self.IsDataValid;
                if (GUILayout.Button("Add Track", GUILayout.Height(40)))
                {
                    currentTrackIdx = self.AddTrack();
                    self.UpdateTrack(currentTrackIdx);
                }

                GUI.enabled = self.IsDataValid && self.TrackCount > 0;
                if (GUILayout.Button("Remove Track", GUILayout.Height(40)))
                {
                    currentTrackIdx = self.RemoveTrack(currentTrackIdx);
                    self.UpdateTrack(currentTrackIdx);
                }
                GUI.enabled = true;
            }
            EditorGUILayout.EndHorizontal();


            GUILayout.Space(15);

            GUI.enabled = self.IsDataValid;
            if (GUILayout.Button("Show Trail", GUILayout.Height(50)))
            {
                self.DrawTrail(currentTrackIdx);
            }
            GUI.enabled = true;

            GUILayout.Space(5);
        }
    }

    private void ShowTrackBar(AnimationTrailGenerator self)
    {
        if (!self.IsDataValid) return;

        EditorGUILayout.BeginHorizontal();

        GUI.enabled = currentTrackIdx > 0;
        if (GUILayout.Button("◀", GUILayout.Width(30)))
        {
            self.UpdateTrack(--currentTrackIdx);
        }
        GUI.enabled = true;

        GUILayout.FlexibleSpace();
        GUILayout.Label($"Track {currentTrackIdx + 1}");
        GUILayout.FlexibleSpace();

        GUI.enabled = currentTrackIdx < self.TrackCount - 1;
        if (GUILayout.Button("▶", GUILayout.Width(30)))
        {
            self.UpdateTrack(++currentTrackIdx);
        }
        GUI.enabled = true;

        EditorGUILayout.EndHorizontal();
    }

    private void ShowTimeBar(AnimationTrailGenerator self)
    {
        GUILayout.Space(10);

        EditorGUILayout.BeginHorizontal();

        GUILayout.Label($"{self.TrailStartTime:F3}");
        GUILayout.FlexibleSpace();
        GUILayout.Label("Timeline");
        GUILayout.FlexibleSpace();
        GUILayout.Label($"{self.TrailEndTime:F3}");

        EditorGUILayout.EndHorizontal();

        GUILayout.Space(10);

        (float start, float end) previous = (self.TrailStartTime, self.TrailEndTime);
        (float start, float end) current = previous;

        EditorGUILayout.MinMaxSlider(ref current.start, ref current.end, 0F, 1F);

        self.TrailStartTime = current.start;
        self.TrailEndTime = current.end;

        if (Math.IsNotZero(Mathf.Abs(self.TrailStartTime - previous.start)))
        {
            self.SetPose(self.TrailStartTime);
        }
        else if (Math.IsNotZero(Mathf.Abs(self.TrailEndTime - previous.end)))
        {
            self.SetPose(self.TrailEndTime);
        }
    }

    private void CheckTrailDataFile(AnimationTrailGenerator self)
    {
        if (self.targetData == null)
        {
            self.targetData = ScriptableObject.CreateInstance<AnimationTrailData>();
            AssetDatabase.CreateAsset(self.targetData, $"Assets/{self.CurrentClip.name}.asset");
            currentTrackIdx = 0;
        }
    }


    private T CustomObjectField<T>(string label, Object obj, bool allowSceneObjects, System.Action<T> onChanged = null, params GUILayoutOption[] options) where T : Object
    {
        T current = EditorGUILayout.ObjectField(label, obj, typeof(T), allowSceneObjects, options) as T;
        if (ReferenceEquals(current, obj)) return current;
        onChanged?.Invoke(current);
        return current;
    }
}
#endif