using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Anomaly
{
    public abstract class State
    {
        public enum Identity
        {
            None,
            PlayerLocomotion,
            PlayerInteraction,
            PlayerAttack
        }

        public abstract Identity ID { get; }

        public abstract void OnEnter(CustomObject target);
        public abstract void OnExit(CustomObject target);

        public abstract bool IsTransition(out Identity next);

        public abstract void OnFixedUpdate(CustomObject target);
        public abstract void OnUpdate(CustomObject target);
        public abstract void OnLateUpdate(CustomObject target);

        public static State New<T>() where T : State, new()
        {
            return new T();
        }

        public static State Bind(params State[] states)
        {
            return new CompositeState(states);
        }
    }
}