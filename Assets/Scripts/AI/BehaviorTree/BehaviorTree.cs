namespace UnityBehaviorTree
{
    using System.Collections.Generic;
    using UnityEngine;

    [System.Serializable]
    public class BehaviorTree
    {
        [SerializeField]
        private Sequence sequence;

        private Stack<Action> callStack = new Stack<Action>();

        public static BehaviorTree Create(Sequence seq)
        {
            return new BehaviorTree() { sequence = seq };
        }

        public ReturnState Update(Anomaly.CustomBehaviour obj, float dt)
        {
            callStack.Clear();
            callStack.Push(sequence);

            return sequence.Update(callStack, obj, dt);
        }

#if UNITY_EDITOR
        public Sequence Sequence => sequence;
        public Stack<Action> CallStack => callStack;
#endif
    }
}
