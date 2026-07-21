---
marp: true
theme: default
paginate: true
backgroundColor: #fff
---

# Day 1 Wrap-Up

## What You've Built. What's Next.

---

# What You Practiced on Day 1

| Lab | Skill | Key Takeaway |
|---|---|---|
| Lab 1 | Agentic model | Claude Code reads and acts — it's not autocomplete |
| Lab 2 | CLAUDE.md | Project memory written once, applied every session |
| Lab 3 | TDD | Tests as a feedback loop, not an afterthought |
| Lab 4 | Plan Mode & extended thinking | Plan before you touch files — zero-risk iteration |
| Lab 5 | Phased refactoring | Context limits are a design constraint |
| Lab 6 | Checkpoints & rewind | A session-level safety net, separate from git |
| Lab 7 | Git workflow | `/review` before staging, not after pushing |
| Lab 8 | Security review | `/security-review` as a dedicated gate, not a side effect |

---

# The Mental Model Shift

**Before Day 1:**
> "I'll use Claude Code to help me write code faster."

**After Day 1:**
> "I direct a workflow. Claude Code executes it. The quality of my direction determines the quality of the output."

This shift is what separates productive Claude Code users from frustrated ones.

---

# What You Now Do Differently

**CLAUDE.md** — you don't re-explain your project every session.
The file does it for you.

**TDD** — you don't review 500 lines of generated code hoping it's right.
The tests tell you immediately.

**Plan Mode** — you don't hand Claude Code a big task and hope.
You see the approach before anything changes, and iterate risk-free.

**Phased prompts + checkpoints** — you break large work into scoped phases,
and you have rewind as a session-level safety net if a phase overreaches.

**`/review` and `/security-review`** — you don't commit and discover issues
in PR review. You find them — including security issues — before staging.

---

# What Day 2 Covers

Day 1 was about **how to work with Claude Code**.

Day 2 is about **how to configure and extend it**:

| Lab | Topic |
|---|---|
| 9 | Permissions & hooks — control what Claude Code can do |
| 10 | MCP servers — connect Claude Code to external systems |
| 11 | Skills — package reusable, on-demand instructions |
| 12 | Sub-agents — structure tasks for parallel execution |
| 13 | Headless mode — Claude Code without a human in the loop |
| 14 | Output styles — change how Claude Code communicates |
| 15 | Capstone — design a full team configuration |

---

# End of Day 1

Nice work — that's the full foundation: agentic model, project memory, TDD,
planning, phased refactoring, session recovery, git workflow, and security
review.

**Before Day 2:**
- Make sure Day 1's work is committed (or at least not in a dirty, half-finished state)
- Have your GitHub fork ready — Labs 10, 13, and 15 need it
- If you haven't already, skim the [Agent Governance Guide](../../../guides/agent-governance.md) — it'll make more sense after today, and Day 2 builds toward it

See you tomorrow for Day 2.

---

<!-- Navigation -->
[← 08 Security Review](08-security-review.md) | **Day 1 Wrap-Up** | [Day 2 Start: 09 Permissions & Hooks →](../part2/00-permissions-and-hooks.md)
