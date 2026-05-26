# Workshop Lab Walkthroughs

Step-by-step guides for all 10 Claude Code workshop labs.

Each lab includes clear learning objectives, step-by-step instructions with prompts, expected outputs, troubleshooting, and extension exercises.

---

## Part 1: Fundamentals

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
**Duration**: 45 min — bilingual (🔷 .NET and 🟩 Spring Boot)

Apply the full Red-Green-Refactor TDD cycle. Review multi-file diffs before accepting. Use tests as a feedback loop — Claude Code runs them and fixes failures automatically.

**Key Concept**: Directing a workflow vs. accepting individual completions.

---

### [Lab 4: Multi-File Refactoring & Context Management](lab-04-refactoring-and-context.md)
**Duration**: 45 min — bilingual (🔷 .NET and 🟩 Spring Boot)

Write characterization tests before refactoring. Refactor LegacyTaskProcessor in phases. Use `/compact` before hitting context limits.

**Key Concept**: Large tasks need phases and checkpoints; `/compact` is a proactive tool, not an emergency measure.

---

### [Lab 5: Git Workflow & PR Preparation](lab-05-git-workflow.md)
**Duration**: 15 min

Use `/review` for pre-commit code review. Generate conventional commit messages grounded in actual diffs. Produce a complete PR description from session context.

**Key Concept**: `/review` is a quality gate, not a post-hoc check.

---

## Part 2: Advanced Claude Code

### [Lab 6: Permissions, Safety & Hooks](lab-06-permissions-and-hooks.md)
**Duration**: 25 min

Understand the three-level permissions hierarchy. Modify `.claude/settings.json`. Write pre- and post-tool hooks for workflow automation.

**Key Concept**: The permissions model is a safety boundary — the deny list is as important as the allow list.

---

### [Lab 7: MCP Servers](lab-07-mcp-servers.md)
**Duration**: 30 min

Understand MCP (Model Context Protocol) and why it exists. Configure the filesystem and GitHub MCP servers. Use MCP-backed tools in real workflows.

**Key Concept**: MCP adds new tool capabilities; hooks add side effects on existing tools — they're complementary, not competing.

---

### [Lab 8: Sub-Agents & Parallel Workflows](lab-08-sub-agents.md)
**Duration**: 30 min

Understand when Claude Code uses sub-agents. Structure tasks for parallel execution. Apply the multi-phase pattern for large implementations.

**Key Concept**: Small, focused, well-scoped tasks unlock Claude Code's best performance.

---

### [Lab 9: Headless & Automation Mode](lab-09-headless-automation.md)
**Duration**: 25 min

Run Claude Code non-interactively with `--print`. Use `--continue` and `--resume`. Integrate Claude Code into CI pipelines. Understand the safety implications of headless mode.

**Key Concept**: Headless mode removes the human-in-the-loop — restrictive permissions are mandatory.

---

### [Lab 10: Capstone — Design a Team Configuration](lab-10-capstone-team-config.md)
**Duration**: 45 min

Design a complete team Claude Code configuration: CLAUDE.md hierarchy, permissions & hooks, MCP integration, governance model. Present and defend your design.

**Key Concept**: A well-designed team configuration multiplies Claude Code's value across every developer.

---

## Learning Path

### Part 1: Fundamentals

```
Lab 1 (Agent Model)
    ↓
Lab 2 (CLAUDE.md)
    ↓
Lab 3 (TDD)
    ↓
Lab 4 (Refactoring + Context)
    ↓
Lab 5 (Git Workflow)
```

### Part 2: Advanced

```
Lab 6 (Permissions & Hooks)
    ↓
Lab 7 (MCP)
    ↓
Lab 8 (Sub-Agents)
    ↓
Lab 9 (Headless)
    ↓
Lab 10 (Capstone)
    ↓
Apply to Real Projects!
```

---

## Time Estimates

| Lab | Minimum | Comfortable | With Extensions |
|---|---|---|---|
| **Part 1** | | | |
| Lab 1 | 15 min | 20 min | 30 min |
| Lab 2 | 25 min | 30 min | 45 min |
| Lab 3 | 35 min | 45 min | 60 min |
| Lab 4 | 35 min | 45 min | 60 min |
| Lab 5 | 12 min | 15 min | 25 min |
| **Part 1 Total** | **122 min** | **155 min** | **220 min** |
| **Part 2** | | | |
| Lab 6 | 20 min | 25 min | 35 min |
| Lab 7 | 25 min | 30 min | 45 min |
| Lab 8 | 25 min | 30 min | 45 min |
| Lab 9 | 20 min | 25 min | 35 min |
| Lab 10 | 40 min | 50 min | 65 min |
| **Part 2 Total** | **130 min** | **160 min** | **225 min** |
| **Full Workshop** | **252 min** | **315 min** | **445 min** |

---

## Common Slash Commands Reference

| Command | When to Use |
|---|---|
| `/help` | List all slash commands |
| `/status` | Check context usage and session info |
| `/cost` | See session cost so far |
| `/compact` | Compress conversation history (use at >60% context) |
| `/clear` | Start a fresh session |
| `/review` | Run a structured code review on current changes |
| `/exit` | Exit Claude Code |

---

## Getting Started

1. Clone the repo and create your branch: `git checkout -b workshop/your-name`
2. Verify Claude Code: `claude --version`
3. Verify your stack: `dotnet build` or `cd src-springboot && mvn clean test`
4. Start with [Lab 1](lab-01-first-contact.md)
