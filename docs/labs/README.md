# Workshop Lab Walkthroughs

Step-by-step guides for all 15 Claude Code workshop labs.

Each lab includes clear learning objectives, step-by-step instructions with prompts, expected outputs, troubleshooting, and extension exercises.

---

## Day 1: Fundamentals

### [Lab 1: First Contact](lab-01-first-contact.md)
**Duration**: 20 min

Install and authenticate Claude Code. Understand the agentic model. Navigate the terminal UI: slash commands, permission prompts, session management.

**Key Concept**: The agentic model — Claude Code reads your codebase and executes actions, rather than suggesting completions.

---

### [Lab 2: CLAUDE.md & Project Memory](lab-02-claude-md-memory.md)
**Duration**: 30 min

Understand the CLAUDE.md hierarchy (project / subdirectory / user). Write CLAUDE.md files that measurably improve Claude Code's output. Understand what belongs in CLAUDE.md vs. what doesn't.

**Key Concept**: CLAUDE.md is project memory — write it once, Claude Code uses it every session.

---

### [Lab 3: TDD with Claude Code](lab-03-tdd-with-claude-code.md)
**Duration**: 60 min — bilingual (🔷 .NET and 🟩 Spring Boot)

Apply the full Red-Green-Refactor TDD cycle. Review multi-file diffs before accepting. Use tests as a feedback loop — Claude Code runs them and fixes failures automatically.

**Key Concept**: Directing a workflow vs. accepting individual completions.

---

### [Lab 4: Plan Mode & Extended Thinking](lab-04-plan-mode-and-extended-thinking.md)
**Duration**: 20 min

Enter Plan Mode and draft a phased refactor plan with zero file edits. Iterate risk-free before approving. Use extended-thinking triggers (`think`, `think hard`, `ultrathink`) and know when the added latency is worth it.

**Key Concept**: Plan Mode is a hard boundary — while active, Claude Code can propose but not touch files. The plan from this lab is what Lab 5 executes.

---

### [Lab 5: Multi-File Refactoring & Context Management](lab-05-refactoring-and-context.md)
**Duration**: 60 min — bilingual (🔷 .NET and 🟩 Spring Boot)

Write characterization tests before refactoring. Refactor LegacyTaskProcessor in phases, starting from the plan drafted in Lab 4. Use `/compact` before hitting context limits.

**Key Concept**: Large tasks need phases and checkpoints; `/compact` is a proactive tool, not an emergency measure.

---

### [Lab 6: Checkpoints & Rewind](lab-06-checkpoints-and-rewind.md)
**Duration**: 20 min

Deliberately damage the Lab 5 refactor with an over-broad follow-up edit, then recover using rewind — a session-level safety net separate from git.

**Key Concept**: Checkpoints/rewind solve session-local recovery; git solves durable, cross-session recovery. Know which tool fits which situation.

---

### [Lab 7: Git Workflow & PR Preparation](lab-07-git-workflow.md)
**Duration**: 15 min

Use `/review` for pre-commit code review. Generate conventional commit messages grounded in actual diffs. Produce a complete PR description from session context.

**Key Concept**: `/review` is a quality gate, not a post-hoc check.

---

### [Lab 8: Security Review Workflows](lab-08-security-review-workflows.md)
**Duration**: 30 min

Introduce a realistic authorization vulnerability, find it with `/security-review`, fix it at the root cause, and build the repo's first project-specific security slash command.

**Key Concept**: `/security-review` is a dedicated, deeper gate than `/review` — and this is the last lab of Day 1.

---

## Day 2: Advanced Claude Code

### [Lab 9: Permissions, Safety & Hooks](lab-09-permissions-and-hooks.md)
**Duration**: 25 min

Understand the three-level permissions hierarchy. Modify `.claude/settings.json`. Write pre- and post-tool hooks for workflow automation.

**Key Concept**: The permissions model is a safety boundary — the deny list is as important as the allow list.

---

### [Lab 10: MCP Servers](lab-10-mcp-servers.md)
**Duration**: 35 min

Understand MCP (Model Context Protocol) and why it exists. Configure the filesystem and GitHub MCP servers. Use MCP-backed tools in real workflows.

**Key Concept**: MCP adds new tool capabilities; hooks add side effects on existing tools — they're complementary, not competing.

---

### [Lab 11: Skills](lab-11-skills.md)
**Duration**: 30 min

Learn the SKILL.md format and progressive disclosure. Build the repo's first skill — a Clean Architecture boundary checker — and confirm it activates on matching prompts without false-triggering.

**Key Concept**: A Skill is a packaged, on-demand procedure — not a new tool (MCP) and not always-on context (CLAUDE.md).

---

### [Lab 12: Sub-Agents & Parallel Workflows](lab-12-sub-agents.md)
**Duration**: 35 min

Understand when Claude Code uses sub-agents. Structure tasks for parallel execution. Apply the multi-phase pattern for large implementations.

**Key Concept**: Small, focused, well-scoped tasks unlock Claude Code's best performance.

---

### [Lab 13: Headless & Automation Mode](lab-13-headless-automation.md)
**Duration**: 25 min

Run Claude Code non-interactively with `--print`. Use `--continue` and `--resume`. Integrate Claude Code into CI pipelines. Understand the safety implications of headless mode.

**Key Concept**: Headless mode removes the human-in-the-loop — restrictive permissions are mandatory.

---

### [Lab 14: Output Styles & Custom Personas](lab-14-output-styles-and-personas.md)
**Duration**: 20 min

Create and activate a custom output style, then compare its behavior against the default side by side. Discuss standardizing a style at the team level vs. leaving it personal.

**Key Concept**: An output style is a persistent, system-prompt-level change to *how* Claude Code communicates — distinct from every other mechanism, which changes what it knows or can do.

---

### [Lab 15: Capstone — Design a Team Configuration](lab-15-capstone-team-config.md)
**Duration**: 65 min

Design a complete team Claude Code configuration: CLAUDE.md hierarchy, permissions & hooks, MCP integration, skills, output styles, and governance model. Present and defend your design.

**Key Concept**: A well-designed team configuration multiplies Claude Code's value across every developer.

---

## Learning Path

### Day 1: Fundamentals

```
Lab 1 (Agent Model)
    ↓
Lab 2 (CLAUDE.md)
    ↓
Lab 3 (TDD)
    ↓
Lab 4 (Plan Mode & Extended Thinking)
    ↓
Lab 5 (Refactoring + Context)
    ↓
Lab 6 (Checkpoints & Rewind)
    ↓
Lab 7 (Git Workflow)
    ↓
Lab 8 (Security Review)
```

### Day 2: Advanced

```
Lab 9 (Permissions & Hooks)
    ↓
Lab 10 (MCP)
    ↓
Lab 11 (Skills)
    ↓
Lab 12 (Sub-Agents)
    ↓
Lab 13 (Headless)
    ↓
Lab 14 (Output Styles)
    ↓
Lab 15 (Capstone)
    ↓
Apply to Real Projects!
```

---

## Time Estimates

| Lab | Minimum | Comfortable | With Extensions |
|---|---|---|---|
| **Day 1** | | | |
| Lab 1 | 15 min | 20 min | 30 min |
| Lab 2 | 25 min | 30 min | 45 min |
| Lab 3 | 45 min | 60 min | 75 min |
| Lab 4 | 15 min | 20 min | 30 min |
| Lab 5 | 45 min | 60 min | 75 min |
| Lab 6 | 15 min | 20 min | 30 min |
| Lab 7 | 12 min | 15 min | 25 min |
| Lab 8 | 25 min | 30 min | 40 min |
| **Day 1 Total** | **197 min** | **255 min** | **350 min** |
| **Day 2** | | | |
| Lab 9 | 20 min | 25 min | 35 min |
| Lab 10 | 28 min | 35 min | 50 min |
| Lab 11 | 25 min | 30 min | 40 min |
| Lab 12 | 28 min | 35 min | 50 min |
| Lab 13 | 20 min | 25 min | 35 min |
| Lab 14 | 15 min | 20 min | 30 min |
| Lab 15 | 50 min | 65 min | 80 min |
| **Day 2 Total** | **186 min** | **235 min** | **320 min** |
| **Full Workshop** | **383 min** | **490 min** | **670 min** |

(Lab-only totals — kickoff/tour/wrap-up sections add roughly another 80 minutes across both days; see the root [README schedule](../../README.md#schedule) for the full day-by-day agenda.)

---

## Common Slash Commands Reference

| Command | When to Use |
|---|---|
| `/help` | List all slash commands |
| `/compact` | Compress conversation history when sessions grow long |
| `/clear` | Start a fresh session |
| `/review` | Run a structured code review on current changes |
| `/security-review` | Run a security-focused review (Lab 8) |
| `/exit` | Exit Claude Code |

---

## Getting Started

1. Clone the repo and create your branch: `git checkout -b workshop/your-name`
2. Verify Claude Code: `claude --version`
3. Verify your stack: `dotnet build` or `cd src-springboot && mvn clean test`
4. Start with [Lab 1](lab-01-first-contact.md)
