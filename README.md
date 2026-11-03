# Workshop: Using AI for Application Development with Claude Code

## Overview

Build real software faster and better using **Claude Code** as an agentic AI collaborator. This workshop teaches developers how to work with Claude Code — Anthropic's CLI for Claude — across the full development lifecycle: project setup, TDD, refactoring, testing, documentation, and workflow automation.

Unlike completion-based tools, Claude Code is an agent that reads your codebase, runs commands, and takes multi-step actions. This workshop teaches you how to harness that power effectively — and safely.

**Choose Your Technology Stack:** This workshop supports both **🔷 .NET** and **🟩 Spring Boot** implementations.

**This workshop runs over two days:**

### Day 1: Fundamentals (6 hours)

Learn to work with Claude Code as a development partner — setting up project memory with CLAUDE.md, turning requirements into code with TDD, choosing the right permission mode and planning large tasks in Plan Mode, refactoring code with a checkpoint safety net, and integrating Claude Code into your git workflow. Works with either **.NET 9** or **Spring Boot 3.x**.

### Day 2: Advanced Claude Code (6 hours)

Master advanced Claude Code capabilities: a dedicated security review gate, the permissions and safety model, extending Claude Code with MCP servers and Skills, building and chaining custom subagents, headless/automation mode for CI pipelines, and a capstone where you build your own subagent or skill and package it as a plugin. Output styles (Lab 14) are self-study.

**Full workshop duration:** about 12 hours of instruction across two days (plus breaks)

**What makes this workshop unique:**
- **Agentic-first mental model** — teaches the trust-and-verify workflow, not just "how to prompt"
- **CLAUDE.md mastery** — project/subdir/user memory hierarchy plus path-scoped `.claude/rules/` for encoding team standards
- **Ready-to-use customizations** — shipped subagents, a skill, and rules in [`.claude/`](#whats-in-claude) to study, run, and extend
- **Hooks & automation** — `.claude/settings.json` for pre/post-tool workflow gates
- **MCP server integration** — extending Claude Code with GitHub, filesystem, and custom tools
- **Safety by design** — permissions model, reviewing diffs, scoping tasks appropriately
- **Bilingual support** — same concepts, your technology stack (.NET or Spring Boot)

---

## Prerequisites

### Universal Prerequisites

- **Claude Code**: Installed with the native installer (recommended — auto-updates, no Node.js needed):
  - macOS / Linux / WSL: `curl -fsSL https://claude.ai/install.sh | bash`
  - Windows PowerShell: `irm https://claude.ai/install.ps1 | iex`
  - Alternatives: `brew install --cask claude-code`, `winget install Anthropic.ClaudeCode`, or `npm install -g @anthropic-ai/claude-code` (Node.js 22+)
- **A Claude account**, one of:
  - A **Claude subscription** (Pro, Max, Team, or Enterprise) — sign in with `/login` on first run
  - A **Claude Console API key** from [platform.claude.com](https://platform.claude.com), set as `ANTHROPIC_API_KEY`
  - Amazon Bedrock, Google Vertex AI, or Microsoft Foundry access (see the [authentication docs](https://code.claude.com/docs/en/authentication))
- **Visual Studio Code** (optional but recommended): 1.94+, with the Claude Code extension (`anthropic.claude-code`)
- **Git**: Basic familiarity with git commands, and a GitHub account to fork this repo

> **Devcontainer users**: The containers install Claude Code for you. Sign in inside the container with `/login`, or set `CLAUDE_CODE_OAUTH_TOKEN` (from `claude setup-token`) or `ANTHROPIC_API_KEY` on your host before opening it — both are forwarded automatically. See [`.devcontainer/README.md`](.devcontainer/README.md).

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
claude doctor       # Install & settings diagnostics

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
- **Turn requirements into code** — from a user story to a tested `Priority`/`DueDate` feature and `POST /tasks` endpoint
- **Apply TDD with Claude Code** using the full Red-Green-Refactor cycle
- **Choose the right permission mode** (Manual, Accept edits, Plan, Auto), plan large tasks in Plan Mode before any files change, and dial reasoning with `/effort` or `ultrathink` when it's worth the cost
- **Perform multi-file refactoring** safely, reviewing diffs before accepting changes
- **Recover from a bad edit** using checkpoints and rewind, without touching git
- **Manage context** with `/compact`, session continuity, and scoped tasks
- **Integrate with git** — conventional commits, PR descriptions, `/code-review`

### Day 2: Advanced Claude Code

By the end of Day 2, participants will be able to:

- **Run a dedicated security review** with `/security-review` before code goes out as a PR
- **Navigate the permissions model** — allow/deny tool use, understand safety boundaries
- **Automate workflows with hooks** — pre/post-tool hooks in `.claude/settings.json`
- **Configure MCP servers** — extend Claude Code with GitHub MCP, filesystem MCP, and custom tools
- **Package reusable procedures as Skills** — SKILL.md, progressive disclosure, when to use a skill vs. MCP vs. CLAUDE.md
- **Build and chain custom subagents** — use the shipped `backlog-generator`, `architecture-reviewer`, and `test-strategist` agents, and design your own with least-privilege tools
- **Use headless/automation mode** — `-p`, `--output-format`, `--continue`, CI pipeline integration
- **Customize how Claude Code communicates** with output styles *(self-study, Lab 14)*
- **Design and share a team configuration** — encode standards in CLAUDE.md and rules, then package a subagent or skill as a plugin

---

## Schedule

Each numbered module below is a short slide intro *and* its matching hands-on lab, delivered back-to-back as one time block — the duration shown is the combined total (condensed intro plus the upper end of the lab's stated duration). See the [Presentations Catalog](docs/presentations/index.md) for full slide-only durations and the [Facilitator Guide](FACILITATOR_GUIDE.md) for an agenda with breaks and fast-track options.

### Day 1: Fundamentals (~6 hours plus breaks)

| Module | Duration |
|---|---|
| Kickoff & Setup | 15 min |
| Claude Code Features Tour | 10 min |
| Lab 1: First Contact — CLI basics & the agent model *(no dedicated module; follows the intro above)* | 25 min |
| **Module 2**: CLAUDE.md & Project Memory *(+ Lab 2)* | 55 min |
| **Module 3**: TDD & Requirements → Code *(+ Lab 3, two parts)* | 105 min |
| **Module 4**: Plan Mode, Permission Modes & Thinking *(+ Lab 4)* | 35 min |
| **Module 5**: Multi-File Refactoring & Context Management *(+ Lab 5)* | 60 min |
| **Module 6**: Checkpoints & Rewind *(+ Lab 6)* | 25 min |
| **Module 7**: Git Workflow & PR Preparation *(+ Lab 7)* | 30 min |
| Day 1 Wrap-Up & Discussion | 15 min |
| **Day 1 total** | **375 min** |

### Day 2: Advanced Claude Code (~6 hours plus breaks)

| Module | Duration |
|---|---|
| Day 2 Kickoff & Recap | 10 min |
| **Module 8**: Security Review Workflows *(+ Lab 8)* | 45 min |
| **Module 9**: Permissions, Safety & Hooks *(+ Lab 9)* | 40 min |
| **Module 10**: MCP Servers *(+ Lab 10)* | 45 min |
| **Module 11**: Skills *(+ Lab 11)* | 45 min |
| **Module 12**: Custom Subagents & Agent Workflows *(+ Lab 12)* | 50 min |
| **Module 13**: Headless & Automation Mode *(+ Lab 13)* | 40 min |
| **Module 15**: Capstone — Team Configuration & Plugins *(+ Lab 15)* | 75 min |
| Day 2 Wrap-Up & Next Steps | 15 min |
| **Day 2 total** | **365 min** |

**Self-study:** Module 14, Output Styles & Custom Personas *(+ Lab 14, 20–25 min)*. It's the least dependent lab, so it's done after the workshop. Facilitators with spare time can run it between Modules 13 and 15.

Module numbers here match each module's lab number exactly (Module 9 pairs with Lab 9, Module 15 with Lab 15, and so on) — the only exception is Lab 1, which has no dedicated module of its own and follows directly after the Day 1 intro.

---

## Workshop Materials

### Lab Guides

#### Day 1: Fundamentals (Labs 1–7)

- **[Lab 1: First Contact](docs/labs/lab-01-first-contact.md)** (20–25 min) — Install, authenticate, first run, slash commands, permission modes and prompts
- **[Lab 2: CLAUDE.md & Project Memory](docs/labs/lab-02-claude-md-memory.md)** (40–45 min) — Project/subdir/user hierarchy, `/init`, imports, path-scoped `.claude/rules/`
- **[Lab 3: TDD & Requirements → Code](docs/labs/lab-03-tdd-with-claude-code.md)** (90–100 min, two parts) — Red-Green-Refactor, then a user story to `Priority`/`DueDate` and `POST /tasks`
- **[Lab 4: Plan Mode, Permission Modes & Thinking](docs/labs/lab-04-plan-mode-and-extended-thinking.md)** (25–30 min) — Manual / Accept edits / Plan / Auto, read-only planning, `/effort` and `ultrathink`
- **[Lab 5: Multi-File Refactoring](docs/labs/lab-05-refactoring-and-context.md)** (45–50 min) — Legacy code, context management, `/context`, `/compact`
- **[Lab 6: Checkpoints & Rewind](docs/labs/lab-06-checkpoints-and-rewind.md)** (20 min) — Session-level undo with `/rewind`, separate from git
- **[Lab 7: Git Workflow & PR Preparation](docs/labs/lab-07-git-workflow.md)** (20–25 min) — Conventional commits, `/code-review`, PR descriptions

#### Day 2: Advanced Claude Code (Labs 8–15)

- **[Lab 8: Security Review Workflows](docs/labs/lab-08-security-review-workflows.md)** (30–35 min) — `/security-review`, a project-specific security skill
- **[Lab 9: Permissions, Safety & Hooks](docs/labs/lab-09-permissions-and-hooks.md)** (25–30 min) — Permission rules and modes, `.claude/settings.json`, hooks
- **[Lab 10: MCP Servers](docs/labs/lab-10-mcp-servers.md)** (30–35 min) — Configure and use MCP servers (including GitHub's) with Claude Code
- **[Lab 11: Skills](docs/labs/lab-11-skills.md)** (30–35 min) — SKILL.md, progressive disclosure, the shipped `test-data-generator` skill, building your own
- **[Lab 12: Custom Subagents & Agent Workflows](docs/labs/lab-12-sub-agents.md)** (35–40 min) — Built-in subagents, the shipped `.claude/agents/`, chaining backlog → architecture review → test strategy
- **[Lab 13: Headless & Automation Mode](docs/labs/lab-13-headless-automation.md)** (25–30 min) — `-p`, output formats, piping, CI integration with GitHub Actions
- **[Lab 14: Output Styles & Custom Personas](docs/labs/lab-14-output-styles-and-personas.md)** (20–25 min, self-study) — Persistent tone/behavior customization
- **[Lab 15: Capstone — Team Configuration](docs/labs/lab-15-capstone-team-config.md)** (60–65 min) — Design a team config, build your own subagent or skill, and package it as a plugin

### Presentation Decks

Presentations use [Marp](https://marp.app/) format. View in VS Code with the [Marp extension](https://marketplace.visualstudio.com/items?itemName=marp-team.marp-vscode).

- **[Day 1 Modules](docs/presentations/modules/part1/)** — 9 standalone modules
- **[Day 2 Modules](docs/presentations/modules/part2/)** — 9 standalone modules
- **[Presentations Catalog](docs/presentations/index.md)** — delivery patterns and timing

### Participant Guides

- **[Local Setup Guide](docs/LOCAL_SETUP.md)** — For participants who cannot use devcontainers
- **[Pre-Workshop Checklist](docs/PRE_WORKSHOP_CHECKLIST.md)** — Everything to complete before the session
- **[Customization Decision Guide](docs/guides/customization-decision-guide.md)** — CLAUDE.md vs. rules vs. skills vs. subagents vs. hooks vs. MCP vs. plugins: when to use which
- **[Agent Design Guide](docs/guides/agent-design-guide.md)** — How to design, test, and chain effective subagents
- **[Agent Governance Guide](docs/guides/agent-governance.md)** — Managing Claude Code config at enterprise/team scale

### Facilitator Materials

- **[Facilitator Guide](FACILITATOR_GUIDE.md)** — Pre-workshop setup, agenda and timings, per-lab talking points and pitfalls, troubleshooting

### Reference Configuration

- **[`CLAUDE.md`](CLAUDE.md)** — Project-level memory for this workshop repo
- **[`src-dotnet/CLAUDE.md`](src-dotnet/CLAUDE.md)** — .NET-specific context for Claude Code
- **[`src-springboot/CLAUDE.md`](src-springboot/CLAUDE.md)** — Spring Boot-specific context
- **[`.claude/`](.claude/)** — Shipped subagents, skill, rules, and settings (see [What's in `.claude/`](#whats-in-claude))

---

## What's in `.claude/`

This repo ships a working set of Claude Code customizations. You'll study, run, and extend them in the labs, and you can copy them into your own projects.

| Path | What it is | Used in |
|---|---|---|
| [`.claude/agents/architecture-reviewer.md`](.claude/agents/architecture-reviewer.md) | Read-only subagent that checks Clean Architecture/DDD boundaries in both stacks | Labs 3, 12, 15 |
| [`.claude/agents/backlog-generator.md`](.claude/agents/backlog-generator.md) | Subagent that turns a feature request into INVEST user stories saved to `docs/backlog/` | Labs 3, 12 |
| [`.claude/agents/test-strategist.md`](.claude/agents/test-strategist.md) | Read-only subagent that plans tests (xUnit v3 / JUnit 5) before you write them | Labs 3, 12 |
| [`.claude/skills/test-data-generator/`](.claude/skills/test-data-generator/SKILL.md) | Skill (`/test-data-generator`) that generates builders, theory data, and JSON payloads through the real domain factories | Lab 11 |
| [`.claude/rules/dotnet.md`](.claude/rules/dotnet.md), [`.claude/rules/springboot.md`](.claude/rules/springboot.md) | Path-scoped rules that load only when Claude works on files in that stack | Lab 2 |
| [`.claude/rules/follow-up-questions.md`](.claude/rules/follow-up-questions.md) | Always-on rule: ask targeted clarifying questions before non-trivial changes | Lab 2 |
| [`.claude/settings.json`](.claude/settings.json) | Teaching artifact: allow/ask/deny permission rules plus audit-log hooks (requires `jq`) | Labs 1, 9 |

Not sure which mechanism to reach for? See the [Customization Decision Guide](docs/guides/customization-decision-guide.md) and the [Agent Design Guide](docs/guides/agent-design-guide.md).

---

## Relationship to the GitHub Copilot Workshop

This workshop is the Claude Code counterpart to Centric's GitHub Copilot workshop and follows a similar learning arc. If you've done that workshop, here's how the concepts map:

| GitHub Copilot | Claude Code | Where you'll see it |
|---|---|---|
| `.github/copilot-instructions.md` | `CLAUDE.md` (project, subdirectory, user) | Lab 2 |
| `*.instructions.md` with `applyTo` globs | `.claude/rules/*.md` with `paths:` frontmatter | Lab 2 |
| Prompt files (`*.prompt.md`) | Skills you invoke with `/name` (legacy `.claude/commands/*.md` still work) | Labs 8, 11 |
| Agent skills (`SKILL.md`) | Skills — same [Agent Skills](https://agentskills.io) open standard | Lab 11 |
| Custom agents (`*.agent.md`) | Subagents (`.claude/agents/*.md`) | Lab 12 |
| Agent handoffs | Subagent chaining ("use X, then use Y") | Lab 12 |
| Ask / Plan / Agent modes | Permission modes: Plan / Manual / Accept edits / Auto (Shift+Tab) | Labs 1, 4 |
| `@workspace` / codebase search | Agentic codebase reading and the built-in Explore subagent | Labs 1, 12 |
| MCP servers | MCP servers (`claude mcp add`, `.mcp.json`) | Lab 10 |
| Hooks | Hooks in `.claude/settings.json` | Lab 9 |
| Copilot code review | `/code-review` and `/security-review` | Labs 7, 8 |
| Copilot coding agent in GitHub Actions | Headless mode (`claude -p`) and the Claude Code GitHub Action | Lab 13 |
| — | Output styles | Lab 14 |
| — | Plugins (bundle skills, subagents, hooks, and MCP servers for sharing) | Lab 15 |

---


## Starter Solution Structure

**🔷 .NET Solution** (`src-dotnet/` directory):
- Clean Architecture: Domain / Application / Infrastructure / API layers
- `LegacyTaskProcessor` for refactoring exercises
- xUnit test stubs with FakeItEasy — `dotnet test` initially reports **1 passed, 11 skipped, 0 failed**; you implement the skipped stubs during the labs
- Starter domain: a `Task` with title, description, and status — priorities, due dates, and more are added during the labs

**🟩 Spring Boot Solution** (`src-springboot/` directory):
- Clean Architecture: domain / application / infrastructure / web layers
- `LegacyTaskProcessor` / `LegacyBatchImportService` for refactoring exercises
- JUnit 5 tests with Mockito

**Claude Code Configuration**:
- **`CLAUDE.md`** (project root): Workshop repo memory
- **`src-dotnet/CLAUDE.md`**: .NET context for Claude Code
- **`src-springboot/CLAUDE.md`**: Spring Boot context for Claude Code
- **`.claude/settings.json`**: Permissions and hooks example
- **`.claude/agents/`, `.claude/skills/`, `.claude/rules/`**: Shipped subagents, skill, and rules (see [What's in `.claude/`](#whats-in-claude))

---

## Getting Started

1. **Fork this repository** on GitHub (click **Fork** in the top-right corner).

   > Forking is required — not optional. Labs 8 (`/security-review` needs an `origin` remote), 10 (GitHub MCP), 13 (headless/CI), and 15 (the capstone) all work against your own remote. You'll push commits, trigger workflows, and query your fork's issues during the workshop.

2. **Clone your fork**:

   ```bash
   git clone https://github.com/YOUR-GITHUB-USERNAME/ai-coding-workshop-claude-code.git
   cd ai-coding-workshop-claude-code
   ```

3. **Add the upstream remote** (so you can pull workshop updates):

   ```bash
   git remote add upstream https://github.com/centricconsulting/ai-coding-workshop-claude-code.git
   ```

4. **Create your working branch**:

   ```bash
   git checkout -b workshop/your-name
   ```

5. **Verify Claude Code**:

   ```bash
   claude --version
   claude doctor
   claude   # Opens Claude Code (sign in on first run) — try asking "what is this project?"
   ```

6. **Verify your stack**:

   ```bash
   # .NET — expect 1 passed, 11 skipped, 0 failed (skipped = test stubs you'll implement)
   dotnet build src-dotnet && dotnet test src-dotnet

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
