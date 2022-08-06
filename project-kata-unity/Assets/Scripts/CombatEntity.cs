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

    [SerializeField] private Transform weaponStart;
    [SerializeField] private Transform weaponEnd;

    [SerializeField] private BoxCollider weaponCollider;

    private Anomaly.Utils.Stream inputStream;

    private Queue<BoxCastInfo> debugQueue = new Queue<BoxCastInfo>();


    public UnityEngine.Events.UnityEvent onAttackEventInvoked;


    public async void DoLineAttack(AnimationEvent param)
    {
        float damage = param.floatParameter;

        float distance = (weaponEnd.position - weaponStart.position).magnitude;

        debugQueue.Clear();

        Vector3 start = weaponStart.position, end = weaponEnd.position;
        for (int i = 0; i < param.intParameter; ++i)
        {
            var direction = weaponEnd.position - weaponStart.position;

            debugQueue.Enqueue(new BoxCastInfo()
            {
                center = weaponStart.position + direction.normalized * distance * 0.5f,
                size = weaponCollider.size,
                rotation = weaponStart.rotation
            });

            Debug.DrawLine(start, weaponStart.position, Color.green, 0.5f);
            Debug.DrawLine(end, weaponEnd.position, Color.green, 0.5f);
            Debug.DrawLine(weaponStart.position, weaponEnd.position, Color.green, 0.5f);

            start = weaponStart.position;
            end = weaponEnd.position;

            await Task.Yield();
        }

        foreach (var info in debugQueue)
        {
            var hits = Physics.OverlapBox(info.center, info.size * 0.5f, info.rotation, ~(1 << LayerMask.GetMask("Hittable")));
            if (hits == null || hits.Length == 0) continue;
            bool flag = false;
            for (int i = 0; i < hits.Length && !flag; ++i)
            {
                flag = hits[i].name == "Cube";
            }
            if (!flag) return;

            Debug.Log("Hit");
            return;
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
        Debug.Log("Open");
        for (int i = 0; i < param.intParameter; ++i)
        {
            await Task.Yield();
        }
        inputStream.Close();
        Debug.Log("Close");
    }

    private void OnDrawGizmos()
    {
        return;
        foreach (var cube in debugQueue)
        {
            Gizmos.matrix = Matrix4x4.TRS(cube.center, cube.rotation, Vector3.one);
            Gizmos.DrawWireCube(Vector3.zero, cube.size);
        }
    }
}
