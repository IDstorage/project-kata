using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Anomaly;

public class PlayerBlockState : State
{
    public override Identity ID => Identity.PlayerBlock;

    public override bool IsTransition(CustomBehaviour target, out Identity next)
    {
        var player = target as Player;
        if (player.Animator.GetCurrentState().IsName("Block") || player.Animator.GetNextState().IsName("Block"))
        {
            next = Identity.None;
            return false;
        }
        next = Identity.PlayerDefense;
        return true;
    }

    public override void OnEnter(CustomBehaviour target)
    {
        (target as Player).Animator.SetTrigger("Block");
    }

    public override void OnExit(CustomBehaviour target)
    {
        var player = target as Player;

        player.Animator.SetFloat("VSpeed", 0);
        player.Animator.SetFloat("HSpeed", 0);
    }

    public override void OnFixedUpdate(CustomBehaviour target)
    {

    }

    public override void OnLateUpdate(CustomBehaviour target)
    {

    }

    public override void OnUpdate(CustomBehaviour target)
    {
        (target as Player).HandleCamera();
    }
}
