# Lab 5: Git Workflow & PR Preparation

**Duration**: 15–20 minutes
**Part**: 1 of 2 (Fundamentals)

**Learning Objectives**:
- Use Claude Code's `/review` command for pre-commit code review
- Generate conventional commit messages grounded in actual diffs
- Produce a complete PR description from session context
- Understand when to trust Claude Code's git operations and when to verify

> **Compare to Copilot**: Copilot's `@workspace` can generate PR descriptions and commit messages from chat. Claude Code's `/review` runs a structured code review pass against your actual diff — not just the conversation.

---

## Prerequisites

- Completed Labs 1–4
- Uncommitted changes from Labs 3 and 4 on your personal branch

---

## Step 1: Pre-Commit Code Review (5 min)

Before committing anything, run a code review:

```bash
claude
```

```
/review
```

The `/review` command runs a structured review of your staged and unstaged changes. It will:
- Check for bugs, logic errors, and missing error handling
- Flag code style issues against the conventions in `CLAUDE.md`
- Identify missing tests
- Call out security issues if any

### 1.1 Act on the Review

If `/review` flags real issues:

```
Fix the issues flagged in the review. Don't change anything that wasn't flagged.
```

If everything looks good, move to Step 2.

> **Key practice**: Think of `/review` as a gate before every commit, not a step you do occasionally. It only takes a few seconds and catches things you'll be glad you found before pushing.

---

## Step 2: Stage Changes (3 min)

Verify what you're about to commit:

```bash
git status
git diff --stat
```

Stage the files:

```bash
git add src/     # .NET changes
# or
git add src-springboot/  # Spring Boot changes
git add tests/           # test files
```

Ask Claude Code to verify the staged set makes sense:

```
Look at what's staged (git diff --cached) and tell me if there's anything that shouldn't be committed yet, or anything I forgot to stage.
```

---

## Step 3: Conventional Commit Message (5 min)

Ask Claude Code to generate a commit message:

```
Based on the staged changes (git diff --cached), write a conventional commit message. Follow this format: type(scope): description, then a blank line, then a body explaining what changed and why. Use one of: feat, fix, refactor, test, docs, chore.
```

Claude Code will read the actual diff and produce a message grounded in what changed — not a generic description.

### 3.1 Example Output

```
refactor(domain): modernize LegacyTaskProcessor with guard clauses and extracted methods

Replace nested conditionals with early-return guard clauses throughout LegacyTaskProcessor.
Extract ProcessHighPriority, ProcessOverdue, and ValidateTaskState as private methods.
Add characterization tests covering all existing behavior paths before refactoring.

All 24 characterization tests pass. No logic changes — behavior is preserved.
```

Adjust the message if anything is off, then commit:

```bash
git commit -m "$(cat <<'EOF'
refactor(domain): modernize LegacyTaskProcessor with guard clauses and extracted methods

...your message body...
EOF
)"
```

---

## Step 4: PR Description (5 min)

For a full PR-ready description:

```
Write a complete pull request description for the work done in this branch. Include: a summary of changes, the motivation (why this refactoring matters), a testing section, and a checklist of things to verify during review. Base it on the git log and diff.
```

Claude Code will read `git log` and `git diff origin/main...HEAD` to understand everything on the branch before generating the description.

### 4.1 Review the PR Description

A good Claude Code-generated PR description includes:
- What changed (from the diff)
- Why it changed (from the commit messages and any context Claude Code gathered during the session)
- How it was tested (test file names, pass counts)
- What reviewers should look at closely

Copy the output to your PR when you create it.

---

## Reflection: Git Workflow with Claude Code

### What Claude Code Does Well
- Reading diffs and producing accurate commit messages
- Generating PR descriptions that reflect actual changes (not just the last conversation)
- Running `/review` against the real code, not a summarized version

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

- [ ] `/review` run and any real issues addressed
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

**End of Part 1**

---

**Next Part**: [Part 2 — Advanced Claude Code](lab-06-permissions-and-hooks.md)

**Part 2 begins with**: [Lab 6: Permissions, Safety & Hooks](lab-06-permissions-and-hooks.md)
