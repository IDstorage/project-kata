using System.Collections.Generic;
using UnityEngine;
using Anomaly;

[System.Serializable]
public class StateMachineComponent : CustomComponent, IFixedUpdater, IUpdater, ILateUpdater
{
    [SerializeField]
    private CustomBehaviour caller;

    private List<State> states = new List<State>();

    public State CurrentState { get; private set; }


    public void Run(int entryIndex = 0, params State[] states)
    {
        this.states.AddRange(states);

        Debug.Assert(entryIndex >= 0 && entryIndex < this.states.Count);
        ChangeState(entryIndex);
    }

    public void ChangeState(State.Identity id)
    {
        ChangeState(states.FindIndex(s => s.ID == id));
    }

    public void ChangeState(int index)
    {
        CurrentState?.OnExit(caller);
        CurrentState = states[index];
        CurrentState?.OnEnter(caller);
    }

    public void Update()
    {
        if (CurrentState != null && CurrentState.IsTransition(caller, out var next))
        {
            ChangeState(next);
        }
        CurrentState?.OnUpdate(caller);
    }

    public void FixedUpdate()
    {
        CurrentState?.OnFixedUpdate(caller);
    }

    public void LateUpdate()
    {
        CurrentState?.OnLateUpdate(caller);
    }
}
