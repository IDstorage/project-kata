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


    private float hFollow = 0F, vFollow = 0F;


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


    public void HandleCamera()
    {
        ThirdPerson.HandleMouseInput(Input.GetAxisRaw("Mouse X"), Input.GetAxisRaw("Mouse Y"));
        ThirdPerson.HandleCameraLook();
        ThirdPerson.CalculateCameraDistance();
    }

    public void Move()
    {
        float h = Input.GetAxis("Horizontal"),
            v = Input.GetAxis("Vertical");

        hFollow = Mathf.Lerp(hFollow, h, Time.deltaTime * 15F);
        vFollow = Mathf.Lerp(vFollow, v, Time.deltaTime * 15F);

        var moveDir = Character.MoveAndRotate(ThirdPerson.GetForwardVector(), h, v);

        SetAniParam(hFollow, vFollow);

        Debug.DrawRay(transform.position, ThirdPerson.GetForwardVector() * 5F, Color.red);


        void SetAniParam(float _h, float _v)
        {
            _h = Character.HasFollowTarget ? _h : 0F;
            _v = Character.HasFollowTarget ? _v : Mathf.Clamp01(Mathf.Abs(_h) + Mathf.Abs(_v));
            Animator.SetFloat("HSpeed", _h);
            Animator.SetFloat("VSpeed", _v);
        }
    }


    public void Attack()
    {
        StateMachine.ChangeState(State.Identity.PlayerAttack);
    }


    public void Hit(HitEvent e)
    {
        if (StateMachine.CurrentState.ID == State.Identity.PlayerDefense
            && e.hitPart.CompareTag("Weapon"))
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
