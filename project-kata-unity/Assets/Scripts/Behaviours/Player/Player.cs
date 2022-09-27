using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Anomaly;

public class Player : Actor, ICombat
{
    public ThirdPersonComponent ThirdPerson;

    public StateMachineComponent<Player> StateMachine;

    public CombatComponent Combat;


    private float hFollow = 0F, vFollow = 0F;


    protected override void Initialize()
    {
        base.Initialize();

        Cursor.lockState = CursorLockMode.Locked;

        StateMachine.Run(0,
            new PlayerLocomotionState(),
            new PlayerAttackState(),
            new PlayerDefenseState(),
            new PlayerBlockState(),
            new PlayerParryState());

        Combat.Initialize();
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
            _v = Character.HasFollowTarget ? _v : Mathf.Max(Mathf.Abs(_h), Mathf.Abs(_v));
            _h = Character.HasFollowTarget ? _h : 0F;
            Animator.SetFloat("HSpeed", _h);
            Animator.SetFloat("VSpeed", _v);
        }
    }


    public void OnTargeting(UnityEngine.InputSystem.InputAction.CallbackContext context)
    {
        if (!context.performed) return;

        if (!ThirdPerson.Targeting(this, out var target)) return;

        Character.SetTarget(target);
        ThirdPerson.SetTarget(target);
    }


    #region Animation Event
    public void TrailCast(AnimationEvent param)
    {
        Combat.TrailCast(param.stringParameter.Split('|')[1].Trim(), param.intParameter);
    }

    public void CollectInputEvent(AnimationEvent param)
    {
        Combat.CollectInputEvent();
    }
    public void StopInputEvent(AnimationEvent param)
    {
        Combat.StopInputEvent();
    }

    public void AddImpulseForward(AnimationEvent param)
    {
        CharacterPhysics.SetForceAttenScale(param.intParameter);
        CharacterPhysics.AddImpulse(Character.GetModelForward(), param.floatParameter);
    }
    public void AddImpulseBackward(AnimationEvent param)
    {
        CharacterPhysics.SetForceAttenScale(param.intParameter);
        CharacterPhysics.AddImpulse(-Character.GetModelForward(), param.floatParameter);
    }
    #endregion


    #region Combat
    public void Attack()
    {
        StateMachine.ChangeState(StateID.Attack);
    }

    public void Block()
    {
        Debug.Log($"{this.name}: Block");

        StateMachine.ChangeState(StateID.Block);

        Status.AddPosture(-0.075f);
        Status.ResetParryTiming();
    }

    public void Parry()
    {
        Debug.Log($"{this.name}: Parry!");

        StateMachine.ChangeState(StateID.Parry);

        Status.AddPosture(-0.05f);
        Status.DecreaseParryTiming();
    }


    public void OnHit(CustomBehaviour other, params Collider[] hitParts)
    {
        if (StateMachine.CurrentState.ID != StateID.Defense)
        {
            Status.AddHP(-10F);
            Status.AddPosture(-0.125f);
            Debug.Log($"{this.name}: Hit by {other.name}");
            return;
        }

        foreach (var part in hitParts)
        {
            if (!part.CompareTag("Weapon")) continue;

            if (Combat.CanParry) Parry();
            else Block();

            break;
        }
    }

    public void OnBlocked(CustomBehaviour other)
    {
    }

    public void OnParried(CustomBehaviour other)
    {
    }

    public void TryParry()
    {
        Combat.TryParry(status.defenseToParryInterval, status.currentParryTiming);
    }
    #endregion
}
