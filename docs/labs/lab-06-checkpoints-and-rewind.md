# Lab 6: Checkpoints & Rewind

**Duration**: 20 minutes
**Day**: 1 of 2 (Fundamentals)

**Learning Objectives**:
- Understand checkpoints as Claude Code's automatic, per-turn session safety net
- Distinguish session-level undo (rewind) from commit-level undo (`git`)
- Use rewind to recover from a damaged multi-file edit without touching git
- Choose between the rewind menu's restore options (code, conversation, both, summarize)
- Know what checkpoints *don't* track: Bash-made changes, external edits, most subagent edits
- Know when to reach for rewind vs. `git stash` vs. `git reset --hard` vs. asking Claude Code to undo

> **Note**: Claude Code checkpoints Claude's file edits automatically before each prompt you send, so you can rewind a multi-file, multi-turn change as a single unit, independent of git.

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

Every prompt you send that starts a turn creates a checkpoint: Claude Code snapshots the files its editing tools (Edit, Write, etc.) are about to change. This happens without you asking for it. A checkpoint isn't a commit: it's not in your git history and nobody else sees it. It's saved with the conversation, so it's still there if you `/resume` the session later (local snapshots are cleaned up after about 30 days by default). It exists purely so that *you* can undo what Claude did in *this* session.

### A.2 Rewind

Open the rewind menu with `Esc Esc` (double-tap Escape on an **empty** prompt — if there's text, it clears the text instead) or `/rewind` (aliases `/checkpoint`, `/undo`). The menu lists the prompts you sent; pick one, then pick an action:

| Option | What it does |
|---|---|
| **Restore code and conversation** | Revert both files and chat to just before that prompt |
| **Restore conversation** | Rewind the chat, keep current code |
| **Restore code** | Revert Claude's file edits, keep the chat (Claude still "remembers" what it tried) |
| **Summarize from here** | Compress the conversation from this prompt forward — a targeted `/compact`; files untouched |
| **Summarize up to here** | Compress everything *before* this prompt, keep later messages intact |
| **Never mind** | Back out without changes |

The code options only appear when that checkpoint has tracked file changes to revert.

### A.3 What Checkpoints Don't Track

- **Bash-made changes**: files changed *only* by shell commands (`dotnet format`, `mvn`, `sed`, `rm`, a test that writes files) aren't captured. Use git.
- **External changes**: your own edits in the IDE, or edits from another Claude Code session, normally aren't captured.
- **Subagent edits**: edits made by subagents (including a background `/code-review --fix`) generally aren't restored. Use git.

### A.4 How This Differs from Git

| | Checkpoints / Rewind | `git stash` | `git reset --hard` |
|---|---|---|---|
| Scope | Files Claude edited with its editing tools, in this session | Whatever's uncommitted | Whatever's uncommitted, relative to a commit |
| Granularity | Per prompt | Whole working tree, one shot | Whole working tree, one shot |
| Needs a commit to exist? | No | No | Yes — resets *to* a commit |
| Visible to teammates / in history? | No | No (until you pop it, and even then it's local) | No |
| Survives closing Claude Code? | Yes, if you `/resume` that session (local, time-limited) | Yes, until popped or dropped | N/A — it's already applied |
| Catches Bash / IDE edits? | No | Yes | Yes |

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
Read src-dotnet/src/TaskManager.Infrastructure/Legacy/LegacyTaskProcessor.cs as it stands
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

Let Claude Code run with this. Because the instruction is broad ("across the whole file," "everywhere you see room for it") rather than scoped to one method or one concern, there's a good chance it touches more than you'd have approved turn-by-turn. In a validation run it rewrote most of the file, removed the `Thread.Sleep`, narrowed which exceptions are swallowed, and sealed the class, none of which you asked for.

### B.4 Confirm the Damage

Start with the tests:

```
Run the characterization tests. Did anything break?
```

**They may well still pass. That's the lesson, not a reason to stop.** Characterization tests only pin down the behavior they assert; an over-broad edit can change timing, exception handling, or class shape without failing any of them. So read the diff too:

```bash
git diff --stat
git diff
```

Then ask Claude to account for it:

```
List every behavior change in that edit, including ones the characterization tests don't cover.
```

You're looking for anything you didn't ask for: a failing test, a removed `Thread.Sleep`, changed exception types, a class that's now `sealed`, or methods restructured that had nothing to do with error handling. Whether or not a test failed, you're going to rewind.

### B.5 Rewind to the Last-Good Checkpoint

Trigger rewind:

```
Esc Esc
```

or

```
/rewind
```

Claude Code lists the prompts from this session. Select the **B.3 prompt** (the over-broad "simplify the error handling" request) — restoring to a prompt puts things back to how they were just *before* that prompt ran, which is your B.2 last-good state. Then choose **Restore code and conversation**.

> Try the difference: if you pick **Restore code** instead, the files revert but Claude keeps the conversation — useful when you want it to remember what went wrong and try a narrower approach.

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

> **Key practice**: Rewind undoes *Claude Code's* edits for the session — it does not touch git. If you'd committed between B.2 and the bad edit, that commit would still be there after rewind; rewind only ever restores files Claude changed with its editing tools.

### B.7 What Rewind Can't See (3 min)

Ask Claude Code to make one change with its edit tool and one with a shell command:

```
1. Using your file edit tool, add a one-line comment at the top of LegacyTaskProcessor saying "Reviewed in Lab 6".
2. Then, using a Bash command (echo/append, not your edit tool), create a file named lab6-scratch.txt in the repo root containing "made by bash".
```

Now open the rewind menu, select that prompt, and choose **Restore code**. Check:

```bash
git status
```

The comment in `LegacyTaskProcessor` is gone; `lab6-scratch.txt` is still there. Checkpoints only track Claude's file-editing tools. Delete the scratch file yourself (or with `git clean -n` to preview, then `git clean -f lab6-scratch.txt`).

> The same applies to files your code writes at runtime: `LegacyTaskProcessor` writes `task_{id}.txt` files when it processes type-1 tasks (unless you extracted that in Lab 5 Phase 4), so the characterization tests leave those behind (🔷 in the test project's `bin/` output folder; 🟩 in the `taskmanager-infrastructure/` module directory). Rewind won't remove them.

---

## Part C: Rewind vs. Git vs. Just Asking — Decision Table (5 min)

| Situation | Best tool | Why |
|---|---|---|
| Claude Code's last edit (this turn or a few turns back) is wrong, mid-session | **Rewind** | Fastest, precise, no git ceremony |
| You want to discard everything since your last commit, across any number of turns or tools | **`git reset --hard`** (⚠️ irreversible on uncommitted work) | Rewind is scoped to *this session's* checkpoints; git resets to a real commit regardless of session |
| You want to set uncommitted work aside temporarily, keep it, come back later | **`git stash`** | Rewind discards; stash preserves for later retrieval |
| The last change was small and you'd rather just describe the fix | **Ask Claude Code to undo/fix it directly** | Sometimes faster than rewinding — "revert just the exception handling change, keep everything else" |
| The bad state spans a previous Claude Code *session* | **`/resume` + rewind**, or **Git** | Checkpoints are saved with the conversation, so `/resume` the session and rewind; for anything older or shared, use git |
| The damage came from a Bash command, your IDE, or a subagent | **Git** | Checkpoints don't track those changes |
| The edit was fine but the conversation is bloated | **Rewind → Summarize from here** | Frees context without touching files |
| You're not sure how far back the problem started | **Rewind through checkpoints one at a time**, checking tests at each step | Checkpoints give you fine-grained per-turn history that git commits usually don't |

> **Note**: this repo's `.claude/settings.json` denies `Bash(git reset --hard*)`, so if you ask Claude Code to run it, the request is refused. Run it yourself if you really mean it. That deny rule is one of the things you'll study in Lab 9.

**The general rule**: reach for rewind first when the damage is recent and session-local — it's cheaper and more precise than any git operation. Reach for git when you need to cross a session boundary, need a durable record, or need to preserve rather than discard.

---

## Success Criteria

- [ ] You identified a checkpoint right after Lab 5's refactor completed
- [ ] You caused a deliberate over-broad edit and found what it changed, using the diff as well as the tests
- [ ] You used rewind (not git) to restore to the last-good checkpoint
- [ ] You can name the four restore/summarize options and when each is useful
- [ ] You saw a Bash-made file survive a rewind and can explain why
- [ ] You diffed the working tree before and after rewind to confirm recovery
- [ ] You can explain when rewind is the right tool vs. `git stash` vs. `git reset --hard` vs. asking Claude Code to undo directly

---

## Extension Exercise

Repeat the exercise, but this time make **two** follow-up edits after your last-good checkpoint before rewinding — one that's fine, one that's damaging. Use `/rewind` and inspect the checkpoint list: can you rewind past just the damaging one while keeping the fine one, or does rewind only work by restoring to a single prior point in time? Discuss what that implies about ordering your requests so damaging changes are easy to isolate.

---

## Troubleshooting

**`/rewind` shows no checkpoints, or fewer than expected**

Checkpoints belong to the conversation. If you started a fresh conversation (restarted `claude` without `--resume`/`--continue`, or ran `/clear`), the earlier checkpoints aren't in this list — `/resume` the old session to get them back. After `/clear`, the rewind menu also shows a `/resume <session-id> (previous session)` entry at the top. The code restore options also only appear for prompts where Claude actually edited files.

**Rewind didn't undo a change**

Was it made by a Bash command (formatter, build, `sed`), by you in the IDE, or by a subagent? Those aren't tracked. Use `git diff` / `git checkout -- <file>`.

**Rewind restored the file, but the tests still fail**

Confirm you rewound to the checkpoint from *after* B.2's confirmation, not an earlier one from before Lab 5's refactor was complete. Check `git diff` against your Lab 5 commit (if you made one) to see exactly what state you're in.

**You want to keep part of the "damaging" edit**

Rewind is all-or-nothing per checkpoint. If part of the over-broad change was actually good, rewind first to get back to clean ground, then ask Claude Code for that specific improvement again, scoped narrowly this time.

**Unsure whether something is safe to `git reset --hard`**

Don't guess — run `git status` and `git diff` first, or better, use rewind instead. `git reset --hard` on uncommitted work is not recoverable the way rewind is.

---

**Next**: [Lab 7: Git Workflow & PR Preparation](lab-07-git-workflow.md)
