using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Anomaly;

public class DummyAnimationTester : CustomBehaviour, ICombat
{
    [SerializeField]
    private AnimatorComponent animator;

    [SerializeField]
    private CombatComponent combat;


    protected override void Initialize()
    {
        base.Initialize();

        combat.Initialize();
    }


    #region Animation Event
    public void TrailCast(AnimationEvent param)
    {
        combat.TrailCast(param.stringParameter.Split('|')[1].Trim(), param.intParameter);
    }

    public void CollectInputEvent(AnimationEvent param)
    {
        combat.CollectInputEvent();
    }
    public void StopInputEvent(AnimationEvent param)
    {
        combat.StopInputEvent();
    }

    public void Attack()
    {
    }

    public void Block(CustomBehaviour other)
    {
    }

    public void Parry(CustomBehaviour other)
    {
    }

    public void OnHit(CustomBehaviour other, params Collider[] hitParts)
    {
    }

    public void OnBlocked(CustomBehaviour other)
    {
    }

    public void OnParried(CustomBehaviour other)
    {
        Debug.Log("Parried!");
    }
    #endregion
}
