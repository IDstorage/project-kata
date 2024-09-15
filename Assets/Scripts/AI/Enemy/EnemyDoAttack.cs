using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityBehaviorTree;
using Anomaly;

public class EnemyDoAttack : Action
{
    private Enemy enemy;
    private Anomaly.Utils.SmartCoroutine coroutine;

    public override ReturnState Update(Stack<Action> callStack, CustomBehaviour obj, float dt)
    {
        if (enemy == null) enemy = obj as Enemy;
        if (coroutine == null) coroutine = Anomaly.Utils.SmartCoroutine.Create(CoAttackLoop);

        if (enemy == null || coroutine.IsRunning) return ReturnState.FAILURE;

        coroutine.Start();
        return ReturnState.SUCCESS;
    }

    private IEnumerator CoAttackLoop()
    {
        enemy.Animator.SetTrigger("DefaultAttack");
        yield return new WaitForSeconds(3F);
    }
}
