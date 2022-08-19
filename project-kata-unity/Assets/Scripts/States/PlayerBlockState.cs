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
        if (player.Animator.GetNextState().IsName("Locomotion"))
        {
            next = Identity.PlayerLocomotion;
            return true;
        }
        next = Identity.None;
        return false;
    }

    public override void OnEnter(CustomBehaviour target)
    {
        (target as Player).Animator.SetTrigger("Block");
    }

    public override void OnExit(CustomBehaviour target)
    {

    }

    public override void OnFixedUpdate(CustomBehaviour target)
    {

    }

    public override void OnLateUpdate(CustomBehaviour target)
    {

    }

    public override void OnUpdate(CustomBehaviour target)
    {

    }
}
