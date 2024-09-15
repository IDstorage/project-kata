using System.Collections.Generic;
using UnityEngine;

public class ParryEvent : Anomaly.BaseEvent
{
    public override void Invoke()
    {
        var combat = receiver as ICombat;
        if (combat == null) return;

        combat.OnParried(sender);
    }
}