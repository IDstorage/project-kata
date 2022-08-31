using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Anomaly;
using Anomaly.Utils;
#if UNITY_EDITOR
using UnityEditor;
using System.Globalization;
#endif

public class AnimationTrailEditor : MonoBehaviour
{
    [HideInInspector] public AnimationTrailData targetData;

    [Space(10)]
    [SerializeField] private Animator animator;

    [Space(10)]
    [SerializeField] private Transform weaponStart;
    [SerializeField] private Transform weaponEnd;

    [SerializeField] private BoxCollider weaponCollider;

    [Space(10)]
    [SerializeField] private int targetFrame = 60;

    private (float start, float end) trailNormalizedTime;
    public float TrailStartTime
    {
        get { return trailNormalizedTime.start; }
        set { trailNormalizedTime.start = Mathf.Clamp01(value); }
    }
    public float TrailEndTime
    {
        get { return trailNormalizedTime.end; }
        set { trailNormalizedTime.end = Mathf.Clamp01(value); }
    }

    public AnimationClip CurrentClip => animator.GetCurrentAnimatorClipInfo(0)[0].clip;

    public float DeltaTime => animator == null ? 1F : 1F / targetFrame;

    public bool IsDataValid => targetData != null;
    public int TrackCount => !IsDataValid ? 0 : targetData.tracks.Count;


    private void Awake()
    {
        SetPose(0);
    }

    public void SetPose(float time)
    {
        animator.speed = 0F;
        animator.Play("Default", 0, time);
        animator.Update(Time.deltaTime);
    }


    public void GenerateTrail(int trackIdx = 0)
    {
        List<AnimationTrailData.Box> boxes = new List<AnimationTrailData.Box>();

        float katanaLength = (weaponEnd.position - weaponStart.position).magnitude;

        float t = TrailStartTime;
        float dt = DeltaTime;

        SetPose(t);
        (Vector3 start, Vector3 end) previous = (weaponStart.position, weaponEnd.position);

        while (t <= trailNormalizedTime.end)
        {
            SetPose(t);

            AddCurrent();

            (Vector3 start, Vector3 end) current = (weaponStart.position, weaponEnd.position);

            Debug.DrawLine(previous.start, current.start, Color.green, 2.5f);
            Debug.DrawLine(previous.end, current.end, Color.green, 2.5f);
            Debug.DrawLine(current.start, current.end, Color.green, 2.5f);

            previous = current;

            t += dt;
        }

        SetPose(TrailEndTime);
        AddCurrent();

        var track = new AnimationTrailData.Track()
        {
            start = TrailStartTime,
            end = TrailEndTime,
            boxes = boxes.ToArray()
        };
        if (!targetData.TrySet(trackIdx, track)) return;


        void AddCurrent()
        {
            (Vector3 start, Vector3 end) current = (weaponStart.position, weaponEnd.position);

            var boxInfo = new AnimationTrailData.Box()
            {
                center = current.start + (current.end - current.start).normalized * katanaLength * 0.5f,
                size = weaponCollider.size,
                eulerAngles = weaponStart.eulerAngles
            };

            boxes.Add(boxInfo);
        }
    }

    public void DrawTrail(int trackIdx)
    {
        if (targetData.tracks.Count <= trackIdx) return;
        if (targetData.tracks[trackIdx].boxes == null || targetData.tracks[trackIdx].boxes.Length == 0) return;

        var previous = GetPositions(targetData.tracks[trackIdx].boxes[0]);

        foreach (var box in targetData.tracks[trackIdx].boxes)
        {
            var current = GetPositions(box);

            Debug.DrawLine(current.start, current.end, Color.green, 2.5f);
            Debug.DrawLine(current.start, previous.start, Color.green, 2.5f);
            Debug.DrawLine(current.end, previous.end, Color.green, 2.5f);

            previous = current;
        }


        (Vector3 start, Vector3 end) GetPositions(AnimationTrailData.Box box)
        {
            return (box.center + Quaternion.Euler(box.eulerAngles) * Vector3.down * box.size.y * 0.5f,
                    box.center + Quaternion.Euler(box.eulerAngles) * Vector3.up * box.size.y * 0.5f);
        }
    }


    public int AddTrack()
    {
        targetData.tracks.Add(new AnimationTrailData.Track()
        {
            start = 0F,
            end = 0F,
            boxes = null
        });
        return targetData.tracks.Count - 1;
    }
    public int RemoveTrack(int idx)
    {
        if (targetData.tracks.Count == 1)
        {
            targetData.tracks[0] = new AnimationTrailData.Track()
            {
                start = 0F,
                end = 0F,
                boxes = null
            };
            return 0;
        }
        targetData.tracks.RemoveAt(idx);
        return Mathf.Min(targetData.tracks.Count - 1, idx);
    }

    public void UpdateTrack(int idx)
    {
        if (targetData == null) return;
        if (targetData.tracks.Count <= idx) return;

        TrailStartTime = targetData.tracks[idx].start;
        TrailEndTime = targetData.tracks[idx].end;
    }
}

#if UNITY_EDITOR
[CustomEditor(typeof(AnimationTrailEditor))]
public class AnimationTrailEditorEditor : Editor
{
    private int currentTrackIdx = 0;

    private void OnEnable()
    {
        (target as AnimationTrailEditor).UpdateTrack(currentTrackIdx);
    }

    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();

        if (!EditorApplication.isPlaying)
        {
            GUILayout.Space(20);
            if (GUILayout.Button("Play", GUILayout.Height(40)))
            {
                EditorApplication.EnterPlaymode();
            }
            return;
        }

        GUILayout.Space(10);

        Anomaly.Editor.EditorUtils.DrawHorizontalLine(Color.gray);

        GUILayout.Space(20);


        var self = target as AnimationTrailEditor;

        var prevData = self.targetData;
        self.targetData = EditorGUILayout.ObjectField("Data", self.targetData, typeof(AnimationTrailData), false) as AnimationTrailData;
        if (!ReferenceEquals(self.targetData, prevData))
        {
            currentTrackIdx = 0;
            self.UpdateTrack(currentTrackIdx);
        }

        GUILayout.Space(30);

        EditorGUILayout.BeginVertical("box");

        ShowTrackBar(self);

        GUILayout.Space(10);

        ShowTimeBar(self);

        GUILayout.Space(20);

        if (GUILayout.Button("Generate", GUILayout.Height(40)))
        {
            CheckTrailDataFile(self);
            self.GenerateTrail(currentTrackIdx);
            AssetDatabase.Refresh();
        }

        EditorGUILayout.BeginHorizontal();

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

        EditorGUILayout.EndHorizontal();


        GUILayout.Space(15);

        GUI.enabled = self.IsDataValid;
        if (GUILayout.Button("Show Trail", GUILayout.Height(50)))
        {
            self.DrawTrail(currentTrackIdx);
        }
        GUI.enabled = true;

        EditorGUILayout.EndVertical();
    }

    private void ShowTrackBar(AnimationTrailEditor self)
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

    private void ShowTimeBar(AnimationTrailEditor self)
    {
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

    public void CheckTrailDataFile(AnimationTrailEditor self)
    {
        if (self.targetData == null)
        {
            self.targetData = ScriptableObject.CreateInstance<AnimationTrailData>();
            AssetDatabase.CreateAsset(self.targetData, $"Assets/{self.CurrentClip.name}.asset");
            currentTrackIdx = 0;
        }
    }
}
#endif
