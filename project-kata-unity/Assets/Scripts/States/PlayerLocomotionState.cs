using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Anomaly;
using Anomaly.Utils;


public class PlayerLocomotionState : State<Player>
{
    public override StateID ID => StateID.PlayerLocomotion;

    private Vector3 handlePos = new Vector3(5F, 0.5f, -10F);


    public override void OnEnter(Player target)
    {
    }

    public override void OnExit(Player target)
    {

    }


    public override bool IsTransition(Player target, out StateID next)
    {
        if (AInput.IsHeld(CustomKey.Current.Defense))
        {
            next = StateID.Defense;
            target.TryParry();
            return true;
        }
        if (AInput.IsPressed(CustomKey.Current.Attack))
        {
            next = StateID.Attack;
            return true;
        }
        next = StateID.None;
        return false;
    }


    public override void OnFixedUpdate(Player target)
    {

    }

    public override void OnUpdate(Player target)
    {
        target.HandleCamera();
        target.Move();
    }

    public override void OnLateUpdate(Player target)
    {

    }
}
