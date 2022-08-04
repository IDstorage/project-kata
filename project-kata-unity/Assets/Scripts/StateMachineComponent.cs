using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Anomaly;

public class StateMachineComponent : CustomComponent
{
    [System.Serializable]
    [SharedComponentData(typeof(StateMachineComponent))]
    public class Data : CustomComponent.BaseData
    {
        public CustomObject caller;
        [SerializeField] private List<State> states = new List<State>();


        public List<State> States => states;

        public State CurrentState { get; set; }


        public void AddStates(params State[] states)
        {
            this.states.AddRange(states);
        }
    }


    public void Run(Data target, int entryIndex = 0)
    {
        Debug.Assert(entryIndex >= 0 && entryIndex < target.States.Count);
        ChangeState(target, entryIndex);
    }

    public void ChangeState(Data target, State.Identity id)
    {
        ChangeState(target, target.States.FindIndex(s => s.ID == id));
    }

    public void ChangeState(Data target, int index)
    {
        target.CurrentState?.OnExit(target.caller);
        target.CurrentState = target.States[index];
        target.CurrentState?.OnEnter(target.caller);
    }

    public void OnFixedUpdate(Data target)
    {
        target.CurrentState?.OnFixedUpdate(target.caller);
    }
    public void OnUpdate(Data target)
    {
        if (target.CurrentState != null && target.CurrentState.IsTransition(out var next))
        {
            ChangeState(target, next);
        }
        target.CurrentState?.OnUpdate(target.caller);
    }
    public void OnLateUpdate(Data target)
    {
        target.CurrentState?.OnLateUpdate(target.caller);
    }
}
