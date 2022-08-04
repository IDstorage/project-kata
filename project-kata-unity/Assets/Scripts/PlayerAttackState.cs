using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Anomaly;


public class PlayerAttackState : State
{
    public override Identity ID => State.Identity.PlayerAttack;

    private ThirdPersonComponent thirdPerson;
    private ThirdPersonComponent.Data thirdPersonData;


    public override void OnEnter(CustomObject target)
    {
        thirdPerson = target.GetSharedComponent<ThirdPersonComponent>();
        thirdPersonData = target.GetComponentData<ThirdPersonComponent.Data>();
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

        Debug.DrawRay(target.transform.position, thirdPerson.GetForwardVector(thirdPersonData) * 5F, Color.red);
    }

    public override void OnLateUpdate(CustomObject target)
    {

    }
}
