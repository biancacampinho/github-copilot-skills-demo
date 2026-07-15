# Workshop Sviluppo AI - Sessione 8 - Boost Productivity with GitHub Copilot: Custom Skills in Action
In this session we'll go beyond the basics of GitHub Copilot to explore how Custom Skills can turn it into a tailored assistant for your development workflow.
Theory:

-What a skill is
-Why build one
-How it's structured
-Best practices

Demo:
On a real solution, we'll showcase 3 custom skills that analyze a microservice, generate an API, and create the related unit tests.
Who it's for: developers already using GitHub Copilot who want to take their productivity to the next level by leveraging AI in a targeted, non-generic way.
Format: hands-on session combining theory with live demos directly in the GitHub Copilot CLI.

## Custom Skills

Le skill utilizzate in questo repo sono state create a partire da un template riutilizzabile disponibile in [`templates/skills-template`](./templates/skills-template), pensato per standardizzare la struttura e velocizzare la creazione di nuove skill personalizzate per GitHub Copilot.

Per la demo sono state sviluppate 3 skill, pensate per lavorare in sequenza su un microservizio:

###  `my-research`
Analizza il codebase del microservizio e produce un documento di ricerca che ne descrive l'architettura, le convenzioni utilizzate e i pattern principali. Il documento generato funge da contesto per le skill successive.

###  `my-unit-tests`
Aggiunge unit test seguendo le convenzioni di testing già presenti nel repo. Misura la coverage del codice e **non modifica mai i file in `src/`**, garantendo che venga toccato solo il livello di test.

###  `my-api-implementation`
Crea un nuovo endpoint end-to-end, seguendo i pattern architetturali descritti nel documento di contesto generato da `my-research`. Il risultato è coerente con le convenzioni già in uso nel microservizio.

---

**Come funzionano insieme:** `my-research` genera il contesto → `my-api-implementation` lo usa per creare il nuovo endpoint rispettando i pattern esistenti → `my-unit-tests` copre il nuovo codice con test adeguati, senza intaccare la logica applicativa.

## Solution to apply the skill -> SkillGhcDemo: E-Commerce Microservice Demo (.NET 8 · Clean Architecture · CQRS)

Demo solution of a microservice in **.NET 8** following **Clean Architecture** with **light DDD**, **CQRS (MediatR)**, **EF Core (Code First + Migrations)**, **FluentValidation**, and **Swagger**.

Domain: **E-COMMERCE** — management of **Users** (customers), **Categories**, **Products**, **Prices** (per-product price history), and **Orders** with their **OrderItems** (lines with a price snapshot taken at purchase time).

> This solution was **migrated** from a previous *Subscriptions* domain to *E-Commerce*, keeping the architecture and the patterns (Clean Architecture, CQRS/MediatR, EF Core, `Result<T>`, FluentValidation, Swagger) untouched.

---

## 1. Architecture and layers

```
SkillGhcDemo.sln
├── src/
│   ├── SkillGhcDemo.Domain          → Entities, enums, invariant rules. No external dependencies.
│   ├── SkillGhcDemo.Application      → Use cases (CQRS): Commands/Queries, Handlers,
│   │                                Validators, DTOs, Result<T>, MediatR behaviors.
│   ├── SkillGhcDemo.Infrastructure   → EF Core: AppDbContext, configurations, migrations, DI.
│   └── SkillGhcDemo.Api              → ASP.NET Core Web API: controllers, middleware, Swagger.
├── tests/
│   └── SkillGhcDemo.UnitTests        → xUnit + FluentAssertions + Moq (custom organization, see §6).
└── sql/
    ├── schema.sql                 → CREATE DATABASE + CREATE TABLE for the whole domain.
    └── seed.sql                   → INSERTs with realistic e-commerce sample data.
```

**Dependency rule** (always points inward):
`Api → Application → Domain` and `Api → Infrastructure → Application → Domain`.
The Application layer does not know about Infrastructure — it depends only on the `IAppDbContext` abstraction, implemented by Infrastructure's `AppDbContext`.

### Applied patterns
- **CQRS with MediatR**: each operation is a `Command` or `Query` with its own `Handler`.
- **Pipeline behaviors**: `LoggingBehavior` (request/response logging) and `ValidationBehavior` (runs the `FluentValidation` validators before the handler).
- **Standardized Result / envelope**: `Result` and `Result<T>` (`Common/Models/Result.cs`) avoid exceptions for expected business flows; `ApiControllerBase` maps `ResultErrorType` to the HTTP status (`404`, `409`, `400`...).
- **DTOs + manual mapping** (`ToDto()` extensions), no AutoMapper, to keep the demo lean.
- **Logging** via native `ILogger<T>`.

### Domain entities
| Entity | Description |
|--------|-------------|
| `User` | Customer/buyer. Has a history of orders. |
| `Category` | Product category (e.g. Electronics, Books). |
| `Product` | Sellable product. Belongs to a `Category` and has a price history. |
| `Price` | Price of a `Product` (1:N history). The current one = active price with the most recent `ValidFromUtc`. |
| `Order` | A `User`'s order. Aggregates `OrderItem`s and keeps the computed total. |
| `OrderItem` | Order line: links `Product` + `Order`, with quantity and a **snapshot** of the unit price. |

### Relationships
- `User` **1:N** `Order`
- `Order` **1:N** `OrderItem`
- `OrderItem` **N:1** `Product` (with a price snapshot taken at purchase time)
- `Product` **N:1** `Category`
- `Product` **1:N** `Price` (price history)

---

## 2. Prerequisites

- [.NET 8 SDK](https://dotnet.microsoft.com/download)
- **SQL Server** — LocalDB (default on Windows/Visual Studio) or a SQL Server/Express instance.
- EF Core CLI tool (for manual migrations):
  ```bash
  dotnet tool install --global dotnet-ef --version 8.0.7
  ```

---
## 3. Test organization (a particularity of this solution)

Tests live in `tests/SkillGhcDemo.UnitTests`, organized into folders **by ARTIFACT TYPE** (no per-entity subfolders):

```
tests/SkillGhcDemo.UnitTests/
├── Commands/            → one <Command>Data class per command (test-data)
├── Queries/             → one <Query>Data class per query (test-data)
├── Handlers/
│   ├── Commands/        → command-handler tests
│   └── Queries/         → query-handler tests
├── Validators/          → FluentValidation TestValidate tests
├── Controllers/         → controller tests with a mocked ISender
└── Common/              → TestDbContextFactory, TestLogger, ResultAssertions
```

| Folder | Contains |
|--------|----------|
| `Commands/` | One `<Command>Data` class per command (e.g. `CreateCategoryCommandData`) exposing reusable command instances — **valid, invalid, and edge-case** variants — via factory methods (`Valid()`, `WithEmptyName()`, `WithTooLongName()`...). |
| `Queries/` | One `<Query>Data` class per query, same idea (`All()`, `OnlyActive()`, `ForId()`, `Unknown()`...). |
| `Handlers/Commands/` · `Handlers/Queries/` | The MediatR handler tests, split into command and query handlers. Use EF Core InMemory (`TestDbContextFactory`) + `TestLogger` for `ILogger`. |
| `Validators/` | All validator tests, using FluentValidation's `TestValidate`. |
| `Controllers/` | Controller tests with a **mocked `ISender`** (via `IServiceProvider`), asserting the `IActionResult` mapping (`OkObjectResult`, `NotFoundObjectResult`, `CreatedAtActionResult`, `ConflictObjectResult`, `NoContentResult`...). |
| `Common/` | Shared helpers: `TestDbContextFactory` (InMemory context), `TestLogger` (no-op `ILogger<T>`), `ResultAssertions` (`ShouldBeSuccess` / `ShouldBeFailureOfType`). |

**Why the `Commands/`/`Queries/` data classes?** Centralizing the command/query instances there lets the handler, validator, and controller tests **reuse** them without duplicating object assembly — there is a single place to tweak the "default valid data" and its variants.

---
