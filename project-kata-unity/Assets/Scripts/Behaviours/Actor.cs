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


    public void SetHP(float hp, bool notify = true)
    {
        Status.hp = Mathf.Clamp(hp, 0F, Status.maximumHP);
        if (notify) Status.onHPChanged?.Invoke(hp / Status.maximumHP);
    }
    public void AddHP(float value, bool notify = true) => SetHP(Status.hp + value, notify);

    public void SetPosture(float posture, bool notify = true)
    {
        Status.posture = Mathf.Clamp01(posture);
        if (notify) Status.onPostureChanged?.Invoke(1F - posture);
    }
    public void AddPosture(float value, bool notify = true) => SetPosture(Status.posture + value, notify);
}
