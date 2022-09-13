using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Anomaly;
using Anomaly.Utils;

public class PlayerDefenseState : State<Player>
{
    public override StateID ID => StateID.PlayerDefense;


    private float hFollow = 0F, vFollow = 0F;


    public override void OnEnter(Player target)
    {
        target.Animator.SetBool("IsBlocking", true);

        target.Combat.AddPose(CombatComponent.Pose.Defense);
    }

    public override void OnExit(Player target)
    {
        target.Animator.SetBool("IsBlocking", false);

        target.Combat.ReleasePose(CombatComponent.Pose.Defense);
    }


    public override bool IsTransition(Player target, out StateID next)
    {
        if (!AInput.IsHeld(CustomKey.Current.Defense))
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
        target.Move();
    }

    public override void OnLateUpdate(Player target)
    {

    }
}
