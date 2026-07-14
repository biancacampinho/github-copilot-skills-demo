/* ============================================================================
   SkillGhcDemo — seed.sql
   Dati di esempio realistici di E-COMMERCE. Eseguire DOPO schema.sql.
   Usa GUID fissi per permettere una riesecuzione deterministica e i riferimenti
   tra le tabelle. È idempotente: elimina i record noti prima di reinserirli.

   Enum (INT):
     OrderStatus: 1=Pending, 2=Paid, 3=Shipped, 4=Delivered, 5=Cancelled
   ============================================================================ */

USE [SkillGhcDemoDb];
GO

/* Pulizia (ordine inverso delle FK) ---------------------------------------- */
DELETE FROM [dbo].[OrderItems];
DELETE FROM [dbo].[Prices];
DELETE FROM [dbo].[Orders];
DELETE FROM [dbo].[Products];
DELETE FROM [dbo].[Users];
DELETE FROM [dbo].[Categories];
GO

/* --------------------------------------------------------------------------
   Categorie
   -------------------------------------------------------------------------- */
INSERT INTO [dbo].[Categories]
    ([Id], [Name], [Description], [IsActive], [CreatedAtUtc], [UpdatedAtUtc])
VALUES
    ('a1111111-1111-1111-1111-111111111101', N'Elettronica',    N'Dispositivi e accessori elettronici',      1, SYSUTCDATETIME(), NULL),
    ('a1111111-1111-1111-1111-111111111102', N'Libri',          N'Libri cartacei ed e-book',                 1, SYSUTCDATETIME(), NULL),
    ('a1111111-1111-1111-1111-111111111103', N'Casa & Cucina',  N'Utensili ed elettrodomestici',             1, SYSUTCDATETIME(), NULL),
    ('a1111111-1111-1111-1111-111111111104', N'Fuori produzione', N'Categoria mantenuta solo per lo storico', 0, SYSUTCDATETIME(), NULL);
GO

/* --------------------------------------------------------------------------
   Utenti (clienti)
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
   Prodotti (CategoryId punta a una delle categorie sopra)
   -------------------------------------------------------------------------- */
INSERT INTO [dbo].[Products]
    ([Id], [Name], [Description], [Sku], [IsActive], [CategoryId], [CreatedAtUtc], [UpdatedAtUtc])
VALUES
    ('b3333333-3333-3333-3333-333333333301', N'Cuffie Bluetooth',    N'Over-ear con cancellazione del rumore', N'SKU-ELEC-001', 1, 'a1111111-1111-1111-1111-111111111101', SYSUTCDATETIME(), NULL),
    ('b3333333-3333-3333-3333-333333333302', N'Smartphone X',        N'6.5", 128GB, dual SIM',                 N'SKU-ELEC-002', 1, 'a1111111-1111-1111-1111-111111111101', SYSUTCDATETIME(), NULL),
    ('b3333333-3333-3333-3333-333333333303', N'Tastiera Meccanica',  N'Switch blu, layout IT',                 N'SKU-ELEC-003', 1, 'a1111111-1111-1111-1111-111111111101', SYSUTCDATETIME(), NULL),
    ('b3333333-3333-3333-3333-333333333304', N'Clean Architecture',  N'Robert C. Martin (libro)',              N'SKU-BOOK-001', 1, 'a1111111-1111-1111-1111-111111111102', SYSUTCDATETIME(), NULL),
    ('b3333333-3333-3333-3333-333333333305', N'Macchina per Espresso', N'15 bar, serbatoio 1.5L',              N'SKU-HOME-001', 1, 'a1111111-1111-1111-1111-111111111103', SYSUTCDATETIME(), NULL),
    ('b3333333-3333-3333-3333-333333333306', N'Mouse Legacy',        N'Prodotto fuori produzione (per demo)',  N'SKU-OLD-001',  0, 'a1111111-1111-1111-1111-111111111101', SYSUTCDATETIME(), NULL);
GO

/* --------------------------------------------------------------------------
   Prezzi (storico; il prezzo "corrente" è quello attivo con ValidFromUtc più recente)
   -------------------------------------------------------------------------- */
INSERT INTO [dbo].[Prices]
    ([Id], [ProductId], [Amount], [Currency], [ValidFromUtc], [IsActive], [CreatedAtUtc], [UpdatedAtUtc])
VALUES
    -- Cuffie: prezzo vecchio (inattivo) e prezzo corrente
    ('c4444444-4444-4444-4444-444444444401', 'b3333333-3333-3333-3333-333333333301', 79.90,  'EUR', '2025-06-01T00:00:00', 0, SYSUTCDATETIME(), NULL),
    ('c4444444-4444-4444-4444-444444444402', 'b3333333-3333-3333-3333-333333333301', 89.90,  'EUR', '2026-01-01T00:00:00', 1, SYSUTCDATETIME(), NULL),
    -- Smartphone X
    ('c4444444-4444-4444-4444-444444444403', 'b3333333-3333-3333-3333-333333333302', 699.00, 'EUR', '2026-01-01T00:00:00', 1, SYSUTCDATETIME(), NULL),
    -- Tastiera Meccanica
    ('c4444444-4444-4444-4444-444444444404', 'b3333333-3333-3333-3333-333333333303', 129.90, 'EUR', '2026-01-01T00:00:00', 1, SYSUTCDATETIME(), NULL),
    -- Clean Architecture (libro)
    ('c4444444-4444-4444-4444-444444444405', 'b3333333-3333-3333-3333-333333333304', 39.90,  'EUR', '2026-01-01T00:00:00', 1, SYSUTCDATETIME(), NULL),
    -- Macchina per Espresso
    ('c4444444-4444-4444-4444-444444444406', 'b3333333-3333-3333-3333-333333333305', 249.00, 'EUR', '2026-01-01T00:00:00', 1, SYSUTCDATETIME(), NULL),
    -- Mouse Legacy (prodotto inattivo, prezzo inattivo)
    ('c4444444-4444-4444-4444-444444444407', 'b3333333-3333-3333-3333-333333333306', 19.90,  'EUR', '2024-01-01T00:00:00', 0, SYSUTCDATETIME(), NULL);
GO

/* --------------------------------------------------------------------------
   Ordini (Orders) + Righe (OrderItems, con snapshot del prezzo)
   TotalAmount = somma dei LineTotal; LineTotal = UnitPrice × Quantity
   -------------------------------------------------------------------------- */
INSERT INTO [dbo].[Orders]
    ([Id], [UserId], [TotalAmount], [Currency], [Status], [OrderDateUtc], [CreatedAtUtc], [UpdatedAtUtc])
VALUES
    -- Mario: 1x Cuffie (89.90) + 2x Libro (79.80) = 169.70 — Pagato
    ('55555555-5555-5555-5555-555555555501', '22222222-2222-2222-2222-222222222201', 169.70, 'EUR', 2, '2026-02-01T10:00:00', SYSUTCDATETIME(), NULL),
    -- Giulia: 1x Smartphone (699.00) — In attesa
    ('55555555-5555-5555-5555-555555555502', '22222222-2222-2222-2222-222222222202', 699.00, 'EUR', 1, '2026-03-05T14:30:00', SYSUTCDATETIME(), NULL),
    -- Luca: 2x Tastiera (259.80) + 1x Macchina per Espresso (249.00) = 508.80 — Spedito
    ('55555555-5555-5555-5555-555555555503', '22222222-2222-2222-2222-222222222203', 508.80, 'EUR', 3, '2026-03-10T09:15:00', SYSUTCDATETIME(), NULL);
GO

INSERT INTO [dbo].[OrderItems]
    ([Id], [OrderId], [ProductId], [Quantity], [UnitPrice], [Currency], [LineTotal], [CreatedAtUtc], [UpdatedAtUtc])
VALUES
    -- Ordine 1 (Mario)
    ('66666666-6666-6666-6666-666666666601', '55555555-5555-5555-5555-555555555501', 'b3333333-3333-3333-3333-333333333301', 1, 89.90,  'EUR', 89.90,  SYSUTCDATETIME(), NULL),
    ('66666666-6666-6666-6666-666666666602', '55555555-5555-5555-5555-555555555501', 'b3333333-3333-3333-3333-333333333304', 2, 39.90,  'EUR', 79.80,  SYSUTCDATETIME(), NULL),
    -- Ordine 2 (Giulia)
    ('66666666-6666-6666-6666-666666666603', '55555555-5555-5555-5555-555555555502', 'b3333333-3333-3333-3333-333333333302', 1, 699.00, 'EUR', 699.00, SYSUTCDATETIME(), NULL),
    -- Ordine 3 (Luca)
    ('66666666-6666-6666-6666-666666666604', '55555555-5555-5555-5555-555555555503', 'b3333333-3333-3333-3333-333333333303', 2, 129.90, 'EUR', 259.80, SYSUTCDATETIME(), NULL),
    ('66666666-6666-6666-6666-666666666605', '55555555-5555-5555-5555-555555555503', 'b3333333-3333-3333-3333-333333333305', 1, 249.00, 'EUR', 249.00, SYSUTCDATETIME(), NULL);
GO
