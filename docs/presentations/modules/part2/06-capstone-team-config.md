---
marp: true
theme: default
paginate: true
backgroundColor: #fff
---

# Module 15: Capstone — Team Configuration

## Designing Claude Code for an Entire Team

---

# Where We Are

You've built the full Claude Code skill set:

| Skill | Lab |
|---|---|
| Agentic model, first contact | 1 |
| CLAUDE.md project memory | 2 |
| TDD | 3 |
| Plan Mode & extended thinking | 4 |
| Phased refactoring | 5 |
| Checkpoints & rewind | 6 |
| Git workflow integration | 7 |
| Security review workflows | 8 |
| Permissions & hooks | 9 |
| MCP servers | 10 |
| Skills | 11 |
| Sub-agents & parallelism | 12 |
| Headless & automation | 13 |
| Output styles & personas | 14 |

**Lab 15: put it all together for a real team — and ship it as a plugin.**

---

# The Capstone: Configuration + a Specialist + a Plugin

Three deliverables:

1. **Design a team configuration** — CLAUDE.md, rules, permissions, hooks, MCP.
   Ambient: shapes every task without deliberate invocation.
2. **Build your own subagent or skill** — one specialist workflow,
   tested against written success criteria (like the shipped
   `architecture-reviewer` subagent or `test-data-generator` skill).
3. **Package it as a plugin** — installable from a marketplace,
   so it travels with the team instead of one developer's `.claude/` folder.

---

# Why Configuration Is High-Leverage

A single developer learns Claude Code well → one developer benefits.

A well-designed team configuration → *every* developer benefits,
even ones who haven't read any docs.

```
CLAUDE.md + .claude/rules/  — what the project is and how it works
.claude/settings.json       — what Claude Code is allowed to do (+ hooks)
.mcp.json                   — what external systems Claude Code can reach
.claude/agents, skills      — specialists for repeatable workflows
plugin + marketplace        — how all of the above ships to the team
```

This is ambient intelligence: it shapes every interaction automatically.

---

# The Configuration Stack

```
CLAUDE.md (root)
  ↓ What this project is. Tech stack. Build + test. Conventions. Don't-do list.

src/CLAUDE.md
  ↓ Architecture layers. Domain vocabulary. Pattern rules. Key file locations.

.claude/rules/*.md  (paths: frontmatter)
  ↓ Glob-scoped conventions: tests, per-stack rules.

.claude/settings.json
  ↓ Allow/ask/deny rules. Hooks (format, test-run, block push). outputStyle.

.mcp.json
  ↓ External tools: GitHub (official server), Jira, Confluence.

.claude/agents/ + .claude/skills/
  ↓ Specialists: reviewers, generators, procedures.

team-plugin/  (.claude-plugin/plugin.json)
  ↓ Bundles agents, skills, hooks, MCP → installable everywhere.
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

**`.claude/rules/tests.md`** (`paths: ["tests/**"]`):
What does a good test look like here?
(Naming, structure, coverage expectations, what's in scope)

Rules are scoped by glob, subdirectory CLAUDE.md files by directory.

The test: ask Claude Code to create a new feature with no other context.
Does it produce something consistent with your standards?

---

# Part C: Permissions & Hooks Design

**Permission questions to answer:**

1. What always runs safely without confirmation? (build, test, lint)
2. What should never run without explicit approval? (deploy, push, migrate)
3. What should be denied outright? (destructive deletes, prod DB)

**Hook design table** (input = JSON on stdin → `jq`):

| Event | Matcher | Script does | Why |
|---|---|---|---|
| PostToolUse | `Edit\|Write` | `.cs` file? → `dotnet format` | Auto-format |
| PostToolUse | `Edit\|Write` | test file? → `dotnet test --filter` | Instant feedback |
| PreToolUse | `Bash` | `git push`? → stderr + **`exit 2`** | Block accidental push |
| PostToolUse | `.*` | append to `audit.log` | Compliance |

⚠️ `exit 1` does **not** block — it's a non-blocking error. Block with `exit 2`
or JSON `{"hookSpecificOutput":{"permissionDecision":"deny",...}}`.

---

# Part D: MCP Integration Design

For each external system your team uses, decide:

| System | Approach | Why |
|---|---|---|
| GitHub | Official GitHub MCP server (remote HTTP) | Issues, PRs, Actions |
| ADO | MCP server or `Bash(az boards *)` | Typed queries vs. simplicity |
| Internal docs | MCP resource | Read-only, structured |
| Simple CLI tool | Hook or `Bash(*)` allow | Not worth an MCP server |
| Prod database | **Not at all** | Read-only shadow DB only |
| Confluence | Custom MCP | No public server exists |

**The right answer is often "skip it."**
MCP complexity is justified when the external system is accessed frequently
and benefits from structured, typed interaction.

---

# Part E: Build Your Own Subagent or Skill

| Build a… | When | File |
|---|---|---|
| **Subagent** | Own context window, restricted tools, different model | `.claude/agents/<name>.md` |
| **Skill** | A procedure/template run in the main conversation | `.claude/skills/<name>/SKILL.md` |

1. **Success criteria first** — 3 must-catch, 2 must-not-do, output format
2. **Write the definition** — precise `description`, least-privilege `tools`, deliberate `model`
3. **Test** against a real file (e.g. `LegacyTaskProcessor`)
4. **Iterate** — vague description? drifting format? too many tools?

References: shipped `.claude/agents/`, `test-data-generator` skill,
Customization Decision Guide, Agent Design Guide.

---

# Part F: Package It as a Plugin

```
team-plugin/
  .claude-plugin/plugin.json     ← name, description, version
  agents/  skills/  hooks/hooks.json  .mcp.json
```

```bash
claude plugin validate ./team-plugin
claude --plugin-dir ./team-plugin              # try it for one session
claude plugin marketplace add ./team-marketplace
claude plugin install taskmanager-team@taskmanager-team-marketplace
```

- Plugin skills/agents are namespaced: `/taskmanager-team:<skill>`
- Hook scripts use `${CLAUDE_PLUGIN_ROOT}`
- Team rollout: `extraKnownMarketplaces` + `enabledPlugins` in `.claude/settings.json`
- Org rollout: managed settings can force-install and restrict marketplaces

---

# Part G: Team Guide & Governance

A configuration no one understands will be ignored or broken.

Write a one-page team guide:
1. How to onboard (what to run, what to read first)
2. The most important CLAUDE.md conventions to know
3. What the hooks do and why
4. Your subagent/skill: when and how to invoke it
5. MCP servers available and what they're for
6. How to propose changes (PR process? Who reviews?)

**Governance questions:**
- Who owns the CLAUDE.md files and the team plugin?
- How do you version the plugin, and when do teammates get updates?
- What's a developer's path for adding a new hook, subagent, or skill?
- Plugin (all repos) vs. repo `.claude/` vs. personal `~/.claude/`?

---

# Configuration *and* Agents

**Ambient configuration** (CLAUDE.md, rules, permissions, hooks, output styles):
shapes every interaction. *"Claude Code already knows how to do X correctly here."*

**Specialists** (subagents, skills): invoked by name or delegated to automatically.
*"Run this specialist to do X."*

**Plugins**: package both so they ship to the whole team.

The strongest setups use all three: ambient defaults for consistency,
a few well-tested specialists for high-value workflows, a plugin to distribute them.

---

# Lab 15: Capstone

👉 Open [docs/labs/lab-15-capstone-team-config.md](../../../labs/lab-15-capstone-team-config.md)

**Goal for the next 65 minutes:**
- Root CLAUDE.md + one path-scoped rule, tested with a real prompt
- `.claude/settings.json` with allow/ask/deny rules and at least 2 hooks (blocking = `exit 2`)
- MCP decision documented (even if the decision is "not needed")
- A custom subagent or skill, tested and refined
- A plugin that passes `claude plugin validate` and installs from a local marketplace
- One-page team guide drafted

**Then:** peer review — install each other's plugins and try the specialists.

---

# Workshop Complete

You now have the full Claude Code platform:

```
Foundations          Platform Layer         Automation
─────────────        ──────────────         ──────────
First contact        Permissions & hooks    Headless claude -p
CLAUDE.md & rules    MCP servers            claude-code-action CI
TDD                  Skills                 Nightly reports
Plan Mode            Subagents              Pre-commit hooks
Phased refactoring   Context management
Checkpoints & rewind Output styles
Git workflow         Plugins & marketplaces
Security review      Team configuration
```

**The highest-leverage next step:**
Apply the CLAUDE.md and hooks patterns to your real project this week,
and ship one specialist in a team plugin. Lab 15 is the starting point.

---

<!-- Navigation -->
[← 14 Output Styles](05-output-styles.md) | **Day 2 Module 15** | Workshop Complete
