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

    [SerializeField]
    private Animator animator;

    [SerializeField]
    private float animationFadeMultiplier = 1F;


    private ThirdPersonComponent thirdPerson = null;
    private CharacterComponent character = null;

    private float hFollower = 0F, vFollower = 0F;
    private float floatCutoff = 0.005f;

    private float GetSign(float value)
    {
        if (Mathf.Abs(value) <= floatCutoff) return 0F;
        return Mathf.Sign(value);
    }


    protected override void Initialize()
    {
        base.Initialize();

        Cursor.lockState = CursorLockMode.Locked;

        thirdPerson = GetSharedComponent<ThirdPersonComponent>();
        thirdPerson.InitializeCamera(thirdPersonData);

        character = GetSharedComponent<CharacterComponent>();
    }

    public void OnUpdate()
    {
        thirdPerson.HandleMouseInput(thirdPersonData, Input.GetAxisRaw("Mouse X"), Input.GetAxisRaw("Mouse Y"));
        thirdPerson.HandleCameraLook(thirdPersonData);

        float h = Input.GetAxis("Horizontal"),
            v = Input.GetAxis("Vertical");

        hFollower += GetSign(h - hFollower) * Time.deltaTime * animationFadeMultiplier;
        if (Mathf.Abs(hFollower) <= floatCutoff) hFollower = 0F;

        vFollower += GetSign(v - vFollower) * Time.deltaTime * animationFadeMultiplier;
        if (Mathf.Abs(vFollower) <= floatCutoff) vFollower = 0F;

        bool isMoving = character.UpdateMovement(characterData, thirdPerson.GetForwardVector(thirdPersonData), h, v, out var dir);

        if (isMoving)
            model.rotation = Quaternion.Slerp(model.rotation, Quaternion.LookRotation(dir.normalized, model.up), Time.deltaTime * 10F);

        animator.SetFloat("VSpeed", Mathf.Clamp01(Mathf.Abs(h) + Mathf.Abs(v)));

        if (Input.GetMouseButtonDown(0)) animator.SetTrigger("DefaultAttack");

        Debug.DrawRay(transform.position, thirdPerson.GetForwardVector(thirdPersonData) * 5F, Color.red);
    }
}
