using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Anomaly;
using System.Threading.Tasks;

public class CombatEntity : CustomBehaviour
{
    public struct BoxCastInfo
    {
        public Vector3 center;
        public Vector3 size;
        public Quaternion rotation;
    }

    public PhysicsComponent Physics;

    [SerializeField] private CustomBehaviour rootBehaviour;

    [Space(10)]
    [SerializeField] private Transform weaponStart;
    [SerializeField] private Transform weaponEnd;

    [SerializeField] private BoxCollider weaponCollider;

    private Anomaly.Utils.Stream inputStream;

    private Queue<BoxCastInfo> boxCastQueue = new Queue<BoxCastInfo>();


    public UnityEngine.Events.UnityEvent onAttackEventInvoked;


    /* 
     * floatParam: dividend (maximum length)
     * intParam: plane count
     */
    public async void DoLineAttack(AnimationEvent param)
    {
        float maximumLength = param.floatParameter;
        float ignoreThreshold = maximumLength / param.intParameter;

        float katanaLength = (weaponEnd.position - weaponStart.position).magnitude;

        float totalLength = 0F;
        (Vector3 start, Vector3 end) previous = (weaponStart.position, weaponEnd.position);

        HashSet<CustomBehaviour> hitList = new HashSet<CustomBehaviour>();

        boxCastQueue.Clear();

        while (totalLength < maximumLength)
        {
            (Vector3 start, Vector3 end) current = (weaponStart.position, weaponEnd.position);

            float length = current.end == previous.end ? 0F : (current.end - previous.end).magnitude;

            if (current.end != previous.end && length < ignoreThreshold)
            {
                await Task.Yield();
                continue;
            }

            var castInfo = new BoxCastInfo()
            {
                center = current.start + (current.end - current.start).normalized * katanaLength * 0.5f,
                size = weaponCollider.size,
                rotation = weaponStart.rotation
            };

            if (IsOverlap(castInfo, out var hits))
            {
                SendHitEvent(hits);
            }

            boxCastQueue.Enqueue(castInfo);

            Debug.DrawLine(previous.start, current.start, Color.green, 0.5f);
            Debug.DrawLine(previous.end, current.end, Color.green, 0.5f);
            Debug.DrawLine(current.start, current.end, Color.green, 0.5f);

            totalLength += length;

            previous = current;

            await Task.Yield();
        }

        bool IsOverlap(BoxCastInfo info, out Collider[] hits)
        {
            hits = UnityEngine.Physics.OverlapBox(info.center, info.size * 0.5f, info.rotation, 1 << LayerMask.NameToLayer("Hittable"));
            if (hits == null || hits.Length == 0) return false;
            return true;
        }

        void SendHitEvent(Collider[] hits)
        {
            for (int i = 0; i < hits.Length; ++i)
            {
                if (ReferenceEquals(hits[i], weaponCollider)) continue;

                var root = hits[i].GetComponent<RootSelector>().Root;
                if (root == null)
                {
                    Debug.Log("RootSelector: Root is Null");
                    continue;
                }

                if (hitList.Contains(root)) continue;

                hitList.Add(root);
                EventDispatcher.Instance.Send(new HitEvent()
                {
                    sender = rootBehaviour,
                    receiver = root,
                    hitPart = hits[i]
                });
            }
        }
    }

    public async void CollectInputEvent(AnimationEvent param)
    {
        if (inputStream == null)
        {
            inputStream = Anomaly.Utils.Stream.Create(this);
            inputStream.Select(() => Input.GetMouseButtonDown(0))
                       .Subscribe(data =>
                       {
                           inputStream.Close();
                           onAttackEventInvoked?.Invoke();
                       });
            inputStream.Start();
        }

        inputStream.Open();
        for (int i = 0; i < param.intParameter; ++i)
        {
            await Task.Yield();
        }
        inputStream.Close();
    }

    public void AddImpulseForward(AnimationEvent param)
    {
        var player = rootBehaviour as Player;

        var forward = player.Character.GetModelForward();

        this.Physics.SetForceAttenScale(param.intParameter);
        this.Physics.AddImpulse(forward, param.floatParameter);
    }

    public void AddImpulseBackward(AnimationEvent param)
    {
        var player = rootBehaviour as Player;

        var backward = -player.Character.GetModelForward();

        this.Physics.SetForceAttenScale(param.intParameter);
        this.Physics.AddImpulse(backward, param.floatParameter);
    }


    // private void OnDrawGizmos()
    // {
    //     foreach (var cube in boxCastQueue)
    //     {
    //         Gizmos.matrix = Matrix4x4.TRS(cube.center, cube.rotation, Vector3.one);
    //         Gizmos.DrawWireCube(Vector3.zero, cube.size);
    //     }
    // }
}
