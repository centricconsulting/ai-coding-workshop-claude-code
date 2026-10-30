# Lab 8: Security Review Workflows

**Duration**: 30–35 minutes
**Day**: 1 of 2 (Fundamentals)

**Learning Objectives**:
- Distinguish `/code-review` (correctness gate, alias `/review`) from `/security-review` (security-focused pass)
- Recognize a realistic authorization vulnerability in a multi-tenant-style domain
- Run `/security-review`, evaluate its finding, and fix the underlying issue
- Build a project-specific **skill** (`/security-checklist`) that layers domain knowledge on top of a generic review
- Know the team/CI options: the `security-guidance` plugin and the `anthropics/claude-code-security-review` GitHub Action

> **Note**: `/security-review` is a single pass purpose-built for vulnerability classes (injection, secrets, broken auth, unsafe input handling) rather than general code quality. It reviews the **commits on your current branch compared with `origin`'s default branch** — so it needs an `origin` remote, and changes must be committed to show up.

---

## Overview

Lab 7 introduced `/code-review` as a pre-commit gate for correctness bugs. Security isn't its focus. This lab goes deeper. You'll add a small, deliberately flawed feature to TaskManager, run `/security-review` against it, fix what it finds, and then codify TaskManager-specific security knowledge into a skill so future reviews catch domain-specific risks automatically.

> **About the starter code**: TaskManager's `Task` has no assignment or organization fields, and the API has no authentication. Part B adds both fields explicitly, and simulates the caller's identity with request headers. That simplification is itself a security smell — expect `/security-review` to point it out.

This is the last lab of Day 1.

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

> ⚠️ **Don't install the `security-guidance` plugin until Part E.** It reviews Claude's edits as it works and would fix the deliberate vulnerability in Part B before you get to find it.

---

## Part A: `/code-review` Recap vs. `/security-review` (5 min)

`/code-review` (or `/review`), from Lab 7, hunts for correctness bugs in your diff and follows `CLAUDE.md` conventions. It *may* trip over an obvious security issue along the way — but that's a side effect, not its focus.

`/security-review` is narrower and deeper. It's purpose-built to look for:
- **Input validation gaps** — unvalidated or under-validated parameters reaching business logic or storage
- **Injection** — SQL/command/log injection, unsanitized content flowing into places that interpret it
- **Secrets** — hardcoded credentials, tokens, or connection strings
- **Broken authentication/authorization** — missing or incorrect checks on who's allowed to do what to whose data

```
/security-review
```

Run it now. It reviews every commit on your branch that isn't on `origin`'s default branch — your Labs 3–7 work — so this is a baseline: see the output format, and note anything it flags in code you've already written.

---

## Part B: Introduce and Fix a Real Vulnerability (18 min)

### B.1 Introduce a Cross-Tenant Authorization Gap

You're going to add a small, realistic feature to TaskManager — task assignment — with a deliberate flaw: it lets a caller reassign *any* task to *any* user, without checking that the requester and the task belong to the same organization. Neither the fields nor the endpoint exist yet; the prompts below create them. This is a textbook broken-access-control bug (OWASP A01): the code checks that the *task exists*, but never checks that the *requester is allowed to touch it*.

#### 🔷 .NET Prompt

```
Following the conventions in src-dotnet/CLAUDE.md, add a task-assignment feature:

1. Add an AssignedToUserId (nullable string) and OrganizationId (string) property
   to the Task entity in src-dotnet/src/TaskManager.Domain/Tasks/Task.cs, plus an Assign(string userId)
   method that sets AssignedToUserId. Keep existing constructors/factory methods working
   (default OrganizationId if needed) so earlier labs' code still compiles.
2. Add a minimal API endpoint PUT /tasks/{id}/assign in
   src-dotnet/src/TaskManager.Api/Extensions/EndpointExtensions.cs that accepts a userId in the
   request body, loads the task with ITaskRepository.FindByIdAsync, calls Assign, and
   saves with SaveChangesAsync. Return 404 if the task doesn't exist.

There's no authentication in this API yet. For this first pass, don't add any
authorization check beyond confirming the task exists — I want to see the simplest
possible version first.
```

#### 🟩 Spring Boot Prompt

```
Following the conventions in src-springboot/CLAUDE.md, add a task-assignment feature:

1. Add an assignedToUserId (nullable String) and organizationId (String) field to the
   domain Task in taskmanager-domain, plus an assign(String userId) method that sets
   assignedToUserId. Carry both fields through TaskEntity and TaskMapper in
   taskmanager-infrastructure. Keep existing constructors/tests working.
2. Add PUT /api/tasks/{id}/assign to TaskController that accepts a userId in the request
   body, loads the task, calls assign, and saves it. Return 404 if it doesn't exist.

There's no authentication in this API yet. For this first pass, don't add any
authorization check beyond confirming the task exists — I want to see the simplest
possible version first.
```

Review what Claude Code proposes before it writes. You should see exactly what was asked for: the endpoint resolves the task by ID and reassigns it — with no check that the caller has any relationship to that task's organization. Any authenticated caller who knows (or guesses) a task ID from another organization can reassign it. That's the vulnerability.

> This is a deliberately introduced training bug — you're building it on purpose so the next steps have something real to find and fix. In a real codebase, this exact pattern (existence check without an ownership/tenancy check) is one of the most common ways broken access control ships to production.

Build to confirm it compiles (`dotnet build src-dotnet` / `mvn -q compile` from `src-springboot`), then **commit it** to your personal branch — `/security-review` reviews committed branch changes:

```bash
git add -A && git commit -m "feat(tasks): add task assignment endpoint (lab 8, intentionally insecure)"
```

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
before allowing the reassignment. Reject with 403 Forbidden otherwise. There's no
auth middleware yet, so read the caller's organization from an X-Organization-Id
request header behind a small ICurrentUser-style abstraction, so it can later be
swapped for authenticated claims. Put the rule in the domain/application layer, not
only in the endpoint, and add tests for the allowed and forbidden cases. Follow the
conventions in src-dotnet/CLAUDE.md.
```

#### 🟩 Spring Boot Prompt

```
Fix the finding from /security-review: the PUT /tasks/{id}/assign endpoint doesn't
verify the requester is authorized to reassign the task. Add an authorization check
that confirms the requesting user's organization matches the task's organizationId
before allowing the reassignment. Reject with 403 Forbidden otherwise. There's no
Spring Security yet (don't add it), so read the caller's organization from an
X-Organization-Id request header behind a small CurrentUser-style abstraction, so it
can later be swapped for authenticated principals. Put the rule in the
domain/application layer, not only in the controller, and add tests for the allowed
and forbidden cases. Follow the conventions in src-springboot/CLAUDE.md.
```

### B.4 Re-run `/security-review` to Confirm

Commit the fix, then:

```
/security-review
```

The finding from B.2 should no longer appear. Expect a related finding instead: the organization ID is now trusted from a client-supplied header rather than an authenticated session. That's correct — and it's why the fix used an abstraction. Discuss it as a known limitation of the lab (real fix: authentication, out of scope today) rather than iterating on it. For any *other* new issue, keep iterating — this is the same fix-then-verify loop as `/code-review` in Lab 7, just aimed at a security-specific finding.

---

## Part C: Build a Project-Specific Security Skill (7 min)

Generic security review is valuable, but it doesn't know TaskManager's specific risk areas: multi-tenant authorization on task operations, and — per the project's domain — notification content handling. You're going to create the repo's first custom security artifact so every future review checks these automatically.

> **Commands vs. skills**: Claude Code has merged custom slash commands into **Skills**. A file at `.claude/commands/security-checklist.md` still works and still creates `/security-checklist`, but the recommended form is a skill directory, `.claude/skills/security-checklist/SKILL.md`. Skills use the same frontmatter, can hold supporting files (e.g. a longer checklist alongside `SKILL.md`), and can be invoked by Claude automatically — unless you set `disable-model-invocation: true`, which keeps this one an explicit, run-when-you-type-it command.

```
Create a skill at .claude/skills/security-checklist/SKILL.md. Frontmatter: name
security-checklist, a one-line description, argument-hint "[path]",
disable-model-invocation: true, and allowed-tools limited to Read, Grep, Glob, and
Bash(git diff *) / Bash(git log *). The body should layer TaskManager-specific risk
areas on top of a generic security review of the branch diff (or of $ARGUMENTS if a
path is given):

- Any endpoint or handler that reads or mutates a Task must verify the requester's
  organization matches the task's OrganizationId (multi-tenant authorization) —
  flag any place that checks existence but not ownership
- Any code that renders or stores notification content (email/SMS body text) must
  treat task titles/descriptions as untrusted input — flag any place user-supplied
  text is interpolated into a message without sanitization
- Anything a generic /security-review would already catch (secrets, injection,
  missing input validation) is still in scope — the skill should tell the user to run
  /security-review as a baseline, then apply these project-specific checks on top
- Report each finding with file:line, risk, and a suggested fix
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
Assume /security-review has already been run as a baseline.
...
```

### C.1 Run It

```
/security-checklist
```

Confirm it shows up in the `/` menu and in `/skills`, runs, and produces output that references both the generic security categories and the two TaskManager-specific risk areas. Try `/security-checklist src-dotnet/src/TaskManager.Api` (🟩 `src-springboot/taskmanager-api`) to scope it with `$ARGUMENTS`.

Commit `.claude/skills/security-checklist/` — skills in `.claude/skills/` are shared with everyone who clones the repo.

> **Key practice**: A project skill like this is how a team encodes "things every reviewer on this project should know to check" into something that runs the same way every time — not tribal knowledge that lives in one senior engineer's head.

---

## Part D: Where Security Review Fits in PR Prep (part of the 30 min, discussion)

Lab 7 established `/code-review` as a pre-commit gate. Where does `/security-review` (and now `/security-checklist`) fit relative to it?

- **Before `/code-review`**: catch the security issue early, before you've invested review time in a version of the code that has to change anyway
- **After `/code-review`**: some teams treat security as the last gate before opening the PR — `/code-review` for correctness, then `/security-review` (which reads committed branch changes) right before pushing
- **Both, at different points**: run `/security-checklist` whenever you touch anything that reads/writes Task data across a boundary — a new endpoint, a new handler — even mid-session, not just at PR time. Run `/code-review` as the general pre-commit gate regardless.

For TaskManager specifically: any change touching task assignment, task queries, or notification content is exactly the kind of change that should get `/security-checklist`, not just `/code-review` — because that's precisely the surface area the skill exists to check.

---

## Part E: Team and CI Options (read / discuss, 3 min)

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
- [ ] You introduced the cross-tenant assignment vulnerability and confirmed `/security-review` found it
- [ ] You fixed the underlying authorization gap (not just a surface patch) and re-ran `/security-review` clean
- [ ] `.claude/skills/security-checklist/SKILL.md` exists with `disable-model-invocation: true`, references real TaskManager fields, and runs via `/security-checklist`
- [ ] You can explain where security review fits relative to `/code-review` in the PR-prep flow, and name one team/CI option (plugin or GitHub Action)

---

## Extension Exercise

Introduce a second, different vulnerability class — a notification body (in the `NotificationService` from Lab 3) that interpolates a task's `Description` directly into an HTML email template without escaping (stored/reflected injection risk). Run `/security-checklist` (not the generic `/security-review`) against it and confirm the TaskManager-specific notification-content check catches it. Fix it, then re-run to confirm.

---

## Troubleshooting

**`/security-review` fails with an `ambiguous argument` error, or says there's nothing to review**

It diffs your branch against `origin`'s default branch. Check `git remote -v` shows an `origin`, run `git fetch origin && git remote set-head origin --auto`, and make sure your changes are **committed** — uncommitted edits aren't part of the branch diff.

**`/security-review` finds nothing on the deliberately vulnerable code**

Confirm the change is committed (see above). Then make sure the endpoint code actually matches what was asked for in B.1 — read the diff and confirm there's genuinely no authorization check, only an existence check. If Claude Code added a check anyway (it sometimes will, unprompted, because it's the obviously-correct thing to do), ask it to remove just that check so the lab has something to find, or treat that as a teaching moment: sometimes Claude Code catches the gap before your review tooling does.

**The fix in B.3 breaks existing tests**

That's expected if earlier labs' tests assumed unrestricted access. Ask Claude Code to update the affected tests to include a valid organization/user context, rather than removing the new check.

**`/security-checklist` doesn't seem to reference the real field names**

The skill file is static text — if B.1's field names differed from what's shown here (you renamed something), ask Claude Code to update `.claude/skills/security-checklist/SKILL.md` to match what's actually in the codebase.

**`/security-checklist` doesn't appear in the `/` menu**

Check the path is exactly `.claude/skills/security-checklist/SKILL.md` (uppercase `SKILL.md`). Skills reload automatically when edited, but a brand-new top-level skills directory may need `/reload-skills` or a restart. If you also have `.claude/commands/security-checklist.md`, delete one — keep the skill.

**Claude fixed the vulnerability before you could review it**

If you installed the `security-guidance` plugin early, it reviews Claude's edits as it works. Disable it with `/plugin` for the lab, or treat it as a preview of Part E.

**Not sure if a finding is a true positive**

Ask Claude Code directly: "Walk me through an actual exploit scenario for this finding — what would a malicious or careless caller need to do to trigger it?" A finding that can't be turned into a concrete scenario is worth scrutinizing before you spend time fixing it.

---

**End of Day 1**

Nice work — you've gone from first contact with Claude Code through TDD, planning, refactoring, context management, session recovery, git workflow, and security review. See the Day 1 wrap-up for a recap of everything covered.

**Day 2 begins with**: [Lab 9: Permissions, Safety & Hooks](lab-09-permissions-and-hooks.md)
