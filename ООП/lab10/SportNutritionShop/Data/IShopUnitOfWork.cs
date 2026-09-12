using System.Linq.Expressions;

namespace SportNutritionShop.Data;

// Contracts keep business operations independent of EF Core and SQLite.
public interface IRepository<T> where T : class
{
    Task<List<T>> GetAllAsync();
    Task<T?> GetByIdAsync(int id);
    Task<T?> FirstOrDefaultAsync(Expression<Func<T, bool>> predicate);
    T? FirstOrDefault(Expression<Func<T, bool>> predicate);
    Task<bool> AnyAsync(Expression<Func<T, bool>> predicate);
    void Add(T entity);
    void Remove(T entity);
}

public interface IProductRepository : IRepository<ProductEntity>
{
    Task<List<ProductEntity>> GetCatalogAsync();
    Task<List<ProductEntity>> SearchAsync(string? term, string field, bool descending);
    Task<List<ProductEntity>> GetLowStockAsync(int limit);
}

public interface ICategoryRepository : IRepository<CategoryEntity>
{
    Task<CategoryEntity?> GetByNameAsync(string name);
    Task<List<CategoryStatistics>> GetStatisticsAsync();
}

public sealed record CategoryStatistics(string Category, int ProductCount, double? AveragePrice, int Stock);

public interface IShopTransaction : IAsyncDisposable
{
    Task CommitAsync();
}

public interface IShopUnitOfWork : IDisposable, IAsyncDisposable
{
    IProductRepository Products { get; }
    ICategoryRepository Categories { get; }
    IRepository<UserEntity> Users { get; }
    IRepository<OrderEntity> Orders { get; }
    IRepository<OrderItemEntity> OrderItems { get; }
    IRepository<AuditEntryEntity> AuditLog { get; }
    IRepository<StoredCommandEntity> StoredCommands { get; }
    Task InitializeSchemaAsync();
    Task<IShopTransaction> BeginTransactionAsync();
    Task<int> SaveChangesAsync();
    int SaveChanges();
}
