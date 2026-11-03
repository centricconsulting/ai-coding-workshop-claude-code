# Workshop Lab Walkthroughs

Step-by-step guides for all 15 Claude Code workshop labs.

Each lab includes clear learning objectives, step-by-step instructions with prompts, expected outputs, troubleshooting, and extension exercises.

---

## Day 1: Fundamentals

### [Lab 1: First Contact](lab-01-first-contact.md)
**Duration**: 20–25 min

Install Claude Code with the native installer and sign in with a Claude subscription or API key. Understand the agentic model. Navigate the terminal UI: `/help`, `/context`, `/model`, `/effort`, Shift+Tab permission modes, and permission prompts. Run the tests (11 skipped stubs on .NET) through Claude Code.

**Key Concept**: The agentic model — Claude Code reads your codebase and executes actions, rather than suggesting completions.

---

### [Lab 2: CLAUDE.md & Project Memory](lab-02-claude-md-memory.md)
**Duration**: 40–45 min

Understand the CLAUDE.md hierarchy (project / subdirectory / user), `/init`, `@` imports, and path-scoped `.claude/rules/`. Write CLAUDE.md files that measurably improve Claude Code's output. Understand what belongs in CLAUDE.md vs. what doesn't.

**Key Concept**: CLAUDE.md is project memory — write it once, Claude Code uses it every session.

---

### [Lab 3: TDD & Requirements → Code](lab-03-tdd-with-claude-code.md)
**Duration**: 90–100 min (Part 1: 45–50 · Part 2: ~45) — bilingual (🔷 .NET and 🟩 Spring Boot)

Apply the full Red-Green-Refactor TDD cycle. Review multi-file diffs before accepting. Use tests as a feedback loop — Claude Code runs them and fixes failures automatically. Then take a user story from requirements to code: a `Priority` value object, a `DueDate`, and a `POST /tasks` endpoint, with the key design decision recorded.

**Key Concept**: Directing a workflow vs. accepting individual completions — and making requirements, not prompts, the source of truth.

---

### [Lab 4: Plan Mode, Permission Modes & Thinking](lab-04-plan-mode-and-extended-thinking.md)
**Duration**: 25–30 min

Compare the permission modes (Manual, Accept edits, Plan, Auto) you cycle with Shift+Tab. Enter Plan Mode and draft a phased refactor plan with zero file edits. Iterate risk-free before approving. Control reasoning depth with `/effort` (or `ultrathink` for a single turn) and know when the added latency is worth it.

**Key Concept**: Plan Mode is a hard boundary — while active, Claude Code can propose but not touch files. The plan from this lab is what Lab 5 executes.

---

### [Lab 5: Multi-File Refactoring & Context Management](lab-05-refactoring-and-context.md)
**Duration**: 45–50 min — bilingual (🔷 .NET and 🟩 Spring Boot)

Write characterization tests before refactoring. Refactor LegacyTaskProcessor in phases, starting from the plan drafted in Lab 4. Watch usage with `/context` and use `/compact` before hitting context limits.

**Key Concept**: Large tasks need phases and checkpoints; `/compact` is a proactive tool, not an emergency measure.

---

### [Lab 6: Checkpoints & Rewind](lab-06-checkpoints-and-rewind.md)
**Duration**: 20 min

Deliberately damage the Lab 5 refactor with an over-broad follow-up edit, then recover using rewind — a session-level safety net separate from git.

**Key Concept**: Checkpoints/rewind solve session-local recovery; git solves durable, cross-session recovery. Know which tool fits which situation.

---

### [Lab 7: Git Workflow & PR Preparation](lab-07-git-workflow.md)
**Duration**: 20–25 min

Use `/code-review` (alias `/review`) for pre-commit code review. Generate conventional commit messages grounded in actual diffs. Produce a complete PR description from session context.

**Key Concept**: `/code-review` is a quality gate, not a post-hoc check.

---

## Day 2: Advanced Claude Code

### [Lab 8: Security Review Workflows](lab-08-security-review-workflows.md)
**Duration**: 30–35 min

Introduce a realistic cross-tenant authorization gap, see why `/security-review` (requires an `origin` remote — your fork) doesn't report it, build a project-specific `/security-checklist` skill that does, and fix the gap at the root cause.

**Key Concept**: `/security-review` is a dedicated, deeper gate than `/code-review` — and Day 2 opens with it, reviewing the branch you built on Day 1.

---

### [Lab 9: Permissions, Safety & Hooks](lab-09-permissions-and-hooks.md)
**Duration**: 25–30 min

Understand permission rules (allow / ask / deny), settings scopes, and permission modes. Study and modify the shipped `.claude/settings.json`. Write pre- and post-tool hooks that read their JSON input from stdin (with `jq`) for workflow automation.

**Key Concept**: The permissions model is a safety boundary — the deny list is as important as the allow list.

---

### [Lab 10: MCP Servers](lab-10-mcp-servers.md)
**Duration**: 30–35 min

Understand MCP (Model Context Protocol) and why it exists. Add servers with `claude mcp add`, choose a scope (local / project / user), and connect GitHub's MCP server to your fork. Use MCP-backed tools in real workflows.

**Key Concept**: MCP adds new tool capabilities; hooks add side effects on existing tools — they're complementary, not competing.

---

### [Lab 11: Skills](lab-11-skills.md)
**Duration**: 30–35 min

Learn the SKILL.md format, frontmatter, `$ARGUMENTS`, and progressive disclosure. Explore the shipped `test-data-generator` skill, then build your own — a Clean Architecture boundary checker — and confirm it activates on matching prompts without false-triggering. Custom slash commands are now skills.

**Key Concept**: A Skill is a packaged, on-demand procedure — not a new tool (MCP) and not always-on context (CLAUDE.md).

---

### [Lab 12: Custom Subagents & Agent Workflows](lab-12-sub-agents.md)
**Duration**: 35–40 min

Meet the built-in subagents (Explore, Plan, general-purpose). Use the shipped `.claude/agents/` — `backlog-generator`, `architecture-reviewer`, and `test-strategist` — and chain them: requirement → user stories → architecture check → test plan. Learn the design principles behind a good subagent: a clear description, least-privilege tools, and a defined output contract.

**Key Concept**: Subagents give each specialist its own context window and tool set; chaining them turns a big task into focused, reviewable handoffs.

---

### [Lab 13: Headless & Automation Mode](lab-13-headless-automation.md)
**Duration**: 25–30 min

Run Claude Code non-interactively with `-p` (`--print`) and structured `--output-format`. Use `--continue` and `--resume`. Integrate Claude Code into CI pipelines. Understand the safety implications of headless mode.

**Key Concept**: Headless mode removes the human-in-the-loop — restrictive permissions are mandatory.

---

### [Lab 14: Output Styles & Custom Personas](lab-14-output-styles-and-personas.md)
**Duration**: 20–25 min · **Self-study** (not run in class by default)

Create and activate a custom output style, then compare its behavior against the default side by side. Discuss standardizing a style at the team level vs. leaving it personal.

**Key Concept**: An output style is a persistent, system-prompt-level change to *how* Claude Code communicates — distinct from every other mechanism, which changes what it knows or can do.

---

### [Lab 15: Capstone — Design a Team Configuration](lab-15-capstone-team-config.md)
**Duration**: 60–65 min

Design a complete team Claude Code configuration: CLAUDE.md hierarchy and rules, permissions & hooks, MCP integration, and governance. Build your own subagent or skill, package it as a plugin (`.claude-plugin/plugin.json`), and test it with `--plugin-dir`. Present and defend your design.

**Key Concept**: A well-designed team configuration multiplies Claude Code's value across every developer.

---

## Learning Path

### Day 1: Fundamentals

```
Lab 1 (Agent Model)
    ↓
Lab 2 (CLAUDE.md)
    ↓
Lab 3 (TDD & Requirements → Code)
    ↓
Lab 4 (Plan Mode, Permission Modes & Thinking)
    ↓
Lab 5 (Refactoring + Context)
    ↓
Lab 6 (Checkpoints & Rewind)
    ↓
Lab 7 (Git Workflow)
```

### Day 2: Advanced

```
Lab 8 (Security Review)
    ↓
Lab 9 (Permissions & Hooks)
    ↓
Lab 10 (MCP)
    ↓
Lab 11 (Skills)
    ↓
Lab 12 (Custom Subagents)
    ↓
Lab 13 (Headless)
    ↓
Lab 15 (Capstone + Plugin)
    ↓
Apply to Real Projects!
```

**Self-study:** Lab 14 (Output Styles) — the least dependent lab; do it after the workshop.

---

## Time Estimates

"Minimum" and "Comfortable" are the lower and upper ends of each lab's stated duration.

| Lab | Minimum | Comfortable | With Extensions |
|---|---|---|---|
| **Day 1** | | | |
| Lab 1 | 20 min | 25 min | 35 min |
| Lab 2 | 40 min | 45 min | 60 min |
| Lab 3 | 90 min | 100 min | 130 min |
| Lab 4 | 25 min | 30 min | 40 min |
| Lab 5 | 45 min | 50 min | 65 min |
| Lab 6 | 20 min | 20 min | 30 min |
| Lab 7 | 20 min | 25 min | 35 min |
| **Day 1 Total** | **260 min** | **295 min** | **395 min** |
| **Day 2** | | | |
| Lab 8 | 30 min | 35 min | 45 min |
| Lab 9 | 25 min | 30 min | 40 min |
| Lab 10 | 30 min | 35 min | 50 min |
| Lab 11 | 30 min | 35 min | 45 min |
| Lab 12 | 35 min | 40 min | 55 min |
| Lab 13 | 25 min | 30 min | 40 min |
| Lab 15 | 60 min | 65 min | 90 min |
| **Day 2 Total** | **235 min** | **270 min** | **365 min** |
| **Self-study** | | | |
| Lab 14 | 20 min | 25 min | 35 min |
| **Full Workshop** (incl. Lab 14) | **515 min** | **590 min** | **795 min** |

(Lab-only totals — module intros, kickoff, tour, and wrap-up add roughly 90 minutes per day; see the root [README schedule](../../README.md#schedule) for the day-by-day agenda and the [Facilitator Guide](../../FACILITATOR_GUIDE.md) for breaks and fast-track options.)

---

## Common Slash Commands Reference

| Command | When to Use |
|---|---|
| `/help` | List available commands |
| `/init` | Generate or improve a `CLAUDE.md` for the project (Lab 2) |
| `/context` | See what's using the context window |
| `/model` / `/effort` | Choose a model / set reasoning effort (Lab 4) |
| `/compact` | Summarize conversation history when sessions grow long |
| `/clear` | Start a fresh conversation |
| `/rewind` | Restore code and/or conversation to an earlier checkpoint (Lab 6) |
| `/code-review` | Review current changes, a branch, or a PR — `/review` is an alias (Lab 7) |
| `/security-review` | Run a security-focused review; needs an `origin` remote (Lab 8) |
| `/permissions` | View and edit permission rules (Lab 9) |
| `/mcp` | Manage MCP servers and authenticate (Lab 10) |
| `/plugin` | Browse and install plugins (Lab 15) |
| `/status` | Show version, model, account, and active credential |
| `/exit` | Exit Claude Code |

Keyboard: **Shift+Tab** cycles permission modes, **Esc** interrupts, **Esc Esc** opens rewind.

---

## Getting Started

1. Fork the repo, clone your fork, and create your branch: `git checkout -b workshop/your-name`
2. Verify Claude Code: `claude --version` and `claude doctor`
3. Verify your stack: `dotnet build src-dotnet && dotnet test src-dotnet` (expect 1 passed, 11 skipped, 0 failed) or `cd src-springboot && mvn clean test` (expect about 100 tests, all passing)
4. Start with [Lab 1](lab-01-first-contact.md)

New to choosing between CLAUDE.md, rules, skills, subagents, hooks, and MCP? See the [Customization Decision Guide](../guides/customization-decision-guide.md).
