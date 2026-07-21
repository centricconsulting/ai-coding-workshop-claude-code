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

Module 07 / Lab 7: `/review` became your pre-commit quality gate —
bugs, style drift, missing tests, and *incidentally* security issues
if `/review` happens to notice them.

**This is the last module of Day 1.** Security deserves a dedicated pass,
not a side effect of a general review.

---

# `/review` Recap

```
/review
```

Runs against your staged and unstaged changes. Checks:
- Bugs, logic errors, missing error handling
- Style drift from `CLAUDE.md`
- Missing tests
- Security issues — **if it happens to notice them**

General-purpose. Security is not its focus.

---

# `/security-review`: Purpose-Built

```
/security-review
```

Specifically looks for:

- **Input validation gaps** — unvalidated data reaching logic or storage
- **Injection** — SQL/command/log injection, unsanitized content
- **Secrets** — hardcoded credentials, tokens, connection strings
- **Broken authorization** — missing or wrong checks on *who* can touch *whose* data

Reads your actual diff. Not a description of it, not a summary — the diff.

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

- Task gets `AssignedToUserId` and `OrganizationId`
- Endpoint reassigns a task to any user, given just a task ID
- **First pass: no check that the requester belongs to the task's organization**

Any caller who knows (or guesses) a task ID from another org can reassign it.

---

# Find It, Fix It

```
/security-review
```

→ Finding: assignment endpoint verifies existence, not authorization.

**Fix**: add a check — requester's organization must match the task's
`OrganizationId` (or `organizationId`) before the reassignment is allowed.
Reject with `403 Forbidden` otherwise.

```
/security-review
```
→ Confirm the finding is gone.

---

# Beyond Generic: Project-Specific Risk

`/security-review` doesn't know TaskManager's specific risk areas.

This lab creates the repo's **first custom security artifact**:

```
.claude/commands/security-checklist.md
```

Layers TaskManager-specific checks on top of the generic review:
- Multi-tenant authorization on every Task read/write path
- Notification content (email/SMS body) treated as untrusted input

---

# Why a Custom Command, Not Tribal Knowledge

**Without it**: "make sure you check the org ID on task endpoints" lives in
one senior engineer's head, repeated in code review comments forever.

**With it**:

```
/security-checklist
```

Same checks, every time, for anyone on the team — encoded once, run automatically.

---

# Where Security Review Fits in PR Prep

| Timing | When it makes sense |
|---|---|
| **Before `/review`** | Catch security issues early, before investing review time in code that has to change anyway |
| **After `/review`** | Final gate right before commit, on top of general quality |
| **Both, at different points** | `/security-checklist` any time you touch task assignment, queries, or notification content — not just at commit time |

For TaskManager: assignment, task queries, notification content →
always get `/security-checklist`, not just `/review`.

---

# Lab 8 Preview

## What You'll Do

1. Recap `/review` vs. `/security-review`
2. Introduce a real cross-tenant authorization gap
3. Run `/security-review`, review the finding, fix the actual issue
4. Build `.claude/commands/security-checklist.md`
5. Discuss where security review sits in PR prep

---

# Lab 8 Starts Here

👉 Open [docs/labs/lab-08-security-review-workflows.md](../../../labs/lab-08-security-review-workflows.md)

**Goal for the next 30 minutes:**
- One real finding, fixed at the root cause
- `.claude/commands/security-checklist.md` created and runnable
- A clear answer to "where does security review fit in PR prep?"

**This is the last lab of Day 1** — Day 1 wrap-up comes next.

---

<!-- Navigation -->
[← 07 Git Workflow](07-git-workflow.md) | **Day 1 Module 08** | [Next: Day 1 Wrap-Up →](09-wrapup-day1.md)
