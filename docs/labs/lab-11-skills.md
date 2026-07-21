# Lab 11: Skills

**Duration**: 25–30 minutes
**Day**: 2 of 2 (Advanced)

**Learning Objectives**:
- Understand what a Skill is and how it differs from MCP, hooks, and CLAUDE.md
- Learn the SKILL.md format and the progressive disclosure model
- Create the first skill in this repo: a Clean Architecture boundary checker
- Know when to package instructions as a skill vs. putting them in CLAUDE.md

---

## Overview

Lab 10 gave Claude Code new tools via MCP. This lab gives Claude Code new *packaged behaviors* — reusable, on-demand instructions that activate automatically when relevant, without bloating every prompt or permanently loading into context the way CLAUDE.md does.

A **Skill** is a directory containing a `SKILL.md` file: a short description (always in context) plus a full body of instructions (loaded only when the skill is actually invoked). Skills sit between CLAUDE.md (always-on context) and Sub-Agents (delegated, isolated execution, taught next in Lab 12) on the extension-mechanism spectrum you started building in Lab 10.

---

## Prerequisites

- Completed Labs 1–10
- Familiarity with the Clean Architecture layers described in `src/CLAUDE.md` and `src-springboot/CLAUDE.md`

---

## Part A: What Is a Skill? (8 min)

### A.1 The Extension-Mechanism Spectrum

By now you've used several ways to shape Claude Code's behavior. Skills add one more:

| Mechanism | Loaded | Best For |
|---|---|---|
| **CLAUDE.md** | Always, every turn | Context, conventions, project knowledge |
| **Hooks** | On every matching tool call | Side effects (format, audit, test) |
| **MCP** | Tool list at startup; tool body on call | New tool capabilities (query GitHub, DB, Slack) |
| **Skills** | Description always; body on invocation | Packaged, reusable instructions/procedures |
| **Sub-Agents** (Lab 12) | On delegation | Isolated, parallel, or context-heavy work |

Skills fill a specific gap: you want Claude Code to know a procedure exists — so it can decide to use it — without paying the context cost of loading the whole procedure on every single turn. That's what CLAUDE.md does, and it doesn't scale past a handful of procedures.

### A.2 Progressive Disclosure

This is the core idea behind Skills:

1. Claude Code loads every skill's **name** and **description** into context at session start — cheap, always there.
2. When a task matches a skill's description, Claude Code reads the **full SKILL.md body** — the actual instructions — only at that moment.
3. If the skill references additional files (scripts, templates, reference docs), those load only if the body tells Claude Code to read them.

This is why the *description* field matters more than almost anything else in a skill — it's the only part of the skill that's "always on," and it's what determines whether the skill ever gets used at all.

### A.3 SKILL.md Anatomy

```
.claude/skills/
  <skill-name>/
    SKILL.md          ← required: frontmatter + instructions
    (optional supporting files: scripts, templates, reference docs)
```

```markdown
---
name: skill-name
description: One or two sentences describing what this skill does and, critically, WHEN to use it. This is the only part loaded into every context — write it to trigger correctly.
---

# Skill Body

Full instructions, steps, examples, checklists — anything Claude Code
needs once the skill is actually invoked.
```

Compare this to a hook: a hook is a shell command bound to a tool-call event, with no reasoning involved. A skill is closer to a **runbook** — a set of instructions Claude Code reads and reasons over.

---

## Part B: Build the First Skill in This Repo (15 min)

No `.claude/skills/` directory exists anywhere in this repo yet — you're creating it.

### B.1 The Target Problem

Both `src/CLAUDE.md` and `src-springboot/CLAUDE.md` document the same rule: **dependencies flow inward only.**

- .NET: `Api → Application → Domain`, with `Infrastructure` implementing `Domain` interfaces
- Spring Boot: `web → application → domain`, with `infrastructure` implementing `domain` interfaces

It's easy for a code change to quietly violate this — e.g., a Domain entity that references an Infrastructure repository class, or an Application handler that imports a web/API DTO. This is exactly the kind of check that's tedious to repeat by hand on every change but easy to describe once, which makes it a good candidate for a skill instead of restating the rule in every prompt.

### B.2 Scaffold the Skill

```
Create the directory .claude/skills/clean-architecture-boundary-check/ and a SKILL.md inside it. I want to define the frontmatter first — show me the name and description before writing the body.
```

Aim for a description like:

```yaml
name: clean-architecture-boundary-check
description: Verifies that a proposed or existing code change respects Clean Architecture dependency direction (Domain → Application → Infrastructure → API, dependencies flow inward only). Use this whenever adding a reference, import, or dependency between layers in src/ (.NET) or src-springboot/ (Spring Boot), or when asked to review a change for architecture violations.
```

The description should name the trigger conditions explicitly — "whenever adding a reference between layers," "when asked to review architecture" — since that's the only text deciding whether this skill fires.

### B.3 Write the Body

Ask Claude Code to draft the instructions, pointing it at the source of truth:

```
Now write the SKILL.md body. It should teach Claude Code to:
1. Identify which layer a changed file belongs to (Domain, Application, Infrastructure, API/web) based on its path, for both src/ (.NET) and src-springboot/ (Spring Boot)
2. Check whether the change introduces a dependency that violates the inward-only rule from src/CLAUDE.md and src-springboot/CLAUDE.md (e.g., Domain referencing Infrastructure or API types, Application referencing API/web types)
3. Report a clear pass/fail with the specific offending import or reference cited
4. Suggest the correct fix (usually: introduce or use a Domain-level interface/abstraction) when a violation is found

Use src/CLAUDE.md and src-springboot/CLAUDE.md as the source of truth for what "inward only" means in this repo.
```

Review the generated body. It should reference concrete layer names and paths from this repo, not generic architecture advice.

### B.4 Test That It Activates

Restart Claude Code so it picks up the new skill:

```bash
/exit
claude
```

Trigger it with a prompt that should match the skill's description:

```
I want to add a method to the Domain Task entity that calls the Infrastructure repository directly to check for duplicate titles. Review whether this respects our architecture boundaries before I implement it.
```

Confirm the skill actually activates — Claude Code's response should reflect the SKILL.md instructions (naming the layers, citing the inward-only rule, flagging the violation). If it doesn't fire, see Troubleshooting.

Also test a case that should NOT trigger a violation, to confirm the skill doesn't just reflexively object to everything:

```
I want to add a method to the Application layer that calls an ITaskRepository interface defined in Domain. Review whether this respects our architecture boundaries.
```

---

## Part C: Packaging and Sharing Skills Across a Team (5 min)

Skills live in `.claude/skills/` inside the repo, so like `CLAUDE.md` and `.claude/settings.json`, they're versioned and shared automatically for everyone who clones the project — no separate distribution step.

That also means they need the same governance discipline described in the [Agent Governance Guide](../guides/agent-governance.md):

- **Ownership**: who can add or edit a shared skill? Treat it like `CLAUDE.md` — reviewable via the normal PR process, since a bad or misleading skill description can cause Claude Code to activate the wrong instructions at the wrong time.
- **Review**: a reviewer should check the skill's *description* as carefully as its body — an overly broad description causes false triggers; an overly narrow one means the skill never fires when it should.
- **Versioning**: skills evolve with the codebase they document. If `src/CLAUDE.md`'s layer boundaries change, `clean-architecture-boundary-check/SKILL.md` needs to change with it — the same "keep CLAUDE.md current" discipline applies.
- **Discoverability**: unlike a hook (which just runs) or CLAUDE.md (which is always visible), a skill's existence is easy for teammates to forget. Document available skills somewhere onboarding-visible, the same way you'd document available MCP servers.

Discuss with your table:

```
Where should the line be between "this belongs in CLAUDE.md" and "this belongs in a skill"? Use the clean-architecture-boundary-check skill we just built as the example — why is this a skill and not just a paragraph in src/CLAUDE.md?
```

---

## Success Criteria

- [ ] `.claude/skills/clean-architecture-boundary-check/SKILL.md` exists with a name, description, and full instruction body
- [ ] You restarted Claude Code and confirmed the skill activates on a matching prompt
- [ ] You confirmed the skill does NOT falsely trigger on a compliant change
- [ ] You can explain where Skills sit relative to MCP, hooks, and CLAUDE.md on the extension-mechanism spectrum

---

## Extension Challenge

Add a second skill: `.claude/skills/test-naming-convention-check/SKILL.md`, which verifies that test method names in a proposed change follow this repo's conventions (`MethodName_StateUnderTest_ExpectedBehavior` for .NET, `methodName_stateUnderTest_expectedBehavior` for Spring Boot — see the Testing Conventions sections of `src/CLAUDE.md` and `src-springboot/CLAUDE.md`). Test it against a real test file in `tests/` or `src-springboot/src/test/`.

---

## Troubleshooting

**The skill doesn't seem to activate**

- Confirm the file is at `.claude/skills/clean-architecture-boundary-check/SKILL.md` — the directory name and frontmatter should match
- Restart Claude Code — skills are loaded at session start, not picked up mid-session
- Broaden the description — if it's too narrow (e.g., only mentions ".NET"), a Spring Boot-flavored prompt won't trigger it
- Ask directly: `Which skills are currently available to you?` — confirms Claude Code sees the skill at all

**The skill fires on everything, even unrelated changes**

- The description is too broad. Tighten the trigger conditions — name the specific file paths, layers, or actions that should cause it to fire

**Skill body doesn't match this repo's actual structure**

- If Claude Code generated generic Clean Architecture advice instead of referencing `src/CLAUDE.md` / `src-springboot/CLAUDE.md` directly, ask it to revise the body to cite those files explicitly

---

**Next**: [Lab 12: Sub-Agents & Parallel Workflows](lab-12-sub-agents.md)
