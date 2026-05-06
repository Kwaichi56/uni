namespace ServiceLocator;

public sealed class ServiceCollection
{
    private readonly List<ServiceDescriptor> _descriptors = new();

    public ServiceCollection AddTransient<TService, TImplementation>()
        where TImplementation : TService
    {
        _descriptors.Add(new ServiceDescriptor(
            typeof(TService),
            typeof(TImplementation),
            ServiceLifetime.Transient));

        return this;
    }

    public ServiceCollection AddScoped<TService, TImplementation>()
        where TImplementation : TService
    {
        _descriptors.Add(new ServiceDescriptor(
            typeof(TService),
            typeof(TImplementation),
            ServiceLifetime.Scoped));

        return this;
    }

    public ServiceCollection AddSingleton<TService, TImplementation>()
        where TImplementation : TService
    {
        _descriptors.Add(new ServiceDescriptor(
            typeof(TService),
            typeof(TImplementation),
            ServiceLifetime.Singleton));

        return this;
    }

    public ServiceProvider BuildServiceProvider()
    {
        return new ServiceProvider(_descriptors);
    }
}
