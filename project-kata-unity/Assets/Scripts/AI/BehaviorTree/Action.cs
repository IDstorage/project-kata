namespace UnityBehaviorTree
{
    using System.Collections.Generic;
    using UnityEngine;

    // BehaviorTree return value
    public enum ReturnState
    {
        SUCCESS,
        FAILURE,
        RUNNING
    }

    // Task base
    [System.Serializable]
    public abstract class Action
    {
        // Use this instead of new operator
        public static Action Create<T>() where T : Action, new() { return new T(); }

        public static Action Failure() { return new ForceFailure(); }
        public static Action Success() { return new ForceSuccess(); }

        public abstract ReturnState Update(Stack<Action> callStack, GameObject obj, float dt);

        public virtual string Name { get; } = "Action";
        public virtual string Description { get; } = string.Empty;


#if UNITY_EDITOR
        private string guid = System.Guid.NewGuid().ToString();
        public string GUID => guid;
#endif
    }
}