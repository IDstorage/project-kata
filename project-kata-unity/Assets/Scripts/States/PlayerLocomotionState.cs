using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Anomaly;


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
        if (Input.GetMouseButton(1))
        {
            next = StateID.PlayerDefense;
            return true;
        }
        if (Input.GetMouseButtonDown(0))
        {
            next = StateID.PlayerAttack;
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
