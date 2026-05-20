namespace IRC;

public interface IRCGenerator<T>
{
    Func<T, bool>? Next { get; set; }
    void Start(T context);
}

public interface IRCHandler<T>
{
    void LastUse(T context);
}

public interface IResponsibilityChain<T>
{
    IResponsibilityChain<T> Use(Func<T, Func<T, bool>, bool> step);
}
