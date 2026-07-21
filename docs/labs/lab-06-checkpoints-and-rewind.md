# Lab 6: Checkpoints & Rewind

**Duration**: 20 minutes
**Day**: 1 of 2 (Fundamentals)

**Learning Objectives**:
- Understand checkpoints as Claude Code's automatic, per-turn session safety net
- Distinguish session-level undo (rewind) from commit-level undo (`git`)
- Use rewind to recover from a damaged multi-file edit without touching git
- Know when to reach for rewind vs. `git stash` vs. `git reset --hard` vs. asking Claude Code to undo

> **Note**: Claude Code checkpoints the working tree automatically at each turn, so you can rewind a multi-file, multi-turn change as a single unit, independent of git.

---

## Overview

Lab 5 left you with a refactored `LegacyTaskProcessor` and passing characterization tests. This lab starts right after that point and deliberately breaks it: you'll ask Claude Code for a follow-up change that's too broad, let it overreach, and then recover — using rewind, not git.

### Choose Your Stack

> Follow sections marked 🔷 for .NET or 🟩 for Spring Boot. Checkpoints and rewind behave identically in both — only the file paths differ.

---

## Prerequisites

- Completed Lab 5 — `LegacyTaskProcessor` refactored, characterization tests passing
- Working tree in the state Lab 5 left it in (ideally committed, but rewind works either way)

---

## Part A: Checkpoints vs. Git (5 min)

### A.1 What a Checkpoint Is

Claude Code automatically saves the state of your working tree at each turn of a session — before it makes edits. This happens without you asking for it. A checkpoint isn't a commit: it's not in your git history, nobody else sees it, and it disappears when the session ends. It exists purely so that *you* can undo what just happened in *this* session.

### A.2 Rewind

Rewind restores your working tree to a prior checkpoint. Trigger it with `Esc Esc` (double-tap Escape) or `/rewind`, then pick which checkpoint to restore to from the list Claude Code shows you.

### A.3 How This Differs from Git

| | Checkpoints / Rewind | `git stash` | `git reset --hard` |
|---|---|---|---|
| Scope | This Claude Code session only | Whatever's uncommitted | Whatever's uncommitted, relative to a commit |
| Granularity | Per-turn (every prompt/response cycle) | Whole working tree, one shot | Whole working tree, one shot |
| Needs a commit to exist? | No | No | Yes — resets *to* a commit |
| Visible to teammates / in history? | No | No (until you pop it, and even then it's local) | No |
| Survives session end? | No | Yes, until popped or dropped | N/A — it's already applied |

Checkpoints are a **session-level** safety net. Git is a **commit-level** safety net. They solve different problems and you'll use both — checkpoints for "that last thing Claude Code did was wrong," git for "I want to go back to a known-good state from an earlier day."

---

## Part B: Break It, Then Rewind (10 min)

### B.1 Confirm Your Starting Point

```bash
git status
git diff --stat
```

You should see Lab 5's refactor — either committed or sitting as uncommitted changes. Either way is fine for this exercise; rewind doesn't care about git state.

Start Claude Code in the same directory:

```bash
claude
```

### B.2 Ask Claude Code to Do Something Small and Correct First

Establish a checkpoint you'll want to return to:

#### 🔷 .NET Prompt

```
Read src/TaskManager.Infrastructure/Legacy/LegacyTaskProcessor.cs as it stands
now, after the Lab 5 refactor. Confirm the characterization tests still pass.
```

#### 🟩 Spring Boot Prompt

```
Read src-springboot/taskmanager-infrastructure/src/main/java/com/example/taskmanager/infrastructure/legacy/LegacyTaskProcessor.java
as it stands now, after the Lab 5 refactor. Confirm the characterization tests still pass.
```

This turn becomes your "last-good" checkpoint — the refactor is done, tests are green, nothing is broken.

### B.3 Ask for an Over-Broad Follow-Up Edit

Now deliberately ask for something vague and sweeping — the kind of prompt that invites overreach:

```
Also simplify the error handling across the whole file. Don't worry about
matching my exact style, just make it cleaner and more consistent everywhere
you see room for it.
```

Let Claude Code run with this. Because the instruction is broad ("across the whole file," "everywhere you see room for it") rather than scoped to one method or one concern, there's a good chance it touches more than you'd have approved turn-by-turn — restructuring exception handling in ways that change behavior, touching methods that didn't need it, or diverging from the characterization tests' expectations.

### B.4 Confirm the Damage

```bash
git diff
```

Or ask Claude Code:

```
Run the characterization tests. Did anything break?
```

You're looking for evidence that the "simplify error handling" edit went further than intended — failing tests, changed exception types, removed logging, or logic that no longer matches what the characterization tests expect.

### B.5 Rewind to the Last-Good Checkpoint

Trigger rewind:

```
Esc Esc
```

or

```
/rewind
```

Claude Code shows you a list of checkpoints from this session. Pick the one from right after B.2 — before the over-broad edit ran.

### B.6 Diff Before/After to Confirm Recovery

```bash
git diff
git status
```

Confirm:
- The over-broad error-handling changes are gone
- The Lab 5 refactor is still intact
- The characterization tests pass again

```
Run the characterization tests again and confirm they're all green.
```

> **Key practice**: Rewind undoes *Claude Code's* edits for the session — it does not touch git. If you'd committed between B.2 and the bad edit, that commit would still be there after rewind; rewind only ever affects the uncommitted, session-scoped state Claude Code changed.

---

## Part C: Rewind vs. Git vs. Just Asking — Decision Table (5 min)

| Situation | Best tool | Why |
|---|---|---|
| Claude Code's last edit (this turn or a few turns back) is wrong, mid-session | **Rewind** | Fastest, precise, no git ceremony |
| You want to discard everything since your last commit, across any number of turns or tools | **`git reset --hard`** (⚠️ irreversible on uncommitted work) | Rewind is scoped to *this session's* checkpoints; git resets to a real commit regardless of session |
| You want to set uncommitted work aside temporarily, keep it, come back later | **`git stash`** | Rewind discards; stash preserves for later retrieval |
| The last change was small and you'd rather just describe the fix | **Ask Claude Code to undo/fix it directly** | Sometimes faster than rewinding — "revert just the exception handling change, keep everything else" |
| The bad state spans a previous Claude Code *session* (you closed and reopened) | **Git** | Checkpoints don't survive session end — only git history does |
| You're not sure how far back the problem started | **Rewind through checkpoints one at a time**, checking tests at each step | Checkpoints give you fine-grained per-turn history that git commits usually don't |

**The general rule**: reach for rewind first when the damage is recent and session-local — it's cheaper and more precise than any git operation. Reach for git when you need to cross a session boundary, need a durable record, or need to preserve rather than discard.

---

## Success Criteria

- [ ] You identified a checkpoint right after Lab 5's refactor completed
- [ ] You caused a deliberate over-broad edit that damaged the refactored file
- [ ] You used rewind (not git) to restore to the last-good checkpoint
- [ ] You diffed the working tree before and after rewind to confirm recovery
- [ ] You can explain when rewind is the right tool vs. `git stash` vs. `git reset --hard` vs. asking Claude Code to undo directly

---

## Extension Exercise

Repeat the exercise, but this time make **two** follow-up edits after your last-good checkpoint before rewinding — one that's fine, one that's damaging. Use `/rewind` and inspect the checkpoint list: can you rewind past just the damaging one while keeping the fine one, or does rewind only work by restoring to a single prior point in time? Discuss what that implies about ordering your requests so damaging changes are easy to isolate.

---

## Troubleshooting

**`/rewind` shows no checkpoints, or fewer than expected**

Checkpoints are created per-turn within the current session. If you started a new session (`claude` was restarted, or you ran `/clear`), earlier checkpoints are gone — this is expected, not a bug.

**Rewind restored the file, but the tests still fail**

Confirm you rewound to the checkpoint from *after* B.2's confirmation, not an earlier one from before Lab 5's refactor was complete. Check `git diff` against your Lab 5 commit (if you made one) to see exactly what state you're in.

**You want to keep part of the "damaging" edit**

Rewind is all-or-nothing per checkpoint. If part of the over-broad change was actually good, rewind first to get back to clean ground, then ask Claude Code for that specific improvement again, scoped narrowly this time.

**Unsure whether something is safe to `git reset --hard`**

Don't guess — run `git status` and `git diff` first, or better, use rewind instead. `git reset --hard` on uncommitted work is not recoverable the way rewind is.

---

**Next**: [Lab 7: Git Workflow & PR Preparation](lab-07-git-workflow.md)
