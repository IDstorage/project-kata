using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Anomaly;

public class Player : CustomBehaviour
{
    public ThirdPersonComponent ThirdPerson;
    public CharacterComponent Character;
    public AnimatorComponent Animator;

    public StateMachineComponent StateMachine;


    protected override void Initialize()
    {
        base.Initialize();

        Cursor.lockState = CursorLockMode.Locked;

        StateMachine.Run(0, new PlayerLocomotionState(), new PlayerAttackState());
    }

    public void OnFixedUpdate()
    {
    }

    public void OnUpdate()
    {
        // thirdPerson.HandleMouseInput(thirdPersonData, Input.GetAxisRaw("Mouse X"), Input.GetAxisRaw("Mouse Y"));
        // thirdPerson.HandleCameraLook(thirdPersonData);
        // thirdPerson.CalculateCameraDistance(thirdPersonData);

        // float h = Input.GetAxis("Horizontal"),
        //     v = Input.GetAxis("Vertical");

        // bool isMoving = character.UpdateMovement(characterData, thirdPerson.GetForwardVector(thirdPersonData), h, v, out var dir);

        // if (isMoving)
        //     model.rotation = Quaternion.Slerp(model.rotation, Quaternion.LookRotation(dir.normalized, model.up), Time.deltaTime * 10F);

        // animator.SetFloat(animatorData, "VSpeed", Mathf.Clamp01(Mathf.Abs(h) + Mathf.Abs(v)));

        // if (Input.GetMouseButtonDown(0)) animator.SetTrigger(animatorData, "DefaultAttack");
        // animator.SetBool(animatorData, "IsBlocking", Input.GetMouseButton(1));

        // Debug.DrawRay(transform.position, thirdPerson.GetForwardVector(thirdPersonData) * 5F, Color.red);
    }
}
