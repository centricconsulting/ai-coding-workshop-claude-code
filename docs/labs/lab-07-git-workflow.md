# Lab 7: Git Workflow & PR Preparation

**Duration**: 20–25 minutes
**Day**: 1 of 2 (Fundamentals)

**Learning Objectives**:
- Use Claude Code's `/code-review` command (alias `/review`) for a pre-commit review, including effort levels, targets, and `--fix`
- Generate documentation comments (XML docs / Javadoc) for the code you changed
- Generate conventional commit messages grounded in actual diffs
- Produce a complete PR description from session context and open the PR with the `gh` CLI
- Understand when to trust Claude Code's git operations and when to verify

> **Note**: `/code-review` is a bundled skill that reviews your actual diff — not just the conversation — for correctness bugs. `/review` is an alias for it. (Before v2.1.223, `/review` was a different command that reviewed a GitHub PR by number; older blog posts describe that behavior.)

---

## Prerequisites

- Completed Labs 1–6
- Uncommitted changes from Labs 3, 5, and 6 on your personal branch
- `gh` CLI installed and authenticated (`gh auth status`) — needed only for the optional PR steps

---

## Step 1: Pre-Commit Code Review (5 min)

Before committing anything, run a code review:

```bash
claude
```

```
/code-review
```

(`/review` does the same thing.) With no target, `/code-review` reviews your branch's commits ahead of its upstream **plus** your uncommitted changes (if the branch has no upstream, it compares against `main`). It:
- Looks for **correctness bugs** — logic errors, broken edge cases, missing error handling
- Follows your `CLAUDE.md` conventions while it reviews
- At higher effort levels, also suggests reuse/simplification/efficiency cleanups
- Runs as a **background subagent** with its own context window — keep working; findings arrive in the conversation when it's done

It is *not* a security scanner — that's `/security-review` in Lab 8.

### 1.1 Tune the Review

The command takes an effort level, flags, and a target:

```
/code-review [low|medium|high|xhigh|max|ultra] [--fix] [--comment] [--max-findings n|all] [pr#|branch|path]
```

Try a couple of variations:

```
/code-review low src-dotnet/src/TaskManager.Infrastructure/Legacy
```

```
/code-review high
```

| Option | Effect |
|---|---|
| `low` | Fewer, high-confidence findings (fewest false positives) |
| `medium` … `max` | Broader coverage. With no level, it reuses the last level you typed |
| `ultra` | Deep multi-agent review in the cloud (limited free runs, then usage credits) — not needed today |
| `--fix` | Also apply the findings to your working tree (edits go through your permission mode) |
| `--comment` | Post findings as inline comments on a GitHub PR |
| target | A PR number, branch, path, or revision range such as `main...my-branch` |

🟩 Spring Boot: use `src-springboot/taskmanager-infrastructure` as the path target.

### 1.2 Act on the Review

If the review flags real issues, either rerun with `--fix` or ask:

```
Fix the issues flagged in the review. Don't change anything that wasn't flagged.
```

> ⚠️ A background `/code-review --fix` edits files *outside* your session's checkpoints (Lab 6) — `/rewind` won't undo them. Commit or stash first, or use git to revert.

If everything looks good, move to Step 2.

> **Key practice**: Think of `/code-review` as a gate before every commit, not a step you do occasionally. For cleanup-only passes (no bug hunting), the bundled `/simplify` skill is the complement.

---

## Step 2: Document What You Changed (4 min)

Mirroring the documentation step in the Copilot version of this workshop, have Claude Code add doc comments to the public surface you touched:

#### 🔷 .NET Prompt

```
Add XML documentation comments (/// <summary>, <param>, <returns>, <exception>) to the public members of NotificationService and the refactored LegacyTaskProcessor. Describe behavior and edge cases, not implementation. Don't change any code.
```

#### 🟩 Spring Boot Prompt

```
Add Javadoc (@param, @return, @throws) to the public methods of NotificationService and the refactored LegacyTaskProcessor. Describe behavior and edge cases, not implementation. Don't change any code.
```

Review the diff — doc comments that just restate the method name ("Gets the task") add noise; ask Claude Code to make them say something a caller needs to know.

> **Optional**: "Write docs/api/tasks.md documenting the /tasks endpoints that exist on this branch, with example requests and responses." Ask Claude to base it only on endpoints that actually exist.

---

## Step 3: Stage Changes (3 min)

Verify what you're about to commit:

```bash
git status
git diff --stat
```

Stage the files:

```bash
git add src-dotnet/      # .NET changes (src/ and tests/)
# or
git add src-springboot/  # Spring Boot changes
```

Ask Claude Code to verify the staged set makes sense:

```
Look at what's staged (git diff --cached) and tell me if there's anything that shouldn't be committed yet, or anything I forgot to stage.
```

---

## Step 4: Conventional Commit Message (4 min)

Ask Claude Code to generate a commit message:

```
Based on the staged changes (git diff --cached), write a conventional commit message. Follow this format: type(scope): description, then a blank line, then a body explaining what changed and why. Use one of: feat, fix, refactor, test, docs, chore.
```

Claude Code will read the actual diff and produce a message grounded in what changed — not a generic description.

### 4.1 Example Output

```
refactor(legacy): modernize LegacyTaskProcessor with guard clauses and extracted methods

Replace nested conditionals with early-return guard clauses throughout LegacyTaskProcessor.
Rename abbreviated members (Chk, _rdy, _cnt) and extract the case-inversion and
sentence-case transforms into private methods. Update LegacyBatchImportService callers.
Add characterization tests covering all existing behavior paths before refactoring.

All characterization tests pass. No logic changes — behavior is preserved.
```

Adjust the message if anything is off, then commit:

```bash
git commit -m "$(cat <<'EOF'
refactor(legacy): modernize LegacyTaskProcessor with guard clauses and extracted methods

...your message body...
EOF
)"
```

---

## Step 5: PR Description (5 min)

For a full PR-ready description:

```
Write a complete pull request description for the work done in this branch. Include: a summary of changes, the motivation (why this refactoring matters), a testing section, and a checklist of things to verify during review. Base it on the git log and diff.
```

Claude Code will read `git log` and `git diff origin/main...HEAD` to understand everything on the branch before generating the description.

### 5.1 Review the PR Description

A good Claude Code-generated PR description includes:
- What changed (from the diff)
- Why it changed (from the commit messages and any context Claude Code gathered during the session)
- How it was tested (test file names, pass counts)
- What reviewers should look at closely

### 5.2 Open the PR with `gh` (optional)

Claude Code uses the `gh` CLI for GitHub operations. Push your branch to your fork and ask:

```
Save that PR description to pr-body.md, then open a pull request against my fork's main branch with gh pr create --body-file pr-body.md. Show me the command before you run it.
```

Then review the PR itself and post the findings as inline comments:

```
/code-review <pr-number> --comment
```

To work through reviewer feedback later, just ask — there's no dedicated command (the old `/pr-comments` command was removed):

```
Use gh to read the review comments on PR #<number> and address each one. Summarize what you changed per comment.
```

---

## Reflection: Git Workflow with Claude Code

### What Claude Code Does Well
- Reading diffs and producing accurate commit messages
- Generating PR descriptions that reflect actual changes (not just the last conversation)
- Running `/code-review` against the real diff, not a summarized version
- Driving `gh` for PRs, review comments, and CI status

### What to Watch For
- Claude Code can stage and commit files — confirm `git status` before running any git add/commit prompts
- If you ask Claude Code to push, it should ask for confirmation — pushing is irreversible
- Generated commit messages are a *starting point* — edit them if needed

### The Trust-But-Verify Rule
Before running any git command that Claude Code suggests:
1. Read the command
2. Understand what it does
3. Run it yourself or approve the permission prompt

Never approve `git push --force` or `git reset --hard` without thinking carefully.

---

## Success Criteria

- [ ] `/code-review` (or `/review`) run, with at least one effort level or target variation, and real issues addressed
- [ ] Doc comments (XML docs / Javadoc) added to the code you changed
- [ ] Changes committed with a conventional commit message grounded in the actual diff
- [ ] PR description generated from the branch history, not just the last prompt
- [ ] You understand when to verify Claude Code's git operations manually

---

## Extension Exercise

Use Claude Code to generate a CHANGELOG entry from the git log:

```
Read git log --oneline origin/main..HEAD and generate a CHANGELOG entry following Keep a Changelog format (Added, Changed, Fixed sections).
```

---

**Next**: [Lab 8: Security Review Workflows](lab-08-security-review-workflows.md)
