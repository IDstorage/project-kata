using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Anomaly;

public class PlayerParryState : State<Player>
{
    public override StateID ID => StateID.PlayerParry;

    public override bool IsTransition(Player target, out StateID next)
    {
        if (target.Animator.GetCurrentState().IsName("Parry") || target.Animator.GetNextState().IsName("Parry"))
        {
            next = StateID.None;
            return false;
        }
        next = StateID.PlayerDefense;
        return true;
    }

    public override void OnEnter(Player target)
    {
        int parryIndex = Random.Range(0, 2);
        target.Animator.SetFloat("ParryIndex", parryIndex);
        target.Animator.SetTrigger("Parry");

        target.Combat.AddPose(CombatComponent.Pose.Parry);
    }

    public override void OnExit(Player target)
    {
        target.Animator.SetFloat("VSpeed", 0);
        target.Animator.SetFloat("HSpeed", 0);

        target.Combat.ReleasePose(CombatComponent.Pose.Parry);
    }

    public override void OnFixedUpdate(Player target)
    {

    }

    public override void OnLateUpdate(Player target)
    {

    }

    public override void OnUpdate(Player target)
    {
        target.HandleCamera();
    }
}
