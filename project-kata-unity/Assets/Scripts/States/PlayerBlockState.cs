using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Anomaly;

public class PlayerBlockState : State<Player>
{
    public override StateID ID => StateID.Block;

    public override bool IsTransition(Player target, out StateID next)
    {
        if (target.Animator.GetCurrentState().IsName("Block") || target.Animator.GetNextState().IsName("Block"))
        {
            next = StateID.None;
            return false;
        }
        next = StateID.Defense;
        return true;
    }

    public override void OnEnter(Player target)
    {
        target.Animator.SetTrigger("Block");
    }

    public override void OnExit(Player target)
    {
        target.Animator.SetFloat("VSpeed", 0);
        target.Animator.SetFloat("HSpeed", 0);
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
