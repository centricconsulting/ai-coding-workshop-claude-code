# Lab 2: CLAUDE.md & Project Memory

**Duration**: 30–35 minutes
**Part**: 1 of 2 (Fundamentals)

**Learning Objectives**:
- Understand the CLAUDE.md hierarchy: project / subdirectory / user
- Know what belongs in CLAUDE.md vs. what doesn't
- Write a CLAUDE.md that measurably improves Claude Code's output
- Understand how CLAUDE.md differs from GitHub Copilot Instructions

> **Compare to Copilot**: Copilot uses `.github/instructions/` files with `applyTo` glob patterns — context that loads based on the file being edited. CLAUDE.md is project-level memory that Claude Code reads at session start and references throughout the conversation, regardless of which file is currently open.

---

## Prerequisites

- Completed Lab 1
- Personal branch checked out: `git checkout -b workshop/your-name`

---

## Part A: Understanding CLAUDE.md (8 min)

### A.1 Read the Existing CLAUDE.md Files

This workshop repo already has three CLAUDE.md files. Start Claude Code and ask about them:

```bash
claude
```

```
Read all three CLAUDE.md files in this project (root, src/, and src-springboot/) and explain what each one covers and why they're organized that way.
```

Claude Code will read all three and give you a structured explanation.

### A.2 The CLAUDE.md Hierarchy

CLAUDE.md files follow a directory-based hierarchy. Claude Code reads all applicable CLAUDE.md files based on the current working directory:

```
~/CLAUDE.md                          ← User-level (applies everywhere)
~/src/CLAUDE.md                      ← Repo-level (for all work in this repo)
~/src/ai-code-workshop/CLAUDE.md     ← Project root (this workshop)
~/src/ai-code-workshop/src/CLAUDE.md ← Subdirectory (overrides for .NET work)
```

When Claude Code starts in `ai-code-workshop-claude-code/src/`, it reads:
1. `~/CLAUDE.md` (if it exists)
2. `ai-code-workshop-claude-code/CLAUDE.md`
3. `ai-code-workshop-claude-code/src/CLAUDE.md`

More specific files win when there are conflicts.

### A.3 What Belongs in CLAUDE.md?

**Good CLAUDE.md content:**
- Build commands (`dotnet build`, `mvn test`)
- Architecture overview and layer responsibilities
- Coding conventions (naming, style, patterns)
- What NOT to do (common mistakes, things to avoid)
- Key file locations and their purpose
- Domain vocabulary (so Claude Code uses your terminology)
- Constraints (no database migrations, don't modify X)

**What does NOT belong:**
- Code patterns derivable from reading the source
- Git history or recent changes
- PR descriptions or task details
- Debugging solutions or fix recipes
- Anything that changes weekly

The test: *would this still be true and useful in 6 months?* If yes, it belongs.

---

## Part B: Observing CLAUDE.md in Action (7 min)

### B.1 Start Without CLAUDE.md Context

Open a new Claude Code session (`/clear` or restart) and try this without any CLAUDE.md:

```
Create a simple TaskService in the Application layer.
```

Note the response — Claude Code will have to guess at conventions.

### B.2 Compare With CLAUDE.md Context

Now, still in the same session (don't clear), ask:

```
Read src/CLAUDE.md and then create a TaskService that follows all the conventions described there.
```

Notice the difference: Claude Code now uses file-scoped namespaces, sealed classes, guard clauses, and async patterns because `src/CLAUDE.md` specifies them.

### B.3 The Automatic Load

In normal usage, Claude Code reads CLAUDE.md files automatically when it starts. You don't need to explicitly ask. Start a fresh session:

```bash
/clear
```

```
Create a simple TaskService in the Application layer.
```

This time Claude Code should follow conventions from `src/CLAUDE.md` automatically, because it reads all applicable CLAUDE.md files at startup.

> **Key insight**: CLAUDE.md removes the need to re-state your conventions in every prompt. You write it once; Claude Code reads it every session.

---

## Part C: Writing CLAUDE.md for a Subdirectory (12 min)

The `tests/` directory currently has no CLAUDE.md. You'll write one.

### C.1 Ask Claude Code to Explore the Tests

```
Read the test projects in the tests/ directory and describe the current test structure, what frameworks are used, and what testing patterns you observe.
```

Claude Code will read the test project files and give you a summary.

### C.2 Draft a CLAUDE.md for the Tests Directory

Ask Claude Code to help you write it:

```
Based on what you found in the tests/ directory, draft a CLAUDE.md for tests/ that captures: the testing frameworks, the naming conventions, the project structure, and any important "don't do this" rules for tests in this codebase.
```

Review the draft. It should include things like:
- xUnit with FakeItEasy
- Test naming: `MethodName_StateUnderTest_ExpectedBehavior`
- Arrange/Act/Assert with blank line separators
- Not to mock things that don't need mocking

### C.3 Write and Verify

Ask Claude Code to write the file:

```
Write this as tests/CLAUDE.md.
```

Verify it was created:

```bash
cat tests/CLAUDE.md
```

Then test that it works:

```
/clear
```

```
Write a test for a hypothetical TaskService.GetTask method that returns null when the task doesn't exist.
```

The test should now follow the conventions in `tests/CLAUDE.md` without you needing to specify them.

---

## Part D: User-Level CLAUDE.md (5 min)

Some preferences apply everywhere, not just this project. Claude Code supports a user-level CLAUDE.md at `~/.claude/CLAUDE.md`.

### D.1 Examine What Would Go There

Think about conventions you want across *all* your projects:
- Your preferred commit message style
- Languages you want to avoid (e.g., "never suggest shell scripts when Python would work")
- Personal style preferences
- Things you always want Claude Code to remember about you

### D.2 Create a User-Level CLAUDE.md

```bash
mkdir -p ~/.claude
```

Create `~/.claude/CLAUDE.md` with your personal preferences. A minimal starting point:

```markdown
# Personal Claude Code Preferences

## Communication Style
- Be concise. I prefer short answers with code examples over long explanations.
- When suggesting multiple approaches, lead with your recommendation.

## Code Preferences
- Conventional Commits for all commit messages
- Ask before adding new dependencies
- Prefer explicit over implicit
```

You won't use this file in the remaining labs, but it's a good habit to start.

---

## Reflection: CLAUDE.md vs Copilot Instructions

| | Copilot Instructions (`.github/instructions/`) | Claude Code CLAUDE.md |
|---|---|---|
| **When loaded** | When a matching file is opened | At session start, for all tasks |
| **Scope** | Per-file-type via `applyTo:` glob | Per-directory and below |
| **User-level** | Not supported (repo-only) | `~/.claude/CLAUDE.md` |
| **Format** | Markdown with YAML frontmatter | Plain markdown |
| **Effect** | Influences inline completions + chat | Influences all Claude Code actions |

The deeper difference: Copilot Instructions tune *suggestions*. CLAUDE.md tunes an *agent's judgment* across everything it does — file reads, writes, commands, and reasoning.

---

## Success Criteria

- [ ] You can explain the CLAUDE.md hierarchy (user → project → subdirectory)
- [ ] You wrote a `tests/CLAUDE.md` that Claude Code uses to generate better tests
- [ ] You understand what belongs in CLAUDE.md vs. what doesn't
- [ ] You created (or planned) a `~/.claude/CLAUDE.md` for personal preferences

---

## Extension Exercise

Add a "Domain Vocabulary" section to the root `CLAUDE.md` that defines the bounded context terms used in this project (Task, Priority, Assignee, Notification, etc.) and ask Claude Code to implement a new `Assignee` value object. Observe how the vocabulary shapes the output.

---

## Troubleshooting

**Claude Code isn't following CLAUDE.md conventions**
- Try `/clear` to start a fresh session — CLAUDE.md is read at startup
- Verify the file is in the right directory: run `find . -name "CLAUDE.md"` from the repo root
- Check for syntax issues: CLAUDE.md is plain markdown, not YAML

**CLAUDE.md changes not taking effect**
- Start a new session with `/clear` or restart `claude` — changes to CLAUDE.md take effect at session start
- If you're in a subdirectory, Claude Code reads CLAUDE.md files up the directory tree

**Conflicting instructions across CLAUDE.md files**
- More specific (deeper) files win over more general ones
- If both say different things, Claude Code generally uses the more specific one

---

**Next**: [Lab 3: TDD with Claude Code](lab-03-tdd-with-claude-code.md)
