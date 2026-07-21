---
marp: true
theme: default
paginate: true
backgroundColor: #fff
---

# Module 10: Capstone — Team Configuration

## Designing Claude Code for an Entire Team

---

# Where We Are

You've built the full Claude Code skill set:

| Skill | Lab |
|---|---|
| Agentic model, first contact | 1 |
| CLAUDE.md project memory | 2 |
| TDD | 3 |
| Phased refactoring | 4 |
| Git workflow integration | 5 |
| Permissions & hooks | 6 |
| MCP servers | 7 |
| Sub-agents & parallelism | 8 |
| Headless & automation | 9 |

**Lab 10: put it all together for a real team.**

---

# The Capstone Is Different

In a Copilot workshop, the capstone is "build an agent" —
a discrete tool you invoke by name for a specific workflow.

**Here, the capstone is "design a configuration."**

A set of files that make Claude Code behave well for an entire team,
on every task, without requiring deliberate invocation.

---

# Why Configuration Is High-Leverage

A single developer learns Claude Code well → one developer benefits.

A well-designed team configuration → *every* developer benefits,
even ones who haven't read any docs.

```
CLAUDE.md           — encodes what the project is and how it works
.claude/settings.json — encodes what Claude Code is allowed to do
.mcp.json           — encodes what external systems Claude Code can reach
```

This is ambient intelligence: it shapes every interaction automatically.

---

# The Configuration Stack

```
CLAUDE.md (root)
  ↓ What this project is. Tech stack. Build + test. Conventions. Don't-do list.

src/CLAUDE.md
  ↓ Architecture layers. Domain vocabulary. Pattern rules. Key file locations.

tests/CLAUDE.md
  ↓ Test naming. AAA structure. Coverage expectations.

.claude/settings.json
  ↓ Allow/deny lists. Hooks (format, test-run, audit).

.mcp.json
  ↓ External tools: GitHub, Jira, Confluence, deployment status.
```

Each layer narrows and specializes the one above it.

---

# Choosing Your Scenario

Pick the one closest to your real team:

| Scenario | Stack | Key Challenge |
|---|---|---|
| Enterprise .NET API | .NET / C# | Clean Architecture enforcement, ADO |
| Spring Boot microservices | Java / Spring Boot | Service boundaries, Kafka |
| Full-stack TypeScript | TypeScript / React / Node | Frontend + backend conventions differ |
| Data engineering | Python / SQL | Pipeline conventions, data validation |
| **Your actual team** | — | — |

The goal is a configuration you could take to work on Monday.

---

# Part B: Designing the CLAUDE.md Hierarchy

For your scenario, answer per file:

**Root CLAUDE.md:**
What does Claude Code need to know to work in this repo at all?
(Stack, build commands, conventions, what NOT to do)

**src/ CLAUDE.md:**
What does Claude Code need to know to write correct code here?
(Architecture, naming, domain vocabulary, patterns)

**tests/ CLAUDE.md:**
What does a good test look like here?
(Naming, structure, coverage expectations, what's in scope)

The test: ask Claude Code to create a new feature with no other context.
Does it produce something consistent with your standards?

---

# Part C: Permissions & Hooks Design

**Permission questions to answer:**

1. What always runs safely without confirmation? (build, test, lint)
2. What should never run without explicit approval? (deploy, push, migrate)
3. What should be denied outright? (destructive deletes, prod DB)

**Hook design table:**

| Event | Trigger | Command | Why |
|---|---|---|---|
| PostToolUse | Write `*.cs` | `dotnet format` | Auto-format |
| PostToolUse | Write `*Test*.cs` | `dotnet test --no-build` | Instant feedback |
| PreToolUse | `git push` | `exit 1` | Block accidental push |
| PostToolUse | `.*` | `>> audit.log` | Compliance |

---

# Part D: MCP Integration Design

For each external system your team uses, decide:

| System | Approach | Why |
|---|---|---|
| GitHub / ADO | MCP server | Typed queries, reusable across team |
| Internal docs | MCP resource | Read-only, structured |
| Simple CLI tool | Hook or `Bash(*)` allow | Not worth an MCP server |
| Prod database | **Not at all** | Read-only shadow DB only |
| Confluence | Custom MCP | No public server exists |

**The right answer is often "skip it."**
MCP complexity is justified when the external system is accessed frequently
and benefits from structured, typed interaction.

---

# Part E: Team Guide & Governance

A configuration no one understands will be ignored or broken.

Write a one-page team guide:
1. How to onboard (what to run, what to read first)
2. The most important CLAUDE.md conventions to know
3. What the hooks do and why
4. MCP servers available and what they're for
5. How to propose changes (PR process? Who reviews?)

**Governance questions:**
- Who owns the CLAUDE.md files? (Engineering lead? All via PR?)
- How do you version config changes alongside code?
- What's a developer's path for adding a new hook?
- How do user-level settings interact with project-level settings?

---

# Configuration vs. Agent Design

**Copilot agents**: discrete, task-specific tools invoked by name.
Build them when you need a repeatable, specialized workflow.

**Claude Code configuration**: ambient, shapes every interaction.
Design it when you want consistent behavior across an entire team.

Both encode team standards. Different models, different leverage points.

Copilot agent: *"Run this tool to do X."*
Claude Code config: *"Claude Code already knows how to do X correctly here."*

---

# Lab 10: Capstone

👉 Open [docs/labs/lab-10-capstone-team-config.md](../../../labs/lab-10-capstone-team-config.md)

**Goal for the next 45 minutes:**
- Root CLAUDE.md drafted and tested with a real prompt
- `src/` CLAUDE.md with stack-specific conventions
- `.claude/settings.json` with allow/deny rules and at least 2 hooks
- MCP decision documented (even if the decision is "not needed")
- One-page team guide drafted

**Then:** peer review with another participant.
Review each other's designs for gaps, friction, and missed opportunities.

---

# Workshop Complete

You now have the full Claude Code platform:

```
Foundations          Platform Layer         Automation
─────────────        ──────────────         ──────────
First contact        Permissions & hooks    Headless --print
CLAUDE.md            MCP servers            CI integration
TDD                  Sub-agents             Nightly reports
Phased refactoring   Context management     Pre-commit hooks
Git workflow         Team configuration
```

**The highest-leverage next step:**
Apply the CLAUDE.md and hooks patterns to your real project this week.
The configuration from Lab 10 is the starting point.

---

<!-- Navigation -->
[← 03 Headless & Automation](03-headless-automation.md) | **Part 2 Module 04** | Workshop Complete
