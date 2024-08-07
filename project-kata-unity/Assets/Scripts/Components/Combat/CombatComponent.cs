using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AddressableAssets;
using Anomaly;
using Anomaly.Utils;
using System.Linq;

[System.Serializable]
public class CombatComponent : CustomComponent
{
    public struct BoxCastInfo
    {
        public Vector3 center;
        public Vector3 size;
        public Quaternion rotation;
    }

    [SerializeField] private Transform model;

    [Space(10)]
    [SerializeField] private Transform weaponStart;
    [SerializeField] private Transform weaponEnd;

    [SerializeField] private BoxCollider weaponCollider;

    [Space(10)]
    [SerializeField] private AssetLabelReference trailDataLabel;


    private Stream inputStream;
    private SmartCoroutine parryTriggerCoroutine;

    private Queue<BoxCastInfo> boxCastQueue = new Queue<BoxCastInfo>();


    public bool CanParry { get; set; } = false;


    public void Initialize()
    {
        // Load trail data first
        if (trailDataLabel == null) return;
        Addressables.DownloadDependenciesAsync(trailDataLabel);
    }


    /* 
     * stringParam: trail data name
     * intParam: track index
     */
    public async void TrailCast(string trailDataName, int trackIdx)
    {
        string dataKey = $"{trailDataLabel.labelString}_{trailDataName}";
        var opHandle = Addressables.LoadAssetAsync<AnimationTrailData>(dataKey);
        await opHandle.Task;

        AnimationTrailData trailData = opHandle.Result;

        if (trackIdx < 0 || trailData.tracks.Count <= trackIdx)
        {
            Debug.LogError($"Wrong track index: {dataKey} / {trackIdx}");
            return;
        }

        var track = trailData.tracks[trackIdx];

        Dictionary<CustomBehaviour, HashSet<Collider>> hitList = new Dictionary<CustomBehaviour, HashSet<Collider>>();

        boxCastQueue.Clear();

        for (int i = 0; i < track.boxes.Length; ++i)
        {
            var box = track.boxes[i];

            box.start = (Vector3)box.start * model.lossyScale.x;
            box.end = (Vector3)box.end * model.lossyScale.x;
            box.width *= model.lossyScale.x;
            box.height *= model.lossyScale.x;

            Vector3 dir = model.rotation * ((Vector3)box.end - (Vector3)box.start);
            Vector3 center = (model.rotation * box.start + dir * 0.5f + model.position);
            Vector3 size = model.rotation * new Vector3(box.width, dir.magnitude, box.height);
            Quaternion rotation = Quaternion.FromToRotation(Vector3.up, dir.normalized);

            boxCastQueue.Enqueue(new BoxCastInfo()
            {
                center = center,
                size = size,
                rotation = rotation
            });

            Debug.DrawRay(center, dir, Color.green);

            var hits = Physics.OverlapBox(center, new Vector3(box.width, dir.magnitude, box.height), rotation, 1 << LayerMask.NameToLayer("Hittable"));
            if (hits == null || hits.Length == 0) continue;

            for (int j = 0; j < hits.Length; ++j)
            {
                var hit = hits[j];
                if (ReferenceEquals(hit, weaponCollider)) continue;

                var selector = hit.GetComponent<RootSelector>();
                if (selector == null) continue;
                if (selector.Root == null)
                {
                    Debug.Log("RootSelector: Root is Null");
                    continue;
                }

                if (ReferenceEquals(selector.Root, behaviour)) continue;

                if (!hitList.ContainsKey(selector.Root))
                {
                    hitList.Add(selector.Root, new HashSet<Collider>());
                }

                if (hitList[selector.Root].Contains(hit)) continue;

                hitList[selector.Root].Add(hit);
            }
        }

        foreach (var hits in hitList)
        {
            ICombat combat = hits.Key as ICombat;
            if (combat == null) continue;
            //combat.OnHit(behaviour, hits.Value.ToArray());

            behaviour.SendEvent<HitEvent>(hits.Key, new HitEvent() { hitParts = hits.Value });
        }
    }

    public void CollectInputEvent()
    {
        if (inputStream != null)
        {
            inputStream.Open();
            return;
        }

        inputStream = Anomaly.Utils.Stream.Create(behaviour);
        inputStream.Select(() => AInput.IsPressed(CustomKey.Current.Attack))
                    .Subscribe(data =>
                    {
                        inputStream.Close();
                        var combat = behaviour as ICombat;
                        if (combat == null) return;
                        combat.Attack();
                    });
        inputStream.Start();

        inputStream.Open();
    }
    public void StopInputEvent()
    {
        inputStream.Close();
    }

    public void TryParry(float delay, float parryTiming)
    {
        if (parryTriggerCoroutine == null)
        {
            parryTriggerCoroutine = SmartCoroutine.Create(CoTryParry)
                                                  .OnAborted(() => { CanParry = false; });
        }

        parryTriggerCoroutine.Stop();
        parryTriggerCoroutine.Start();

        IEnumerator CoTryParry()
        {
            CanParry = false;
            yield return new WaitForSeconds(delay);
            CanParry = true;
            yield return new WaitForSeconds(parryTiming);
            CanParry = false;
        }
    }

    private void OnDrawGizmos()
    {
        foreach (var cube in boxCastQueue)
        {
            Gizmos.matrix = Matrix4x4.TRS(cube.center, cube.rotation, Vector3.one);
            Gizmos.DrawWireCube(Vector3.zero, cube.size);
        }
    }
}
