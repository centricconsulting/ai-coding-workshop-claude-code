# Lab 4: Multi-File Refactoring & Context Management

**Duration**: 45–50 minutes
**Part**: 1 of 2 (Fundamentals)

**Learning Objectives**:
- Use Claude Code to refactor legacy code across multiple files
- Understand context window limits and when to use `/compact`
- Practice the diff-review workflow for large changes
- Recognize when a task is too large for a single conversation

> **Compare to Copilot**: Copilot's Edit mode handles multi-file changes with explicit file selection. Claude Code discovers which files need to change by reading the codebase — you don't have to tell it which files to open.

---

## Overview

The `LegacyTaskProcessor` class exists in both stacks. It's intentionally problematic code — the kind you'd find in any production codebase that grew organically without discipline. You'll use Claude Code to modernize it, following Object Calisthenics and the CLAUDE.md conventions.

### Choose Your Stack

> Follow sections marked 🔷 for .NET or 🟩 for Spring Boot.

---

## Prerequisites

- Completed Labs 1–3
- Personal branch with Lab 3 work committed

---

## Step 1: Understand the Legacy Code (8 min)

Start Claude Code and ask it to analyze the legacy file:

```bash
claude
```

#### 🔷 .NET Prompt

```
Read src/TaskManager.Domain/LegacyTaskProcessor.cs and give me a code review. Identify: code smells, Object Calisthenics violations, naming issues, missing tests, and any correctness bugs. Don't change anything yet.
```

#### 🟩 Spring Boot Prompt

```
Read the LegacyTaskProcessor in src-springboot/ and give me a code review. Identify: code smells, Object Calisthenics violations, naming issues, missing tests, and any correctness bugs. Don't change anything yet.
```

### 1.1 Ask for a Refactoring Plan

```
Based on your analysis, what's the safest order to refactor this? I want to modernize it without introducing bugs. Give me a step-by-step plan.
```

Discuss the plan. This is a good moment to adjust scope — a 45-minute lab should tackle the highest-value changes, not everything.

---

## Step 2: Write Characterization Tests First (10 min)

Before changing anything, capture the current behavior with tests. This is how you refactor safely when there are no existing tests.

```
Before we change any code, write characterization tests that document the current behavior of LegacyTaskProcessor — even the bad parts. These tests should pass against the current implementation and will protect us during refactoring.
```

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

### Phase 3: Extract Methods

```
Extract any code blocks longer than 5–6 lines into well-named private methods. Apply the "one level of indentation" rule where possible. Run the characterization tests.
```

### 3.1 Context Management During Long Tasks

After 3 phases of refactoring, check how long your session has been running. If the conversation feels long or Claude Code seems to be losing track of earlier context, use `/compact` before continuing:

```
/compact
```

`/compact` summarizes the conversation history into a condensed form, freeing up context for more work. Claude Code retains the essential context — which files it changed, what the characterization tests cover — but drops the verbose back-and-forth.

> **Key practice**: Use `/compact` proactively, before you hit the context limit. Waiting until the limit causes abrupt conversation truncation.

---

## Step 4: Update Dependent Code (7 min)

Renaming methods means callers may be broken.

```
Search the codebase for any code that calls LegacyTaskProcessor methods. If anything calls the old method names, update those callers too.
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
| Session has grown long and Claude Code seems to "forget" earlier context | Run `/compact` |
| Claude Code is repeating questions it already answered | Run `/compact` or start a new focused task |
| Task spans >10 files | Consider splitting into sub-tasks |
| Claude Code is repeating itself | Run `/clear` and re-describe the remaining work |

The best practice is **small, focused tasks** with clear scope. Rather than "refactor the whole codebase," break it into phases as you did in this lab.

---

## Success Criteria

- [ ] Characterization tests written and passing before any refactoring
- [ ] LegacyTaskProcessor refactored in at least two phases
- [ ] All characterization tests still pass after refactoring
- [ ] You used `/compact` or understand when to use it
- [ ] You reviewed the full git diff before committing

---

## Extension Exercise

Pick one more Object Calisthenics principle to apply:
- **Wrap all primitives**: wrap `string taskId` in a `TaskId` value object
- **First class collections**: wrap `List<Task>` in a `TaskCollection` type
- **No getters/setters**: replace property access with behavior methods

Write new tests for the wrapped types, then implement.

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

Run `/compact`. If the task is very large, commit what you have, `/clear`, and start a new session for the next phase.

**"File not found" errors after renames**

Claude Code may have renamed a file without updating all references. Run the build to identify missing references:

```
Run dotnet build / mvn compile and fix any compilation errors.
```

---

**Next**: [Lab 5: Git Workflow & PR Preparation](lab-05-git-workflow.md)
