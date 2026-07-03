namespace ServiceLocatorLib;

public sealed class ScopedStorage<TService> : IServiceStorage
    where TService : class
{
    private readonly Func<ServiceScope, TService> _factory;
    private readonly Type _serviceType;

    public ScopedStorage(Func<ServiceScope, TService> factory)
    {
        _factory = factory;
        _serviceType = typeof(TService);
    }

    public object Get(ServiceScope scope)
    {
        if (scope.TryGetScoped(_serviceType, out var instance))
            return instance;

        var created = _factory(scope);
        scope.SetScoped(_serviceType, created);
        return created;
    }
}
