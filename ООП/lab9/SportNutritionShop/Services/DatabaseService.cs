using System.Data;
using System.Globalization;
using System.IO;
using System.Reflection;
using System.Xml.Linq;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using SportNutritionShop.Data;
using SportNutritionShop.Models;

namespace SportNutritionShop.Services;

/// <summary>EF Core Code First persistence for the shop and its administrative grid.</summary>
public static class DatabaseService
{
    public static string DatabasePath { get; private set; } = "";
    public static string ConnectionString { get; private set; } = "";
    public static int LowStockLimit { get; private set; } = 5;

    private static ShopDbContext Open() => new(ConnectionString);

    public static async Task InitializeAsync()
    {
        LowStockLimit = ReadIntSetting("LowStockLimit", 5);
        ConnectionString = BuildConnectionString();
        await using var db = Open();
        await db.Database.EnsureCreatedAsync();
        await db.Database.ExecuteSqlRawAsync("""
            CREATE TRIGGER IF NOT EXISTS trg_Products_Insert AFTER INSERT ON Products
            BEGIN INSERT INTO AuditLog(TableName, Action, RecordId, ChangedAt)
            VALUES ('Products', 'INSERT', NEW.Id, datetime('now')); END;
            """);
        await db.Database.ExecuteSqlRawAsync("""
            CREATE TRIGGER IF NOT EXISTS trg_Products_Update AFTER UPDATE ON Products
            BEGIN INSERT INTO AuditLog(TableName, Action, RecordId, ChangedAt)
            VALUES ('Products', 'UPDATE', NEW.Id, datetime('now')); END;
            """);
        await db.Database.ExecuteSqlRawAsync("""
            CREATE TRIGGER IF NOT EXISTS trg_Products_Delete AFTER DELETE ON Products
            BEGIN INSERT INTO AuditLog(TableName, Action, RecordId, ChangedAt)
            VALUES ('Products', 'DELETE', OLD.Id, datetime('now')); END;
            """);
        await SeedAsync(db);
    }

    public static async Task<List<Product>> GetProductsAsync()
    {
        await using var db = Open();
        var entities = await db.Products.AsNoTracking().Include(p => p.Category)
            .OrderBy(p => p.ShortName).ToListAsync();
        var products = new List<Product>(entities.Count);
        foreach (var entity in entities)
            products.Add(await ToProductAsync(entity));
        return products;
    }

    // LINQ-to-Entities: selected field or several fields, with server-side sorting.
    public static async Task<DataTable> SearchProductsAsync(
        string? term, string field, bool descending)
    {
        await using var db = Open();
        IQueryable<ProductEntity> query = db.Products.AsNoTracking().Include(p => p.Category);
        if (!string.IsNullOrWhiteSpace(term))
        {
            var pattern = "%" + term.Trim() + "%";
            query = field switch
            {
                "ShortName" => query.Where(p => EF.Functions.Like(p.ShortName, pattern)),
                "Category" => query.Where(p => EF.Functions.Like(p.Category.Name, pattern)),
                "Brand" => query.Where(p => EF.Functions.Like(p.Brand, pattern)),
                "All" => query.Where(p => EF.Functions.Like(p.ShortName, pattern)
                    || EF.Functions.Like(p.FullName, pattern)
                    || EF.Functions.Like(p.Brand, pattern)
                    || EF.Functions.Like(p.Category.Name, pattern)),
                _ => throw new ArgumentException("Недопустимое поле поиска.", nameof(field))
            };
        }
        query = descending ? query.OrderByDescending(p => p.Price) : query.OrderBy(p => p.Price);
        return ToDataTable(await query.ToListAsync(), "Products");
    }

    public static async Task SaveProductsAsync(IEnumerable<Product> products)
    {
        await using var db = Open();
        await using var transaction = await db.Database.BeginTransactionAsync();
        foreach (var product in products)
        {
            var entity = product.Id > 0 ? await db.Products.FindAsync(product.Id) : null;
            if (entity is null)
            {
                entity = new ProductEntity { Id = product.Id };
                db.Products.Add(entity);
            }
            await CopyProductAsync(db, product, entity);
        }
        await db.SaveChangesAsync();
        await transaction.CommitAsync();
    }

    public static async Task DeleteProductAsync(int id)
    {
        await using var db = Open();
        var product = await db.Products.FindAsync(id);
        if (product is null) return;
        db.Products.Remove(product);
        await db.SaveChangesAsync();
    }

    public static async Task CreateOrderAsync(string login, IEnumerable<CartItem> cartItems)
    {
        var items = cartItems.ToList();
        if (items.Count == 0) return;
        await using var db = Open();
        await using var transaction = await db.Database.BeginTransactionAsync();
        var user = await db.Users.SingleOrDefaultAsync(u => u.Login == login)
            ?? throw new InvalidOperationException("Пользователь не найден.");
        var order = new OrderEntity
        {
            UserId = user.Id,
            CreatedAt = DateTimeOffset.Now.ToString("O"),
            Status = "Создан"
        };
        db.Orders.Add(order);
        foreach (var item in items)
        {
            if (item.Quantity <= 0) throw new InvalidOperationException("Количество должно быть положительным.");
            var product = await db.Products.FindAsync(item.Product.Id)
                ?? throw new InvalidOperationException("Товар не найден.");
            if (product.Quantity < item.Quantity)
                throw new InvalidOperationException($"Недостаточно товара «{product.ShortName}» на складе.");
            product.Quantity -= item.Quantity;
            product.SoldCount += item.Quantity;
            var unitPrice = product.Price * (1 - product.DiscountPercent / 100);
            order.Items.Add(new OrderItemEntity
            {
                ProductId = product.Id, ProductName = product.ShortName,
                Quantity = item.Quantity, UnitPrice = unitPrice
            });
            order.Total += unitPrice * item.Quantity;
        }
        await db.SaveChangesAsync();
        await transaction.CommitAsync();
    }

    public static bool TryLogin(string login, string password, out UserRole role)
    {
        using var db = Open();
        var userRole = db.Users.AsNoTracking()
            .Where(u => u.Login == login && u.Password == password)
            .Select(u => u.Role).SingleOrDefault();
        role = userRole == "Admin" ? UserRole.Admin : UserRole.Client;
        return userRole is not null;
    }

    public static UserProfile LoadProfile(string login)
    {
        using var db = Open();
        var user = db.Users.AsNoTracking().SingleOrDefault(u => u.Login == login)
            ?? throw new InvalidOperationException("Пользователь не найден в базе данных.");
        return new UserProfile
        {
            Login = user.Login, FirstName = user.FirstName, LastName = user.LastName,
            Email = user.Email, Phone = user.Phone
        };
    }

    public static void SaveProfile(UserProfile profile)
    {
        using var db = Open();
        var user = db.Users.SingleOrDefault(u => u.Login == profile.Login)
            ?? throw new InvalidOperationException("Пользователь не найден.");
        user.FirstName = profile.FirstName;
        user.LastName = profile.LastName;
        user.Email = profile.Email;
        user.Phone = profile.Phone;
        db.SaveChanges();
    }

    public static async Task<DataTable> LoadTableAsync(string tableName)
    {
        await using var db = Open();
        return tableName switch
        {
            "Categories" => ToDataTable(await db.Categories.AsNoTracking().OrderByDescending(x => x.Id).ToListAsync(), tableName),
            "Users" => ToDataTable(await db.Users.AsNoTracking().OrderByDescending(x => x.Id).ToListAsync(), tableName),
            "Products" => ToDataTable(await db.Products.AsNoTracking().OrderByDescending(x => x.Id).ToListAsync(), tableName),
            "Orders" => ToDataTable(await db.Orders.AsNoTracking().OrderByDescending(x => x.Id).ToListAsync(), tableName),
            "OrderItems" => ToDataTable(await db.OrderItems.AsNoTracking().OrderByDescending(x => x.Id).ToListAsync(), tableName),
            "AuditLog" => ToDataTable(await db.AuditLog.AsNoTracking().OrderByDescending(x => x.Id).ToListAsync(), tableName),
            "StoredCommands" => ToDataTable(await db.StoredCommands.AsNoTracking().OrderByDescending(x => x.Id).ToListAsync(), tableName),
            _ => throw new ArgumentException("Недопустимое имя таблицы.", nameof(tableName))
        };
    }

    public static async Task SaveTableAsync(string tableName, DataTable table)
    {
        if (tableName is not ("Categories" or "Users" or "Products"))
            throw new InvalidOperationException("Эта таблица доступна только для просмотра.");
        await using var db = Open();
        await using var transaction = await db.Database.BeginTransactionAsync();
        switch (tableName)
        {
            case "Categories": await SaveRowsAsync(db, db.Categories, table); break;
            case "Users": await SaveRowsAsync(db, db.Users, table); break;
            case "Products": await SaveRowsAsync(db, db.Products, table); break;
        }
        await db.SaveChangesAsync();
        await transaction.CommitAsync();
        table.AcceptChanges();
    }

    public static async Task<DataTable> GetLowStockAsync(int limit)
    {
        await using var db = Open();
        var results = await db.Products.AsNoTracking()
            .Where(p => p.Quantity <= limit)
            .OrderBy(p => p.Quantity).ThenBy(p => p.ShortName)
            .Select(p => new { p.Id, p.ShortName, p.Brand, p.Quantity, p.Price })
            .ToListAsync();
        var table = new DataTable("LowStock");
        table.Columns.Add("Id", typeof(int));
        table.Columns.Add("ShortName", typeof(string));
        table.Columns.Add("Brand", typeof(string));
        table.Columns.Add("Quantity", typeof(int));
        table.Columns.Add("Price", typeof(double));
        foreach (var p in results) table.Rows.Add(p.Id, p.ShortName, p.Brand, p.Quantity, p.Price);
        table.AcceptChanges();
        return table;
    }

    // SQLite has no stored procedures; this named operation retains the lab 8 demonstration.
    public static async Task<DataTable> ExecuteStoredCommandAsync(string name, params SqliteParameter[] parameters)
    {
        if (name != "sp_ProductStatistics")
            throw new InvalidOperationException($"Именованная команда «{name}» не поддерживается.");
        await using var db = Open();
        var results = await db.Categories.AsNoTracking()
            .Select(c => new
            {
                Category = c.Name,
                ProductCount = c.Products.Count,
                AveragePrice = c.Products.Select(p => (double?)p.Price).Average(),
                Stock = c.Products.Sum(p => p.Quantity)
            }).OrderBy(x => x.Category).ToListAsync();
        var table = new DataTable(name);
        table.Columns.Add("Category", typeof(string));
        table.Columns.Add("ProductCount", typeof(int));
        table.Columns.Add("AveragePrice", typeof(double));
        table.Columns.Add("Stock", typeof(int));
        foreach (var item in results)
            table.Rows.Add(item.Category, item.ProductCount,
                item.AveragePrice is null ? DBNull.Value : Math.Round(item.AveragePrice.Value, 2),
                item.Stock);
        table.AcceptChanges();
        return table;
    }

    private static async Task SaveRowsAsync<T>(ShopDbContext db, DbSet<T> set, DataTable table) where T : class, new()
    {
        var properties = typeof(T).GetProperties(BindingFlags.Public | BindingFlags.Instance)
            .Where(p => p.CanWrite && IsScalar(p.PropertyType)).ToDictionary(p => p.Name);
        foreach (DataRow row in table.Rows)
        {
            if (row.RowState == DataRowState.Unchanged) continue;
            T? entity = null;
            if (row.RowState != DataRowState.Added)
            {
                var id = Convert.ToInt32(row["Id", row.RowState == DataRowState.Deleted
                    ? DataRowVersion.Original : DataRowVersion.Current], CultureInfo.InvariantCulture);
                entity = await set.FindAsync(id) ?? throw new InvalidOperationException($"Запись {id} удалена другим пользователем.");
            }
            if (row.RowState == DataRowState.Deleted)
            {
                set.Remove(entity!);
                continue;
            }
            if (entity is null)
            {
                entity = new T();
                set.Add(entity);
            }
            foreach (var property in properties.Values.Where(p => p.Name != "Id" && table.Columns.Contains(p.Name)))
            {
                var raw = row[property.Name];
                var target = Nullable.GetUnderlyingType(property.PropertyType) ?? property.PropertyType;
                var value = raw == DBNull.Value ? null
                    : target == typeof(byte[]) ? raw
                    : Convert.ChangeType(raw, target, CultureInfo.InvariantCulture);
                property.SetValue(entity, value);
            }
        }
    }

    private static DataTable ToDataTable<T>(IEnumerable<T> entities, string name)
    {
        var table = new DataTable(name) { Locale = CultureInfo.InvariantCulture };
        var properties = typeof(T).GetProperties(BindingFlags.Public | BindingFlags.Instance)
            .Where(p => p.CanRead && IsScalar(p.PropertyType)).ToArray();
        foreach (var property in properties)
            table.Columns.Add(property.Name, Nullable.GetUnderlyingType(property.PropertyType) ?? property.PropertyType);
        foreach (var entity in entities)
            table.Rows.Add(properties.Select(p => p.GetValue(entity) ?? DBNull.Value).ToArray());
        table.AcceptChanges();
        return table;
    }

    private static bool IsScalar(Type type)
    {
        type = Nullable.GetUnderlyingType(type) ?? type;
        return type.IsPrimitive || type == typeof(string) || type == typeof(decimal) || type == typeof(byte[]);
    }

    private static async Task CopyProductAsync(ShopDbContext db, Product source, ProductEntity target)
    {
        var categoryName = string.IsNullOrWhiteSpace(source.Category) ? "Без категории" : source.Category.Trim();
        var category = db.Categories.Local.FirstOrDefault(c => c.Name == categoryName)
            ?? await db.Categories.SingleOrDefaultAsync(c => c.Name == categoryName);
        if (category is null)
        {
            category = new CategoryEntity { Name = categoryName };
            db.Categories.Add(category);
        }
        target.Category = category;
        target.ShortName = source.ShortName.Trim();
        target.FullName = source.FullName.Trim();
        target.Description = source.Description.Trim();
        target.Brand = source.Brand.Trim();
        target.Country = source.Country.Trim();
        target.Flavor = source.Flavor.Trim();
        target.Weight = source.Weight.Trim();
        target.Price = (double)source.Price;
        target.DiscountPercent = (double)source.DiscountPercent;
        target.Quantity = source.Quantity;
        target.Rating = source.Rating;
        target.SoldCount = source.SoldCount;
        var image = await ReadImageAsync(source.ImagePath);
        if (image.Data is not null)
        {
            target.ImageFileName = image.FileName;
            target.ImageData = image.Data;
        }
    }

    private static async Task<Product> ToProductAsync(ProductEntity entity)
    {
        var image = entity.ImageData is null ? "" :
            await MaterializeImageAsync(entity.Id, entity.ImageFileName ?? "product.jpg", entity.ImageData);
        return new Product
        {
            Id = entity.Id, ShortName = entity.ShortName, FullName = entity.FullName,
            Description = entity.Description, Category = entity.Category.Name,
            Brand = entity.Brand, Country = entity.Country, Flavor = entity.Flavor,
            Weight = entity.Weight, Price = (decimal)entity.Price,
            DiscountPercent = (decimal)entity.DiscountPercent, Quantity = entity.Quantity,
            Rating = entity.Rating, SoldCount = entity.SoldCount, ImagePath = image
        };
    }

    private static async Task SeedAsync(ShopDbContext db)
    {
        if (await db.Users.AnyAsync()) return;
        await using var transaction = await db.Database.BeginTransactionAsync();
        db.Users.AddRange(
            new UserEntity { Login = "admin", Password = "123", Role = "Admin", FirstName = "admin" },
            new UserEntity { Login = "client", Password = "123", Role = "Client", FirstName = "client" });
        db.StoredCommands.Add(new StoredCommandEntity
        {
            Name = "sp_ProductStatistics",
            SqlText = "LINQ: Categories.Select(c => new { c.Name, Count = c.Products.Count, Average = c.Products.Average(p => p.Price) })",
            Description = "Статистика каталога через LINQ to Entities"
        });
        var products = new[]
        {
            new Product { ShortName = "Creatine Monohydrate", FullName = "OstroVit Creatine Monohydrate 300 g", Description = "Креатин для роста силы.", Category = "Креатин", Brand = "OstroVit", Country = "Польша", Flavor = "Без вкуса", Weight = "300 г", Price = 48, Quantity = 14, Rating = 4.8, SoldCount = 121, ImagePath = Asset("creatine.jpg") },
            new Product { ShortName = "Whey Protein", FullName = "BioTech USA 100% Pure Whey 1000 g", Description = "Сывороточный белок.", Category = "Протеин", Brand = "BioTech USA", Country = "США", Flavor = "Chocolate", Weight = "1000 г", Price = 99, DiscountPercent = 10, Quantity = 8, Rating = 4.9, SoldCount = 250, ImagePath = Asset("whey.jpg") },
            new Product { ShortName = "BCAA 2:1:1", FullName = "Mutant BCAA 2:1:1 400 g", Description = "Аминокислоты для поддержки мышц.", Category = "Аминокислоты", Brand = "Mutant", Country = "Канада", Flavor = "Orange", Weight = "400 г", Price = 65, DiscountPercent = 5, Quantity = 12, Rating = 4.6, SoldCount = 89, ImagePath = Asset("bcaa.jpg") },
            new Product { ShortName = "Mass Gainer", FullName = "Optimum Nutrition Serious Mass 2700 g", Description = "Гейнер для набора массы.", Category = "Гейнер", Brand = "Optimum Nutrition", Country = "США", Flavor = "Vanilla", Weight = "2700 г", Price = 170, Quantity = 6, Rating = 4.7, SoldCount = 73, ImagePath = Asset("gainer.jpg") },
            new Product { ShortName = "Multivitamin", FullName = "NOW Foods ADAM Superior Men's Multi", Description = "Комплекс витаминов.", Category = "Витамины", Brand = "NOW Foods", Country = "США", Flavor = "Neutral", Weight = "90 капсул", Price = 54, Quantity = 20, Rating = 4.5, SoldCount = 61, ImagePath = Asset("vitamins.jpg") },
            new Product { ShortName = "Pre-Workout", FullName = "Bombbar Pre Workout 300 g", Description = "Предтренировочный комплекс.", Category = "Предтрен", Brand = "Bombbar", Country = "Россия", Flavor = "Berry", Weight = "300 г", Price = 59, Quantity = 10, Rating = 4.4, SoldCount = 97, ImagePath = Asset("preworkout.jpg") }
        };
        foreach (var source in products)
        {
            var entity = new ProductEntity();
            db.Products.Add(entity);
            await CopyProductAsync(db, source, entity);
        }
        await db.SaveChangesAsync();
        await transaction.CommitAsync();
    }

    private static string Asset(string name) => Path.Combine(AppContext.BaseDirectory, "Assets", "Products", name);

    private static async Task<(string? FileName, byte[]? Data)> ReadImageAsync(string path)
    {
        if (string.IsNullOrWhiteSpace(path)) return (null, null);
        var absolute = Path.IsPathRooted(path) ? path : Path.Combine(AppContext.BaseDirectory, path.Replace('/', Path.DirectorySeparatorChar));
        return File.Exists(absolute) ? (Path.GetFileName(absolute), await File.ReadAllBytesAsync(absolute)) : (null, null);
    }

    private static async Task<string> MaterializeImageAsync(int id, string fileName, byte[] data)
    {
        var extension = Path.GetExtension(fileName);
        if (string.IsNullOrWhiteSpace(extension)) extension = ".jpg";
        var directory = Path.Combine(AppContext.BaseDirectory, "Data", "ImageCache");
        Directory.CreateDirectory(directory);
        var path = Path.Combine(directory, $"product_{id}{extension}");
        if (!File.Exists(path) || new FileInfo(path).Length != data.LongLength)
            await File.WriteAllBytesAsync(path, data);
        return path;
    }

    private static string BuildConnectionString()
    {
        var builder = new SqliteConnectionStringBuilder(ReadConfigValue("connectionStrings", "SportNutritionShopDb")
            ?? "Data Source=Data\\SportNutritionShop.db;Foreign Keys=True");
        var source = builder.DataSource;
        if (!Path.IsPathRooted(source)) source = Path.GetFullPath(Path.Combine(AppContext.BaseDirectory, source));
        DatabasePath = source;
        Directory.CreateDirectory(Path.GetDirectoryName(source)!);
        builder.DataSource = source;
        builder.ForeignKeys = true;
        builder.DefaultTimeout = ReadIntSetting("CommandTimeoutSeconds", 30);
        return builder.ToString();
    }

    private static int ReadIntSetting(string key, int fallback) =>
        int.TryParse(ReadConfigValue("appSettings", key), CultureInfo.InvariantCulture, out var value) && value > 0
            ? value : fallback;

    private static string? ReadConfigValue(string section, string key)
    {
        foreach (var path in new[]
        {
            Path.Combine(AppContext.BaseDirectory, "App.config"),
            Path.Combine(AppContext.BaseDirectory, "SportNutritionShop.dll.config"),
            Path.Combine(AppContext.BaseDirectory, "SportNutritionShop.exe.config")
        }.Where(File.Exists))
        {
            var entry = XDocument.Load(path).Root?.Element(section)?.Elements("add").FirstOrDefault(e =>
                (string?)e.Attribute(section == "appSettings" ? "key" : "name") == key);
            var value = (string?)entry?.Attribute(section == "appSettings" ? "value" : "connectionString");
            if (!string.IsNullOrWhiteSpace(value)) return value;
        }
        return null;
    }
}
