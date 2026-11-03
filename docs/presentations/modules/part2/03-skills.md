---
marp: true
theme: default
paginate: true
backgroundColor: #fff
---

# Module 11: Skills

## Packaging Reusable Instructions for Claude Code

---

# Where We Are

You extended Claude Code's reach with MCP (Module 10) — new tools it can call.

**Module 11 is about packaging *instructions*, not tools.**

A Skill doesn't add a new capability the way MCP does. It packages a procedure — something Claude Code already knows how to do — so the procedure only loads into context when it's actually relevant.

> **Custom slash commands are now skills.** `.claude/commands/x.md` and `.claude/skills/x/SKILL.md` both create `/x`. Old command files still work; skills add supporting files, richer frontmatter, and auto-invocation.

---

# The Extension-Mechanism Spectrum

<style scoped>
section { font-size: 26px; }
</style>

| Mechanism | Loaded | Best For |
|---|---|---|
| **CLAUDE.md** | Every turn | Context, conventions, project knowledge |
| **Rules** (`.claude/rules/`) | Startup, or on matching `paths:` | Stack-/folder-specific conventions |
| **Hooks** | On matching event | Deterministic side effects |
| **MCP** | Tool list at startup | New tool capabilities |
| **Skills** | Description always; body on demand | Packaged, reusable procedures |
| **Subagents** | On delegation | Isolated / parallel / context-heavy work |

---

# Why Not Just Put It in CLAUDE.md?

CLAUDE.md loads into context on every single turn, whether relevant or not.

That's fine for a handful of always-relevant conventions. It stops scaling once you have a dozen detailed procedures that are each only relevant *sometimes* — "check architecture boundaries," "generate test data," "write a migration checklist."

**Skills solve this with progressive disclosure.**

---

# Progressive Disclosure

1. **Always loaded** — every skill's `name` + `description` (+ `when_to_use`)
2. **On invocation** — the full SKILL.md body, when Claude matches the description *or* you type `/skill-name`. It then stays in context for the session.
3. **On reference** — supporting files (templates, scripts) only when the body says to read them

**The description is the only always-on part. If it doesn't state the trigger conditions precisely, the skill never fires — or fires on the wrong tasks.**

---

# SKILL.md Anatomy

```
.claude/skills/test-data-generator/
  SKILL.md                  ← required
  templates/TaskTestData.cs ← loaded only when referenced
```

```markdown
---
name: test-data-generator
description: Generates realistic test data for TaskManager entities…
argument-hint: "[entity] [count] [csharp|java|json|theory]"
arguments: [entity, count, format]
allowed-tools: Read Grep Glob
---
Request: $ARGUMENTS   Entity: $entity   Count: $count
```

---

# Key Frontmatter

<style scoped>
section { font-size: 27px; }
</style>

| Field | What it does |
|---|---|
| `description` / `when_to_use` | Drives automatic invocation |
| `argument-hint`, `arguments` | Autocomplete hint; named `$args` |
| `disable-model-invocation: true` | Only *you* can run it (deploys, commits) |
| `user-invocable: false` | Only *Claude* loads it (background knowledge) |
| `allowed-tools` | Pre-approves tools — does **not** restrict |
| `context: fork` + `agent` | Run in an isolated subagent |
| `paths`, `model`, `effort` | Scope activation; override model/effort |

**Substitutions:** `$ARGUMENTS`, `$0`/`$1`, `$name`, `${CLAUDE_SKILL_DIR}`, and `` !`cmd` `` to inject shell output before Claude reads the skill.

---

# Inline vs. `context: fork`

| | Inline (default) | `context: fork` |
|---|---|---|
| Runs in | Your conversation | A subagent (`Explore`, `Plan`, `general-purpose`, custom) |
| Sees history? | Yes | No — skill body is the task |
| Good for | Conventions, guided procedures | Self-contained research or scans |
| Context cost | Body stays in your window | Only the summary comes back |

A forked skill that only contains guidelines (no task) returns nothing useful.

---

# Where Skills Live

| Scope | Location | Invoked as |
|---|---|---|
| Personal (user) | `~/.claude/skills/<name>/` | `/name` |
| Project | `.claude/skills/<name>/` (commit it) | `/name` |
| Plugin | `<plugin>/skills/<name>/` | `/plugin-name:name` |
| Enterprise | Managed settings dir | `/name` |

- **`/skills`** — browse everything available (project, personal, plugin, bundled like `/code-review`, `/simplify`, `/debug`)
- Skills are **live-reloaded** — edit SKILL.md, no restart (`/reload-skills` for a brand-new top-level dir)
- Follows the open **Agent Skills** standard (agentskills.io)

---

# Skills vs. Hooks vs. MCP

| | Hook | MCP | Skill |
|---|---|---|---|
| Triggers on | Event | Explicit tool call | Matching description or `/name` |
| Involves reasoning? | No — fixed command | Yes | Yes — Claude follows it |
| Adds a capability? | No | Yes (new tool) | No (packages a procedure) |
| Cost when unused | None | Tool list in context | One description in context |

---

# Lab 11 Preview

<style scoped>
section { font-size: 25px; }
</style>

## What You'll Do

1. Place Skills on the spectrum (vs. CLAUDE.md, rules, hooks, MCP, subagents)
2. **Inspect & run the shipped skill:** `/skills` → `/test-data-generator Task 5 csharp`, then let Claude auto-invoke it
3. **Build your own:** `.claude/skills/clean-architecture-boundary-check/SKILL.md` with `$ARGUMENTS`
4. Confirm it activates (no restart) — and doesn't false-trigger
5. Scope & governance: personal vs. project vs. plugin

## The Key Insight

A Skill is a runbook Claude Code reads only when it needs it —
not a tool, not a standing rule, a *procedure on standby*.

---

# Lab 11: Skills

👉 Open [docs/labs/lab-11-skills.md](../../../labs/lab-11-skills.md)

**Goal for the next 30 minutes:**
- Ran `/test-data-generator` for at least one stack
- `.claude/skills/clean-architecture-boundary-check/SKILL.md` created
- Confirmed it activates on a matching prompt — and NOT on a compliant change

---

<!-- Navigation -->
[← 10 MCP Servers](02-mcp-servers.md) | **Day 2 Module 11** | [Next: Custom Subagents →](04-sub-agents.md)
