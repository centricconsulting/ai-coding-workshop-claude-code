---
marp: true
theme: default
paginate: true
backgroundColor: #fff
---

# Module 07: Git Workflow & PR Preparation

## `/code-review` as a Quality Gate, Not a Post-Hoc Check

---

# Where We Are

You've implemented `NotificationService` (Lab 3) and refactored
`LegacyTaskProcessor` through at least Phase 2 (Lab 5), with a
checkpoint safety net from Lab 6.

You have uncommitted or recently-committed changes.

**Lab 7 is about what you do before and after the code is done:**
- Code review before committing
- Doc comments for what you changed
- Commit message generation
- PR description from session context, opened with `gh`

---

# The `/code-review` Command

```
/code-review          # /review is an alias
```

A bundled **skill** that reviews **your branch's commits ahead of
upstream + uncommitted changes** (no upstream → compares to `main`):

- Hunts for **correctness bugs**
- Follows your CLAUDE.md conventions
- Higher effort → also reuse / simplification / efficiency cleanups
- Runs as a **background subagent** — findings arrive when done

Not a security scanner — that's `/security-review` (next module).

**This is a pre-commit check — not a post-merge review.**

> Before v2.1.223, `/review` was a separate read-only PR review. Old blog posts describe that.

---

# Tuning `/code-review`

```
/code-review [low|medium|high|xhigh|max|ultra] [--fix] [--comment]
             [--max-findings n|all] [pr#|branch|path]
```

| You want… | Type |
|---|---|
| High-confidence findings only | `/code-review low` |
| Broader coverage | `/code-review high` |
| Just the legacy folder | `/code-review src-dotnet/src/TaskManager.Infrastructure/Legacy` |
| Review and apply fixes | `/code-review --fix` |
| Review a PR, post inline comments | `/code-review 42 --comment` |
| Deep multi-agent cloud review | `/code-review ultra` |

⚠️ Background `--fix` edits aren't covered by `/rewind` — use git.

---

# What `/code-review` Reports

Example output:

```
Reviewed 4 files (branch + uncommitted changes)

1. LegacyTaskProcessor.cs:42 — correctness
   Validate() now returns true before Init() is called; the
   original Chk() returned false. Characterization test
   Chk_BeforeInit_ReturnsFalse was updated to match — was
   this behavior change intended?

2. NotificationService.cs:31 — correctness
   Exception from the sender is caught and swallowed; callers
   can't tell a notification failed.
```

*(Illustrative — your findings will differ.)*

Fix them — or rerun with `--fix`. Then commit.

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

# Documentation Comments

Same idea as the Copilot workshop's `/doc` step — just ask:

```
Add XML documentation comments to the public members of
NotificationService and the refactored LegacyTaskProcessor.
Describe behavior and edge cases. Don't change any code.
```

🟩 Spring Boot: ask for **Javadoc** (`@param`, `@return`, `@throws`).

Review it: a comment that restates the method name is noise.

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

# GitHub via the `gh` CLI

Claude Code drives GitHub through `gh` — no special command needed:

```
Save the PR description to pr-body.md and open a PR against my
fork's main with gh pr create --body-file pr-body.md.
```

```
Use gh to read the review comments on PR #12 and address each one.
```

`/pr-comments` was **removed** — just ask Claude, as above.
`/code-review 12 --comment` posts findings as inline PR comments.

---

# The Fork-First Workflow

This workshop uses a **fork** (not just a branch) because:

- Labs 10, 13, and 15 require a GitHub remote you control
- The GitHub MCP server needs to query your fork's issues
- CI patterns in Lab 13 need to push to your fork

```bash
# Your fork
git remote -v
origin   git@github.com:your-name/ai-coding-workshop-claude-code.git

# Upstream (read-only — never push here)
upstream git@github.com:centricconsulting/ai-coding-workshop-claude-code.git
```

**If you haven't forked yet, do it before Lab 10.**

---

# The Pre-Commit Safety Net

Summary of the full workflow:

```
Make changes
     ↓
/code-review  ← Quality gate. Fix issues before staging.
     ↓
Doc comments for what changed
     ↓
git add <files>
     ↓
"Generate a conventional commit message"
     ↓
git commit -m "..."
     ↓
git push origin workshop/your-name
     ↓
"Write a PR description" → gh pr create
```

**`/code-review` is step 1, not step last.**

---

# Lab 7 Preview

## What You'll Do

1. Stage your changes from Labs 3, 5, and 6
2. Run `/code-review` — read the output, fix at least one issue
3. Add XML docs / Javadoc to what you changed
4. Ask Claude Code to generate a commit message, then commit
5. Ask Claude Code to write a PR description
6. (Optional) Open a PR on your fork with `gh`, then `/code-review <pr#> --comment`

## The Key Experiment

Run `/code-review low`, then `/code-review high`.
Compare: how do coverage and false positives change?

---

# Lab 7: Git Workflow & PR Preparation

👉 Open [docs/labs/lab-07-git-workflow.md](../../../labs/lab-07-git-workflow.md)

**Goal for the next 20 minutes:**
- `/code-review` run at least once on real changes
- Doc comments added
- Commit message generated and committed
- PR description drafted

**This lab is shorter** — it's skill practice, not new concepts.
If you finish early, go back and extend Labs 3 or 5.

---

# Up Next: Security Review

`/code-review` is a correctness gate. The next lab covers the
category it deliberately leaves out — security.

👉 [Lab 8: Security Review Workflows](../../../labs/lab-08-security-review-workflows.md)

Then Day 1 wraps up before we pick up Day 2 with permissions,
MCP, skills, sub-agents, headless mode, and team configuration.

---

<!-- Navigation -->
[← 06 Checkpoints & Rewind](06-checkpoints-and-rewind.md) | **Day 1 Module 07** | [Next: Security Review →](08-security-review.md)
