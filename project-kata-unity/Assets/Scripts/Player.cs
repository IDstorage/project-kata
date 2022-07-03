using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Anomaly;

public class Player : CustomObject
{
    [SerializeField] private ThirdPersonComponent.Data thirdPersonData;
    [SerializeField] private CharacterComponent.Data characterData;
    [SerializeField] private AnimatorComponent.Data animatorData;

    [SerializeField]
    private Transform model;

    [SerializeField]
    private float animationFadeMultiplier = 1F;


    private ThirdPersonComponent thirdPerson = null;
    private CharacterComponent character = null;
    private AnimatorComponent animator = null;


    protected override void Initialize()
    {
        base.Initialize();

        Cursor.lockState = CursorLockMode.Locked;

        thirdPerson = GetSharedComponent<ThirdPersonComponent>();
        thirdPerson.InitializeCamera(thirdPersonData);

        character = GetSharedComponent<CharacterComponent>();

        animator = GetSharedComponent<AnimatorComponent>();
    }

    public void OnFixedUpdate()
    {

    }

    public void OnUpdate()
    {
        thirdPerson.HandleMouseInput(thirdPersonData, Input.GetAxisRaw("Mouse X"), Input.GetAxisRaw("Mouse Y"));
        thirdPerson.HandleCameraLook(thirdPersonData);
        thirdPerson.CalculateCameraDistance(thirdPersonData);

        float h = Input.GetAxis("Horizontal"),
            v = Input.GetAxis("Vertical");

        bool isMoving = character.UpdateMovement(characterData, thirdPerson.GetForwardVector(thirdPersonData), h, v, out var dir);

        if (isMoving)
            model.rotation = Quaternion.Slerp(model.rotation, Quaternion.LookRotation(dir.normalized, model.up), Time.deltaTime * 10F);

        animator.SetFloat(animatorData, "VSpeed", Mathf.Clamp01(Mathf.Abs(h) + Mathf.Abs(v)));

        if (Input.GetMouseButtonDown(0)) animator.SetTrigger(animatorData, "DefaultAttack");
        animator.SetBool(animatorData, "IsBlocking", Input.GetMouseButton(1));

        Debug.DrawRay(transform.position, thirdPerson.GetForwardVector(thirdPersonData) * 5F, Color.red);
    }
}
