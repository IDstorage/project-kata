using System.Collections.Generic;
using UnityEngine;
using Anomaly;

[System.Serializable]
public class StateMachineComponent<T> : CustomComponent, IFixedUpdater, IUpdater, ILateUpdater where T : CustomBehaviour
{
    [SerializeField]
    private T caller;

    private List<State<T>> states = new List<State<T>>();

    public State<T> CurrentState { get; private set; }


    public void Run(int entryIndex = 0, params State<T>[] states)
    {
        this.states.AddRange(states);

        Debug.Assert(entryIndex >= 0 && entryIndex < this.states.Count);
        ChangeState(entryIndex);
    }

    public void ChangeState(StateID id)
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
