# Lab 3: TDD with Claude Code

**Duration**: 90–100 minutes (Part 1: 45–50 min · Part 2: ~45 min)
**Day**: 1 of 2 (Fundamentals)

> **Timing note**: This lab now has two parts. Part 1 (TDD a `NotificationService`) is the core exercise. Part 2 (Requirements → Code) takes a user story all the way to a working endpoint. If you're short on time, finish Part 1 and treat Part 2 as homework or a Day 2 warm-up.

**Learning Objectives**:
- Apply the full Red-Green-Refactor TDD cycle using Claude Code
- Understand how Claude Code writes files directly, rather than suggesting completions to accept one at a time
- Review multi-file diffs before accepting changes
- Run tests from within Claude Code and iterate on failures
- Turn a user story into a backlog, then into tested code across every Clean Architecture layer
- Record a design decision as a lightweight Architecture Decision Record (ADR)

---

## Overview

- **Part 1 — TDD Fundamentals**: build a `NotificationService` that sends task notifications via email and SMS, test-first.
- **Part 2 — Requirements → Code**: take a user story about task priorities and due dates, break it into backlog items, and implement one item end-to-end (domain → application → API), tests first.

### Choose Your Stack

> Follow sections marked 🔷 for .NET or 🟩 for Spring Boot. The TDD cycle is identical — only syntax and frameworks differ.

---

## Prerequisites

- Completed Labs 1 and 2
- Personal branch checked out
- `src-dotnet/CLAUDE.md` or `src-springboot/CLAUDE.md` in place
- `src-dotnet/tests/CLAUDE.md` (.NET, written in Lab 2) and `.claude/rules/domain-layer.md` (written in Lab 2, Part C)

---

# Part 1: TDD Fundamentals — NotificationService (45–50 min)

---

## Step 1: Design Phase — Interface First (5 min)

Before writing tests, define what the `NotificationService` should look like.

Start Claude Code:

```bash
claude
```

#### 🔷 .NET Prompt

```
Following the Clean Architecture and conventions in src-dotnet/CLAUDE.md, design an INotificationService interface in the Application layer. It should support:
- Sending an email notification about a specific task
- Sending an SMS notification about a specific task
- Sending both email and SMS together

Don't implement anything yet — just the interface. Show me what you'd create and ask me to confirm before writing any files.
```

#### 🟩 Spring Boot Prompt

```
Following the Clean Architecture and conventions in src-springboot/CLAUDE.md, design a NotificationService interface in the application layer. It should support:
- Sending an email notification about a specific task
- Sending an SMS notification about a specific task
- Sending both email and SMS together

Don't implement anything yet — just the interface. Show me what you'd create and ask me to confirm before writing any files.
```

**Review the proposal.** Ask Claude Code to adjust anything that doesn't fit the codebase. Once satisfied, tell it:

```
Looks good. Go ahead and create the interface file.
```

> **Key practice**: Asking Claude Code to *propose before writing* is a habit worth building. It's the agentic equivalent of "show me the plan before executing."

---

## Step 2: Red Phase — Write Failing Tests (12 min)

Now ask Claude Code to write tests for the interface — **before any implementation exists**.

#### 🔷 .NET Prompt

```
Write a comprehensive xUnit test class for INotificationService following the conventions in src-dotnet/CLAUDE.md and src-dotnet/tests/CLAUDE.md. Cover:
- SendEmailAsync succeeds for a valid task
- SendEmailAsync throws ArgumentNullException for a null task
- SendSmsAsync succeeds for a valid task  
- SendSmsAsync throws ArgumentNullException for a null task
- SendBothAsync calls both email and SMS
- SendBothAsync throws if either channel fails

Use FakeItEasy for any dependencies. Write all the tests but do NOT create the implementation. Place the test file in src-dotnet/tests/TaskManager.UnitTests/.
```

#### 🟩 Spring Boot Prompt

```
Write a comprehensive JUnit 5 test class for NotificationService following the conventions in src-springboot/CLAUDE.md. Cover:
- sendEmail succeeds for a valid task
- sendEmail throws IllegalArgumentException for a null task
- sendSms succeeds for a valid task
- sendSms throws IllegalArgumentException for a null task
- sendBoth calls both email and SMS
- sendBoth throws if either channel fails

Use Mockito for any dependencies. Write all the tests but do NOT create the implementation.
```

### 2.1 Review the Test File

Before Claude Code writes it, examine the proposed content. Ask:

```
Show me the test file before writing it.
```

Check:
- Test names follow the `MethodName_StateUnderTest_ExpectedBehavior` pattern
- AAA structure with blank line separators
- Mocks are used appropriately (not over-mocked)

If satisfied: `Write the test file now.`

### 2.2 Run the Failing Tests

```
Run the tests and show me the failure output.
```

You should see compilation errors or test-not-found errors — this is the **Red** phase. The tests exist but the implementation doesn't.

---

## Step 3: Green Phase — Implement to Pass Tests (15 min)

Now implement `NotificationService` to make the tests pass.

#### 🔷 .NET Prompt

```
Create a NotificationService class that implements INotificationService. Make all the tests in the test file pass. Follow the conventions in src-dotnet/CLAUDE.md. Use constructor injection for any dependencies.
```

#### 🟩 Spring Boot Prompt

```
Create a NotificationService class that implements the NotificationService interface. Make all the tests pass. Follow the conventions in src-springboot/CLAUDE.md. Use constructor injection.
```

### 3.1 Watch Claude Code Plan Before Writing

Claude Code should read the test file, understand what's needed, then write the implementation. You'll see it:
1. Read the test file
2. Read `CLAUDE.md` or `src-dotnet/CLAUDE.md` for conventions
3. Write the implementation

### 3.2 Run the Tests Again

```
Run the tests again. Are they all passing?
```

If some fail, ask Claude Code to fix them:

```
These tests are still failing: [paste failure output]. Fix the implementation.
```

Repeat until all tests pass.

> **Key practice**: Running tests from inside Claude Code creates a tight feedback loop. Claude Code can see the failure output and fix the implementation without you copying/pasting error messages.

---

## Step 4: Refactor Phase (8 min)

With green tests, you can safely refactor.

```
Now that the tests pass, review the NotificationService implementation for code quality. Apply Object Calisthenics principles: guard clauses instead of nested conditions, no abbreviations, wrapped primitives where appropriate. Don't break the tests.
```

### 4.1 Run Tests After Refactoring

```
Run the tests to confirm they still pass after the refactor.
```

This is the **safety net** of TDD — you can refactor confidently because the tests will catch regressions.

---

## Step 5: Review the Full Diff (5 min)

Before committing, review everything Claude Code created.

```bash
git diff
```

You should see:
- The new interface file
- The test file
- The implementation file

Ask Claude Code to summarize the changes:

```
Summarize the changes you made in this session as a conventional commit message.
```


---

# Part 2: Requirements → Code (~45 min)

In Part 1 the interface was handed to you. Real work starts from a requirement. In this part you'll take a user story, decompose it into backlog items with plan mode, and implement one item through every layer — tests first at each step.

> 💡 You'll go deeper on plan mode in Lab 4. Here you only need two things: **Shift+Tab** cycles permission modes until the status bar shows `⏸ plan mode on`, and **`/plan <task>`** enters plan mode and starts planning in one step. In plan mode Claude reads and proposes but doesn't edit files until you approve.

### The User Story

```
As a user, I want to manage tasks with priorities and due dates
so that I can focus on what's important and meet deadlines.

Acceptance criteria:
- Tasks have a priority: Low, Medium, High, or Critical (default: Medium)
- Tasks can have an optional due date, which must be in the future when set
- Tasks can be created with a title, description, priority, and due date via the API
- Invalid priorities and past due dates are rejected with a clear error
- Tasks can be listed, filtered by priority, and sorted by due date
```

---

## Step 6: Decompose the Story in Plan Mode (8 min)

```
/plan Break this user story into 4–6 backlog items that each fit in a small PR. For each item give a title, a one-paragraph description, acceptance criteria, the Clean Architecture layers it touches, and dependencies on other items. The only file to write is docs/backlog/task-priorities.md — no code.

<paste the user story>
```

Review the plan. A good decomposition looks roughly like this:

| # | Item | Layers |
|---|---|---|
| 1 | `Priority` value object | Domain |
| 2 | `DueDate` on `Task` | Domain |
| 3 | Create a task with priority and due date | Application + API (+ Infrastructure for Spring) |
| 4 | List tasks filtered by priority / sorted by due date | Application + API |
| 5 | Validation & error responses | API |

If Claude Code merged or split items differently, choose **No, keep planning** and push back: *"Item 3 is too big for one PR — split it"* or *"Make Priority its own item."* When you're happy, approve with **Yes, manually approve edits** so you can review the backlog file before it's written. We'll implement Items 1–3 now.

> **Why plan mode here?** Decomposition is pure reading and thinking. Plan mode lets Claude explore the domain and API code freely without touching anything, and gives you a single review point before work begins.

---

## Step 7: Priority Value Object — Tests First (8 min)

#### 🔷 .NET Prompt

```
Implement backlog Item 1 using TDD. First write xUnit tests only, in src-dotnet/tests/TaskManager.UnitTests/Domain/PriorityTests.cs, for a Priority value object in src-dotnet/src/TaskManager.Domain/Tasks/:
- Four values: Low, Medium, High, Critical, each with a Name and a numeric Level for ordering (Low=1 … Critical=4)
- Priority.FromName("high") is case-insensitive and returns Priority.High
- FromName with an unknown, null, or empty name throws ArgumentException whose message lists the valid values
- Two priorities with the same name are equal

Run the tests and show me the failures. Do not create Priority yet.
```

Then:

```
Now implement Priority as a sealed record in src-dotnet/src/TaskManager.Domain/Tasks/Priority.cs to make the tests pass. Run the tests.
```

#### 🟩 Spring Boot Prompt

```
Implement backlog Item 1 using TDD. First write JUnit 5 tests only, in taskmanager-domain/src/test/java/com/example/taskmanager/domain/tasks/PriorityTest.java, for a Priority enum in com.example.taskmanager.domain.tasks:
- Four values: LOW, MEDIUM, HIGH, CRITICAL, each with a numeric level for ordering (LOW=1 … CRITICAL=4)
- Priority.fromString("high") is case-insensitive and returns HIGH
- fromString with an unknown, null, or blank value throws IllegalArgumentException whose message lists the valid values

Run the domain module tests and show me the failures. Do not create Priority yet.
```

Then:

```
Now implement Priority in taskmanager-domain to make the tests pass. Run: mvn -q -pl taskmanager-domain test
```

**Check the rule from Lab 2.** Your `.claude/rules/domain-layer.md` matches these paths. Did the error message list the valid values, and is there no logging or framework type in the domain class — without you mentioning it? If not, ask: *"Which rules in .claude/rules/ apply to this file?"*

---

## Step 8: DueDate on Task — Tests First (7 min)

#### 🔷 .NET Prompt

```
Implement backlog Item 2 using TDD. Task currently has Create(title, description) in src-dotnet/src/TaskManager.Domain/Tasks/Task.cs.

First write failing tests in src-dotnet/tests/TaskManager.UnitTests/Domain/TaskTests.cs:
- Task.Create(title, description) defaults Priority to Medium and DueDate to null
- Task.Create(title, description, Priority.High, futureUtcDate) sets both
- A due date in the past throws ArgumentException
- Title null/empty/whitespace throws ArgumentException (this closes the TODO in Create)

Run them and show the failures. Then add Priority and DueDate (DateTime?, UTC) to Task with the minimal change to make them pass, keeping existing callers compiling.
```

#### 🟩 Spring Boot Prompt

```
Implement backlog Item 2 using TDD. Task has create(title), create(title, description) and reconstitute(...) in taskmanager-domain.

First add failing tests to TaskTest.java:
- create(...) defaults priority to MEDIUM and dueDate to null
- create(title, description, Priority.HIGH, futureDate) sets both
- A due date in the past throws IllegalArgumentException
- reconstitute(...) accepts priority and dueDate (no future-date check — persisted tasks may now be overdue)

Run them and show the failures. Then add priority and dueDate (LocalDateTime) to Task with the minimal change. Keep the existing create overloads working.
```

> 🟩 **Spring note**: changing `reconstitute` ripples into `TaskMapper` and `TaskEntity` in `taskmanager-infrastructure`. Ask Claude Code to update them and `TaskMapperTest` in the same step, then run `mvn -q test` from `src-springboot/`. The `dev` profile uses H2 with `create-drop`, so no migration is needed.

---

## Step 9: Create a Task with Priority and Due Date — Application + API (15 min)

This is backlog Item 3. Work **outside-in on tests, inside-out on code**: write the application tests first, then the API, verifying each layer.

#### 🔷 .NET Prompt — Application layer

```
Implement backlog Item 3, application layer, using TDD.

1. Add a CreateTaskCommand record (Title, Description, Priority as string, DueDate as DateTime?) in src-dotnet/src/TaskManager.Application/Tasks/Commands/.
2. In src-dotnet/tests/TaskManager.UnitTests/UnitTest1.cs, replace the skipped AddTaskAsync stubs in TaskServiceTests with real tests for AddTaskAsync(CreateTaskCommand):
   - valid command → task saved via ITaskRepository.AddTaskAsync and SaveChangesAsync, returns the TaskId
   - unknown priority → ArgumentException, nothing saved
   - missing priority → defaults to Medium
3. Run the tests and show the failures.
4. Change TaskService.AddTaskAsync to take the command and make the tests pass. Log the created task ID.

Leave the GetTaskAsync / UpdateTaskStatusAsync stubs alone.
```

> **Design choice**: Claude Code may suggest a dedicated `CreateTaskCommandHandler` instead of a `TaskService` method. Either is fine — pick one and you'll record why in Step 10.

#### 🔷 .NET Prompt — API layer

```
Implement POST /tasks in src-dotnet/src/TaskManager.Api/Extensions/EndpointExtensions.cs. Uncomment and adapt the CreateTaskRequest and TaskResponse records to include priority and dueDate. Return 201 Created with a Location header on success, and 400 ProblemDetails when the service throws ArgumentException. Keep the other commented-out endpoints as they are.
```

Verify it end-to-end:

```bash
dotnet run --project src-dotnet/src/TaskManager.Api
```

In a second terminal (or ask Claude Code to run these with `!` in front):

```bash
curl -i -X POST http://localhost:5215/tasks \
  -H "Content-Type: application/json" \
  -d '{"title":"Ship v2","description":"Release notes","priority":"High","dueDate":"2030-01-15T17:00:00Z"}'

curl -i -X POST http://localhost:5215/tasks \
  -H "Content-Type: application/json" \
  -d '{"title":"Bad","description":"x","priority":"Urgent"}'
```

Expect `201` for the first and `400` (with the valid priorities listed) for the second.

> **Why no integration test here?** The .NET integration test stubs (`src-dotnet/tests/TaskManager.IntegrationTests/UnitTest1.cs`) for `POST /tasks` belong to Lab 5, where you'll write characterization tests before refactoring. For now, unit tests plus a `curl` check are enough.

#### 🟩 Spring Boot Prompt — Application + API

```
Implement backlog Item 3 using TDD.

1. Add failing tests first:
   - TaskServiceTest: createTask with priority and dueDate saves a task with those values; an unknown priority throws IllegalArgumentException
   - TaskApiIntegrationTest (MockMvc): POST /api/tasks with priority "HIGH" and a future dueDate returns 201 and echoes both fields; priority "URGENT" returns 400 ProblemDetail; omitting priority defaults to MEDIUM
2. Run the tests and show the failures.
3. Then implement: add a TaskService.createTask overload taking priority and dueDate, add priority and dueDate to CreateTaskRequest and TaskResponse, and update TaskController's POST handler. GlobalExceptionHandler already maps IllegalArgumentException to 400.
4. Run mvn -q test from src-springboot/.
```

Optional manual check:

```bash
cd src-springboot
mvn -q install -DskipTests
mvn -pl taskmanager-api spring-boot:run -Dspring-boot.run.profiles=dev
# then, in another terminal:
curl -i -X POST http://localhost:8080/api/tasks \
  -H "Content-Type: application/json" \
  -d '{"title":"Ship v2","description":"Release notes","priority":"HIGH","dueDate":"2030-01-15T17:00:00"}'
```

---

## Step 10: Record the Decision — an ADR (5 min)

You made at least one real design decision: how to model `Priority` (value object vs. enum vs. string), and — on .NET — service method vs. command handler. Capture it so the next person (or the next Claude Code session) knows why.

```
Create docs/adr/0001-task-priority-model.md as a short Architecture Decision Record with sections: Title, Status (Accepted), Date, Context, Decision, Consequences (positive and negative), and Alternatives Considered. Base it on what we actually built in this session. Keep it under 40 lines.
```

The `docs/adr/` folder doesn't exist yet — Claude Code will create it. Read the result and correct anything that's wrong; an ADR is only useful if it's accurate.

> **Make it stick**: ask Claude Code to add one line to the project `CLAUDE.md` — *"Architecture decisions are recorded in docs/adr/; read relevant ADRs before changing domain models."* Now future sessions will find it.

---

## Step 11: Review and Commit (2 min)

```bash
git status
git diff --stat
```

You should see the backlog file, domain changes, application/API changes, tests, and the ADR.

```
Run all tests, then summarize this work as a conventional commit message.
```

Commit on your personal branch. Items 4 and 5 stay in the backlog — they make good extension exercises.

---

## Reflection: TDD Workflow Comparison

### Completion-Based TDD Workflow
1. Open file → chat with the assistant → `/explain` → type interface manually
2. Open test file → prompt for each test method → accept suggestions
3. Open impl file → prompt for implementation → accept suggestions
4. Run tests yourself → fix manually or re-prompt

### Claude Code TDD Workflow
1. Describe the design goal → Claude Code proposes interface → you approve → it writes
2. Ask for all tests at once → review proposed file → approve → it writes
3. Ask for implementation → Claude Code reads tests to understand spec → writes impl
4. Ask Claude Code to run tests → it runs, reads output, fixes failures automatically
5. Ask for refactoring → it refactors → re-runs tests → confirms green

**The difference**: With Claude Code, you're directing a workflow, not accepting individual completions. The feedback loop is tighter because Claude Code can run commands and see their output.

### Requirements → Code (Part 2)
- **Plan mode** turned a story into a reviewable backlog *before* any file changed
- **Tests first at each layer** kept each step small and verifiable
- **Path-scoped rules** from Lab 2 enforced domain conventions without restating them
- **The ADR** captured *why* — something tests and code can't express

---

## Success Criteria

**Part 1**
- [ ] `INotificationService` (or `NotificationService` interface) created in the Application layer
- [ ] Test file written **before** the implementation file
- [ ] All tests pass after implementation
- [ ] Tests still pass after refactor
- [ ] You understand the role of CLAUDE.md in keeping generated code consistent

**Part 2**
- [ ] User story decomposed into a backlog in plan mode and saved to `docs/backlog/`
- [ ] `Priority` value object (.NET) or enum (Spring) with tests written first
- [ ] `Task` has a priority and an optional due date, with validation tests
- [ ] Creating a task with priority and due date works through the API (curl for .NET; MockMvc tests for Spring)
- [ ] `docs/adr/0001-task-priority-model.md` exists and accurately reflects your decision

---

## Extension Exercise

Add a new method to the interface: `SendDigestAsync(IEnumerable<Task> tasks)` that sends a summary notification for multiple tasks. Follow the TDD cycle: update the interface, write new tests (they should fail), then implement.

**Part 2 extension**: implement backlog Item 4 — list tasks filtered by priority and sorted by due date (`GET /tasks?priority=High&sort=dueDate`). Start a fresh session with `/clear`, point Claude Code at `docs/backlog/task-priorities.md` and your ADR, and see how much less you need to explain.

---

## Troubleshooting

**Tests not found after Claude Code writes them**

```bash
dotnet build src-dotnet   # or: cd src-springboot && mvn compile
```

Often a build error is hiding the test discovery failure. Fix the build error first.

**Claude Code writes the implementation before tests**

Add to your prompt: "Do not write any implementation files until I confirm the test file looks correct."

**Tests pass without a real implementation (false green)**

This usually means the interface had default implementations or the mocks were set up to return success regardless. Ask:

```
Why are the tests passing? Show me what each test is actually asserting.
```

**Plan mode won't let Claude Code save the backlog file**

That's expected — edits are blocked until you approve the plan. Approve it, or press Shift+Tab to leave plan mode, then ask again.

**.NET: `POST /tasks` returns 404**

`Program.cs` already calls `MapTaskEndpoints()`, so the route is most likely still commented out in `EndpointExtensions.cs`, or the API wasn't restarted. Ask: *"Show me which routes MapTaskEndpoints registers."*

**Spring: tests fail with a column or mapping error after adding priority**

`TaskEntity` and `TaskMapper` need the new fields too. Ask Claude Code to update them and `TaskMapperTest`, then rerun `mvn -q test`.

**Claude Code produces code that doesn't follow CLAUDE.md conventions**

Use `/clear` or restart Claude Code — the root CLAUDE.md loads at startup, so edits made mid-session may not be picked up. Run `/context` to confirm which memory files are loaded.

---

**Next**: [Lab 4: Plan Mode, Permission Modes & Thinking](lab-04-plan-mode-and-extended-thinking.md)
