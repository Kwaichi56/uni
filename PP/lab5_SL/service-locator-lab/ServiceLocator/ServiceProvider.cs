namespace ServiceLocator;

public sealed class ServiceProvider
{
    private readonly Dictionary<Type, ServiceDescriptor> _descriptors;
    private readonly Dictionary<Type, object> _singletonInstances = new();

    public ServiceProvider(IEnumerable<ServiceDescriptor> descriptors)
    {
        _descriptors = descriptors.ToDictionary(d => d.ServiceType);
    }

    public T GetService<T>() where T : class
    {
        return (T)GetService(typeof(T), null);
    }

    public object GetService(Type serviceType)
    {
        return GetService(serviceType, null);
    }

    internal object GetService(Type serviceType, Dictionary<Type, object>? scopedInstances)
    {
        if (_descriptors.TryGetValue(serviceType, out var descriptor))
        {
            return descriptor.Lifetime switch
            {
                ServiceLifetime.Transient => CreateInstance(descriptor.ImplementationType, scopedInstances),
                ServiceLifetime.Singleton => GetSingleton(serviceType, descriptor, scopedInstances),
                ServiceLifetime.Scoped => GetScoped(serviceType, descriptor, scopedInstances),
                _ => throw new InvalidOperationException("Неизвестный жизненный цикл.")
            };
        }

        if (!serviceType.IsAbstract && !serviceType.IsInterface)
            return CreateInstance(serviceType, scopedInstances);

        throw new InvalidOperationException($"Сервис типа {serviceType.Name} не зарегистрирован.");
    }

    private object GetSingleton(Type serviceType, ServiceDescriptor descriptor, Dictionary<Type, object>? scopedInstances)
    {
        if (_singletonInstances.TryGetValue(serviceType, out var instance))
            return instance;

        instance = CreateInstance(descriptor.ImplementationType, scopedInstances);
        _singletonInstances[serviceType] = instance;
        return instance;
    }

    private object GetScoped(Type serviceType, ServiceDescriptor descriptor, Dictionary<Type, object>? scopedInstances)
    {
        if (scopedInstances is null)
            throw new InvalidOperationException($"Сервис {serviceType.Name} со временем жизни Scoped можно получать только внутри scope.");

        if (scopedInstances.TryGetValue(serviceType, out var instance))
            return instance;

        instance = CreateInstance(descriptor.ImplementationType, scopedInstances);
        scopedInstances[serviceType] = instance;
        return instance;
    }

    private object CreateInstance(Type implementationType, Dictionary<Type, object>? scopedInstances)
    {
        var constructors = implementationType.GetConstructors();

        if (constructors.Length == 0)
            throw new InvalidOperationException($"У типа {implementationType.Name} нет открытого конструктора.");

        var constructor = constructors
            .OrderByDescending(c => c.GetParameters().Length)
            .First();

        var parameters = constructor.GetParameters();
        var arguments = new object[parameters.Length];

        for (int i = 0; i < parameters.Length; i++)
            arguments[i] = GetService(parameters[i].ParameterType, scopedInstances);

        return Activator.CreateInstance(implementationType, arguments)
               ?? throw new InvalidOperationException($"Не удалось создать экземпляр {implementationType.Name}.");
    }

    public ServiceScope CreateScope()
    {
        return new ServiceScope(this);
    }
}
