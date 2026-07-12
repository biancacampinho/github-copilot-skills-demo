/* ============================================================================
   MicroDemo — schema.sql
   Cria o banco de dados e todas as tabelas do domínio de E-COMMERCE
   (SQL Server / T-SQL). O schema espelha exatamente o modelo do EF Core
   (migration InitialCreate).

   Ordem de criação respeita as dependências de FK:
     Categories → Users → Products → Orders → Prices → OrderItems

   Enums são persistidos como INT:
     OrderStatus: 1=Pending, 2=Paid, 3=Shipped, 4=Delivered, 5=Cancelled
   ============================================================================ */

IF DB_ID(N'MicroDemoDb') IS NULL
BEGIN
    CREATE DATABASE [MicroDemoDb];
END;
GO

USE [MicroDemoDb];
GO

/* --------------------------------------------------------------------------
   Limpeza idempotente (drop na ordem inversa das dependências)
   -------------------------------------------------------------------------- */
IF OBJECT_ID(N'[dbo].[OrderItems]', N'U') IS NOT NULL DROP TABLE [dbo].[OrderItems];
IF OBJECT_ID(N'[dbo].[Prices]', N'U')     IS NOT NULL DROP TABLE [dbo].[Prices];
IF OBJECT_ID(N'[dbo].[Orders]', N'U')     IS NOT NULL DROP TABLE [dbo].[Orders];
IF OBJECT_ID(N'[dbo].[Products]', N'U')   IS NOT NULL DROP TABLE [dbo].[Products];
IF OBJECT_ID(N'[dbo].[Users]', N'U')      IS NOT NULL DROP TABLE [dbo].[Users];
IF OBJECT_ID(N'[dbo].[Categories]', N'U') IS NOT NULL DROP TABLE [dbo].[Categories];
GO

/* --------------------------------------------------------------------------
   Categories (categorias de produtos)
   -------------------------------------------------------------------------- */
CREATE TABLE [dbo].[Categories] (
    [Id]           UNIQUEIDENTIFIER NOT NULL CONSTRAINT [PK_Categories] PRIMARY KEY,
    [Name]         NVARCHAR(120)    NOT NULL,
    [Description]  NVARCHAR(500)    NULL,
    [IsActive]     BIT              NOT NULL,
    [CreatedAtUtc] DATETIME2        NOT NULL,
    [UpdatedAtUtc] DATETIME2        NULL
);
GO
CREATE UNIQUE INDEX [IX_Categories_Name] ON [dbo].[Categories] ([Name]);
GO

/* --------------------------------------------------------------------------
   Users (clientes/compradores)
   -------------------------------------------------------------------------- */
CREATE TABLE [dbo].[Users] (
    [Id]           UNIQUEIDENTIFIER NOT NULL CONSTRAINT [PK_Users] PRIMARY KEY,
    [FullName]     NVARCHAR(150)    NOT NULL,
    [Email]        NVARCHAR(256)    NOT NULL,
    [PhoneNumber]  NVARCHAR(30)     NULL,
    [IsActive]     BIT              NOT NULL,
    [CreatedAtUtc] DATETIME2        NOT NULL,
    [UpdatedAtUtc] DATETIME2        NULL
);
GO
CREATE UNIQUE INDEX [IX_Users_Email] ON [dbo].[Users] ([Email]);
GO

/* --------------------------------------------------------------------------
   Products (produtos) — pertence a uma categoria
   -------------------------------------------------------------------------- */
CREATE TABLE [dbo].[Products] (
    [Id]           UNIQUEIDENTIFIER NOT NULL CONSTRAINT [PK_Products] PRIMARY KEY,
    [Name]         NVARCHAR(150)    NOT NULL,
    [Description]  NVARCHAR(1000)   NULL,
    [Sku]          NVARCHAR(50)     NOT NULL,
    [IsActive]     BIT              NOT NULL,
    [CategoryId]   UNIQUEIDENTIFIER NOT NULL,
    [CreatedAtUtc] DATETIME2        NOT NULL,
    [UpdatedAtUtc] DATETIME2        NULL,
    CONSTRAINT [FK_Products_Categories_CategoryId]
        FOREIGN KEY ([CategoryId]) REFERENCES [dbo].[Categories] ([Id]) ON DELETE NO ACTION
);
GO
CREATE UNIQUE INDEX [IX_Products_Sku]        ON [dbo].[Products] ([Sku]);
CREATE INDEX        [IX_Products_CategoryId] ON [dbo].[Products] ([CategoryId]);
GO

/* --------------------------------------------------------------------------
   Orders (pedidos) — realizados por um utilizador
   -------------------------------------------------------------------------- */
CREATE TABLE [dbo].[Orders] (
    [Id]           UNIQUEIDENTIFIER NOT NULL CONSTRAINT [PK_Orders] PRIMARY KEY,
    [UserId]       UNIQUEIDENTIFIER NOT NULL,
    [TotalAmount]  DECIMAL(18,2)    NOT NULL,
    [Currency]     NCHAR(3)         NOT NULL,
    [Status]       INT              NOT NULL,
    [OrderDateUtc] DATETIME2        NOT NULL,
    [CreatedAtUtc] DATETIME2        NOT NULL,
    [UpdatedAtUtc] DATETIME2        NULL,
    CONSTRAINT [FK_Orders_Users_UserId]
        FOREIGN KEY ([UserId]) REFERENCES [dbo].[Users] ([Id]) ON DELETE CASCADE
);
GO
CREATE INDEX [IX_Orders_UserId] ON [dbo].[Orders] ([UserId]);
GO

/* --------------------------------------------------------------------------
   Prices (histórico de preços de um produto)
   -------------------------------------------------------------------------- */
CREATE TABLE [dbo].[Prices] (
    [Id]           UNIQUEIDENTIFIER NOT NULL CONSTRAINT [PK_Prices] PRIMARY KEY,
    [ProductId]    UNIQUEIDENTIFIER NOT NULL,
    [Amount]       DECIMAL(18,2)    NOT NULL,
    [Currency]     NCHAR(3)         NOT NULL,
    [ValidFromUtc] DATETIME2        NOT NULL,
    [IsActive]     BIT              NOT NULL,
    [CreatedAtUtc] DATETIME2        NOT NULL,
    [UpdatedAtUtc] DATETIME2        NULL,
    CONSTRAINT [FK_Prices_Products_ProductId]
        FOREIGN KEY ([ProductId]) REFERENCES [dbo].[Products] ([Id]) ON DELETE CASCADE
);
GO
CREATE INDEX [IX_Prices_ProductId_IsActive_ValidFromUtc]
    ON [dbo].[Prices] ([ProductId], [IsActive], [ValidFromUtc]);
GO

/* --------------------------------------------------------------------------
   OrderItems (linhas do pedido) — liga Product + Order, com snapshot do preço
   -------------------------------------------------------------------------- */
CREATE TABLE [dbo].[OrderItems] (
    [Id]           UNIQUEIDENTIFIER NOT NULL CONSTRAINT [PK_OrderItems] PRIMARY KEY,
    [OrderId]      UNIQUEIDENTIFIER NOT NULL,
    [ProductId]    UNIQUEIDENTIFIER NOT NULL,
    [Quantity]     INT              NOT NULL,
    [UnitPrice]    DECIMAL(18,2)    NOT NULL,
    [Currency]     NCHAR(3)         NOT NULL,
    [LineTotal]    DECIMAL(18,2)    NOT NULL,
    [CreatedAtUtc] DATETIME2        NOT NULL,
    [UpdatedAtUtc] DATETIME2        NULL,
    CONSTRAINT [FK_OrderItems_Orders_OrderId]
        FOREIGN KEY ([OrderId]) REFERENCES [dbo].[Orders] ([Id]) ON DELETE CASCADE,
    CONSTRAINT [FK_OrderItems_Products_ProductId]
        FOREIGN KEY ([ProductId]) REFERENCES [dbo].[Products] ([Id]) ON DELETE NO ACTION
);
GO
CREATE INDEX [IX_OrderItems_OrderId]   ON [dbo].[OrderItems] ([OrderId]);
CREATE INDEX [IX_OrderItems_ProductId] ON [dbo].[OrderItems] ([ProductId]);
GO
