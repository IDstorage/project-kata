using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Anomaly;


public class PlayerLocomotionState : State
{
    public override Identity ID => State.Identity.PlayerLocomotion;

    private Vector3 handlePos = new Vector3(5F, 0.5f, -10F);

    private CharacterComponent character;
    private CharacterComponent.Data characterData;

    private ThirdPersonComponent thirdPerson;
    private ThirdPersonComponent.Data thirdPersonData;

    private AnimatorComponent animator;
    private AnimatorComponent.Data animatorData;


    public override void OnEnter(CustomObject target)
    {
        character = target.GetSharedComponent<CharacterComponent>();
        characterData = target.GetComponentData<CharacterComponent.Data>();

        thirdPerson = target.GetSharedComponent<ThirdPersonComponent>();
        thirdPersonData = target.GetComponentData<ThirdPersonComponent.Data>();

        animator = target.GetSharedComponent<AnimatorComponent>();
        animatorData = target.GetComponentData<AnimatorComponent.Data>();
    }

    public override void OnExit(CustomObject target)
    {

    }


    public override bool IsTransition(out Identity next)
    {
        next = Identity.None;
        return false;
    }


    public override void OnFixedUpdate(CustomObject target)
    {

    }

    public override void OnUpdate(CustomObject target)
    {
        thirdPerson.HandleMouseInput(thirdPersonData, Input.GetAxisRaw("Mouse X"), Input.GetAxisRaw("Mouse Y"));
        thirdPerson.HandleCameraLook(thirdPersonData);
        thirdPerson.CalculateCameraDistance(thirdPersonData);

        float h = Input.GetAxis("Horizontal"),
            v = Input.GetAxis("Vertical");

        var moveDir = character.MoveAndRotate(characterData, thirdPerson.GetForwardVector(thirdPersonData), h, v);

        animator.SetFloat(animatorData, "VSpeed", Mathf.Clamp01(Mathf.Abs(h) + Mathf.Abs(v)));

        if (Input.GetMouseButtonDown(0))
        {
            var stateMachineData = target.GetComponentData<StateMachineComponent.Data>();
            target.GetSharedComponent<StateMachineComponent>().ChangeState(stateMachineData, Identity.PlayerAttack);
            //animator.SetTrigger(animatorData, "DefaultAttack");
        }
        //animator.SetBool(animatorData, "IsBlocking", Input.GetMouseButton(1));

        Debug.DrawRay(target.transform.position, thirdPerson.GetForwardVector(thirdPersonData) * 5F, Color.red);
    }

    public override void OnLateUpdate(CustomObject target)
    {

    }
}
