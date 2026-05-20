namespace Test_ServiceLocator;

public interface ITransientService
{
    Guid Id { get; }
}

public interface ISingletonService
{
    Guid Id { get; }
}

public interface IScopedService
{
    Guid Id { get; }
}

public sealed class TransientService : ITransientService
{
    public Guid Id { get; } = Guid.NewGuid();
}

public sealed class SingletonService : ISingletonService
{
    public Guid Id { get; } = Guid.NewGuid();
}

public sealed class ScopedService : IScopedService
{
    public Guid Id { get; } = Guid.NewGuid();
}

public sealed class Reporter
{
    private readonly ITransientService _transient;
    private readonly ISingletonService _singleton;
    private readonly IScopedService _scoped;

    public Reporter(ITransientService transient, ISingletonService singleton, IScopedService scoped)
    {
        _transient = transient;
        _singleton = singleton;
        _scoped = scoped;
    }

    public void Print(string title)
    {
        Console.WriteLine(title);
        Console.WriteLine($"Transient : {_transient.Id}");
        Console.WriteLine($"Singleton : {_singleton.Id}");
        Console.WriteLine($"Scoped    : {_scoped.Id}");
        Console.WriteLine(new string('-', 60));
    }
}
