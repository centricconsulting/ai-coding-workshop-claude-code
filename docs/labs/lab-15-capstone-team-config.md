# Lab 15: Capstone — Design a Team Claude Code Configuration

**Duration**: 60–65 minutes
**Day**: 2 of 2 (Advanced)

**Learning Objectives**:
- Design a complete team Claude Code configuration from scratch
- Apply CLAUDE.md, rules, permissions, hooks, and MCP together
- Build, test, and refine your own subagent and/or skill
- Package the team configuration as a plugin and install it from a local marketplace
- Think through governance: who owns the config, how it evolves
- Present and defend your design to peers

---

## Overview

This capstone combines two things:

1. **A team configuration** — the CLAUDE.md files, rules, `.claude/settings.json`, and MCP setup that encode your team's standards so Claude Code works well for everyone without constant re-prompting. This is *ambient*: it shapes every interaction.
2. **Your own specialist** — a custom subagent or skill that performs one workflow well, the way the shipped `architecture-reviewer`, `backlog-generator`, and `test-strategist` subagents and the `test-data-generator` skill do. This is *invoked*: you (or Claude) call it by name.

You'll then **package both as a plugin** — the way teams actually distribute Claude Code configuration across repositories and developers.

A well-designed team configuration multiplies the value of Claude Code across every developer on the team. Plugins are how you ship it.

---

## Prerequisites

- Completed Labs 1–14
- Familiarity with your team's actual development workflow (from work, not this workshop)
- `jq` installed (used by the hook script)
- Skim these references before you start:
  - [Customization Decision Guide](../guides/customization-decision-guide.md) — CLAUDE.md vs. rules vs. skills vs. subagents vs. hooks vs. MCP vs. output styles vs. plugins
  - [Agent Design Guide](../guides/agent-design-guide.md) — principles for focused, testable subagents
  - Shipped examples in this repo: `.claude/agents/`, `.claude/skills/test-data-generator/SKILL.md`, `.claude/rules/`

---

## Part A: Choose Your Scenario (5 min)

Pick one of these team scenarios that most closely matches your real work. If your team has a different scenario, use that instead.

| # | Scenario | Stack | Key Challenge |
|---|---|---|---|
| A | Enterprise .NET API team | .NET / C# | Clean Architecture enforcement, ADO work items |
| B | Spring Boot microservices team | Java / Spring Boot | Service boundaries, Kafka integration |
| C | Full-stack TypeScript team | TypeScript / React / Node | Frontend + backend conventions differ |
| D | Data engineering team | Python / SQL | Pipeline conventions, data validation |
| E | Your team's actual scenario | — | — |

Write down:
- Your chosen scenario
- The 3 biggest friction points where Claude Code produces inconsistent or incorrect output today
- The 3 most valuable automations that would save your team time
- The **one specialist workflow** you'd most like to hand to a dedicated subagent or skill (you'll build it in Part E)

> **Working location:** do the design work in a scratch folder in this repo, e.g. `capstone/`, so it doesn't collide with the workshop's own `.claude/` config. You'll turn it into a plugin in Part F.

---

## Part B: Design the CLAUDE.md Hierarchy and Rules (10 min)

### B.1 Map the Directory Structure

For your scenario, draw the directory tree and mark where each memory file goes:

```
project-root/
  CLAUDE.md                ← What goes here? (always loaded)
  CLAUDE.local.md          ← Personal, gitignored — what belongs here vs. the team file?
  .claude/
    rules/
      backend.md           ← paths: ["src-dotnet/src/**/*.cs"] — loaded only when matching files are in play
      tests.md             ← paths: ["src-dotnet/tests/**"]
  src-dotnet/
    CLAUDE.md              ← What goes here? (loaded when Claude works in src-dotnet/)
  docs/
    CLAUDE.md              ← Maybe not needed
```

**CLAUDE.md vs. `.claude/rules/`:** subdirectory CLAUDE.md files are scoped by *directory*; rule files with `paths:` frontmatter are scoped by *glob*, so one rule can cover `**/*Test*.cs` across the whole repo. Look at this repo's `.claude/rules/dotnet.md` and `.claude/rules/springboot.md` for the pattern.

### B.2 Write the Root CLAUDE.md

Ask Claude Code to help you draft it:

```
I'm designing a CLAUDE.md for [your scenario]. Help me write the root-level CLAUDE.md. It should cover: what the project is, the tech stack, how to build and test, key conventions, and what Claude Code should NOT do. Keep it under 100 lines — move stack-specific detail into .claude/rules/ files. Ask me questions to fill in any gaps.
```

Claude Code will ask clarifying questions — answer them as if this were your real project. Iterate on the draft until it accurately reflects your team's standards.

### B.3 Write One Path-Scoped Rule

```
Now draft a .claude/rules/ file for [your main code area], with paths: frontmatter that targets only those files. Cover: architecture layer responsibilities, naming, patterns to use and avoid, and domain vocabulary. Use .claude/rules/dotnet.md in this repo as a format reference.
```

### B.4 Review: Would This Actually Help?

Test your CLAUDE.md with a prompt that often goes wrong without it:

```
Create a new [feature component appropriate to your scenario] following our conventions.
```

If Claude Code produces something inconsistent with your CLAUDE.md, refine it. Use `/memory` to confirm which memory files are loaded.

---

## Part C: Design the Permissions & Hooks (10 min)

### C.1 Permissions Design

Answer these questions for your scenario:

1. **What commands should always be allowed?** (Build, test, lint — safe reads and compilations)
2. **What commands should require confirmation?** (Deploy, migrate, push)
3. **What commands should be denied outright?** (Destructive deletes, prod database access)

Write the `allow` / `ask` / `deny` lists in `.claude/settings.json` format:

```json
{
  "permissions": {
    "allow": ["Bash(dotnet build *)", "Bash(dotnet test *)", "Bash(git status)", "Bash(git diff *)"],
    "ask":   ["Bash(git push *)", "Bash(dotnet ef *)"],
    "deny":  ["Bash(rm -rf *)", "Read(./.env)", "Read(./secrets/**)"]
  }
}
```

> Rules beat hooks for simple allow/deny decisions — they're declarative and visible in `/permissions`. Use a hook when the decision needs **logic** (inspect arguments, check the branch, call a policy service) or a **side effect** (format, test, audit).

### C.2 Hooks Design

For your scenario, identify 2–3 hooks that would save real time or enforce real quality gates. Remember:

- Hook input arrives as **JSON on stdin** — parse it with `jq` (e.g. `.tool_input.file_path`, `.tool_input.command`). There is no `$CLAUDE_TOOL_INPUT` variable.
- `matcher` matches the **tool name** (`Bash`, `Edit|Write`, `mcp__github__.*`); filter file types inside the script, or with the handler's `if` field (permission-rule syntax, e.g. `"if": "Bash(git push *)"`).
- **Exit 0** = proceed. **Exit 2** = block (PreToolUse) and feed stderr back to Claude. **Any other exit code (including 1) is a non-blocking error — the action still runs.**

| Hook Event | Matcher | What the script does | Why |
|---|---|---|---|
| PostToolUse | `Edit\|Write` | If `.tool_input.file_path` ends in `.cs`, run `dotnet format --include <file>` | Auto-format on every edit |
| PostToolUse | `Edit\|Write` | If the file is a test file, run `dotnet test --filter <class>` | Instant test feedback |
| PreToolUse | `Bash` | If `.tool_input.command` contains `git push`, print a reason to stderr and `exit 2` | Block accidental push |
| SessionStart | — | Print current branch and open ticket to stdout (added to context) | Orient every session |

**Example: a PreToolUse hook that actually blocks `git push`**

`.claude/hooks/block-git-push.sh`:

```bash
#!/bin/bash
# Hook input is JSON on stdin
COMMAND=$(jq -r '.tool_input.command // empty')

if echo "$COMMAND" | grep -qE '(^|[;&|[:space:]])git[[:space:]]+push([[:space:]]|$)'; then
  echo "Blocked by team policy: Claude may not run 'git push'. Ask a human to push." >&2
  exit 2   # exit 2 = block the tool call; stderr is shown to Claude
fi

exit 0
```

Wire it up in `.claude/settings.json`:

```json
{
  "hooks": {
    "PreToolUse": [
      {
        "matcher": "Bash",
        "hooks": [
          { "type": "command", "command": "\"$CLAUDE_PROJECT_DIR\"/.claude/hooks/block-git-push.sh" }
        ]
      }
    ]
  }
}
```

> **Alternative — structured JSON output:** instead of `exit 2`, a PreToolUse hook can `exit 0` and print
> `{"hookSpecificOutput": {"hookEventName": "PreToolUse", "permissionDecision": "deny", "permissionDecisionReason": "git push is blocked by team policy"}}`.
> `permissionDecision` can also be `"allow"` or `"ask"`, which lets one hook encode richer policy.

Write the hooks section for your scenario's `.claude/settings.json`, then check it with `/hooks`.

### C.3 Validate the Design

```
Review my .claude/settings.json design for this scenario: [paste your settings]. Are there any gaps — commands I should have denied but didn't, hooks that would create friction instead of helping, or hooks that exit 1 when they meant to block?
```

---

## Part D: Design the MCP Integration (5 min)

### D.1 Identify External Systems

For your scenario, list the external systems Claude Code would benefit from accessing:

- **Ticket system**: Jira, ADO, Linear — so Claude Code can query real requirements
- **Documentation**: Confluence, Notion — so Claude Code can read architecture decisions
- **Observability**: Datadog, Grafana, Sentry — so Claude Code can check error rates (read-only)
- **Source control / CI**: GitHub's official MCP server — issues, PRs, Actions status

### D.2 Choose Your MCP Strategy

For each external system, decide:
- **Use an existing MCP server** (GitHub's official server, vendor-hosted servers from the Anthropic Directory)
- **Build a custom MCP server** (internal systems)
- **Skip MCP, use a CLI + permission rule** (a simple `Bash(az boards *)` allow is sufficient)
- **Skip it entirely** (not worth the complexity)

Document your decisions and rationale.

### D.3 Design the `.mcp.json`

Write the project-scoped `.mcp.json` for your scenario (placeholders are fine). Use `${VAR}` expansion so no secret is ever committed:

```json
{
  "mcpServers": {
    "github": {
      "type": "http",
      "url": "https://api.githubcopilot.com/mcp/",
      "headers": { "Authorization": "Bearer ${GITHUB_MCP_PAT}" }
    },
    "jira": {
      "type": "stdio",
      "command": "node",
      "args": ["${JIRA_MCP_SERVER_PATH:-./tools/jira-mcp/index.js}"],
      "env": {
        "JIRA_BASE_URL": "https://your-org.atlassian.net",
        "JIRA_API_TOKEN": "${JIRA_API_TOKEN}"
      }
    }
  }
}
```

---

## Part E: Build Your Own Subagent or Skill (15 min)

The team configuration is ambient. Now build one **specialist** that your team would invoke for a specific workflow — the Claude Code equivalent of "build your own agent."

### E.1 Pick the Right Mechanism

| Build a… | When | Lives in |
|---|---|---|
| **Subagent** | The work benefits from its **own context window**, its own tool restrictions, or a different model — e.g. a reviewer that reads a lot and returns a short verdict | `.claude/agents/<name>.md` |
| **Skill** | It's a **procedure or template** Claude should follow in the main conversation — e.g. a checklist, a generator, a release process | `.claude/skills/<name>/SKILL.md` |

Not sure? See the [Customization Decision Guide](../guides/customization-decision-guide.md). Ambitious teams build both: a skill that encodes the procedure and a subagent that preloads it (`skills:` frontmatter).

**Role ideas:**
- **Code reviewer** for your stack's conventions (compare with the shipped `architecture-reviewer`)
- **Documentation writer** — XML docs / Javadoc / ADRs
- **Performance auditor** — N+1 queries, allocations in hot paths
- **Security reviewer** — OWASP checks for your framework
- **Migration planner**, **API contract checker**, **release-notes writer** — or your own

### E.2 Define Success Criteria First

Before writing the definition, write down (in the spirit of TDD):

- **3 things it must catch or produce** (e.g. "flags a domain entity that references EF Core")
- **2 things it must *not* do** (e.g. "never edits files", "doesn't comment on formatting")
- **The output format** you expect (sections, severity levels, max length)
- **One test input** in this repo (e.g. `src-dotnet/src/TaskManager.Infrastructure/Legacy/LegacyTaskProcessor.cs`)

### E.3 Create the Definition

**Option 1 — Subagent.** Study `.claude/agents/architecture-reviewer.md`, then:

```
Create .claude/agents/<your-agent-name>.md for a [role] subagent. Use .claude/agents/architecture-reviewer.md as a format reference and follow docs/guides/agent-design-guide.md. Requirements:
- description: a precise "use when..." sentence so Claude knows when to delegate
- tools: least privilege (read-only unless it truly must write)
- model: choose deliberately (haiku for fast/cheap checks, sonnet/opus for deep reasoning) or inherit
- body: responsibilities, constraints, step-by-step process, and an exact output format
Here are my success criteria: [paste E.2]. Show me the file before writing it.
```

A skeleton for reference:

```markdown
---
name: perf-auditor
description: Use when reviewing C# or Java code for performance problems (N+1 queries, allocations in loops, blocking async). Read-only; returns a ranked findings list.
tools: Read, Grep, Glob
model: sonnet
---

You are a performance auditor for the TaskManager codebase.

## Constraints
- Never edit files. Report only.
- Ignore style and naming — performance only.

## Process
1. Identify hot paths (handlers, repositories, loops over collections).
2. Check for: N+1 data access, sync-over-async, allocations in loops, missing pagination.
3. Rank findings by expected impact.

## Output format
### Findings (highest impact first)
- **[Severity]** `File.cs:line` — problem — suggested fix
### Verdict
One sentence.
```

**Option 2 — Skill.** Study `.claude/skills/test-data-generator/SKILL.md`, then:

```
Create .claude/skills/<your-skill-name>/SKILL.md for [procedure]. Use .claude/skills/test-data-generator/SKILL.md as a format reference. Include name, description (when to use it), argument-hint, and allowed-tools in the frontmatter. Use $ARGUMENTS for the input. Set disable-model-invocation: true only if it should run exclusively when a human types the command. Here are my success criteria: [paste E.2]. Show me the file before writing it.
```

> New agent or skill files are picked up when Claude Code starts. If yours doesn't appear, restart the session. (`/agents` just reminds you where agent files live — you create them as files, as above.)

### E.4 Test It

Run it against your test input:

```
Use the <your-agent-name> subagent to review src-dotnet/src/TaskManager.Infrastructure/Legacy/LegacyTaskProcessor.cs.
```

or

```
/<your-skill-name> <arguments>
```

Score the result against your E.2 criteria: ✅ caught / ❌ missed / ⚠️ did something it shouldn't.

### E.5 Iterate

| Symptom | Likely fix |
|---|---|
| Claude never delegates to it automatically | Make `description` more specific: "Use when…" + concrete triggers |
| Output format drifts | Put an exact template in the body and say "use exactly this format" |
| It edits files when it shouldn't | Remove `Edit`/`Write` from `tools` (don't rely on instructions alone) |
| Too generic / misses project specifics | Point it at CLAUDE.md conventions or preload a skill with `skills:` |
| Too slow or expensive | Narrow scope; try `model: haiku` |

Do at least one refine-and-retest cycle.

---

## Part F: Package It as a Plugin (10 min)

So far everything lives in one repo's `.claude/` folder. A **plugin** bundles skills, subagents, hooks, and MCP servers into one installable unit that teammates enable with a single command — across every repository that needs it.

### F.1 Create the Plugin Structure

From your capstone folder (or the repo root):

```bash
mkdir -p team-plugin/.claude-plugin team-plugin/agents team-plugin/skills team-plugin/hooks
```

Create `team-plugin/.claude-plugin/plugin.json`:

```json
{
  "name": "taskmanager-team",
  "description": "TaskManager team standards: reviewers, generators, and guardrail hooks",
  "version": "0.1.0",
  "author": { "name": "Your Team" }
}
```

> Only `plugin.json` goes inside `.claude-plugin/`. Components go at the plugin root.

### F.2 Copy In Your Components

```bash
cp .claude/agents/<your-agent-name>.md team-plugin/agents/
cp -r .claude/skills/<your-skill-name> team-plugin/skills/          # if you built a skill
mkdir -p team-plugin/scripts && cp .claude/hooks/block-git-push.sh team-plugin/scripts/
```

Create `team-plugin/hooks/hooks.json` — same shape as the `hooks` object in settings, but reference scripts via `${CLAUDE_PLUGIN_ROOT}`:

```json
{
  "hooks": {
    "PreToolUse": [
      {
        "matcher": "Bash",
        "hooks": [
          { "type": "command", "command": "\"${CLAUDE_PLUGIN_ROOT}\"/scripts/block-git-push.sh" }
        ]
      }
    ]
  }
}
```

Optionally add your `.mcp.json` from Part D at the plugin root.

Resulting layout:

```
team-plugin/
  .claude-plugin/plugin.json
  agents/<your-agent-name>.md
  skills/<your-skill-name>/SKILL.md
  hooks/hooks.json
  scripts/block-git-push.sh
  .mcp.json                    (optional)
```

### F.3 Validate and Try It

```bash
claude plugin validate ./team-plugin
claude --plugin-dir ./team-plugin
```

Inside that session, plugin components are **namespaced** by the plugin name:
- Skills run as `/taskmanager-team:<your-skill-name>`
- Subagents appear as `taskmanager-team:<your-agent-name>`
- Run `/plugin` to see it loaded, and `/reload-plugins` after editing files

### F.4 Publish Through a Local Marketplace

A marketplace is just a catalog — a directory (or git repo) with `.claude-plugin/marketplace.json`:

```bash
mkdir -p team-marketplace/.claude-plugin team-marketplace/plugins
cp -r team-plugin team-marketplace/plugins/taskmanager-team
```

`team-marketplace/.claude-plugin/marketplace.json`:

```json
{
  "name": "taskmanager-team-marketplace",
  "description": "TaskManager team plugins",
  "owner": { "name": "Your Team" },
  "plugins": [
    {
      "name": "taskmanager-team",
      "source": "./plugins/taskmanager-team",
      "description": "TaskManager team standards"
    }
  ]
}
```

```bash
claude plugin validate ./team-marketplace
claude plugin marketplace add ./team-marketplace
claude plugin install taskmanager-team@taskmanager-team-marketplace
claude plugin list
```

(Inside a session, the equivalents are `/plugin marketplace add ./team-marketplace` and `/plugin install taskmanager-team@taskmanager-team-marketplace`.)

In real life you'd push `team-marketplace/` to a git repo; teammates run `claude plugin marketplace add your-org/your-marketplace-repo`. To turn it on for everyone in a project, commit the marketplace and plugin to the repo's `.claude/settings.json`:

```json
{
  "extraKnownMarketplaces": {
    "taskmanager-team-marketplace": {
      "source": { "source": "github", "repo": "your-org/taskmanager-team-marketplace" }
    }
  },
  "enabledPlugins": {
    "taskmanager-team@taskmanager-team-marketplace": true
  }
}
```

Teammates who trust the folder are prompted to install it. Organizations can go further with **managed settings** (force-install plugins, restrict which marketplaces are allowed) — see the [Agent Governance Guide](../guides/agent-governance.md).

> **Clean up:** `claude plugin uninstall taskmanager-team@taskmanager-team-marketplace` and `claude plugin marketplace remove taskmanager-team-marketplace` when you're done experimenting. If you copied your hook into both `.claude/settings.json` and the plugin, it runs twice — keep one.

---

## Part G: Present Your Design (10 min)

### G.1 Document the Configuration

Write a brief `docs/guides/claude-code-team-guide.md` (or the plugin's `README.md`) that explains:
1. How to onboard: install the plugin, approve `.mcp.json` servers, what to read first
2. The most important CLAUDE.md conventions and rules to know
3. What the hooks do and why
4. Your custom subagent/skill: when to use it, how to invoke it, sample output
5. MCP servers available and what they're for
6. How to propose changes to the configuration

```
Help me write a one-page team guide for our Claude Code configuration and plugin. It should be practical — what does a new developer need to know to use Claude Code effectively in our setup?
```

### G.2 Peer Review (group exercise)

Share your configuration and plugin with another participant. Better yet, have them **install your plugin** from your marketplace folder and run your subagent/skill against their own test input. Review each other's designs for:

- **CLAUDE.md / rules**: Is it complete? Is anything always-loaded that should be path-scoped?
- **Permissions**: Are the deny rules tight enough? Are allows too broad?
- **Hooks**: Do the blocking hooks really `exit 2`? Will they create friction or save time?
- **Subagent / skill**: Does the description trigger delegation correctly? Are its tools least-privilege? Did it meet its success criteria?
- **MCP**: Is the complexity justified? Could a simpler approach work?

### G.3 Governance Questions

Discuss with your partner or the group:

1. Who owns the CLAUDE.md files and the team plugin? (Engineering lead? All devs via PR process?)
2. How do you version the plugin — and when do teammates get updates?
3. What's the process for a developer to propose a new hook, permission, subagent, or skill?
4. What belongs in the plugin (shared across repos) vs. the repo's `.claude/` (this repo only) vs. `~/.claude/` (personal)?
5. Which settings, if any, should the organization enforce via managed settings?

---

## Reflection: Configuration *and* Agents

The GitHub Copilot version of this workshop ends with "build an agent." Claude Code's model lets you do both — and they reinforce each other:

- **Ambient configuration** (CLAUDE.md, rules, permissions, hooks, output styles) shapes *every* interaction without deliberate invocation — like training an employee with good defaults and guardrails.
- **Specialists** (subagents, skills) are invoked by name or delegated to automatically for a specific workflow — like building a focused tool.
- **Plugins** package both so they travel with the team instead of living in one developer's `.claude/` folder.

The strongest team setups use all three: ambient defaults for consistency, a small set of well-tested specialists for high-value workflows, and a plugin to distribute them.

---

## Success Criteria

- [ ] Root CLAUDE.md written for your scenario — tested with at least one prompt
- [ ] At least one path-scoped `.claude/rules/` file with stack-specific conventions
- [ ] `.claude/settings.json` with allow/ask/deny rules and at least 2 hooks designed — any blocking hook reads stdin with `jq` and uses `exit 2` (or a JSON `deny`)
- [ ] MCP integration decided (even if the decision is "not needed")
- [ ] A custom subagent or skill created, tested against written success criteria, and refined at least once
- [ ] A plugin with `.claude-plugin/plugin.json` that passes `claude plugin validate` and loads with `--plugin-dir` or from a local marketplace
- [ ] Team guide written (at least a draft)
- [ ] You can articulate the governance model for maintaining the configuration

---

## Extension Exercise

Write a `scripts/validate-claude-config.sh` script that:
1. Checks that all required CLAUDE.md files exist
2. Validates `.claude/settings.json` and `.mcp.json` are valid JSON (`jq empty <file>`)
3. Runs `claude plugin validate` on your plugin and marketplace
4. Greps hook scripts for `exit 1` used where a block was intended
5. Prints a summary: "Claude Code configuration is valid" or lists issues

This is the kind of script you'd run in CI (Lab 13) to ensure the team configuration stays healthy.

---

**Congratulations — you've completed the Claude Code workshop!**

---

## Next Steps

- Apply the CLAUDE.md, rules, and hooks patterns to your real project this week
- Turn one repeated workflow on your team into a subagent or skill, and ship it in a team plugin
- Browse the official plugin marketplace (`/plugin` → Discover) for ideas before building your own
- Explore the [Claude Code documentation](https://code.claude.com/docs/en/overview) for features not covered in this workshop
- Read the [Agent Governance Guide](../guides/agent-governance.md) for how to manage Claude Code configuration at enterprise scale
