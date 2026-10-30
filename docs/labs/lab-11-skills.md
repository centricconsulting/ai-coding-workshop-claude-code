# Lab 11: Skills

**Duration**: 30–35 minutes
**Day**: 2 of 2 (Advanced)

**Learning Objectives**:
- Understand what a Skill is and how it differs from MCP, hooks, rules, and CLAUDE.md
- Learn the current SKILL.md frontmatter, argument substitution (`$ARGUMENTS`), and the progressive disclosure model
- Inspect and run a skill that already ships with this repo (`/test-data-generator`)
- Build your own skill: a Clean Architecture boundary checker
- Know where skills live (personal, project, plugin) and when to use a skill vs. CLAUDE.md, a rule, or a subagent

---

## Overview

Lab 10 gave Claude Code new tools through MCP. This lab gives Claude Code new *packaged behaviors*: reusable instructions and procedures. They load on demand, so they don't take up context on every turn the way CLAUDE.md does.

A **Skill** is a directory containing a `SKILL.md` file. Its short description is always in context, and its full body of instructions loads only when the skill is invoked, either automatically by Claude or by you typing `/skill-name`. Skills sit between CLAUDE.md (always-on context) and subagents (delegated, isolated execution, taught next in Lab 12).

> **Custom commands are now skills.** A file at `.claude/commands/deploy.md` and a skill at `.claude/skills/deploy/SKILL.md` both create `/deploy`. Existing `.claude/commands/` files keep working, but skills add a directory for supporting files, richer frontmatter, and automatic invocation. Prefer skills for new work.

---

## Prerequisites

- Completed Labs 1–10
- Familiarity with the Clean Architecture layers described in `src-dotnet/CLAUDE.md` and `src-springboot/CLAUDE.md`

---

## Part A: What Is a Skill? (8 min)

### A.1 The Extension-Mechanism Spectrum

By now you've used several ways to shape Claude Code's behavior. Skills add one more:

| Mechanism | Loaded | Best For |
|---|---|---|
| **CLAUDE.md** | Always, every turn | Context, conventions, project knowledge |
| **Rules** (`.claude/rules/*.md`) | At startup, or when Claude touches files matching `paths:` | Stack- or folder-specific conventions |
| **Hooks** | On every matching tool call or event | Deterministic side effects (format, audit, block) |
| **MCP** | Tool list at startup; tool call on use | New tool capabilities (GitHub, databases, Slack) |
| **Skills** | Description always; body on invocation | Packaged, reusable instructions and procedures |
| **Subagents** (Lab 12) | On delegation, in their own context window | Isolated, parallel, or context-heavy work |

Skills fill a specific gap. You want Claude Code to know that a procedure exists, so it can decide to use it, without paying to load the whole procedure on every turn. CLAUDE.md loads everything every turn, which doesn't scale past a handful of procedures.

> This repo already ships path-scoped rules in `.claude/rules/` (`dotnet.md`, `springboot.md`, `follow-up-questions.md`). Open one and notice the `paths:` frontmatter. Rules are always-on *conventions*; skills are on-demand *procedures*.

### A.2 Progressive Disclosure

This is the core idea behind Skills:

1. At session start, Claude Code loads every skill's **name** and **description** (plus optional `when_to_use`) into context. This is cheap and always present.
2. When a task matches a skill's description, or you type `/skill-name`, Claude Code loads the **full SKILL.md body**. Once loaded, it stays in context for the rest of the session.
3. Supporting files (templates, scripts, reference docs) load only when the body tells Claude to read them.

This is why the *description* matters more than almost anything else in a skill. It's the only part that is always loaded, and it decides whether the skill gets used at all. Keep `SKILL.md` under about 500 lines and move detail into supporting files.

### A.3 SKILL.md Anatomy

```
.claude/skills/
  <skill-name>/
    SKILL.md          ← required: frontmatter + instructions
    templates/        ← optional supporting files, linked from SKILL.md
    scripts/
```

```markdown
---
name: skill-name
description: What this skill does and, critically, WHEN to use it.
argument-hint: "[entity] [count]"
---

# Skill Body

Instructions, steps, examples, and checklists. Claude reads these once the
skill is invoked. User input arrives here: $ARGUMENTS
```

Frequently used frontmatter fields (all optional; `description` is strongly recommended):

| Field | Purpose |
|---|---|
| `name` | Command name (`/name`). Defaults to the directory name |
| `description` | What it does and when to use it. Drives automatic invocation |
| `when_to_use` | Extra trigger phrases, appended to the description. The two combined are capped at 1,536 characters |
| `argument-hint` | Autocomplete hint, for example `[issue-number]` |
| `arguments` | Named positional arguments, available as `$name` in the body |
| `disable-model-invocation` | `true` = only you can run it (good for deploys and commits). Its description is not loaded into context |
| `user-invocable` | `false` = hidden from the `/` menu; only Claude can load it (background knowledge) |
| `allowed-tools` | Tools pre-approved while the skill is active. This grants permission; it does **not** restrict other tools |
| `model`, `effort` | Override the model or effort level while the skill runs |
| `context: fork` + `agent` | Run the skill in an isolated subagent (`Explore`, `Plan`, `general-purpose`, or a custom agent) instead of inline |
| `paths` | Glob patterns that limit when Claude auto-loads the skill |

**Argument substitution:**

- `$ARGUMENTS` is everything typed after `/skill-name`. If no placeholder in the body receives the input, Claude Code appends it as `ARGUMENTS: <value>`.
- `$ARGUMENTS[0]`, or the shorthand `$0`, `$1`, are positional, with shell-style quoting.
- `$entity`, `$count`, and other names map to the `arguments:` list.
- `${CLAUDE_SKILL_DIR}` is the skill's own directory, useful for referencing bundled scripts.
- `` !`command` `` runs a shell command *before* Claude sees the skill and injects its output, for example the current `git diff`.

**`context: fork`** runs the skill in a subagent with no access to your conversation history. The skill body becomes the subagent's task. Use it for self-contained tasks ("research X and summarize"), not for reference guidelines. A forked skill with only conventions and no task returns nothing useful.

Compare this to a hook. A hook is a shell command bound to an event, with no reasoning involved. A skill is closer to a **runbook**: a set of instructions Claude Code reads and reasons over.

---

## Part B: Inspect and Use a Shipped Skill (5 min)

This repo already ships one project skill: `.claude/skills/test-data-generator/`.

### B.1 Find It

In Claude Code, run:

```
/skills
```

The menu lists every available skill and its source (project, personal, plugin, bundled). Type to filter, and press `t` to sort by token cost. Find `test-data-generator`. Bundled skills such as `/code-review`, `/simplify`, and `/debug` also appear here.

Then open the files in your editor (or ask Claude to show them):

- `.claude/skills/test-data-generator/SKILL.md`
- `.claude/skills/test-data-generator/templates/TaskTestData.cs`
- `.claude/skills/test-data-generator/templates/TaskTestData.java`

Notice:
- `arguments: [entity, count, format]` and the use of `$entity`, `$count`, `$format`, and `$ARGUMENTS` in the body
- `allowed-tools: Read Grep Glob`, which pre-approves read-only tools for this skill
- The body tells Claude to **read the current domain model first**, so the skill stays correct as later labs add priorities and due dates
- The templates are linked from SKILL.md and are only read when needed (progressive disclosure)

### B.2 Invoke It Directly

```
/test-data-generator Task 5 csharp
```

Then try the other stack and a different format:

```
/test-data-generator Task 6 theory
```

```
/test-data-generator Task 3 java
```

Check that the output uses `Task.Create(...)` / `Task.create(...)` and business methods, not object initializers or public constructors. Both `Task` types have private constructors.

### B.3 Let Claude Invoke It

Now ask in plain language without the slash command:

```
I'm about to write xUnit tests for Task status transitions. I need some realistic sample tasks in different statuses plus a few invalid titles for edge cases.
```

The description and `when_to_use` should cause Claude to load the skill on its own. If it doesn't, compare your wording to the trigger phrases in the frontmatter.

---

## Part C: Build Your Own Skill (15 min)

### C.1 The Target Problem

Both `src-dotnet/CLAUDE.md` and `src-springboot/CLAUDE.md` document the same rule: **dependencies flow inward only.**

- .NET: `Api → Application → Domain`, with `Infrastructure` implementing `Domain` interfaces
- Spring Boot: `web → application → domain`, with `infrastructure` implementing `domain` interfaces

It's easy for a code change to quietly violate this. For example, a Domain entity might reference an Infrastructure repository class, or an Application handler might import an API DTO. The check is tedious to repeat by hand on every change but easy to describe once, which makes it a good candidate for a skill instead of restating the rule in every prompt.

### C.2 Scaffold the Skill

```
Create the directory .claude/skills/clean-architecture-boundary-check/ and a SKILL.md inside it. I want to define the frontmatter first — show me the name, description, and argument-hint before writing the body.
```

Aim for frontmatter like:

```yaml
---
name: clean-architecture-boundary-check
description: Verifies that a proposed or existing code change respects Clean Architecture dependency direction (dependencies flow inward only toward Domain). Use whenever adding a reference, import, or dependency between layers in src-dotnet/ (.NET) or src-springboot/ (Spring Boot), or when asked to review a change for architecture violations.
argument-hint: "[file-or-folder]"
---
```

The description names the trigger conditions explicitly ("whenever adding a reference between layers", "when asked to review architecture"). That text is the only thing that decides whether the skill fires automatically.

### C.3 Write the Body

Ask Claude Code to draft the instructions, pointing it at the source of truth:

```
Now write the SKILL.md body. It should teach Claude Code to:
1. Use $ARGUMENTS as the file or folder to check; if empty, check the files changed in the current git diff
2. Identify which layer each file belongs to (Domain, Application, Infrastructure, API/web) based on its path, for both src-dotnet/ (.NET) and src-springboot/ (Spring Boot)
3. Check whether the change introduces a dependency that violates the inward-only rule from src-dotnet/CLAUDE.md and src-springboot/CLAUDE.md (e.g., Domain referencing Infrastructure or API types, Application referencing API/web types)
4. Report a clear pass/fail with the specific offending import or reference cited
5. Suggest the correct fix (usually: introduce or use a Domain-level interface/abstraction) when a violation is found

Use src-dotnet/CLAUDE.md and src-springboot/CLAUDE.md as the source of truth for what "inward only" means in this repo.
```

Review the generated body. It should reference concrete layer names and paths from this repo, not generic architecture advice.

> **Optional:** add `` !`git diff --name-only` `` to the body so the list of changed files is injected before Claude reads the skill. This works only if the command is allowed by your permissions.

### C.4 Test That It Activates

No restart is needed. Claude Code watches `.claude/skills/` and picks up new or edited skills within the current session. (If you create a top-level skills directory that didn't exist when the session started, run `/reload-skills`.)

Confirm it appears in `/skills`, then trigger it **automatically** with a prompt that matches the description:

```
I want to add a method to the Domain Task entity that calls the Infrastructure repository directly to check for duplicate titles. Review whether this respects our architecture boundaries before I implement it.
```

Confirm the skill actually activates. Claude Code's response should follow the SKILL.md instructions: it names the layers, cites the inward-only rule, and flags the violation. If it doesn't fire, see Troubleshooting.

Test a case that should NOT be flagged, to confirm the skill doesn't object to everything:

```
I want to add a method to the Application layer that calls an ITaskRepository interface defined in Domain. Review whether this respects our architecture boundaries.
```

Finally, invoke it **directly** with an argument:

```
/clean-architecture-boundary-check src-dotnet/src/TaskManager.Application
```

---

## Part D: Scope, Sharing, and Governance (5 min)

### D.1 Where Skills Live

| Scope | Location | Applies to |
|---|---|---|
| Personal (user) | `~/.claude/skills/<name>/SKILL.md` | All your projects on this machine |
| Project | `.claude/skills/<name>/SKILL.md` | Everyone who clones this repo (commit it) |
| Plugin | `<plugin>/skills/<name>/SKILL.md` | Wherever the plugin is enabled; invoked as `/plugin-name:skill-name` |
| Enterprise | Managed settings directory | Everyone in the organization |

When names collide, enterprise beats personal, and personal beats project. Plugin skills are namespaced, so they never collide. Skills in nested `.claude/skills/` folders (for example under `src-springboot/`) load once Claude works on files there.

Skills follow the open [Agent Skills](https://agentskills.io) standard, so a well-written `SKILL.md` is portable to other tools that support it.

### D.2 Governance

Project skills are versioned and shared automatically, so they need the same discipline described in the [Agent Governance Guide](../guides/agent-governance.md):

- **Ownership**: Treat skills like `CLAUDE.md` and review them through the normal PR process. A misleading description can make Claude activate the wrong instructions at the wrong time.
- **Review**: Check the *description* as carefully as the body. An overly broad description causes false triggers. An overly narrow one means the skill never fires.
- **Permissions**: Review `allowed-tools` carefully, because it pre-approves tools without prompting.
- **Versioning**: Skills evolve with the code they describe. If the layer boundaries in `src-dotnet/CLAUDE.md` change, `clean-architecture-boundary-check/SKILL.md` must change too.
- **Discoverability**: Point teammates to `/skills`, and list shared skills somewhere visible during onboarding.

Discuss with your table:

```
Where should the line be between "this belongs in CLAUDE.md or a rule" and "this belongs in a skill"? Use the clean-architecture-boundary-check skill we just built as the example — why is this a skill and not just a paragraph in src-dotnet/CLAUDE.md or .claude/rules/dotnet.md?
```

---

## Success Criteria

- [ ] You found `test-data-generator` in `/skills` and ran it with arguments for at least one stack
- [ ] You saw Claude load `test-data-generator` automatically from a plain-language request
- [ ] `.claude/skills/clean-architecture-boundary-check/SKILL.md` exists with a name, description, argument-hint, and full instruction body that uses `$ARGUMENTS`
- [ ] The skill activates on a matching prompt without a restart, and does NOT falsely trigger on a compliant change
- [ ] You can explain where Skills sit relative to CLAUDE.md, rules, hooks, MCP, and subagents

---

## Extension Challenge

1. Add a second skill, `.claude/skills/test-naming-convention-check/SKILL.md`. It verifies that test method names follow this repo's conventions: `MethodName_StateUnderTest_ExpectedBehavior` for .NET and `methodName_stateUnderTest_expectedBehavior` for Spring Boot (see the Testing Conventions sections of `src-dotnet/CLAUDE.md` and `src-springboot/CLAUDE.md`). Test it against a real test file in `src-dotnet/tests/` or `src-springboot/*/src/test/`.
2. Make a forked variant: add `context: fork` and `agent: Explore` to a copy of your boundary checker so it scans the whole codebase in an isolated context and returns only a summary. Compare the context usage (`/context`) with the inline version.
3. Add `disable-model-invocation: true` to a skill and confirm it disappears from automatic use but still works as `/name`.

---

## Troubleshooting

**The skill doesn't seem to activate**

- Confirm the file is at `.claude/skills/clean-architecture-boundary-check/SKILL.md`
- Run `/skills` to confirm Claude Code sees it. If the `.claude/skills/` directory didn't exist when your session started, run `/reload-skills`
- Broaden the description. If it only mentions ".NET", a Spring Boot prompt won't trigger it
- Check that you didn't set `disable-model-invocation: true`, which removes the description from Claude's context

**The skill fires on everything, even unrelated changes**

- The description is too broad. Tighten the trigger conditions by naming the specific paths, layers, or actions that should cause it to fire, or add `paths:` to limit where it auto-loads

**Arguments don't show up**

- Use `$ARGUMENTS`, `$0`, or a named argument declared in `arguments:`. An indexed placeholder with no matching argument stays as literal text

**Skill body doesn't match this repo's actual structure**

- If Claude Code generated generic Clean Architecture advice, ask it to revise the body to cite `src-dotnet/CLAUDE.md` and `src-springboot/CLAUDE.md` explicitly

---

**Next**: [Lab 12: Custom Subagents & Agent Workflows](lab-12-sub-agents.md)
