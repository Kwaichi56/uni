namespace ServiceLocatorLib;

public sealed class ServiceScope : IDisposable
{
    private readonly ServiceLocator _locator;
    private readonly Dictionary<Type, object> _scopedInstances = new();
    private bool _disposed;

    public ServiceScope(ServiceLocator locator)
    {
        _locator = locator;
    }

    public T Get<T>()
        where T : class
    {
        return _locator.Get<T>(this);
    }

    internal bool TryGetScoped(Type serviceType, out object instance)
    {
        return _scopedInstances.TryGetValue(serviceType, out instance!);
    }

    internal void SetScoped(Type serviceType, object instance)
    {
        _scopedInstances[serviceType] = instance;
    }

    public void Dispose()
    {
        if (_disposed)
            return;

        foreach (var item in _scopedInstances.Values)
        {
            if (item is IDisposable disposable)
                disposable.Dispose();
        }

        _scopedInstances.Clear();
        _disposed = true;
    }
}
