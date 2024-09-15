using System.Collections;
using System.Collections.Generic;
using UnityBehaviorTree;
using UnityEngine;

public class EnemyIfTargetIsNear : Action
{
    public override ReturnState Update(Stack<Action> callStack, Anomaly.CustomBehaviour obj, float dt)
    {
        var enemy = obj as Enemy;
        if (enemy == null) return ReturnState.FAILURE;

        return enemy.GetTargetDirection().sqrMagnitude < 49F ? ReturnState.SUCCESS : ReturnState.FAILURE;
    }
}
