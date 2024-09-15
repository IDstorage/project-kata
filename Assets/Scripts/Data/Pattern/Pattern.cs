using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;

public abstract class Pattern : ScriptableObject
{
    public abstract Task Execute(Actor target, CombatComponent combat);
}

public abstract class PatternBranch : ScriptableObject
{
    public abstract bool UseAlternative(Actor target, CombatComponent combat);
}
