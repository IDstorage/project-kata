using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;
using Anomaly;
using Anomaly.Utils;
using System.Threading.Tasks;

public class CombatEntity : CustomBehaviour
{
    public struct BoxCastInfo
    {
        public Vector3 center;
        public Vector3 size;
        public Quaternion rotation;
    }

    [SerializeField] private CustomBehaviour rootBehaviour;

    [Space(10)]
    [SerializeField] private Transform weaponStart;
    [SerializeField] private Transform weaponEnd;

    [SerializeField] private BoxCollider weaponCollider;

    [Space(10)]
    [SerializeField] private AssetLabelReference trailDataLabel;
    [SerializeField] private string trailDataPrefix = "AnimationTrail_";

    [Space(10)]
    public UnityEngine.Events.UnityEvent onAttackEventInvoked;


    private Anomaly.Utils.Stream inputStream;

    private Queue<BoxCastInfo> boxCastQueue = new Queue<BoxCastInfo>();


    protected override void Initialize()
    {
        base.Initialize();

        // Load trail data first
        if (trailDataLabel == null) return;
        Addressables.DownloadDependenciesAsync(trailDataLabel);
    }


    /* 
     * stringParam: trail data name
     * intParam: track index
     */
    public async void DoLineAttack(AnimationEvent param)
    {
        string dataKey = $"{trailDataPrefix}{param.stringParameter.Split('|')[1]}";
        var opHandle = Addressables.LoadAssetAsync<AnimationTrailData>(dataKey);
        await opHandle.Task;

        AnimationTrailData trailData = opHandle.Result;

        if (param.intParameter < 0 || trailData.tracks.Count <= param.intParameter)
        {
            Debug.LogError($"Wrong track index: {dataKey} / {param.intParameter}");
            return;
        }

        var track = trailData.tracks[param.intParameter];

        Dictionary<CustomBehaviour, HashSet<Collider>> hitList = new Dictionary<CustomBehaviour, HashSet<Collider>>();

        boxCastQueue.Clear();

        for (int i = 0; i < track.boxes.Length; ++i)
        {
            var box = track.boxes[i];

            Vector3 dir = transform.rotation * ((Vector3)box.end - (Vector3)box.start);
            Vector3 center = (transform.rotation * box.start + dir * 0.5f + transform.position);
            Vector3 size = transform.rotation * new Vector3(box.width, dir.magnitude, box.height);
            Quaternion rotation = Quaternion.FromToRotation(Vector3.up, dir.normalized);

            boxCastQueue.Enqueue(new BoxCastInfo()
            {
                center = center,
                size = size,
                rotation = rotation
            });

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

                if (ReferenceEquals(selector.Root, rootBehaviour)) continue;

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
            EventDispatcher.Instance.Send(new HitEvent()
            {
                sender = rootBehaviour,
                receiver = hits.Key,
                hitParts = hits.Value
            });
        }
    }

    public void CollectInputEvent(AnimationEvent param)
    {
        if (inputStream == null)
        {
            inputStream = Anomaly.Utils.Stream.Create(this);
            inputStream.Select(() => AInput.IsPressed(CustomKey.Current.Attack))
                       .Subscribe(data =>
                       {
                           inputStream.Close();
                           onAttackEventInvoked?.Invoke();
                       });
            inputStream.Start();
        }

        inputStream.Open();
    }
    public void StopInputEvent(AnimationEvent param)
    {
        inputStream.Close();
    }

    public void AddImpulseForward(AnimationEvent param)
    {
        var actor = rootBehaviour as Actor;
        var forward = actor.Character.GetModelForward();

        actor.CharacterPhysics.SetForceAttenScale(param.intParameter);
        actor.CharacterPhysics.AddImpulse(forward, param.floatParameter);
    }

    public void AddImpulseBackward(AnimationEvent param)
    {
        var actor = rootBehaviour as Actor;
        var backward = -actor.Character.GetModelForward();

        actor.CharacterPhysics.SetForceAttenScale(param.intParameter);
        actor.CharacterPhysics.AddImpulse(backward, param.floatParameter);
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
