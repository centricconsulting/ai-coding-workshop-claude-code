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
| Lab 4 | Plan Mode & effort levels | Plan before you touch files — zero-risk iteration |
| Lab 5 | Phased refactoring & context | Context limits are a design constraint — `/context`, `/compact` |
| Lab 6 | Checkpoints & rewind | A session-level safety net, separate from git |
| Lab 7 | Git workflow | `/code-review` before staging, not after pushing |
| Lab 8 | Security review | `/security-review` + a project skill as a dedicated gate |

---

# Day 1 Command Cheat Sheet

| Need | Command |
|---|---|
| Project memory | `/init`, `/memory`, `CLAUDE.md` |
| Model & reasoning depth | `/model`, `/effort`, `ultrathink` |
| Plan before acting | `/plan`, `Shift+Tab` |
| What's in context / free space | `/context`, `/compact [focus]`, `/clear` |
| What it's costing | `/usage` (`/cost`) |
| Undo Claude's edits | `/rewind` (`/checkpoint`, `/undo`), `Esc Esc` |
| Review for bugs | `/code-review` (`/review`) — levels, `--fix`, `--comment` |
| Review for vulnerabilities | `/security-review`, `/security-checklist` (your skill) |
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

**CLAUDE.md** — you don't re-explain your project every session.
The file does it for you.

**TDD** — you don't review 500 lines of generated code hoping it's right.
The tests tell you immediately.

**Plan Mode** — you don't hand Claude Code a big task and hope.
You see the approach before anything changes, and iterate risk-free.

**Phased prompts + context hygiene + checkpoints** — you break large work into
scoped phases, compact on your terms, and have rewind as a session-level
safety net if a phase overreaches (git for everything rewind can't see).

**`/code-review` and `/security-review`** — you don't commit and discover issues
in PR review. You find them — including security issues — before staging.

---

# What Day 2 Covers

Day 1 was about **how to work with Claude Code**.

Day 2 is about **how to configure and extend it**:

| Lab | Topic |
|---|---|
| 9 | Permissions & hooks — permission modes, rules, `/permissions`, `/hooks` |
| 10 | MCP servers — connect Claude Code to external systems (`/mcp`) |
| 11 | Skills — package reusable, on-demand instructions (`/skills`) |
| 12 | Subagents — use and chain custom agents in `.claude/agents/` |
| 13 | Headless mode & CI — `claude -p`, GitHub Actions |
| 14 | Output styles — change how Claude Code communicates |
| 15 | Capstone — design a team configuration and share it as a plugin (`/plugin`) |

You already met two Day 2 ideas today: the `/security-checklist` **skill**,
and the `/code-review` **background subagent**.

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
