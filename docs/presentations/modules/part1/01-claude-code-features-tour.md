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
  1. Read(src-dotnet/src/TaskManager.Domain/Tasks/Task.cs)  ← sees the model
  2. Read(src-dotnet/CLAUDE.md)                          ← loads conventions
  3. Edit(Task.cs, ...)                           ← makes the change
  4. Bash(dotnet test)                            ← verifies it works
  5. Reports results to you
```

Claude Code is not predicting text — it's **executing a plan**.

---

# The Core Tools

<style scoped>
section { font-size: 22px; }
</style>

| Tool | What It Does |
|---|---|
| `Read` | Read a file (respects context window) |
| `Glob` / `Grep` | Find files and search their contents |
| `Write` | Create a new file |
| `Edit` | Make targeted edits to an existing file |
| `Bash` | Run shell commands |
| `WebSearch` / `WebFetch` | Look things up |
| `Agent` | Delegate to a subagent (built-in: Explore, Plan, general-purpose) |
| `Skill` | Load a skill's instructions when relevant |

Every action is **visible** — and, depending on your permission mode, **approvable** before it runs.

---

# Permission Prompts: Your Safety Net

```
┌─────────────────────────────────────────────┐
│  Bash command                               │
│    dotnet test                              │
│  Do you want to proceed?                    │
│  ❯ 1. Yes                                   │
│    2. Yes, and don't ask again for:         │
│       dotnet test *                         │
│    3. No                                    │
│  Esc to cancel · Tab to amend               │
└─────────────────────────────────────────────┘
```

- **Yes** — this execution only
- **Yes, and don't ask again** — saves an allow rule (Bash rules go to `.claude/settings.local.json`)
- **No** — blocked; tell Claude what to do instead (`Tab` adds a note)

---

# What Gets Prompted vs. What Runs Freely

<style scoped>
section { font-size: 24px; }
</style>

It depends on the **permission mode** — cycle with `Shift+Tab`:

| Mode | Runs without asking |
|---|---|
| Manual (`manual`) | Reads only |
| `acceptEdits` | Reads + file edits |
| `plan` | Reads only — no edits until you approve a plan |
| `auto` | Everything, with background safety checks (the starting mode in current versions) |

Plus your **rules** in `.claude/settings.json`: `allow` (e.g. `Bash(dotnet *)`), `ask` (e.g. `Bash(git push *)`), and `deny` — deny wins in every mode.

**Not seeing prompts?** You're probably in `auto`. For Day 1, try `claude --permission-mode manual` to watch every action. *(Day 2, Lab 9 goes deep.)*

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

<style scoped>
section { font-size: 24px; }
</style>

| Command | When to Use |
|---|---|
| `/help` | List all available commands (type `/` to browse) |
| `/init` · `/memory` | Generate / edit CLAUDE.md project memory |
| `/model` · `/effort` | Pick the model; set reasoning depth |
| `/plan` | Enter plan mode (or `Shift+Tab`) |
| `/context` | See what's filling the context window |
| `/compact [focus]` · `/clear` | Summarize history · start a fresh conversation |
| `/rewind` | Undo Claude's edits / conversation (`Esc Esc`) |
| `/code-review` | Review your diff for bugs (`/review` is an alias) |
| `/security-review` | Security pass on your branch |
| `/usage` | Session cost & plan limits (`/cost` is an alias) |

---

# Configuration Commands (Day 2)

<style scoped>
section { font-size: 27px; }
</style>

| Command | What it manages |
|---|---|
| `/permissions` | Allow / ask / deny rules |
| `/hooks` | View configured hooks |
| `/mcp` | MCP server connections & auth |
| `/skills` | Project, user, and plugin skills |
| `/agents` | Points you to `.claude/agents/` — subagents are just Markdown files |
| `/plugin` | Install plugins from marketplaces |
| `/resume` | Return to an earlier conversation (`/continue` alias) |

**`/compact` is proactive, not emergency:** check `/context` and compact at a phase boundary, before auto-compact decides for you.

---

# Models and Effort

<style scoped>
section { font-size: 27px; }
</style>

```
/model            # picker: opus, sonnet, haiku, opusplan, ...
/effort high      # low | medium | high | xhigh | max
claude --model sonnet --effort medium
```

| Alias | Use for |
|---|---|
| `opus` | Hardest reasoning, big refactors, design |
| `sonnet` | Everyday coding |
| `haiku` | Fast, simple tasks |
| `opusplan` | Opus while planning → Sonnet while executing |

Need one extra-careful turn? Put **`ultrathink`** in the prompt.
(Plain "think hard" is just text now — use `/effort`.)
Available levels and defaults vary by model — check `/model`.

---

# Interactive vs. Headless Mode

**Interactive** (what you've been using):
```bash
claude
> Describe what you want...
```

**Headless** (`-p` / `--print`):
```bash
claude -p "List all TODO comments in src-dotnet/"
claude -p "..." --output-format json --max-turns 5
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

`/context` shows the real breakdown. Near the limit, Claude Code
**auto-compacts** (summarizes older history). Run `/compact <what to keep>`
yourself first so the summary keeps what *you* care about.

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

<style scoped>
section { font-size: 27px; }
</style>

This workshop has two parallel stacks:

| | .NET | Spring Boot |
|---|---|---|
| Build | `dotnet build` | `mvn clean test` |
| Test | `dotnet test` (from `src-dotnet/`) | `mvn test` (from `src-springboot/`) |
| Entry | `src-dotnet/` | `src-springboot/` |
| Config | `src-dotnet/CLAUDE.md` | `src-springboot/CLAUDE.md` |

Claude Code always loads the root `CLAUDE.md`, and pulls in `src-dotnet/CLAUDE.md`
or `src-springboot/CLAUDE.md` (plus matching `.claude/rules/`) when it works in that tree.

You can work in either stack for all labs. The concepts are identical.

---

# Where Claude Code Runs

| Surface | Notes |
|---|---|
| **Terminal CLI** | What this workshop uses — full feature set |
| **VS Code extension** | Recommended way to use it in VS Code (also Cursor): inline diffs, plan review, @-mentions |
| **JetBrains plugin** | IntelliJ, Rider, etc.: diff viewing, selection context |
| **Desktop app** | macOS / Windows: parallel sessions, visual diffs, cloud sessions |
| **Web** (claude.ai/code) | Cloud sessions on repos you don't have locally; `/teleport` pulls one into your terminal |

Same engine, same `CLAUDE.md`, skills, and settings across local surfaces.

---

# What's Coming in Day 1

| Module | Key Concept |
|---|---|
| **02 CLAUDE.md** | Project memory — teach Claude Code once |
| **03 TDD** | Tests as a feedback loop |
| **04 Plan Mode** | Plan large tasks before executing them; tune effort |
| **05 Refactoring** | Large tasks need phases — and `/context` / `/compact` |
| **06 Checkpoints & Rewind** | Recover from a bad edit without touching git |
| **07 Git Workflow** | `/code-review` as a quality gate |

Hands-on time with all of these after Lab 1. Security Review opens Day 2.

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
