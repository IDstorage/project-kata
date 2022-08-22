using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Anomaly;


public class PlayerLocomotionState : State
{
    public override Identity ID => State.Identity.PlayerLocomotion;

    private Vector3 handlePos = new Vector3(5F, 0.5f, -10F);


    public override void OnEnter(CustomBehaviour target)
    {
    }

    public override void OnExit(CustomBehaviour target)
    {

    }


    public override bool IsTransition(CustomBehaviour target, out Identity next)
    {
        if (InputManager.Instance.IsHeld(InputManager.Button.Defense))
        {
            next = Identity.PlayerDefense;
            return true;
        }
        if (InputManager.Instance.IsPressed(InputManager.Button.Attack))
        {
            next = Identity.PlayerAttack;
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
