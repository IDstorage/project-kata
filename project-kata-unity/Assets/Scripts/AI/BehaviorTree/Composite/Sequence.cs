namespace UnityBehaviorTree
{
    using System.Collections.Generic;
    using UnityEngine;

    [System.Serializable]
    public class Sequence : Action
    {
        public override string Name => "Sequence";
        public override string Description => "Execute children sequently";

        [SerializeField]
        private List<Action> children = new List<Action>();
        private int latest = 0;

        public static Sequence Create(params Action[] nodes)
        {
            Sequence s = new Sequence();
            s.children.AddRange(nodes);
            return s;
        }

        public override ReturnState Update(Stack<Action> callStack, Anomaly.CustomBehaviour obj, float dt)
        {
            while (latest < children.Count)
            {
                var ret = children[latest].Update(callStack, obj, dt);
                callStack.Push(children[latest]);

                if (ret == ReturnState.SUCCESS)
                {
                    latest++;
                    continue;
                }

                if (ret == ReturnState.FAILURE) latest = 0;
                return ret;
            }

            latest = 0;
            return ReturnState.SUCCESS;
        }

#if UNITY_EDITOR
        public List<Action> Children => children;
#endif
    }
}