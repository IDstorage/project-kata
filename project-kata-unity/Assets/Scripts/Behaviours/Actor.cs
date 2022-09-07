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

    private ActorStatus copiedStatus;
    public ActorStatus Status => copiedStatus ?? (copiedStatus = status.Clone());


    protected override void Initialize()
    {
        base.Initialize();

        Status?.Initialize();
    }


    public void SetHP(float hp)
    {
        Status.hp = Mathf.Clamp(hp, 0F, Status.maximumHP);
    }
    public void AddHP(float value) => SetHP(Status.hp + value);

    public void SetPosture(float posture)
    {
        Status.posture = Mathf.Clamp01(posture);
    }
    public void AddPosture(float value) => SetPosture(Status.posture + value);
}
