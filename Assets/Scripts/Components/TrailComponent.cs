using System.Collections;
using System.Collections.Generic;
using Anomaly;
using Anomaly.Utils;
using UnityEngine;

[System.Serializable]
public class TrailComponent : CustomComponent
{
    public enum Type
    {
        Test = -99,

        None = 0,
        Default
    }

    private BaseTrail trail;

    [SerializeField] private Transform trailParent;

    public void Create(Type type, Transform parent = null)
    {
        if (parent == null) parent = trailParent;
        if (type != Type.Test) return;
        trail = PoolManager.Instance.Get("TestTrail") as TestTrail;
        trail.BeginTracing(parent);
    }

    public void Destroy()
    {
        trail.EndTracing();
    }
}
