---
marp: true
theme: default
paginate: true
backgroundColor: #fff
---

# Module 02: CLAUDE.md & Project Memory

## Teaching Claude Code Your Team's Standards Once

---

# The Problem Without CLAUDE.md

Every session, Claude Code starts cold. Without guidance:

```
You: Write a query handler for fetching tasks by priority.

Claude Code: [creates a class with public fields, no async,
              underscore-prefixed params, controller-style
              comments — none of which match your standards]

You: We use records, not classes for query results
You: Everything should be async
You: No underscore prefixes
You: We don't write XML doc comments
...
```

**You're teaching the same lessons every session.**

---

# CLAUDE.md Is Project Memory

```markdown
# src-dotnet/CLAUDE.md

## C# Conventions
- File-scoped namespaces
- Sealed classes by default
- Guard clauses — no else branches
- async/await throughout — never .Result or .Wait()
- Records for DTOs and value objects
- No abbreviations in identifiers
```

Claude Code loads it automatically whenever it works in `src-dotnet/`.
**You teach it once. It applies those lessons forever.**

---

# The Memory Model

| Scope | Location | Shared with |
|---|---|---|
| Managed policy | OS-level path set by IT | Whole organization |
| User | `~/.claude/CLAUDE.md`, `~/.claude/rules/` | You, all projects |
| Project | `./CLAUDE.md` **or** `./.claude/CLAUDE.md` | Team, via git |
| Project rules | `./.claude/rules/*.md` (`paths:` optional) | Team, via git |
| Local | `./CLAUDE.local.md` (keep out of git) | You, this project |
| Subdirectory | `src-dotnet/CLAUDE.md`, `src-dotnet/tests/CLAUDE.md` | Team, via git |

Plus **auto memory** — notes Claude writes for itself.

---

# How Files Load

- **At launch**: `CLAUDE.md` / `CLAUDE.local.md` in the working dir **and every parent**, plus rules with **no** `paths:`
- **On demand**: a subdirectory `CLAUDE.md` or a path-scoped rule loads the first time Claude reads/edits a matching file
- **Concatenated, not overridden** — broader files first, closer files last

**Conflicts aren't resolved by precedence** — Claude may follow either. Remove them.

Check what's loaded: `/context` → **Memory files**

---

# Path-Scoped Rules: `.claude/rules/`

```markdown
---
paths:
  - "src-dotnet/src/**/*.cs"
  - "src-dotnet/tests/**/*.cs"
---
# .NET Conventions
- File-scoped namespaces, sealed by default, guard clauses ...
```

Shipped in this repo:

| Rule | Loads when Claude touches… |
|---|---|
| `dotnet.md` | `src-dotnet/src/**/*.cs`, `src-dotnet/tests/**/*.cs`, `**/*.csproj` |
| `springboot.md` | `src-springboot/**/*.java`, `pom.xml`, `*.{yml,yaml,properties}` |
| `follow-up-questions.md` | *(no `paths:` → every session)* |

`paths` is the only frontmatter field rules use. Globs support `**` and `{a,b}`.

---

# `@path` Imports

```markdown
# src-dotnet/tests/CLAUDE.md
Domain-layer rules: @../../.claude/rules/domain-layer.md
Git workflow: @docs/git-instructions.md
```

- Relative to the importing file; nest up to 4 hops
- Organizes files — **doesn't save context** (imports load in full)
- Imports from outside the repo trigger a one-time approval dialog
- Wrap in backticks to mention a path without importing it

---

# What Belongs in CLAUDE.md

**High-value content:**

- Build and test commands (`dotnet build`, `mvn clean test`)
- Architecture rules ("dependencies flow inward only")
- Naming and coding conventions
- What NOT to do ("do not add packages without asking")
- Key file locations ("the domain aggregate is `Task.cs`")
- Technology choices ("xUnit + FakeItEasy, not Moq")

---

# What Does NOT Belong in CLAUDE.md

**Don't put these in CLAUDE.md:**

| Don't Include | Why |
|---|---|
| Specific task instructions | Belongs in the prompt |
| Current sprint goals | Changes every sprint |
| "Fix the bug in X" | That's a prompt, not project memory |
| Long prose explanations | Bullet points; Claude Code doesn't need narrative |
| Secrets or API keys | Never in files |
| Rules for one part of the codebase | Use a path-scoped rule |
| Multi-step procedures | Use a skill (Lab 11) |

If it would be stale in 2 weeks, it doesn't belong.
Keep each file **under ~200 lines** — longer files reduce adherence.

---

# The "What NOT to Do" Section

This is the **highest-value section** most teams skip.

```markdown
## What NOT to Do

- Do not add NuGet packages without asking first
- Do not modify .devcontainer/ files during labs
- Do not commit directly to main
- Do not use field injection (@Autowired on fields)
- Do not run dotnet ef database update (no migrations)
- Do not write XML doc comments — we don't use them
```

Claude Code is helpful. It will add packages, run migrations,
and write doc comments if you don't tell it not to.

---

# Before and After: A Real Example

**Without CLAUDE.md** — Claude Code produces:

```csharp
/// <summary>Creates a new task.</summary>
/// <param name="command">The create task command.</param>
public async Task<TaskDto> Handle(CreateTaskCommand command)
{
    var task = new TaskManager.Domain.Entities.Task();
    task.Title = command.Title;
    // ...
```

**With `src-dotnet/CLAUDE.md`** — Claude Code produces:

```csharp
public async Task<TaskDto> Handle(CreateTaskCommand command)
{
    if (command is null) throw new ArgumentNullException(nameof(command));
    var task = Task.Create(command.Title, command.DueDate);
    // ...
```

---

# User-Level CLAUDE.md

`~/.claude/CLAUDE.md` applies to **all your projects**.

Good content for personal CLAUDE.md:
```markdown
## My Preferences

- I prefer short, direct responses — no multi-paragraph explanations
- Don't add trailing summaries unless I ask for them
- When in doubt, ask rather than assume
- I always want to see the diff before you write files

## My Stack
I work primarily in C# and Java. I know both well.
Explain framework-specific things, not language basics.
```

---

# Personal Memory: Local and Auto

**`CLAUDE.local.md`** — your notes for *this* repo (sandbox URLs, "I'm on the Spring track"). Keep it out of git.

**Auto memory** — on by default. Claude saves what it learns about *you*:

```
~/.claude/projects/<project>/memory/
├── MEMORY.md            ← index; first 200 lines / 25 KB load every session
└── feedback_testing.md  ← topic files, read on demand
```

- *"Remember that I prefer…"* → auto memory (you, this machine)
- *"Add this to CLAUDE.md"* → shared with the team

---

# The Memory Commands

| Command | What it does |
|---|---|
| `/memory` | Open/create any memory file; toggle auto memory; open the auto memory folder |
| `/context` | See what's in context, including **Memory files** |
| `/init` | Generate a starter `CLAUDE.md` (or suggest improvements to an existing one) |

**You can verify loading:** ask *"Which project instructions apply to this file, and where did each come from?"*

**Remember:** memory is **context, not enforcement**. To guarantee a behavior, use permissions or hooks (Lab 9).

---

# Lab 2 Preview

## What You'll Do

1. Read the shipped `CLAUDE.md` files and `.claude/rules/`
2. Use `/context` and `/memory` to see what's loaded
3. Watch a path-scoped rule load — then write your own (`domain-layer.md`)
4. Write `src-dotnet/tests/CLAUDE.md` with an `@path` import
5. Set up personal memory: `~/.claude/CLAUDE.md`, `CLAUDE.local.md`, auto memory

## The Key Experiment

Ask the same question outside the repo and inside it.
**You'll see exactly how much memory affects output.**

---

# Lab 2: CLAUDE.md & Project Memory

👉 Open [docs/labs/lab-02-claude-md-memory.md](../../../labs/lab-02-claude-md-memory.md)

**Goal for the next 40 minutes:**
- You understand what's in each memory file, and when it loads
- You've added a path-scoped rule and a subdirectory `CLAUDE.md`
- You've verified Claude Code applied them without prompting

---

<!-- Navigation -->
[← 01 Features Tour](01-claude-code-features-tour.md) | **Day 1 Module 02** | [Next: TDD →](03-tdd-with-claude-code.md)
