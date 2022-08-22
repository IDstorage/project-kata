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


    [Space(10), SerializeField]
    private Transform followTarget;


    public bool HasFollowTarget => followTarget != null;


    public bool UpdateMovement(Vector3 camForward, float h, float v, out Vector3 moveDir)
    {
        var forward = HasFollowTarget ? (followTarget.position - character.transform.position).normalized : camForward;

        moveDir = forward * v + Quaternion.AngleAxis(90F, Vector3.up) * forward * h;

        bool isMoving = Mathf.Abs(h) >= Mathf.Epsilon || Mathf.Abs(v) >= Mathf.Epsilon;
        if (!isMoving) return false;

        var moveVector = moveDir * moveSpeed * moveSpeedMultiflier;

        character.Move(moveVector * Time.deltaTime);
        return true;
    }

    public Vector3 MoveAndRotate(Vector3 forward, float h, float v)
    {
        bool isMoving = UpdateMovement(forward, h, v, out var dir);

        Quaternion targetQuat = model.rotation;

        if (isMoving)
        {
            targetQuat = Quaternion.LookRotation(dir.normalized, model.up);
        }
        if (HasFollowTarget)
        {
            targetQuat = Quaternion.LookRotation((followTarget.position - character.transform.position).normalized, character.transform.up);
        }

        model.rotation = Quaternion.Slerp(model.rotation, targetQuat, Time.deltaTime * 10F);
        return dir;
    }


    public Vector3 GetModelForward()
    {
        return model.forward;
    }


    public void SetTarget(Transform target)
    {
        followTarget = target;
    }
}
