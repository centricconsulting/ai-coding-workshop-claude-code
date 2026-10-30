# Lab 2: CLAUDE.md & Project Memory

**Duration**: 40–45 minutes
**Day**: 1 of 2 (Fundamentals)

**Learning Objectives**:
- Understand Claude Code's memory model: user, project, local, and subdirectory `CLAUDE.md` files, plus path-scoped rules in `.claude/rules/`
- Know what belongs in `CLAUDE.md` vs. a path-scoped rule vs. auto memory
- Use `@path` imports, `/memory`, `/context`, and `/init`
- Write instructions that measurably improve Claude Code's output

---

## Prerequisites

- Completed Lab 1
- Personal branch checked out: `git checkout -b workshop/your-name`

---

## Part A: Understanding the Memory Model (10 min)

### A.1 Read the Existing Memory Files

This workshop repo ships three `CLAUDE.md` files (root, `src-dotnet/`, `src-springboot/`) and a set of path-scoped rules in `.claude/rules/`. Start Claude Code and ask about them:

```bash
claude
```

```
Read the CLAUDE.md files in this project (root, src-dotnet/, and src-springboot/) and the files in .claude/rules/. Explain what each one covers, when each one is loaded, and why they're organized that way.
```

Claude Code will read them and give you a structured explanation.

### A.2 Where Memory Lives

Claude Code has two complementary memory systems, both loaded at the start of every session:

| | `CLAUDE.md` files and rules | Auto memory |
|---|---|---|
| **Who writes it** | You (and your team) | Claude |
| **What it contains** | Instructions and conventions | Learnings: your preferences, corrections, project context it can't derive from code |
| **Where it lives** | In the repo and `~/.claude/` | `~/.claude/projects/<project>/memory/` (machine-local) |

The instruction files live in several places, each with a different scope:

| Scope | Location | Shared with |
|---|---|---|
| **Managed policy** | OS-level path set by IT (e.g., `/Library/Application Support/ClaudeCode/CLAUDE.md` on macOS) | Everyone in the organization |
| **User** | `~/.claude/CLAUDE.md` (and `~/.claude/rules/`) | Only you, all projects |
| **Project** | `./CLAUDE.md` **or** `./.claude/CLAUDE.md` | Your team, via git |
| **Project rules** | `./.claude/rules/*.md` (optionally scoped with `paths:` frontmatter) | Your team, via git |
| **Local** | `./CLAUDE.local.md` — keep it out of git | Only you, this project |
| **Subdirectory** | `src-dotnet/CLAUDE.md`, `src-dotnet/tests/CLAUDE.md`, … | Your team, via git |

> 💡 Repos that already use `AGENTS.md` for other coding tools are supported too — see the [memory docs](https://code.claude.com/docs/en/memory) for how `AGENTS.md` and `CLAUDE.md` interact.

### A.3 How the Files Load

- **At launch**, Claude Code loads `CLAUDE.md` and `CLAUDE.local.md` from your working directory **and every directory above it**, plus any rule in `.claude/rules/` that has **no** `paths:` frontmatter.
- **On demand**, it loads a subdirectory's `CLAUDE.md` (e.g., `src-dotnet/CLAUDE.md`) the first time Claude reads or edits a file in that subdirectory, and a **path-scoped rule** the first time Claude reads or edits a file matching its `paths:` globs.
- Files are **concatenated, not overridden**. Broader files appear first; files closer to where you're working are read last. If two files contradict each other, Claude may pick either one — so the fix for a conflict is to remove it, not to rely on precedence.

So when you start `claude` at the repo root and ask it to change `src-dotnet/src/TaskManager.Domain/Tasks/Task.cs`, its context contains:

1. `~/.claude/CLAUDE.md` (if it exists)
2. `./CLAUDE.md` + any unscoped rules (e.g., `.claude/rules/follow-up-questions.md`)
3. `./CLAUDE.local.md` (if it exists)
4. `src-dotnet/CLAUDE.md` and `.claude/rules/dotnet.md` — pulled in once Claude touches a `.cs` file under `src-dotnet/`

### A.4 Check What's Actually Loaded

Two built-in commands show you the memory system at work:

```
/context
```

`/context` visualizes what's filling the context window — look for the **Memory files** section to see which `CLAUDE.md` and rules files loaded at launch.

```
/memory
```

`/memory` lists every memory file location (user, project, local — including ones that don't exist yet), opens any of them in your editor, and lets you toggle **auto memory** and open the auto memory folder.

### A.5 What Belongs Where?

**Good `CLAUDE.md` content** (facts Claude should hold in *every* session):
- Build and test commands (`dotnet build`, `mvn test`)
- Architecture overview and layer responsibilities
- Coding conventions (naming, style, patterns)
- What NOT to do (common mistakes, things to avoid)
- Key file locations and their purpose
- Domain vocabulary (so Claude Code uses your terminology)
- Constraints (no database migrations, don't modify X)

**Move it to a path-scoped rule** when it only matters for part of the codebase (".NET conventions", "domain-layer rules").
**Move it to a skill** (Lab 11) when it's a multi-step procedure you only need sometimes.

**What does NOT belong in any of them:**
- Code patterns derivable from reading the source
- Git history or recent changes
- PR descriptions or task details
- Debugging solutions or fix recipes
- Anything that changes weekly

The test: *would this still be true and useful in 6 months?* If yes, it belongs. Keep each `CLAUDE.md` short — the docs recommend **under ~200 lines** per file; long files cost context and reduce adherence.

---

## Part B: Observing CLAUDE.md in Action (7 min)

### B.1 A Prompt Outside the Project

To see Claude Code *without* project memory, start a session in an empty folder outside the repo:

```bash
mkdir -p ~/claude-scratch && cd ~/claude-scratch && claude
```

```
Sketch a TaskService class for a .NET Clean Architecture Application layer.
```

Note the response — Claude Code has to guess at conventions. Exit (`/exit`) and return to the workshop repo.

### B.2 The Automatic Load

Back in the workshop repo, start a fresh session:

```bash
cd <path-to-workshop-repo> && claude
```

```
Look at src-dotnet/src/TaskManager.Application, then sketch a TaskService class for the .NET Application layer. Don't write any files — just show me the code.
```

This time Claude Code should follow conventions from `CLAUDE.md`, `src-dotnet/CLAUDE.md`, and `.claude/rules/dotnet.md` (file-scoped namespaces, sealed classes, guard clauses, async patterns) without you restating them — the root file loads at launch, and the `src-dotnet/` file and `.cs` rule load automatically as soon as Claude reads a file under `src-dotnet/`.

> **Key insight**: Memory removes the need to re-state your conventions in every prompt. You write it once; Claude Code reads it every session.

---

## Part C: Path-Scoped Rules (10 min)

`CLAUDE.md` in a subdirectory scopes instructions by *folder*. Rules in `.claude/rules/` scope them by *file pattern* — they're Claude Code's equivalent of "apply these instructions only to `*.cs` files".

### C.1 Inspect the Shipped Rules

```bash
ls .claude/rules/
head -15 .claude/rules/dotnet.md
```

A path-scoped rule is a markdown file with a `paths:` list in YAML frontmatter:

```markdown
---
paths:
  - "src-dotnet/src/**/*.cs"
  - "src-dotnet/tests/**/*.cs"
---

# .NET Conventions
- ...
```

| Shipped rule | `paths:` | Loads when… |
|---|---|---|
| `dotnet.md` | `src-dotnet/src/**/*.cs`, `src-dotnet/tests/**/*.cs`, `**/*.csproj` | Claude reads/edits a C# or project file |
| `springboot.md` | `src-springboot/**/*.java`, `pom.xml`, `*.{yml,yaml,properties}` | Claude reads/edits Java, POM, or config files in the Spring Boot tree |
| `follow-up-questions.md` | *(none)* | Every session, at launch |

Notes:
- `paths` is the **only** frontmatter field Claude Code reads from a rule; other fields are ignored.
- Rules are discovered recursively, so you can organize them into `.claude/rules/frontend/`, `.claude/rules/backend/`, etc.
- Globs support `**`, `*`, and brace expansion such as `src-dotnet/src/**/*.{ts,tsx}`.

### C.2 Watch a Rule Load

In a fresh session (`/clear`), run `/context` and note which memory files are listed. Then:

```
Read src-dotnet/src/TaskManager.Domain/Tasks/Task.cs and tell me which project instructions apply to this file and where each one came from.
```

Claude should now cite `src-dotnet/CLAUDE.md` **and** `.claude/rules/dotnet.md` — neither was in context until it touched a matching file. (If you're on the Spring Boot track, read `src-springboot/taskmanager-domain/src/main/java/com/example/taskmanager/domain/tasks/Task.java` instead and look for `springboot.md`.)

### C.3 Add and Trigger Your Own Rule

Create a rule that only applies to the **domain layer** of both stacks. Ask Claude Code to write it:

```
Create .claude/rules/domain-layer.md with paths frontmatter matching
src-dotnet/src/TaskManager.Domain/**/*.cs and src-springboot/taskmanager-domain/**/*.java.
Rules:
- Value objects expose a static parse method (FromName in C#, fromString in Java)
- Invalid input throws ArgumentException (C#) / IllegalArgumentException (Java)
  with a message that lists the valid values
- The domain layer never references logging, HTTP, or persistence types
Keep it under 15 lines.
```

Now trigger it. Start a fresh session (`/clear`) and ask:

```
Show me (don't write it yet) a small Effort value object (Small, Medium, Large)
for the domain layer of my stack.
```

Check the proposal against your rule: Is there a `FromName`/`fromString`? Does the error message list valid values? Then ask:

```
Which rule files influenced that answer?
```

Keep `domain-layer.md` — you'll use it again in Lab 3, Part 2 when you build a `Priority` value object.

> **Rules vs. subdirectory `CLAUDE.md`**: both are fine. Rules win when the scope is a file *pattern* that cuts across folders (e.g., "all test files", "all `*.java` in the domain module"), and they keep `.claude/` as the single place to look for team instructions.

---

## Part D: Writing a CLAUDE.md for a Subdirectory (8 min)

The `src-dotnet/tests/` directory currently has no `CLAUDE.md`. You'll write one.

### D.1 Ask Claude Code to Explore the Tests

```
Read the test projects in the src-dotnet/tests/ directory and describe the current test structure, what frameworks are used, and what testing patterns you observe.
```

Claude Code will read the test project files and give you a summary. (Expect xUnit v3 + FakeItEasy, and test methods that are currently `Skip` stubs — `dotnet test` reports them as skipped until you implement them in Labs 3 and 5.)

### D.2 Draft and Write `src-dotnet/tests/CLAUDE.md`

```
Based on what you found in the src-dotnet/tests/ directory, draft a CLAUDE.md for src-dotnet/tests/ that captures: the testing frameworks, the naming conventions, the project structure, and any important "don't do this" rules for tests in this codebase. Keep it short. If something is already covered by src-dotnet/CLAUDE.md (which loads alongside it) or .claude/rules/dotnet.md, reference it instead of repeating it.
```

Review the draft. It should include things like:
- xUnit v3 with FakeItEasy
- Test naming: `MethodName_StateUnderTest_ExpectedBehavior`
- Arrange/Act/Assert with blank line separators
- Not to mock things that don't need mocking

Then:

```
Write this as src-dotnet/tests/CLAUDE.md.
```

### D.3 Use an `@path` Import

`CLAUDE.md` files can pull in other files with `@path/to/file`. Relative paths resolve from the file containing the import, and imports can nest (up to four hops). `src-dotnet/CLAUDE.md` already loads whenever Claude works under `src-dotnet/`, so there's no need to import it. Instead, pull in the domain-layer rule you wrote in Part C, so tests for domain code follow it even when Claude is only reading test files. Ask Claude Code to add a line like this to `src-dotnet/tests/CLAUDE.md`:

```markdown
Domain-layer rules: @../../.claude/rules/domain-layer.md
```

> ⚠️ Imports organize a file; they **don't** save context — the imported file loads in full alongside the one that imports it. To mention a path *without* importing it, wrap it in backticks: `` `@README.md` ``.

### D.4 Verify

```
/clear
```

```
Read src-dotnet/tests/TaskManager.UnitTests/UnitTest1.cs, then write a test for TaskService.GetTaskAsync returning null when the task doesn't exist. Show it to me; don't write the file.
```

Reading a file under `src-dotnet/tests/` pulls `src-dotnet/tests/CLAUDE.md` (and its import) into context, so the test should follow those conventions without you needing to specify them.

---

## Part E: Personal Memory — User, Local, and Auto (5 min)

### E.1 User-Level `CLAUDE.md`

Some preferences apply everywhere, not just this project. Run `/memory` and open **User memory** (`~/.claude/CLAUDE.md`) — it's created if it doesn't exist. A minimal starting point:

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

### E.2 `CLAUDE.local.md` — Personal, Project-Specific

For preferences that only apply to *you* in *this* repo (your sandbox URL, preferred test data, "I'm on the Spring Boot track"), create `CLAUDE.local.md` at the repo root and keep it out of git:

```bash
echo "CLAUDE.local.md" >> .git/info/exclude
```

```markdown
# My local notes
- I'm doing the Spring Boot track. Default to src-springboot/ unless I say otherwise.
```

### E.3 Auto Memory

Auto memory is on by default. Claude decides on its own what's worth keeping (your corrections, preferences, project context it can't derive from code) and stores it in `~/.claude/projects/<project>/memory/` — a `MEMORY.md` index (first 200 lines / 25 KB load every session) plus topic files.

Try it:

```
Remember that I prefer FakeItEasy's A.CallTo(...).MustHaveHappenedOnceExactly() over looser assertions.
```

Look for a "Saved … memory" message, then run `/memory` → open the auto memory folder to see what was written. Auto memory is plain markdown — edit or delete entries any time, or toggle the feature off from `/memory`.

> **Rule of thumb**: "Remember X" → auto memory (just for you, on this machine). "Add X to CLAUDE.md" → shared with the team via git. Team conventions belong in `CLAUDE.md` or `.claude/rules/`, not in auto memory.

### E.4 Bootstrapping a New Repo with `/init`

You won't need it here (this repo already has memory files), but on your own projects `/init` analyzes the codebase and generates a starter `CLAUDE.md` with build commands, test instructions, and conventions. If a `CLAUDE.md` already exists, it suggests improvements instead of overwriting. Set `CLAUDE_CODE_NEW_INIT=1` for an interactive flow that also offers to set up skills, hooks, and `CLAUDE.local.md`.

---

## Reflection: Why Several Layers?

Claude Code's memory tunes an *agent's judgment* across everything it does — file reads, writes, commands, and reasoning — not just what gets suggested at the cursor. Each layer answers a different question:

- **`CLAUDE.md` (project)** — what should *everyone* know in *every* session?
- **Subdirectory `CLAUDE.md` / `.claude/rules/` with `paths:`** — what matters only for *this part* of the codebase?
- **`CLAUDE.local.md` / `~/.claude/CLAUDE.md`** — what's true only for *me*?
- **Auto memory** — what has Claude *learned* from working with me?

Remember: all of these are **context, not enforcement**. To guarantee something never happens (e.g., block `git push`), use permissions or hooks (Lab 9).

---

## Success Criteria

- [ ] You can explain the memory model (managed → user → project/rules → local → subdirectory, plus auto memory) and when each file loads
- [ ] You used `/context` and `/memory` to see which memory files are active
- [ ] You watched a path-scoped rule load and created `.claude/rules/domain-layer.md`
- [ ] You wrote a `src-dotnet/tests/CLAUDE.md` (with an `@path` import) that Claude Code uses to generate better tests
- [ ] You created (or planned) a `~/.claude/CLAUDE.md` and know when to use `CLAUDE.local.md`

---

## Extension Exercise

Add a "Domain Vocabulary" section to the root `CLAUDE.md` that defines the bounded context terms used in this project (Task, Priority, Due Date, Assignee, Notification, etc.) and ask Claude Code to propose a new `Assignee` value object. Observe how the vocabulary — and your `domain-layer.md` rule — shape the output.

---

## Troubleshooting

**Claude Code isn't following CLAUDE.md conventions**
- Run `/context` and check the **Memory files** list — is the file actually loaded?
- Subdirectory `CLAUDE.md` files and path-scoped rules load only after Claude reads or edits a matching file; ask it to read one first
- Verify the file is in the right place: `find . -name "CLAUDE.md"` and `ls .claude/rules/`
- Make instructions concrete and verifiable ("Use file-scoped namespaces", not "Write clean code")

**A path-scoped rule never loads**
- Check the frontmatter: it must be valid YAML between `---` markers, with a `paths:` list of quoted globs
- Globs are relative to the project root (the folder containing `.claude/`)
- If the YAML doesn't parse, the rule loads as if it had no `paths`; `claude --debug` shows the parse error

**CLAUDE.md changes not taking effect**
- Start a new session with `/clear` or restart `claude` — files are loaded at session start (or first touch, for subdirectory files and scoped rules)

**Conflicting instructions across memory files**
- Files are concatenated, not overridden — Claude may follow either instruction. Remove the conflict.
- Run `/doctor prompt-audit` (recent Claude Code versions) to have Claude look for outdated or contradictory instructions across your memory files

**Claude saved something to auto memory that should be in CLAUDE.md**
- Ask Claude: "Move that note from auto memory into CLAUDE.md", or edit both via `/memory`

---

**Next**: [Lab 3: TDD with Claude Code](lab-03-tdd-with-claude-code.md)
