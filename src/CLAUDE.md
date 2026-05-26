# .NET TaskManager — Claude Code Context

## Stack

- **.NET 9** with C# 13
- **Minimal APIs** (no controllers)
- **xUnit v3** + **FakeItEasy** for testing
- **Clean Architecture** with four layers

## Architecture Layers

```
TaskManager.Domain          ← Entities, value objects, domain interfaces
TaskManager.Application     ← Commands, queries, handlers, application services
TaskManager.Infrastructure  ← Repository implementations, external service adapters
TaskManager.Api             ← Minimal API endpoints, DI registration, middleware
```

Dependencies flow **inward only**: Api → Application → Domain. Infrastructure implements Domain interfaces.

## C# Conventions

- File-scoped namespaces: `namespace TaskManager.Domain.Entities;`
- Sealed classes by default unless designed for inheritance
- Guard clauses (no `else` branches): throw or return early
- `async`/`await` throughout — no `.Result` or `.Wait()`
- `ILogger<T>` for structured logging, never `Console.Write`
- Records for value objects and DTOs
- Required properties over constructor injection for simple value types
- No abbreviations in identifiers

## Testing Conventions

- xUnit with `[Fact]` and `[Theory]`
- FakeItEasy for mocks: `A.Fake<IRepository>()`
- Arrange / Act / Assert sections with blank line separators
- Test method naming: `MethodName_StateUnderTest_ExpectedBehavior`
- Never test implementation details — test observable behavior
- Integration tests use `WebApplicationFactory<Program>`

## Common Commands

```bash
dotnet build                          # Build all projects
dotnet test                           # Run all tests
dotnet test --filter "Category=Unit"  # Run unit tests only
dotnet run --project src/TaskManager.Api
```

## Key Files

- `src/TaskManager.Domain/Entities/Task.cs` — core aggregate
- `src/TaskManager.Application/Tasks/Commands/` — CQRS command handlers
- `src/TaskManager.Application/Tasks/Queries/` — CQRS query handlers
- `src/TaskManager.Infrastructure/Repositories/` — in-memory implementations
- `src/TaskManager.Api/Program.cs` — DI registration and endpoint mapping
- `src/TaskManager.Infrastructure/Legacy/LegacyTaskProcessor.cs` — intentionally messy, used for Lab 4

## What NOT to Do

- Do not add `[ApiController]` or `ControllerBase` — this project uses Minimal APIs
- Do not use `var` for non-obvious types
- Do not use `null!` suppress — fix the nullability properly
- Do not add `using` statements for namespaces already in global usings (`GlobalUsings.cs`)
