# Lab 4: Plan Mode, Permission Modes & Thinking

**Duration**: 25–30 minutes
**Day**: 1 of 2 (Fundamentals)

**Learning Objectives**:
- Treat permission modes as interaction models: ask-before-act, plan-first, and act-autonomously
- Switch modes deliberately with `Shift+Tab`, `/plan`, and `--permission-mode`
- Use plan mode to iterate on a refactor plan before any source file changes
- Control reasoning depth with effort levels (`/effort`, `--effort`) and the one-turn `ultrathink` boost — and know when it's worth it

---

## Overview

So far you've asked Claude Code to write tests and implementations directly. For small, well-scoped changes that's fine. The next lab (Lab 5) refactors `LegacyTaskProcessor` — a messier, multi-file, higher-risk task. Before touching that code, you'll plan the refactor without any risk of Claude Code editing files mid-thought.

Along the way you'll learn the two dials that shape *every* Claude Code session:

1. **Permission mode** — how much Claude Code may do before checking with you
2. **Effort** — how hard Claude Code thinks before answering

> **Coming from GitHub Copilot?** Copilot Chat's **Ask / Plan / Agent** modes map closely onto Claude Code's permission modes. The difference: in Claude Code it's one agent the whole time — you're changing *what it's allowed to do without asking*, not switching to a different assistant.

### Choose Your Stack

> Follow sections marked 🔷 for .NET or 🟩 for Spring Boot. Modes and effort behave identically in both — only the file paths differ.

---

## Prerequisites

- Completed Labs 1–3
- Lab 3 work committed or stashed (`git status` clean), so it's easy to see what this lab changes

---

## Part A: Permission Modes as Interaction Models (7 min)

### A.1 The Modes

| Claude Code mode | Status bar | What runs without asking | Copilot analogue | Use it for |
|---|---|---|---|---|
| `manual` (shown as **Manual**; older alias `default`) | `⏸ manual mode on` | Reads only — every edit and command asks first | **Ask** (but Claude can still act once you approve) | Unfamiliar code, sensitive work |
| `plan` | `⏸ plan mode on` | Reads and exploration; **source edits are blocked** until you approve a plan | **Plan** | Multi-step or risky changes |
| `acceptEdits` | `⏵⏵ accept edits on` | Reads, file edits, and common filesystem commands in the working directory | **Agent** (edits auto-applied) | Iterating on code you're reviewing as it lands |
| `auto` | `⏵⏵ auto mode on` | Everything, with a background safety classifier blocking risky actions | **Agent** (fully autonomous) | Long tasks you trust; the built-in starting mode in recent versions |

Two more exist for automation, not day-to-day work: `dontAsk` (only pre-approved tools — for CI) and `bypassPermissions` (no checks — containers/VMs only). Neither is used in this workshop.

> **Note**: Plan mode blocks *edits*, not all commands. When auto mode is available, a safety classifier lets Claude run exploratory shell commands during planning; otherwise, anything outside the built-in read-only set asks first. Your working tree's source files stay untouched until you approve.

### A.2 Switch Modes

Start Claude Code and press **`Shift+Tab`** a few times. Watch the status bar under the prompt. From `auto`, the cycle runs `manual → accept edits → plan → auto`. (If your organization disables auto mode, you'll start in Manual and the cycle skips it.)

Other ways in:

```bash
claude --permission-mode plan      # start a session in plan mode
claude --permission-mode manual    # older docs call it: default
```

```
/plan Explain how tasks are persisted     ← enter plan mode and start on this task in one step
```

To make a mode the project default, set `"permissions": { "defaultMode": "plan" }` in `.claude/settings.json` (don't do this now — this repo's settings are shared by everyone).

### A.3 Feel the Difference

Switch to **Manual** (`⏸ manual mode on`) and ask:

```
Replace the XML doc comment on the TaskId class with one line: "Strongly-typed ID for a Task."
```

(🟩 Spring: *"Replace the Javadoc on the TaskId record with one line: 'Strongly-typed ID for a Task.'"*)

Claude Code proposes the edit and **asks** before writing. Choose **No** — and notice that you were the gate. Now press `Shift+Tab` to `⏵⏵ accept edits on`, repeat the request, and watch it apply immediately. Undo it:

```
Revert that change.
```

> **Key practice**: Pick the mode for the *task*, not for the day. Manual for anything unfamiliar, plan mode for anything with more than one step, accept-edits or auto once you trust the direction.

---

## Part B: Plan the LegacyTaskProcessor Refactor (12 min)

This is the main exercise: **the plan you produce here is what Lab 5 starts from.** Don't refactor anything now.

### B.1 Enter Plan Mode with a Task

#### 🔷 .NET Prompt

```
/plan Read src-dotnet/src/TaskManager.Infrastructure/Legacy/LegacyTaskProcessor.cs and src-dotnet/CLAUDE.md.
I want to modernize LegacyTaskProcessor to follow the conventions in src-dotnet/CLAUDE.md
(guard clauses, no abbreviations, log the swallowed file-write exception with ILogger
instead of ignoring it, Object Calisthenics)
without changing its observable behavior.
Draft a phased plan: what order should the work happen in, what does each phase
touch, and what's the safety net (tests) for each phase before moving to the next?
```

#### 🟩 Spring Boot Prompt

```
/plan Read src-springboot/taskmanager-infrastructure/src/main/java/com/example/taskmanager/infrastructure/legacy/LegacyTaskProcessor.java
and src-springboot/CLAUDE.md.
I want to modernize LegacyTaskProcessor to follow the conventions in src-springboot/CLAUDE.md
(guard clauses, no abbreviations, log the swallowed exceptions with SLF4J instead of
ignoring them, constructor injection)
without changing its observable behavior.
Draft a phased plan: what order should the work happen in, what does each phase
touch, and what's the safety net (tests) for each phase before moving to the next?
```

Watch what happens while it researches. In plan mode, Claude Code often delegates codebase research to the built-in **Plan** subagent (and uses the read-only **Explore** subagent for searches elsewhere). Both run in their own context window and can't write or edit files, so the exploration noise stays out of your main conversation. Expect a plan along these lines: characterize current behavior with tests → rename/extract with no logic changes → fix real bugs → clean up logging and guards.

### B.2 Iterate — Risk-Free

When the plan appears, you'll get three choices: **Yes, and use auto mode** (or **Yes, auto-accept edits**), **Yes, manually approve edits**, and **No, keep planning**. Choose **No, keep planning** and push back like you would on a design doc:

```
Phase 3 feels too broad — it's touching both the validation bug and the logging
in one step. Split those into separate phases so each has its own test checkpoint.
```

```
Which phase is riskiest? What could go wrong there, and how would we detect it
before moving to the next phase?
```

Every round stays in plan mode — your source files don't change no matter how many iterations this takes.

> 💡 Press **`Ctrl+G`** while the plan is shown to open it in your text editor and edit it directly.

### B.3 Keep the Plan for Lab 5 — Without Executing It

Approving a plan exits plan mode **and starts execution**, so don't approve it here. Instead:

1. Choose **No, keep planning** one last time, then press `Shift+Tab` until you're in **Manual** mode (`⏸ manual mode on`). This leaves plan mode without approving anything.
2. Ask:

   ```
   Save the final plan we agreed on to docs/plans/legacy-refactor-plan.md. Don't change any other file.
   ```

3. Approve that single write when prompted.

> **Lab 5 starts from this file.** Because it's in the repo, it survives `/clear`, a new session, or a restart.

---

## Part C: Effort and Thinking (6 min)

### C.1 What Actually Controls Reasoning Depth

You may have seen advice to write "think", "think hard", or "think harder" in prompts. **Claude Code doesn't treat those as keywords** — they're passed through as ordinary text. The real controls are:

| Control | Scope | How |
|---|---|---|
| **Effort level** | Session (or saved default) | `/effort low\|medium\|high\|xhigh\|max\|auto`, the left/right arrows in `/model`, or `claude --effort high` at launch |
| **`ultrathink`** | One turn | Type the word anywhere in a prompt; Claude Code adds an instruction to reason deeply on that turn (on recent versions it shows in rainbow colors and can raise that turn to `xhigh`) |
| **Thinking on/off** | Session | `Option+T` (macOS) / `Alt+T` (Windows/Linux); default in `/config`. The newest models always think, so the toggle may show *Thinking can't be turned off* |
| **See the thinking** | Display | `Ctrl+O` toggles verbose output, showing reasoning in gray italics |

Effort levels, roughly:

| Level | Good for |
|---|---|
| `low` | Quick exchanges you review step by step — renames, first sketches |
| `medium` | Day-to-day features with a clear scope (the default on the newest models) |
| `high` | Bug fixes in existing code, work where edge cases matter |
| `xhigh` | Deeper reasoning at higher token cost |
| `max` | Hard problems you want worked through without you; session-only and prone to overthinking |

Check your current level:

```
/effort status
```

### C.2 Compare Effort Levels on the Same Question

Ask a narrow, risk-focused question at low effort:

```
/effort low
```

```
List the edge cases in LegacyTaskProcessor that a naive refactor could silently break.
```

Then restore your previous level (`/effort auto` resets to the model default) and ask again with a one-turn boost:

```
ultrathink — list the edge cases in LegacyTaskProcessor that a naive refactor could silently break, and say which phase of our plan would catch each one.
```

Compare:
- Did the deeper answer surface a risk the plan in `docs/plans/legacy-refactor-plan.md` missed? If so, add it to the plan.
- How much longer did it take? (Thinking tokens are billed even when collapsed.)
- Would you pay that cost for every prompt — or only for this kind of question?

> **Key practice**: Effort is a dial, not a default. Leave it at the model's default for routine work, raise it (or use `ultrathink`) for architecture decisions, subtle bugs, and security-sensitive changes, and lower it for quick, reviewed iterations.

### C.3 (Optional) A Model for Planning: `opusplan`

```
/model opusplan
```

`opusplan` is a model alias that uses **Opus while you're in plan mode** and switches to **Sonnet for execution** — strong reasoning where the decisions are made, a faster model for carrying them out. Switch back with `/model` when you're done.

---

## Success Criteria

- [ ] You switched between Manual, accept-edits, and plan mode and can explain when to use each
- [ ] You entered plan mode with `/plan` and iterated on the plan at least once
- [ ] `docs/plans/legacy-refactor-plan.md` exists — and `git status` shows **no other changes**
- [ ] You compared a low-effort answer with an `ultrathink` answer and can describe the difference
- [ ] You can explain why "think hard" in a prompt doesn't change Claude Code's reasoning budget

---

## Extension Exercise

In plan mode, plan a *different* task — the `SendDigestAsync` extension from Lab 3 — and compare how much shorter and more confident that plan is than the `LegacyTaskProcessor` plan. Discuss why: task size, existing test coverage, and ambiguity all affect how much a plan needs to say. Then approve it with **Yes, manually approve edits** and watch plan mode hand off to execution.

---

## Troubleshooting

**Claude Code edited a file while I thought I was in plan mode**

Check the status bar — approving a plan exits plan mode and switches to the mode you picked. Run `git status` and `git diff` to see what changed, and ask Claude Code to revert anything unexpected.

**`Shift+Tab` doesn't show auto mode**

Auto mode needs a supported model and can be disabled by your organization. Manual, accept-edits, and plan mode are always available.

**Plan mode keeps asking permission to run commands**

That happens when auto mode isn't available: commands outside the built-in read-only set ask first. Approve read-only ones like `git log`; deny anything that writes.

**`ultrathink` isn't shown in rainbow colors**

Retype it outside quotes, brackets, or paths, in a prompt that doesn't start with `/`. Some providers and effort caps can't raise the turn; use `/effort` instead.

**The plan is too vague to act on**

Ask: "For each phase, name the specific methods or files it touches, and the specific test command that verifies it before moving on."

---

**Next**: [Lab 5: Multi-File Refactoring & Context Management](lab-05-refactoring-and-context.md)
