using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HitEvent : Anomaly.BaseEvent
{
    public HashSet<Collider> hitParts;
}

public class HitEventStream : Anomaly.EventStream<HitEvent>
{
    public HitEventStream(MonoBehaviour mono) : base(mono)
    {
    }
}
