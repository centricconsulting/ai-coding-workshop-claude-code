# Lab 8: Sub-Agents & Parallel Workflows

**Duration**: 30–35 minutes
**Part**: 2 of 2 (Advanced)

**Learning Objectives**:
- Understand when Claude Code uses sub-agents internally
- Structure tasks to take advantage of parallel agent work
- Recognize when a task is too large for one agent turn
- Apply the "scope and delegate" pattern for complex multi-step workflows

---

## Overview

Claude Code can spawn sub-agents to handle complex tasks — running multiple independent pieces of work in parallel rather than sequentially. This is most powerful for research, analysis, and generation tasks that don't depend on each other.

Understanding how sub-agents work helps you structure prompts to unlock this parallelism and recognize when you're accidentally serializing work that could run in parallel.

---

## Prerequisites

- Completed Labs 1–7

---

## Part A: What Are Sub-Agents? (8 min)

### A.1 The Agent Tool

When Claude Code encounters a complex task, it can use an internal `Agent` tool to spawn a sub-agent with a specific prompt. The sub-agent:
- Has its own context window
- Can read files, run commands, and write files
- Reports results back to the parent agent
- Runs in isolation — it doesn't see the parent's conversation history

### A.2 When Sub-Agents Appear

Claude Code uses sub-agents when:
- The task has clearly independent parts (analyze file A AND analyze file B)
- A subtask needs significant context that shouldn't pollute the main conversation
- Parallel execution would be meaningfully faster than sequential

### A.3 Seeing Sub-Agents in Action

Run a task designed to trigger parallel sub-agents:

```bash
claude
```

```
Analyze the test coverage situation across both stacks simultaneously. For the .NET project (src/ and tests/), identify which classes have test coverage and which don't. At the same time, do the same for the Spring Boot project (src-springboot/). Give me a comparison table at the end.
```

Watch the output — you should see two parallel analysis threads running. Claude Code will read .NET and Spring Boot files simultaneously rather than sequentially.

---

## Part B: Structuring Tasks for Parallelism (10 min)

The key to triggering parallel sub-agents is framing your task with *independent* workstreams.

### B.1 Serial vs. Parallel Task Structure

**Serial (slow):**
```
Read all the domain entities, then read all the repositories, then read all the handlers, then tell me if the repository pattern is consistently applied.
```

**Parallel (fast):**
```
Analyze whether the repository pattern is consistently applied across all layers. Check the Domain, Application, and Infrastructure layers in parallel.
```

The parallel version lets Claude Code read all three layers simultaneously rather than waiting for each to complete.

### B.2 Exercise: Parallel Code Quality Analysis

Structure a task that will naturally parallelize:

```
Run a parallel code quality analysis across this codebase. Simultaneously check:
1. The domain layer for DDD pattern compliance (entities, value objects, aggregates)
2. The application layer for CQRS pattern compliance (commands, queries, handlers)
3. The test projects for test naming and AAA structure compliance

Give me a scorecard for each area.
```

### B.3 Exercise: Parallel Generation

Use parallelism for generation tasks:

```
Generate three things simultaneously:
1. A new Priority value object for the .NET domain layer
2. A matching Priority class for the Spring Boot domain layer
3. Tests for both Priority implementations

Write all three files. The .NET and Spring Boot implementations should be semantically equivalent.
```

Observe how Claude Code handles the three-way parallel generation. It may write sequentially anyway if it determines the files have dependencies — watch the tool call output to see its actual execution pattern.

---

## Part C: Scoping Large Tasks (10 min)

Sub-agents protect the main context window by offloading large subtasks. You can use this explicitly by breaking large tasks into scoped phases.

### C.1 The Problem: Context Exhaustion

A task like "implement the complete notification system end-to-end including domain, application, infrastructure, API, and tests" will eventually exceed a single context window. Signs:
- Claude Code starts repeating earlier work
- Quality degrades mid-task
- `/status` shows >80% context used

### C.2 The Solution: Explicit Phase Breakdown

Rather than one massive prompt, structure it as phases with checkpoints:

**Phase 1 prompt:**
```
Implement Phase 1 only: Create the domain model for the notification system. This includes: the Notification entity, NotificationType value object, and INotificationRepository interface. Write the files and run the tests. Stop after Phase 1.
```

**Verify:**
```
Run the tests and confirm Phase 1 is complete and green. Give me a Phase 1 summary: what was created, what tests pass, and what's needed next.
```

**Phase 2 prompt (fresh session):**
```
We completed Phase 1 of the notification system: [paste Phase 1 summary]. Now implement Phase 2: the application layer — CreateNotificationCommand, CreateNotificationCommandHandler, and the unit tests. The domain interfaces from Phase 1 are already in place.
```

This pattern keeps each session focused and avoids context exhaustion.

### C.3 Exercise: Plan a Multi-Phase Implementation

Ask Claude Code to plan a phased implementation you could execute:

```
Plan a 4-phase implementation of a task assignment feature (assigning tasks to team members). Each phase should be completable in a single Claude Code session. For each phase, describe: what gets created, what tests prove it's done, and what the next phase depends on.
```

Review the plan. This is the kind of output you'd produce *before* starting implementation on a large feature.

---

## Part D: Task Scope Guidelines (5 min)

**Good scope for one Claude Code session:**
- One class or interface + its tests
- One refactoring target (like LegacyTaskProcessor)
- One API endpoint end-to-end (domain → application → API + tests)
- A code review pass on a specific file or directory

**Too large for one session:**
- An entire feature from requirements to deployment
- "Clean up the whole codebase"
- Multiple unrelated features at once

**The right framing:**
> Instead of "build the notification system," ask: "build the notification domain model" — one phase, clear scope, testable outcome.

---

## Success Criteria

- [ ] You observed parallel sub-agent execution in at least one task
- [ ] You structured at least one task to take advantage of parallelism
- [ ] You understand the multi-phase pattern for large implementations
- [ ] You can articulate what scope is appropriate for a single session

---

## Extension Exercise

Design a "parallel architecture review" task:

```
Simultaneously review three architecture concerns:
1. Does the Domain layer have any dependencies on Application or Infrastructure? (It shouldn't.)
2. Does the Application layer have any direct dependencies on the API layer? (It shouldn't.)
3. Are there any circular dependencies between projects?

For each concern, cite the specific files or imports that violate the rule (if any).
```

This is a good example of a task where parallel sub-agents are genuinely faster than sequential analysis.

---

## Troubleshooting

**Claude Code runs everything sequentially even when tasks seem independent**

Sequential execution is sometimes correct — Claude Code may have determined that the tasks actually have dependencies. Ask:

```
Could you have done those analyses in parallel? What dependency caused you to run them sequentially?
```

**Sub-agent results seem lower quality than the main agent**

Sub-agents have their own context window and don't have the full conversation history. If a sub-agent needs specific context, include it explicitly in the task scope description.

**Phase 2 doesn't know about Phase 1**

This is expected — sub-agents and new sessions start fresh. Always pass the Phase 1 summary explicitly in the Phase 2 prompt.

---

**Next**: [Lab 9: Headless & Automation Mode](lab-09-headless-automation.md)
