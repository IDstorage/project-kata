using Anomaly;

public abstract partial class State
{
    public abstract Identity ID { get; }

    public abstract void OnEnter(CustomBehaviour target);
    public abstract void OnExit(CustomBehaviour target);

    public abstract bool IsTransition(CustomBehaviour target, out Identity next);

    public abstract void OnFixedUpdate(CustomBehaviour target);
    public abstract void OnUpdate(CustomBehaviour target);
    public abstract void OnLateUpdate(CustomBehaviour target);

    public static State New<T>() where T : State, new()
    {
        return new T();
    }

    public static State Bind(params State[] states)
    {
        return new CompositeState(states);
    }
}