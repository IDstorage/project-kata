namespace UnityBehaviorTree
{
    using System.Collections.Generic;
    using UnityEngine;

    [System.Serializable]
    public class If : Action
    {
        public override string Name => "If";
        public override string Description => string.Empty;

        [SerializeField]
        private Action condition;

        [SerializeField]
        private Action child;

        public static If Create(Action condition, Action child)
        {
            If i = new If();
            i.child = child;
            i.condition = condition;
            return i;
        }

        public override ReturnState Update(Stack<Action> callStack, GameObject obj, float dt)
        {
            bool ret = condition.Update(callStack, obj, dt) != ReturnState.FAILURE;

            callStack.Push(child);
            if (ret) return child.Update(callStack, obj, dt);
            return ReturnState.FAILURE;
        }

#if UNITY_EDITOR
        public Action Child => child;
        public Action Condition => condition;
#endif
    }
}