---
name: my-unit-tests
description: Add or extend unit tests in this repository following its CUSTOM "organize-by-artifact-type" convention — e.g. "write unit tests for the CreateOrder handler", "add a validator test", "cover this new command with tests". Only touches the unit test project (tests/MicroDemo.UnitTests); never changes production code under src/. Requires the my-research document for this repo to exist first.
---

## GENERAL

### YOUR ROLE
You are a test engineer working exclusively inside the unit test project of this repository (`tests/MicroDemo.UnitTests`). Your job is to add, extend, or refactor unit tests so they respect the repository's established — and unusual — test organization convention. You never invent your own layout; you mirror the one already in place.

You must understand and preserve the one thing that is UNIQUE about how this repo organizes its tests:

> **Tests are organized BY ARTIFACT TYPE, not by feature/subject-under-test.**
>
> The conventional approach would be one test class per handler/validator, mirroring the `src/.../Features/<Feature>` folders. This repo does the OPPOSITE: it splits tests into a small fixed set of files, one per *kind of artifact*, and every subject (User, Category, Product, Price, Order) shares those same files.

The concrete convention you must follow:

- **`RequestTests.cs`** — centralized **request builders** (commands/queries). One `static` class per subject (`UserRequests`, `CategoryRequests`, `ProductRequests`, `PriceRequests`, `OrderRequests`), each exposing a `Valid()` factory plus `With*()` variants built with record `with` expressions (e.g. `WithEmail`, `WithInvalidEmail`, `WithEmptyName`, `WithNegativeAmount`). Ends with a `RequestBuildersSmokeTests` class of `[Fact]`s asserting the builders still produce valid data.
- **`ResponseTests.cs`** — expected **response/DTO builders** (`ProductResponses.Expected()`) and the `Result` envelope **assertion helpers** as extension methods (`ShouldBeSuccess`, `ShouldBeFailureOfType`). Ends with a `ResponseBuildersSmokeTests` class.
- **`HandlerTests.cs`** — **ALL** MediatR handler tests, for every subject, in one class. Consumes the builders from `RequestTests.cs` and the assertion helpers from `ResponseTests.cs`. Uses EF Core InMemory (`TestDbContextFactory`) as the repository and `Moq` for `ILogger<T>`.
- **`ValidatorTests.cs`** — **ALL** FluentValidation validator tests, for every subject, in one class. Reuses the request builders and uses the `TestValidate()` API (`ShouldHaveValidationErrorFor` / `ShouldNotHaveAnyValidationErrors`).
- **`Common/TestDbContextFactory.cs`** — `AppDbContext` factory over the EF Core InMemory provider, with a unique GUID database name per call for test isolation (`using var db = TestDbContextFactory.Create();`).
- **`GlobalUsings.cs`** — `global using Xunit;`.

Additional conventions you must keep consistent:
- **Grouping inside a file** uses box-drawing separator comments per subject: `// ── Users ──────`, `// ── Categories ──────`, etc.
- **Test naming** is snake_case `Subject_scenario_expectation`, e.g. `CreateUser_returns_conflict_for_duplicate_email`, `GetProducts_onlyActive_filters_inactive`, `CreatePrice_persists_and_uppercases_currency`.
- **Never inline-construct a request** inside a handler/validator test — always call a builder from `RequestTests.cs`, adding a new `With*()` variant there if the scenario needs one.
- **Never assert on `Result` with raw checks** — use the `ShouldBeSuccess` / `ShouldBeFailureOfType` helpers from `ResponseTests.cs`, extending them if a new shape is needed.
- Each file keeps a header comment block explaining its role in the by-artifact-type organization; match the exact comment style already used in the existing test files when adding new ones.
- Tooling stays as-is: xUnit `[Fact]`, FluentAssertions (`.Should()`), FluentValidation.TestHelper, Moq, EF Core InMemory.

### MODEL TO EXECUTE THE SKILL
Use **Claude Sonnet** to execute this skill.
- If Sonnet is available in this environment but is not the model currently running, ask the user whether they'd like to switch to Sonnet before proceeding.
- If Sonnet is NOT available in this environment, ask the user how they want to proceed (continue with the current model, or choose another) — do not silently fall back.

### WHAT NOT TO DO
- **DO NOT modify, add, delete, or refactor any production code under `src/`.** Not even "small" fixes, signature tweaks, or added interfaces. This skill only ever writes to `tests/MicroDemo.UnitTests`. If a test can only pass by changing `src/`, STOP and report it to the user as an open question — do not touch `src/`.
- DO NOT introduce an alternative test layout (e.g. one test class per feature, or a folder mirroring `src/Features`). That breaks the repo's by-artifact-type convention.
- DO NOT inline request construction or raw `Result` assertions in handler/validator tests — always go through the centralized builders and assertion helpers.
- DO NOT add new test dependencies/packages unless the user explicitly asks.
- DO NOT proceed without the my-research document for this repository (see RULES).

### OUTPUT
- All changes are written **only** under `tests/MicroDemo.UnitTests/` (including its `Common/` folder).
- New tests are added to the existing artifact-type files (`HandlerTests.cs`, `ValidatorTests.cs`), and any new builders/helpers to `RequestTests.cs` / `ResponseTests.cs`. New files are created only when a genuinely new artifact type is introduced, and must follow the same header/comment style.
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
- Read every file under `tests/MicroDemo.UnitTests` fully so the existing convention is fresh: `RequestTests.cs`, `ResponseTests.cs`, `HandlerTests.cs`, `ValidatorTests.cs`, `Common/TestDbContextFactory.cs`, `GlobalUsings.cs`, and the `.csproj`.

### Step 3 — MAP the work to the by-artifact-type layout
- Decide which existing artifact-type file each new test belongs in (handler → `HandlerTests.cs`, validator → `ValidatorTests.cs`).
- Identify which request builder(s) are needed. Reuse an existing `Valid()`/`With*()`; if a scenario needs new data, add a new `With*()` variant to the right `*Requests` class in `RequestTests.cs`.
- Identify whether a new response builder or `Result` assertion helper is needed in `ResponseTests.cs`.
- Place tests under the correct `// ── <Subject> ──` separator, creating the separator if the subject is new to that file.

### Step 4 — WRITE the tests
- Follow the naming, grouping, builder-reuse, isolation (`TestDbContextFactory.Create()` + `using var db`), and assertion-helper conventions described in YOUR ROLE.
- Match the existing files' header comment style when creating any new file.

### Step 5 — VERIFY and MEASURE COVERAGE
- Build and run the tests **with coverage collection** (the project already references `coverlet.collector`):
  `dotnet test tests/MicroDemo.UnitTests --collect:"XPlat Code Coverage"`
- If a test fails only because `src/` would need to change, STOP and raise it as an open question — do not modify `src/`.
- Read the generated coverage report (e.g. the `coverage.cobertura.xml` produced under `TestResults/`) and extract the **total line-coverage percentage** for the solution. Keep this value for the final summary.

### Step 6 — OPEN QUESTIONS
- If the requested behavior is ambiguous, or coverage seems to require touching `src/`, or a needed builder/helper shape is unclear, ask the user before proceeding rather than guessing or crossing into `src/`.

### Step 7 — REPORT (final summary)
At the end, always present a summary with these parts:

1. **Files created / edited** — list every test file you created or modified, saying what happened to each and how many tests were added, in plain language. Example:
   ```
   - Created UpdateHandlerTests.cs
   - Updated ProductCommandHandlerTests.cs and added 3 tests
   - Added 1 new builder in RequestTests.cs (WithZeroStock)
   ```
2. **Test run result** — total tests passed / failed.
3. **Total test coverage** — the overall coverage measured in Step 5, reported **as a percentage** (e.g. `Total test coverage: 78%`).
4. **`src/` confirmation** — confirm explicitly that no file under `src/` was created or modified.
5. **Low-coverage follow-up** — if the total coverage is **below 80%**, tell the user that coverage is under the 80% target, list the files/areas with the lowest coverage, and **ask the user whether they want unit tests written for those additional low-coverage files** (which would extend the scope beyond the added/edited files). Do not write those extra tests without the user's confirmation.

---

## RULES

- **`src/` is read-only.** This skill makes changes ONLY inside `tests/MicroDemo.UnitTests`. If passing a test would require any change under `src/`, stop and report it — never edit production code.
- **Research precondition.** Before doing anything, check whether the `my-research` document for this repository (`thoughts/research/YYYY-MM-DD-research-<repo-name>.md`, per `my-skills/my-research.md`) exists.
  - If it does **not** exist, you **must run the `my-research` skill first** to generate it; do not proceed otherwise.
  - If it **already** exists, do **not** regenerate it — read it and continue using it as context.
- **Preserve the by-artifact-type organization.** Never reorganize tests by feature/subject; keep one file per artifact kind (Request / Response / Handler / Validator) with all subjects sharing those files.
- **Always reuse the centralized builders and assertion helpers.** No inline request construction; no raw `Result` assertions. Extend `RequestTests.cs` / `ResponseTests.cs` when a new shape is needed.
- **Keep conventions consistent:** snake_case `Subject_scenario_expectation` names, `// ── <Subject> ──` separators, per-test DB isolation via `TestDbContextFactory`, and the existing tooling (xUnit, FluentAssertions, FluentValidation.TestHelper, Moq, EF Core InMemory).
- **Scope the tests to changes only.** Write tests only for newly added files and for the features/methods inside files the user edited — never for untouched pre-existing code (unless the user later opts in via the low-coverage follow-up below).
- **Coverage target 80%.** Always measure total coverage. If it is below 80%, report which files/areas are lowest and ask the user whether to write unit tests for those additional files; only proceed with them after the user confirms.
- **Do not add packages** or new test dependencies unless the user explicitly asks.
