using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Anomaly;

public class Actor : CustomBehaviour
{
    public CharacterComponent Character;
    public CharacterPhysicsComponent CharacterPhysics;

    public AnimatorComponent Animator;

    public StateMachineComponent StateMachine;
}
