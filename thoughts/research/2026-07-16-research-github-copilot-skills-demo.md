---
date: 2026-07-16T17:34:50+02:00
researcher: Bianca Rangel Campinho de Andrade
git_commit: 0e06a7eb75d271c678a8666c541e9ed635f4aa5f
branch: feature/new-api-orders
repository: github-copilot-skills-demo
topic: "Orders feature architecture: entities, existing queries/handlers/controller, DTOs, and 'list by filter' query patterns (e.g. GetProductsQuery), for implementing 'retrieve orders by product id'"
tags: [research, codebase, orders, products, users, cqrs, mediatr]
status: complete
last_updated: 2026-07-16
last_updated_by: Bianca Rangel Campinho de Andrade
---

# Research: Orders feature architecture and "list by filter" query patterns

## Research Question
How is the Orders feature (and the related Products/Users features) implemented in this codebase — entities, existing queries/handlers/controller, DTOs — and how do existing "list by filter" queries (e.g. `GetProductsQuery`'s `CategoryId` filter) work, in order to implement a new endpoint that retrieves orders by product id?

## Summary
The solution follows a CQRS/MediatR layering with four projects: `SkillGhcDemo.Domain` (entities), `SkillGhcDemo.Application` (Queries/Commands, Handlers, Dtos, `IAppDbContext` abstraction), `SkillGhcDemo.Infrastructure` (EF Core `AppDbContext`, configurations, migrations), and `SkillGhcDemo.Api` (thin controllers calling MediatR). Each feature (Categories, Orders, Prices, Products, Users) follows the same folder-per-feature layout for Queries/Commands/Handlers, a single DTO file per entity with a `ToDto()` mapping extension, and a controller inheriting `ApiControllerBase` that calls `Mediator.Send(...)` and converts the `Result<T>` envelope via `ToResponse(...)`.

Orders currently only exposes `GetOrderByIdQuery` (single order by id, no list endpoint) and `CreateOrderCommand`. Products exposes a "list by filter" pattern via `GetProductsQuery(string? Search, Guid? CategoryId, bool OnlyActive)`, which is the closest analogous template for filtering a list by a foreign key. An `Order` has a `UserId`/`User` navigation and a collection of `OrderItem`s; each `OrderItem` has a `ProductId`/`Product` navigation and a snapshotted `UnitPrice`/`Currency`. There is no existing DTO that already returns "orders with full product and user information" — `OrderDto` currently only exposes `UserId` (not the full `UserDto`) and `OrderItemDto` only exposes `ProductId` (not the full `ProductDto`).

## Detailed Findings

### Solution/project layout
- `src/SkillGhcDemo.Domain/Entities/` — `Order.cs`, `OrderItem.cs`, `Product.cs`, `User.cs`, `Category.cs`, `Price.cs`; `Common/BaseEntity.cs` (likely `Id`, audit fields); `Enums/OrderStatus.cs`.
- `src/SkillGhcDemo.Application/` — `Queries/<Feature>/`, `Commands/<Feature>/`, `Handlers/Queries/<Feature>/`, `Handlers/Commands/<Feature>/`, `Dtos/<X>Dto.cs`, `Common/Models/Result.cs`, `Common/Interfaces/IAppDbContext.cs`, `Common/Behaviors/` (Logging/Validation MediatR pipeline behaviors), `DependencyInjection.cs` (assembly scanning for MediatR handlers/validators).
- `src/SkillGhcDemo.Infrastructure/Persistence/` — `AppDbContext.cs` (implements `IAppDbContext`), `Configurations/*Configuration.cs` (EF Core Fluent API per entity), `Migrations/`.
- `src/SkillGhcDemo.Api/Controllers/` — one controller per feature (`OrdersController`, `ProductsController`, `UsersController`, `CategoriesController`, `PricesController`), all inheriting `Api/Common/ApiControllerBase.cs`.
- `tests/SkillGhcDemo.UnitTests/` — mirrors `Handlers/Queries`, `Handlers/Commands`, `Queries` (test data), `Commands` (test data), `Validators`, `Controllers`, and `Common/` (test helpers: `ResultAssertions.cs`, `TestDbContextFactory.cs`, `TestLogger.cs`).

### Domain entities
- **`Order`** (`src/SkillGhcDemo.Domain/Entities/Order.cs:10-26`): `Id` (via `BaseEntity`), `UserId`/`User` navigation, `TotalAmount`, `Currency` (default `"EUR"`), `Status` (`OrderStatus`, default `Pending`), `OrderDateUtc`, and `Items` (`ICollection<OrderItem>`).
- **`OrderItem`** (`src/SkillGhcDemo.Domain/Entities/OrderItem.cs:10-27`): `OrderId`/`Order`, `ProductId`/`Product` navigation, `Quantity`, `UnitPrice` (price snapshot at purchase time), `Currency`, `LineTotal`.
- **`Product`** (`src/SkillGhcDemo.Domain/Entities/Product.cs:9-27`): `Name`, `Description`, `Sku`, `IsActive`, `CategoryId`/`Category`, `Prices` (`ICollection<Price>`, price history), `OrderItems` (`ICollection<OrderItem>`) — i.e. `Product` already has a navigation back to the `OrderItem`s it appears in.
- **`User`** (`src/SkillGhcDemo.Domain/Entities/User.cs:9-21`): `FullName`, `Email`, `PhoneNumber`, `IsActive`, `Orders` (`ICollection<Order>`).

### Existing Orders feature files
- **Query**: `src/SkillGhcDemo.Application/Queries/Orders/GetOrderByIdQuery.cs:7` — `public record GetOrderByIdQuery(Guid Id) : IRequest<Result<OrderDto>>;` (single-order lookup only; no list/filter query exists yet for Orders).
- **Query handler**: `src/SkillGhcDemo.Application/Handlers/Queries/Orders/GetOrderByIdQueryHandler.cs:10-27` — injects `IAppDbContext`, does `_db.Orders.AsNoTracking().Include(o => o.Items).FirstOrDefaultAsync(o => o.Id == request.Id, ct)`, returns `Result<OrderDto>.NotFound(...)` or `Result<OrderDto>.Success(order.ToDto())`.
- **Command**: `Commands/Orders/CreateOrderCommand.cs` + `CreateOrderCommandValidator.cs` (not shown in full above, but present).
- **Command handler**: `Handlers/Commands/Orders/CreateOrderCommandHandler.cs`.
- **Controller**: `src/SkillGhcDemo.Api/Controllers/OrdersController.cs:9-34` — `OrdersController : ApiControllerBase`, with `[HttpGet("{id:guid}")] GetById(Guid id)` calling `GetOrderByIdQuery`, and `[HttpPost] Create(...)` calling `CreateOrderCommand` with the `CreatedAtAction` pattern. Route prefix is `api/[controller]` (from `ApiControllerBase`), i.e. `api/Orders`.

### "List by filter" query pattern (Products, the closest template)
- **Query**: `src/SkillGhcDemo.Application/Queries/Products/GetProductsQuery.cs:8-9` — `public record GetProductsQuery(string? Search = null, Guid? CategoryId = null, bool OnlyActive = false) : IRequest<Result<IReadOnlyList<ProductDto>>>;` — optional filters as default-valued positional parameters, returns `Result<IReadOnlyList<T>>`.
- **Handler**: `src/SkillGhcDemo.Application/Handlers/Queries/Products/GetProductsQueryHandler.cs:10-39` — builds an `IQueryable` from `_db.Products.AsNoTracking().Include(p => p.Prices).AsQueryable()`, conditionally applies `.Where(...)` per filter, materializes with `.OrderBy(...).ToListAsync(ct)`, maps each entity via `.ToDto()`, and wraps in `Result<IReadOnlyList<ProductDto>>.Success(dtos)`. No `NotFound` case for empty lists — an empty filter result still returns `Success` with an empty list.
- **Controller action**: `src/SkillGhcDemo.Api/Controllers/ProductsController.cs:12-18` — `[HttpGet] GetAll([FromQuery] string? search, [FromQuery] Guid? categoryId, [FromQuery] bool onlyActive)` → `Mediator.Send(new GetProductsQuery(search, categoryId, onlyActive))`. Filter params are passed via query string, mapped positionally into the query record.

### DTOs and mapping extensions
- **`OrderDto`** (`src/SkillGhcDemo.Application/Dtos/OrderDto.cs:7-53`): `Id`, `UserId` (Guid only, not full `UserDto`), `TotalAmount`, `Currency`, `Status`, `OrderDateUtc`, `CreatedAtUtc`, `Items` (`IReadOnlyList<OrderItemDto>`). `OrderItemDto`: `Id`, `ProductId` (Guid only, not full `ProductDto`), `Quantity`, `UnitPrice`, `Currency`, `LineTotal`. `OrderMappingExtensions.ToDto()` maps `Order`→`OrderDto` and `OrderItem`→`OrderItemDto` directly from entity fields (no join to `Product`/`User` DTOs).
- **`ProductDto`** (`src/SkillGhcDemo.Application/Dtos/ProductDto.cs:6-43`): `Id`, `Name`, `Description`, `Sku`, `IsActive`, `CategoryId`, `CreatedAtUtc`, `CurrentPrice`/`Currency` (computed from the most recent active `Price`). `ProductMappingExtensions.ToDto()` requires `product.Prices` to be loaded/included (selects most recent active price).
- **`UserDto`** (`src/SkillGhcDemo.Application/Dtos/UserDto.cs:6-27`): `Id`, `FullName`, `Email`, `PhoneNumber`, `IsActive`, `CreatedAtUtc`. Simple 1:1 mapping, no included navigations required.
- No existing DTO currently combines an order with its full `ProductDto`/`UserDto` (only ids are exposed today, per `OrderDto`/`OrderItemDto` above).

### `Result<T>` envelope and controller response mapping
- `src/SkillGhcDemo.Application/Common/Models/Result.cs:35-55` — `Result<T>` carries `Data`, with static factories `Success(data)`, `Failure(error, type)`, `NotFound(error)`, `Invalid(errors)`. `ResultErrorType` enum: `None`, `Failure`, `Validation`, `NotFound`, `Conflict`.
- `src/SkillGhcDemo.Api/Common/ApiControllerBase.cs:14-39` — `ApiControllerBase` is `[ApiController] [Route("api/[controller]")] [Produces("application/json")]`, exposes `Mediator` (lazy `ISender`), and `ToResponse<T>(Result<T>)`/`ToResponse(Result)` map `Succeeded` → `Ok(...)`/`NoContent()`, and failures via `MapError` → `NotFound`/`Conflict`/`BadRequest` (validation)/`BadRequest` (generic), based on `ErrorType`.

### `IAppDbContext` abstraction
- `src/SkillGhcDemo.Application/Common/Interfaces/IAppDbContext.cs:10-20` — exposes `DbSet<User> Users`, `DbSet<Category> Categories`, `DbSet<Product> Products`, `DbSet<Price> Prices`, `DbSet<Order> Orders`, `DbSet<OrderItem> OrderItems`, and `SaveChangesAsync`. Implemented by `Infrastructure/Persistence/AppDbContext.cs`. This is what all handlers inject (never the concrete `AppDbContext`), enabling in-memory/test doubles per `tests/SkillGhcDemo.UnitTests/Common/TestDbContextFactory.cs`.

### DI wiring
- `src/SkillGhcDemo.Application/DependencyInjection.cs` registers MediatR handlers and FluentValidation validators via assembly scanning — new queries/handlers/validators added to the Application assembly are picked up automatically, no manual registration needed.

### Unit test structure (for later `my-unit-tests` reference, not touched by this research)
- `tests/SkillGhcDemo.UnitTests/Queries/<Query>Data.cs` — test data factories per query (e.g. `GetProductsQueryData.cs`, `GetOrderByIdQueryData.cs`).
- `tests/SkillGhcDemo.UnitTests/Handlers/Queries/<Query>HandlerTests.cs` — handler tests per query (e.g. `GetProductsQueryHandlerTests.cs`, `GetOrderByIdQueryHandlerTests.cs`).
- `tests/SkillGhcDemo.UnitTests/Controllers/<Feature>ControllerTests.cs` — one file per controller (e.g. `OrdersControllerTests.cs`, `ProductsControllerTests.cs`), extended per new action.
- `tests/SkillGhcDemo.UnitTests/Common/` — shared helpers: `ResultAssertions.cs`, `TestDbContextFactory.cs`, `TestLogger.cs`.

## Code References
- `src/SkillGhcDemo.Domain/Entities/Order.cs:10-26` - `Order` entity definition
- `src/SkillGhcDemo.Domain/Entities/OrderItem.cs:10-27` - `OrderItem` entity, links `Order` to `Product`
- `src/SkillGhcDemo.Domain/Entities/Product.cs:9-27` - `Product` entity, has `OrderItems` navigation
- `src/SkillGhcDemo.Domain/Entities/User.cs:9-21` - `User` entity, has `Orders` navigation
- `src/SkillGhcDemo.Application/Queries/Orders/GetOrderByIdQuery.cs:7` - existing single-order query
- `src/SkillGhcDemo.Application/Handlers/Queries/Orders/GetOrderByIdQueryHandler.cs:10-27` - existing order handler pattern
- `src/SkillGhcDemo.Application/Queries/Products/GetProductsQuery.cs:8-9` - "list by filter" query template
- `src/SkillGhcDemo.Application/Handlers/Queries/Products/GetProductsQueryHandler.cs:10-39` - "list by filter" handler template
- `src/SkillGhcDemo.Application/Dtos/OrderDto.cs:7-53` - `OrderDto`/`OrderItemDto` + mapping extensions
- `src/SkillGhcDemo.Application/Dtos/ProductDto.cs:6-43` - `ProductDto` + mapping extension
- `src/SkillGhcDemo.Application/Dtos/UserDto.cs:6-27` - `UserDto` + mapping extension
- `src/SkillGhcDemo.Api/Controllers/OrdersController.cs:9-34` - `OrdersController` actions
- `src/SkillGhcDemo.Api/Controllers/ProductsController.cs:9-61` - `ProductsController`, filter query-param binding pattern
- `src/SkillGhcDemo.Api/Common/ApiControllerBase.cs:14-39` - base controller, `Result` → HTTP mapping
- `src/SkillGhcDemo.Application/Common/Models/Result.cs:1-65` - `Result`/`Result<T>` envelope
- `src/SkillGhcDemo.Application/Common/Interfaces/IAppDbContext.cs:10-20` - DbContext abstraction used by handlers

## Architecture Documentation
- **Layering**: `Domain` (entities/enums) → `Application` (Queries/Commands + Handlers + Dtos + `Result<T>` + `IAppDbContext`) → `Infrastructure` (EF Core implementation) → `Api` (thin MediatR-calling controllers). `Application` never depends on `Infrastructure` directly — it only depends on the `IAppDbContext` interface.
- **Query pattern**: single-item queries are `record Get<X>ByIdQuery(Guid Id) : IRequest<Result<XDto>>`; list/filter queries are `record Get<X>Query(<optional filters with defaults>) : IRequest<Result<IReadOnlyList<XDto>>>`.
- **Handler pattern**: constructor-injects `IAppDbContext`, builds query with `AsNoTracking()` + `Include()` for needed navigations, applies `.Where()` filters conditionally (only when the filter parameter is non-null/non-empty), materializes with `ToListAsync`/`FirstOrDefaultAsync`, maps entities to DTOs via extension methods, wraps result in `Result<T>`.
- **Controller pattern**: one controller per feature, route `api/[controller]` from `ApiControllerBase`, GET actions bind filters via `[FromQuery]`, call `Mediator.Send(...)`, return `ToResponse(result)`; POST actions use the `CreatedAtAction` pattern referencing the `GetById` action.
- **DI**: MediatR handlers and FluentValidation validators are auto-registered via assembly scanning in `Application/DependencyInjection.cs` — no manual wiring per feature.

## Related Research
None found in `thoughts/research/` prior to this document.

## Open Questions
- Whether the new "orders by product id" endpoint should return `OrderDto` (ids only, current shape) extended with full `ProductDto`/`UserDto` payloads, or whether a new richer DTO/shape is needed — current `OrderDto`/`OrderItemDto` only expose `UserId`/`ProductId`, not the full nested objects.
- Whether the "product" information should be the single product matching the filter (since one order can contain multiple items) or all products in the order's items.
