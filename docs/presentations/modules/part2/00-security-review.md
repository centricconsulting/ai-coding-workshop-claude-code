---
marp: true
theme: default
paginate: true
backgroundColor: #fff
---

# Module 08: Security Review Workflows

## From General Quality Gate to Security-Specific Review

---

# Where We Are

Module 07 / Lab 7: `/code-review` (alias `/review`) became your
pre-commit gate for **correctness bugs** — and *incidentally*
security issues if it happens to notice them.

**Day 2 starts here.** Your Day 1 branch commits are the review target.
Security deserves a dedicated pass, not a side effect of a general review.

---

# `/code-review` Recap

```
/code-review [low…max|ultra] [--fix] [--comment] [target]
```

Reviews branch commits + uncommitted changes. Looks for:
- Correctness bugs, broken edge cases
- (higher effort) reuse / simplification cleanups
- Follows `CLAUDE.md` while it reviews
- Security issues — **only if it happens to notice them**

Correctness-focused. Security is not its job.

---

# `/security-review`: Purpose-Built

<style scoped>
section { font-size: 27px; }
</style>

```
/security-review
```

Specifically looks for:

- **Input validation gaps** — unvalidated data reaching logic or storage
- **Injection** — SQL/command/log injection, unsanitized content
- **Secrets** — hardcoded credentials, tokens, connection strings
- **Broken authorization** — missing or wrong checks on *who* can touch *whose* data

Reads your actual diff: **commits on your branch vs. `origin`'s default branch**.

- Needs an `origin` remote (your fork) — `git remote set-head origin --auto` if it errors
- Commit first — uncommitted edits aren't in the branch diff

---

# The Vulnerability Class This Lab Targets

Broken access control (OWASP A01) — specifically:

```
Endpoint checks: "does this task exist?"          ✅
Endpoint checks: "is this caller allowed to        ❌
                   touch THIS task?"
```

**Existence check without an ownership/tenancy check.**

One of the most common ways real broken-access-control bugs ship to production —
and small enough to build and fix inside one lab.

---

# What Lab 8 Builds

```
PUT /tasks/{id}/assign
```

- Task gets **new** `AssignedToUserId` and `OrganizationId` fields (the lab adds them — they don't exist in the starter code)
- Endpoint reassigns a task to any user, given just a task ID
- No auth in the API yet → caller's org is simulated via an `X-Organization-Id` header
- **First pass: no check that the requester belongs to the task's organization**

Any caller who knows a task ID from another org can reassign it.

---

# Will the Generic Review Find It?

```
/security-review
```

→ Usually **not reported**. No auth, unguessable GUID IDs, no list endpoint:
it scores the gap below its confidence bar (3/10 and 7/10 in validation runs).

**Ask it why.** Its reasoning is sound for a generic tool: it weighs how
exploitable something is *today*.

Your team's rule doesn't depend on that:
**every operation on a Task checks that the caller owns it.**
A generic reviewer can't know that rule. A project skill can.

---

# Beyond Generic: Project-Specific Risk

<style scoped>
section { font-size: 25px; }
</style>

`/security-review` doesn't know TaskManager's specific risk areas.

This lab creates the repo's **first custom security artifact** — a skill:

```
.claude/skills/security-checklist/SKILL.md
```

```yaml
---
name: security-checklist
description: TaskManager-specific security review
argument-hint: "[path]"
disable-model-invocation: true   # only runs when you type it
allowed-tools: Read, Grep, Glob, Bash(git diff *)
---
```

Layers TaskManager-specific checks on top of the generic review:
- Multi-tenant authorization on every Task read/write path
- Notification content (email/SMS body) treated as untrusted input

---

# Commands → Skills

Custom slash commands have been **merged into Skills**.

| | `.claude/commands/x.md` | `.claude/skills/x/SKILL.md` |
|---|---|---|
| Creates `/x` | ✅ | ✅ |
| Same frontmatter | ✅ | ✅ |
| Supporting files | ❌ | ✅ (checklists, scripts, templates) |
| Claude can auto-invoke | ✅ unless disabled | ✅ unless `disable-model-invocation: true` |

Old command files still work. New work: **use a skill.**
(Lab 11, later today, goes deep on skills.)

---

# Why a Project Skill, Not Tribal Knowledge

**Without it**: "make sure you check the org ID on task endpoints" lives in
one senior engineer's head, repeated in code review comments forever.

**With it**:

```
/security-checklist
```

Same checks, every time, for anyone on the team — encoded once, run automatically.

→ Flags the gap: *assign checks the task exists, never that the caller owns it*.

**Fix** (403 unless orgs match, behind an `ICurrentUser` abstraction), then
re-run `/security-checklist` and `/security-review`: both clean.

---

# Where Security Review Fits in PR Prep

| Timing | When it makes sense |
|---|---|
| **Before `/code-review`** | Catch security issues early, before investing review time in code that has to change anyway |
| **After `/code-review`** | Final gate right before push/PR, on top of correctness |
| **Both, at different points** | `/security-checklist` any time you touch task assignment, queries, or notification content — not just at commit time |

For TaskManager: assignment, task queries, notification content →
always get `/security-checklist`, not just `/code-review`.

---

# Security in Layers: Team & CI Options

<style scoped>
section { font-size: 22px; }
</style>

| Stage | Tool |
|---|---|
| While Claude writes | **`security-guidance` plugin** — Claude reviews & fixes its own edits automatically |
| On demand | **`/security-review`** — single pass on branch changes |
| On demand, project rules | **`/security-checklist`** — your skill |
| On demand, deep scan | **`claude-security` plugin** — multi-agent repo/diff scan |
| Every PR | **`anthropics/claude-code-security-review`** GitHub Action — comments findings on PRs |

```
/plugin install security-guidance@claude-plugins-official
```

⚠️ Install it *after* Lab 8 — it would fix the deliberate bug before you find it.
⚠️ The Action isn't hardened against prompt injection — trusted PRs only.

---

# Lab 8 Preview

## What You'll Do

1. Recap `/code-review` vs. `/security-review`
2. Add task assignment with a cross-tenant authorization gap; commit it
3. Run `/security-review` and read why it doesn't report the gap
4. Build `.claude/skills/security-checklist/SKILL.md`; use it to find the gap
5. Fix the actual issue, then re-run both checks
6. Discuss where security review sits in PR prep, and the plugin / GitHub Action options

---

# Lab 8 Starts Here

👉 Open [docs/labs/lab-08-security-review-workflows.md](../../../labs/lab-08-security-review-workflows.md)

**Goal for the next 30 minutes:**
- A gap the generic review missed, found by your skill and fixed at the root cause
- `.claude/skills/security-checklist/SKILL.md` created and runnable
- A clear answer to "where does security review fit in PR prep?"

**Next:** Lab 9 moves from *reviewing* for risk to *preventing* it with permissions and hooks.

---

<!-- Navigation -->
[← Day 1 Wrap-Up](../part1/08-wrapup-day1.md) | **Day 2 Module 08** | [Next: 09 Permissions & Hooks →](01-permissions-and-hooks.md)
