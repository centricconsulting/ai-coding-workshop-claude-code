# Lab 3: TDD with Claude Code

**Duration**: 45–50 minutes
**Day**: 1 of 2 (Fundamentals)

**Learning Objectives**:
- Apply the full Red-Green-Refactor TDD cycle using Claude Code
- Understand how Claude Code writes files directly, rather than suggesting completions to accept one at a time
- Review multi-file diffs before accepting changes
- Run tests from within Claude Code and iterate on failures

---

## Overview

You'll build a `NotificationService` that sends task notifications via email and SMS.

### Choose Your Stack

> Follow sections marked 🔷 for .NET or 🟩 for Spring Boot. The TDD cycle is identical — only syntax and frameworks differ.

---

## Prerequisites

- Completed Labs 1 and 2
- Personal branch checked out
- `src/CLAUDE.md` or `src-springboot/CLAUDE.md` in place

---

## Step 1: Design Phase — Interface First (5 min)

Before writing tests, define what the `NotificationService` should look like.

Start Claude Code:

```bash
claude
```

#### 🔷 .NET Prompt

```
Following the Clean Architecture and conventions in src/CLAUDE.md, design an INotificationService interface in the Application layer. It should support:
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
Write a comprehensive xUnit test class for INotificationService following the conventions in src/CLAUDE.md and tests/CLAUDE.md. Cover:
- SendEmailAsync succeeds for a valid task
- SendEmailAsync throws ArgumentNullException for a null task
- SendSmsAsync succeeds for a valid task  
- SendSmsAsync throws ArgumentNullException for a null task
- SendBothAsync calls both email and SMS
- SendBothAsync throws if either channel fails

Use FakeItEasy for any dependencies. Write all the tests but do NOT create the implementation. Place the test file in tests/TaskManager.UnitTests/.
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
Create a NotificationService class that implements INotificationService. Make all the tests in the test file pass. Follow the conventions in src/CLAUDE.md. Use constructor injection for any dependencies.
```

#### 🟩 Spring Boot Prompt

```
Create a NotificationService class that implements the NotificationService interface. Make all the tests pass. Follow the conventions in src-springboot/CLAUDE.md. Use constructor injection.
```

### 3.1 Watch Claude Code Plan Before Writing

Claude Code should read the test file, understand what's needed, then write the implementation. You'll see it:
1. Read the test file
2. Read `CLAUDE.md` or `src/CLAUDE.md` for conventions
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

---

## Success Criteria

- [ ] `INotificationService` (or `NotificationService` interface) created in the Application layer
- [ ] Test file written **before** the implementation file
- [ ] All tests pass after implementation
- [ ] Tests still pass after refactor
- [ ] You understand the role of CLAUDE.md in keeping generated code consistent

---

## Extension Exercise

Add a new method to the interface: `SendDigestAsync(IEnumerable<Task> tasks)` that sends a summary notification for multiple tasks. Follow the TDD cycle: update the interface, write new tests (they should fail), then implement.

---

## Troubleshooting

**Tests not found after Claude Code writes them**

```bash
dotnet build   # or: mvn compile
```

Often a build error is hiding the test discovery failure. Fix the build error first.

**Claude Code writes the implementation before tests**

Add to your prompt: "Do not write any implementation files until I confirm the test file looks correct."

**Tests pass without a real implementation (false green)**

This usually means the interface had default implementations or the mocks were set up to return success regardless. Ask:

```
Why are the tests passing? Show me what each test is actually asserting.
```

**Claude Code produces code that doesn't follow CLAUDE.md conventions**

Use `/clear` to start a fresh session — CLAUDE.md context loads at startup. If Claude Code was mid-conversation when you updated CLAUDE.md, it may not have the latest version.

---

**Next**: [Lab 4: Plan Mode & Extended Thinking](lab-04-plan-mode-and-extended-thinking.md)
