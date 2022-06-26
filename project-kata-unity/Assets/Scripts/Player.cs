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

        character.UpdateMovement(characterData, thirdPerson.GetForwardVector(thirdPersonData),
                                    Input.GetAxis("Horizontal"), Input.GetAxis("Vertical"));

        Debug.DrawRay(transform.position, thirdPerson.GetForwardVector(thirdPersonData), Color.red);
    }
}
