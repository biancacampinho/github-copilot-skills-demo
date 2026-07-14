/* ============================================================================
   SkillGhcDemo — seed.sql
   Dados de exemplo realistas de E-COMMERCE. Execute APÓS schema.sql.
   Usa GUIDs fixos para permitir reexecução determinística e referências entre
   tabelas. É idempotente: apaga os registros conhecidos antes de reinserir.

   Enums (INT):
     OrderStatus: 1=Pending, 2=Paid, 3=Shipped, 4=Delivered, 5=Cancelled
   ============================================================================ */

USE [SkillGhcDemoDb];
GO

/* Limpeza (ordem inversa das FKs) ------------------------------------------ */
DELETE FROM [dbo].[OrderItems];
DELETE FROM [dbo].[Prices];
DELETE FROM [dbo].[Orders];
DELETE FROM [dbo].[Products];
DELETE FROM [dbo].[Users];
DELETE FROM [dbo].[Categories];
GO

/* --------------------------------------------------------------------------
   Categories
   -------------------------------------------------------------------------- */
INSERT INTO [dbo].[Categories]
    ([Id], [Name], [Description], [IsActive], [CreatedAtUtc], [UpdatedAtUtc])
VALUES
    ('a1111111-1111-1111-1111-111111111101', N'Eletrónica',      N'Dispositivos e acessórios eletrónicos', 1, SYSUTCDATETIME(), NULL),
    ('a1111111-1111-1111-1111-111111111102', N'Livros',          N'Livros físicos e e-books',              1, SYSUTCDATETIME(), NULL),
    ('a1111111-1111-1111-1111-111111111103', N'Casa & Cozinha',  N'Utensílios e eletrodomésticos',         1, SYSUTCDATETIME(), NULL),
    ('a1111111-1111-1111-1111-111111111104', N'Descontinuado',   N'Categoria mantida apenas para histórico', 0, SYSUTCDATETIME(), NULL);
GO

/* --------------------------------------------------------------------------
   Users (clientes)
   -------------------------------------------------------------------------- */
INSERT INTO [dbo].[Users]
    ([Id], [FullName], [Email], [PhoneNumber], [IsActive], [CreatedAtUtc], [UpdatedAtUtc])
VALUES
    ('22222222-2222-2222-2222-222222222201', N'Mario Rossi',    N'mario.rossi@example.com',    N'+39 333 1112221', 1, SYSUTCDATETIME(), NULL),
    ('22222222-2222-2222-2222-222222222202', N'Giulia Bianchi', N'giulia.bianchi@example.com', N'+39 333 2223332', 1, SYSUTCDATETIME(), NULL),
    ('22222222-2222-2222-2222-222222222203', N'Luca Verdi',     N'luca.verdi@example.com',     N'+39 333 3334443', 1, SYSUTCDATETIME(), NULL),
    ('22222222-2222-2222-2222-222222222204', N'Sofia Esposito', N'sofia.esposito@example.com', N'+39 333 4445554', 1, SYSUTCDATETIME(), NULL),
    ('22222222-2222-2222-2222-222222222205', N'Marco Ferrari',  N'marco.ferrari@example.com',  N'+39 333 5556665', 1, SYSUTCDATETIME(), NULL);
GO

/* --------------------------------------------------------------------------
   Products (CategoryId aponta para uma das categorias acima)
   -------------------------------------------------------------------------- */
INSERT INTO [dbo].[Products]
    ([Id], [Name], [Description], [Sku], [IsActive], [CategoryId], [CreatedAtUtc], [UpdatedAtUtc])
VALUES
    ('b3333333-3333-3333-3333-333333333301', N'Auscultadores Bluetooth', N'Over-ear com cancelamento de ruído', N'SKU-ELEC-001', 1, 'a1111111-1111-1111-1111-111111111101', SYSUTCDATETIME(), NULL),
    ('b3333333-3333-3333-3333-333333333302', N'Smartphone X',            N'6.5", 128GB, dual SIM',              N'SKU-ELEC-002', 1, 'a1111111-1111-1111-1111-111111111101', SYSUTCDATETIME(), NULL),
    ('b3333333-3333-3333-3333-333333333303', N'Teclado Mecânico',        N'Switches azuis, layout PT',          N'SKU-ELEC-003', 1, 'a1111111-1111-1111-1111-111111111101', SYSUTCDATETIME(), NULL),
    ('b3333333-3333-3333-3333-333333333304', N'Clean Architecture',      N'Robert C. Martin (livro)',           N'SKU-BOOK-001', 1, 'a1111111-1111-1111-1111-111111111102', SYSUTCDATETIME(), NULL),
    ('b3333333-3333-3333-3333-333333333305', N'Cafeteira Espresso',      N'15 bar, depósito 1.5L',              N'SKU-HOME-001', 1, 'a1111111-1111-1111-1111-111111111103', SYSUTCDATETIME(), NULL),
    ('b3333333-3333-3333-3333-333333333306', N'Rato Legado',             N'Produto descontinuado (p/ demo)',    N'SKU-OLD-001',  0, 'a1111111-1111-1111-1111-111111111101', SYSUTCDATETIME(), NULL);
GO

/* --------------------------------------------------------------------------
   Prices (histórico; o preço "corrente" é o ativo com ValidFromUtc mais recente)
   -------------------------------------------------------------------------- */
INSERT INTO [dbo].[Prices]
    ([Id], [ProductId], [Amount], [Currency], [ValidFromUtc], [IsActive], [CreatedAtUtc], [UpdatedAtUtc])
VALUES
    -- Auscultadores: preço antigo (inativo) e preço corrente
    ('c4444444-4444-4444-4444-444444444401', 'b3333333-3333-3333-3333-333333333301', 79.90,  'EUR', '2025-06-01T00:00:00', 0, SYSUTCDATETIME(), NULL),
    ('c4444444-4444-4444-4444-444444444402', 'b3333333-3333-3333-3333-333333333301', 89.90,  'EUR', '2026-01-01T00:00:00', 1, SYSUTCDATETIME(), NULL),
    -- Smartphone X
    ('c4444444-4444-4444-4444-444444444403', 'b3333333-3333-3333-3333-333333333302', 699.00, 'EUR', '2026-01-01T00:00:00', 1, SYSUTCDATETIME(), NULL),
    -- Teclado Mecânico
    ('c4444444-4444-4444-4444-444444444404', 'b3333333-3333-3333-3333-333333333303', 129.90, 'EUR', '2026-01-01T00:00:00', 1, SYSUTCDATETIME(), NULL),
    -- Clean Architecture (livro)
    ('c4444444-4444-4444-4444-444444444405', 'b3333333-3333-3333-3333-333333333304', 39.90,  'EUR', '2026-01-01T00:00:00', 1, SYSUTCDATETIME(), NULL),
    -- Cafeteira Espresso
    ('c4444444-4444-4444-4444-444444444406', 'b3333333-3333-3333-3333-333333333305', 249.00, 'EUR', '2026-01-01T00:00:00', 1, SYSUTCDATETIME(), NULL),
    -- Rato Legado (produto inativo, preço inativo)
    ('c4444444-4444-4444-4444-444444444407', 'b3333333-3333-3333-3333-333333333306', 19.90,  'EUR', '2024-01-01T00:00:00', 0, SYSUTCDATETIME(), NULL);
GO

/* --------------------------------------------------------------------------
   Orders (pedidos) + OrderItems (linhas com snapshot do preço)
   TotalAmount = soma dos LineTotal; LineTotal = UnitPrice × Quantity
   -------------------------------------------------------------------------- */
INSERT INTO [dbo].[Orders]
    ([Id], [UserId], [TotalAmount], [Currency], [Status], [OrderDateUtc], [CreatedAtUtc], [UpdatedAtUtc])
VALUES
    -- Mario: 1x Auscultadores (89.90) + 2x Livro (79.80) = 169.70 — Pago
    ('55555555-5555-5555-5555-555555555501', '22222222-2222-2222-2222-222222222201', 169.70, 'EUR', 2, '2026-02-01T10:00:00', SYSUTCDATETIME(), NULL),
    -- Giulia: 1x Smartphone (699.00) — Pendente
    ('55555555-5555-5555-5555-555555555502', '22222222-2222-2222-2222-222222222202', 699.00, 'EUR', 1, '2026-03-05T14:30:00', SYSUTCDATETIME(), NULL),
    -- Luca: 2x Teclado (259.80) + 1x Cafeteira (249.00) = 508.80 — Enviado
    ('55555555-5555-5555-5555-555555555503', '22222222-2222-2222-2222-222222222203', 508.80, 'EUR', 3, '2026-03-10T09:15:00', SYSUTCDATETIME(), NULL);
GO

INSERT INTO [dbo].[OrderItems]
    ([Id], [OrderId], [ProductId], [Quantity], [UnitPrice], [Currency], [LineTotal], [CreatedAtUtc], [UpdatedAtUtc])
VALUES
    -- Pedido 1 (Mario)
    ('66666666-6666-6666-6666-666666666601', '55555555-5555-5555-5555-555555555501', 'b3333333-3333-3333-3333-333333333301', 1, 89.90,  'EUR', 89.90,  SYSUTCDATETIME(), NULL),
    ('66666666-6666-6666-6666-666666666602', '55555555-5555-5555-5555-555555555501', 'b3333333-3333-3333-3333-333333333304', 2, 39.90,  'EUR', 79.80,  SYSUTCDATETIME(), NULL),
    -- Pedido 2 (Giulia)
    ('66666666-6666-6666-6666-666666666603', '55555555-5555-5555-5555-555555555502', 'b3333333-3333-3333-3333-333333333302', 1, 699.00, 'EUR', 699.00, SYSUTCDATETIME(), NULL),
    -- Pedido 3 (Luca)
    ('66666666-6666-6666-6666-666666666604', '55555555-5555-5555-5555-555555555503', 'b3333333-3333-3333-3333-333333333303', 2, 129.90, 'EUR', 259.80, SYSUTCDATETIME(), NULL),
    ('66666666-6666-6666-6666-666666666605', '55555555-5555-5555-5555-555555555503', 'b3333333-3333-3333-3333-333333333305', 1, 249.00, 'EUR', 249.00, SYSUTCDATETIME(), NULL);
GO
