# Lab 5: Multi-File Refactoring & Context Management

**Duration**: 45–50 minutes
**Day**: 1 of 2 (Fundamentals)

**Learning Objectives**:
- Use Claude Code to refactor legacy code across multiple files (`LegacyTaskProcessor` and its caller `LegacyBatchImportService`)
- Apply Object Calisthenics rules incrementally, protected by characterization tests
- Inspect context usage with `/context` and manage it with `/compact [instructions]`, `/clear`, and auto-compact
- Check session cost and plan usage with `/usage` (`/cost` is an alias)
- Practice the diff-review workflow for large changes
- Recognize when a task is too large for a single conversation

> **Note**: Claude Code discovers which files need to change by reading the codebase — you don't have to tell it which files to open.

---

## Overview

The `LegacyTaskProcessor` class exists in both stacks. It's intentionally problematic code — the kind you'd find in any production codebase that grew organically without discipline. You'll use Claude Code to modernize it, following Object Calisthenics and the CLAUDE.md conventions.

`LegacyTaskProcessor` has one real consumer: `LegacyBatchImportService`, which calls `Init(true)`, `Chk(...)`, `ProcessTask(...)`, `GetCount()`, and `GetLast()`. Any rename you make has to ripple into that caller — that's what makes this a multi-file refactor.

| | 🔷 .NET | 🟩 Spring Boot |
|---|---|---|
| Legacy class | `src-dotnet/src/TaskManager.Infrastructure/Legacy/LegacyTaskProcessor.cs` | `src-springboot/taskmanager-infrastructure/src/main/java/com/example/taskmanager/infrastructure/legacy/LegacyTaskProcessor.java` |
| Caller | `src-dotnet/src/TaskManager.Infrastructure/Legacy/LegacyBatchImportService.cs` | `src-springboot/.../infrastructure/legacy/LegacyBatchImportService.java` |

### Choose Your Stack

> Follow sections marked 🔷 for .NET or 🟩 for Spring Boot.

---

## Prerequisites

- Completed Labs 1–4
- Personal branch with Lab 3 work committed
- The phased refactor plan from Lab 4 (Plan Mode) — this lab executes it

---

## Step 1: Understand the Legacy Code (8 min)

Start Claude Code and ask it to analyze the legacy file:

```bash
claude
```

#### 🔷 .NET Prompt

```
Read src-dotnet/src/TaskManager.Infrastructure/Legacy/LegacyTaskProcessor.cs and its caller LegacyBatchImportService.cs and give me a code review. Identify: code smells, Object Calisthenics violations, naming issues, missing tests, and any correctness bugs. Don't change anything yet.
```

#### 🟩 Spring Boot Prompt

```
Read the LegacyTaskProcessor and LegacyBatchImportService in src-springboot/ and give me a code review. Identify: code smells, Object Calisthenics violations, naming issues, missing tests, and any correctness bugs. Don't change anything yet.
```

### 1.1 Ask for a Refactoring Plan

```
Based on your analysis, what's the safest order to refactor this? I want to modernize it without introducing bugs. Give me a step-by-step plan.
```

Discuss the plan. This is a good moment to adjust scope — a 45-minute lab should tackle the highest-value changes, not everything.

> **Tip**: If you saved the phased plan from Lab 4, paste it (or reference the file with `@path/to/plan.md`) instead of generating a new one.

### 1.2 Check Your Starting Context

Before the heavy lifting starts, take a baseline:

```
/context
```

`/context` draws a colored grid of how full the context window is and breaks it down by category — system prompt, CLAUDE.md and rules, skills, MCP tools, and messages — with suggestions for anything taking up a lot of space. Note the percentage; you'll compare it after the refactoring phases.

---

## Step 2: Write Characterization Tests First (10 min)

Before changing anything, capture the current behavior with tests. This is how you refactor safely when there are no existing tests.

```
Before we change any code, write characterization tests that document the current behavior of LegacyTaskProcessor and LegacyBatchImportService — even the bad parts (for example, Chk returning false until Init(true) is called). These tests should pass against the current implementation and will protect us during refactoring.
```

🔷 .NET: put them in a new test class under `src-dotnet/tests/TaskManager.UnitTests/` (xUnit v3). 🟩 Spring Boot: put them under `src-springboot/taskmanager-infrastructure/src/test/java/` (JUnit 5).

### 2.1 Run the Tests

```
Run the new tests and confirm they all pass against the current implementation.
```

All tests should be green. If any fail, fix them — they need to match the current behavior, not the desired behavior.

> **Key practice**: Characterization tests are your safety net. They don't test correctness — they test *consistency*. If they pass now and still pass after refactoring, you haven't introduced regressions.

---

## Step 3: Refactor in Phases (20 min)

### Phase 1: Naming and Readability

```
Refactor LegacyTaskProcessor to fix naming issues only — no logic changes. Replace abbreviations with full words, rename unclear variables and methods. Run the characterization tests after each rename to confirm nothing broke.
```

After Claude Code makes the changes, review the diff:

```
Show me a summary of what you changed.
```

Run tests:

```
Run all tests and confirm the characterization tests still pass.
```

### Phase 2: Guard Clauses and Early Returns

```
Now refactor the conditional logic: replace nested if/else blocks with guard clauses and early returns. No logic changes — just restructure the conditionals. Run the characterization tests after.
```

Review, run tests, confirm green.

### Phase 3: Extract Methods (Object Calisthenics)

```
Extract any code blocks longer than 5–6 lines into well-named private methods. Apply these Object Calisthenics rules: one level of indentation per method, don't use the else keyword, and don't abbreviate names. Run the characterization tests.
```

### Phase 4 (if time permits): Modernize

These are the same targets the GitHub Copilot version of this workshop uses for `LegacyTaskProcessor`:

```
Continue refactoring LegacyTaskProcessor:
- Replace the magic numbers for type (1, 2) with an enum
- Use StringBuilder (or a stream) instead of string concatenation in loops
- Stop swallowing exceptions; inject a logger (ILogger<T> / SLF4J) and log failures
- Extract the file write into an ITaskOutputWriter abstraction
- Remove Thread.Sleep
Update LegacyBatchImportService to match. Run the characterization tests after each change.
```

Some of these intentionally change behavior (logging, no sleep). If a characterization test now documents behavior you *meant* to change, update that test deliberately and say so in your commit message.

### 3.1 Context Management During Long Tasks

After 3 phases of refactoring, check how much context you've used:

```
/context
```

Compare it with your baseline from Step 1.2. Every file read, test run, and diff has been added to the conversation. If usage is high or Claude Code seems to be losing track of earlier decisions, compact before continuing — and tell it what to keep:

```
/compact Keep: the characterization test names and what they cover, the files changed so far, and the remaining refactoring phases. Drop: full test output and old versions of the code.
```

`/compact [instructions]` replaces the conversation with a structured summary, freeing context for more work. Your project CLAUDE.md is re-injected from disk afterward, so conventions survive; path-scoped rules and nested CLAUDE.md files reload when Claude next touches a matching file.

Then check what the session has cost so far:

```
/usage
```

`/usage` shows session cost, plan usage limits, and activity stats (`/cost` is an alias).

> **Key practice**: Claude Code **auto-compacts** as you approach the context limit, so a full window won't end your session — but the automatic summary guesses what matters. Running `/compact` yourself at a phase boundary, with focus instructions, keeps the details *you* care about.

---

## Step 4: Update Dependent Code (7 min)

Renaming methods means callers may be broken.

```
Search the codebase for any code that calls LegacyTaskProcessor methods. LegacyBatchImportService is one — are there others? Update every caller to the new method names and run all tests.
```

Claude Code should:
1. Search for usages with grep or file reads
2. Identify affected files
3. Update the callers
4. Run all tests to confirm everything still compiles and passes

This is multi-file coordination — Claude Code handles it naturally because it can both search and write.

---

## Step 5: Review the Complete Diff (5 min)

Before committing, review everything:

```bash
git diff
```

Or ask Claude Code:

```
Summarize all the files you changed in this session and what you did in each one.
```

### 5.1 The Diff Review Discipline

With Claude Code making multi-file changes, **reviewing the diff is not optional**. Scan for:

- Changes you didn't ask for
- Logic changes disguised as renames
- Removed code that might matter
- New dependencies added

If you see anything unexpected:

```
In [filename], you changed [X]. I didn't ask for that. Revert just that change.
```

Claude Code can undo specific changes without reverting the whole session.

---

## Reflection: Context Management

As tasks get larger, context management becomes a skill:

| Signal | Action |
|---|---|
| You're not sure how full the window is | Run `/context` |
| A phase just finished and the next one needs only a summary of it | Run `/compact <what to keep>` |
| Session has grown long and Claude Code seems to "forget" earlier context | Run `/compact` with focus instructions |
| Task spans >10 files | Consider splitting into sub-tasks |
| Switching to an unrelated task, or Claude Code is going in circles | Run `/clear` (new conversation, empty context; CLAUDE.md still loads) and re-describe the remaining work. `/resume` gets the old conversation back |
| Wondering what this is costing | Run `/usage` |

`/compact` keeps continuity but is itself a large request (it reads everything it summarizes). `/clear` is free and gives a clean slate. Pick based on whether you need the history.

The best practice is **small, focused tasks** with clear scope. Rather than "refactor the whole codebase," break it into phases as you did in this lab.

---

## Success Criteria

- [ ] Characterization tests written and passing before any refactoring
- [ ] LegacyTaskProcessor refactored in at least two phases
- [ ] All characterization tests still pass after refactoring
- [ ] `LegacyBatchImportService` updated to match any renames
- [ ] You checked context usage with `/context` and used `/compact` (ideally with instructions) at least once
- [ ] You can explain when to use `/compact` vs `/clear`
- [ ] You reviewed the full git diff before committing

---

## Extension Exercise

Pick one more Object Calisthenics principle to apply:
- **Wrap all primitives**: wrap `string taskId` in a `TaskId` value object
- **First class collections**: wrap `List<Task>` in a `TaskCollection` type
- **No getters/setters**: replace property access with behavior methods

Write new tests for the wrapped types, then implement.

🔷 .NET: add API-level characterization tests. `src-dotnet/tests/TaskManager.IntegrationTests/UnitTest1.cs` contains skipped stubs marked "Lab 5", and `HealthEndpointTests.cs` beside it is a working `WebApplicationFactory<Program>` example to copy. Start with `CreateTask_WithValidData_ShouldReturn201`: you built `POST /tasks` in Lab 3 Part 2, so it can pass today.

```
Using HealthEndpointTests.cs as the pattern, implement CreateTask_WithValidData_ShouldReturn201 in UnitTest1.cs against POST /tasks. Remove its Skip, run it, and show me it passing. Don't change any production code.
```

The other stubs (`GET /tasks/{id}`, `PUT /tasks/{id}/status`, `GET /tasks`) need their endpoints first. Those handlers are still `TODO`s in `EndpointExtensions.cs`, so implement them test-first if you have time.

---

## Troubleshooting

**Claude Code changes logic during "rename only" phase**

Be explicit: "Rename only. Do not change any logic, conditions, or return values — only identifiers."

**Characterization tests fail after refactoring**

This means the refactoring changed behavior. Ask:

```
A characterization test is failing: [paste failure]. Which change caused this? Revert that change and try a different approach.
```

**Context overflow — Claude Code losing track of the task**

Run `/context` to confirm, then `/compact` with instructions about what to keep. If the task is very large, commit what you have, `/clear`, and start the next phase in a fresh conversation (paste in the remaining plan).

**"File not found" errors after renames**

Claude Code may have renamed a file without updating all references. Run the build to identify missing references:

```
Run dotnet build / mvn compile and fix any compilation errors.
```

---

**Next**: [Lab 6: Checkpoints & Rewind](lab-06-checkpoints-and-rewind.md)
