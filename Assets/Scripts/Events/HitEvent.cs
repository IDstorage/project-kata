using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class HitEvent : Anomaly.BaseEvent
{
    public HashSet<Collider> hitParts;

    public override void Invoke()
    {
        var combat = receiver as ICombat;
        if (combat == null) return;

        combat.OnHit(sender, hitParts.ToArray());
    }
}