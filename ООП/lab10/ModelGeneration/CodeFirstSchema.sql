CREATE TABLE "AuditLog" (
    "Id" INTEGER NOT NULL CONSTRAINT "PK_AuditLog" PRIMARY KEY AUTOINCREMENT,
    "TableName" TEXT NOT NULL,
    "Action" TEXT NOT NULL,
    "RecordId" INTEGER NULL,
    "ChangedAt" TEXT NOT NULL
);


CREATE TABLE "Categories" (
    "Id" INTEGER NOT NULL CONSTRAINT "PK_Categories" PRIMARY KEY AUTOINCREMENT,
    "Name" TEXT NOT NULL
);


CREATE TABLE "StoredCommands" (
    "Id" INTEGER NOT NULL CONSTRAINT "PK_StoredCommands" PRIMARY KEY AUTOINCREMENT,
    "Name" TEXT NOT NULL,
    "SqlText" TEXT NOT NULL,
    "Description" TEXT NOT NULL
);


CREATE TABLE "Users" (
    "Id" INTEGER NOT NULL CONSTRAINT "PK_Users" PRIMARY KEY AUTOINCREMENT,
    "Login" TEXT NOT NULL,
    "Password" TEXT NOT NULL,
    "Role" TEXT NOT NULL,
    "FirstName" TEXT NOT NULL,
    "LastName" TEXT NOT NULL,
    "Email" TEXT NOT NULL,
    "Phone" TEXT NOT NULL
);


CREATE TABLE "Products" (
    "Id" INTEGER NOT NULL CONSTRAINT "PK_Products" PRIMARY KEY AUTOINCREMENT,
    "CategoryId" INTEGER NOT NULL,
    "ShortName" TEXT NOT NULL,
    "FullName" TEXT NOT NULL,
    "Description" TEXT NOT NULL,
    "Brand" TEXT NOT NULL,
    "Country" TEXT NOT NULL,
    "Flavor" TEXT NOT NULL,
    "Weight" TEXT NOT NULL,
    "Price" REAL NOT NULL,
    "DiscountPercent" REAL NOT NULL,
    "Quantity" INTEGER NOT NULL,
    "Rating" REAL NOT NULL,
    "SoldCount" INTEGER NOT NULL,
    "ImageFileName" TEXT NULL,
    "ImageData" BLOB NULL,
    CONSTRAINT "FK_Products_Categories_CategoryId" FOREIGN KEY ("CategoryId") REFERENCES "Categories" ("Id") ON DELETE RESTRICT
);


CREATE TABLE "Orders" (
    "Id" INTEGER NOT NULL CONSTRAINT "PK_Orders" PRIMARY KEY AUTOINCREMENT,
    "UserId" INTEGER NOT NULL,
    "CreatedAt" TEXT NOT NULL,
    "Total" REAL NOT NULL,
    "Status" TEXT NOT NULL,
    CONSTRAINT "FK_Orders_Users_UserId" FOREIGN KEY ("UserId") REFERENCES "Users" ("Id") ON DELETE RESTRICT
);


CREATE TABLE "OrderItems" (
    "Id" INTEGER NOT NULL CONSTRAINT "PK_OrderItems" PRIMARY KEY AUTOINCREMENT,
    "OrderId" INTEGER NOT NULL,
    "ProductId" INTEGER NULL,
    "ProductName" TEXT NOT NULL,
    "Quantity" INTEGER NOT NULL,
    "UnitPrice" REAL NOT NULL,
    CONSTRAINT "FK_OrderItems_Orders_OrderId" FOREIGN KEY ("OrderId") REFERENCES "Orders" ("Id") ON DELETE CASCADE,
    CONSTRAINT "FK_OrderItems_Products_ProductId" FOREIGN KEY ("ProductId") REFERENCES "Products" ("Id") ON DELETE SET NULL
);


CREATE UNIQUE INDEX "IX_Categories_Name" ON "Categories" ("Name");


CREATE INDEX "IX_OrderItems_OrderId" ON "OrderItems" ("OrderId");


CREATE INDEX "IX_OrderItems_ProductId" ON "OrderItems" ("ProductId");


CREATE INDEX "IX_Orders_UserId" ON "Orders" ("UserId");


CREATE INDEX "IX_Products_CategoryId" ON "Products" ("CategoryId");


CREATE UNIQUE INDEX "IX_StoredCommands_Name" ON "StoredCommands" ("Name");


CREATE UNIQUE INDEX "IX_Users_Login" ON "Users" ("Login");


