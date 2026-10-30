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

MCP servers expose three kinds of capabilities:

**Tools** — actions Claude Code can invoke (named `mcp__<server>__<tool>`):
- `search_issues` (GitHub MCP)
- `list_directory` (Filesystem MCP)
- `run_query` (Database MCP)

**Resources** — readable data you `@`-mention, like files:
- `@github:issue://123`, `@docs:file://api/authentication`

**Prompts** — reusable commands that show up under `/`:
- `/mcp__github__pr_review 456`

---

# MCP vs. Hooks vs. CLAUDE.md vs. Skills

| Mechanism | Best For |
|---|---|
| **CLAUDE.md / rules** | Context, conventions, project knowledge |
| **Skills** | Reusable procedures Claude can follow |
| **Hooks** | Deterministic side effects on tool calls (format, audit, block) |
| **MCP** | New tool capabilities (query GitHub, read Slack, access DB) |

Use MCP when the capability is **external** and benefits from **typed, structured interaction**.

Use a hook when you just need to run a shell command as a side effect.

---

# Adding MCP Servers: `claude mcp add`

```bash
# Remote server over HTTP (recommended for hosted servers)
claude mcp add --transport http <name> <url> [--header "Key: value"]

# Local stdio server — everything after -- is the launch command
claude mcp add <name> [--env KEY=value] -- <command> [args...]

# Manage
claude mcp list | get <name> | remove <name>
```

Inside a session: **`/mcp`** shows status (✔/✘), tools, OAuth sign-in, and reconnect.

SSE transport is **deprecated** — use `--transport http`.

---

# Scopes: Who Gets the Server?

| Scope | Flag | Stored in | Shared? |
|---|---|---|---|
| **Local** (default) | `--scope local` | `~/.claude.json` (per project) | No |
| **Project** | `--scope project` | `.mcp.json` at repo root | **Yes** (git) |
| **User** | `--scope user` | `~/.claude.json` | No — all your projects |

```bash
claude mcp add --scope project filesystem \
  -- npx -y @modelcontextprotocol/server-filesystem .
```

Project servers from `.mcp.json` require a **one-time approval** per developer.

---

# `.mcp.json` — Team-Shared Config

```json
{
  "mcpServers": {
    "filesystem": {
      "type": "stdio",
      "command": "npx",
      "args": ["-y", "@modelcontextprotocol/server-filesystem",
               "${CLAUDE_PROJECT_DIR:-.}"]
    },
    "github": {
      "type": "http",
      "url": "https://api.githubcopilot.com/mcp/",
      "headers": { "Authorization": "Bearer ${GITHUB_MCP_PAT}" }
    }
  }
}
```

`${VAR}` / `${VAR:-default}` expansion keeps secrets and machine paths **out of git**.

---

# The GitHub MCP Server

⚠️ `@modelcontextprotocol/server-github` (npm) is **archived**.
Use GitHub's official server — hosted, or via Docker:

```bash
# Hosted (recommended) — fine-grained PAT in a header
claude mcp add --transport http github https://api.githubcopilot.com/mcp/ \
  --header "Authorization: Bearer $GITHUB_MCP_PAT"

# Or local via Docker (stdio)
claude mcp add github --env GITHUB_PERSONAL_ACCESS_TOKEN=$GITHUB_MCP_PAT \
  -- docker run -i --rm -e GITHUB_PERSONAL_ACCESS_TOKEN ghcr.io/github/github-mcp-server
```

Now Claude Code can search issues, read PRs, and list repositories —
without you copying and pasting anything from the browser.

---

# Tool Search: Many Servers, Small Context

- MCP tool definitions are **deferred by default** — only names load at startup
- Claude looks up full schemas on demand when it needs a tool
- Adding more servers has minimal context cost — check with `/context`

**Permissions still apply:**

```json
"permissions": {
  "allow": ["mcp__github__search_issues"],
  "deny":  ["mcp__github__merge_pull_request"]
}
```

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
check whether there's already code in src-dotnet/ that addresses it.
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
tools/call → { name, arguments } → { content: [...] }
```

Official SDKs (TypeScript, Python, C#, Java, …) at modelcontextprotocol.io.
Scaffold one with the `mcp-server-dev` plugin from `claude-plugins-official`.

---

# MCP Architecture Decision

Before building or configuring MCP, ask:

| Question | If Yes → |
|---|---|
| Is the tool typed and reusable? | MCP server |
| Is it a one-off shell call? | Hook or `Bash(*)` allow |
| Is it just context/docs? | CLAUDE.md or resource |
| Is it a repeatable procedure, not a new capability? | Skill |
| Does it need auth/credentials? | MCP (headers / `--env` / OAuth via `/mcp`) |
| Is the external system shared by the whole team? | `--scope project` → `.mcp.json` |
| Is it personal (e.g., your Jira token)? | `--scope local` or `--scope user` → `~/.claude.json` |

---

# Lab 10 Preview

## What You'll Do

1. Understand how MCP differs from hooks
2. Add the filesystem MCP server with `claude mcp add --scope project` → `.mcp.json`
3. Approve it and inspect tools with `/mcp`
4. Connect GitHub's official MCP server (if you have a token)
5. Use the GitHub MCP in a real workflow: query issues, cross-reference code
6. Explore `@` resources and `/mcp__…` prompts
7. Discuss: when would you build a custom MCP server?

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
