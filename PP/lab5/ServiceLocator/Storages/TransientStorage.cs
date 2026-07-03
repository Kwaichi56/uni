namespace ServiceLocatorLib;

public sealed class TransientStorage<TService> : IServiceStorage
    where TService : class
{
    private readonly Func<ServiceScope, TService> _factory;

    public TransientStorage(Func<ServiceScope, TService> factory)
    {
        _factory = factory;
    }

    public object Get(ServiceScope scope)
    {
        return _factory(scope);
    }
}
