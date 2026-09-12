using Microsoft.EntityFrameworkCore;

namespace SportNutritionShop.Data;

public sealed class ShopDbContext(string connectionString) : DbContext
{
    public DbSet<CategoryEntity> Categories => Set<CategoryEntity>();
    public DbSet<UserEntity> Users => Set<UserEntity>();
    public DbSet<ProductEntity> Products => Set<ProductEntity>();
    public DbSet<OrderEntity> Orders => Set<OrderEntity>();
    public DbSet<OrderItemEntity> OrderItems => Set<OrderItemEntity>();
    public DbSet<AuditEntryEntity> AuditLog => Set<AuditEntryEntity>();
    public DbSet<StoredCommandEntity> StoredCommands => Set<StoredCommandEntity>();

    protected override void OnConfiguring(DbContextOptionsBuilder options) =>
        options.UseSqlite(connectionString);

    protected override void OnModelCreating(ModelBuilder model)
    {
        model.Entity<CategoryEntity>(e =>
        {
            e.ToTable("Categories");
            e.HasKey(x => x.Id);
            e.HasIndex(x => x.Name).IsUnique();
            e.Property(x => x.Name).IsRequired();
            e.HasMany(x => x.Products).WithOne(x => x.Category)
                .HasForeignKey(x => x.CategoryId).OnDelete(DeleteBehavior.Restrict);
        });
        model.Entity<UserEntity>(e =>
        {
            e.ToTable("Users");
            e.HasKey(x => x.Id);
            e.HasIndex(x => x.Login).IsUnique();
            e.Property(x => x.Login).IsRequired();
            e.Property(x => x.Password).IsRequired();
            e.HasMany(x => x.Orders).WithOne(x => x.User)
                .HasForeignKey(x => x.UserId).OnDelete(DeleteBehavior.Restrict);
        });
        model.Entity<ProductEntity>(e =>
        {
            e.ToTable("Products");
            e.HasKey(x => x.Id);
            e.HasMany(x => x.OrderItems).WithOne(x => x.Product)
                .HasForeignKey(x => x.ProductId).OnDelete(DeleteBehavior.SetNull);
        });
        model.Entity<OrderEntity>(e =>
        {
            e.ToTable("Orders");
            e.HasKey(x => x.Id);
            e.HasMany(x => x.Items).WithOne(x => x.Order)
                .HasForeignKey(x => x.OrderId).OnDelete(DeleteBehavior.Cascade);
        });
        model.Entity<OrderItemEntity>(e => { e.ToTable("OrderItems"); e.HasKey(x => x.Id); });
        model.Entity<AuditEntryEntity>(e => { e.ToTable("AuditLog"); e.HasKey(x => x.Id); });
        model.Entity<StoredCommandEntity>(e =>
        {
            e.ToTable("StoredCommands");
            e.HasKey(x => x.Id);
            e.HasIndex(x => x.Name).IsUnique();
        });
    }
}
