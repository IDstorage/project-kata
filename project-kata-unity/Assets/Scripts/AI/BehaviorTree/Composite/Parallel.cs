namespace UnityBehaviorTree
{
    using System.Collections.Generic;
    using UnityEngine;

    // Execute tasks at the same time
    [System.Serializable]
    public class Parallel : Action
    {
        public override string Name => "Parallel";
        public override string Description => "Execute children at the same time";

        [SerializeField]
        private List<Action> children = new List<Action>();
        private int[] indices;
        private int successCount = 0;

        public static Parallel Create(params Action[] nodes)
        {
            Parallel s = new Parallel();
            s.children.AddRange(nodes);
            s.indices = new int[nodes.Length];
            for (int i = 0; i < s.indices.Length; ++i) s.indices[i] = i;
            return s;
        }

        public override ReturnState Update(Stack<Action> callStack, Anomaly.CustomBehaviour obj, float dt)
        {
            for (int i = 0; i < indices.Length - successCount; ++i)
            {
                var ret = children[indices[i]].Update(callStack, obj, dt);
                callStack.Push(children[indices[i]]);

                if (ret == ReturnState.FAILURE) return ReturnState.FAILURE;

                if (ret == ReturnState.SUCCESS)
                {
                    int temp = indices[i];
                    indices[i] = indices[indices.Length - successCount - 1];
                    indices[indices.Length - successCount - 1] = temp;
                    successCount++;
                }
            }

            if (successCount == indices.Length)
            {
                successCount = 0;
                return ReturnState.SUCCESS;
            }
            return ReturnState.RUNNING;
        }

#if UNITY_EDITOR
        public List<Action> Children => children;
#endif
    }

}