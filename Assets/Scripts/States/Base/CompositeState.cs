using System.Collections.Generic;
using Anomaly;

public class CompositeState<T> : State<T> where T : CustomBehaviour
{
    public override StateID ID => StateID.None;

    private List<State<T>> states = new List<State<T>>();

    public CompositeState(params State<T>[] states)
    {
        this.states.AddRange(states);
    }


    public override void OnEnter(T target)
    {
        for (int i = 0; i < states.Count; ++i)
        {
            states[i].OnEnter(target);
        }
    }

    public override void OnExit(T target)
    {
        for (int i = 0; i < states.Count; ++i)
        {
            states[i].OnExit(target);
        }
    }


    public override bool IsTransition(T target, out StateID next)
    {
        next = StateID.None;
        for (int i = 0; i < states.Count; ++i)
        {
            if (states[i].IsTransition(target, out next)) return true;
        }
        return false;
    }


    public override void OnFixedUpdate(T target)
    {
        for (int i = 0; i < states.Count; ++i)
        {
            states[i].OnFixedUpdate(target);
        }
    }

    public override void OnLateUpdate(T target)
    {
        for (int i = 0; i < states.Count; ++i)
        {
            states[i].OnLateUpdate(target);
        }
    }

    public override void OnUpdate(T target)
    {
        for (int i = 0; i < states.Count; ++i)
        {
            states[i].OnUpdate(target);
        }
    }
}