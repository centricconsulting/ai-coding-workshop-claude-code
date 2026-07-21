# Lab 8: Security Review Workflows

**Duration**: 30 minutes
**Day**: 1 of 2 (Fundamentals)

**Learning Objectives**:
- Distinguish `/review` (general quality gate) from `/security-review` (security-focused deep dive)
- Recognize a realistic authorization vulnerability in a multi-tenant-style domain
- Run `/security-review`, evaluate its finding, and fix the underlying issue
- Build a project-specific slash command that layers domain knowledge on top of a generic review

> **Note**: `/security-review` is a structured pass purpose-built for vulnerability classes (injection, secrets, broken auth, unsafe input handling) rather than general code quality, and it reads your actual diff, not a description of it.

---

## Overview

Lab 7 introduced `/review` as a pre-commit quality gate: bugs, style, missing tests, and — incidentally — security issues if `/review` happens to notice them. This lab goes deeper. You'll introduce a real, realistic vulnerability into TaskManager, run `/security-review` against it, fix what it finds, and then codify TaskManager-specific security knowledge into a custom slash command so future reviews catch domain-specific risks automatically.

This is the last lab of Day 1.

### Choose Your Stack

> Follow sections marked 🔷 for .NET or 🟩 for Spring Boot.

---

## Prerequisites

- Completed Lab 7 — comfortable running `/review` and generating commit messages from diffs
- Personal branch, clean working tree (commit or stash Lab 7's work first)

---

## Part A: `/review` Recap vs. `/security-review` (5 min)

`/review`, from Lab 7, is a general-purpose gate: bugs, missing error handling, style drift from `CLAUDE.md`, missing tests, and it *may* flag an obvious security issue along the way — but that's a side effect, not its focus.

`/security-review` is narrower and deeper. It's purpose-built to look for:
- **Input validation gaps** — unvalidated or under-validated parameters reaching business logic or storage
- **Injection** — SQL/command/log injection, unsanitized content flowing into places that interpret it
- **Secrets** — hardcoded credentials, tokens, or connection strings
- **Broken authentication/authorization** — missing or incorrect checks on who's allowed to do what to whose data

```
/security-review
```

Run it now against your current (clean) branch — with nothing to find, this is just to see the baseline output format before you have something real to review.

---

## Part B: Introduce and Fix a Real Vulnerability (18 min)

### B.1 Introduce a Cross-Tenant Authorization Gap

You're going to add a small, realistic feature to TaskManager — task assignment — with a deliberate flaw: it lets a caller reassign *any* task to *any* user, without checking that the requester and the task belong to the same organization. This is a textbook broken-access-control bug (OWASP A01): the code checks that the *task exists*, but never checks that the *requester is allowed to touch it*.

#### 🔷 .NET Prompt

```
Following the conventions in src/CLAUDE.md, add a task-assignment feature:

1. Add an AssignedToUserId (nullable string) and OrganizationId (string) property
   to the Task entity in src/TaskManager.Domain/Tasks/Task.cs, plus an Assign(string userId)
   method that sets AssignedToUserId.
2. Add a minimal API endpoint PUT /tasks/{id}/assign that accepts a userId in the
   request body and calls Assign on the task, then saves it.

For this first pass, don't add any authorization check beyond confirming the task
exists — I want to see the simplest possible version first.
```

#### 🟩 Spring Boot Prompt

```
Following the conventions in src-springboot/CLAUDE.md, add a task-assignment feature:

1. Add an assignedToUserId (nullable String) and organizationId (String) field to the
   Task entity, plus an assign(String userId) method that sets assignedToUserId.
2. Add a REST endpoint PUT /tasks/{id}/assign that accepts a userId in the request
   body and calls assign on the task, then saves it.

For this first pass, don't add any authorization check beyond confirming the task
exists — I want to see the simplest possible version first.
```

Review what Claude Code proposes before it writes. You should see exactly what was asked for: the endpoint resolves the task by ID and reassigns it — with no check that the caller has any relationship to that task's organization. Any authenticated caller who knows (or guesses) a task ID from another organization can reassign it. That's the vulnerability.

> This is a deliberately introduced training bug — you're building it on purpose so the next steps have something real to find and fix. In a real codebase, this exact pattern (existence check without an ownership/tenancy check) is one of the most common ways broken access control ships to production.

### B.2 Run `/security-review`

```
/security-review
```

Read the finding carefully. A good result names the specific gap: the assignment endpoint verifies the task exists but not that the requester belongs to the task's organization (or is otherwise authorized to reassign it) — cross-tenant/cross-user broken access control.

### B.3 Fix the Underlying Issue

Don't just patch symptoms — ask for the actual authorization check:

#### 🔷 .NET Prompt

```
Fix the finding from /security-review: the PUT /tasks/{id}/assign endpoint doesn't
verify the requester is authorized to reassign the task. Add an authorization check
that confirms the requesting user's organization matches the task's OrganizationId
before allowing the reassignment. Reject with 403 Forbidden otherwise. Follow the
conventions in src/CLAUDE.md.
```

#### 🟩 Spring Boot Prompt

```
Fix the finding from /security-review: the PUT /tasks/{id}/assign endpoint doesn't
verify the requester is authorized to reassign the task. Add an authorization check
that confirms the requesting user's organization matches the task's organizationId
before allowing the reassignment. Reject with 403 Forbidden otherwise. Follow the
conventions in src-springboot/CLAUDE.md.
```

### B.4 Re-run `/security-review` to Confirm

```
/security-review
```

The finding from B.2 should no longer appear. If it still does, or a new related issue shows up (e.g., the organization ID itself trusted from an unvalidated client-supplied field instead of the authenticated session), keep iterating — this is the same fix-then-verify loop as `/review` in Lab 7, just aimed at a security-specific finding.

---

## Part C: Build a Project-Specific Security Command (7 min)

Generic security review is valuable, but it doesn't know TaskManager's specific risk areas: multi-tenant authorization on task operations, and — per the project's domain — notification content handling. You're going to create the repo's first custom security artifact so every future review checks these automatically.

```
Create a custom slash command at .claude/commands/security-checklist.md. It should
layer TaskManager-specific risk areas on top of a generic security review:

- Any endpoint or handler that reads or mutates a Task must verify the requester's
  organization matches the task's OrganizationId (multi-tenant authorization) —
  flag any place that checks existence but not ownership
- Any code that renders or stores notification content (email/SMS body text) must
  treat task titles/descriptions as untrusted input — flag any place user-supplied
  text is interpolated into a message without sanitization
- Anything a generic /security-review would already catch (secrets, injection,
  missing input validation) is still in scope — this command should say to run
  /security-review as a baseline, then apply these project-specific checks on top

Follow the format of Claude Code's built-in slash commands: frontmatter description,
then the prompt body.
```

Review the generated file before it's written — confirm it references the real entity/field names from B.1 (`OrganizationId` / `organizationId`, `AssignedToUserId` / `assignedToUserId`), not generic placeholders.

### C.1 Run It

```
/security-checklist
```

Confirm it runs and produces output that references both the generic security categories and the two TaskManager-specific risk areas.

> **Key practice**: A custom slash command like this is how a team encodes "things every reviewer on this project should know to check" into something that runs the same way every time — not tribal knowledge that lives in one senior engineer's head.

---

## Part D: Where Security Review Fits in PR Prep (part of the 30 min, discussion)

Lab 7 established `/review` as a pre-commit gate. Where does `/security-review` (and now `/security-checklist`) fit relative to it?

- **Before `/review`**: catch the security issue early, before you've invested review time in a version of the code that has to change anyway
- **After `/review`**: some teams treat security as the last gate before staging — `/review` for general quality, then `/security-review` as a final check right before commit
- **Both, at different points**: run `/security-review` (or `/security-checklist`) whenever you touch anything that reads/writes Task data across a boundary — a new endpoint, a new handler — even mid-session, not just at commit time. Run `/review` as the general pre-commit gate regardless.

For TaskManager specifically: any change touching task assignment, task queries, or notification content is exactly the kind of change that should get `/security-checklist`, not just `/review` — because that's precisely the surface area the custom command exists to check.

---

## Success Criteria

- [ ] You can state the difference between `/review` and `/security-review` in one sentence
- [ ] You introduced the cross-tenant assignment vulnerability and confirmed `/security-review` found it
- [ ] You fixed the underlying authorization gap (not just a surface patch) and re-ran `/security-review` clean
- [ ] `.claude/commands/security-checklist.md` exists, references real TaskManager fields, and runs via `/security-checklist`
- [ ] You can explain where security review fits relative to `/review` in the PR-prep flow

---

## Extension Exercise

Introduce a second, different vulnerability class — a notification body that interpolates a task's `Description` directly into an HTML email template without escaping (stored/reflected injection risk). Run `/security-checklist` (not the generic `/security-review`) against it and confirm the TaskManager-specific notification-content check catches it. Fix it, then re-run to confirm.

---

## Troubleshooting

**`/security-review` finds nothing on the deliberately vulnerable code**

Make sure the endpoint code actually matches what was asked for in B.1 — read the diff and confirm there's genuinely no authorization check, only an existence check. If Claude Code added a check anyway (it sometimes will, unprompted, because it's the obviously-correct thing to do), ask it to remove just that check so the lab has something to find, or treat that as a teaching moment: sometimes Claude Code catches the gap before your review tooling does.

**The fix in B.3 breaks existing tests**

That's expected if earlier labs' tests assumed unrestricted access. Ask Claude Code to update the affected tests to include a valid organization/user context, rather than removing the new check.

**`/security-checklist` doesn't seem to reference the real field names**

The command file is static text — if B.1's field names differed from what's shown here (you renamed something), ask Claude Code to update `.claude/commands/security-checklist.md` to match what's actually in the codebase.

**Not sure if a finding is a true positive**

Ask Claude Code directly: "Walk me through an actual exploit scenario for this finding — what would a malicious or careless caller need to do to trigger it?" A finding that can't be turned into a concrete scenario is worth scrutinizing before you spend time fixing it.

---

**End of Day 1**

Nice work — you've gone from first contact with Claude Code through TDD, planning, refactoring, session recovery, git workflow, and security review. See the Day 1 wrap-up for a recap of everything covered.

**Day 2 begins with**: [Lab 9: Permissions, Safety & Hooks](lab-09-permissions-and-hooks.md)
