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
        next = Identity.None;
        return false;
    }


    public override void OnFixedUpdate(CustomBehaviour target)
    {

    }

    public override void OnUpdate(CustomBehaviour target)
    {
        var player = target as Player;

        player.ThirdPerson.HandleMouseInput(Input.GetAxisRaw("Mouse X"), Input.GetAxisRaw("Mouse Y"));
        player.ThirdPerson.HandleCameraLook();
        player.ThirdPerson.CalculateCameraDistance();

        float h = Input.GetAxis("Horizontal"),
            v = Input.GetAxis("Vertical");

        var moveDir = player.Character.MoveAndRotate(player.ThirdPerson.GetForwardVector(), h, v);

        player.Animator.SetFloat("VSpeed", Mathf.Clamp01(Mathf.Abs(h) + Mathf.Abs(v)));

        if (Input.GetMouseButtonDown(0))
        {
            player.StateMachine.ChangeState(Identity.PlayerAttack);
            //animator.SetTrigger(animatorData, "DefaultAttack");
        }
        //animator.SetBool(animatorData, "IsBlocking", Input.GetMouseButton(1));

        Debug.DrawRay(target.transform.position, player.ThirdPerson.GetForwardVector() * 5F, Color.red);
    }

    public override void OnLateUpdate(CustomBehaviour target)
    {

    }
}
