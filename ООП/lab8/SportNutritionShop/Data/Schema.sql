PRAGMA foreign_keys = ON;

CREATE TABLE IF NOT EXISTS Categories (
    Id INTEGER PRIMARY KEY AUTOINCREMENT,
    Name TEXT NOT NULL UNIQUE
);

CREATE TABLE IF NOT EXISTS Users (
    Id INTEGER PRIMARY KEY AUTOINCREMENT,
    Login TEXT NOT NULL UNIQUE,
    Password TEXT NOT NULL,
    Role TEXT NOT NULL CHECK (Role IN ('Admin', 'Client')),
    FirstName TEXT NOT NULL DEFAULT '',
    LastName TEXT NOT NULL DEFAULT '',
    Email TEXT NOT NULL DEFAULT '',
    Phone TEXT NOT NULL DEFAULT ''
);

CREATE TABLE IF NOT EXISTS Products (
    Id INTEGER PRIMARY KEY AUTOINCREMENT,
    CategoryId INTEGER NOT NULL,
    ShortName TEXT NOT NULL,
    FullName TEXT NOT NULL,
    Description TEXT NOT NULL,
    Brand TEXT NOT NULL,
    Country TEXT NOT NULL,
    Flavor TEXT NOT NULL,
    Weight TEXT NOT NULL,
    Price REAL NOT NULL CHECK (Price >= 0),
    DiscountPercent REAL NOT NULL DEFAULT 0 CHECK (DiscountPercent BETWEEN 0 AND 100),
    Quantity INTEGER NOT NULL DEFAULT 0 CHECK (Quantity >= 0),
    Rating REAL NOT NULL DEFAULT 0 CHECK (Rating BETWEEN 0 AND 5),
    SoldCount INTEGER NOT NULL DEFAULT 0 CHECK (SoldCount >= 0),
    ImageFileName TEXT,
    ImageData BLOB,
    FOREIGN KEY (CategoryId) REFERENCES Categories(Id) ON UPDATE CASCADE ON DELETE RESTRICT
);

CREATE TABLE IF NOT EXISTS Orders (
    Id INTEGER PRIMARY KEY AUTOINCREMENT,
    UserId INTEGER NOT NULL,
    CreatedAt TEXT NOT NULL,
    Total REAL NOT NULL CHECK (Total >= 0),
    Status TEXT NOT NULL DEFAULT 'Создан',
    FOREIGN KEY (UserId) REFERENCES Users(Id) ON UPDATE CASCADE ON DELETE RESTRICT
);

CREATE TABLE IF NOT EXISTS OrderItems (
    Id INTEGER PRIMARY KEY AUTOINCREMENT,
    OrderId INTEGER NOT NULL,
    ProductId INTEGER,
    ProductName TEXT NOT NULL,
    Quantity INTEGER NOT NULL CHECK (Quantity > 0),
    UnitPrice REAL NOT NULL CHECK (UnitPrice >= 0),
    FOREIGN KEY (OrderId) REFERENCES Orders(Id) ON UPDATE CASCADE ON DELETE CASCADE,
    FOREIGN KEY (ProductId) REFERENCES Products(Id) ON UPDATE CASCADE ON DELETE SET NULL
);

CREATE TABLE IF NOT EXISTS AuditLog (
    Id INTEGER PRIMARY KEY AUTOINCREMENT,
    TableName TEXT NOT NULL,
    Action TEXT NOT NULL,
    RecordId INTEGER,
    ChangedAt TEXT NOT NULL
);

CREATE TABLE IF NOT EXISTS StoredCommands (
    Id INTEGER PRIMARY KEY AUTOINCREMENT,
    Name TEXT NOT NULL UNIQUE,
    SqlText TEXT NOT NULL,
    Description TEXT NOT NULL
);

CREATE VIEW IF NOT EXISTS vw_ProductCatalog AS
SELECT p.Id,
       p.ShortName,
       p.FullName,
       c.Name AS Category,
       p.Brand,
       p.Price,
       p.DiscountPercent,
       ROUND(p.Price * (1 - p.DiscountPercent / 100.0), 2) AS FinalPrice,
       p.Quantity,
       p.Rating,
       p.SoldCount
FROM Products p
JOIN Categories c ON c.Id = p.CategoryId;

CREATE TRIGGER IF NOT EXISTS trg_Products_Insert
AFTER INSERT ON Products
BEGIN
    INSERT INTO AuditLog(TableName, Action, RecordId, ChangedAt)
    VALUES ('Products', 'INSERT', NEW.Id, datetime('now'));
END;

CREATE TRIGGER IF NOT EXISTS trg_Products_Update
AFTER UPDATE ON Products
BEGIN
    INSERT INTO AuditLog(TableName, Action, RecordId, ChangedAt)
    VALUES ('Products', 'UPDATE', NEW.Id, datetime('now'));
END;

CREATE TRIGGER IF NOT EXISTS trg_Products_Delete
AFTER DELETE ON Products
BEGIN
    INSERT INTO AuditLog(TableName, Action, RecordId, ChangedAt)
    VALUES ('Products', 'DELETE', OLD.Id, datetime('now'));
END;
