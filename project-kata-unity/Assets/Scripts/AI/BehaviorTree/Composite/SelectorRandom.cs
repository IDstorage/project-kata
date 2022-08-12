namespace UnityBehaviorTree
{
    using System.Collections.Generic;
    using UnityEngine;

    [System.Serializable]
    public class SelectorRandom : Action
    {
        public override string Name => "Selector(R)";
        public override string Description => "Select child randomly";

        [SerializeField]
        private List<Action> children = new List<Action>();
        private int latest = -1;

        public static SelectorRandom Create(params Action[] nodes)
        {
            SelectorRandom s = new SelectorRandom();
            s.children.AddRange(nodes);
            return s;
        }

        public override ReturnState Update(Stack<Action> callStack, GameObject obj, float dt)
        {
            if (latest < 0) latest = Random.Range(0, children.Count);

            var ret = children[latest].Update(callStack, obj, dt);
            callStack.Push(children[latest]);

            if (ret == ReturnState.SUCCESS)
            {
                latest = -1;
            }

            return ret;
        }

#if UNITY_EDITOR
        public List<Action> Children => children;
#endif
    }
}