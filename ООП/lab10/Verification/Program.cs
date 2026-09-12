using Microsoft.Data.Sqlite;
using SportNutritionShop.Data;
using SportNutritionShop.Models;
using SportNutritionShop.Services;

var path = Path.Combine(Path.GetTempPath(), $"lab10-verification-{Guid.NewGuid():N}.db");
var connection = $"Data Source={path};Foreign Keys=True";
var appConfig = Path.Combine(AppContext.BaseDirectory, "App.config");
var originalConfig = File.Exists(appConfig) ? File.ReadAllText(appConfig) : null;
try
{
    await using (var work = new EfShopUnitOfWork(connection))
    {
        await work.InitializeSchemaAsync();
        await using var transaction = await work.BeginTransactionAsync();
        var category = new CategoryEntity { Name = "Проверка" };
        work.Categories.Add(category);
        work.Products.Add(new ProductEntity { Category = category, ShortName = "Товар", FullName = "Товар для проверки", Price = 20, Quantity = 3 });
        await work.SaveChangesAsync();
        await transaction.CommitAsync();
    }

    await using (var work = new EfShopUnitOfWork(connection))
    {
        Assert((await work.Products.GetCatalogAsync()).Count == 1, "Каталог не прочитан через репозиторий.");
        Assert((await work.Products.SearchAsync("Товар", "ShortName", false)).Count == 1, "Поиск не работает.");
        Assert((await work.Products.GetLowStockAsync(3)).Count == 1, "Запрос остатков не работает.");
        Assert((await work.Categories.GetStatisticsAsync())[0].ProductCount == 1, "Статистика не работает.");
    }

    await using (var work = new EfShopUnitOfWork(connection))
    {
        await using var transaction = await work.BeginTransactionAsync();
        work.Categories.Add(new CategoryEntity { Name = "Должна откатиться" });
        await work.SaveChangesAsync();
        // Leaving without CommitAsync rolls back the whole unit of work.
    }

    await using (var work = new EfShopUnitOfWork(connection))
        Assert((await work.Categories.GetAllAsync()).Count == 1, "Транзакция не откатилась.");

    File.WriteAllText(appConfig, $"<configuration><connectionStrings><add name=\"SportNutritionShopDb\" connectionString=\"{connection}\" /></connectionStrings></configuration>");
    DatabaseService.ConfigureUnitOfWork(value => new EfShopUnitOfWork(value));
    await DatabaseService.InitializeAsync();
    Assert(DatabaseService.TryLogin("admin", "123", out var role) && role == UserRole.Admin, "Авторизация не работает.");
    var products = await DatabaseService.GetProductsAsync();
    Assert(products.Count > 0, "Начальный каталог пуст.");
    var first = products[0];
    await DatabaseService.CreateOrderAsync("client", [new CartItem { Product = first, Quantity = 1 }]);
    var updated = (await DatabaseService.GetProductsAsync()).Single(p => p.Id == first.Id);
    Assert(updated.Quantity == first.Quantity - 1, "Заказ не уменьшил остаток.");
    Assert((await DatabaseService.LoadTableAsync("Orders")).Rows.Count == 1, "Заказ не сохранён.");
    Console.WriteLine("Repository, Unit of Work, rollback, login and order: OK");
}
finally
{
    SqliteConnection.ClearAllPools();
    File.Delete(path);
    if (originalConfig is null) File.Delete(appConfig);
    else File.WriteAllText(appConfig, originalConfig);
}

static void Assert(bool condition, string message)
{
    if (!condition) throw new Exception(message);
}
