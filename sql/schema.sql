/* ============================================================================
   MicroDemo — schema.sql
   Cria o banco de dados e todas as tabelas do domínio (SQL Server / T-SQL).
   O schema espelha exatamente o modelo do EF Core (migration InitialCreate).

   Ordem de criação respeita as dependências de FK:
     Prices → Utenti → Subscriptions → Orders

   Enums são persistidos como INT:
     BillingPeriod:      1=Monthly, 2=Quarterly, 3=Yearly
     SubscriptionStatus: 1=Active,  2=Cancelled, 3=Expired
     OrderStatus:        1=Pending, 2=Paid, 3=Failed, 4=Refunded
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
IF OBJECT_ID(N'[dbo].[Orders]', N'U')        IS NOT NULL DROP TABLE [dbo].[Orders];
IF OBJECT_ID(N'[dbo].[Subscriptions]', N'U') IS NOT NULL DROP TABLE [dbo].[Subscriptions];
IF OBJECT_ID(N'[dbo].[Utenti]', N'U')        IS NOT NULL DROP TABLE [dbo].[Utenti];
IF OBJECT_ID(N'[dbo].[Prices]', N'U')        IS NOT NULL DROP TABLE [dbo].[Prices];
GO

/* --------------------------------------------------------------------------
   Prices (planos/preços)
   -------------------------------------------------------------------------- */
CREATE TABLE [dbo].[Prices] (
    [Id]            UNIQUEIDENTIFIER NOT NULL CONSTRAINT [PK_Prices] PRIMARY KEY,
    [Name]          NVARCHAR(120)    NOT NULL,
    [Description]   NVARCHAR(500)    NULL,
    [Amount]        DECIMAL(18,2)    NOT NULL,
    [Currency]      NCHAR(3)         NOT NULL,
    [BillingPeriod] INT              NOT NULL,
    [IsActive]      BIT              NOT NULL,
    [CreatedAtUtc]  DATETIME2        NOT NULL,
    [UpdatedAtUtc]  DATETIME2        NULL
);
GO
CREATE INDEX [IX_Prices_Name] ON [dbo].[Prices] ([Name]);
GO

/* --------------------------------------------------------------------------
   Utenti (usuários/clientes) — pode ter um plano/preço padrão
   -------------------------------------------------------------------------- */
CREATE TABLE [dbo].[Utenti] (
    [Id]             UNIQUEIDENTIFIER NOT NULL CONSTRAINT [PK_Utenti] PRIMARY KEY,
    [FullName]       NVARCHAR(150)    NOT NULL,
    [Email]          NVARCHAR(256)    NOT NULL,
    [PhoneNumber]    NVARCHAR(30)     NULL,
    [IsActive]       BIT              NOT NULL,
    [DefaultPriceId] UNIQUEIDENTIFIER NULL,
    [CreatedAtUtc]   DATETIME2        NOT NULL,
    [UpdatedAtUtc]   DATETIME2        NULL,
    CONSTRAINT [FK_Utenti_Prices_DefaultPriceId]
        FOREIGN KEY ([DefaultPriceId]) REFERENCES [dbo].[Prices] ([Id]) ON DELETE SET NULL
);
GO
CREATE UNIQUE INDEX [IX_Utenti_Email]          ON [dbo].[Utenti] ([Email]);
CREATE INDEX        [IX_Utenti_DefaultPriceId] ON [dbo].[Utenti] ([DefaultPriceId]);
GO

/* --------------------------------------------------------------------------
   Subscriptions (associação temporal Utente ↔ Price)
   -------------------------------------------------------------------------- */
CREATE TABLE [dbo].[Subscriptions] (
    [Id]           UNIQUEIDENTIFIER NOT NULL CONSTRAINT [PK_Subscriptions] PRIMARY KEY,
    [UtenteId]     UNIQUEIDENTIFIER NOT NULL,
    [PriceId]      UNIQUEIDENTIFIER NOT NULL,
    [StartDateUtc] DATETIME2        NOT NULL,
    [EndDateUtc]   DATETIME2        NULL,
    [Status]       INT              NOT NULL,
    [CreatedAtUtc] DATETIME2        NOT NULL,
    [UpdatedAtUtc] DATETIME2        NULL,
    CONSTRAINT [FK_Subscriptions_Utenti_UtenteId]
        FOREIGN KEY ([UtenteId]) REFERENCES [dbo].[Utenti] ([Id]) ON DELETE CASCADE,
    CONSTRAINT [FK_Subscriptions_Prices_PriceId]
        FOREIGN KEY ([PriceId]) REFERENCES [dbo].[Prices] ([Id]) ON DELETE NO ACTION
);
GO
CREATE INDEX [IX_Subscriptions_PriceId]         ON [dbo].[Subscriptions] ([PriceId]);
CREATE INDEX [IX_Subscriptions_UtenteId_Status] ON [dbo].[Subscriptions] ([UtenteId], [Status]);
GO

/* --------------------------------------------------------------------------
   Orders (pedidos/cobranças)
   -------------------------------------------------------------------------- */
CREATE TABLE [dbo].[Orders] (
    [Id]             UNIQUEIDENTIFIER NOT NULL CONSTRAINT [PK_Orders] PRIMARY KEY,
    [UtenteId]       UNIQUEIDENTIFIER NOT NULL,
    [SubscriptionId] UNIQUEIDENTIFIER NULL,
    [TotalAmount]    DECIMAL(18,2)    NOT NULL,
    [Currency]       NCHAR(3)         NOT NULL,
    [Status]         INT              NOT NULL,
    [OrderDateUtc]   DATETIME2        NOT NULL,
    [CreatedAtUtc]   DATETIME2        NOT NULL,
    [UpdatedAtUtc]   DATETIME2        NULL,
    CONSTRAINT [FK_Orders_Utenti_UtenteId]
        FOREIGN KEY ([UtenteId]) REFERENCES [dbo].[Utenti] ([Id]) ON DELETE CASCADE,
    CONSTRAINT [FK_Orders_Subscriptions_SubscriptionId]
        FOREIGN KEY ([SubscriptionId]) REFERENCES [dbo].[Subscriptions] ([Id]) ON DELETE SET NULL
);
GO
CREATE INDEX [IX_Orders_UtenteId]       ON [dbo].[Orders] ([UtenteId]);
CREATE INDEX [IX_Orders_SubscriptionId] ON [dbo].[Orders] ([SubscriptionId]);
GO
