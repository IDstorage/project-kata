using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Pattern : ScriptableObject
{
    public abstract void Execute(Actor target);
}
