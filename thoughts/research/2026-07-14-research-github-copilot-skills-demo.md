---
date: 2026-07-14T18:10:00+02:00
researcher: Bianca Rangel Campinho de Andrade
git_commit: 58d73ed76f3e00fd4d846ac81ee4b3033c39adb9
branch: feature/test-skills-demo
repository: github-copilot-skills-demo
topic: "How the codebase is structured to support a new 'orders by product id' read API (product + user info)"
tags: [research, codebase, cqrs, mediatr, orders, products, users, api]
status: complete
last_updated: 2026-07-14
last_updated_by: Bianca Rangel Campinho de Andrade
---

# Research: How the codebase is structured to support a new "orders by product id" read API (product + user info)

**Date**: 2026-07-14T18:10:00+02:00
**Researcher**: Bianca Rangel Campinho de Andrade
**Repository**: github-copilot-skills-demo

## Research Question
The user wants to create a new API that retrieves orders by product id, returning information about the product and the user for each order. This research documents the existing architecture, conventions, and code paths relevant to implementing such a read endpoint.

## Summary
The repository (`SkillGhcDemo`) is a .NET solution organized in four layers — `Domain`, `Application`, `Infrastructure`, `Api` — following CQRS with MediatR. Each read operation is implemented as a `Query` record in `Application/Queries/<Aggregate>/`, handled by a matching `Handler` in `Application/Handlers/Queries/<Aggregate>/` that queries `IAppDbContext` (EF Core `DbSet`s) and maps entities to DTOs via extension methods in `Application/Dtos/`. Controllers in `Api/Controllers/` expose these queries over HTTP through a shared `ApiControllerBase` that funnels `Result<T>` into HTTP responses. There is already a `GetOrderByIdQuery` returning an `OrderDto` (order + its `OrderItem`s), and an `Order` → `User` navigation, and `OrderItem` → `Product` navigation, meaning all relationships needed for an "orders by product id, with product and user info" query already exist in the EF model (`Order.User`, `OrderItem.Product`, `OrderItem.Order.User`). No such query/endpoint currently exists — it would be a new slice following the same Query/Handler/DTO/Controller pattern, analogous to `GetOrderByIdQuery`/`GetProductsQuery`.

## Detailed Findings

### Solution / project layout
- `src/SkillGhcDemo.Domain` — entities, enums, common base types (no dependencies on other layers).
- `src/SkillGhcDemo.Application` — CQRS layer: `Commands/`, `Queries/`, `Handlers/Commands/`, `Handlers/Queries/`, `Dtos/`, `Common/` (Behaviors, Exceptions, Interfaces, Models), `DependencyInjection.cs`.
- `src/SkillGhcDemo.Infrastructure` — EF Core `AppDbContext`, entity `Configurations/`, `Migrations/`, `DependencyInjection.cs`.
- `src/SkillGhcDemo.Api` — ASP.NET Core Web API: `Controllers/`, `Middleware/`, `Common/ApiControllerBase.cs`, `Program.cs`.
- `tests/SkillGhcDemo.UnitTests` — mirrors Application structure: `Queries/` (test-data builders, suffixed `...Data.cs`), `Handlers/Commands|Queries/` (`...HandlerTests.cs`), `Validators/` (`...ValidatorTests.cs`), `Controllers/` (`...ControllerTests.cs`), `Commands/`.
- `sql/schema.sql`, `sql/seed.sql` — raw T-SQL mirroring the EF Core migration `InitialCreate`.

### Domain model (entities and relationships)
- `Order` (`src/SkillGhcDemo.Domain/Entities/Order.cs:10-26`): has `UserId`/`User` navigation, `TotalAmount`, `Currency`, `Status` (`OrderStatus` enum), `OrderDateUtc`, and `ICollection<OrderItem> Items`.
- `OrderItem` (`src/SkillGhcDemo.Domain/Entities/OrderItem.cs:10-27`): has `OrderId`/`Order` navigation, `ProductId`/`Product` navigation, `Quantity`, price snapshot fields (`UnitPrice`, `Currency`, `LineTotal`).
- `Product`, `User`, `Category`, `Price` entities live alongside (`src/SkillGhcDemo.Domain/Entities/`), all inheriting `BaseEntity` (`Domain/Common`).
- FK chain relevant to "orders by product": `OrderItem.ProductId → Product`, `OrderItem.OrderId → Order`, `Order.UserId → User` — confirmed both in the domain navigation properties and in `sql/schema.sql:88-145` (`Orders.UserId → Users`, `OrderItems.OrderId → Orders`, `OrderItems.ProductId → Products`).
- This means: to find "orders that contain a given product" you go `OrderItems` filtered by `ProductId`, then join to `Order` (for order/user data) and `Product` (for product data).

### Existing CQRS slice for Orders (read side)
- Query: `GetOrderByIdQuery` (`src/SkillGhcDemo.Application/Queries/Orders/GetOrderByIdQuery.cs:7`) — `record GetOrderByIdQuery(Guid Id) : IRequest<Result<OrderDto>>`.
- Handler: `GetOrderByIdQueryHandler` (`src/SkillGhcDemo.Application/Handlers/Queries/Orders/GetOrderByIdQueryHandler.cs:10-27`) — uses `_db.Orders.AsNoTracking().Include(o => o.Items).FirstOrDefaultAsync(...)`, returns `Result<OrderDto>.NotFound(...)` or `.Success(order.ToDto())`.
- DTO + mapping: `OrderDto`/`OrderItemDto` and `OrderMappingExtensions.ToDto()` (`src/SkillGhcDemo.Application/Dtos/OrderDto.cs:1-53`). Note `OrderDto` currently does **not** include `User` or `Product` info — only `UserId` and `ProductId` (ids only), so a richer DTO would be a new type (or an extension of the existing one) for the new use case.
- Controller: `OrdersController.GetById` (`src/SkillGhcDemo.Api/Controllers/OrdersController.cs:11-16`) maps `GetOrderByIdQuery` to `[HttpGet("{id:guid}")]` via `Mediator.Send` and `ToResponse(...)`.
- `OrdersController.Create` (lines 18-36) is explicitly marked "★ ENDPOINT RESERVED FOR MANUAL IMPLEMENTATION (custom skill) ★" — its handler `CreateOrderCommandHandler` is a stub; unrelated to this read scenario but shows the convention for commands too.

### List-style query pattern (filters) — `GetProductsQuery`
- `GetProductsQuery` (`src/SkillGhcDemo.Application/Queries/Products/GetProductsQuery.cs:8-9`): `record GetProductsQuery(string? Search = null, Guid? CategoryId = null, bool OnlyActive = false) : IRequest<Result<IReadOnlyList<ProductDto>>>` — demonstrates the convention for a filtered list query (multiple optional parameters, `IReadOnlyList<T>` result) which is the closest existing analog to an "orders by product id" list query.
- Corresponding handler in `src/SkillGhcDemo.Application/Handlers/Queries/Products/GetProductsQueryHandler.cs` (not fully inspected line-by-line, but sibling of `GetProductByIdQueryHandler.cs` in the same folder).
- `ProductsController.GetAll` (`src/SkillGhcDemo.Api/Controllers/ProductsController.cs:14-18`) shows binding of query-string filters via `[FromQuery]` parameters into the query record, returning `IReadOnlyList<ProductDto>`.

### DTOs and mapping convention
- Each aggregate has a `<Entity>Dto.cs` in `src/SkillGhcDemo.Application/Dtos/` plus a `<Entity>MappingExtensions` static class with a `ToDto(this Entity e)` extension method (see `OrderDto.cs:30-53`, `ProductDto.cs:21-43`, `UserDto.cs:16-27`).
- `ProductDto` (`src/SkillGhcDemo.Application/Dtos/ProductDto.cs:6-19`) includes `CurrentPrice`/`Currency` computed from `product.Prices` (active, most recent by `ValidFromUtc`) — mapping logic can reach into related collections, not just flat fields.
- `UserDto` (`src/SkillGhcDemo.Application/Dtos/UserDto.cs:6-14`) is a flat projection of `User` (no navigation properties).
- No combined DTO currently exists that nests `ProductDto` and `UserDto` together inside an order projection — this would be new for the requested feature.

### Result envelope and error mapping
- `Result`/`Result<T>` (`src/SkillGhcDemo.Application/Common/Models/Result.cs:1-64`) is the standard envelope: `Success`, `Failure`, `NotFound`, `Invalid`, carrying a `ResultErrorType` (`None`, `Failure`, `Validation`, `NotFound`, `Conflict`).
- `ApiControllerBase.ToResponse<T>(Result<T>)` and `ToResponse(Result)` (`src/SkillGhcDemo.Api/Common/ApiControllerBase.cs:20-38`) translate the envelope into `Ok`, `NoContent`, `NotFound`, `Conflict`, or `BadRequest` with `{ error, errors }` payloads. All controllers inherit from this base and use `Mediator` (an `ISender`) resolved lazily from `HttpContext.RequestServices`.

### Data access abstraction
- `IAppDbContext` (`src/SkillGhcDemo.Application/Common/Interfaces/IAppDbContext.cs:10-20`) exposes `DbSet<User>`, `DbSet<Category>`, `DbSet<Product>`, `DbSet<Price>`, `DbSet<Order>`, `DbSet<OrderItem>`, plus `SaveChangesAsync`. Handlers depend on this interface (not the concrete `AppDbContext`), enabling unit testing with an in-memory/fake implementation.
- Concrete `AppDbContext` and EF configurations live in `src/SkillGhcDemo.Infrastructure/Persistence/` (`AppDbContext.cs`, `Configurations/*Configuration.cs`, `Migrations/`).
- Query handlers use `AsNoTracking()` plus `.Include(...)` for eager loading (see `GetOrderByIdQueryHandler.cs:18-21`), and LINQ `FirstOrDefaultAsync`/filtering directly against the `DbSet`.

### Pipeline behaviors and DI wiring
- `Application/DependencyInjection.cs` (`src/SkillGhcDemo.Application/DependencyInjection.cs:14-27`) registers MediatR handlers from the executing assembly, FluentValidation validators from the assembly, and two pipeline behaviors in order: `LoggingBehavior` (outer) then `ValidationBehavior` (inner) (`src/SkillGhcDemo.Application/Common/Behaviors/`).
- `Program.cs` (`src/SkillGhcDemo.Api/Program.cs:10-11`) wires `AddApplication()` and `AddInfrastructure(...)`, plus Swagger with XML comments, an `ExceptionHandlingMiddleware`, and (dev-only) automatic `db.Database.Migrate()`.

### Test conventions (for later `my-unit-tests` skill use)
- `tests/SkillGhcDemo.UnitTests/Queries/*Data.cs` — static factory classes producing sample query instances, e.g. `GetOrderByIdQueryData.ForId(id)` / `.Unknown()` (`tests/SkillGhcDemo.UnitTests/Queries/GetOrderByIdQueryData.cs:6-11`).
- `tests/SkillGhcDemo.UnitTests/Handlers/Queries/*HandlerTests.cs` — one test class per handler (e.g. `GetOrderByIdQueryHandlerTests.cs`, `GetProductsQueryHandlerTests.cs`).
- `tests/SkillGhcDemo.UnitTests/Controllers/*ControllerTests.cs` — one per controller (e.g. `OrdersControllerTests.cs`).
- `tests/SkillGhcDemo.UnitTests/Validators/*ValidatorTests.cs` — one per FluentValidation validator, only for Commands (Create/Update), not Queries.
- This is the "organize-by-artifact-type" convention referenced by the `my-unit-tests` skill: tests are grouped by artifact kind (Queries/Handlers/Validators/Controllers), not co-located with production code, and never placed under `src/`.

## Code References
- `src/SkillGhcDemo.Domain/Entities/Order.cs:10-26` - `Order` entity with `User` navigation and `Items` collection
- `src/SkillGhcDemo.Domain/Entities/OrderItem.cs:10-27` - `OrderItem` entity with `Order` and `Product` navigations, price snapshot
- `src/SkillGhcDemo.Application/Queries/Orders/GetOrderByIdQuery.cs:7` - existing single-order query record
- `src/SkillGhcDemo.Application/Handlers/Queries/Orders/GetOrderByIdQueryHandler.cs:10-27` - existing handler, `Include(o => o.Items)` + `NotFound`/`Success` pattern
- `src/SkillGhcDemo.Application/Queries/Products/GetProductsQuery.cs:8-9` - filtered list query pattern (closest analog for "by product id" list)
- `src/SkillGhcDemo.Application/Dtos/OrderDto.cs:1-53` - `OrderDto`/`OrderItemDto` + `ToDto()` mapping extensions
- `src/SkillGhcDemo.Application/Dtos/ProductDto.cs:1-43` - `ProductDto` + mapping including computed `CurrentPrice`
- `src/SkillGhcDemo.Application/Dtos/UserDto.cs:1-27` - `UserDto` + mapping
- `src/SkillGhcDemo.Application/Common/Interfaces/IAppDbContext.cs:10-20` - DbSet abstraction used by all handlers
- `src/SkillGhcDemo.Application/Common/Models/Result.cs:1-64` - `Result`/`Result<T>` envelope
- `src/SkillGhcDemo.Api/Common/ApiControllerBase.cs:14-39` - shared controller base, `Mediator`, `ToResponse`
- `src/SkillGhcDemo.Api/Controllers/OrdersController.cs:9-37` - existing Orders endpoints
- `src/SkillGhcDemo.Api/Controllers/ProductsController.cs:9-61` - existing Products endpoints (list + filters + CRUD)
- `src/SkillGhcDemo.Application/DependencyInjection.cs:14-27` - MediatR/FluentValidation/behavior registration
- `sql/schema.sql:88-145` - `Orders`, `Prices`, `OrderItems` table definitions and FKs
- `tests/SkillGhcDemo.UnitTests/Queries/GetOrderByIdQueryData.cs:6-11` - test-data builder convention
- `tests/SkillGhcDemo.UnitTests/Handlers/Queries/` - handler test class listing
- `tests/SkillGhcDemo.UnitTests/Controllers/` - controller test class listing

## Architecture Documentation
- **Layering**: `Api → Application → (Domain, Infrastructure)`; `Application` defines `IAppDbContext` (Dependency Inversion), implemented by `Infrastructure.Persistence.AppDbContext`.
- **CQRS via MediatR**: every read is a `record ... : IRequest<Result<T>>` in `Application/Queries/<Aggregate>/`, paired 1:1 with a handler in `Application/Handlers/Queries/<Aggregate>/`. Writes follow the same shape under `Commands/`/`Handlers/Commands/`.
- **DTO mapping**: no AutoMapper — plain static extension methods (`ToDto()`) colocated with each DTO file.
- **Error handling convention**: handlers never throw for expected "not found" flows; they return `Result<T>.NotFound(...)`. Controllers convert `Result` → `IActionResult` uniformly via `ApiControllerBase`.
- **Cross-cutting concerns**: `LoggingBehavior` and `ValidationBehavior` wrap every MediatR request/response pipeline.
- **Test organization**: "organize-by-artifact-type" — parallel folder structure to `Application` under `tests/SkillGhcDemo.UnitTests`, split further by artifact kind (Queries test-data, Handlers tests, Validators tests, Controllers tests) rather than one folder per feature.

## Related Research
None found in `thoughts/research/` prior to this document (folder did not exist before this research).

## Open Questions
- Whether the new "orders by product id" endpoint should return one row per `Order` (aggregated) or one row per `OrderItem` (line-level, in case a product appears multiple times across items) — the DTO shape (e.g., include `Quantity`/`LineTotal` for that specific product) is not yet decided.
- Whether `User` info returned should reuse the existing `UserDto` as-is or a trimmed subset.
- Whether pagination/filtering (e.g., by order status, date range) is desired, following the `GetProductsQuery` filter pattern.

---
Open Questions:
- Whether the new "orders by product id" endpoint should return one row per `Order` (aggregated) or one row per `OrderItem` (line-level).
- Whether `User` info returned should reuse the existing `UserDto` as-is or a trimmed subset.
- Whether pagination/filtering (e.g., by order status, date range) is desired.

Would you like to:
1. Keep iterating - e.g. I can inspect `GetProductsQueryHandler.cs` and `GetProductByIdQueryHandler.cs` in full detail, or the `Category`/`Price` slices, if you want more precedent before implementing.
2. Move to implementation: invoke the `my-api-implementation` skill to build the new "orders by product id" endpoint (product + user info) using this research as context.
