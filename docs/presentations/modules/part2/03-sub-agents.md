---
marp: true
theme: default
paginate: true
backgroundColor: #fff
---

# Module 12: Custom Subagents & Agent Workflows

## Roles, Handoffs, and Isolated Context

---

# Where We Are

- **Module 11:** Skills package *procedures* that run **in** your conversation
- **Module 12:** Subagents package *roles* that run **beside** it

A **subagent** = its own context window + system prompt + tool allowlist + model.
Claude delegates a task → the subagent works independently → **only the final result** comes back.

The file reads, searches, and dead ends stay out of your context.

---

# Built-in Subagents

| Built-in | Tools | Used for |
|---|---|---|
| **Explore** | Read-only | Codebase search: *quick* / *medium* / *very thorough* |
| **Plan** | Read-only | Research while in plan mode |
| **general-purpose** | All | Multi-step tasks needing exploration *and* changes |

Explore and Plan skip CLAUDE.md to stay fast. **Custom subagents load CLAUDE.md and rules.**

---

# This Repo Ships Three Custom Subagents

`.claude/agents/` — ported from the Copilot workshop's custom agents

| Subagent | Role | Tools | Model |
|---|---|---|---|
| `backlog-generator` | Feature → INVEST stories → `docs/backlog/` | Read, Grep, Glob, **Write** | sonnet |
| `architecture-reviewer` | Clean Architecture / DDD review, both stacks | Read, Grep, Glob | inherit |
| `test-strategist` | TDD-ready test plan, both stacks | Read, Grep, Glob | sonnet |

---

# Anatomy of a Subagent

```markdown
---
name: architecture-reviewer
description: Read-only Clean Architecture and DDD reviewer ...
  Use proactively after changes that add a reference between layers ...
tools: Read, Grep, Glob        # omit = inherit ALL tools (incl. MCP)
model: inherit                 # sonnet | opus | haiku | <id> | inherit
---
You are a senior software architect ...   ← the ENTIRE system prompt
```

Optional: `disallowedTools`, `permissionMode`, `maxTurns`, `skills`, `mcpServers`, `hooks`, `memory`, `background`, `isolation: worktree`, `color`

Scope: `.claude/agents/` (project) · `~/.claude/agents/` (personal) · plugin `agents/`

---

# Three Ways to Invoke

| How | Example | Guarantee |
|---|---|---|
| **Automatic** | "Is it OK to call Infrastructure from TaskService?" | Claude decides, based on `description` |
| **Natural language** | "Use the architecture-reviewer subagent to…" | Strong hint |
| **@-mention** | `@"test-strategist (agent)" plan tests…` or `@agent-test-strategist` | That subagent runs |

Plus: `claude --agent architecture-reviewer` runs the *whole session* as that agent.

> `/agents` is **not** a wizard anymore. Ask Claude or edit the files directly; changes are picked up live.

---

# Chaining: The Main Conversation Orchestrates

```
backlog-generator ──► architecture-reviewer ──► test-strategist ──► implement (TDD)
 stories + ACs          layer placement            test plan
 docs/backlog/*.md      ADR-worthy decisions       per stack
```

- Copilot uses **handoff buttons**. In Claude Code, each subagent **returns to Claude**, which passes context to the next one
- Subagents don't talk to each other directly. Files (`docs/backlog/`) are durable handoff artifacts
- Each agent ends with a fixed output format and "Suggested Handoffs"

---

# Agent Design Principles

| Principle | Ask yourself |
|---|---|
| **Role** | Who is this agent? |
| **Scope** | What does it own, and what does it *not* do? |
| **Trigger** | When should Claude delegate? ("Use proactively when…") |
| **Tools** | Least privilege. Reviewers don't need Write or Bash |
| **Model** | Does it need the strongest model, or will `haiku`/`sonnet` do? |
| **Output format** | A fixed shape the next step can consume |
| **Handoff** | What happens next, and where does the artifact live? |

---

# Parallel & Background

- Subagents spawned from the main conversation start **in the background** by default
- **Ctrl+B** backgrounds a running task; **`/tasks`** lists subagents, their models, and transcripts
- Fan out independent work: *"Research the Domain, Application, and API layers in parallel using separate subagents"*
- Every result lands in your context, so ask for **summaries**

---

# Subagent, Skill, or Main Conversation?

| Use | When |
|---|---|
| **Main conversation** | Iterative back-and-forth; shared context across phases |
| **Skill** | Reusable procedure that runs *in* your context |
| **Subagent** | Verbose or self-contained work; enforced tool limits; a distinct role or model |
| **Skill + `context: fork`** | One-off isolated task written as a skill |

---

# Lab 12 Preview

1. **Part 0:** Run the built-in Explore subagent and watch it in `/tasks`
2. **Part A:** Use `architecture-reviewer` (natural language) and `test-strategist` (@-mention)
3. **Part B:** Chain backlog → architecture → tests on *task assignment + notifications*
4. **Part C:** Apply the design principles and build your own subagent
5. **Part D:** Fan out parallel subagents

## The Key Insight

A subagent is a **role with a boundary**: its own context, its own tools, and one job.
The main conversation is the orchestrator.

---

# Lab 12: Custom Subagents & Agent Workflows

👉 Open [docs/labs/lab-12-sub-agents.md](../../../labs/lab-12-sub-agents.md)

**Goal for the next 35–40 minutes:**
- Invoked the shipped subagents three ways
- `docs/backlog/<feature>.md` produced by the chained workflow
- One custom subagent of your own in `.claude/agents/`

---

<!-- Navigation -->
[← 11 Skills](02-skills.md) | **Day 2 Module 12** | [Next: 13 Headless & Automation →](04-headless-automation.md)
