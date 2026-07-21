---
marp: true
theme: default
paginate: true
backgroundColor: #fff
---

# Module 04: Plan Mode & Extended Thinking

## Think Before You Touch Files

---

# Where We Are

Module 03 / Lab 3: you drove the Red-Green-Refactor loop with Claude Code writing tests and implementation directly.

That worked because the task was small and well-scoped.

**Next up**: Lab 5 asks Claude Code to refactor `LegacyTaskProcessor` — bigger, messier, higher-risk. Before touching that code, you need a way to see the approach *before* anything changes.

That's Plan Mode.

---

# The Problem: Read → Decide → Edit, All in One Breath

Normally, one prompt does all three:

```
You: Refactor LegacyTaskProcessor to follow our conventions.

Claude Code:
  [reads the file]
  [decides on an approach]
  [starts editing — right now, in this same turn]
```

If the approach is wrong, you find out **after** files changed.
You're reviewing a diff to reconstruct what happened,
instead of reviewing a plan to decide what should happen.

---

# Plan Mode: A Hard Boundary

While Plan Mode is active:

- ✅ Claude Code can read files, search, run read-only commands
- ❌ Claude Code **cannot** write, edit, or run mutating commands
- Every response is a proposal — not an action

This isn't "Claude Code will probably be careful."
It's structural: the write/edit tools simply aren't available.

---

# Entering and Exiting

**Enter:**
```
Shift+Tab     (cycles through permission modes)
```
or
```bash
claude --permission-mode plan
```

**Exit:**
- **Approve** the plan → Claude Code switches to normal mode and executes
- **Cancel** → back out, nothing was ever touched either way

---

# Why This Is Safe to Use Liberally

```
You: [propose approach A]
Claude Code: [plan A]
You: "What about edge case X?"
Claude Code: [revised plan]
You: "Split phase 3, it's doing too much."
Claude Code: [revised plan again]
You: "Good. Approve."
```

Ten rounds of back-and-forth. Working tree: **byte-for-byte unchanged**
until the moment you approve.

**Any time you're unsure what Claude Code will decide to do,
enter Plan Mode first. It costs nothing.**

---

# Extended Thinking: A Dial, Not a Default

Claude Code recognizes thinking triggers in your prompt, in increasing depth:

```
think  →  think hard  →  think harder  →  ultrathink
```

More reasoning space before responding.
More latency. More cost. Not always more value.

---

# When Extended Thinking Earns Its Cost

| Reach for it when | Skip it when |
|---|---|
| Genuine ambiguity in the approach | The answer is already obvious |
| Architecture or design decisions | Routine, well-scoped edits |
| Security-sensitive changes | Small, low-risk tasks |
| Subtle bugs, tricky edge cases | You've done this exact thing before |

**The tradeoff is real**: `ultrathink` on a trivial task is wasted latency.
`think` (or nothing) on a hard problem risks a shallow answer.

---

# Plan Mode + Extended Thinking, Together

They compose:

```
ultrathink: [inside Plan Mode] Draft the safest possible phased plan
to refactor LegacyTaskProcessor with zero risk of behavior change.
```

Maximum reasoning depth, zero risk of edits while you evaluate it.
This is exactly what Lab 4's main exercise does.

---

# Lab 4 Preview

## What You'll Do

1. Enter Plan Mode
2. Ask Claude Code to draft a phased refactor plan for `LegacyTaskProcessor`
3. Iterate on the plan — push back, ask about risk, split phases — all risk-free
4. Approve and exit once satisfied
5. Re-run one planning prompt with `ultrathink` and compare

**Save the plan you produce — Lab 5 starts from it.**

---

# Lab 4 Starts Here

👉 Open [docs/labs/lab-04-plan-mode-and-extended-thinking.md](../../../labs/lab-04-plan-mode-and-extended-thinking.md)

**Goal for the next 20 minutes:**
- A written, phased refactor plan for `LegacyTaskProcessor`
- Zero file edits — `git status` stays clean the whole lab
- A clear sense of when `ultrathink` is worth the wait

---

<!-- Navigation -->
[← 03 TDD](03-tdd-with-claude-code.md) | **Day 1 Module 04** | [Next: Refactoring & Context →](05-refactoring-and-context.md)
