---
marp: true
theme: default
paginate: true
backgroundColor: #fff
---

# Claude Code for Application Development

## AI-Assisted Development with an Agentic Pair Programmer

**Duration:** 3 Hours (Part 1 of 2)
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

**Completion-based tools** (GitHub Copilot, etc.):
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
                                (applies to tests/ and below)
```

More specific files win when there are conflicts.

---

# Technology Overview

## Claude Code CLI

```bash
claude              # Start interactive session
claude --print "…"  # Run non-interactively
claude --continue   # Resume last session
```

**Essential slash commands:**

| Command | Purpose |
|---|---|
| `/help` | List all commands |
| `/status` | Context usage, session info |
| `/compact` | Compress history (use at 60%+ context) |
| `/review` | Pre-commit code review |
| `/clear` | Fresh session |

---

# Session Setup

## What participants need

- **Claude Code installed**: `npm install -g @anthropic-ai/claude-code`
- **Authenticated**: `claude` → follow login flow
- **Repository cloned**: `git clone …`
- **Personal branch**: `git checkout -b workshop/your-name`

## Verify your stack

```bash
# .NET
dotnet --version && dotnet build

# Spring Boot
java -version && cd src-springboot && mvn clean test
```

---

# Lab 1 Preview: First Contact

## What you'll do

- Install and authenticate Claude Code *(if not done)*
- Ask Claude Code to explore the TaskManager codebase
- Experience permission prompts firsthand
- Learn the essential slash commands

## The key experiment

Ask Claude Code: *"What is this project?"*

Watch how it reads multiple files before answering — not guessing from a prompt, but actually reading your code.

---

# What Makes This Workshop Different

## From the GitHub Copilot workshop

> If you took the Copilot workshop: you'll build the same NotificationService and refactor the same LegacyTaskProcessor — but the *how* is entirely different.

| Copilot Workshop | Claude Code Workshop |
|---|---|
| Tool-per-action | Task-per-outcome |
| `.github/instructions/` | `CLAUDE.md` |
| Custom agents | Hooks + MCP |
| Bilingual split | Language-agnostic config |

Watch for **"Compare to Copilot"** callouts throughout the labs.

---

# Let's Begin

## Lab 1: First Contact

👉 Open [docs/labs/lab-01-first-contact.md](../../labs/lab-01-first-contact.md)

**Goal for the next 20 minutes:**
- Claude Code is installed and authenticated
- You've had your first conversation about the TaskManager project
- You've seen at least one permission prompt

---

<!-- Navigation -->
**Part 1 Module 00** | [Next: Features Tour →](01-claude-code-features-tour.md)
