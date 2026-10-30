---
marp: true
theme: default
paginate: true
backgroundColor: #fff
---

# Module 06: Checkpoints & Rewind

## A Session-Level Safety Net, Separate from Git

---

# Where We Are

Module 05 / Lab 5: you refactored `LegacyTaskProcessor` in phases, with characterization tests as your safety net at each step.

That safety net was tests + your own review discipline.

**What happens when a follow-up edit overreaches and you need to undo it — right now, mid-session, without a git ceremony?**

That's what checkpoints and rewind are for.

---

# What a Checkpoint Is

Every prompt you send creates a checkpoint: Claude Code snapshots
the files its **edit tools** change. You don't ask for this. It just happens.

A checkpoint is **not** a commit:

- Not in git history
- Not visible to teammates
- Saved with the conversation — still there after `/resume` (local, ~30-day retention)

It exists purely so *you* can undo what Claude did, *in this session*.

---

# Rewind

```
Esc Esc          (double-tap Escape on an empty prompt)
/rewind          (aliases: /checkpoint, /undo)
```

Pick a prompt from the list, then an action:

| Option | Effect |
|---|---|
| Restore code and conversation | Both back to just before that prompt |
| Restore conversation | Chat rewinds, code stays |
| Restore code | Files revert, Claude keeps the chat |
| Summarize from here / up to here | Compress part of the chat — files untouched |
| Never mind | Back out |

**No git command runs. Nothing is committed, stashed, or reset.**

---

# What Checkpoints Don't See

- **Bash-made changes** — formatters, builds, `sed`, `rm`, files your tests write
- **External changes** — your IDE edits, other Claude Code sessions
- **Subagent edits** — including a background `/code-review --fix`

For all of these: **git** is your undo.

---

# Checkpoints vs. Git — Different Layers

| | Checkpoints / Rewind | `git stash` | `git reset --hard` |
|---|---|---|---|
| Scope | Claude's edit-tool changes, this session | Uncommitted tree | Uncommitted tree, to a commit |
| Granularity | Per prompt | One shot | One shot |
| Needs a commit? | No | No | Yes |
| Catches Bash/IDE edits? | No | Yes | Yes |

**Checkpoints = session-level undo.**
**Git = commit-level, durable undo.**
You'll use both — they solve different problems.

---

# The Failure Mode This Prevents

```
You: Also simplify the error handling across the whole file.
     Don't worry about matching my style, just make it cleaner
     everywhere you see room for it.

Claude Code:
  [touches far more than intended]
  [changes exception types]
  [characterization tests start failing]
```

Broad, vague instructions ("across the whole file," "everywhere") invite overreach.
This is a normal, recoverable outcome — not a crisis.

---

# Recovering Without Git

```
1. Run the characterization tests → confirm something broke
2. Esc Esc  (or /rewind)
3. Select the over-broad prompt → "Restore code and conversation"
   (restores to just *before* that prompt ran)
4. git diff → confirm the damaging edit is gone
5. Run tests again → confirm green
```

**No `git reset`. No `git stash`. No commit history touched at all.**

---

# Decision Table: Which Tool, When

| Situation | Best tool |
|---|---|
| Claude Code's last edit(s) this session are wrong | **Rewind** |
| Discard everything since your last commit | **`git reset --hard`** ⚠️ |
| Set uncommitted work aside, keep it for later | **`git stash`** |
| Small fix, faster to just describe it | **Ask Claude Code to undo/fix directly** |
| Bad state is from a *previous* session | **`/resume` + rewind**, or **Git** |
| Damage came from Bash, your IDE, or a subagent | **Git** — not tracked by checkpoints |
| Code is fine, context is bloated | **Rewind → Summarize from here** |
| Not sure how far back the problem started | **Rewind one checkpoint at a time**, test at each step |

---

# The General Rule

**Rewind first** when the damage is recent and session-local —
cheaper, more precise, no git ceremony.

**Git** when you need to cross a session boundary,
need a durable record, or need to *preserve* rather than discard.

Rewind is all-or-nothing per checkpoint — it restores to *a* point in time,
not a selective merge of good parts and bad parts.

---

# Lab 6 Preview

## What You'll Do

1. Confirm your Lab 5 refactor is the "last-good" state
2. Ask for a deliberately over-broad follow-up edit
3. Confirm it did real damage (failing characterization tests)
4. Rewind back to last-good — no git involved
5. Diff before/after to confirm full recovery
6. Watch a Bash-made file survive a rewind

---

# Lab 6 Starts Here

👉 Open [docs/labs/lab-06-checkpoints-and-rewind.md](../../../labs/lab-06-checkpoints-and-rewind.md)

**Goal for the next 20 minutes:**
- A damaged multi-file edit, recovered using rewind alone
- A clear mental model of rewind vs. `git stash` vs. `git reset --hard`

---

<!-- Navigation -->
[← 05 Refactoring & Context](05-refactoring-and-context.md) | **Day 1 Module 06** | [Next: Git Workflow →](07-git-workflow.md)
