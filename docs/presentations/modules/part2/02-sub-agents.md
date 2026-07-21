---
marp: true
theme: default
paginate: true
backgroundColor: #fff
---

# Module 08: Sub-Agents & Parallel Workflows

## Structuring Tasks to Unlock Parallelism

---

# Where We Are

You've extended Claude Code with permissions, hooks, and MCP.

**Lab 8 is about how Claude Code itself works internally** —
and how your prompt structure determines whether it works fast or slow.

---

# What Happens Inside a Complex Task

When you give Claude Code a large task, it doesn't always execute it linearly.
It can spawn **sub-agents**: separate agent instances that run in parallel.

```
Main Agent
    ├── Sub-Agent A: analyze .NET project
    ├── Sub-Agent B: analyze Spring Boot project
    └── (waits for both, then synthesizes)
```

Each sub-agent has its own context window and tool access.
The main agent synthesizes their results.

---

# The Agent Tool

Sub-agents are invoked via an internal `Agent` tool.

Each sub-agent:
- Has its own context window (doesn't see parent conversation history)
- Can read files, run commands, and write files
- Reports results back to the parent
- Runs in isolation — context is passed explicitly in the task description

This is why sub-agent quality depends on **how well you scope the task**.

---

# When Claude Code Uses Sub-Agents

Sub-agents appear when:
- The task has clearly **independent** parts
- A subtask needs significant context that would pollute the main conversation
- Parallel execution would be meaningfully faster than sequential

Claude Code decides. But your prompt structure **signals** whether parallelism is possible.

---

# Serial vs. Parallel Task Structure

**Serial (slow) — hidden dependencies implied:**
```
Read all the domain entities, then read all the repositories,
then read all the handlers, then tell me if the repository
pattern is consistently applied.
```

**Parallel (fast) — independence explicit:**
```
Analyze whether the repository pattern is consistently applied.
Check the Domain, Application, and Infrastructure layers in parallel.
```

The second framing lets Claude Code read all three simultaneously.

---

# What Enables Parallelism

Two things signal to Claude Code that sub-tasks can run in parallel:

1. **Explicit "simultaneously" / "in parallel" / "at the same time"** language
2. **Independent workstreams** — tasks that don't depend on each other's output

```
Analyze three things simultaneously:
1. The domain layer for DDD pattern compliance
2. The application layer for CQRS pattern compliance
3. The test projects for AAA structure compliance

Give me a scorecard for each area.
```

Watch the tool output — you'll see three parallel read threads.

---

# The Context Window Problem

A task like this will eventually fail:

```
Implement the complete notification system end-to-end:
domain, application, infrastructure, API, and tests.
```

Signs of context exhaustion:
- Claude Code repeats earlier work
- Quality degrades mid-task
- Context indicator shows >80% used

The fix isn't a bigger context window — it's **phased prompting**.

---

# The Multi-Phase Pattern

**Phase 1:**
```
Implement Phase 1 only: the domain model for notifications.
Notification entity, NotificationType value object,
INotificationRepository interface. Write and run the tests. Stop.
```

**Verify:**
```
Run the tests and give me a Phase 1 summary: what was created,
what tests pass, what's needed next.
```

**Phase 2 (fresh session):**
```
Phase 1 summary: [paste]. Now implement Phase 2: the application layer —
CreateNotificationCommand, handler, and tests.
Domain interfaces from Phase 1 are in place.
```

---

# Right-Sizing a Session

**Good scope for one session:**
- One class or interface + its tests
- One refactoring target (`LegacyTaskProcessor`)
- One API endpoint end-to-end (domain → application → API + tests)
- A code review pass on a specific file or directory

**Too large for one session:**
- An entire feature from requirements to deployment
- "Clean up the whole codebase"
- Multiple unrelated features at once

> Instead of "build the notification system," ask: "build the notification domain model."

---

# Sub-Agents vs. Phases: When to Use Which

| Pattern | Use When |
|---|---|
| **Parallel sub-agents** | Independent analysis or generation tasks |
| **Multi-phase sessions** | Sequential work where phase 2 builds on phase 1 |
| **Single session** | Scoped, focused tasks under ~1 hour of work |

The patterns aren't mutually exclusive — a phase 1 session might internally use sub-agents for parallel file analysis.

---

# Lab 8 Preview

## What You'll Do

1. Run a task designed to trigger parallel sub-agents — watch it happen
2. Structure a task explicitly for parallelism and observe the difference
3. Run a parallel generation task (.NET + Spring Boot + tests simultaneously)
4. Ask Claude Code to plan a 4-phase feature implementation
5. Practice the multi-phase pattern with a fresh session start

## The Key Insight

You don't control *whether* Claude Code uses sub-agents.
You control *whether your task structure allows it.*

---

# Lab 8: Sub-Agents & Parallel Workflows

👉 Open [docs/labs/lab-08-sub-agents.md](../../../labs/lab-08-sub-agents.md)

**Goal for the next 30 minutes:**
- Observe parallel sub-agent execution in at least one task
- Structure at least one task explicitly for parallelism
- Produce a multi-phase plan for a larger feature

**Watch the tool call output** — that's where you see the parallelism.
It won't show up in the text response.

---

<!-- Navigation -->
[← 01 MCP Servers](01-mcp-servers.md) | **Part 2 Module 02** | [Next: 03 Headless & Automation →](03-headless-automation.md)
