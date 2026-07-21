---
marp: true
theme: default
paginate: true
backgroundColor: #fff
---

# Module 10: MCP Servers

## Adding New Tool Capabilities to Claude Code

---

# Where We Are

You can now control what Claude Code is allowed to do (Lab 9).

**Lab 10 is about what Claude Code can *reach*.**

Hooks let Claude Code trigger side effects on tool calls.
MCP lets Claude Code call entirely new tools — ones that don't ship with Claude Code.

---

# The Problem MCP Solves

Without MCP, adding capabilities means:
- Writing bash scripts and wrapping them in hooks
- Giving Claude Code broad shell permissions to run ad-hoc CLI tools
- Losing structure — Claude Code can't reason about typed arguments

With MCP, Claude Code calls external tools the same way it calls `Read` or `Write`:
typed inputs, typed outputs, described parameters.

---

# What MCP Is

**Model Context Protocol** — an open standard for connecting AI models to external tools and data sources.

```
Claude Code
    │
    │  MCP Protocol (JSON-RPC over stdio or HTTP)
    │
    ▼
MCP Server (e.g., GitHub MCP)
    │
    │  API calls / file access / database queries
    │
    ▼
External System
```

Any tool that implements the MCP spec works with Claude Code.

---

# What MCP Servers Provide

MCP servers expose two types of capabilities:

**Tools** — actions Claude Code can invoke:
- `search_issues` (GitHub MCP)
- `write_file` (Filesystem MCP)
- `run_query` (Database MCP)

**Resources** — readable data Claude Code can reference:
- Repository contents
- Documentation pages
- Schema definitions

Claude Code lists available MCP tools at startup and uses them natively.

---

# MCP vs. Hooks vs. CLAUDE.md

| Mechanism | Best For |
|---|---|
| **CLAUDE.md** | Context, conventions, project knowledge |
| **Hooks** | Side effects on tool calls (format, audit, test) |
| **MCP** | New tool capabilities (query GitHub, read Slack, access DB) |

Use MCP when the capability is **external** and benefits from **typed, structured interaction**.

Use a hook when you just need to run a shell command as a side effect.

---

# Configuring MCP Servers

MCP servers are configured in `.mcp.json` at the project root
(shared with the team via version control):

```json
{
  "mcpServers": {
    "filesystem": {
      "command": "npx",
      "args": [
        "-y",
        "@modelcontextprotocol/server-filesystem",
        "/path/to/your/project"
      ]
    }
  }
}
```

Restart Claude Code after changing `.mcp.json` — servers start at launch.

---

# The GitHub MCP Server

Gives Claude Code direct access to GitHub repos, issues, and PRs:

```json
{
  "mcpServers": {
    "github": {
      "command": "npx",
      "args": ["-y", "@modelcontextprotocol/server-github"],
      "env": {
        "GITHUB_PERSONAL_ACCESS_TOKEN": "${GITHUB_PERSONAL_ACCESS_TOKEN}"
      }
    }
  }
}
```

Now Claude Code can search issues, read PRs, and list repositories —
without you copying and pasting anything from the browser.

---

# MCP in a Real Workflow

**Without MCP:**
1. Open GitHub in browser
2. Find relevant issues
3. Copy issue text
4. Paste into Claude Code
5. Ask Claude Code to cross-reference with code

**With GitHub MCP:**
```
Find open issues labeled "enhancement" and for each one,
check whether there's already code in src/ that addresses it.
```

Claude Code does the GitHub query and the code analysis in one task.

---

# When to Build a Custom MCP Server

**Good candidates for custom MCP:**
- Internal ticket system (Jira, ADO) — query your real backlog
- Internal docs / wiki — read Confluence or Notion pages
- Deployment status — check CI/CD state (read-only)
- Test result databases — query historical failure data

**The MCP server contract is minimal:**

```typescript
tools/list → { tools: [{ name, description, inputSchema }] }
tools/call → { toolName, arguments } → { content: [...] }
```

TypeScript and Python SDKs available from Anthropic.

---

# MCP Architecture Decision

Before building or configuring MCP, ask:

| Question | If Yes → |
|---|---|
| Is the tool typed and reusable? | MCP server |
| Is it a one-off shell call? | Hook or `Bash(*)` allow |
| Is it just context/docs? | CLAUDE.md or resource |
| Does it need auth/credentials? | MCP (handles env vars cleanly) |
| Is the external system shared by the whole team? | `.mcp.json` (project-level) |
| Is it personal (e.g., your Jira token)? | `~/.claude.json` (user-level) |

---

# Lab 10 Preview

## What You'll Do

1. Understand how MCP differs from hooks
2. Configure the filesystem MCP server via `.mcp.json`
3. Restart Claude Code and list the available MCP tools
4. Configure the GitHub MCP server (if you have a token)
5. Use the GitHub MCP in a real workflow: query issues, cross-reference code
6. Discuss: when would you build a custom MCP server?

## The Key Insight

MCP extends Claude Code's reach into external systems —
without giving it unconstrained shell access.

---

# Lab 10: MCP Servers

👉 Open [docs/labs/lab-10-mcp-servers.md](../../../labs/lab-10-mcp-servers.md)

**Goal for the next 30 minutes:**
- At least one MCP server configured and verified
- At least one MCP-backed tool used in a real workflow
- Clear on when MCP is the right extension mechanism

**If you don't have a GitHub token:** focus on the filesystem MCP
and observe the GitHub MCP from a facilitator demo.

---

<!-- Navigation -->
[← 09 Permissions & Hooks](00-permissions-and-hooks.md) | **Day 2 Module 10** | [Next: 11 Skills →](02-skills.md)
