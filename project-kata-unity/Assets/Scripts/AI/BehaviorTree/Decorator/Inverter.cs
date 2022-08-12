namespace UnityBehaviorTree
{
    using System.Collections.Generic;
    using UnityEngine;

    [System.Serializable]
    public class Inverter : Action
    {
        public override string Name => "Invert";
        public override string Description => "Invert child's result";

        [SerializeField]
        private Action child;

        public static Inverter Create(Action child)
        {
            Inverter i = new Inverter();
            i.child = child;
            return i;
        }

        public override ReturnState Update(Stack<Action> callStack, GameObject obj, float dt)
        {
            var ret = child.Update(callStack, obj, dt);
            callStack.Push(child);

            if (ret == ReturnState.SUCCESS) return ReturnState.FAILURE;
            if (ret == ReturnState.FAILURE) return ReturnState.SUCCESS;
            return ReturnState.RUNNING;
        }

#if UNITY_EDITOR
        public Action Child => child;
#endif
    }
}