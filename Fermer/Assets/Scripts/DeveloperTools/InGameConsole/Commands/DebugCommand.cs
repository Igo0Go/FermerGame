using UnityEngine.Events;

public class DebugCommand : BaseDebugCommand
{
    public UnityEvent Execute { get; }

    public DebugCommand(string id, string description, string format) : base(id, description, format)
    {
        Execute = new UnityEvent();
    }

    public void Invoke()
    {
        Execute.Invoke();
    }
}

public class DebugCommand<T1> : BaseDebugCommand
{
    public UnityEvent<T1> Execute { get; }

    public DebugCommand(string id, string description, string format) : base(id, description, format)
    {
        Execute = new UnityEvent<T1>();
    }

    public void Invoke(T1 value)
    {
        Execute.Invoke(value);
    }
}
