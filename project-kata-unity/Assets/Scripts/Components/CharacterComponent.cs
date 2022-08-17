using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Anomaly;

[System.Serializable]
public class CharacterComponent : CustomComponent
{
    [SerializeField]
    private Transform model;
    [SerializeField]
    private CharacterController character;

    [Space(10), SerializeField]
    private float moveSpeed;
    [SerializeField]
    private float moveSpeedMultiflier;

    public bool UpdateMovement(Vector3 camForward, float h, float v, out Vector3 moveDir)
    {
        moveDir = camForward * v + Quaternion.AngleAxis(90F, Vector3.up) * camForward * h;

        bool isMoving = Mathf.Abs(h) >= Mathf.Epsilon || Mathf.Abs(v) >= Mathf.Epsilon;
        if (!isMoving) return false;

        var moveVector = moveDir * moveSpeed * moveSpeedMultiflier;

        character.Move(moveVector * Time.deltaTime);
        return true;
    }

    public Vector3 MoveAndRotate(Vector3 forward, float h, float v)
    {
        bool isMoving = UpdateMovement(forward, h, v, out var dir);

        if (isMoving)
        {
            model.rotation = Quaternion.Slerp(model.rotation, Quaternion.LookRotation(dir.normalized, model.up), Time.deltaTime * 10F);
        }

        return dir;
    }


    public Vector3 GetModelForward()
    {
        return model.forward;
    }
}
