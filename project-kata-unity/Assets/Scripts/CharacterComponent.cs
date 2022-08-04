using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Anomaly;

public class CharacterComponent : CustomComponent
{
    [System.Serializable]
    [SharedComponentData(typeof(CharacterComponent))]
    public class Data : CustomComponent.BaseData
    {
        public Transform model;
        public CharacterController character;

        [Space(10)]
        public float moveSpeed;
        public float moveSpeedMultiflier;
    }

    public bool UpdateMovement(Data target, Vector3 camForward, float h, float v, out Vector3 moveDir)
    {
        moveDir = camForward * v + Quaternion.AngleAxis(90F, Vector3.up) * camForward * h;
        //moveDir.Normalize();
        //var moveDir = target.character.transform.forward * v + target.character.transform.right * h;

        bool isMoving = Mathf.Abs(h) >= Mathf.Epsilon || Mathf.Abs(v) >= Mathf.Epsilon;
        if (!isMoving) return false;

        //target.character.transform.rotation = Quaternion.Slerp(target.character.transform.rotation, quat, Time.deltaTime * 10F);
        //target.character.Move((target.character.transform.forward * target.moveSpeed) * target.moveSpeedMultiflier * Time.deltaTime);
        target.character.Move((moveDir * target.moveSpeed) * target.moveSpeedMultiflier * Time.deltaTime);
        return true;
    }

    public Vector3 MoveAndRotate(Data target, Vector3 forward, float h, float v)
    {
        bool isMoving = UpdateMovement(target, forward, h, v, out var dir);

        if (isMoving)
        {
            target.model.rotation = Quaternion.Slerp(target.model.rotation, Quaternion.LookRotation(dir.normalized, target.model.up), Time.deltaTime * 10F);
        }

        return dir;
    }
}
