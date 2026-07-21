---
marp: true
theme: default
paginate: true
backgroundColor: #fff
---

# Module 01: Claude Code Features Tour

## What Claude Code Actually Does Under the Hood

---

# The Tool-Use Loop

Every Claude Code response is built from tool calls:

```
You: "Add a method to validate email addresses"

Claude Code:
  1. Read(TaskManager.Domain/Entities/Task.cs)   ← sees the model
  2. Read(src/CLAUDE.md)                          ← loads conventions
  3. Edit(Task.cs, ...)                           ← makes the change
  4. Bash(dotnet test)                            ← verifies it works
  5. Reports results to you
```

Claude Code is not predicting text — it's **executing a plan**.

---

# The Core Tools

| Tool | What It Does |
|---|---|
| `Read` | Read a file (respects context window) |
| `Write` | Create a new file |
| `Edit` | Make targeted edits to an existing file |
| `Bash` | Run shell commands |
| `WebSearch` / `WebFetch` | Look things up |

Every action is **visible and approvable** before it runs.

---

# Permission Prompts: Your Safety Net

```
┌─────────────────────────────────────────────┐
│  Claude Code wants to run:                  │
│                                             │
│  Bash: git push origin main                 │
│                                             │
│  [Allow once]  [Allow always]  [Deny]       │
└─────────────────────────────────────────────┘
```

Three responses:
- **Allow once** — this execution only
- **Allow always** — add to `allow` list in settings
- **Deny** — blocked; Claude Code must work around it

---

# What Gets Prompted vs. What Runs Freely

**Runs without prompting** (once allowed):
- Reading files in the project
- Running `dotnet build`, `dotnet test`, `mvn clean test`
- Editing files you're working on

**Always prompts** (unless explicitly allowed):
- `git push`
- Destructive commands (`rm -rf`, `git reset --hard`)
- Network calls outside the project
- Anything in the `deny` list

---

# The Session Model

Claude Code sessions maintain context across multiple turns:

```
Turn 1: "What is this project?"
  → Claude Code reads 6 files, builds a mental model

Turn 2: "Add a NotificationService"
  → Uses the model from Turn 1, doesn't re-read everything

Turn 3: "Write tests for it"
  → Knows about the NotificationService it just created
```

**Context is cumulative** — and finite. You'll learn to manage it.

---

# Essential Slash Commands

| Command | When to Use |
|---|---|
| `/help` | List all available commands |
| `/compact` | Compress history when context fills up |
| `/review` | Run a structured code review |
| `/clear` | Start a completely fresh session |

**`/compact` is proactive, not emergency:** use it at ~60% context, not when Claude Code starts forgetting things.

---

# Interactive vs. Headless Mode

**Interactive** (what you've been using):
```bash
claude
> Describe what you want...
```

**Headless** (`--print` flag):
```bash
claude --print "List all TODO comments in src/"
```
No session opens. Claude Code reads, processes, prints to stdout, exits.

**Useful for:** scripts, CI pipelines, pre-commit hooks, nightly reports.
*(Covered in Day 2, Lab 13)*

---

# Context Window: What Claude Code Knows

```
┌──────────────────────────────────────┐
│  Session Context Window              │
│                                      │
│  CLAUDE.md files (auto-loaded)       │
│  Files read this session             │
│  Commands run + their output         │
│  Your conversation history           │
│                                      │
│  [████████████░░░░░░░░]  60% full   │
└──────────────────────────────────────┘
```

When context fills, Claude Code starts dropping early turns.
`/compact` summarizes history to free space before that happens.

---

# File Targeting: How Claude Code Knows What to Read

Claude Code doesn't read your entire codebase upfront.

It reads files **on demand** based on:
1. What you mention explicitly ("look at `Task.cs`")
2. What it infers from your task ("add a command handler" → reads existing handlers)
3. CLAUDE.md entries pointing it to key files
4. Import/dependency tracing

**Tip:** If Claude Code is producing code that doesn't match your conventions, it probably hasn't read the right file yet. Name the file explicitly.

---

# What Claude Code Is Not

| Myth | Reality |
|---|---|
| "It reads my whole codebase" | Reads files on demand, guided by context |
| "It never makes mistakes" | It can hallucinate APIs that don't exist |
| "I should just accept its output" | Review every diff — you own the code |
| "It replaces engineers" | It accelerates engineers who review its output |

**The skill being developed in this workshop:**
Directing Claude Code precisely enough that its output is right the first time.

---

# The Bilingual Setup

This workshop has two parallel stacks:

| | .NET | Spring Boot |
|---|---|---|
| Build | `dotnet build` | `mvn clean test` |
| Test | `dotnet test` | `mvn test` |
| Entry | `src/` | `src-springboot/` |
| Config | `src/CLAUDE.md` | `src-springboot/CLAUDE.md` |

Claude Code reads **whichever `CLAUDE.md` is closest** to the file it's working on.

You can work in either stack for all labs. The concepts are identical.

---

# What's Coming in Day 1

| Module | Key Concept |
|---|---|
| **02 CLAUDE.md** | Project memory — teach Claude Code once |
| **03 TDD** | Tests as a feedback loop |
| **04 Plan Mode** | Plan large tasks before executing them |
| **05 Refactoring** | Large tasks need phases and checkpoints |
| **06 Checkpoints & Rewind** | Recover from a bad edit without touching git |
| **07 Git Workflow** | `/review` as a quality gate |
| **08 Security Review** | A deeper gate before every PR |

After Lab 1, you'll have hands-on time with all of these.

---

# Lab 1 is Underway

## If you haven't finished Lab 1 yet

**Minimum to have done:**
- Claude Code installed and authenticated
- At least one conversation about the TaskManager project
- Seen at least one permission prompt

**The key question to ask** (if you haven't):
*"What is the architecture of this project?"*

Watch Claude Code read files before it answers.

---

<!-- Navigation -->
[← 00 Kickoff](00-kickoff-and-setup.md) | **Day 1 Module 01** | [Next: CLAUDE.md & Memory →](02-claude-md-memory.md)
