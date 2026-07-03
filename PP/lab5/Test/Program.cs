using ServiceLocatorLib;

public interface ITransientService
{
    Guid Id { get; }
}

public interface IScopedService
{
    Guid Id { get; }
}

public interface ISingletonService
{
    Guid Id { get; }
}

public class TransientService : ITransientService
{
    public Guid Id { get; } = Guid.NewGuid();
}

public class ScopedService : IScopedService
{
    public Guid Id { get; } = Guid.NewGuid();
}

public class SingletonService : ISingletonService
{
    public Guid Id { get; } = Guid.NewGuid();
}

internal class Program
{
    private static void Main(string[] args)
    {
        var locator = new ServiceLocator();

        locator.AddTransient<ITransientService>(_ => new TransientService());
        locator.AddScoped<IScopedService>(_ => new ScopedService());
        locator.AddSingleton<ISingletonService>(_ => new SingletonService());

        Console.WriteLine("========== TRANSIENT ==========");
        using (var scope = locator.CreateScope())
        {
            var t1 = scope.Get<ITransientService>();
            var t2 = scope.Get<ITransientService>();

            Console.WriteLine($"Transient #1 Id: {t1.Id}");
            Console.WriteLine($"Transient #2 Id: {t2.Id}");
            Console.WriteLine($"Same object: {ReferenceEquals(t1, t2)}");
        }

        Console.WriteLine();
        Console.WriteLine("========== SCOPED ==========");
        IScopedService s1;
        IScopedService s2;
        IScopedService s3;

        using (var scope1 = locator.CreateScope())
        {
            s1 = scope1.Get<IScopedService>();
            s2 = scope1.Get<IScopedService>();

            Console.WriteLine($"Scope1 Scoped #1 Id: {s1.Id}");
            Console.WriteLine($"Scope1 Scoped #2 Id: {s2.Id}");
            Console.WriteLine($"Same object in scope1: {ReferenceEquals(s1, s2)}");
        }

        using (var scope2 = locator.CreateScope())
        {
            s3 = scope2.Get<IScopedService>();

            Console.WriteLine($"Scope2 Scoped #1 Id: {s3.Id}");
            Console.WriteLine($"Same object between scopes: {ReferenceEquals(s1, s3)}");
        }

        Console.WriteLine();
        Console.WriteLine("========== SINGLETON ==========");
        using (var scope1 = locator.CreateScope())
        using (var scope2 = locator.CreateScope())
        {
            var one = scope1.Get<ISingletonService>();
            var two = scope2.Get<ISingletonService>();

            Console.WriteLine($"Singleton from scope1 Id: {one.Id}");
            Console.WriteLine($"Singleton from scope2 Id: {two.Id}");
            Console.WriteLine($"Same object: {ReferenceEquals(one, two)}");
        }
    }
}
