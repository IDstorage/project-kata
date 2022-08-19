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
        if (!Input.GetMouseButton(1))
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

        player.Move();
        // player.ThirdPerson.HandleMouseInput(Input.GetAxisRaw("Mouse X"), Input.GetAxisRaw("Mouse Y"));
        // player.ThirdPerson.HandleCameraLook();
        // player.ThirdPerson.CalculateCameraDistance();

        // float h = Input.GetAxis("Horizontal"),
        //     v = Input.GetAxis("Vertical");

        // hFollow = Mathf.Lerp(hFollow, h, Time.deltaTime * 15F);
        // vFollow = Mathf.Lerp(vFollow, v, Time.deltaTime * 15F);

        // var moveDir = player.Character.MoveAndRotate(player.ThirdPerson.GetForwardVector(), hFollow, vFollow);

        // player.Animator.SetFloat("VSpeed", Mathf.Clamp01(Mathf.Abs(hFollow) + Mathf.Abs(vFollow)));

        // Debug.DrawRay(target.transform.position, player.ThirdPerson.GetForwardVector() * 5F, Color.red);
    }

    public override void OnLateUpdate(CustomBehaviour target)
    {

    }
}
