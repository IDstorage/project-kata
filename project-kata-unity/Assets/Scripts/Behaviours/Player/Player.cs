using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Anomaly;

public class Player : CustomBehaviour
{
    public ThirdPersonComponent ThirdPerson;
    public CharacterComponent Character;
    public AnimatorComponent Animator;

    public StateMachineComponent StateMachine;


    protected override void Initialize()
    {
        base.Initialize();

        Cursor.lockState = CursorLockMode.Locked;

        StateMachine.Run(0,
            new PlayerLocomotionState(),
            new PlayerAttackState(),
            new PlayerDefenseState());
    }


    public void Attack()
    {
        StateMachine.ChangeState(State.Identity.PlayerAttack);
    }
}
