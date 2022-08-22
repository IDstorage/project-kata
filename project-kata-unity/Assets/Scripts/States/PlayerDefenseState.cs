using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Anomaly;


public class PlayerDefenseState : State
{
    public override Identity ID => State.Identity.PlayerDefense;


    private float hFollow = 0F, vFollow = 0F;


    public override void OnEnter(CustomBehaviour target)
    {
        (target as Player).Animator.SetBool("IsBlocking", true);
    }

    public override void OnExit(CustomBehaviour target)
    {
        (target as Player).Animator.SetBool("IsBlocking", false);
    }


    public override bool IsTransition(CustomBehaviour target, out Identity next)
    {
        if (!InputManager.Instance.IsHeld(InputManager.Button.Defense))
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
        player.Move();
    }

    public override void OnLateUpdate(CustomBehaviour target)
    {

    }
}
