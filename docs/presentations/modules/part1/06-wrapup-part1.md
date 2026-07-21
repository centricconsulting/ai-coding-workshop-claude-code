---
marp: true
theme: default
paginate: true
backgroundColor: #fff
---

# Part 1 Wrap-Up

## What You've Built. What's Next.

---

# What You Practiced in Part 1

| Lab | Skill | Key Takeaway |
|---|---|---|
| Lab 1 | Agentic model | Claude Code reads and acts — it's not autocomplete |
| Lab 2 | CLAUDE.md | Project memory written once, applied every session |
| Lab 3 | TDD | Tests as a feedback loop, not an afterthought |
| Lab 4 | Phased refactoring | Context limits are a design constraint |
| Lab 5 | Git workflow | `/review` before staging, not after pushing |

---

# The Mental Model Shift

**Before Part 1:**
> "I'll use Claude Code to help me write code faster."

**After Part 1:**
> "I direct a workflow. Claude Code executes it. The quality of my direction determines the quality of the output."

This shift is what separates productive Claude Code users from frustrated ones.

---

# What You Now Do Differently

**CLAUDE.md** — you don't re-explain your project every session.
The file does it for you.

**TDD** — you don't review 500 lines of generated code hoping it's right.
The tests tell you immediately.

**Phased prompts** — you don't hand Claude Code a 4-hour task and hope.
You break it into scoped, testable phases.

**`/review`** — you don't commit and discover issues in PR review.
You find them before staging.

---

# What Part 2 Covers

Part 1 was about **how to work with Claude Code**.

Part 2 is about **how to configure and extend it**:

| Module | Topic |
|---|---|
| 00 | Permissions & hooks — control what Claude Code can do |
| 01 | MCP servers — connect Claude Code to external systems |
| 02 | Sub-agents — structure tasks for parallel execution |
| 03 | Headless mode — Claude Code without a human in the loop |
| 04 | Capstone — design a full team configuration |

---

# Break

**15 minutes**

When you return:
- Have `.claude/settings.json` open in your editor
- You'll be modifying it in the first exercise of Lab 6

---

# Lab 6 Starts Here

When you're back, open:

👉 [docs/labs/lab-06-permissions-and-hooks.md](../../../labs/lab-06-permissions-and-hooks.md)

The first thing you'll do is ask Claude Code to explain what's already in `.claude/settings.json` — and why.

---

<!-- Navigation -->
[← 05 Git Workflow](05-git-workflow.md) | **Part 1 Wrap-Up** | [Part 2 Start: 00 Permissions & Hooks →](../part2/00-permissions-and-hooks.md)
