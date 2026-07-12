/* ============================================================================
   MicroDemo — seed.sql
   Dados de exemplo realistas. Execute APÓS schema.sql.
   Usa GUIDs fixos para permitir reexecução determinística e referências entre
   tabelas. É idempotente: apaga os registros conhecidos antes de reinserir.

   Enums (INT):
     BillingPeriod:      1=Monthly, 2=Quarterly, 3=Yearly
     SubscriptionStatus: 1=Active,  2=Cancelled, 3=Expired
     OrderStatus:        1=Pending, 2=Paid, 3=Failed, 4=Refunded
   ============================================================================ */

USE [MicroDemoDb];
GO

/* Limpeza (ordem inversa das FKs) ------------------------------------------ */
DELETE FROM [dbo].[Orders];
DELETE FROM [dbo].[Subscriptions];
DELETE FROM [dbo].[Utenti];
DELETE FROM [dbo].[Prices];
GO

/* --------------------------------------------------------------------------
   Prices
   -------------------------------------------------------------------------- */
INSERT INTO [dbo].[Prices]
    ([Id], [Name], [Description], [Amount], [Currency], [BillingPeriod], [IsActive], [CreatedAtUtc], [UpdatedAtUtc])
VALUES
    ('11111111-1111-1111-1111-111111111101', N'Free',         N'Plano gratuito com limites básicos',     0.00,  'EUR', 1, 1, SYSUTCDATETIME(), NULL),
    ('11111111-1111-1111-1111-111111111102', N'Starter',      N'Ideal para começar — cobrança mensal',   9.90,  'EUR', 1, 1, SYSUTCDATETIME(), NULL),
    ('11111111-1111-1111-1111-111111111103', N'Pro',          N'Plano profissional mensal',              29.90, 'EUR', 1, 1, SYSUTCDATETIME(), NULL),
    ('11111111-1111-1111-1111-111111111104', N'Pro Annual',   N'Plano profissional com cobrança anual',  299.00,'EUR', 3, 1, SYSUTCDATETIME(), NULL),
    ('11111111-1111-1111-1111-111111111105', N'Enterprise',   N'Plano corporativo com SLA dedicado',     990.00,'EUR', 3, 1, SYSUTCDATETIME(), NULL),
    ('11111111-1111-1111-1111-111111111106', N'Legacy Basic', N'Plano descontinuado (mantido p/ demo)',  4.90,  'EUR', 1, 0, SYSUTCDATETIME(), NULL);
GO

/* --------------------------------------------------------------------------
   Utenti (DefaultPriceId aponta para um dos planos acima)
   -------------------------------------------------------------------------- */
INSERT INTO [dbo].[Utenti]
    ([Id], [FullName], [Email], [PhoneNumber], [IsActive], [DefaultPriceId], [CreatedAtUtc], [UpdatedAtUtc])
VALUES
    ('22222222-2222-2222-2222-222222222201', N'Mario Rossi',    N'mario.rossi@example.com',    N'+39 333 1112221', 1, '11111111-1111-1111-1111-111111111103', SYSUTCDATETIME(), NULL),
    ('22222222-2222-2222-2222-222222222202', N'Giulia Bianchi', N'giulia.bianchi@example.com', N'+39 333 2223332', 1, '11111111-1111-1111-1111-111111111102', SYSUTCDATETIME(), NULL),
    ('22222222-2222-2222-2222-222222222203', N'Luca Verdi',     N'luca.verdi@example.com',     N'+39 333 3334443', 1, '11111111-1111-1111-1111-111111111104', SYSUTCDATETIME(), NULL),
    ('22222222-2222-2222-2222-222222222204', N'Sofia Esposito', N'sofia.esposito@example.com', N'+39 333 4445554', 1, '11111111-1111-1111-1111-111111111105', SYSUTCDATETIME(), NULL),
    ('22222222-2222-2222-2222-222222222205', N'Marco Ferrari',  N'marco.ferrari@example.com',  N'+39 333 5556665', 1, '11111111-1111-1111-1111-111111111101', SYSUTCDATETIME(), NULL);
GO

/* --------------------------------------------------------------------------
   Subscriptions (Utente ↔ Price ao longo do tempo)
   -------------------------------------------------------------------------- */
INSERT INTO [dbo].[Subscriptions]
    ([Id], [UtenteId], [PriceId], [StartDateUtc], [EndDateUtc], [Status], [CreatedAtUtc], [UpdatedAtUtc])
VALUES
    -- Mario: Pro ativa
    ('33333333-3333-3333-3333-333333333301', '22222222-2222-2222-2222-222222222201', '11111111-1111-1111-1111-111111111103', '2026-01-01T00:00:00', NULL,                  1, SYSUTCDATETIME(), NULL),
    -- Giulia: Starter ativa
    ('33333333-3333-3333-3333-333333333302', '22222222-2222-2222-2222-222222222202', '11111111-1111-1111-1111-111111111102', '2026-02-15T00:00:00', NULL,                  1, SYSUTCDATETIME(), NULL),
    -- Luca: Pro Annual ativa
    ('33333333-3333-3333-3333-333333333303', '22222222-2222-2222-2222-222222222203', '11111111-1111-1111-1111-111111111104', '2026-01-10T00:00:00', '2027-01-10T00:00:00', 1, SYSUTCDATETIME(), NULL),
    -- Sofia: Enterprise ativa
    ('33333333-3333-3333-3333-333333333304', '22222222-2222-2222-2222-222222222204', '11111111-1111-1111-1111-111111111105', '2026-03-01T00:00:00', NULL,                  1, SYSUTCDATETIME(), NULL),
    -- Mario: Starter anterior, já cancelada (histórico)
    ('33333333-3333-3333-3333-333333333305', '22222222-2222-2222-2222-222222222201', '11111111-1111-1111-1111-111111111102', '2025-06-01T00:00:00', '2025-12-31T00:00:00', 2, SYSUTCDATETIME(), NULL);
GO

/* --------------------------------------------------------------------------
   Orders (pedidos/cobranças ligados a utenti e, opcionalmente, assinaturas)
   -------------------------------------------------------------------------- */
INSERT INTO [dbo].[Orders]
    ([Id], [UtenteId], [SubscriptionId], [TotalAmount], [Currency], [Status], [OrderDateUtc], [CreatedAtUtc], [UpdatedAtUtc])
VALUES
    ('44444444-4444-4444-4444-444444444401', '22222222-2222-2222-2222-222222222201', '33333333-3333-3333-3333-333333333301', 29.90,  'EUR', 2, '2026-01-01T00:05:00', SYSUTCDATETIME(), NULL),
    ('44444444-4444-4444-4444-444444444402', '22222222-2222-2222-2222-222222222202', '33333333-3333-3333-3333-333333333302', 9.90,   'EUR', 2, '2026-02-15T00:05:00', SYSUTCDATETIME(), NULL),
    ('44444444-4444-4444-4444-444444444403', '22222222-2222-2222-2222-222222222203', '33333333-3333-3333-3333-333333333303', 299.00, 'EUR', 2, '2026-01-10T00:05:00', SYSUTCDATETIME(), NULL),
    ('44444444-4444-4444-4444-444444444404', '22222222-2222-2222-2222-222222222204', '33333333-3333-3333-3333-333333333304', 990.00, 'EUR', 1, '2026-03-01T00:05:00', SYSUTCDATETIME(), NULL),
    ('44444444-4444-4444-4444-444444444405', '22222222-2222-2222-2222-222222222201', NULL,                                     29.90,  'EUR', 3, '2026-02-01T00:05:00', SYSUTCDATETIME(), NULL);
GO
