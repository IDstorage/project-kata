using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Anomaly;

public class Actor : CustomBehaviour
{
    public CharacterComponent Character;
    public CharacterPhysicsComponent CharacterPhysics;

    public AnimatorComponent Animator;

    [SerializeField]
    protected ActorStatus status;
    public ActorStatus Status => status;


    protected override void Initialize()
    {
        base.Initialize();

        status?.Initialize();
    }


    public void SetHP(float hp)
    {
        if (status == null) return;

        status.hp = hp;
    }
    public void AddHP(float value)
    {
        if (status == null) return;

        status.hp += value;
    }

    public void SetPosture(float posture)
    {
        if (status == null) return;

        status.posture = Mathf.Clamp01(posture);
    }
    public void AddPosture(float value)
    {
        if (status == null) return;

        status.posture += value;
    }
}
