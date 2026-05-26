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
# src/CLAUDE.md

## C# Conventions
- File-scoped namespaces
- Sealed classes by default
- Guard clauses — no else branches
- async/await throughout — never .Result or .Wait()
- Records for DTOs and value objects
- No abbreviations in identifiers
```

Claude Code reads this at the start of every session.
**You teach it once. It applies those lessons forever.**

---

# The Hierarchy

```
~/.claude/CLAUDE.md              ← Personal (you, all projects)

project-root/CLAUDE.md           ← Project overview, build commands,
                                    what NOT to do

src/CLAUDE.md                    ← .NET-specific conventions

src-springboot/CLAUDE.md         ← Java-specific conventions

tests/CLAUDE.md                  ← Test-specific conventions
```

**More specific files win** when there are conflicts.

Claude Code reads all files in the hierarchy from root to the
current working file.

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

If it would be stale in 2 weeks, it doesn't belong.

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

**With `src/CLAUDE.md`** — Claude Code produces:

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

# How Claude Code Loads CLAUDE.md

1. Reads `~/.claude/CLAUDE.md` first (personal)
2. Reads `project-root/CLAUDE.md` (project overview)
3. As it reads files in `src/`, loads `src/CLAUDE.md`
4. If it reads a file in `src/TaskManager.Domain/`, `src/CLAUDE.md` still applies

**You can verify this:** ask Claude Code *"What conventions are you following for this project?"* It will describe what it found in your CLAUDE.md files.

---

# CLAUDE.md vs. GitHub Copilot Instructions

**Copilot approach:**
```
.github/copilot-instructions.md      ← project-wide
.github/instructions/*.instructions.md  ← file-pattern scoped
```

**Claude Code approach:**
```
project-root/CLAUDE.md               ← project-wide
src/CLAUDE.md                        ← directory-scoped
tests/CLAUDE.md                      ← directory-scoped
~/.claude/CLAUDE.md                  ← personal/global
```

The concepts are parallel. The key difference: Claude Code's hierarchy
is **directory-depth based**, giving very fine-grained control.

---

# Lab 2 Preview

## What You'll Do

1. Read the existing CLAUDE.md files in this repo
2. Ask Claude Code: *"What conventions are you following?"*
3. Add a convention to `src/CLAUDE.md` that's currently missing
4. Verify Claude Code applies the new convention without being told

## The Key Experiment

Remove a convention from CLAUDE.md. Ask Claude Code to generate code.
See what it produces. Then add it back and repeat.

**You'll see exactly how much CLAUDE.md affects output.**

---

# Lab 2: CLAUDE.md & Project Memory

👉 Open [docs/labs/lab-02-claude-md-memory.md](../../../labs/lab-02-claude-md-memory.md)

**Goal for the next 30 minutes:**
- You understand what's in each CLAUDE.md file and why
- You've added at least one new convention
- You've verified Claude Code applied it without prompting

---

<!-- Navigation -->
[← 01 Features Tour](01-claude-code-features-tour.md) | **Part 1 Module 02** | [Next: TDD →](03-tdd-with-claude-code.md)
