using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Anomaly;


public class PlayerAttackState : State<Player>
{
    public override StateID ID => StateID.PlayerAttack;


    public override void OnEnter(Player target)
    {
        target.Animator.SetTrigger("DefaultAttack");
    }

    public override void OnExit(Player target)
    {

    }


    public override bool IsTransition(Player target, out StateID next)
    {
        if (target.Animator.GetNextState().IsName("Locomotion"))
        {
            next = StateID.PlayerLocomotion;
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

        Debug.DrawRay(target.transform.position, target.ThirdPerson.GetForwardVector() * 5F, Color.red);
    }

    public override void OnLateUpdate(Player target)
    {

    }
}
