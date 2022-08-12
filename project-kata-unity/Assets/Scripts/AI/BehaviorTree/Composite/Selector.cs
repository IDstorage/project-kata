namespace UnityBehaviorTree
{
    using System.Collections.Generic;
    using UnityEngine;

    [System.Serializable]
    public class Selector : Action
    {
        public override string Name => "Selector";
        public override string Description => "Select child sequently";

        [SerializeField]
        private List<Action> children = new List<Action>();
        private int latest = 0;

        public static Selector Create(params Action[] nodes)
        {
            Selector s = new Selector();
            s.children.AddRange(nodes);
            return s;
        }

        public override ReturnState Update(Stack<Action> callStack, GameObject obj, float dt)
        {
            while (latest < children.Count)
            {
                var ret = children[latest].Update(callStack, obj, dt);
                callStack.Push(children[latest]);

                if (ret == ReturnState.FAILURE)
                {
                    latest++;
                    continue;
                }

                if (ret == ReturnState.SUCCESS) latest = 0;
                return ret;
            }

            latest = 0;
            return ReturnState.FAILURE;
        }

#if UNITY_EDITOR
        public List<Action> Children => children;
#endif
    }
}