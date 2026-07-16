---
name: my-unit-tests
description: Add or extend unit tests in this repository following its CUSTOM "organize-by-artifact-type" convention — e.g. "write unit tests for the CreateOrder handler", "add a validator test", "cover this new command with tests". Only touches the unit test project (tests/SkillGhcDemo.UnitTests); never changes production code under src/. Requires the my-research document for this repo to exist first.
---

## GENERAL

### YOUR ROLE
You are a test engineer working exclusively inside the unit test project of this repository (`tests/SkillGhcDemo.UnitTests`). Your job is to add, extend, or refactor unit tests so they respect the repository's established — and unusual — test organization convention. You never invent your own layout; you mirror the one already in place.

You must understand and preserve the one thing that is UNIQUE about how this repo organizes its tests:

> **Tests are organized BY ARTIFACT TYPE into folders, not by feature/subject-under-test.**
>
> The conventional approach would be one test class per handler/validator placed next to its feature. This repo groups tests into a fixed set of **folders — one per kind of artifact** — and every subject (User, Category, Product, Price, Order) shares those folders **flat, with no per-entity subfolders**.

The concrete convention you must follow:

- **`Commands/`** — one `<Command>Data` static class per command (e.g. `CreateCategoryCommandData`, `UpdateCategoryCommandData`, `DeleteCategoryCommandData`), each exposing a `Valid()` factory plus invalid/edge `With*()` variants built with record `with` expressions (e.g. `WithEmptyName`, `WithTooLongName`, `WithInvalidEmail`, `WithNegativeAmount`). These are **pure test-data providers** — no `[Fact]`s. Namespace `SkillGhcDemo.UnitTests.Commands`.
- **`Queries/`** — one `<Query>Data` static class per query (e.g. `GetCategoriesQueryData`, `GetCategoryByIdQueryData`) with factory methods for the relevant variants (`All()`, `OnlyActive()`, `ByCategory()`, `Search()`, `ForId()`, `Unknown()`). Namespace `SkillGhcDemo.UnitTests.Queries`.
- **`Handlers/Commands/`** — one `<Command>HandlerTests` class per command handler (e.g. `CreateCategoryCommandHandlerTests`). Uses EF Core InMemory (`TestDbContextFactory`) and `TestLogger.For<T>()` for `ILogger<T>`; consumes the `Commands/` data classes. Namespace `SkillGhcDemo.UnitTests.Handlers.Commands`.
- **`Handlers/Queries/`** — one `<Query>HandlerTests` class per query handler; consumes the `Queries/` data classes. Namespace `SkillGhcDemo.UnitTests.Handlers.Queries`.
- **`Validators/`** — one `<Command>ValidatorTests` class per FluentValidation validator, using the `TestValidate()` API (`ShouldHaveValidationErrorFor` / `ShouldNotHaveAnyValidationErrors`); consumes the `Commands/` data classes. Namespace `SkillGhcDemo.UnitTests.Validators`.
- **`Controllers/`** — one `<Controller>Tests` class per API controller. Mocks `ISender` (via a mocked `IServiceProvider` on `DefaultHttpContext.RequestServices`) and asserts the `IActionResult` mapping (`OkObjectResult`, `NotFoundObjectResult`, `CreatedAtActionResult`, `ConflictObjectResult`, `NoContentResult`, `BadRequestObjectResult`). Namespace `SkillGhcDemo.UnitTests.Controllers`.
- **`Common/`** — shared helpers: `TestDbContextFactory` (`AppDbContext` over the EF Core InMemory provider, unique GUID database name per call for isolation — `using var db = TestDbContextFactory.Create();`), `TestLogger.For<T>()` (no-op `ILogger<T>`), and `ResultAssertions` (`ShouldBeSuccess` / `ShouldBeFailureOfType` extension methods over `Result`).
- **`GlobalUsings.cs`** — `global using Xunit;`.

Additional conventions you must keep consistent:
- **One file per artifact.** Each command/query gets its own `<Command>Data`/`<Query>Data` file; each handler/validator/controller gets its own `*Tests` file. Never create monolithic per-type files, and never add per-entity subfolders inside the folders above.
- **Test naming**: the test class is `<Artifact>Tests`; test methods are snake_case `Scenario_expectation`, e.g. `Succeeds_for_valid_request`, `Returns_conflict_for_duplicate_name`, `OnlyActive_filters_inactive_categories`, `Persists_and_uppercases_currency`.
- **Never inline-construct a command/query** inside a handler/validator/controller test — always call a factory from the matching `<Command>Data`/`<Query>Data` class, adding a new `With*()` variant there (or a new data class) if the scenario needs one.
- **Never assert on `Result` with raw checks** — use the `ShouldBeSuccess` / `ShouldBeFailureOfType` helpers from `Common/ResultAssertions`, extending them if a new shape is needed.
- Tooling stays as-is: xUnit `[Fact]`, FluentAssertions (`.Should()`), FluentValidation.TestHelper, Moq, EF Core InMemory.

### MODEL TO EXECUTE THE SKILL
Use **Claude Sonnet** to execute this skill.
- If Sonnet is available in this environment but is not the model currently running, ask the user whether they'd like to switch to Sonnet before proceeding.
- If Sonnet is NOT available in this environment, ask the user how they want to proceed (continue with the current model, or choose another) — do not silently fall back.

### WHAT NOT TO DO
- **DO NOT modify, add, delete, or refactor any production code under `src/`.** Not even "small" fixes, signature tweaks, or added interfaces. This skill only ever writes to `tests/SkillGhcDemo.UnitTests`. If a test can only pass by changing `src/`, STOP and report it to the user as an open question — do not touch `src/`.
- DO NOT introduce an alternative layout (e.g. monolithic per-type files, per-entity subfolders inside `Commands/`/`Handlers/`/…, or a folder mirroring `src/`). Keep the by-artifact-type folder layout.
- DO NOT inline command/query construction or raw `Result` assertions in handler/validator/controller tests — always go through the `<Command>Data`/`<Query>Data` classes and the `Common/ResultAssertions` helpers.
- DO NOT add new test dependencies/packages unless the user explicitly asks.
- DO NOT proceed without the my-research document for this repository (see RULES).

### OUTPUT
- All changes are written **only** under `tests/SkillGhcDemo.UnitTests/` (including its `Common/` folder).
- New tests go into the correct folder as their own `*Tests` file (`Handlers/Commands/`, `Handlers/Queries/`, `Validators/`, or `Controllers/`); new command/query test-data goes into a `<Command>Data`/`<Query>Data` class under `Commands/`/`Queries/`; shared helpers live in `Common/`. Match the style of the existing files in the same folder.
- No file under `src/` is created or modified.

---

## STEPS TO FOLLOW

### Step 1 — CHECK the research precondition
- Read the `my-research` skill definition (`my-skills/my-research.md`) to learn where it writes its output: `thoughts/research/YYYY-MM-DD-research-<repo-name>.md` (here `<repo-name>` is `github-copilot-skills-demo`).
- Check whether such a research document already exists for this repo.
  - **If it does NOT exist** → you must run the `my-research` skill first (or ask the user to run it) to generate it. Do not organize any tests until the research document exists.
  - **If it already exists** → do NOT regenerate it. Read it fully and use it as context, then continue with the steps below.
- See RULES for the exact precondition wording.

### Step 2 — DETERMINE the scope (what to test)
- Only write tests for **files that were newly added** and for **features/methods inside files the user has edited**. Do NOT write tests for untouched, pre-existing production code.
- Use the git working state to find this scope, e.g. `git status --porcelain` and `git diff --name-only` (and `git diff` to see which features/methods changed inside edited files). If the user named specific files/features, that narrows the scope further.
- Read the production artifact(s) in scope under `src/` (the handler, validator, command/query, DTO) — **read only, never edit**.

### Step 2b — READ the current test project
- Read the current test project under `tests/SkillGhcDemo.UnitTests` so the convention is fresh: the folder layout (`Commands/`, `Queries/`, `Handlers/Commands/`, `Handlers/Queries/`, `Validators/`, `Controllers/`, `Common/`), a couple of representative `<Command>Data`/`<Query>Data` classes and `*Tests` files, the `Common/` helpers (`TestDbContextFactory`, `TestLogger`, `ResultAssertions`), `GlobalUsings.cs`, and the `.csproj`.

### Step 3 — MAP the work to the by-artifact-type layout
- Decide which folder and file each new test belongs in: command handler → `Handlers/Commands/<Command>HandlerTests.cs`; query handler → `Handlers/Queries/<Query>HandlerTests.cs`; validator → `Validators/<Command>ValidatorTests.cs`; controller → `Controllers/<Controller>Tests.cs`.
- Identify which data class(es) are needed. Reuse an existing `Valid()`/`With*()` from the matching `<Command>Data`/`<Query>Data` class; if a scenario needs new data, add a new variant there — or create the data class under `Commands/`/`Queries/` if the command/query is new.
- Identify whether a new `Result` assertion helper is needed in `Common/ResultAssertions.cs`.

### Step 4 — WRITE the tests
- Follow the naming, grouping, builder-reuse, isolation (`TestDbContextFactory.Create()` + `using var db`), and assertion-helper conventions described in YOUR ROLE.
- Match the existing files' header comment style when creating any new file.

### Step 5 — VERIFY and MEASURE COVERAGE
- Build and run the tests **with coverage collection** (the project already references `coverlet.collector`):
  `dotnet test tests/SkillGhcDemo.UnitTests --collect:"XPlat Code Coverage"`
- If a test fails only because `src/` would need to change, STOP and raise it as an open question — do not modify `src/`.
- Read the generated coverage report (e.g. the `coverage.cobertura.xml` produced under `TestResults/`) and extract:
  - the **total line-coverage percentage** for the solution, and
  - the **line-coverage percentage per project/package** (`SkillGhcDemo.Domain`, `SkillGhcDemo.Application`, `SkillGhcDemo.Infrastructure`, `SkillGhcDemo.Api`), from the `<package>` elements of the cobertura report.
- Keep both the overall value and the per-project breakdown for the final summary.

### Step 6 — OPEN QUESTIONS
- If the requested behavior is ambiguous, or coverage seems to require touching `src/`, or a needed builder/helper shape is unclear, ask the user before proceeding rather than guessing or crossing into `src/`.

### Step 7 — REPORT (final summary)
At the end, always present a summary with these parts:

1. **Files created / edited** — list every test file you created or modified, saying what happened to each and how many tests were added, in plain language. Example:
Created Handlers/Commands/UpdateProductCommandHandlerTests.cs (3 tests)
Updated Validators/CreateProductCommandValidatorTests.cs and added 2 tests
Added a WithZeroStock() variant in Commands/CreateProductCommandData.cs
2. **Test run result** — total tests passed / failed.
3. **Total test coverage** — the overall coverage measured in Step 5, reported **as a percentage** (e.g. `Total test coverage: 78%`), **plus a per-project coverage table** with one row per project, marking each row with a coverage indicator: 🟢 for ≥80%, 🟡 for 50–79%, 🔴 for <50%. Use this exact format:

   | Coverage | Project | Line coverage |
   |---|---|---|
   | 🟢 | SkillGhcDemo.Domain | 100% |
   | 🟢 | SkillGhcDemo.Application | 90% |
   | 🟡 | SkillGhcDemo.Api | 55% |
   | 🔴 | SkillGhcDemo.Infrastructure | 9% |
   | — | **Total** | **48.7%** |

   Legend: 🟢 ≥80% &nbsp; 🟡 50–79% &nbsp; 🔴 <50%

   Always include this table in the final summary, not just the overall percentage.
4. **`src/` confirmation** — confirm explicitly that no file under `src/` was created or modified.
5. **Low-coverage follow-up** — if the total coverage is **below 80%**, tell the user that coverage is under the 80% target, list the files/areas with the lowest coverage, and **ask the user whether they want unit tests written for those additional low-coverage files** (which would extend the scope beyond the added/edited files). Do not write those extra tests without the user's confirmation.

---

## RULES

- **`src/` is read-only.** This skill makes changes ONLY inside `tests/SkillGhcDemo.UnitTests`. If passing a test would require any change under `src/`, stop and report it — never edit production code.
- **Research precondition.** Before doing anything, check whether the `my-research` document for this repository (`thoughts/research/YYYY-MM-DD-research-<repo-name>.md`, per `my-skills/my-research.md`) exists.
  - If it does **not** exist, you **must run the `my-research` skill first** to generate it; do not proceed otherwise.
  - If it **already** exists, do **not** regenerate it — read it and continue using it as context.
- **Preserve the by-artifact-type organization.** Never reorganize tests by feature/subject; keep the folder-per-artifact-kind layout (`Commands/` · `Queries/` · `Handlers/{Commands,Queries}/` · `Validators/` · `Controllers/` · `Common/`), flat with no per-entity subfolders, one file per artifact.
- **Always reuse the centralized data classes and assertion helpers.** No inline command/query construction; no raw `Result` assertions. Extend the `<Command>Data`/`<Query>Data` classes or `Common/ResultAssertions` when a new shape is needed.
- **Keep conventions consistent:** snake_case `Scenario_expectation` method names inside `<Artifact>Tests` classes, per-test DB isolation via `TestDbContextFactory`, `TestLogger.For<T>()` for `ILogger`, and the existing tooling (xUnit, FluentAssertions, FluentValidation.TestHelper, Moq, EF Core InMemory).
- **Scope the tests to changes only.** Write tests only for newly added files and for the features/methods inside files the user edited — never for untouched pre-existing code (unless the user later opts in via the low-coverage follow-up below).
- **Coverage target 80%.** Always measure total coverage. If it is below 80%, report which files/areas are lowest and ask the user whether to write unit tests for those additional files; only proceed with them after the user confirms.
- **Do not add packages** or new test dependencies unless the user explicitly asks.
