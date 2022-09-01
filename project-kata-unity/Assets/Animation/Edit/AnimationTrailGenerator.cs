using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimationTrailGenerator : MonoBehaviour
{
    [HideInInspector] public AnimationTrailData targetData;
    [HideInInspector] public AnimationClip targetAnimation;

    [Space(10)]
    [SerializeField] private Animator animator;

    [Space(10)]
    [SerializeField] private Transform weaponStart;
    [SerializeField] private Transform weaponEnd;

    [SerializeField] private BoxCollider weaponCollider;

    [Space(10)]
    [SerializeField] private int targetFrame = 60;

    private AnimatorOverrideController animOverride;
    private string overrideKey;

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


#if UNITY_EDITOR
    private void OnEnable()
    {
        AnimationTrailGeneratorWindow.OpenEditor(this);
    }

    private void OnDisable()
    {
        AnimationTrailGeneratorWindow.CloseEditor();
    }
#endif


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


    public void UpdateAnimation(AnimationClip clip)
    {
        targetAnimation = clip;

        if (targetAnimation == null) return;

        if (animOverride == null)
        {
            animOverride = new AnimatorOverrideController(animator.runtimeAnimatorController);
            animator.runtimeAnimatorController = animOverride;
            overrideKey = CurrentClip.name;
        }

        animOverride[overrideKey] = targetAnimation;

        targetData = null;
    }
}
