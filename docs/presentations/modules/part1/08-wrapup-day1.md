---
marp: true
theme: default
paginate: true
backgroundColor: #fff
---

# Day 1 Wrap-Up

## What You've Built. What's Next.

---

# What You Practiced on Day 1 (1/2)

| Lab | Skill | Key Takeaway |
|---|---|---|
| Lab&nbsp;1 | Agentic model | Direct an agent that reads, plans, and acts — not one suggestion at a time |
| Lab&nbsp;2 | CLAUDE.md | Project memory written once, applied every session |
| Lab&nbsp;3 | TDD | Make red-first the default with a rule; hooks (Day 2) enforce that tests run |
| Lab&nbsp;4 | Plan Mode & effort levels | Plan before you touch files — zero-risk iteration |

---

# What You Practiced on Day 1 (2/2)

| Lab | Skill | Key Takeaway |
|---|---|---|
| Lab&nbsp;5 | Phased refactoring & context | Context limits are a design constraint — `/context`, `/compact` |
| Lab&nbsp;6 | Checkpoints & rewind | A session-level safety net, separate from git |
| Lab&nbsp;7 | Git workflow | `/code-review` before staging, not after pushing |

---

# Day 1 Command Cheat Sheet

<style scoped>
section { font-size: 22px; }
</style>

| Need | Command |
|---|---|
| Project memory | `/init`, `/memory`, `CLAUDE.md` |
| Model & reasoning depth | `/model`, `/effort`, `ultrathink` |
| Plan before acting | `/plan`, `Shift+Tab` |
| What's in context / free space | `/context`, `/compact [focus]`, `/clear` |
| What it's costing | `/usage` (`/cost`) |
| Undo Claude's edits | `/rewind` (`/checkpoint`, `/undo`), `Esc Esc` |
| Review for bugs | `/code-review` (`/review`) — levels, `--fix`, `--comment` |
| Get back to yesterday's session | `claude --continue`, `/resume` |

---

# The Mental Model Shift

**Before Day 1:**
> "I'll use Claude Code to help me write code faster."

**After Day 1:**
> "I direct a workflow. Claude Code executes it. The quality of my direction determines the quality of the output."

This shift is what separates productive Claude Code users from frustrated ones.

---

# What You Now Do Differently

<style scoped>
section { font-size: 27px; }
</style>

**CLAUDE.md** — you don't re-explain your project every session.
The file does it for you.

**TDD** — red-first is the default, not a habit you must remember.
A rule makes Claude write the failing test first; hooks (Day 2) enforce that tests run.

**Plan Mode** — you don't hand Claude Code a big task and hope.
You see the approach before anything changes, and iterate risk-free.

**Phased prompts + context hygiene + checkpoints** — you break large work into
scoped phases, compact on your terms, and have rewind as a session-level
safety net if a phase overreaches (git for everything rewind can't see).

**`/code-review`** — you don't commit and discover issues in PR review.
You find them before staging.

---

# What Day 2 Covers (1/2)

Day 1 was about **how to work with Claude Code**.

Day 2 opens with a security gate, then turns to **how to configure and extend it**:

| Lab | Topic |
|---|---|
| 8 | Security review — `/security-review` and a project-specific security skill |
| 9 | Permissions & hooks — permission modes, rules, `/permissions`, `/hooks`, a test-run hook |
| 10 | MCP servers — connect Claude Code to external systems (`/mcp`) |
| 11 | Skills — package reusable, on-demand instructions (`/skills`) |
| 12 | Subagents — use and chain custom agents in `.claude/agents/` |

---

# What Day 2 Covers (2/2)

| Lab | Topic |
|---|---|
| 13 | Headless mode & CI — `claude -p`, GitHub Actions |
| 14 | Output styles — change how Claude Code communicates *(self-study)* |
| 15 | Capstone — design a team configuration and share it as a plugin (`/plugin`) |

You already met one Day 2 idea today: the `/code-review` **background subagent**.

---

# End of Day 1

<style scoped>
section { font-size: 25px; }
</style>

Nice work — that's the full foundation: agentic model, project memory, TDD
from user story to working endpoint, planning, phased refactoring, session
recovery, and git workflow.

**Before Day 2:**
- Commit and push Day 1's work — Lab 8 opens Day 2 by reviewing your branch commits against `origin`
- Have a GitHub fork ready — Lab 10 Part C and Lab 13 D.2 use it (even if your main host is different)
- If you haven't already, skim the [Agent Governance Guide](../../../guides/agent-governance.md) — it'll make more sense after today, and Day 2 builds toward it

See you tomorrow for Day 2.

---

<!-- Navigation -->
[← 07 Git Workflow](07-git-workflow.md) | **Day 1 Wrap-Up** | [Day 2 Start: 08 Security Review →](../part2/00-security-review.md)
