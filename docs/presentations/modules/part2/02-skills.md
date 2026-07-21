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

---

# The Extension-Mechanism Spectrum

| Mechanism | Loaded | Best For |
|---|---|---|
| **CLAUDE.md** | Every turn | Context, conventions, project knowledge |
| **Hooks** | On matching tool call | Side effects (format, audit, test) |
| **MCP** | Tool list at startup | New tool capabilities |
| **Skills** | Description always; body on demand | Packaged, reusable procedures |
| **Sub-Agents** | On delegation | Isolated / parallel / context-heavy work |

Skills are the newest row — extending the comparison you started in Module 10.

---

# Why Not Just Put It in CLAUDE.md?

CLAUDE.md loads into context on every single turn, whether relevant or not.

That's fine for a handful of always-relevant conventions. It stops scaling once you have a dozen detailed procedures that are each only relevant *sometimes* — "how to check architecture boundaries," "how to validate test naming," "how to generate a migration checklist."

**Skills solve this with progressive disclosure.**

---

# Progressive Disclosure

Two-stage loading:

1. **Always loaded** — every skill's name + description (cheap: a sentence or two each)
2. **Loaded on demand** — the full SKILL.md body, only once Claude Code decides the task matches a skill's description

If the skill references extra files (scripts, templates), those load only if the body tells Claude Code to read them — a third stage.

**The description is the only part that's always "on." If it doesn't describe the trigger conditions precisely, the skill never fires — or fires on the wrong tasks.**

---

# SKILL.md Anatomy

```
.claude/skills/
  <skill-name>/
    SKILL.md          ← required
    (optional: scripts, templates, reference docs)
```

```markdown
---
name: skill-name
description: What this does AND when to use it — the only always-loaded part.
---

# Skill Body

Full instructions Claude Code reads once the skill is invoked.
```

---

# Skills vs. Hooks vs. MCP — a Worked Comparison

| | Hook | MCP | Skill |
|---|---|---|---|
| Triggers on | Tool-call event | Explicit tool call | Matching task description |
| Involves reasoning? | No — runs a fixed command | Yes — Claude decides to call it | Yes — Claude decides to invoke, then follows it |
| Adds a new capability? | No | Yes (new tool) | No (packages an existing capability) |
| Cost when unused | None (event never fires) | Tool list stays in context | One description line stays in context |

---

# Lab 11 Preview

## What You'll Do

1. Review the extension-mechanism spectrum and where Skills fit
2. Create the first skill in this repo: `.claude/skills/clean-architecture-boundary-check/SKILL.md`
3. Write a description precise enough to trigger correctly
4. Write a body that checks Domain → Application → Infrastructure → API dependency direction from `src/CLAUDE.md` / `src-springboot/CLAUDE.md`
5. Restart Claude Code and confirm the skill activates — and doesn't false-trigger
6. Discuss packaging and governing shared skills across a team

## The Key Insight

A Skill is a runbook Claude Code reads only when it needs it —
not a tool, not a standing rule, a *procedure on standby*.

---

# Lab 11: Skills

👉 Open [docs/labs/lab-11-skills.md](../../../labs/lab-11-skills.md)

**Goal for the next 25 minutes:**
- `.claude/skills/clean-architecture-boundary-check/SKILL.md` created
- Confirmed it activates on a matching prompt
- Confirmed it does NOT fire on a compliant change

---

<!-- Navigation -->
[← 10 MCP Servers](01-mcp-servers.md) | **Day 2 Module 11** | [Next: Sub-Agents →](03-sub-agents.md)
