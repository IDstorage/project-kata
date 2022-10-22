using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Anomaly;
using UnityBehaviorTree;

public class Enemy : Actor, ICombat
{
    public BehaviorTreeComponent BehaviorTree;
    public CombatComponent Combat;

    private float hFollow = 0F, vFollow = 0F;

    [SerializeField] private CustomBehaviour target;

    protected override void Initialize()
    {
        base.Initialize();

        Cursor.lockState = CursorLockMode.Locked;

        Combat.Initialize();


        BehaviorTree.SetBehaviorTree(
        UnityBehaviorTree.BehaviorTree.Create(
            Sequence.Create(
                If.Create(
                    Action.Create<EnemyIfTargetIsNear>(),
                    Action.Create<EnemyDoAttack>()
                )
            )
        ));
    }


    public void Move()
    {
        // float h = Input.GetAxis("Horizontal"),
        //     v = Input.GetAxis("Vertical");

        // hFollow = Mathf.Lerp(hFollow, h, Time.deltaTime * 15F);
        // vFollow = Mathf.Lerp(vFollow, v, Time.deltaTime * 15F);

        // var moveDir = Character.MoveAndRotate(ThirdPerson.GetForwardVector(), h, v);

        // SetAniParam(hFollow, vFollow);

        // Debug.DrawRay(transform.position, ThirdPerson.GetForwardVector() * 5F, Color.red);


        // void SetAniParam(float _h, float _v)
        // {
        //     _v = Character.HasFollowTarget ? _v : Mathf.Max(Mathf.Abs(_h), Mathf.Abs(_v));
        //     _h = Character.HasFollowTarget ? _h : 0F;
        //     Animator.SetFloat("HSpeed", _h);
        //     Animator.SetFloat("VSpeed", _v);
        // }
    }


    #region Enemy
    public Vector3 GetTargetDirection()
    {
        if (target == null) return Vector3.positiveInfinity;

        return target.transform.position - transform.position;
    }
    #endregion


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
        //StateMachine.ChangeState(StateID.Attack);
    }

    public void Block(CustomBehaviour other)
    {
        // Debug.Log($"{this.name}: Block");

        // StateMachine.ChangeState(StateID.Block);

        // Status.AddPosture(-0.075f);
        // Status.ResetParryTiming();
    }

    public void Parry(CustomBehaviour other)
    {
        // Debug.Log($"{this.name}: Parry!");

        // StateMachine.ChangeState(StateID.Parry);

        // Status.AddPosture(-0.05f);
        // Status.DecreaseParryTiming();
    }


    public void OnHit(CustomBehaviour other, params Collider[] hitParts)
    {
        // if (StateMachine.CurrentState.ID != StateID.Defense)
        // {
        //     Status.AddHP(-10F);
        //     Status.AddPosture(-0.125f);
        //     Debug.Log($"{this.name}: Hit by {other.name}");
        //     return;
        // }

        // foreach (var part in hitParts)
        // {
        //     if (!part.CompareTag("Weapon")) continue;

        //     if (Combat.CanParry) Parry();
        //     else Block();

        //     break;
        // }
    }

    public void OnBlocked(CustomBehaviour other)
    {
    }

    public void OnParried(CustomBehaviour other)
    {
        Debug.Log($"{this.name}: Parried!");
    }

    public void TryParry()
    {
        Combat.TryParry(status.defenseToParryInterval, status.currentParryTiming);
    }
    #endregion
}
