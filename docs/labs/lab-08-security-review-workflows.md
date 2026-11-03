# Lab 8: Security Review Workflows

**Duration**: 30–35 minutes
**Day**: 2 of 2 (Advanced) — first lab of the day

**Learning Objectives**:
- Distinguish `/code-review` (correctness gate, alias `/review`) from `/security-review` (security-focused pass)
- Recognize a realistic authorization vulnerability in a multi-tenant-style domain
- Understand why a generic security review can miss a project-specific rule, and read its reasoning
- Build a project-specific **skill** (`/security-checklist`) that encodes those rules, and use it to find and fix the gap
- Know the team/CI options: the `security-guidance` plugin and the `anthropics/claude-code-security-review` GitHub Action

> **Note**: `/security-review` is a single pass purpose-built for vulnerability classes (injection, secrets, broken auth, unsafe input handling) rather than general code quality. It reviews the **commits on your current branch compared with `origin`'s default branch** — so it needs an `origin` remote, and changes must be committed to show up. It deliberately reports only findings it's confident are exploitable, which is the point of Part B.

---

## Overview

Lab 7 introduced `/code-review` as a pre-commit gate for correctness bugs. Security isn't its focus. This lab goes deeper. You'll add a small, deliberately flawed feature to TaskManager, run `/security-review` against it, and see why a generic reviewer may not report it. Then you'll codify TaskManager's own security rules in a skill, use it to find the flaw, and fix it.

> **About the starter code**: TaskManager's `Task` has no assignment or organization fields, and the API has no authentication. Part B adds both fields and simulates the caller's organization with an `X-Organization-Id` request header. That simplification matters: it's part of why a generic reviewer discounts the flaw.

This is the first lab of Day 2. It reviews the branch you built on Day 1, then sets up Lab 9: first you *review* for risk, then you *prevent* it with permissions and hooks.

### Choose Your Stack

> Follow sections marked 🔷 for .NET or 🟩 for Spring Boot.

---

## Prerequisites

- Completed Lab 7 — comfortable running `/code-review` and generating commit messages from diffs
- Personal branch, clean working tree (commit or stash Lab 7's work first)
- An `origin` remote (your fork) with a known default branch. Check with:

```bash
git remote -v
git rev-parse --abbrev-ref origin/HEAD   # should print origin/main
```

If the second command fails, run `git fetch origin && git remote set-head origin --auto`.

> ⚠️ **Don't install the `security-guidance` plugin until Part F.** It reviews Claude's edits as it works and would fix the deliberate vulnerability in Part B before you get to find it.

---

## Part A: `/code-review` Recap vs. `/security-review` (5 min)

`/code-review` (or `/review`), from Lab 7, hunts for correctness bugs in your diff and follows `CLAUDE.md` conventions. It *may* trip over an obvious security issue along the way — but that's a side effect, not its focus.

`/security-review` is narrower and deeper. It's purpose-built to look for:
- **Input validation gaps** — unvalidated or under-validated parameters reaching business logic or storage
- **Injection** — SQL/command/log injection, unsanitized content flowing into places that interpret it
- **Secrets** — hardcoded credentials, tokens, or connection strings
- **Broken authentication/authorization** — missing or incorrect checks on who's allowed to do what to whose data

It scores each candidate finding and reports only the ones it's confident are real, exploitable vulnerabilities. That keeps the noise down. It also means it judges risk against how the app works *today*, not against rules your team has decided on.

```
/security-review
```

Run it now. It reviews every commit on your branch that isn't on `origin`'s default branch — your Labs 3–7 work — so this is a baseline: see the output format, and note anything it flags in code you've already written.

---

## Part B: Introduce a Cross-Tenant Gap (8 min)

### B.1 Add Task Assignment, Without an Ownership Check

You're going to add a small, realistic feature to TaskManager — task assignment — with a deliberate flaw: it lets a caller reassign *any* organization's task. Tasks belong to the caller's organization when they're created, but the assignment path checks only that the task *exists*, never that it belongs to the caller. This is a textbook broken-access-control bug (OWASP A01).

#### 🔷 .NET Prompt

```
Following the conventions in src-dotnet/CLAUDE.md, add a task-assignment feature:

1. Add an AssignedToUserId (nullable string) and OrganizationId (string) property to
   the Task entity in src-dotnet/src/TaskManager.Domain/Tasks/Task.cs, plus an
   Assign(string userId) method. Add a Task.Create overload that takes an
   organizationId; keep the existing overloads working (default the organization)
   so earlier labs' code still compiles.
2. POST /tasks: read the caller's organization from an X-Organization-Id request
   header and pass it through CreateTaskCommand, so each task belongs to the
   caller's organization.
3. Add TaskService.AssignTaskAsync(taskId, userId, organizationId) that loads the
   task with ITaskRepository.FindByIdAsync, calls Assign, and saves with
   SaveChangesAsync. Add PUT /tasks/{id}/assign in EndpointExtensions.cs that reads
   X-Organization-Id, takes a userId in the body, and calls the service.
   Return 404 if the task doesn't exist.

For this first pass, don't compare the caller's organization with the task's
OrganizationId — I want to see the simplest possible version first.
```

#### 🟩 Spring Boot Prompt

```
Following the conventions in src-springboot/CLAUDE.md, add a task-assignment feature:

1. Add assignedToUserId (nullable String) and organizationId (String) to the domain
   Task in taskmanager-domain, plus an assign(String userId) method. Carry both
   fields through TaskEntity and TaskMapper in taskmanager-infrastructure. Keep the
   existing create overloads and tests working (default the organization).
2. POST /api/tasks: read the caller's organization from an X-Organization-Id
   request header and store it on the new task.
3. Add TaskService.assignTask(taskId, userId, organizationId) that loads the task,
   calls assign, and saves it. Add PUT /api/tasks/{id}/assign to TaskController that
   reads X-Organization-Id, takes a userId in the body, and calls the service.
   Return 404 if the task doesn't exist.

For this first pass, don't compare the caller's organization with the task's
organizationId — I want to see the simplest possible version first.
```

Review what Claude Code proposes before it writes. The assign path receives the caller's organization and then ignores it. Anyone who knows (or obtains) a task ID from another organization can reassign that task. That's the vulnerability.

> This is a deliberately introduced training bug. In a real codebase, this exact pattern (existence check without an ownership/tenancy check) is one of the most common ways broken access control ships to production.

Build to confirm it compiles (`dotnet build src-dotnet` / `mvn -q compile` from `src-springboot`), then **commit it** — `/security-review` reviews committed branch changes. Use a neutral message; a commit that says "insecure" tips the reviewer off:

```bash
git add -A && git commit -m "feat(tasks): assign tasks to users; tasks belong to the caller's organization"
```

### B.2 Run `/security-review` — and Read Why

```
/security-review
```

**Expect it not to report the flaw**, or to mention it only as a candidate it dropped for low confidence. In validation runs it dropped this finding at 3/10 and 7/10 confidence (it reports at 8/10 and above), reasoning that the API has no authentication, task IDs are random GUIDs, and no endpoint lists other organizations' tasks.

Ask it to show its work:

```
Did you consider PUT /tasks/{id}/assign? Walk me through why it was or wasn't reported.
```

That reasoning is sound *for a generic reviewer*: it weighs how exploitable something is right now. But your team has a rule that doesn't depend on today's exploitability: **every operation on a Task must check that the caller owns it.** A generic tool can't know that rule. The next part teaches it.

> If `/security-review` *does* report the gap, great — note it and continue. The skill in Part C is still the reliable, repeatable check.

---

## Part C: Build a Project-Specific Security Skill (8 min)

A generic security review doesn't know TaskManager's specific rules: multi-tenant authorization on every task operation, and — per the project's domain — careful handling of notification content. You'll encode them in the repo's first custom security artifact, so every future review checks them.

> **Commands vs. skills**: Claude Code has merged custom slash commands into **Skills**. A file at `.claude/commands/security-checklist.md` still works and still creates `/security-checklist`, but the recommended form is a skill directory, `.claude/skills/security-checklist/SKILL.md`. Skills use the same frontmatter, can hold supporting files (e.g. a longer checklist alongside `SKILL.md`), and can be invoked by Claude automatically — unless you set `disable-model-invocation: true`, which keeps this one an explicit, run-when-you-type-it command.

```
Create a skill at .claude/skills/security-checklist/SKILL.md. Frontmatter: name
security-checklist, a one-line description, argument-hint "[path]",
disable-model-invocation: true, and allowed-tools limited to Read, Grep, Glob, and
Bash(git diff *) / Bash(git log *). The body should layer TaskManager-specific rules
on top of a generic security review of the branch diff (or of $ARGUMENTS if a path
is given):

- Any endpoint, service, or handler that reads or mutates a Task must verify the
  requester's organization matches the task's OrganizationId (multi-tenant
  authorization) — flag any place that checks existence but not ownership
- Any code that renders or stores notification content (email/SMS body text) must
  treat task titles/descriptions as untrusted input — flag any place user-supplied
  text is interpolated into a message without sanitization
- Report every match even if the app has no authentication yet: these are project
  rules, not exploitability guesses
- Generic issues (secrets, injection, missing input validation) are still in scope;
  tell the user to run /security-review as the baseline
- Report each finding with file:line, which rule, the risk, and a suggested fix
```

Review the generated file before it's written — confirm it references the real entity/field names from B.1 (`OrganizationId` / `organizationId`, `AssignedToUserId` / `assignedToUserId`), not generic placeholders. It should look roughly like:

```markdown
---
name: security-checklist
description: TaskManager-specific security review (multi-tenant authorization, notification content) on top of /security-review
argument-hint: "[path]"
disable-model-invocation: true
allowed-tools: Read, Grep, Glob, Bash(git diff *), Bash(git log *)
---

Review the changes on this branch (or $ARGUMENTS if given) for TaskManager-specific risks.
Report every rule match, even if the app has no authentication yet.
...
```

### C.1 Run It

```
/security-checklist
```

Confirm it shows up in the `/` menu and in `/skills`. Its first finding should be the gap `/security-review` didn't report: the assign path checks that the task exists but never compares the caller's organization with the task's `OrganizationId`. (In validation, it flagged exactly that, with file and line.) Try `/security-checklist src-dotnet/src/TaskManager.Api` (🟩 `src-springboot/taskmanager-api`) to scope it with `$ARGUMENTS`.

Commit `.claude/skills/security-checklist/` — skills in `.claude/skills/` are shared with everyone who clones the repo.

> **Key practice**: A project skill like this is how a team encodes "things every reviewer on this project should know to check" into something that runs the same way every time — not tribal knowledge that lives in one senior engineer's head.

---

## Part D: Fix the Gap and Re-Check (8 min)

### D.1 Fix the Underlying Issue

Don't just patch the endpoint — put the authorization rule where it can't be bypassed:

#### 🔷 .NET Prompt

```
Fix the finding from /security-checklist: TaskService.AssignTaskAsync doesn't verify
the requester is allowed to reassign the task. Confirm the caller's organization
matches the task's OrganizationId before reassigning; reject with 403 Forbidden
otherwise. Read the caller's organization through a small ICurrentUser-style
abstraction (backed by the X-Organization-Id header for now) so it can later be
swapped for authenticated claims. Put the rule in the domain/application layer, not
only in the endpoint, and add tests for the allowed and forbidden cases. Follow the
conventions in src-dotnet/CLAUDE.md.
```

#### 🟩 Spring Boot Prompt

```
Fix the finding from /security-checklist: TaskService.assignTask doesn't verify the
requester is allowed to reassign the task. Confirm the caller's organization matches
the task's organizationId before reassigning; reject with 403 Forbidden otherwise.
There's no Spring Security yet (don't add it), so read the caller's organization
through a small CurrentUser-style abstraction backed by the X-Organization-Id header,
so it can later be swapped for authenticated principals. Put the rule in the
domain/application layer, not only in the controller, and add tests for the allowed
and forbidden cases. Follow the conventions in src-springboot/CLAUDE.md.
```

### D.2 Re-run Both Checks

Commit the fix, then:

```
/security-checklist
```

The multi-tenant finding should be gone. Then run the generic baseline once more:

```
/security-review
```

Expect no high-confidence findings. It may still note that the organization comes from a client-supplied header rather than an authenticated session. That's correct — and it's why the fix used an abstraction. Treat it as a known limitation of the lab (the real fix is authentication, out of scope today). For any *other* new issue, keep iterating: the same fix-then-verify loop as `/code-review` in Lab 7.

---

## Part E: Where Security Review Fits in PR Prep (part of the 30 min, discussion)

Lab 7 established `/code-review` as a pre-commit gate. Where does `/security-review` (and now `/security-checklist`) fit relative to it?

- **This lab's lesson first**: `/security-review` is a strong generic baseline, but it can't know your project's rules. Pair it with `/security-checklist` for anything that touches task data.
- **Before `/code-review`**: catch the security issue early, before you've invested review time in a version of the code that has to change anyway
- **After `/code-review`**: some teams treat security as the last gate before opening the PR — `/code-review` for correctness, then `/security-review` (which reads committed branch changes) right before pushing
- **Both, at different points**: run `/security-checklist` whenever you touch anything that reads/writes Task data across a boundary — a new endpoint, a new handler — even mid-session, not just at PR time. Run `/code-review` as the general pre-commit gate regardless.

For TaskManager specifically: any change touching task assignment, task queries, or notification content is exactly the kind of change that should get `/security-checklist`, not just `/code-review` — because that's precisely the surface area the skill exists to check.

---

## Part F: Team and CI Options (read / discuss, 3 min)

Security review works best in layers. Beyond what you ran today:

| Stage | Tool | Notes |
|---|---|---|
| While Claude writes code | **`security-guidance` plugin** | `/plugin install security-guidance@claude-plugins-official`. Runs automatically; Claude reviews its own changes for injection, unsafe deserialization, etc. and fixes them in-session. (Needs Python 3.) Try it now: install it, then ask Claude to add a similar unprotected endpoint and watch it self-correct. |
| On demand, single pass | **`/security-review`** | What you used today — committed branch changes vs. `origin` |
| On demand, project rules | **`/security-checklist`** (your skill) | Domain knowledge generic tools don't have |
| On demand, deep scan | **`claude-security` plugin** | Multi-agent scan of a whole repo or diff (`/plugin install claude-security@claude-plugins-official`) |
| On every PR | **`anthropics/claude-code-security-review` GitHub Action** | Diff-aware PR scanner that comments findings on the PR; needs a `claude-api-key` secret. Not hardened against prompt injection — only run it on trusted PRs (require approval for outside contributors). Lab 13 covers Claude Code in CI. |

If `/plugin install` reports `Marketplace "claude-plugins-official" not found`, run `/plugin marketplace add anthropics/claude-plugins-official` first.

---

## Success Criteria

- [ ] You can state the difference between `/code-review` and `/security-review` in one sentence
- [ ] You introduced the cross-tenant assignment gap and can explain why `/security-review` did (or didn't) report it
- [ ] `.claude/skills/security-checklist/SKILL.md` exists with `disable-model-invocation: true`, references real TaskManager fields, and found the gap via `/security-checklist`
- [ ] You fixed the underlying authorization gap (not just a surface patch), and both `/security-checklist` and `/security-review` came back clean
- [ ] You can explain where security review fits relative to `/code-review` in the PR-prep flow, and name one team/CI option (plugin or GitHub Action)

---

## Extension Exercise

Introduce a second, different vulnerability class — a notification body (in the `NotificationService` from Lab 3) that interpolates a task's `Description` directly into an HTML email template without escaping (stored/reflected injection risk). Run `/security-checklist` (not the generic `/security-review`) against it and confirm the TaskManager-specific notification-content check catches it. Fix it, then re-run to confirm.

---

## Troubleshooting

**`/security-review` fails with an `ambiguous argument` error, or says there's nothing to review**

It diffs your branch against `origin`'s default branch. Check `git remote -v` shows an `origin`, run `git fetch origin && git remote set-head origin --auto`, and make sure your changes are **committed** — uncommitted edits aren't part of the branch diff.

**`/security-review` finds nothing on the deliberately vulnerable code**

That's expected in this lab (see B.2). First confirm the change is committed, then ask it why it didn't report the assign path, and move on to Part C. If B.1's code accidentally *does* compare organizations (Claude sometimes adds the check unprompted, because it's the obviously correct thing to do), ask it to remove just that check so `/security-checklist` has something to find — or treat it as a teaching moment.

**`/security-checklist` doesn't report the assign gap either**

Check that the skill body says to report every rule match *even without authentication*. Without that line, Claude may apply the same exploitability judgment as `/security-review` and drop it.

**The fix in D.1 breaks existing tests**

That's expected if earlier labs' tests assumed unrestricted access. Ask Claude Code to update the affected tests to include a valid organization/user context, rather than removing the new check.

**`/security-checklist` doesn't seem to reference the real field names**

The skill file is static text — if B.1's field names differed from what's shown here (you renamed something), ask Claude Code to update `.claude/skills/security-checklist/SKILL.md` to match what's actually in the codebase.

**`/security-checklist` doesn't appear in the `/` menu**

Check the path is exactly `.claude/skills/security-checklist/SKILL.md` (uppercase `SKILL.md`). Skills reload automatically when edited, but a brand-new top-level skills directory may need `/reload-skills` or a restart. If you also have `.claude/commands/security-checklist.md`, delete one — keep the skill.

**Claude fixed the vulnerability before you could review it**

If you installed the `security-guidance` plugin early, it reviews Claude's edits as it works. Disable it with `/plugin` for the lab, or treat it as a preview of Part F.

**Not sure if a finding is a true positive**

Ask Claude Code directly: "Walk me through an actual exploit scenario for this finding — what would a malicious or careless caller need to do to trigger it?" A finding that can't be turned into a concrete scenario is worth scrutinizing before you spend time fixing it.

---

**Next**: [Lab 9: Permissions, Safety & Hooks](lab-09-permissions-and-hooks.md)
