---
marp: true
theme: default
paginate: true
backgroundColor: #fff
---

# Claude Code for Application Development

## AI-Assisted Development with an Agentic Pair Programmer

**Duration:** 6 Hours (Day 1 of 2)
**Format:** Instructor-led, hands-on
**Tool:** Claude Code CLI by Anthropic

---

# Welcome

## What You'll Learn Today

- Understand the **agentic model** — how Claude Code differs from completion tools
- Configure **CLAUDE.md** to encode your team's standards as project memory
- Apply **Test-Driven Development** with Claude Code as a development partner
- **Refactor legacy code** across multiple files safely
- Manage **context** through long sessions
- Integrate Claude Code into your **git workflow**

---

# The One Thing to Know Before We Start

## Claude Code is an agent, not an autocomplete

**Completion-based tools**:
> You select text → ask a question → receive a suggestion → accept or reject

**Claude Code:**
> You describe a task → Claude Code reads your codebase → plans a series of actions → executes them — files written, commands run, tests executed

The mental model shift: **you are directing a collaborator, not editing suggestions**

---

# What This Means in Practice

## With a completion tool
1. Open file
2. Position cursor
3. Prompt for suggestion
4. Accept/reject
5. Repeat for each change

## With Claude Code
1. Describe what you want
2. Claude Code reads relevant files
3. Proposes changes (you review)
4. Executes — writes files, runs tests
5. Reports results

---

# The Trust-and-Verify Workflow

## Core discipline for working with Claude Code

```
Describe task
     ↓
Claude Code plans and executes
     ↓
Review the diff  ← THIS IS MANDATORY
     ↓
Run the tests    ← THIS IS MANDATORY
     ↓
Commit if satisfied
```

Claude Code is fast. The review step is where your engineering judgment lives.
**Never skip the diff review.**

---

# Why CLAUDE.md Changes Everything

## Without CLAUDE.md

```
You: Create a TaskService in the Application layer
Claude Code: [creates something that may or may not
             follow your team's conventions]

You: No, we use file-scoped namespaces and sealed classes
Claude Code: [fixes it]

You: And guard clauses instead of else branches
Claude Code: [fixes it again]

You: And async all the way through...
```

---

# Why CLAUDE.md Changes Everything

## With CLAUDE.md

```
# src/CLAUDE.md
- File-scoped namespaces
- Sealed classes by default
- Guard clauses (no else)
- async/await throughout
```

```
You: Create a TaskService in the Application layer
Claude Code: [creates it correctly the first time]
```

**CLAUDE.md is how you stop teaching Claude Code the same lessons repeatedly.**

---

# The CLAUDE.md Hierarchy

```
~/.claude/CLAUDE.md          ← Your personal preferences
                                (applies to all your projects)

project-root/CLAUDE.md       ← Project overview, how to build
                                (applies to the whole project)

src/CLAUDE.md                ← Layer/stack-specific conventions
                                (applies to src/ and below)

tests/CLAUDE.md              ← Test-specific conventions
                                (loaded when Claude works in tests/)

.claude/rules/*.md           ← Path-scoped rules (`paths:` frontmatter)
                                (loaded only for matching files)
```

More specific instructions take precedence when there are conflicts.

---

# Technology Overview

## Claude Code CLI

```bash
claude              # Start interactive session
claude -p "…"       # Run non-interactively (print mode)
claude --continue   # Resume last session
claude doctor       # Diagnose install & settings
```

| Command | Purpose |
|---|---|
| `/help` · `/context` | List commands · see what fills the context window |
| `/model` · `/effort` | Pick model · set reasoning effort |
| `/compact` · `/clear` | Summarize history · fresh session |
| `/code-review` | Review changes (`/review` is an alias) |

**Shift+Tab** cycles permission modes: Auto → Manual → Accept edits → Plan

---

# Session Setup

## What participants need

- **Claude Code installed**: `curl -fsSL https://claude.ai/install.sh | bash` (Windows: `irm https://claude.ai/install.ps1 | iex`) — or use the devcontainer
- **Authenticated**: Claude subscription (`/login`) **or** Console API key (`ANTHROPIC_API_KEY`); `/status` shows which
- **Repository forked** on GitHub, then cloned from the fork
- **Upstream remote added**: `git remote add upstream https://github.com/centricconsulting/…`
- **Personal branch**: `git checkout -b workshop/your-name`

## Verify your stack

```bash
# .NET — expect 11 skipped, 0 failed
dotnet --version && dotnet build && dotnet test

# Spring Boot
java -version && cd src-springboot && mvn clean test
```

---

# Lab 1 Preview: First Contact

## What you'll do

- Install and authenticate Claude Code *(if not done)*
- Ask Claude Code to explore the TaskManager codebase
- Run the tests and trigger a permission prompt in Manual mode
- Learn the essential slash commands and Shift+Tab permission modes

## The key experiment

Ask Claude Code: *"What is this project?"*

Watch how it reads multiple files before answering — not guessing from a prompt, but actually reading your code.

---

# What Makes This Workshop Different

## Compared to Completion-Based Tools

| Completion-Based Tools | Claude Code Workshop |
|---|---|
| Tool-per-action | Task-per-outcome |
| Per-file-pattern instruction files | `CLAUDE.md` + `.claude/rules/` |
| Custom agents | Subagents in `.claude/agents/` |
| Prompt files | Skills in `.claude/skills/` |
| Bilingual split | Language-agnostic config |

---

# Let's Begin

## Lab 1: First Contact

👉 Open [docs/labs/lab-01-first-contact.md](../../../labs/lab-01-first-contact.md)

**Goal for the next 20 minutes:**
- Claude Code is installed and authenticated
- You've had your first conversation about the TaskManager project
- You've seen at least one permission prompt

---

<!-- Navigation -->
**Day 1 Module 00** | [Next: Features Tour →](01-claude-code-features-tour.md)
