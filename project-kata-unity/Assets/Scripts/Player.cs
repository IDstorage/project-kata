using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Anomaly;

public class Player : CustomObject
{
    [SerializeField]
    private ThirdPersonComponent.Data thirdPersonData;

    [SerializeField]
    private CharacterComponent.Data characterData;

    [SerializeField]
    private Transform model;

    private ThirdPersonComponent thirdPerson = null;
    private CharacterComponent character = null;

    protected override void Initialize()
    {
        base.Initialize();

        thirdPerson = GetSharedComponent<ThirdPersonComponent>();
        thirdPerson.InitializeCamera(thirdPersonData);

        character = GetSharedComponent<CharacterComponent>();
    }

    public void OnUpdate()
    {
        thirdPerson.HandleMouseInput(thirdPersonData, Input.GetAxisRaw("Mouse X"), Input.GetAxisRaw("Mouse Y"));
        thirdPerson.HandleCameraLook(thirdPersonData);

        var dir = character.UpdateMovement(characterData, thirdPerson.GetForwardVector(thirdPersonData),
                                                 Input.GetAxis("Horizontal"), Input.GetAxis("Vertical"));

        if (dir != Vector3.zero)
            model.rotation = Quaternion.Slerp(model.rotation, Quaternion.LookRotation(dir, model.up), Time.deltaTime * 20F);

        Debug.DrawRay(transform.position, thirdPerson.GetForwardVector(thirdPersonData) * 5F, Color.red);
    }
}
