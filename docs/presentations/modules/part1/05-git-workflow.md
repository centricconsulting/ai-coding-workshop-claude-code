---
marp: true
theme: default
paginate: true
backgroundColor: #fff
---

# Module 05: Git Workflow & PR Preparation

## `/review` as a Quality Gate, Not a Post-Hoc Check

---

# Where We Are

You've implemented `NotificationService` (Lab 3) and refactored
`LegacyTaskProcessor` through at least Phase 2 (Lab 4).

You have uncommitted or recently-committed changes.

**Lab 5 is about what you do before and after the code is done:**
- Code review before committing
- Commit message generation
- PR description from session context

---

# The `/review` Command

```
/review
```

Claude Code reviews **your current staged and unstaged changes**:

- Bug risks
- Convention violations from CLAUDE.md
- Missing test coverage
- Security issues (SQL injection, unvalidated input, etc.)
- Naming and structural problems

**This is a pre-commit check — not a post-merge review.**
Run it before `git add`, not after `git push`.

---

# What `/review` Looks For

Example output:

```
Reviewing changes in 3 files...

⚠ NotificationService.cs:47
  Potential null reference: task.AssignedTo is nullable
  but used without null check before calling .Email

⚠ NotificationServiceTests.cs
  Only 2 of 4 public methods have test coverage.
  Missing: SendBatchAsync, GetPendingCount

✓ Naming and conventions: consistent with src/CLAUDE.md
✓ Async/await: correct throughout
✓ No obvious security issues
```

Fix the warnings. Then commit.

---

# Conventional Commit Messages

Instead of:
```
git commit -m "fixed stuff"
git commit -m "wip"
git commit -m "NotificationService"
```

Ask Claude Code:
```
Generate a conventional commit message for my staged changes.
```

Claude Code reads `git diff --staged` and produces:

```
feat(notifications): add NotificationService with async send

Implements INotificationService for email dispatch.
Covers task-complete and overdue triggers.
All unit tests passing.
```

---

# The Commit Message Types

| Type | When to Use |
|---|---|
| `feat:` | New functionality |
| `fix:` | Bug fix |
| `refactor:` | Code change with no behavior change |
| `test:` | Adding or fixing tests |
| `docs:` | Documentation only |
| `chore:` | Build, deps, tooling |

**Scope** (optional) narrows it: `feat(tasks):`, `fix(notifications):`

Claude Code will pick the right type if you ask it to.

---

# PR Descriptions from Session Context

After a long session, Claude Code knows:
- What you changed and why
- What tests you wrote
- What decisions you made along the way
- What still needs to be done

```
Write a PR description for my changes. Include:
- Summary of what changed
- Motivation
- What I tested
- A reviewer checklist
```

This is dramatically faster than writing it from memory after
you've context-switched away from the code.

---

# The Fork-First Workflow

This workshop uses a **fork** (not just a branch) because:

- Labs 7, 9, and 10 require a GitHub remote you control
- The GitHub MCP server needs to query your fork's issues
- CI patterns in Lab 9 need to push to your fork

```bash
# Your fork
git remote -v
origin   git@github.com:your-name/ai-coding-workshop-claude-code.git

# Upstream (read-only — never push here)
upstream git@github.com:centricconsulting/ai-coding-workshop-claude-code.git
```

**If you haven't forked yet, do it before Lab 7.**

---

# The Pre-Commit Safety Net

Summary of the full workflow:

```
Make changes
     ↓
/review  ← Quality gate. Fix issues before staging.
     ↓
git add <files>
     ↓
"Generate a conventional commit message"
     ↓
git commit -m "..."
     ↓
git push origin workshop/your-name
     ↓
"Write a PR description"
```

**`/review` is step 1, not step last.**

---

# Lab 5 Preview

## What You'll Do

1. Stage your changes from Labs 3 and 4
2. Run `/review` — read the output, fix at least one issue
3. Ask Claude Code to generate a commit message
4. Commit
5. Ask Claude Code to write a PR description
6. (Optional) Open an actual PR against your fork

## The Key Experiment

Run `/review` before staging. See what it finds.
Then stage and run it again. See if the output changes.

---

# Lab 5: Git Workflow & PR Preparation

👉 Open [docs/labs/lab-05-git-workflow.md](../../../labs/lab-05-git-workflow.md)

**Goal for the next 15 minutes:**
- `/review` run at least once on real changes
- Commit message generated and committed
- PR description drafted

**This lab is shorter** — it's skill practice, not new concepts.
If you finish early, go back and extend Labs 3 or 4.

---

# Part 1 Complete

After Lab 5, you have the fundamentals:

| Skill | Where You Practiced It |
|---|---|
| Agentic model | Lab 1 |
| CLAUDE.md project memory | Lab 2 |
| TDD with Claude Code | Lab 3 |
| Phased refactoring + context | Lab 4 |
| Git workflow integration | Lab 5 |

**Part 2** covers the platform layer: permissions, hooks, MCP, sub-agents,
headless mode, and team configuration.

---

<!-- Navigation -->
[← 04 Refactoring](04-refactoring-and-context.md) | **Part 1 Module 05** | [Next: Part 1 Wrap-Up →](06-wrapup-part1.md)
