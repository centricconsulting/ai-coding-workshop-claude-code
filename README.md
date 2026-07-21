# Workshop: Using AI for Application Development with Claude Code

## Overview

Build real software faster and better using **Claude Code** as an agentic AI collaborator. This workshop teaches developers how to work with Claude Code — Anthropic's CLI for Claude — across the full development lifecycle: project setup, TDD, refactoring, testing, documentation, and workflow automation.

Unlike completion-based tools, Claude Code is an agent that reads your codebase, runs commands, and takes multi-step actions. This workshop teaches you how to harness that power effectively — and safely.

**Choose Your Technology Stack:** This workshop supports both **🔷 .NET** and **🟩 Spring Boot** implementations.

**This workshop runs over two days:**

### Day 1: Fundamentals (6 hours)

Learn to work with Claude Code as a development partner — setting up project memory with CLAUDE.md, applying TDD, planning large tasks with Plan Mode, refactoring code with a checkpoint safety net, and integrating Claude Code into your git workflow with a dedicated security review gate. Works with either **.NET 9** or **Spring Boot 3.x**.

### Day 2: Advanced Claude Code (6 hours)

Master advanced Claude Code capabilities: the permissions and safety model, extending Claude Code with MCP servers and Skills, understanding sub-agents and parallel work, headless/automation mode for CI pipelines, output styles, and designing a complete team configuration.

**Full workshop duration:** 12 hours across two days

**What makes this workshop unique:**
- **Agentic-first mental model** — teaches the trust-and-verify workflow, not just "how to prompt"
- **CLAUDE.md mastery** — project/subdir/user memory hierarchy for encoding team standards
- **Hooks & automation** — `.claude/settings.json` for pre/post-tool workflow gates
- **MCP server integration** — extending Claude Code with GitHub, filesystem, and custom tools
- **Safety by design** — permissions model, reviewing diffs, scoping tasks appropriately
- **Bilingual support** — same concepts, your technology stack (.NET or Spring Boot)

---

## Prerequisites

### Universal Prerequisites

- **Claude Code**: Installed via `npm install -g @anthropic-ai/claude-code` and authenticated
- **Anthropic API Key**: From [console.anthropic.com](https://console.anthropic.com) — set as `ANTHROPIC_API_KEY` in your shell profile **before** opening any devcontainer
- **Visual Studio Code** (optional but recommended): Latest version, with Claude Code extension
- **Git**: Basic familiarity with git commands

> **Devcontainer users**: The containers forward `ANTHROPIC_API_KEY` from your host automatically. Set it on your host first — see `.devcontainer/README.md` for details.

### 🔷 .NET Track

- **.NET 9 SDK**: Verified with `dotnet --version`
- **C# Dev Kit** extension for VS Code

### 🟩 Spring Boot Track

- **Java 21 JDK**: Verified with `java -version`
- **Maven 3.9+**: For building Spring Boot projects
- **Spring Boot extension pack** for VS Code

### Environment Check

```bash
# Claude Code
claude --version

# .NET Track
dotnet --version    # Should show 9.x.x

# Spring Boot Track
java -version       # Should show Java 21
mvn --version
```

---

## Learning Objectives

### Day 1: Fundamentals

By the end of Day 1, participants will be able to:

- **Explain the agentic model** — how Claude Code differs from completion-based assistants
- **Configure CLAUDE.md** at project and subdirectory levels to encode team standards
- **Apply TDD with Claude Code** using the full Red-Green-Refactor cycle
- **Plan large tasks in Plan Mode** before any files change, and use extended thinking when it's worth the cost
- **Perform multi-file refactoring** safely, reviewing diffs before accepting changes
- **Recover from a bad edit** using checkpoints and rewind, without touching git
- **Manage context** with `/compact`, session continuity, and scoped tasks
- **Integrate with git** — conventional commits, PR descriptions, `/review`
- **Run a dedicated security review** with `/security-review` before code goes out as a PR

### Day 2: Advanced Claude Code

By the end of Day 2, participants will be able to:

- **Navigate the permissions model** — allow/deny tool use, understand safety boundaries
- **Automate workflows with hooks** — pre/post-tool hooks in `.claude/settings.json`
- **Configure MCP servers** — extend Claude Code with GitHub MCP, filesystem MCP, and custom tools
- **Package reusable procedures as Skills** — SKILL.md, progressive disclosure, when to use a skill vs. MCP vs. CLAUDE.md
- **Understand sub-agents** — when Claude Code uses parallel agents and how to scope complex tasks
- **Use headless/automation mode** — `--print`, `--continue`, CI pipeline integration
- **Customize how Claude Code communicates** with output styles
- **Design team CLAUDE.md configurations** — encode standards so Claude Code works well without re-prompting

---

## Schedule

### Day 1: Fundamentals (6 hours)

| Section | Duration |
|---|---|
| Day 1 Kickoff & Setup | 15 min |
| Claude Code Features Tour | 15 min |
| **Lab 1**: First Contact — CLI basics & the agent model | 20 min |
| **Lab 2**: CLAUDE.md & Project Memory | 30 min |
| **Lab 3**: TDD with Claude Code | 60 min |
| **Lab 4**: Plan Mode & Extended Thinking | 20 min |
| **Lab 5**: Multi-File Refactoring & Context Management | 60 min |
| **Lab 6**: Checkpoints & Rewind | 20 min |
| **Lab 7**: Git Workflow & PR Preparation | 15 min |
| **Lab 8**: Security Review Workflows | 30 min |
| Day 1 Wrap-Up & Discussion | 20 min |

### Day 2: Advanced Claude Code (6 hours)

| Section | Duration |
|---|---|
| Day 2 Kickoff & Recap | 15 min |
| **Lab 9**: Permissions, Safety & Hooks | 25 min |
| **Lab 10**: MCP Servers | 35 min |
| **Lab 11**: Skills | 30 min |
| **Lab 12**: Sub-Agents & Complex Workflows | 35 min |
| **Lab 13**: Headless & Automation Mode | 25 min |
| **Lab 14**: Output Styles & Custom Personas | 20 min |
| **Lab 15**: Capstone — Design a Team Claude Code Configuration | 65 min |
| Day 2 Wrap-Up & Next Steps | 15 min |

---

## Workshop Materials

### Lab Guides

#### Day 1: Fundamentals (Labs 1–8)

- **[Lab 1: First Contact](docs/labs/lab-01-first-contact.md)** (20 min) — Install, authenticate, first run, slash commands, permission prompts
- **[Lab 2: CLAUDE.md & Project Memory](docs/labs/lab-02-claude-md-memory.md)** (30 min) — Project/subdir/user hierarchy, encoding team standards
- **[Lab 3: TDD with Claude Code](docs/labs/lab-03-tdd-with-claude-code.md)** (60 min) — Full Red-Green-Refactor cycle, multi-file writes, diff review
- **[Lab 4: Plan Mode & Extended Thinking](docs/labs/lab-04-plan-mode-and-extended-thinking.md)** (20 min) — Read-only planning, `think`/`ultrathink` triggers
- **[Lab 5: Multi-File Refactoring](docs/labs/lab-05-refactoring-and-context.md)** (60 min) — Legacy code, context management, `/compact`
- **[Lab 6: Checkpoints & Rewind](docs/labs/lab-06-checkpoints-and-rewind.md)** (20 min) — Session-level undo, separate from git
- **[Lab 7: Git Workflow & PR Preparation](docs/labs/lab-07-git-workflow.md)** (15 min) — Conventional commits, `/review`, PR descriptions
- **[Lab 8: Security Review Workflows](docs/labs/lab-08-security-review-workflows.md)** (30 min) — `/security-review`, a project-specific security command

#### Day 2: Advanced Claude Code (Labs 9–15)

- **[Lab 9: Permissions, Safety & Hooks](docs/labs/lab-09-permissions-and-hooks.md)** (25 min) — Permissions model, `.claude/settings.json`, hooks
- **[Lab 10: MCP Servers](docs/labs/lab-10-mcp-servers.md)** (35 min) — Configure and use MCP servers with Claude Code
- **[Lab 11: Skills](docs/labs/lab-11-skills.md)** (30 min) — SKILL.md, progressive disclosure, building the repo's first skill
- **[Lab 12: Sub-Agents & Parallel Workflows](docs/labs/lab-12-sub-agents.md)** (35 min) — Complex multi-step tasks, agent scoping
- **[Lab 13: Headless & Automation Mode](docs/labs/lab-13-headless-automation.md)** (25 min) — CLI flags, piping, CI integration
- **[Lab 14: Output Styles & Custom Personas](docs/labs/lab-14-output-styles-and-personas.md)** (20 min) — Persistent tone/behavior customization
- **[Lab 15: Capstone — Team Configuration](docs/labs/lab-15-capstone-team-config.md)** (65 min) — End-to-end CLAUDE.md + hooks + MCP + skills design

### Presentation Decks

Presentations use [Marp](https://marp.app/) format. View in VS Code with the [Marp extension](https://marketplace.visualstudio.com/items?itemName=marp-team.marp-vscode).

- **[Day 1 Modules](docs/presentations/modules/part1/)** — 10 standalone modules
- **[Day 2 Modules](docs/presentations/modules/part2/)** — 7 standalone modules
- **[Presentations Catalog](docs/presentations/index.md)** — delivery patterns and timing

### Participant Guides

- **[Local Setup Guide](docs/LOCAL_SETUP.md)** — For participants who cannot use devcontainers
- **[Pre-Workshop Checklist](docs/PRE_WORKSHOP_CHECKLIST.md)** — Everything to complete before the session
- **[Agent Governance Guide](docs/guides/agent-governance.md)** — Managing Claude Code config at enterprise/team scale

### Reference Configuration

- **[`CLAUDE.md`](CLAUDE.md)** — Project-level memory for this workshop repo
- **[`src/CLAUDE.md`](src/CLAUDE.md)** — .NET-specific context for Claude Code
- **[`src-springboot/CLAUDE.md`](src-springboot/CLAUDE.md)** — Spring Boot-specific context
- **[`.claude/settings.json`](.claude/settings.json)** — Hooks and permissions example

---

## Starter Solution Structure

**🔷 .NET Solution** (`src/` directory):
- Clean Architecture: Domain / Application / Infrastructure / API layers
- `LegacyTaskProcessor` for refactoring exercises
- xUnit test stubs with FakeItEasy

**🟩 Spring Boot Solution** (`src-springboot/` directory):
- Clean Architecture: domain / application / infrastructure / web layers
- Mule ESB legacy code sample for modernization exercises
- JUnit 5 stubs with Mockito

**Claude Code Configuration**:
- **`CLAUDE.md`** (project root): Workshop repo memory
- **`src/CLAUDE.md`**: .NET context for Claude Code
- **`src-springboot/CLAUDE.md`**: Spring Boot context for Claude Code
- **`.claude/settings.json`**: Permissions and hooks example

---

## Getting Started

1. **Fork this repository** on GitHub (click **Fork** in the top-right corner).

   > Forking is required — not optional. Labs 10 (GitHub MCP), 13 (headless/CI), and 15 (the capstone) all work against your own remote. You'll push commits, trigger workflows, and query your fork's issues during the workshop.

2. **Clone your fork**:

   ```bash
   git clone https://github.com/YOUR-GITHUB-USERNAME/ai-code-workshop-claude-code.git
   cd ai-code-workshop-claude-code
   ```

3. **Add the upstream remote** (so you can pull workshop updates):

   ```bash
   git remote add upstream https://github.com/centricconsulting/ai-code-workshop-claude-code.git
   ```

4. **Create your working branch**:

   ```bash
   git checkout -b workshop/your-name
   ```

5. **Verify Claude Code**:

   ```bash
   claude --version
   claude   # Opens Claude Code — try asking "what is this project?"
   ```

6. **Verify your stack**:

   ```bash
   # .NET
   dotnet build && dotnet test

   # Spring Boot
   cd src-springboot && mvn clean test
   ```

7. **Ready!** Follow your facilitator or start with [Lab 1](docs/labs/lab-01-first-contact.md).

---

## Best Practices

- **Write a good CLAUDE.md first** — the more context Claude Code has, the less re-prompting you need
- **Scope tasks clearly** — Claude Code works best with specific, bounded requests
- **Review diffs before accepting** — treat every change as a code review
- **Use `/compact` proactively** — don't wait for context limits
- **Start with read-only tasks** — get familiar with how Claude Code explores your code before letting it write
- **Trust but verify** — Claude Code can be wrong; always run tests after changes

---

## Contributing

Found an issue or have suggestions? Create an issue with the lab number, section, and your environment details.

---

## License

Workshop content is proprietary to Centric Consulting. See LICENSE.md.
