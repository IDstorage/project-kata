using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Anomaly;


public class PlayerAttackState : State
{
    public override Identity ID => State.Identity.PlayerAttack;


    public override void OnEnter(CustomBehaviour target)
    {
        (target as Player).Animator.SetTrigger("DefaultAttack");
    }

    public override void OnExit(CustomBehaviour target)
    {

    }


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


    public override void OnFixedUpdate(CustomBehaviour target)
    {

    }

    public override void OnUpdate(CustomBehaviour target)
    {
        var player = target as Player;

        player.HandleCamera();

        Debug.DrawRay(target.transform.position, player.ThirdPerson.GetForwardVector() * 5F, Color.red);
    }

    public override void OnLateUpdate(CustomBehaviour target)
    {

    }
}
