using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Anomaly;

public class PlayerBlockState : State<Player>
{
    public override StateID ID => StateID.PlayerBlock;

    public override bool IsTransition(Player target, out StateID next)
    {
        if (target.Animator.GetCurrentState().IsName("Block") || target.Animator.GetNextState().IsName("Block"))
        {
            next = StateID.None;
            return false;
        }
        next = StateID.PlayerDefense;
        return true;
    }

    public override void OnEnter(Player target)
    {
        target.Animator.SetTrigger("Block");

        target.Combat.AddPose(CombatComponent.Pose.Block);
    }

    public override void OnExit(Player target)
    {
        target.Animator.SetFloat("VSpeed", 0);
        target.Animator.SetFloat("HSpeed", 0);

        target.Combat.ReleasePose(CombatComponent.Pose.Block);
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
