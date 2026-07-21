# Lab 4: Plan Mode & Extended Thinking

**Duration**: 20 minutes
**Day**: 1 of 2 (Fundamentals)

**Learning Objectives**:
- Understand Plan Mode as a read-only planning state that makes zero edits
- Enter and exit Plan Mode deliberately
- Iterate on a plan risk-free before any code changes happen
- Use extended-thinking triggers (`think`, `think hard`, `think harder`, `ultrathink`) and know when the extra latency is worth it

> **Note**: Claude Code's Plan Mode is a hard boundary — while it's active, no file is written, no command that mutates state runs. You approve the plan, *then* Claude Code executes it.

---

## Overview

So far you've asked Claude Code to write tests and implementations directly. For small, well-scoped changes that's fine. But the next lab (Lab 5) asks Claude Code to refactor `LegacyTaskProcessor` — a messier, multi-file, higher-risk task. Before touching that code, you're going to plan the refactor first, without any risk of Claude Code jumping ahead and editing files mid-thought.

That's what Plan Mode is for: a mode where Claude Code can read the codebase, reason about an approach, and propose a phased plan — but literally cannot write, edit, or run mutating commands until you exit the mode and approve.

### Choose Your Stack

> Follow sections marked 🔷 for .NET or 🟩 for Spring Boot. Plan Mode and extended thinking behave identically in both — only the file paths differ.

---

## Prerequisites

- Completed Labs 1–3
- Personal branch with Lab 3 work committed or stashed (Plan Mode won't touch it, but start clean)

---

## Part A: What Plan Mode Is (5 min)

### A.1 The Problem It Solves

When you ask Claude Code for something non-trivial, it normally does three things in one breath: reads code, decides on an approach, and starts editing. If the approach is wrong, you find out *after* files have already changed — and now you're reviewing a diff to figure out what happened, instead of reviewing a plan to decide what should happen.

Plan Mode splits those two things apart. While it's active:
- Claude Code can use read-only tools freely: reading files, searching, running read-only commands (`git status`, `git log`, test runs that don't mutate code)
- Claude Code **cannot** write files, edit files, or run commands that change state
- Every response is a proposal, not an action

### A.2 Entering and Exiting Plan Mode

Enter Plan Mode with `Shift+Tab` (cycles through modes) or by starting Claude Code with the `--permission-mode plan` flag. While in Plan Mode you'll see an indicator in the CLI showing you're in a read-only state.

To exit, you have two paths:
- **Approve the plan** — Claude Code asks for confirmation and, once you approve, switches to normal mode and starts executing
- **Cancel** — back out without approving anything; no files were touched either way

> **Key practice**: Plan Mode isn't just for big refactors. Any time you're not sure what Claude Code will decide to do, entering Plan Mode first costs you nothing — you can always approve immediately if the first proposal is good.

### A.3 Why Zero Edits Matters

The guarantee isn't "Claude Code will probably be careful." It's structural — the tools available in Plan Mode don't include `Write`, `Edit`, or mutating `Bash` commands. You can iterate on a bad plan ten times in a row and your working tree is byte-for-byte what it was when you started. That's what makes Plan Mode safe to use liberally, even mid-session, even when you're unsure what you want yet.

---

## Part B: Plan the LegacyTaskProcessor Refactor (10 min)

This is the main exercise, and it matters beyond this lab: **the plan you produce here is what Lab 5 starts from.** Don't skip ahead and refactor anything now — the point is to leave this lab with a written plan and an untouched codebase.

### B.1 Enter Plan Mode

```bash
claude
```

Press `Shift+Tab` until you see the Plan Mode indicator, or start with:

```bash
claude --permission-mode plan
```

### B.2 Ask for a Phased Refactor Plan

#### 🔷 .NET Prompt

```
Read src/TaskManager.Infrastructure/Legacy/LegacyTaskProcessor.cs and src/CLAUDE.md.

I want to modernize LegacyTaskProcessor to follow the conventions in src/CLAUDE.md
(guard clauses, no abbreviations, ILogger instead of Console, Object Calisthenics)
without changing its observable behavior.

Draft a phased plan: what order should the work happen in, what does each phase
touch, and what's the safety net (tests) for each phase before moving to the next?
Don't write anything yet — I want to review and adjust the plan first.
```

#### 🟩 Spring Boot Prompt

```
Read src-springboot/taskmanager-infrastructure/src/main/java/com/example/taskmanager/infrastructure/legacy/LegacyTaskProcessor.java
and src-springboot/CLAUDE.md.

I want to modernize LegacyTaskProcessor to follow the conventions in src-springboot/CLAUDE.md
(guard clauses, no abbreviations, SLF4J instead of System.out, constructor injection)
without changing its observable behavior.

Draft a phased plan: what order should the work happen in, what does each phase
touch, and what's the safety net (tests) for each phase before moving to the next?
Don't write anything yet — I want to review and adjust the plan first.
```

Claude Code will read the legacy file and CLAUDE.md, then propose a plan — typically something like: characterize current behavior with tests first, then rename/extract with no logic changes, then address real bugs, then clean up logging and guards. This should look familiar from Lab 5's structure, because that's exactly what you're pre-building here.

### B.3 Iterate on the Plan — Risk-Free

This is the part Plan Mode is built for. Push back on the plan the same way you'd push back on a design doc, because nothing is at risk:

```
Phase 3 feels too broad — it's touching both the validation bug and the logging
in one step. Split those into separate phases so each has its own test checkpoint.
```

```
Which phase is riskiest? What could go wrong there, and how would we detect it
before moving to the next phase?
```

Ask as many follow-up questions as you want. Every response stays inside Plan Mode — no files change no matter how many rounds this takes.

### B.4 Approve and Exit

Once the plan looks right, approve it to exit Plan Mode:

```
This plan looks good. Exit plan mode — but don't start executing yet, I'm saving
this for the next lab.
```

> **Save this plan.** Copy the final plan text somewhere durable — a scratch file, a comment in your terminal scrollback, or just leave this Claude Code session open. **Lab 5 starts from this exact plan.** If your session ends, ask Claude Code to restate the plan from the conversation, or re-run the planning prompt from B.2 to regenerate it before starting Lab 5.

---

## Part C: Extended Thinking — `ultrathink` and the Cost Tradeoff (5 min)

Claude Code supports thinking triggers you can drop into a prompt: `think`, `think hard`, `think harder`, and `ultrathink`, in increasing order of reasoning depth. Each level gives the model more space to reason before responding — useful for genuinely hard problems, wasteful for simple ones.

### C.1 Re-run the Plan with `ultrathink`

Start a fresh planning prompt, this time asking for maximum reasoning depth:

```
ultrathink: Read LegacyTaskProcessor again. I want the single safest possible
phased plan to refactor it with zero risk of behavior change — consider edge
cases in the existing logic that a naive refactor could silently break.
```

### C.2 Compare

Look at the two plans side by side (your Part B plan and this one):

- Did `ultrathink` surface a risk or edge case the first plan missed?
- Did it take noticeably longer to respond?
- Is the extra depth worth it for *this* task, or would it matter more on a harder one?

> **Key practice**: Extended thinking is a dial, not a default. Reach for `think` or `think hard` when a task has real ambiguity or risk — an architecture decision, a subtle bug, a security-sensitive change. Skip it for routine work; the latency cost isn't worth it when the answer is already obvious.

---

## Success Criteria

- [ ] You entered and exited Plan Mode at least once
- [ ] You have a written, phased refactor plan for `LegacyTaskProcessor`
- [ ] Zero files were edited during this lab — `git status` is clean
- [ ] You compared a plan with and without `ultrathink` and can describe the difference
- [ ] You saved or can regenerate the plan for use in Lab 5

---

## Extension Exercise

While still in Plan Mode, ask Claude Code to plan a *different* task — adding the `SendDigestAsync` extension from Lab 3 — and compare how much shorter and more confident that plan is compared to the LegacyTaskProcessor plan. Discuss why: task size, existing test coverage, and ambiguity all affect how much a plan needs to say.

---

## Troubleshooting

**Claude Code seems to edit a file while "in Plan Mode"**

Check the mode indicator in the CLI — it's easy to have exited Plan Mode accidentally (e.g., by approving a plan) without noticing. If in doubt, run `git status` — if there are no unexpected changes, you're fine.

**Plan Mode isn't available / `Shift+Tab` doesn't cycle to it**

Confirm your Claude Code version supports Plan Mode (`claude --version`), or launch directly with `claude --permission-mode plan`.

**The plan is too vague to act on**

Ask Claude Code to make it concrete: "For each phase, name the specific methods or files that phase touches, and the specific test command that verifies it before moving on."

**`ultrathink` response takes a long time or feels like overkill**

That's the tradeoff in action. For a task this size, `think` or even no trigger is often enough — `ultrathink` is worth reaching for on harder, higher-stakes problems.

---

**Next**: [Lab 5: Multi-File Refactoring & Context Management](lab-05-refactoring-and-context.md)
