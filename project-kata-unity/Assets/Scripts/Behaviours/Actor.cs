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
}
