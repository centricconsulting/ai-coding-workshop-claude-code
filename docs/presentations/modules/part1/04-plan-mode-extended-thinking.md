---
marp: true
theme: default
paginate: true
backgroundColor: #fff
---

# Module 04: Plan Mode, Permission Modes & Thinking

## Choose How Much Claude Does — and How Hard It Thinks

---

# Where We Are

Module 03 / Lab 3: you drove the Red-Green-Refactor loop with Claude Code writing tests and implementation directly.

That worked because the task was small and well-scoped.

**Next up**: Lab 5 asks Claude Code to refactor `LegacyTaskProcessor` — bigger, messier, higher-risk. Before touching that code, you need a way to see the approach *before* anything changes.

That's plan mode — one of several **permission modes**.

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

# Permission Modes = Interaction Models

| Mode | Status bar | Runs without asking | Copilot analogue |
|---|---|---|---|
| `default` (**Manual**) | `⏸ manual mode on` | Reads only | **Ask** — but can act once you approve |
| `plan` | `⏸ plan mode on` | Reads + exploration; **edits blocked** | **Plan** |
| `acceptEdits` | `⏵⏵ accept edits on` | Reads, edits, basic file commands | **Agent** |
| `auto` | `⏵⏵ auto mode on` | Everything, with a safety classifier | **Agent**, hands-off |

Same agent, same conversation — you change **what it may do without asking**.
(`dontAsk` for CI and `bypassPermissions` for containers exist; not used here.)

---

# Switching Modes

```
Shift+Tab        auto → manual → accept edits → plan → auto
/plan <task>     enter plan mode and start on <task>
```

```bash
claude --permission-mode plan        # start in plan mode
```

```json
// .claude/settings.json — make it the project default
{ "permissions": { "defaultMode": "plan" } }
```

**Pick the mode for the task, not for the day.**

---

# Plan Mode: Edits Wait for Your Approval

- ✅ Reads files, searches, explores
- ✅ Shell commands: classifier-approved when auto mode is available, otherwise read-only commands only (others ask)
- ❌ **No source edits** until you approve a plan
- Research is delegated to the built-in **Plan** subagent (read-only, own context window)

When the plan is ready:

| Choice | Result |
|---|---|
| Yes, and use auto mode | Exit plan mode, execute hands-off |
| Yes, manually approve edits | Exit plan mode, approve each edit |
| No, keep planning | Stay in plan mode, give feedback |

`Ctrl+G` edits the plan in your editor. `Shift+Tab` leaves without approving.

---

# Why This Is Safe to Use Liberally

```
You: /plan Refactor LegacyTaskProcessor to our conventions
Claude Code: [plan A]
You: "What about edge case X?"            ← No, keep planning
Claude Code: [revised plan]
You: "Split phase 3, it's doing too much."
Claude Code: [revised plan again]
```

Ten rounds of back-and-forth. Source files: **unchanged**
until the moment you approve.

**Any time you're unsure what Claude Code will decide to do, plan first.**

---

# Built-in Read-Only Subagents

| Subagent | Used for | Tools |
|---|---|---|
| **Explore** | Searching and understanding a codebase (quick / medium / very thorough) | Read-only |
| **Plan** | Gathering context during plan mode | Read-only |
| general-purpose | Multi-step work that explores *and* acts | All |

Exploration output stays in the subagent's context — your main conversation stays clean.
Note: Explore and Plan start **without** your `CLAUDE.md` files.

---

# Thinking: The Myth

You may have seen this ladder:

```
think  →  think hard  →  think harder  →  ultrathink
```

**Only `ultrathink` is a keyword.** Claude Code passes "think", "think hard",
and "think more" through as ordinary prompt text.

The real controls are **effort** (session) and **`ultrathink`** (one turn).

---

# Effort: The Real Dial

```
/effort                  slider        /effort status    show current level
/effort high             set level     /effort auto      reset to model default
claude --effort low      at launch     /model  ← →       adjust in the picker
```

| Level | Good for |
|---|---|
| `low` | Quick, reviewed iterations — renames, sketches |
| `medium` | Clear-scope features (default on the newest models) |
| `high` | Bug fixes in existing code, edge cases matter |
| `xhigh` | Deeper reasoning, higher token cost |
| `max` | Hard autonomous problems; session-only, can overthink |

---

# One-Turn Boosts and Toggles

- **`ultrathink`** anywhere in a prompt → deeper reasoning for *that turn* only (rainbow text when it can raise the turn to `xhigh`)
- **`Option+T` / `Alt+T`** → thinking on/off for the session (newest models always think)
- **`Ctrl+O`** → show the thinking in verbose mode

| Reach for more effort when | Stay at default (or lower) when |
|---|---|
| Architecture or design decisions | Routine, well-scoped edits |
| Security-sensitive changes | The answer is already obvious |
| Subtle bugs, tricky edge cases | You review each step anyway |

Thinking tokens are billed even when collapsed.

---

# `opusplan`: A Model for Each Phase

```
/model opusplan
```

- **Opus** while you're in plan mode — strongest reasoning where decisions are made
- **Sonnet** for execution — faster and cheaper once the plan is settled

Plan mode + `ultrathink` + `opusplan` compose: deep reasoning, zero edits, then efficient execution.

---

# Lab 4 Preview

## What You'll Do

1. Cycle permission modes with `Shift+Tab`; feel Manual vs. accept-edits
2. `/plan` a phased refactor of `LegacyTaskProcessor`
3. Iterate — push back, ask about risk, split phases — all risk-free
4. Leave plan mode **without** executing; save the plan to `docs/plans/`
5. Compare a low-effort answer with an `ultrathink` answer

**Lab 5 starts from the plan you save.**

---

# Lab 4 Starts Here

👉 Open [docs/labs/lab-04-plan-mode-and-extended-thinking.md](../../../labs/lab-04-plan-mode-and-extended-thinking.md)

**Goal for the next 25–30 minutes:**
- You can pick the right permission mode for a task
- A written, phased refactor plan in `docs/plans/legacy-refactor-plan.md` — no other files changed
- A clear sense of when extra effort is worth the wait

---

<!-- Navigation -->
[← 03 TDD](03-tdd-with-claude-code.md) | **Day 1 Module 04** | [Next: Refactoring & Context →](05-refactoring-and-context.md)
