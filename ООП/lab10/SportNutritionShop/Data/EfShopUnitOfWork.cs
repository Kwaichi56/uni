using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;

namespace SportNutritionShop.Data;

public sealed class EfShopUnitOfWork : IShopUnitOfWork
{
    private readonly ShopDbContext _context;

    public EfShopUnitOfWork(string connectionString)
    {
        _context = new ShopDbContext(connectionString);
        Products = new EfProductRepository(_context);
        Categories = new EfCategoryRepository(_context);
        Users = new EfRepository<UserEntity>(_context);
        Orders = new EfRepository<OrderEntity>(_context);
        OrderItems = new EfRepository<OrderItemEntity>(_context);
        AuditLog = new EfRepository<AuditEntryEntity>(_context);
        StoredCommands = new EfRepository<StoredCommandEntity>(_context);
    }

    public IProductRepository Products { get; }
    public ICategoryRepository Categories { get; }
    public IRepository<UserEntity> Users { get; }
    public IRepository<OrderEntity> Orders { get; }
    public IRepository<OrderItemEntity> OrderItems { get; }
    public IRepository<AuditEntryEntity> AuditLog { get; }
    public IRepository<StoredCommandEntity> StoredCommands { get; }

    public async Task InitializeSchemaAsync()
    {
        await _context.Database.EnsureCreatedAsync();
        await _context.Database.ExecuteSqlRawAsync("""
            CREATE TRIGGER IF NOT EXISTS trg_Products_Insert AFTER INSERT ON Products
            BEGIN INSERT INTO AuditLog(TableName, Action, RecordId, ChangedAt)
            VALUES ('Products', 'INSERT', NEW.Id, datetime('now')); END;
            """);
        await _context.Database.ExecuteSqlRawAsync("""
            CREATE TRIGGER IF NOT EXISTS trg_Products_Update AFTER UPDATE ON Products
            BEGIN INSERT INTO AuditLog(TableName, Action, RecordId, ChangedAt)
            VALUES ('Products', 'UPDATE', NEW.Id, datetime('now')); END;
            """);
        await _context.Database.ExecuteSqlRawAsync("""
            CREATE TRIGGER IF NOT EXISTS trg_Products_Delete AFTER DELETE ON Products
            BEGIN INSERT INTO AuditLog(TableName, Action, RecordId, ChangedAt)
            VALUES ('Products', 'DELETE', OLD.Id, datetime('now')); END;
            """);
    }

    public async Task<IShopTransaction> BeginTransactionAsync() =>
        new EfShopTransaction(await _context.Database.BeginTransactionAsync());

    public Task<int> SaveChangesAsync() => _context.SaveChangesAsync();
    public int SaveChanges() => _context.SaveChanges();
    public void Dispose() => _context.Dispose();
    public ValueTask DisposeAsync() => _context.DisposeAsync();

    private sealed class EfShopTransaction(IDbContextTransaction transaction) : IShopTransaction
    {
        public Task CommitAsync() => transaction.CommitAsync();
        public ValueTask DisposeAsync() => transaction.DisposeAsync();
    }
}
