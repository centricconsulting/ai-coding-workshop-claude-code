---
paths:
  - "src/**/*.cs"
  - "tests/**/*.cs"
  - "**/*.csproj"
---

# .NET Rules (C# / Clean Architecture / DDD)

These rules complement `src/CLAUDE.md`, which holds the layer map, build commands, and testing conventions. They load only when Claude reads or edits .NET files.

## Before Writing Code

- Write or update the failing test first (TDD). Unit tests live in `tests/TaskManager.UnitTests`, integration tests in `tests/TaskManager.IntegrationTests`.
- Check which layer the file belongs to. Dependencies point inward: Api → Application → Domain; Infrastructure implements Domain interfaces.
- Do not add NuGet packages without asking.

## Domain Modeling

- Aggregates own their invariants. Create them through factory methods (`Task.Create(...)`), never public constructors or object initializers.
- Private setters only. State changes go through intention-revealing methods (`UpdateStatus`, `Assign`, `Complete`).
- Wrap primitives in value objects or strongly-typed IDs (`TaskId`, `Priority`, `DueDate`). Value objects are immutable `sealed record`s that validate in their factory.
- Reference other aggregates by ID, never by object.
- Repository interfaces live in Domain and exist only for aggregate roots.
- Domain has zero references to EF Core, ASP.NET Core, or data annotations.

## C# Style

- `sealed` by default; file-scoped namespaces; nullable reference types on; `record` for DTOs and value objects.
- Guard clauses and early returns instead of nested `if`/`else`. Prefer `ArgumentException.ThrowIfNullOrWhiteSpace` and similar helpers.
- Async all the way: `Async` suffix, pass `CancellationToken`, never `.Result` or `.Wait()`.
- `ILogger<T>` with structured templates (`"Created task {TaskId}"`), never `Console.WriteLine` or string interpolation in log calls.
- Object Calisthenics apply to **domain code**: one level of indentation per method, no `else`, wrap primitives, first-class collections, small classes (< 50 lines), and no more than two instance fields where practical. Relax them for DTOs, tests, and generated code.

## API Layer

- Minimal APIs only, organized as endpoint extension methods (`MapTaskEndpoints`). No `[ApiController]` or `ControllerBase`.
- Endpoints map request DTO → command → Application service → response DTO. No business logic in handlers.
- Return `TypedResults` and Problem Details for errors.

## Verification

Run `dotnet build` and `dotnet test` before declaring a change done. Commit with a Conventional Commit message (`feat:`, `fix:`, `test:`, `refactor:`).
