[System.AttributeUsage(System.AttributeTargets.Class, AllowMultiple = false)]
public sealed class SharedComponentDataAttribute : System.Attribute
{
    public System.Type OuterType { get; private set; }
    public SharedComponentDataAttribute(System.Type outerType)
    {
        OuterType = outerType;
    }
}
