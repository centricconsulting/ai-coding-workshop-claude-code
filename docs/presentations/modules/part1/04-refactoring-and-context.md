---
marp: true
theme: default
paginate: true
backgroundColor: #fff
---

# Module 04: Refactoring & Context Management

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
But `LegacyTaskProcessor` used standalone skips every validation.

A characterization test will expose this. The refactoring should fix it.

---

# Context Management: The `/compact` Command

A refactoring session builds up a lot of context:
- Files read
- Test output from multiple runs
- Previous versions of the code

```
[████████████████████░░░░]  80% full
```

When context fills, Claude Code starts dropping early turns.
It may forget the conventions it loaded from CLAUDE.md.
It may forget what the tests said 10 turns ago.

**Use `/compact` before you hit the wall, not after.**

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
You may need to re-orient it: *"We're now starting Phase 3.
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

# The Lab 4 Target

By the end of Lab 4, `LegacyTaskProcessor` should:

- Have descriptive parameter names (`data` → `input`, `tp` → `taskType`)
- Replace magic numbers with a `ProcessType` enum (or equivalent)
- Extract the case-inversion logic into a focused private method
- Replace the `Chk()` method with a proper `Validate(string input, int taskType)` method
- Add `ILogger` logging at key points
- Have all characterization tests passing

**You don't need to fix every problem in 45 minutes.**
Complete Phase 1 and Phase 2 cleanly. Phase 3 is the extension.

---

# Lab 4 Preview

## What You'll Do

1. Write characterization tests for `LegacyTaskProcessor` (Phase 1)
2. Commit — tests all pass
3. Ask Claude Code to rename params and extract methods — no logic changes (Phase 2)
4. Confirm tests still pass
5. Commit
6. Start Phase 3 if time permits: fix the `Chk`/`Validate` bug

**Before Phase 3:** use `/compact` to compress the session.

---

# Lab 4: Refactoring & Context Management

👉 Open [docs/labs/lab-04-refactoring-and-context.md](../../../labs/lab-04-refactoring-and-context.md)

**Goal for the next 45 minutes:**
- Characterization tests committed and passing
- At least one rename/extract phase completed
- `/compact` used at least once

---

<!-- Navigation -->
[← 03 TDD](03-tdd-with-claude-code.md) | **Part 1 Module 04** | [Next: Git Workflow →](05-git-workflow.md)
