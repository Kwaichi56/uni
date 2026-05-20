using ServiceLocator;
using Test_ServiceLocator;

var services = new ServiceCollection();

services.AddTransient<ITransientService, TransientService>();
services.AddSingleton<ISingletonService, SingletonService>();
services.AddScoped<IScopedService, ScopedService>();
services.AddTransient<Reporter, Reporter>();

var provider = services.BuildServiceProvider();

Console.WriteLine("=== Проверка Transient ===");
var transient1 = provider.GetService<ITransientService>();
var transient2 = provider.GetService<ITransientService>();
Console.WriteLine($"Transient #1: {transient1.Id}");
Console.WriteLine($"Transient #2: {transient2.Id}");
Console.WriteLine($"Одинаковые ссылки: {ReferenceEquals(transient1, transient2)}");
Console.WriteLine();

Console.WriteLine("=== Проверка Singleton ===");
var singleton1 = provider.GetService<ISingletonService>();
var singleton2 = provider.GetService<ISingletonService>();
Console.WriteLine($"Singleton #1: {singleton1.Id}");
Console.WriteLine($"Singleton #2: {singleton2.Id}");
Console.WriteLine($"Одинаковые ссылки: {ReferenceEquals(singleton1, singleton2)}");
Console.WriteLine();

Console.WriteLine("=== Проверка Scoped вне scope ===");
try
{
    provider.GetService<IScopedService>();
}
catch (Exception ex)
{
    Console.WriteLine(ex.Message);
}
Console.WriteLine();

Console.WriteLine("=== Scope 1 ===");
using (var scope1 = provider.CreateScope())
{
    var scoped1 = scope1.GetService<IScopedService>();
    var scoped2 = scope1.GetService<IScopedService>();

    Console.WriteLine($"Scoped #1: {scoped1.Id}");
    Console.WriteLine($"Scoped #2: {scoped2.Id}");
    Console.WriteLine($"Одинаковые ссылки в одном scope: {ReferenceEquals(scoped1, scoped2)}");
    Console.WriteLine();

    var reporter1 = scope1.GetService<Reporter>();
    reporter1.Print("Reporter из Scope 1");
}

Console.WriteLine("=== Scope 2 ===");
using (var scope2 = provider.CreateScope())
{
    var scoped3 = scope2.GetService<IScopedService>();
    Console.WriteLine($"Scoped в Scope 2: {scoped3.Id}");
    Console.WriteLine();

    var reporter2 = scope2.GetService<Reporter>();
    reporter2.Print("Reporter из Scope 2");
}
