# Lab 10: Capstone — Design a Team Claude Code Configuration

**Duration**: 45–50 minutes
**Part**: 2 of 2 (Advanced)

**Learning Objectives**:
- Design a complete team Claude Code configuration from scratch
- Apply CLAUDE.md, hooks, permissions, and MCP together
- Think through governance: who owns the config, how it evolves
- Present and defend your design to peers

---

## Overview

The Copilot workshop capstone (Lab 10) was "build your own agent." The Claude Code capstone is different: you design a *team configuration* — the CLAUDE.md files, `.claude/settings.json`, and MCP setup that encodes your team's standards so Claude Code works well for everyone without constant re-prompting.

This is the highest-leverage Claude Code skill: a well-designed team configuration multiplies the value of Claude Code across every developer on the team.

---

## Prerequisites

- Completed Labs 1–9
- Familiarity with your team's actual development workflow (from work, not this workshop)

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

---

## Part B: Design the CLAUDE.md Hierarchy (12 min)

Design the CLAUDE.md files your scenario needs.

### B.1 Map the Directory Structure

For your scenario, draw the directory tree and mark where each CLAUDE.md goes:

```
project-root/
  CLAUDE.md            ← What goes here?
  src/
    CLAUDE.md          ← What goes here?
  tests/
    CLAUDE.md          ← What goes here?
  docs/
    CLAUDE.md          ← What goes here? (maybe not needed)
  .github/
    workflows/
      CLAUDE.md        ← What goes here? (CI context)
```

### B.2 Write the Root CLAUDE.md

Ask Claude Code to help you draft it:

```
I'm designing a CLAUDE.md for [your scenario]. Help me write the root-level CLAUDE.md. It should cover: what the project is, the tech stack, how to build and test, key conventions, and what Claude Code should NOT do. Ask me questions to fill in any gaps.
```

Claude Code will ask clarifying questions — answer them as if this were your real project. Iterate on the draft until it accurately reflects your team's standards.

### B.3 Write the src/ CLAUDE.md

```
Now draft the CLAUDE.md for the src/ directory specifically. This should cover: architecture layer responsibilities, coding conventions (naming, patterns, what to avoid), key file locations, and domain vocabulary.
```

### B.4 Review: Would This Actually Help?

Test your CLAUDE.md with a prompt that often goes wrong without it:

```
Create a new [feature component appropriate to your scenario] following our conventions.
```

If Claude Code produces something inconsistent with your CLAUDE.md, refine it.

---

## Part C: Design the Permissions & Hooks (10 min)

### C.1 Permissions Design

Answer these questions for your scenario:

1. **What commands should always be allowed?** (Build, test, lint — safe reads and compilations)
2. **What commands should never run without confirmation?** (Deploy, migrate, push)
3. **What commands should be denied outright?** (Destructive deletes, prod database access)

Write the allow/deny lists in `.claude/settings.json` format.

### C.2 Hooks Design

For your scenario, identify 2–3 hooks that would save real time or enforce real quality gates:

| Hook Event | Trigger | Command | Why |
|---|---|---|---|
| PostToolUse | Write `*.cs` | `dotnet format` | Auto-format on every file write |
| PostToolUse | Write `*Test*.cs` | `dotnet test --filter [filename]` | Instant test feedback |
| PreToolUse | Bash(`git push`) | `echo "Push blocked in CI" && exit 1` | Prevent accidental push |

Write the hooks section for your scenario's `.claude/settings.json`.

### C.3 Validate the Design

```
Review my .claude/settings.json design for this scenario: [paste your settings]. Are there any gaps — commands I should have denied but didn't, or hooks that would create friction instead of helping?
```

---

## Part D: Design the MCP Integration (8 min)

### D.1 Identify External Systems

For your scenario, list the external systems Claude Code would benefit from accessing:

Examples:
- **Ticket system**: Jira, ADO, Linear — so Claude Code can query real requirements
- **Documentation**: Confluence, Notion — so Claude Code can read architecture decisions
- **Observability**: Datadog, Grafana — so Claude Code can check error rates (read-only)
- **CI/CD**: GitHub Actions, Azure DevOps — so Claude Code can check build status

### D.2 Choose Your MCP Strategy

For each external system, decide:
- **Use existing MCP server** (GitHub, Slack, filesystem)
- **Build custom MCP server** (internal systems)
- **Skip MCP, use hooks** (simple CLI wrapper is sufficient)
- **Skip it entirely** (not worth the complexity)

Document your decisions and rationale.

### D.3 Design the MCP Configuration

Write the `.claude/mcp.json` for your scenario (even if hypothetical — use placeholder URLs and tokens):

```json
{
  "mcpServers": {
    "jira": {
      "command": "node",
      "args": ["/path/to/jira-mcp-server"],
      "env": {
        "JIRA_BASE_URL": "https://your-org.atlassian.net",
        "JIRA_API_TOKEN": "${JIRA_API_TOKEN}"
      }
    }
  }
}
```

---

## Part E: Present Your Design (10 min)

### E.1 Document the Configuration

Write a brief `docs/guides/claude-code-team-guide.md` that explains:
1. How to onboard to the Claude Code configuration
2. The most important CLAUDE.md conventions to know
3. What the hooks do and why
4. MCP servers available and what they're for
5. How to propose changes to the configuration

```
Help me write a one-page team guide for our Claude Code configuration. It should be practical — what does a new developer need to know to use Claude Code effectively in our setup?
```

### E.2 Peer Review (group exercise)

If delivering in a group setting: share your configuration design with another participant. Review each other's designs for:

- **CLAUDE.md**: Is it complete? Is there anything that shouldn't be there?
- **Permissions**: Are the deny rules tight enough? Are allows too broad?
- **Hooks**: Will they create friction or save time?
- **MCP**: Is the complexity justified? Could a simpler approach work?

### E.3 Governance Questions

Discuss with your partner or the group:

1. Who owns the CLAUDE.md files? (Engineering lead? All devs via PR process?)
2. How do you version the Claude Code configuration alongside code?
3. What's the process for a developer to propose a new hook or permission?
4. How do you handle different Claude Code behaviors on different developer machines (user-level vs. project-level settings)?

---

## Reflection: Configuration vs. Agent Design

In the Copilot workshop, the capstone was "build an agent" — a piece of software that encodes a specific workflow. Here the capstone is "design a configuration" — a set of files that make Claude Code behave well for an entire team.

The difference reflects the tools' different models:
- **Copilot agents** are discrete, task-specific tools you invoke by name
- **Claude Code configuration** is ambient — it shapes every interaction without requiring deliberate invocation

Both approaches encode team standards. But Claude Code's approach is more like *training an employee* with good defaults and guardrails, rather than *building a specialized tool* for a specific task.

---

## Success Criteria

- [ ] Root CLAUDE.md written for your scenario — tested with at least one prompt
- [ ] `src/` CLAUDE.md written with stack-specific conventions
- [ ] `.claude/settings.json` with allow/deny rules and at least 2 hooks designed
- [ ] MCP integration decided (even if the decision is "not needed")
- [ ] Team guide written (at least a draft)
- [ ] You can articulate the governance model for maintaining the configuration

---

## Extension Exercise

Write a `scripts/validate-claude-config.sh` script that:
1. Checks that all required CLAUDE.md files exist
2. Validates `.claude/settings.json` is valid JSON
3. Verifies MCP server commands are installed
4. Prints a summary: "Claude Code configuration is valid" or lists issues

This is the kind of script you'd run in CI to ensure the team configuration stays healthy.

---

**Congratulations — you've completed the Claude Code workshop!**

---

## Next Steps

- Apply the CLAUDE.md and hooks patterns to your real project this week
- Explore the [Claude Code documentation](https://docs.anthropic.com/en/docs/claude-code) for features not covered in this workshop
- Consider sharing your team configuration as a reference for other teams at your organization
- Read the [Agent Governance Guide](../guides/agent-governance.md) for how to manage Claude Code configuration at enterprise scale
