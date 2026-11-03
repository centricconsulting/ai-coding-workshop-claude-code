# Lab 12: Custom Subagents & Agent Workflows

**Duration**: 35–40 minutes
**Day**: 2 of 2 (Advanced)

**Learning Objectives**:
- Understand what a subagent is and recognize Claude Code's built-in subagents (Explore, Plan, general-purpose)
- Invoke the project's custom subagents (`architecture-reviewer`, `backlog-generator`, `test-strategist`) by natural language and by @-mention
- Chain subagents into a feature workflow: backlog → architecture review → test strategy
- Apply agent design principles (role, scope, least-privilege tools, output format, handoff) to build your own subagent
- Run subagents in parallel and in the background, and know when *not* to use one

---

## Overview

A **subagent** is a specialized assistant with its own context window, system prompt, tool allowlist, and (optionally) model. Claude delegates a task to it, the subagent works independently, and only its **final result** comes back to your conversation. The file reads, searches, and dead ends stay out of your context.

Lab 11 packaged *procedures* as skills that run in your conversation. This lab packages *roles* as subagents that run beside it. This repo ships three role-based subagents in `.claude/agents/`, ported from the GitHub Copilot version of this workshop:

| Subagent | Role | Tools | Model |
|---|---|---|---|
| `backlog-generator` | Turns a feature request into INVEST user stories and saves them to `docs/backlog/` | Read, Grep, Glob, Write | sonnet |
| `architecture-reviewer` | Checks code or designs against Clean Architecture/DDD rules in both stacks | Read, Grep, Glob | inherit |
| `test-strategist` | Designs a prioritized, TDD-ready test plan for both stacks | Read, Grep, Glob | sonnet |

---

## Prerequisites

- Completed Labs 1–11
- Working on your personal branch (`workshop/your-name`)

---

## Part 0: Built-in Subagents (5 min)

Claude Code already delegates to built-in subagents without being asked:

| Built-in | Tools | Used for |
|---|---|---|
| **Explore** | Read-only | Searching and understanding a codebase. Claude picks a thoroughness level: *quick*, *medium*, or *very thorough* |
| **Plan** | Read-only | Gathering context while you're in plan mode (Shift+Tab) |
| **general-purpose** | All tools | Complex multi-step tasks that need both exploration and changes |

Try it:

```
Use the Explore subagent (very thorough) to map how a Task flows from the HTTP endpoint to the repository in both src-dotnet/ and src-springboot/. Return a short table per stack.
```

While it runs, notice that Claude keeps working (subagents spawned from the main conversation start **in the background** by default). Run `/tasks` to see running subagents, the model each uses, and open their transcripts.

> Explore and Plan skip CLAUDE.md to stay fast and small. Custom subagents **do** load CLAUDE.md and the project's rules, which is why the shipped agents can say "read `src-dotnet/CLAUDE.md`" and rely on it.

---

## Part A: Use the Shipped Subagents (10 min)

### A.1 Inspect Them

Open `.claude/agents/architecture-reviewer.md`. A subagent is a Markdown file with YAML frontmatter:

```markdown
---
name: architecture-reviewer
description: Read-only Clean Architecture and DDD reviewer ... Use proactively after changes that add a reference or import between layers ...
tools: Read, Grep, Glob
model: inherit
color: blue
---

# Architecture Reviewer
You are a senior software architect ...   ← becomes the subagent's system prompt
```

| Field | Purpose |
|---|---|
| `name`, `description` | Required. The description tells Claude **when** to delegate. "Use proactively when…" encourages automatic delegation |
| `tools` | Allowlist. If you omit it, the subagent inherits **all** tools, including MCP tools (use `disallowedTools` to deny specific ones) |
| `model` | `sonnet`, `opus`, `haiku`, a full model ID, or `inherit` (same model as your session) |
| Others (optional) | `permissionMode`, `maxTurns`, `skills` (preload skills), `mcpServers`, `hooks`, `memory`, `background`, `isolation: worktree`, `color` |

The body is the subagent's **entire** system prompt. It does not receive Claude Code's own system prompt or your conversation history; it receives only the task Claude writes for it.

> `/agents` no longer opens a wizard. It prints a reminder to ask Claude or edit `.claude/agents/` (project) and `~/.claude/agents/` (personal) directly. Files you add or edit there are picked up live.

### A.2 Invoke by Natural Language

```
Use the architecture-reviewer subagent to review src-dotnet/src/TaskManager.Domain and src-springboot/taskmanager-domain for layer violations and primitive obsession.
```

Read the report. It should follow the agent's output format (Summary → ✅ Strengths → ⚠️ Concerns → 🚫 Violations → Recommendations → Suggested Handoffs) and cite real files.

> `architecture-reviewer` has no Bash, so it can't run `git diff`. To review your recent changes, say "Review the files changed in `git diff --name-only main`". The main conversation runs git and passes the list along.

### A.3 Invoke by @-mention

An @-mention **guarantees** that a specific subagent runs, rather than leaving the choice to Claude. Type `@`, then pick the agent from the typeahead (it appears as `test-strategist (agent)`):

```
@"test-strategist (agent)" plan tests for the Task status transitions in both stacks
```

Or type it directly: `@agent-test-strategist plan tests for the Task status transitions in both stacks`.

Compare the plan to the conventions in `src-dotnet/CLAUDE.md` and `src-springboot/CLAUDE.md`. The test names should follow `MethodName_StateUnderTest_ExpectedBehavior` / `methodName_stateUnderTest_expectedBehavior`, and the plan should treat the skipped stub tests as gaps.

### A.4 Automatic Delegation

Now ask without naming any agent:

```
I'm about to add a dependency from TaskService in the Application layer to a new EmailSender class in Infrastructure. Is that OK?
```

The `architecture-reviewer` description says "use proactively after changes that add a reference or import between layers", so Claude may delegate on its own.

> **Expect a surprise:** if you built `clean-architecture-boundary-check` in Lab 11, Claude will often run that **skill** instead. In a validation run it did exactly that. Both descriptions claim the same trigger, so Claude picks one. That's the overlap problem from Troubleshooting below. Compare the two descriptions, decide which should own this question (see C.3), and use `@agent-architecture-reviewer` when you want the subagent for sure.

---

## Part B: Chain a Feature Workflow (12 min)

In the Copilot version of this workshop, agents pass work to each other with "handoff" buttons. In Claude Code, **the main conversation is the orchestrator**: each subagent returns its result to Claude, and Claude passes the relevant context to the next one. Files such as `docs/backlog/*.md` act as durable handoff artifacts.

```
backlog-generator ──► architecture-reviewer ──► test-strategist ──► (you implement, TDD)
   stories +             layer placement,          prioritized test
   acceptance criteria   ADR-worthy decisions      plan per stack
```

### B.1 Run the Chain

```
We need a new feature: Team Leads can assign a task to a team member, and the assignee gets notified when a task is assigned to them or its due date is within 24 hours.

1. Use the backlog-generator subagent to turn this into user stories.
2. Then use the architecture-reviewer subagent to review the stories' Technical Notes against our Clean Architecture layers in both stacks (where do Assignee, the notification port, and the endpoint belong?).
3. Then use the test-strategist subagent to produce a test plan for the first story in the suggested order, mapping every acceptance criterion to a test.

Wait for each subagent to finish before starting the next, and give me a one-paragraph summary at the end.
```

The chain takes several minutes; in a validation run it took about 7 minutes and roughly $1.50 of usage. Keep working or watch `/tasks` while it runs.

### B.2 Inspect the Handoffs

- Open `docs/backlog/<feature-slug>.md`. This is the artifact `backlog-generator` wrote. It's the only one of the three agents that has the `Write` tool.
- Run `/tasks` (or scroll back) and look at each subagent's transcript. Notice how little of the intermediate searching appeared in your main conversation.
- Check whether the architecture review changed anything the test plan relied on (for example, `Assignee` as a value object vs. a raw `Guid`).

### B.3 Discuss

```
What did the main conversation pass from one subagent to the next? What would have been lost if this had all happened in one long conversation instead? What would have been lost if the subagents could talk to each other directly?
```

> **Optional:** implement the first story with TDD (`/test-data-generator` from Lab 11 can produce the fixtures the test plan asks for), then close the loop: `Use the architecture-reviewer subagent to review the files changed in git diff --name-only`.

---

## Part C: Design Your Own Subagent (10 min)

### C.1 Agent Design Principles

All three shipped agents follow the same checklist (see the [Agent Design Guide](../guides/agent-design-guide.md) for the full version):

| Principle | Question to answer | Example from this repo |
|---|---|---|
| **Role** | Who is this agent? | "Senior software architect specializing in Clean Architecture…" |
| **Scope** | What is it responsible for, and what isn't it? | `test-strategist` *plans* tests; it does not write them |
| **Trigger** | When should Claude delegate to it? | `description:` with "Use proactively when…" |
| **Tools (least privilege)** | What is the minimum it needs? | Reviewers get Read/Grep/Glob only; only `backlog-generator` can Write |
| **Model** | Does it need the strongest model? | `sonnet` for structured generation; `inherit` for nuanced review |
| **Output format** | What exact shape should the result have? | Fixed headings so the next agent (or you) can parse it |
| **Handoff** | What happens next? | "Suggested Handoffs" section; backlog saved to a file |

### C.2 Build One

Pick one small, focused role, for example:

- `api-contract-checker`: compares the .NET and Spring Boot endpoints/DTOs for the same resource and reports drift
- `commit-message-writer`: drafts a Conventional Commit message from a supplied diff (no tools needed)
- `legacy-smell-finder`: lists code smells in `LegacyTaskProcessor` in both stacks, ranked by refactoring payoff

Ask Claude to create it:

```
Create a project subagent at .claude/agents/api-contract-checker.md. Requirements:
- description that says exactly when to use it, including "Use proactively when an endpoint or DTO changes in either stack"
- tools: Read, Grep, Glob only
- model: haiku
- body: role, scope (what it does NOT do), step-by-step process, and a fixed output format with a table of endpoint | .NET | Spring Boot | drift
Show me the frontmatter before writing the body.
```

Then test it twice: once with an explicit @-mention, and once with a prompt that should trigger it automatically. If you created `.claude/agents/` yourself during this session and it didn't exist when the session started, restart Claude Code so the new directory is watched.

### C.3 Subagent, Skill, or Main Conversation?

| Use… | When |
|---|---|
| **Main conversation** | Frequent back-and-forth, or phases that share a lot of context; quick targeted changes |
| **Skill** (Lab 11) | A reusable procedure that should run *in* your conversation's context |
| **Subagent** | Verbose or self-contained work whose details you don't need back; enforced tool restrictions; a distinct role or model |
| **Skill with `context: fork`** | A one-off task you want isolated, written as a skill and run by a built-in agent type |

See the [Customization Decision Guide](../guides/customization-decision-guide.md) for the full decision tree.

---

## Part D: Parallel and Background Subagents (5 min)

Independent investigations can run at the same time:

```
Research the Domain, Application, and API layers of src-springboot/ in parallel using separate Explore subagents. Each should list the public types in its layer and any dependency that points outward. Combine the results into one table.
```

- Subagents start in the background, so you can keep typing. Press **Ctrl+B** to send a running foreground task to the background.
- `/tasks` lists running and recently finished subagents; select one to open its transcript, or press `x` to stop it.
- Each subagent's result lands in your main context. Many detailed results can still fill it up, so ask for concise summaries.
- Subagents work inside one session. For independent long-running sessions, see the [background agents](https://code.claude.com/docs/en/agent-view) and [agent teams](https://code.claude.com/docs/en/agent-teams) docs.

---

## Success Criteria

- [ ] You ran the built-in Explore subagent and found it in `/tasks`
- [ ] You invoked `architecture-reviewer` by natural language and `test-strategist` by @-mention
- [ ] You ran the backlog-generator → architecture-reviewer → test-strategist chain, and `docs/backlog/<feature>.md` exists
- [ ] You created one custom subagent in `.claude/agents/` with a focused description, least-privilege `tools`, a `model`, and a fixed output format
- [ ] You can explain when to use a subagent vs. a skill vs. the main conversation

---

## Extension Challenge

1. **Preload a skill:** add `skills: [test-data-generator]` to `test-strategist` so its plan includes ready-made fixtures. Compare the output before and after.
2. **Run a whole session as an agent:** `claude --agent architecture-reviewer` makes the reviewer your main thread. Notice that it can't edit anything.
3. **Persistent memory:** add `memory: project` to `architecture-reviewer` and ask it to record recurring violations it finds. Review what it saved after two runs.
4. **Isolation:** create a refactoring subagent with `isolation: worktree` and point it at `LegacyTaskProcessor`, so its edits happen in a separate git worktree.

---

## Troubleshooting

**Claude doesn't delegate to my subagent automatically**

- Make the `description` more specific about *when* to use it, and include "use proactively"
- Make sure no other agent's description covers the same ground
- Use an @-mention to force it while you tune the description

**The subagent doesn't appear in the @ typeahead**

- Check that the file is in `.claude/agents/` with `---` on the first line and both `name` and `description` set
- If `.claude/agents/` didn't exist when the session started, restart Claude Code

**The subagent can't do something it needs to**

- It only has the tools in its `tools:` list. Add the minimum missing tool, or have the main conversation do that step (for example, run `git diff`) and pass the result in

**The chain loses information between steps**

- Ask each agent to save or return its full output (as `backlog-generator` does), and tell Claude explicitly what to pass to the next agent

**My main context still fills up**

- Ask subagents for summaries, not full dumps, and avoid running many detailed subagents at once

---

**Next**: [Lab 13: Headless & Automation Mode](lab-13-headless-automation.md)
