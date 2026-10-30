---
name: test-data-generator
description: Generates realistic, valid test data (builders, fixtures, xUnit theory data, JUnit parameterized sources, or JSON payloads) for TaskManager domain entities in the .NET or Spring Boot stack. Use when writing tests that need sample tasks, edge-case inputs, or API request bodies.
when_to_use: Triggers include "generate test data", "sample tasks", "test fixtures", "seed data", "theory data", "parameterized test inputs", or a test-strategist plan listing test data needs.
argument-hint: "[entity] [count] [csharp|java|json|theory]"
arguments: [entity, count, format]
allowed-tools: Read Grep Glob
---

# Test Data Generator

Generate test data for the TaskManager domain that goes through the **real factory methods and business methods**, so every generated object respects domain invariants.

Request: $ARGUMENTS

- **Entity:** $entity (default: `Task`)
- **Count:** $count (default: 5)
- **Format:** $format (default: infer from the file being edited; otherwise `csharp`)

## Step 1: Read the current domain model

The domain evolves during the labs (priorities, due dates, assignments, and so on), so read the entity before generating anything:

- .NET: `src/TaskManager.Domain/` (for example `Tasks/Task.cs`, `Tasks/TaskId.cs`, `Tasks/TaskStatus.cs`)
- Spring Boot: `src-springboot/taskmanager-domain/src/main/java/com/example/taskmanager/domain/`

Use only constructors, factory methods, and business methods that actually exist. If the requested entity does not exist, say so and stop. Do not invent types.

## Step 2: Generate data in the requested format

| Format | Output | Template |
|---|---|---|
| `csharp` | Static builder class in the test project's namespace | [templates/TaskTestData.cs](templates/TaskTestData.cs) |
| `java` | Final utility class with static factory methods | [templates/TaskTestData.java](templates/TaskTestData.java) |
| `theory` | xUnit `TheoryData<...>` (.NET) or JUnit `@MethodSource` stream (Spring Boot), whichever stack the target test is in | Same templates, "edge cases" section |
| `json` | Request bodies for the API layer (`CreateTaskRequest` etc.) | Inline |

Adapt the template to the current model rather than copying it verbatim.

## Rules

1. **Factories, not initializers.** Both `Task` types have private constructors. Use `Task.Create(...)` / `Task.create(...)`, then reach other states through business methods (`UpdateStatus`, `start()`, `complete()`, `cancel()`). For persisted state in Spring Boot, `Task.reconstitute(...)` is allowed.
2. **Valid by default.** Titles are non-blank, realistic, and within limits (Spring Boot: title ≤ 200, description ≤ 2000). Put invalid data only in a clearly named edge-case section.
3. **Realistic and varied.** Use believable software-team work items ("Fix login timeout on mobile", "Write ADR for caching strategy"), not "Test 1", "Test 2". Mix statuses and lengths.
4. **Deterministic.** No `Random` without a fixed seed, and no hidden reliance on the current time in assertions. If a time-dependent field exists (due dates), accept a reference time parameter.
5. **Edge cases on request or when `count` ≥ 5:** empty/whitespace title, maximum-length title, unicode and emoji, every status value, and invalid state transitions (for example completing a cancelled task).
6. **Follow stack conventions** in `src/CLAUDE.md` / `src-springboot/CLAUDE.md`: `sealed`/`final`, file-scoped namespaces, records for DTOs, no field injection.

## Output

1. The generated code or JSON in a single fenced block.
2. Where to put it (for example `tests/TaskManager.UnitTests/TestData/TaskTestData.cs` or `src-springboot/taskmanager-domain/src/test/java/.../TaskTestData.java`).
3. One usage example in a test.

Do not write files unless the user asks. This skill only pre-approves read-only tools.

## Examples

```text
/test-data-generator Task 5 csharp
/test-data-generator Task 10 theory
/test-data-generator Task 3 java
/test-data-generator CreateTaskRequest 4 json
```
