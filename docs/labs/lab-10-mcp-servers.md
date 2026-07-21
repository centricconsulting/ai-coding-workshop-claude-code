# Lab 10: MCP Servers

**Duration**: 30–35 minutes
**Day**: 2 of 2 (Advanced)

**Learning Objectives**:
- Understand what MCP (Model Context Protocol) is and why it exists
- Configure an MCP server for Claude Code
- Use an MCP-backed tool in a real workflow
- Know when MCP is the right extension mechanism vs. hooks vs. CLAUDE.md

---

## Overview

MCP (Model Context Protocol) is an open standard that lets Claude Code communicate with external tools and data sources through a consistent interface. Instead of writing custom integrations, you configure an MCP server and Claude Code can use its tools natively — the same way it uses Read, Write, and Bash.

**Common MCP servers:**
- **GitHub MCP** — search issues, read PRs, list repositories
- **Filesystem MCP** — structured file operations with additional safety
- **Database MCP** — query databases with read-only access
- **Slack MCP** — read channels, post messages
- **Custom MCP** — anything you build

---

## Prerequisites

- Completed Labs 1–9
- Node.js 18+ (for the `@modelcontextprotocol` packages)
- A GitHub personal access token with `repo` scope (for Part C)

---

## Part A: What is MCP? (8 min)

### A.1 The Problem MCP Solves

Without MCP, extending Claude Code means:
- Writing bash scripts and wrapping them in hooks
- Giving Claude Code broad shell permissions to run custom tools
- Losing structure — Claude Code can't understand typed tool arguments

With MCP:
- Tools are typed and described (Claude Code knows what arguments to pass)
- Servers can provide both **tools** (actions) and **resources** (readable data)
- The protocol is open — any tool can implement it
- Claude Code calls MCP tools exactly like built-in tools

### A.2 Architecture

```
Claude Code
    │
    │ MCP Protocol (JSON-RPC over stdio or HTTP)
    │
    ▼
MCP Server (e.g., GitHub MCP)
    │
    │ API calls / file access / database queries
    │
    ▼
External System (GitHub API / filesystem / database)
```

### A.3 MCP vs Hooks vs CLAUDE.md

| Mechanism | Best For |
|---|---|
| **CLAUDE.md** | Context, conventions, project knowledge |
| **Hooks** | Side effects on tool calls (format, audit, test) |
| **MCP** | Adding new tool capabilities (query GitHub, read Slack, access DB) |

---

## Part B: Configure the Filesystem MCP Server (10 min)

The `@modelcontextprotocol/server-filesystem` MCP server gives Claude Code structured, sandboxed file operations with explicit read/write permissions.

### B.1 Install the Server

```bash
npm install -g @modelcontextprotocol/server-filesystem
```

### B.2 Add MCP Configuration

Claude Code MCP servers are configured in `.mcp.json` at the project root (shared with the team via version control) or in `~/.claude.json` for user-scoped servers.

Create a project-level MCP config:

```
Create .mcp.json that configures the @modelcontextprotocol/server-filesystem MCP server. Allow it to read from the current project directory (use the absolute path to this repo). Show me the configuration before writing.
```

The file should look like:

```json
{
  "mcpServers": {
    "filesystem": {
      "command": "npx",
      "args": [
        "-y",
        "@modelcontextprotocol/server-filesystem",
        "/path/to/ai-code-workshop-claude-code"
      ]
    }
  }
}
```

### B.3 Restart Claude Code and Verify

```bash
/exit
claude
```

```
List the MCP servers available and their tools.
```

Claude Code should list the filesystem MCP tools: `read_file`, `write_file`, `list_directory`, `search_files`, etc.

---

## Part C: Configure the GitHub MCP Server (12 min)

The GitHub MCP server lets Claude Code query your GitHub repositories, issues, and pull requests directly.

### C.1 Set Up Your GitHub Token

```bash
export GITHUB_PERSONAL_ACCESS_TOKEN=your_token_here
```

> If you don't have a token, skip to Part D — the exercises can be observed from a facilitator demo.

### C.2 Configure the GitHub MCP Server

```
Add the GitHub MCP server to .mcp.json. Use the package @modelcontextprotocol/server-github. It needs the GITHUB_PERSONAL_ACCESS_TOKEN environment variable.
```

Restart Claude Code.

### C.3 Use the GitHub MCP in a Workflow

Try this workflow — ask Claude Code to create a work item backlog from a GitHub issue:

```
Using the GitHub MCP tools, search for issues in the centricconsulting/ai-coding-workshop repository labeled "enhancement". For each one, generate a user story in the format: "As a [role], I want to [capability], so that [benefit]."
```

Claude Code will call the GitHub MCP's search tool, receive structured JSON results, and use them to generate the user stories — without you having to copy/paste anything from GitHub.

### C.4 Cross-Reference Code with Issues

A more advanced use case:

```
Using the GitHub MCP, find the most recently closed issues in this repository. For each one, see if there's relevant code in this repo that implements the fix mentioned in the issue comments.
```

This demonstrates how MCP enables Claude Code to reason across multiple data sources simultaneously.

---

## Part D: When to Build a Custom MCP Server (5 min)

The real power of MCP is building your own servers for internal systems.

### D.1 Scenarios That Call for Custom MCP

- **Internal ticket system** (Jira, ADO) — let Claude Code query your real backlog
- **Internal docs / wiki** — let Claude Code read Confluence or Notion pages
- **Deployment systems** — let Claude Code check deploy status (read-only)
- **Test result databases** — let Claude Code query historical test failure data

### D.2 The MCP Server Contract

A minimal MCP server implements:

```typescript
// List available tools
tools/list → { tools: [{ name, description, inputSchema }] }

// Call a tool
tools/call → { toolName, arguments } → { content: [...] }
```

Any language can implement this over stdio or HTTP. Anthropic provides SDKs in TypeScript and Python.

### D.3 Assessment: MCP vs. Direct API Calls

Ask Claude Code:

```
If I wanted Claude Code to be able to query our Azure DevOps backlog, should I use an MCP server, a hook, or just give Claude Code permission to run az devops CLI commands? Walk me through the tradeoffs.
```

This is a design judgment exercise — there's no single right answer, and the reasoning matters.

---

## Success Criteria

- [ ] You can explain what MCP is and how it differs from hooks
- [ ] You configured at least one MCP server (filesystem or GitHub)
- [ ] You used an MCP-backed tool in a real workflow
- [ ] You can articulate when to build a custom MCP server vs. other extension mechanisms

---

## Extension Exercise

Configure the `@modelcontextprotocol/server-github` MCP and use it to generate a sprint plan:

```
Using GitHub MCP, find the 5 most recent open issues in this repository. Estimate a relative effort (S/M/L) for each based on the issue description. Group them into a 2-week sprint that fits within a hypothetical 20-point capacity.
```

---

## Troubleshooting

**"MCP server not found"**

- Verify the package is installed: `npx @modelcontextprotocol/server-filesystem --version`
- Check `.mcp.json` is valid JSON
- Restart Claude Code after adding MCP config

**"Tools not available"**

- MCP servers must be running when Claude Code starts
- Some MCP servers require additional environment setup (tokens, paths)
- Try: `claude --mcp-debug` for verbose MCP diagnostics

**GitHub MCP authentication errors**

- Verify the token has `repo` scope
- Check `echo $GITHUB_PERSONAL_ACCESS_TOKEN` returns a value
- Ensure the env var is set in the same shell as `claude`

---

**Next**: [Lab 11: Skills](lab-11-skills.md)
