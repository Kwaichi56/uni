namespace ServiceLocatorLib;

public sealed class ServiceLocator
{
    private readonly Dictionary<Type, IServiceStorage> _services = new();

    public void AddService<TService>(IServiceStorage storage)
        where TService : class
    {
        _services[typeof(TService)] = storage;
    }

    public T Get<T>(ServiceScope scope)
        where T : class
    {
        var serviceType = typeof(T);

        if (!_services.TryGetValue(serviceType, out var storage))
            throw new InvalidOperationException($"Service '{serviceType.Name}' is not registered.");

        return (T)storage.Get(scope);
    }

    public ServiceScope CreateScope()
    {
        return new ServiceScope(this);
    }
}
