---
name: my-api-implementation
description: Implement a new read/write API endpoint end-to-end in this repository from a short feature brief — e.g. "Create an api that retrieve all orders by product id", "add an endpoint to list products by category", "expose a query for X by Y". Follows the existing CQRS/MediatR layering (Domain → Application → Api). Requires the my-research document for this repo (runs my-research automatically if missing) and always finishes by running my-unit-tests.
---

## GENERAL

### YOUR ROLE
You are an experienced backend engineer implementing a new API feature in this solution end-to-end, strictly following the CQRS/MediatR pattern already established in the codebase. You never invent a new architecture — you replicate the layering, naming, and file organization of the closest existing feature (Commands/Queries → Handlers → Dtos → Controllers).

The convention you must follow, based on existing features (Categories, Orders, Prices, Products, Users):
- **`src/SkillGhcDemo.Application/Queries/<Feature>/`** — one `Get<X>Query.cs` per query, a `record` implementing `IRequest<Result<T>>` (or `Result<IReadOnlyList<T>>` for lists), with optional filter parameters as default-valued positional parameters.
- **`src/SkillGhcDemo.Application/Commands/<Feature>/`** — one `<Verb><X>Command.cs` per command, same `IRequest<Result<T>>` pattern; add a matching FluentValidation `<Command>Validator.cs` if the command needs input validation (mirror an existing validator in the same folder for style).
- **`src/SkillGhcDemo.Application/Handlers/Queries/<Feature>/`** and **`Handlers/Commands/<Feature>/`** — one `<QueryOrCommand>Handler.cs` per query/command, implementing `IRequestHandler<TRequest, Result<T>>`, injecting `IAppDbContext` (EF Core), using `AsNoTracking()` + `Include()` for reads, returning `Result<T>.Success(...)` / `Result<T>.NotFound(...)` / `Result<T>.Conflict(...)` as appropriate.
- **`src/SkillGhcDemo.Application/Dtos/<X>Dto.cs`** — reuse the existing DTO and its `ToDto()` mapping extension for the entity if one already exists; only add a new DTO/mapping if the shape truly doesn't exist yet.
- **`src/SkillGhcDemo.Api/Controllers/<Feature>Controller.cs`** — add one action to the existing controller (inheriting `ApiControllerBase`), calling `Mediator.Send(...)` and returning `ToResponse(result)` (or the `CreatedAtAction` pattern for creates). Add proper `[HttpGet]`/`[HttpPost]`/route and `[ProducesResponseType]` attributes matching sibling actions.
- MediatR handlers and FluentValidation validators are auto-registered via assembly scanning in `SkillGhcDemo.Application/DependencyInjection.cs` — **no manual DI wiring is needed** for new queries/commands/handlers/validators.

### WHAT NOT TO DO
- DO NOT skip the research precondition — never implement the feature without a current `my-research` document in hand (see RULES/Step 1).
- DO NOT invent a new folder layout, naming scheme, or response envelope — mirror the closest existing feature exactly.
- DO NOT write or edit unit tests yourself — that is the `my-unit-tests` skill's job; you only run it at the end (see Step 5).
- DO NOT touch unrelated files, features, or entities outside the scope of the requested endpoint.
- DO NOT add new NuGet packages or change DI/startup wiring unless the requested feature truly cannot be built without it — ask the user first if you think it's needed.
- DO NOT skip building the solution before handing off to `my-unit-tests`.

### MODEL TO EXECUTE THE SKILL
Use **Claude Sonnet** (or an equivalent strong coding model) to execute this skill — implementation requires accurately mirroring existing patterns across multiple files.
- If Sonnet is available in this environment but not currently running, ask the user whether they'd like to switch before proceeding.
- If unavailable, proceed with the current model rather than blocking.

### BEST PRACTICES & EFFICIENCY
- **Reuse before creating.** Reuse an existing DTO, mapping extension, or filter pattern (e.g. `GetProductsQuery`'s optional-filter style) instead of writing a new one from scratch. Only add a new file when nothing existing covers the shape needed.
- **Efficient queries.** Use `AsNoTracking()` for all reads, only `Include()` the navigation properties actually needed by the response DTO, and push filters into the LINQ query (`Where`) rather than filtering in memory after materialization.
- **Minimal surface area.** Add the smallest set of files/changes that fully implements the requested endpoint — one query/command, one handler, one controller action, reusing DTOs — instead of speculatively generalizing or adding unrequested filters/endpoints.
- **Batch investigation.** Read all the relevant existing feature files (query, handler, DTO, controller, entity) in parallel up front (Step 3) rather than one at a time, to minimize round-trips before implementation starts.
- **No premature abstraction.** Don't introduce generic/base classes, extra layers, or new packages to "future-proof" the endpoint unless the existing codebase already does this for similar features.

### OUTPUT
- Always provide the new API route created as part of the implementation. Example: `GET /pippo/pluto?id={id}`
- New/changed files live under `src/SkillGhcDemo.Application/{Queries,Commands,Handlers,Dtos}/...` and `src/SkillGhcDemo.Api/Controllers/...`, following the folder-per-feature layout described above.
- Always summarize the affected files in a table, marking each row with 🟢 for newly created files and 🟡 for edited/modified files, plus a short "Details" note describing what was added or changed. Use this exact format:

  | Status | File | Details |
  |--------|------|---------|
  | 🟢 Created | `src/SkillGhcDemo.Application/Queries/GetPippoQuery.cs` | record `GetPippoQuery(Guid Id)` |
  | 🟢 Created | `src/SkillGhcDemo.Application/Handlers/GetPippoQueryHandler.cs` | filters + includes logic |
  | 🟡 Edited | `src/SkillGhcDemo.Application/Dtos/PippoDto.cs` | added `PippoWithDetailsDto` mapping |
  | 🟡 Edited | `src/SkillGhcDemo.Api/Controllers/PippoController.cs` | added `GET /pippo?id={id}` action |
  | 🟢 Created | `tests/.../GetPippoQueryData.cs` | test data factory |
  | 🟢 Created | `tests/.../GetPippoQueryHandlerTests.cs` | 2 tests |
  | 🟡 Edited | `tests/.../PippoControllerTests.cs` | +1 test |

- No files are created under `tests/` by this skill — that is delegated entirely to `my-unit-tests` in the final step.

### OPEN QUESTIONS
- If the requested behavior is ambiguous (e.g. unclear filter semantics, unclear whether it's a query or a command, unclear response shape), or a needed DTO/pattern genuinely doesn't exist yet, ask the user before proceeding rather than guessing.
- If implementing the feature seems to require a new package, DI change, or cross-cutting behavior, stop and ask the user instead of adding it silently.

**At the END of every response:**
```
Open Questions:
- [If there are questions, list them here; otherwise write "None."]

Example:

Open Questions:
1. Do you want this analysis to include vendor tools?
2. Is the "no dependencies" principle important for bootstrapping?
```

## STEPS TO FOLLOW

### Step 1 — CHECK the research precondition
- Per `my-skills/my-research.md`, research output lives at `thoughts/research/YYYY-MM-DD-research-<repo-name>.md` (here `<repo-name>` is `github-copilot-skills-demo`).
- **If a research document already exists** for this repo → read it fully and use it as context; do NOT regenerate it.
- **If it does NOT exist** → run the `my-research` skill now, focused specifically on the new feature described in the user's brief (e.g. for "retrieve all orders by product id", focus research on the Orders feature: entities, existing queries/handlers/controller, DTOs, and how similar "list by filter" queries — like `GetProductsQuery` — are implemented).

### Step 2 — UNDERSTAND the brief
- Parse the user's brief into: the target feature/entity (e.g. Orders), the operation type (query vs. command), the filter/input parameters (e.g. `productId`), and the expected shape of the response (e.g. a list of orders).
- Identify the closest existing analogous implementation in the codebase (e.g. a query that already filters a list by an id/foreign key, such as `GetProductsQuery`'s `CategoryId` filter) to use as the concrete template for naming and structure.

### Step 3 — LOCATE the target feature's existing files
- Read the existing files for the target feature across all layers: `Queries/<Feature>/`, `Commands/<Feature>/`, `Handlers/Queries|Commands/<Feature>/`, `Dtos/<X>Dto.cs`, `Api/Controllers/<Feature>Controller.cs`, and the relevant `Domain/Entities` class.
- Confirm whether the required DTO/mapping already exists (reuse it) or needs to be added.

### Step 4 — IMPLEMENT the feature
In this order:
1. Add the `Get<X>By<Filter>Query.cs` (or `<Verb><X>Command.cs`) record under `Queries/<Feature>/` (or `Commands/<Feature>/`).
2. Add the FluentValidation validator alongside it, only if it's a command with inputs that need validation.
3. Add the handler under `Handlers/Queries|Commands/<Feature>/`, injecting `IAppDbContext`, applying the filter via LINQ (`Where`, `Include` as needed), and returning the `Result<T>` envelope.
4. Reuse the existing DTO/`ToDto()` mapping; only extend it if the feature needs fields it doesn't already expose.
5. Add the controller action to the existing `<Feature>Controller.cs`, with correct route, HTTP verb, and `[ProducesResponseType]` attributes matching sibling actions.

### Step 5 — BUILD and VERIFY
- Build the solution (e.g. `dotnet build`) and fix any compilation errors before moving on.
- Do not proceed to Step 6 with a solution that doesn't build.

### Step 6 — RUN my-unit-tests
- Run the `my-unit-tests` skill, scoped to the new/edited files from Step 4, so it generates the corresponding tests following its own by-artifact-type convention.
- Do not write any test code yourself in this skill — let `my-unit-tests` own that entirely.

### Step 7 — REPORT (final summary)
At the end, present:
1. **Files created/edited** — one line per file, in plain language (e.g. `Created Queries/Orders/GetOrdersByProductIdQuery.cs`).
2. **Endpoint added** — the route and HTTP verb (e.g. `GET /api/orders?productId={id}`).
3. **Build result** — confirm the solution builds cleanly.
4. **my-unit-tests summary** — pass through the summary produced by the `my-unit-tests` run (files/tests added, pass/fail count, coverage %).

---

## RULES

- **Research precondition.** Always check for the `my-research` document first. If missing, run `my-research` focused on the feature being implemented before writing any code. If present, read it and do not regenerate it.
- **Mirror, don't invent.** Every new file must follow the folder-per-layer, naming, and `Result<T>` conventions of the closest existing feature — never introduce a new pattern.
- **No manual DI wiring.** Rely on assembly scanning for MediatR handlers and FluentValidation validators; don't touch `DependencyInjection.cs` unless a genuinely new cross-cutting concern is required (ask the user first).
- **Tests are out of scope here.** This skill never writes test code directly — it always finishes by invoking `my-unit-tests` for the files it created or changed.
- **Build before handoff.** The solution must compile before `my-unit-tests` is run.
- **Stay in scope.** Touch only the files needed for the requested endpoint; do not refactor or "improve" unrelated features along the way.
