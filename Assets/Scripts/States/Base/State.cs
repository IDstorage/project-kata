using Anomaly;

public abstract partial class State<T> where T : CustomBehaviour
{
    public abstract StateID ID { get; }

    public abstract void OnEnter(T target);
    public abstract void OnExit(T target);

    public abstract bool IsTransition(T target, out StateID next);

    public abstract void OnFixedUpdate(T target);
    public abstract void OnUpdate(T target);
    public abstract void OnLateUpdate(T target);


    public static State<T> Bind(params State<T>[] states)
    {
        return new CompositeState<T>(states);
    }
}