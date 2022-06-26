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
        public CharacterController character;

        [Space(10)]
        public float moveSpeed;
        public float moveSpeedMultiflier;
    }

    public void UpdateMovement(Data target, Vector3 forward, float h, float v)
    {
        var moveDir = forward * v + Quaternion.AngleAxis(90F, Vector3.up) * forward * h;
        target.character.Move((moveDir * target.moveSpeed) * target.moveSpeedMultiflier * Time.deltaTime);
    }
}
