namespace ServiceLocatorLib;

public sealed class SingletonStorage<TService> : IServiceStorage
    where TService : class
{
    private readonly Func<ServiceScope, TService> _factory;
    private TService? _instance;

    public SingletonStorage(Func<ServiceScope, TService> factory)
    {
        _factory = factory;
    }

    public object Get(ServiceScope scope)
    {
        _instance ??= _factory(scope);
        return _instance;
    }
}
