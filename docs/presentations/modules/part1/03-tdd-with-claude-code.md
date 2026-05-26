---
marp: true
theme: default
paginate: true
backgroundColor: #fff
---

# Module 03: TDD with Claude Code

## Tests as a Feedback Loop, Not an Afterthought

---

# Why TDD Changes with an AI Partner

With hand-written code, you skip TDD because writing tests first is slow.

With Claude Code, the speed argument disappears:

```
You: Write a failing test for NotificationService.SendAsync
     when the task is already complete.

Claude Code: [writes the test, runs it, confirms it fails]
```

**Red is instant.** The discipline of writing failing tests first
is now the same speed as writing passing code.

---

# The Red-Green-Refactor Loop

```
RED: Write a failing test
     ↓
     Claude Code writes the test
     Claude Code runs the tests → confirms failure
     ↓
GREEN: Make it pass
     ↓
     Claude Code writes the implementation
     Claude Code runs the tests → confirms passing
     ↓
REFACTOR: Clean up
     ↓
     Claude Code refactors
     Claude Code runs the tests → confirms still passing
```

**Claude Code runs the tests at every step.** You review the output.

---

# What "Directing a Workflow" Means

**Wrong approach** (accepting completions):
```
You: Write NotificationService with tests
Claude Code: [writes both, you accept or reject]
```

**Right approach** (directing a workflow):
```
You: Write a failing test for the "task not found" case only.
Claude Code: [writes one test, runs it, it fails]

You: Now make it pass with the minimal implementation.
Claude Code: [writes minimal code, runs tests, passes]

You: Now handle the "task already complete" case — test first.
Claude Code: [writes the test, runs it, fails]
...
```

**Small, precise steps. You control the loop.**

---

# Reviewing Multi-File Diffs

Claude Code often changes multiple files in one step.

```
Files modified:
  src/TaskManager.Application/Services/NotificationService.cs  (+45 lines)
  src/TaskManager.Domain/Interfaces/INotificationService.cs    (+8 lines)
  tests/TaskManager.UnitTests/Services/NotificationServiceTests.cs (+62 lines)
```

**Before accepting:**
1. Read the test file — does it test what you asked?
2. Read the implementation — does it match your conventions?
3. Run `git diff` to see the full picture

**Never accept a multi-file change without reviewing all files.**

---

# Tests as a Feedback Loop

Claude Code fixes its own test failures:

```
You: Run the tests

Claude Code:
  Bash(dotnet test) → 2 failures
  
  Failed: NotificationService_SendAsync_WhenTaskNotFound_ThrowsException
  Expected: ArgumentException
  Actual: NullReferenceException
  
  [reads the implementation]
  [fixes the null guard]
  
  Bash(dotnet test) → all passing
  
  Fixed: added null check before repository call.
```

**This loop can run 3–4 times before you see the final result.**
You review the final state — not every intermediate step.

---

# The Discipline: Never Skip the Red Phase

The most common mistake with AI-assisted TDD:

```
You: Write tests and implementation for NotificationService

Claude Code: [writes both at once]
             [runs tests — they all pass]
             [done!]
```

**Problem:** You don't know if the tests actually tested anything.
Tests written alongside the implementation may be shaped by the implementation, not the spec.

**Always ask for the failing test first.**

---

# Naming Tests for Clarity

Claude Code follows your naming convention from CLAUDE.md.
Make sure it's there:

```markdown
## Testing Conventions

Test method naming: `MethodName_StateUnderTest_ExpectedBehavior`
Examples:
  SendAsync_WhenTaskNotFound_ThrowsArgumentException
  SendAsync_WhenAlreadyComplete_ReturnsFailureResult
  SendAsync_WhenValid_PublishesNotificationEvent
```

Good test names are documentation.
Bad test names are `Test1`, `ShouldWork`, `TestSendAsync`.

---

# What Claude Code Tests Well

| Claude Code excels at | Be more hands-on with |
|---|---|
| Happy path tests | Edge cases you haven't described |
| Error handling for named exceptions | Concurrency bugs |
| Boundary conditions (nulls, empty) | Performance tests |
| CRUD handler coverage | Tests that need real data fixtures |
| Following your naming convention | Integration tests with side effects |

**The pattern:** Claude Code is great at covering the cases you name.
You need to think about the cases worth naming.

---

# Integration Tests vs. Unit Tests

**Unit tests** (most of Lab 3):
```csharp
// FakeItEasy for mocks — fast, isolated
var repo = A.Fake<ITaskRepository>();
A.CallTo(() => repo.GetByIdAsync(taskId)).Returns(null);
```

**Integration tests** (stub files in `tests/`):
```csharp
// WebApplicationFactory — full stack, slower
var client = _factory.CreateClient();
var response = await client.PostAsJsonAsync("/tasks", command);
```

The workshop has both. Lab 3 focuses on unit tests.
Integration test stubs are pre-written for later use.

---

# Lab 3 Preview

## What You'll Do

Implement `NotificationService` using TDD:

1. Ask Claude Code to write a failing test for the first case
2. Confirm it fails (`dotnet test` or `mvn test`)
3. Ask for the minimal implementation to make it pass
4. Confirm it passes
5. Repeat for each behavior

## The Key Discipline

Before accepting each step: **read the test Claude Code wrote.**
Does it test what you intended? Is the assertion meaningful?

---

# Lab 3: TDD with Claude Code

👉 Open [docs/labs/lab-03-tdd-with-claude-code.md](../../../labs/lab-03-tdd-with-claude-code.md)

**Goal for the next 45 minutes:**
- `NotificationService` implemented test-first
- All tests named with the workshop convention
- You ran the tests yourself at least once and saw the red → green transition

**Bilingual note:** Labs 3 and 4 have separate .NET and Spring Boot walkthroughs. Use the section for your stack.

---

<!-- Navigation -->
[← 02 CLAUDE.md](02-claude-md-memory.md) | **Part 1 Module 03** | [Next: Refactoring & Context →](04-refactoring-and-context.md)
