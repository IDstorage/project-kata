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


    [SerializeField]
    private HitEventStream hitEventStream;


    protected override void Initialize()
    {
        base.Initialize();

        Cursor.lockState = CursorLockMode.Locked;

        StateMachine.Run(0,
            new PlayerLocomotionState(),
            new PlayerAttackState(),
            new PlayerDefenseState(),
            new PlayerBlockState());

        hitEventStream = new HitEventStream(this);
        hitEventStream.AddListener(Hit);
    }


    public void Attack()
    {
        StateMachine.ChangeState(State.Identity.PlayerAttack);
    }


    public void Hit(HitEvent e)
    {
        if (StateMachine.CurrentState.ID == State.Identity.PlayerDefense)
        {
            Block();
            return;
        }

        Debug.Log($"Hit! {e.sender.name} -> {e.receiver.name}");
    }

    public void Block()
    {
        Debug.Log("Block");
        StateMachine.ChangeState(State.Identity.PlayerBlock);
    }
}
