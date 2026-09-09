using System.Data;
using System.Globalization;
using System.IO;
using System.Xml.Linq;
using Microsoft.Data.Sqlite;
using SportNutritionShop.Models;

namespace SportNutritionShop.Services;

/// <summary>
/// ADO.NET access layer. All SQL values supplied by the user are parameters.
/// </summary>
public static class DatabaseService
{
    private static readonly HashSet<string> EditableTables = new(StringComparer.OrdinalIgnoreCase)
    {
        "Categories", "Users", "Products"
    };

    private static readonly HashSet<string> AllowedTables = new(StringComparer.OrdinalIgnoreCase)
    {
        "Categories", "Users", "Products", "Orders", "OrderItems", "AuditLog", "StoredCommands"
    };

    public static string DatabasePath { get; private set; } = string.Empty;
    public static string ConnectionString { get; private set; } = string.Empty;
    public static int LowStockLimit { get; private set; } = 5;

    public static async Task InitializeAsync()
    {
        LowStockLimit = ReadIntSetting("LowStockLimit", 5);
        ConnectionString = BuildConnectionString();
        await using var connection = new SqliteConnection(ConnectionString);
        await connection.OpenAsync();

        var schemaPath = Path.Combine(AppContext.BaseDirectory, "Data", "Schema.sql");
        if (!File.Exists(schemaPath))
            throw new FileNotFoundException("Не найден скрипт создания базы данных.", schemaPath);

        await using (var command = connection.CreateCommand())
        {
            command.CommandText = await File.ReadAllTextAsync(schemaPath);
            await command.ExecuteNonQueryAsync();
        }

        await SeedAsync(connection);
    }

    public static SqliteConnection CreateConnection() => new(ConnectionString);

    public static async Task<List<Product>> GetProductsAsync()
    {
        var products = new List<Product>();
        await using var connection = CreateConnection();
        await connection.OpenAsync();
        await using var command = connection.CreateCommand();
        command.CommandText = """
            SELECT p.Id, p.ShortName, p.FullName, p.Description, c.Name AS Category,
                   p.Brand, p.Country, p.Flavor, p.Weight, p.Price, p.DiscountPercent,
                   p.Quantity, p.Rating, p.SoldCount, p.ImageFileName, p.ImageData
            FROM Products p
            JOIN Categories c ON c.Id = p.CategoryId
            ORDER BY p.ShortName COLLATE NOCASE;
            """;

        await using var reader = await command.ExecuteReaderAsync();
        while (await reader.ReadAsync())
        {
            var id = reader.GetInt32(0);
            var imageFileName = reader.IsDBNull(14) ? $"product_{id}.jpg" : reader.GetString(14);
            var imagePath = reader.IsDBNull(15)
                ? string.Empty
                : await MaterializeImageAsync(id, imageFileName, (byte[])reader[15]);

            products.Add(new Product
            {
                Id = id,
                ShortName = reader.GetString(1),
                FullName = reader.GetString(2),
                Description = reader.GetString(3),
                Category = reader.GetString(4),
                Brand = reader.GetString(5),
                Country = reader.GetString(6),
                Flavor = reader.GetString(7),
                Weight = reader.GetString(8),
                Price = Convert.ToDecimal(reader.GetDouble(9), CultureInfo.InvariantCulture),
                DiscountPercent = Convert.ToDecimal(reader.GetDouble(10), CultureInfo.InvariantCulture),
                Quantity = reader.GetInt32(11),
                Rating = reader.GetDouble(12),
                SoldCount = reader.GetInt32(13),
                ImagePath = imagePath
            });
        }

        return products;
    }

    public static async Task SaveProductsAsync(IEnumerable<Product> products)
    {
        await using var connection = CreateConnection();
        await connection.OpenAsync();
        await using var transaction = connection.BeginTransaction();

        foreach (var product in products)
        {
            var categoryId = await GetOrCreateCategoryIdAsync(connection, transaction, product.Category);
            var image = await ReadImageForStorageAsync(product.ImagePath);

            await using var command = connection.CreateCommand();
            command.Transaction = transaction;
            command.CommandText = """
                INSERT INTO Products
                    (Id, CategoryId, ShortName, FullName, Description, Brand, Country, Flavor,
                     Weight, Price, DiscountPercent, Quantity, Rating, SoldCount, ImageFileName, ImageData)
                VALUES
                    (@id, @categoryId, @shortName, @fullName, @description, @brand, @country, @flavor,
                     @weight, @price, @discount, @quantity, @rating, @soldCount, @imageFileName, @imageData)
                ON CONFLICT(Id) DO UPDATE SET
                    CategoryId = excluded.CategoryId,
                    ShortName = excluded.ShortName,
                    FullName = excluded.FullName,
                    Description = excluded.Description,
                    Brand = excluded.Brand,
                    Country = excluded.Country,
                    Flavor = excluded.Flavor,
                    Weight = excluded.Weight,
                    Price = excluded.Price,
                    DiscountPercent = excluded.DiscountPercent,
                    Quantity = excluded.Quantity,
                    Rating = excluded.Rating,
                    SoldCount = excluded.SoldCount,
                    ImageFileName = COALESCE(excluded.ImageFileName, Products.ImageFileName),
                    ImageData = COALESCE(excluded.ImageData, Products.ImageData);
                """;
            AddProductParameters(command, product, categoryId, image.FileName, image.Data);
            await command.ExecuteNonQueryAsync();
        }

        transaction.Commit();
    }

    public static async Task DeleteProductAsync(int id)
    {
        await using var connection = CreateConnection();
        await connection.OpenAsync();
        await using var transaction = connection.BeginTransaction();
        await using var command = connection.CreateCommand();
        command.Transaction = transaction;
        command.CommandText = "DELETE FROM Products WHERE Id = @id;";
        command.Parameters.AddWithValue("@id", id);
        await command.ExecuteNonQueryAsync();
        transaction.Commit();
    }

    public static async Task CreateOrderAsync(string login, IEnumerable<CartItem> cartItems)
    {
        var items = cartItems.ToList();
        if (items.Count == 0) return;

        await using var connection = CreateConnection();
        await connection.OpenAsync();
        await using var transaction = connection.BeginTransaction();

        var userId = await GetUserIdAsync(connection, transaction, login);
        var total = items.Sum(x => x.Total);

        await using var orderCommand = connection.CreateCommand();
        orderCommand.Transaction = transaction;
        orderCommand.CommandText = """
            INSERT INTO Orders(UserId, CreatedAt, Total, Status)
            VALUES (@userId, @createdAt, @total, @status);
            SELECT last_insert_rowid();
            """;
        orderCommand.Parameters.AddWithValue("@userId", userId);
        orderCommand.Parameters.AddWithValue("@createdAt", DateTimeOffset.Now.ToString("O"));
        orderCommand.Parameters.AddWithValue("@total", total);
        orderCommand.Parameters.AddWithValue("@status", "Создан");
        var orderId = Convert.ToInt64(await orderCommand.ExecuteScalarAsync(), CultureInfo.InvariantCulture);

        foreach (var item in items)
        {
            await using var stockCommand = connection.CreateCommand();
            stockCommand.Transaction = transaction;
            stockCommand.CommandText = """
                UPDATE Products
                SET Quantity = Quantity - @quantity,
                    SoldCount = SoldCount + @quantity
                WHERE Id = @productId AND Quantity >= @quantity;
                """;
            stockCommand.Parameters.AddWithValue("@quantity", item.Quantity);
            stockCommand.Parameters.AddWithValue("@productId", item.Product.Id);
            if (await stockCommand.ExecuteNonQueryAsync() != 1)
                throw new InvalidOperationException($"Недостаточно товара «{item.Product.ShortName}» на складе.");

            await using var itemCommand = connection.CreateCommand();
            itemCommand.Transaction = transaction;
            itemCommand.CommandText = """
                INSERT INTO OrderItems(OrderId, ProductId, ProductName, Quantity, UnitPrice)
                VALUES (@orderId, @productId, @productName, @quantity, @unitPrice);
                """;
            itemCommand.Parameters.AddWithValue("@orderId", orderId);
            itemCommand.Parameters.AddWithValue("@productId", item.Product.Id);
            itemCommand.Parameters.AddWithValue("@productName", item.Product.ShortName);
            itemCommand.Parameters.AddWithValue("@quantity", item.Quantity);
            itemCommand.Parameters.AddWithValue("@unitPrice", item.Product.FinalPrice);
            await itemCommand.ExecuteNonQueryAsync();
        }

        transaction.Commit();
    }

    public static bool TryLogin(string login, string password, out UserRole role)
    {
        role = UserRole.Client;
        using var connection = CreateConnection();
        connection.Open();
        using var command = connection.CreateCommand();
        command.CommandText = "SELECT Role FROM Users WHERE Login = @login AND Password = @password;";
        command.Parameters.AddWithValue("@login", login);
        command.Parameters.AddWithValue("@password", password);
        var result = command.ExecuteScalar()?.ToString();
        if (result is null) return false;
        role = string.Equals(result, "Admin", StringComparison.OrdinalIgnoreCase)
            ? UserRole.Admin
            : UserRole.Client;
        return true;
    }

    public static UserProfile LoadProfile(string login)
    {
        using var connection = CreateConnection();
        connection.Open();
        using var command = connection.CreateCommand();
        command.CommandText = """
            SELECT Login, FirstName, LastName, Email, Phone
            FROM Users WHERE Login = @login;
            """;
        command.Parameters.AddWithValue("@login", login);
        using var reader = command.ExecuteReader();
        if (!reader.Read())
            throw new InvalidOperationException("Пользователь не найден в базе данных.");
        return new UserProfile
        {
            Login = reader.GetString(0),
            FirstName = reader.GetString(1),
            LastName = reader.GetString(2),
            Email = reader.GetString(3),
            Phone = reader.GetString(4)
        };
    }

    public static void SaveProfile(UserProfile profile)
    {
        using var connection = CreateConnection();
        connection.Open();
        using var transaction = connection.BeginTransaction();
        using var command = connection.CreateCommand();
        command.Transaction = transaction;
        command.CommandText = """
            UPDATE Users
            SET FirstName = @firstName, LastName = @lastName, Email = @email, Phone = @phone
            WHERE Login = @login;
            """;
        command.Parameters.AddWithValue("@firstName", profile.FirstName);
        command.Parameters.AddWithValue("@lastName", profile.LastName);
        command.Parameters.AddWithValue("@email", profile.Email);
        command.Parameters.AddWithValue("@phone", profile.Phone);
        command.Parameters.AddWithValue("@login", profile.Login);
        command.ExecuteNonQuery();
        transaction.Commit();
    }

    public static async Task<DataTable> LoadTableAsync(string tableName)
    {
        EnsureAllowedTable(tableName);
        var table = new DataTable(tableName) { Locale = CultureInfo.InvariantCulture };
        await using var connection = CreateConnection();
        await connection.OpenAsync();
        await using var command = connection.CreateCommand();
        command.CommandText = $"SELECT * FROM [{tableName}] ORDER BY Id DESC;";
        await using var reader = await command.ExecuteReaderAsync();
        table.Load(reader);
        if (table.Columns.Contains("Id"))
            table.PrimaryKey = [table.Columns["Id"]!];
        table.AcceptChanges();
        return table;
    }

    public static async Task SaveTableAsync(string tableName, DataTable table)
    {
        EnsureAllowedTable(tableName);
        if (!EditableTables.Contains(tableName))
            throw new InvalidOperationException("Эта таблица доступна только для просмотра.");

        await using var connection = CreateConnection();
        await connection.OpenAsync();
        await using var transaction = connection.BeginTransaction();

        foreach (DataRow row in table.Rows)
        {
            if (row.RowState == DataRowState.Unchanged) continue;
            await using var command = connection.CreateCommand();
            command.Transaction = transaction;

            if (row.RowState == DataRowState.Deleted)
            {
                command.CommandText = $"DELETE FROM [{tableName}] WHERE Id = @id;";
                command.Parameters.AddWithValue("@id", row["Id", DataRowVersion.Original]);
            }
            else if (row.RowState == DataRowState.Added)
            {
                var columns = table.Columns.Cast<DataColumn>()
                    .Where(c => !string.Equals(c.ColumnName, "Id", StringComparison.OrdinalIgnoreCase))
                    .ToList();
                command.CommandText = $"INSERT INTO [{tableName}] ({string.Join(", ", columns.Select(c => $"[{c.ColumnName}]"))}) VALUES ({string.Join(", ", columns.Select((_, i) => $"@p{i}"))});";
                for (var i = 0; i < columns.Count; i++)
                    command.Parameters.AddWithValue($"@p{i}", NormalizeValue(row[columns[i]]));
            }
            else
            {
                var columns = table.Columns.Cast<DataColumn>()
                    .Where(c => !string.Equals(c.ColumnName, "Id", StringComparison.OrdinalIgnoreCase))
                    .ToList();
                command.CommandText = $"UPDATE [{tableName}] SET {string.Join(", ", columns.Select((c, i) => $"[{c.ColumnName}] = @p{i}"))} WHERE Id = @id;";
                for (var i = 0; i < columns.Count; i++)
                    command.Parameters.AddWithValue($"@p{i}", NormalizeValue(row[columns[i]]));
                command.Parameters.AddWithValue("@id", row["Id"]);
            }

            await command.ExecuteNonQueryAsync();
        }

        transaction.Commit();
        table.AcceptChanges();
    }

    public static async Task<DataTable> GetLowStockAsync(int limit)
    {
        var table = new DataTable("LowStock") { Locale = CultureInfo.InvariantCulture };
        await using var connection = CreateConnection();
        await connection.OpenAsync();
        await using var command = connection.CreateCommand();
        command.CommandText = """
            SELECT Id, ShortName, Brand, Quantity, Price
            FROM Products
            WHERE Quantity <= @limit
            ORDER BY Quantity, ShortName COLLATE NOCASE;
            """;
        command.Parameters.AddWithValue("@limit", limit);
        await using var reader = await command.ExecuteReaderAsync();
        table.Load(reader);
        return table;
    }

    /// <summary>
    /// SQLite has no native stored procedures. This executes a named SQL command stored in the DB,
    /// which is the documented SQLite equivalent used by this laboratory work.
    /// </summary>
    public static async Task<DataTable> ExecuteStoredCommandAsync(string name, params SqliteParameter[] parameters)
    {
        await using var connection = CreateConnection();
        await connection.OpenAsync();

        await using var lookup = connection.CreateCommand();
        lookup.CommandText = "SELECT SqlText FROM StoredCommands WHERE Name = @name;";
        lookup.Parameters.AddWithValue("@name", name);
        var sql = (string?)await lookup.ExecuteScalarAsync()
                  ?? throw new InvalidOperationException($"Именованная команда «{name}» не найдена.");

        var table = new DataTable(name) { Locale = CultureInfo.InvariantCulture };
        await using var command = connection.CreateCommand();
        command.CommandText = sql;
        command.Parameters.AddRange(parameters);
        await using var reader = await command.ExecuteReaderAsync();
        table.Load(reader);
        return table;
    }

    private static string BuildConnectionString()
    {
        var configured = ReadConnectionString();
        var builder = new SqliteConnectionStringBuilder(configured);
        var dataSource = builder.DataSource;
        if (string.IsNullOrWhiteSpace(dataSource)) dataSource = Path.Combine("Data", "SportNutritionShop.db");
        if (!Path.IsPathRooted(dataSource)) dataSource = Path.GetFullPath(Path.Combine(AppContext.BaseDirectory, dataSource));
        DatabasePath = dataSource;
        Directory.CreateDirectory(Path.GetDirectoryName(DatabasePath)!);
        builder.DataSource = DatabasePath;
        builder.ForeignKeys = true;
        builder.DefaultTimeout = ReadIntSetting("CommandTimeoutSeconds", 30);
        return builder.ToString();
    }

    private static string ReadConnectionString()
    {
        var candidates = new[]
        {
            Path.Combine(AppContext.BaseDirectory, "App.config"),
            Path.Combine(AppContext.BaseDirectory, "SportNutritionShop.dll.config"),
            Path.Combine(AppContext.BaseDirectory, "SportNutritionShop.exe.config"),
            AppContext.BaseDirectory + AppDomain.CurrentDomain.FriendlyName + ".config"
        };

        foreach (var path in candidates.Where(File.Exists))
        {
            var document = XDocument.Load(path);
            var value = document.Root?.Element("connectionStrings")?.Elements("add")
                .FirstOrDefault(x => string.Equals((string?)x.Attribute("name"), "SportNutritionShopDb", StringComparison.Ordinal))?
                .Attribute("connectionString")?.Value;
            if (!string.IsNullOrWhiteSpace(value)) return value;
        }

        return "Data Source=Data\\SportNutritionShop.db;Foreign Keys=True";
    }

    private static int ReadIntSetting(string key, int fallback)
    {
        var candidates = new[]
        {
            Path.Combine(AppContext.BaseDirectory, "App.config"),
            Path.Combine(AppContext.BaseDirectory, "SportNutritionShop.dll.config"),
            Path.Combine(AppContext.BaseDirectory, "SportNutritionShop.exe.config"),
            AppContext.BaseDirectory + AppDomain.CurrentDomain.FriendlyName + ".config"
        };

        foreach (var path in candidates.Where(File.Exists))
        {
            var document = XDocument.Load(path);
            var value = document.Root?.Element("appSettings")?.Elements("add")
                .FirstOrDefault(x => string.Equals((string?)x.Attribute("key"), key, StringComparison.Ordinal))?
                .Attribute("value")?.Value;
            if (int.TryParse(value, CultureInfo.InvariantCulture, out var result) && result > 0)
                return result;
        }

        return fallback;
    }

    private static async Task SeedAsync(SqliteConnection connection)
    {
        await using var transaction = connection.BeginTransaction();

        foreach (var category in new[] { "Креатин", "Протеин", "Аминокислоты", "Гейнер", "Витамины", "Предтрен" })
        {
            await using var categoryCommand = connection.CreateCommand();
            categoryCommand.Transaction = transaction;
            categoryCommand.CommandText = "INSERT OR IGNORE INTO Categories(Name) VALUES (@name);";
            categoryCommand.Parameters.AddWithValue("@name", category);
            await categoryCommand.ExecuteNonQueryAsync();
        }

        foreach (var user in new[] { ("admin", "123", "Admin"), ("client", "123", "Client") })
        {
            await using var userCommand = connection.CreateCommand();
            userCommand.Transaction = transaction;
            userCommand.CommandText = """
                INSERT OR IGNORE INTO Users(Login, Password, Role, FirstName)
                VALUES (@login, @password, @role, @firstName);
                """;
            userCommand.Parameters.AddWithValue("@login", user.Item1);
            userCommand.Parameters.AddWithValue("@password", user.Item2);
            userCommand.Parameters.AddWithValue("@role", user.Item3);
            userCommand.Parameters.AddWithValue("@firstName", user.Item1);
            await userCommand.ExecuteNonQueryAsync();
        }

        await using (var storedCommand = connection.CreateCommand())
        {
            storedCommand.Transaction = transaction;
            storedCommand.CommandText = """
                INSERT OR REPLACE INTO StoredCommands(Name, SqlText, Description)
                VALUES
                ('sp_ProductStatistics',
                 'SELECT c.Name AS Category, COUNT(p.Id) AS ProductCount, ROUND(AVG(p.Price), 2) AS AveragePrice, SUM(p.Quantity) AS Stock FROM Categories c LEFT JOIN Products p ON p.CategoryId = c.Id GROUP BY c.Id, c.Name ORDER BY c.Name;',
                 'Статистика каталога по категориям — аналог хранимой процедуры для SQLite'),
                ('sp_OrdersByUser',
                 'SELECT o.Id, o.CreatedAt, o.Total, o.Status FROM Orders o JOIN Users u ON u.Id = o.UserId WHERE u.Login = @login ORDER BY o.CreatedAt DESC;',
                 'Заказы выбранного пользователя — параметризованный аналог хранимой процедуры');
                """;
            await storedCommand.ExecuteNonQueryAsync();
        }

        await using var countCommand = connection.CreateCommand();
        countCommand.Transaction = transaction;
        countCommand.CommandText = "SELECT COUNT(*) FROM Products;";
        if (Convert.ToInt32(await countCommand.ExecuteScalarAsync(), CultureInfo.InvariantCulture) == 0)
        {
            foreach (var product in SeedProducts())
            {
                var categoryId = await GetOrCreateCategoryIdAsync(connection, transaction, product.Category);
                var image = await ReadImageForStorageAsync(product.ImagePath);
                await using var productCommand = connection.CreateCommand();
                productCommand.Transaction = transaction;
                productCommand.CommandText = """
                    INSERT INTO Products
                        (Id, CategoryId, ShortName, FullName, Description, Brand, Country, Flavor,
                         Weight, Price, DiscountPercent, Quantity, Rating, SoldCount, ImageFileName, ImageData)
                    VALUES
                        (@id, @categoryId, @shortName, @fullName, @description, @brand, @country, @flavor,
                         @weight, @price, @discount, @quantity, @rating, @soldCount, @imageFileName, @imageData);
                    """;
                AddProductParameters(productCommand, product, categoryId, image.FileName, image.Data);
                await productCommand.ExecuteNonQueryAsync();
            }
        }

        transaction.Commit();
    }

    private static IEnumerable<Product> SeedProducts()
    {
        return
        [
            new() { Id = 1, ShortName = "Creatine Monohydrate", FullName = "OstroVit Creatine Monohydrate 300 g", Description = "Классический креатин для роста силы и восстановления.", Category = "Креатин", Brand = "OstroVit", Country = "Польша", Flavor = "Без вкуса", Weight = "300 г", Price = 48, DiscountPercent = 0, Quantity = 14, Rating = 4.8, SoldCount = 121, ImagePath = Asset("creatine.jpg") },
            new() { Id = 2, ShortName = "Whey Protein", FullName = "BioTech USA 100% Pure Whey 1000 g", Description = "Сывороточный белок для набора и сохранения мышечной массы.", Category = "Протеин", Brand = "BioTech USA", Country = "США", Flavor = "Chocolate", Weight = "1000 г", Price = 99, DiscountPercent = 10, Quantity = 8, Rating = 4.9, SoldCount = 250, ImagePath = Asset("whey.jpg") },
            new() { Id = 3, ShortName = "BCAA 2:1:1", FullName = "Mutant BCAA 2:1:1 400 g", Description = "Аминокислоты для поддержки мышц и снижения катаболизма.", Category = "Аминокислоты", Brand = "Mutant", Country = "Канада", Flavor = "Orange", Weight = "400 г", Price = 65, DiscountPercent = 5, Quantity = 12, Rating = 4.6, SoldCount = 89, ImagePath = Asset("bcaa.jpg") },
            new() { Id = 4, ShortName = "Mass Gainer", FullName = "Optimum Nutrition Serious Mass 2700 g", Description = "Высококалорийный гейнер для набора массы.", Category = "Гейнер", Brand = "Optimum Nutrition", Country = "США", Flavor = "Vanilla", Weight = "2700 г", Price = 170, DiscountPercent = 0, Quantity = 6, Rating = 4.7, SoldCount = 73, ImagePath = Asset("gainer.jpg") },
            new() { Id = 5, ShortName = "Multivitamin", FullName = "NOW Foods ADAM Superior Men's Multi", Description = "Комплекс витаминов и минералов на каждый день.", Category = "Витамины", Brand = "NOW Foods", Country = "США", Flavor = "Neutral", Weight = "90 капсул", Price = 54, DiscountPercent = 0, Quantity = 20, Rating = 4.5, SoldCount = 61, ImagePath = Asset("vitamins.jpg") },
            new() { Id = 6, ShortName = "Pre-Workout", FullName = "Bombbar Pre Workout 300 g", Description = "Предтренировочный комплекс для энергии и концентрации.", Category = "Предтрен", Brand = "Bombbar", Country = "Россия", Flavor = "Berry", Weight = "300 г", Price = 59, DiscountPercent = 0, Quantity = 10, Rating = 4.4, SoldCount = 97, ImagePath = Asset("preworkout.jpg") }
        ];
    }

    private static string Asset(string name) => Path.Combine(AppContext.BaseDirectory, "Assets", "Products", name);

    private static async Task<int> GetOrCreateCategoryIdAsync(SqliteConnection connection, SqliteTransaction transaction, string category)
    {
        var normalized = string.IsNullOrWhiteSpace(category) ? "Без категории" : category.Trim();
        await using var insert = connection.CreateCommand();
        insert.Transaction = transaction;
        insert.CommandText = "INSERT OR IGNORE INTO Categories(Name) VALUES (@name);";
        insert.Parameters.AddWithValue("@name", normalized);
        await insert.ExecuteNonQueryAsync();

        await using var select = connection.CreateCommand();
        select.Transaction = transaction;
        select.CommandText = "SELECT Id FROM Categories WHERE Name = @name;";
        select.Parameters.AddWithValue("@name", normalized);
        return Convert.ToInt32(await select.ExecuteScalarAsync(), CultureInfo.InvariantCulture);
    }

    private static async Task<int> GetUserIdAsync(SqliteConnection connection, SqliteTransaction transaction, string login)
    {
        await using var command = connection.CreateCommand();
        command.Transaction = transaction;
        command.CommandText = "SELECT Id FROM Users WHERE Login = @login;";
        command.Parameters.AddWithValue("@login", login);
        return Convert.ToInt32(await command.ExecuteScalarAsync() ?? throw new InvalidOperationException("Пользователь не найден."), CultureInfo.InvariantCulture);
    }

    private static void AddProductParameters(SqliteCommand command, Product product, int categoryId, string? imageFileName, byte[]? imageData)
    {
        command.Parameters.AddWithValue("@id", product.Id);
        command.Parameters.AddWithValue("@categoryId", categoryId);
        command.Parameters.AddWithValue("@shortName", product.ShortName.Trim());
        command.Parameters.AddWithValue("@fullName", product.FullName.Trim());
        command.Parameters.AddWithValue("@description", product.Description.Trim());
        command.Parameters.AddWithValue("@brand", product.Brand.Trim());
        command.Parameters.AddWithValue("@country", product.Country.Trim());
        command.Parameters.AddWithValue("@flavor", product.Flavor.Trim());
        command.Parameters.AddWithValue("@weight", product.Weight.Trim());
        command.Parameters.AddWithValue("@price", product.Price);
        command.Parameters.AddWithValue("@discount", product.DiscountPercent);
        command.Parameters.AddWithValue("@quantity", product.Quantity);
        command.Parameters.AddWithValue("@rating", product.Rating);
        command.Parameters.AddWithValue("@soldCount", product.SoldCount);
        command.Parameters.AddWithValue("@imageFileName", (object?)imageFileName ?? DBNull.Value);
        command.Parameters.AddWithValue("@imageData", (object?)imageData ?? DBNull.Value);
    }

    private static async Task<(string? FileName, byte[]? Data)> ReadImageForStorageAsync(string path)
    {
        if (string.IsNullOrWhiteSpace(path)) return (null, null);
        var absolutePath = Path.IsPathRooted(path) ? path : Path.Combine(AppContext.BaseDirectory, path.Replace('/', Path.DirectorySeparatorChar));
        if (!File.Exists(absolutePath)) return (null, null);
        return (Path.GetFileName(absolutePath), await File.ReadAllBytesAsync(absolutePath));
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

    private static object NormalizeValue(object value) => value == DBNull.Value ? DBNull.Value : value;

    private static void EnsureAllowedTable(string tableName)
    {
        if (!AllowedTables.Contains(tableName))
            throw new ArgumentException("Недопустимое имя таблицы.", nameof(tableName));
    }
}
