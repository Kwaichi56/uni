namespace SportNutritionShop.Data;

// Code First entities. UI models are deliberately separate from persisted entities.
public sealed class CategoryEntity
{
    public int Id { get; set; }
    public string Name { get; set; } = "";
    public List<ProductEntity> Products { get; set; } = [];
}

public sealed class UserEntity
{
    public int Id { get; set; }
    public string Login { get; set; } = "";
    public string Password { get; set; } = "";
    public string Role { get; set; } = "Client";
    public string FirstName { get; set; } = "";
    public string LastName { get; set; } = "";
    public string Email { get; set; } = "";
    public string Phone { get; set; } = "";
    public List<OrderEntity> Orders { get; set; } = [];
}

public sealed class ProductEntity
{
    public int Id { get; set; }
    public int CategoryId { get; set; }
    public CategoryEntity Category { get; set; } = null!;
    public string ShortName { get; set; } = "";
    public string FullName { get; set; } = "";
    public string Description { get; set; } = "";
    public string Brand { get; set; } = "";
    public string Country { get; set; } = "";
    public string Flavor { get; set; } = "";
    public string Weight { get; set; } = "";
    public double Price { get; set; }
    public double DiscountPercent { get; set; }
    public int Quantity { get; set; }
    public double Rating { get; set; }
    public int SoldCount { get; set; }
    public string? ImageFileName { get; set; }
    public byte[]? ImageData { get; set; }
    public List<OrderItemEntity> OrderItems { get; set; } = [];
}

public sealed class OrderEntity
{
    public int Id { get; set; }
    public int UserId { get; set; }
    public UserEntity User { get; set; } = null!;
    public string CreatedAt { get; set; } = "";
    public double Total { get; set; }
    public string Status { get; set; } = "Создан";
    public List<OrderItemEntity> Items { get; set; } = [];
}

public sealed class OrderItemEntity
{
    public int Id { get; set; }
    public int OrderId { get; set; }
    public OrderEntity Order { get; set; } = null!;
    public int? ProductId { get; set; }
    public ProductEntity? Product { get; set; }
    public string ProductName { get; set; } = "";
    public int Quantity { get; set; }
    public double UnitPrice { get; set; }
}

public sealed class AuditEntryEntity
{
    public int Id { get; set; }
    public string TableName { get; set; } = "";
    public string Action { get; set; } = "";
    public int? RecordId { get; set; }
    public string ChangedAt { get; set; } = "";
}

public sealed class StoredCommandEntity
{
    public int Id { get; set; }
    public string Name { get; set; } = "";
    public string SqlText { get; set; } = "";
    public string Description { get; set; } = "";
}
