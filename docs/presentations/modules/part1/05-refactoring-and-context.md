---
marp: true
theme: default
paginate: true
backgroundColor: #fff
---

# Module 05: Refactoring & Context Management

## Large Tasks Need Phases and Checkpoints

---

# The Problem: Refactoring at Scale

LegacyTaskProcessor has several problems:

```csharp
public string ProcessTask(int id, string data, int type, bool flag)
{
    var result = "";
    if (data != null) {
        if (data.Length > 0) {
            if (type == 1) {
                if (flag) {
                    for (int i = 0; i < data.Length; i++) {
    // ... 90 more lines of nested conditionals
```

- Poor naming (`id`, `data`, `type`, `flag`)
- Deep nesting (4+ levels)
- Mixed concerns (string manipulation + file I/O + sleep)
- No logging
- Abbreviated method names (`Chk`, `Init`)

**Asking Claude Code to "fix all of this" in one step is a trap.**

---

# Why One-Shot Refactors Fail

```
You: Refactor LegacyTaskProcessor to follow our conventions.

Claude Code:
  [reads file]
  [rewrites everything at once]
  [tests fail — the behavior changed]
  [tries to fix — creates new problems]
  [tries again — now you're 6 turns in with no passing baseline]
```

**Problems:**
- No characterization baseline — you don't know what was supposed to pass
- Hard to review a 150-line rewrite in one diff
- Context fills up fast on large files
- Mistakes compound

---

# The Phased Approach

```
Phase 1: Characterize
  → Write tests for existing behavior (warts and all)
  → Run them: they should ALL pass
  → Commit

Phase 2: Safe rename + extract
  → Rename params, extract methods — no logic changes
  → Run tests: still all pass
  → Commit

Phase 3: Fix logic
  → Now address the actual bugs and design problems
  → Run tests: pass (or fail meaningfully)
  → Commit

Phase 4: Clean up
  → Logging, guards, remove dead code
  → Run tests: all pass
  → Commit
```

---

# Characterization Tests

Before you change a line of `LegacyTaskProcessor`, you need tests
that document what it actually does — not what it should do.

```csharp
[Fact]
public void ProcessTask_Type1WithFlag_InvertsCase()
{
    var processor = new LegacyTaskProcessor();
    string result = processor.ProcessTask(1, "Hello World", 1, true);
    Assert.Equal("hELLO_wORLD", result);
}
```

**These tests exist to tell you if you accidentally changed behavior.**
They're not testing good design — they're testing current behavior.

---

# The Subtle Bug to Find

`LegacyTaskProcessor` has a bug in `Chk()` (or `chk()`):

```csharp
public bool Chk(string d, int tp)
{
    if (_rdy == true)
    {
        // ... returns true in some cases
    }
    return false;
}
```

`_rdy` starts as `false`. `Chk()` always returns `false`
unless you called `Init(true)` first.

**The caller (`LegacyBatchImportService`) does call `Init(true)` before checking.**
But `LegacyTaskProcessor` used standalone rejects every input.
And every rename you make must ripple into `LegacyBatchImportService` —
that's what makes this a *multi-file* refactor.

A characterization test will expose this. The refactoring should fix it.

---

# Context Management: See It First with `/context`

A refactoring session builds up a lot of context:
- Files read
- Test output from multiple runs
- Previous versions of the code

```
/context
```

Shows a colored grid of the window plus a breakdown: system prompt,
CLAUDE.md / rules, skills, MCP tools, messages — with suggestions.

When the window nears its limit, Claude Code **auto-compacts**:
it summarizes older history so the session keeps going.
But the automatic summary guesses what matters — details from
10 turns ago (like which tests cover which branch) can get lost.

**Compact on your terms, at a phase boundary, before auto-compact does it for you.**

---

# The Context Toolkit

| Command | What it does |
|---|---|
| `/context` | Visualize what's filling the window |
| `/compact [instructions]` | Summarize the conversation; instructions say what to keep |
| `/clear` | New conversation, empty context (CLAUDE.md still loads). Aliases `/reset`, `/new` |
| `/resume` | Return to an earlier conversation (`/continue` is an alias) |
| `/usage` | Session cost, plan limits, activity (`/cost` is an alias) |

```
/compact Keep the characterization test list, files changed, and
remaining phases. Drop raw test output.
```

`/compact` is itself a large request; `/clear` is free.
Need continuity → compact. Starting something new → clear.

---

# When to Use `/compact`

**Good time to compact:**
- After a phase completes (you've committed, tests pass)
- Around 60–70% context usage
- Before starting a new phase with different files

**Bad time to compact:**
- Mid-phase, when Claude Code needs recent context to continue
- When you're in the middle of a failing test loop

**After `/compact`:**
Claude Code has a summary of the session, not the raw history.
Project-root CLAUDE.md is re-injected from disk; path-scoped rules
reload when a matching file is touched again.
You may still need to re-orient it: *"We're now starting Phase 3.
The characterization tests are passing. Focus on `ProcessTask`."*

---

# Reviewing Phase Diffs

At each phase boundary, before committing:

```bash
git diff
```

For Phase 1 (characterization tests only):
- The diff should only add test files
- No changes to `LegacyTaskProcessor.cs`
- If you see changes to the implementation, something went wrong

For Phase 2 (rename only):
- Variable names change
- Method names change
- **Logic does not change** — the structure should be identical

---

# What "Minimal Change" Looks Like

**Phase 2 extract: before**
```csharp
if (type == 1) {
    if (flag) {
        for (int i = 0; i < data.Length; i++) {
            // 15 lines of case-inverting logic
        }
    }
}
```

**Phase 2 extract: after**
```csharp
if (type == ProcessType.Transform) {
    return flag
        ? InvertCase(data)
        : data.ToUpper();
}
```

The logic is identical. The structure is readable. Tests still pass.

---

# The Lab 5 Target

By the end of Lab 5, `LegacyTaskProcessor` should:

- Have descriptive parameter names (`data` → `input`, `tp` → `taskType`)
- Replace magic numbers with a `ProcessType` enum (or equivalent)
- Extract the case-inversion logic into a focused private method
- Replace the `Chk()` method with a proper `Validate(string input, int taskType)` method
- Follow Object Calisthenics: one level of indentation, no `else`, no abbreviations
- Add `ILogger` logging at key points
- Keep `LegacyBatchImportService` compiling and passing against the new names
- Have all characterization tests passing

Stretch (same targets as the Copilot workshop): `StringBuilder`, no swallowed
exceptions, file I/O behind an `ITaskOutputWriter`, wrap primitives, first-class collections.

**You don't need to fix every problem in 45 minutes.**
Complete Phase 1 and Phase 2 cleanly. Phase 3 is the extension.

---

# Lab 5 Preview

## What You'll Do

1. Write characterization tests for `LegacyTaskProcessor` and `LegacyBatchImportService` (Phase 1) — starting from the plan you drafted in Lab 4
2. Commit — tests all pass
3. Ask Claude Code to rename params and extract methods — no logic changes (Phase 2)
4. Confirm tests still pass
5. Commit
6. Start Phase 3 if time permits: fix the `Chk`/`Validate` bug

**Before Phase 3:** check `/context`, then `/compact <what to keep>`.

---

# Lab 5: Refactoring & Context Management

👉 Open [docs/labs/lab-05-refactoring-and-context.md](../../../labs/lab-05-refactoring-and-context.md)

**Goal for the next 60 minutes:**
- Characterization tests committed and passing
- At least one rename/extract phase completed
- `/context` checked and `/compact` (with instructions) used at least once

---

<!-- Navigation -->
[← 04 Plan Mode & Extended Thinking](04-plan-mode-extended-thinking.md) | **Day 1 Module 05** | [Next: Checkpoints & Rewind →](06-checkpoints-and-rewind.md)
