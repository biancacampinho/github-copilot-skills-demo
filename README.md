# SkillGhcDemo — E-Commerce Microservice Demo (.NET 8 · Clean Architecture · CQRS)

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

## 3. Connection string

Defined in `src/SkillGhcDemo.Api/appsettings.json` → `ConnectionStrings:DefaultConnection`:

```json
"Server=(localdb)\\MSSQLLocalDB;Database=SkillGhcDemoDb;Trusted_Connection=True;MultipleActiveResultSets=true;TrustServerCertificate=True"
```

Adjust `Server=` to match your instance. To avoid committing secrets, prefer overriding via **User Secrets** or an environment variable:

```bash
# Environment variable (Windows PowerShell)
$env:ConnectionStrings__DefaultConnection = "Server=...;Database=SkillGhcDemoDb;User Id=...;Password=...;TrustServerCertificate=True"
```

---

## 4. How to run

### Option A — EF Core Migrations (recommended)
The database is created/updated by the migrations. In the **Development** environment, the API applies migrations automatically on startup (`db.Database.Migrate()` in `Program.cs`).

```bash
# at the solution root
dotnet build

# apply migrations manually (optional — the API already does this in Development)
dotnet ef database update --project src/SkillGhcDemo.Infrastructure --startup-project src/SkillGhcDemo.Api

# run the API
dotnet run --project src/SkillGhcDemo.Api
```

Open the **Swagger UI** at: `https://localhost:<port>/swagger` (the port is printed in the console).

To load sample data, run `sql/seed.sql` against the created database (see Option B).

### Option B — SQL scripts (no migrations)
If you prefer to create everything with plain SQL:

```bash
sqlcmd -S "(localdb)\MSSQLLocalDB" -i sql/schema.sql
sqlcmd -S "(localdb)\MSSQLLocalDB" -i sql/seed.sql
```

> The scripts are idempotent (they drop/clean before recreating/inserting).

### Handy migration commands
```bash
# create a new migration
dotnet ef migrations add <Name> --project src/SkillGhcDemo.Infrastructure --startup-project src/SkillGhcDemo.Api --output-dir Persistence/Migrations

# generate a SQL script from the migrations
dotnet ef migrations script --project src/SkillGhcDemo.Infrastructure --startup-project src/SkillGhcDemo.Api
```

---

## 5. Endpoints

Base: `/api`. All return JSON and use the `Result<T>` envelope mapped to HTTP status codes.

### Users — full CRUD ✅
| Method | Route | Description |
|--------|-------|-------------|
| GET    | `/api/users?search=&onlyActive=` | List users (optional filter) |
| GET    | `/api/users/{id}` | Get by id |
| POST   | `/api/users` | Create (validates unique email) |
| PUT    | `/api/users/{id}` | Update |
| DELETE | `/api/users/{id}` | Delete (blocked if the user has orders → `409`) |

### Categories — full CRUD ✅
| Method | Route | Description |
|--------|-------|-------------|
| GET    | `/api/categories?onlyActive=` | List categories |
| GET    | `/api/categories/{id}` | Get by id |
| POST   | `/api/categories` | Create (validates unique name) |
| PUT    | `/api/categories/{id}` | Update |
| DELETE | `/api/categories/{id}` | Delete (blocked if it has products → `409`) |

### Products — full CRUD ✅
| Method | Route | Description |
|--------|-------|-------------|
| GET    | `/api/products?search=&categoryId=&onlyActive=` | List products (includes current price) |
| GET    | `/api/products/{id}` | Get by id (includes current price) |
| POST   | `/api/products` | Create (validates category existence and unique SKU) |
| PUT    | `/api/products/{id}` | Update |
| DELETE | `/api/products/{id}` | Delete (blocked if present in orders → `409`) |

### Prices — full CRUD ✅
| Method | Route | Description |
|--------|-------|-------------|
| GET    | `/api/prices?productId=&onlyActive=` | List prices (history) |
| GET    | `/api/prices/{id}` | Get by id |
| POST   | `/api/prices` | Create (validates product existence) |
| PUT    | `/api/prices/{id}` | Update |
| DELETE | `/api/prices/{id}` | Delete |

### Orders
| Method | Route | Description |
|--------|-------|-------------|
| GET    | `/api/orders/{id}` | Get by id, including the lines ✅ |
| POST   | `/api/orders` | **★ RESERVED FOR MANUAL IMPLEMENTATION — see §7** |

---

## 6. Test organization (a particularity of this solution)

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

Run the tests:
```bash
dotnet test
```

---

## 7. ★ Endpoint reserved for manual implementation

**`POST /api/orders`** → `CreateOrderCommandHandler`
(command in `src/SkillGhcDemo.Application/Commands/Orders/`, handler in `src/SkillGhcDemo.Application/Handlers/Commands/Orders/`)

The **handler is deliberately left as a stub** (`throw new NotImplementedException(...)`), awaiting manual implementation with your custom skill. **Already in place**: the `Command` (with multiple `CreateOrderItem`s), the `Validator` (shape validation), the endpoint in `OrdersController`, and the read query `GET /api/orders/{id}`.

**Why this endpoint was chosen:**
- It is the **richest business-logic use case** and the only one that crosses several aggregates (`User` + `Product` + `Price` + `Order` + `OrderItem`), unlike the trivial CRUD of the others.
- It exercises **database-dependent validations** (which don't fit in a shape-level FluentValidator), **current-price resolution + snapshot** into the `OrderItem`, **total computation**, and **error mapping** to `Result<T>` — i.e., high learning value.

**Suggested checklist** (also documented in the handler header):
1. Does the User exist? → otherwise `Result.NotFound`
2. For each item: does the Product exist and is it active? → otherwise `Result.NotFound`/`Conflict`
3. Resolve the product's **current price** (active Price with the most recent `ValidFromUtc`) → otherwise `Result.Conflict`
4. Create each `OrderItem` with `UnitPrice` = price snapshot, `LineTotal` = `UnitPrice × Quantity`
5. Compute `Order.TotalAmount` = sum of `LineTotal`, `Status = Pending`, persist, and return `Result<Guid>.Success(id)`
6. Log via `_logger.LogInformation(...)`

> Note: because it's a stub, calling `POST /api/orders` today throws `NotImplementedException` (the `ExceptionHandlingMiddleware` responds `500`). This is expected until it's implemented. The rest of the solution compiles and all tests pass.

---

## 8. Open questions / decisions I made on my own

Decisions I took without confirmation during the **migration to e-commerce** — review and adjust as needed:

1. **Reserved endpoint = `POST /api/orders` (Order creation).** As suggested, since it's the richest business-logic use case. The rest of the `Order` CRUD (update/delete/list) was **not** created — only `GET /api/orders/{id}` and the reserved `POST`; I can add more if needed.
2. **`Price` was re-linked to `Product`** (it used to be linked to `Utente` via `Subscription`). It became a **history** (1:N): I removed `Name`/`Description`/`BillingPeriod` and added `ProductId` and `ValidFromUtc`. The "current price" is the most recent active one (there's no explicit end date). If you prefer an explicit `ValidToUtc`, I'll adjust.
3. **`OrderItem` stores a snapshot** of `UnitPrice` + `Currency` + `LineTotal` (no FK to `Price`), so price changes don't affect past orders. Because of that, deleting a `Price` is safe and is **not** blocked.
4. **`Subscription`/`Utente` entities and the `BillingPeriod`/`SubscriptionStatus` enums were removed.** `Utente` was replaced by `User` (English naming, aligned with the new domain). I'll switch to another language if you prefer.
5. **`OrderStatus` redefined** for e-commerce: `Pending, Paid, Shipped, Delivered, Cancelled` (previously: `Pending, Paid, Failed, Refunded`).
6. **Uniqueness rules:** `User.Email`, `Category.Name`, and `Product.Sku` are unique (index + check in the handler). The `SKU` is required and provided by the client (not auto-generated).
7. **Deletion blocked (`409`) to preserve integrity:** a `User` with orders, a `Category` with products, and a `Product` present in orders cannot be removed (instead of cascade/soft-delete). DB FKs: `Category→Product` and `Product→OrderItem` use `Restrict`; `User→Order`, `Order→OrderItem`, and `Product→Price` use `Cascade`.
8. **Database = SQL Server (LocalDB).** Kept from the original project. If you prefer PostgreSQL/SQLite, I'll switch the provider in `Infrastructure/DependencyInjection.cs` and regenerate migrations/scripts.
9. **Custom `Result<T>` envelope, manual `ToDto()` mapping, automatic migrations in Development, `Currency` as `nchar(3)` ISO-4217, plain JSON in the middleware (no `ProblemDetails`)** — all original project decisions were **kept** unchanged.
10. **`ProductDto` includes `CurrentPrice`/`Currency`** derived from the price history (read-time convenience). The product read handlers do `Include(p => p.Prices)` for this.
11. **Handler tests use EF Core InMemory.** Fast and infra-free, but it **does not validate real constraints** (e.g., unique indexes — which is why the handlers check in code). For full fidelity, we could use SQLite in-memory or Testcontainers.
12. **Package versions** kept: EF 8.0.7, MediatR 12.4.1, FluentValidation 11.9.2, Swashbuckle 6.4.0.

---

## 9. Current status

- ✅ Solution build: **success** (0 warnings, 0 errors)
- ✅ Tests: **115/115 passing**
- ✅ `InitialCreate` migration (e-commerce) generated
- ✅ `schema.sql` / `seed.sql` updated with e-commerce sample data
- ⏳ `POST /api/orders` (handler): **stub awaiting manual implementation**
