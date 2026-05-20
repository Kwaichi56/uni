namespace ServiceLocator;

public sealed class ServiceScope : IDisposable
{
    private readonly ServiceProvider _provider;
    private readonly Dictionary<Type, object> _scopedInstances = new();

    public ServiceScope(ServiceProvider provider)
    {
        _provider = provider;
    }

    public T GetService<T>() where T : class
    {
        return (T)_provider.GetService(typeof(T), _scopedInstances);
    }

    public object GetService(Type serviceType)
    {
        return _provider.GetService(serviceType, _scopedInstances);
    }

    public void Dispose()
    {
        foreach (var disposable in _scopedInstances.Values.OfType<IDisposable>())
            disposable.Dispose();

        _scopedInstances.Clear();
    }
}
