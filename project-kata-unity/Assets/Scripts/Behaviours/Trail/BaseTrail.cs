using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Anomaly;

public abstract class BaseTrail : Anomaly.Utils.PoolObject
{
    public abstract void OnTrailStarted();
    public abstract void OnTrailEnded();

    public abstract void OnEffectStarted();
    public abstract void OnEffectEnded();
}
