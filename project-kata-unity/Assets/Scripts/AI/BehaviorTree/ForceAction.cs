namespace UnityBehaviorTree
{
    using System.Collections.Generic;
    using UnityEngine;

    [System.Serializable]
    public class ForceFailure : Action
    {
        public override ReturnState Update(Stack<Action> callStack, GameObject obj, float dt)
        {
            return ReturnState.FAILURE;
        }
    }

    [System.Serializable]
    public class ForceSuccess : Action
    {
        public override ReturnState Update(Stack<Action> callStack, GameObject obj, float dt)
        {
            return ReturnState.SUCCESS;
        }
    }
}