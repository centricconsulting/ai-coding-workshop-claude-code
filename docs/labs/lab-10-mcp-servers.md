# Lab 10: MCP Servers

**Duration**: 30–35 minutes
**Day**: 2 of 2 (Advanced)

**Learning Objectives**:
- Understand what MCP (Model Context Protocol) is and why it exists
- Add MCP servers with `claude mcp add` and choose the right scope (local / project / user)
- Inspect and authenticate servers with `/mcp`
- Use MCP tools, resources (`@` mentions), and prompts (`/mcp__...` commands) in a real workflow
- Know when MCP is the right extension mechanism vs. hooks vs. CLAUDE.md vs. skills

---

## Overview

MCP (Model Context Protocol) is an open standard that lets Claude Code communicate with external tools and data sources through a consistent interface. Instead of writing custom integrations, you connect an MCP server and Claude Code can use its tools natively — the same way it uses Read, Edit, and Bash.

**Common MCP servers:**
- **GitHub MCP** — GitHub's official server: search issues, read PRs, list repositories
- **Filesystem MCP** — structured file operations limited to directories you allow
- **Database MCP** — query databases with read-only access
- **Slack / Notion / Jira / Sentry** — many vendors host remote MCP servers (browse the [Anthropic Directory](https://claude.ai/directory))
- **Custom MCP** — anything you build

---

## Prerequisites

- Completed Labs 1–9
- Node.js 18+ with `npx` (Part B runs the filesystem server via `npx`; nothing is installed globally)
- For Part C: a GitHub **fine-grained personal access token** with read access to the repositories you want to query (Issues + Pull requests + Contents: read-only is enough for this lab)
- Optional: Docker, if you prefer running the GitHub MCP server locally instead of using GitHub's hosted endpoint

---

## Part A: What is MCP? (8 min)

### A.1 The Problem MCP Solves

Without MCP, extending Claude Code means:
- Writing bash scripts and wrapping them in hooks
- Giving Claude Code broad shell permissions to run custom tools
- Losing structure — Claude Code can't understand typed tool arguments

With MCP:
- Tools are typed and described (Claude Code knows what arguments to pass)
- Servers can provide **tools** (actions), **resources** (readable data you can `@`-mention), and **prompts** (reusable commands)
- The protocol is open — any tool can implement it
- Claude Code calls MCP tools exactly like built-in tools, subject to the same permission rules

### A.2 Architecture

```
Claude Code
    │
    │ MCP Protocol (JSON-RPC over stdio or streamable HTTP)
    │
    ▼
MCP Server (e.g., GitHub MCP)
    │
    │ API calls / file access / database queries
    │
    ▼
External System (GitHub API / filesystem / database)
```

**Transports:**

| Transport | Use for | How to add |
|---|---|---|
| `stdio` | Local process on your machine (`npx`, `uvx`, `docker run`, a binary) | `claude mcp add <name> -- <command> [args...]` |
| `http` | Remote/hosted servers (**recommended** for remote) | `claude mcp add --transport http <name> <url>` |
| `sse` | Legacy remote servers only — **deprecated**, prefer `http` | `claude mcp add --transport sse <name> <url>` |

### A.3 Scopes: Who Gets the Server?

| Scope | Flag | Loads in | Shared with team | Stored in |
|---|---|---|---|---|
| **Local** (default) | `--scope local` | This project only | No | `~/.claude.json` (under this project's path) |
| **Project** | `--scope project` | This project only | **Yes**, via version control | `.mcp.json` at the repo root |
| **User** | `--scope user` | All your projects | No | `~/.claude.json` |

> Personal credentials belong in **local** or **user** scope. Team-shared servers go in **project** scope (`.mcp.json`) — and reference secrets with `${VAR}` expansion rather than hard-coding them.

### A.4 MCP vs Hooks vs CLAUDE.md vs Skills

| Mechanism | Best For |
|---|---|
| **CLAUDE.md / rules** | Context, conventions, project knowledge |
| **Skills** | Reusable procedures and prompts Claude can follow |
| **Hooks** | Deterministic side effects on tool calls (format, audit, block) |
| **MCP** | Adding new tool capabilities against external systems (GitHub, Slack, DB) |

See [docs/guides/customization-decision-guide.md](../guides/customization-decision-guide.md) for the full decision tree.

---

## Part B: Add the Filesystem MCP Server (10 min)

The reference [`@modelcontextprotocol/server-filesystem`](https://github.com/modelcontextprotocol/servers/tree/main/src/filesystem) server gives Claude Code structured file operations restricted to the directories you pass it. It's a good first server because it needs no credentials.

### B.1 Add the Server at Project Scope

There is nothing to `npm install -g` — `npx -y` downloads and runs the package on demand. From the repo root, in your shell (not inside a Claude session):

```bash
claude mcp add --scope project filesystem -- npx -y @modelcontextprotocol/server-filesystem .
```

Everything after `--` is the command Claude Code runs to start the server. The trailing `.` is the directory the server is allowed to access.

### B.2 Inspect the Generated `.mcp.json`

```bash
cat .mcp.json
```

You should see something like:

```json
{
  "mcpServers": {
    "filesystem": {
      "type": "stdio",
      "command": "npx",
      "args": ["-y", "@modelcontextprotocol/server-filesystem", "."],
      "env": {}
    }
  }
}
```

> **Portability tip:** Don't commit absolute paths like `/Users/you/src/...` — teammates won't have them. `.mcp.json` supports environment variable expansion (`${VAR}` and `${VAR:-default}`) in `command`, `args`, `env`, `url`, and `headers`. Claude Code sets `CLAUDE_PROJECT_DIR` for the server process, so a portable form is `"${CLAUDE_PROJECT_DIR:-.}"` (the `:-.` default is required in `.mcp.json`).

### B.3 Verify the Connection

```bash
claude mcp list            # all configured servers and their status
claude mcp get filesystem  # details for one server
```

Start Claude Code. Because `filesystem` comes from a project `.mcp.json`, Claude Code asks you to **approve** it the first time — this protects you from servers someone else committed. Approve it, then:

```
/mcp
```

`/mcp` shows each server, its scope, connection status (✔ / ✘), and its tools. Select `filesystem` to browse tools such as `read_text_file`, `list_directory`, `search_files`, and `directory_tree`.

```
Using the filesystem MCP tools, show me the directory tree of src/ two levels deep.
```

> **Note on tool search:** By default Claude Code *defers* MCP tool definitions — only tool names load at startup, and Claude looks up full schemas on demand. That's why adding many servers doesn't blow up your context window. Run `/context` to see how little space the MCP tools take.

> **Permissions apply to MCP too.** MCP tools are named `mcp__<server>__<tool>` (e.g. `mcp__filesystem__read_text_file`). You can allow or deny them in `.claude/settings.json` just like built-in tools, e.g. `"allow": ["mcp__filesystem__list_directory"]` or `"deny": ["mcp__filesystem__write_file"]`.

---

## Part C: Connect GitHub's Official MCP Server (12 min)

> ⚠️ The old `@modelcontextprotocol/server-github` npm package is **archived and no longer maintained**. Use GitHub's official server ([github/github-mcp-server](https://github.com/github/github-mcp-server)) instead — either GitHub's hosted endpoint (recommended) or its Docker image.

### C.1 Create a Token

Create a [fine-grained personal access token](https://github.com/settings/personal-access-tokens) with read-only access to the repos you want to query. Export it under a name of your own (Claude Code deliberately blanks some well-known credential variables when expanding remote server headers, so use a dedicated name):

```bash
export GITHUB_MCP_PAT=github_pat_xxxxxxxx
```

> If you don't have a token, skip to Part D — the exercises can be observed from a facilitator demo.

### C.2 Option 1 (recommended): GitHub's Hosted Remote Server over HTTP

Add it at **local** scope so your token stays out of version control:

```bash
claude mcp add --transport http github https://api.githubcopilot.com/mcp/ \
  --header "Authorization: Bearer $GITHUB_MCP_PAT"
```

> Want the whole team to get this server? Put it in `.mcp.json` (project scope) with a variable reference so each developer supplies their own token:
>
> ```json
> {
>   "mcpServers": {
>     "github": {
>       "type": "http",
>       "url": "https://api.githubcopilot.com/mcp/",
>       "headers": { "Authorization": "Bearer ${GITHUB_MCP_PAT}" }
>     }
>   }
> }
> ```

### C.2 Option 2: Run GitHub's Server Locally with Docker (stdio)

```bash
claude mcp add github \
  --env GITHUB_PERSONAL_ACCESS_TOKEN=$GITHUB_MCP_PAT \
  -- docker run -i --rm -e GITHUB_PERSONAL_ACCESS_TOKEN ghcr.io/github/github-mcp-server
```

### C.3 Verify

Restart Claude Code (or run `/mcp` and choose **reconnect**). In `/mcp`, the `github` row should show a checkmark. A ✘ with a `401` means the token is wrong or lacks access.

### C.4 Use the GitHub MCP in a Workflow

Ask Claude Code to turn GitHub issues into a backlog:

```
Using the GitHub MCP tools, search for issues in the centricconsulting/ai-coding-workshop repository labeled "enhancement". For each one, generate a user story in the format: "As a [role], I want to [capability], so that [benefit]."
```

Claude Code will call the GitHub MCP's search tool, receive structured JSON results, and use them to generate the user stories — without you copying/pasting anything from GitHub. Notice the permission prompt for `mcp__github__...` tools the first time each one runs.

### C.5 Cross-Reference Code with Issues

```
Using the GitHub MCP, find the most recently closed issues in this repository. For each one, see if there's relevant code in this repo that implements the fix mentioned in the issue comments.
```

This demonstrates how MCP lets Claude Code reason across multiple data sources simultaneously.

### C.6 Resources and Prompts (Optional, 3 min)

MCP servers can expose more than tools:

- **Resources** — type `@` in the prompt; resources from connected servers appear in the autocomplete alongside files, in the form `@server:protocol://resource/path`. Referenced resources are fetched and attached automatically.
- **Prompts** — type `/` and look for entries marked `(MCP)`. MCP prompts run as `/mcp__<server>__<prompt> [args]`.

Try `@` and `/` now and note what (if anything) each of your servers exposes — not every server offers resources or prompts.

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
tools/call → { name, arguments } → { content: [...] }
```

Any language can implement this over stdio or streamable HTTP. Official SDKs exist for TypeScript, Python, C#, Java, and more (see [modelcontextprotocol.io](https://modelcontextprotocol.io/docs/develop/build-server)). Anthropic's official `mcp-server-dev` plugin can scaffold one for you:

```
/plugin install mcp-server-dev@claude-plugins-official
/mcp-server-dev:build-mcp-server
```

> **Tip for server authors:** with tool search on, Claude finds your tools by name and description — write clear server instructions and put key words near the start of each tool description.

### D.3 Assessment: MCP vs. Direct API Calls

Ask Claude Code:

```
If I wanted Claude Code to be able to query our Azure DevOps backlog, should I use an MCP server, a hook, or just give Claude Code permission to run az devops CLI commands? Walk me through the tradeoffs.
```

This is a design judgment exercise — there's no single right answer, and the reasoning matters.

---

## Success Criteria

- [ ] You can explain what MCP is and how it differs from hooks and skills
- [ ] You added at least one MCP server with `claude mcp add` and verified it in `/mcp`
- [ ] You can explain the difference between local, project, and user scope
- [ ] You used an MCP-backed tool in a real workflow
- [ ] You can articulate when to build a custom MCP server vs. other extension mechanisms

---

## Extension Exercise

With the GitHub MCP connected, generate a sprint plan:

```
Using GitHub MCP, find the 5 most recent open issues in this repository. Estimate a relative effort (S/M/L) for each based on the issue description. Group them into a 2-week sprint that fits within a hypothetical 20-point capacity.
```

Then lock it down: add a deny rule to `.claude/settings.local.json` so the GitHub MCP can never write (for example, deny `mcp__github__create_issue` and `mcp__github__merge_pull_request`), and confirm in `/permissions`.

---

## Clean Up (Optional)

```bash
claude mcp remove filesystem --scope project   # removes it from .mcp.json
claude mcp remove github                         # local scope
```

Don't commit an `.mcp.json` you created only for this lab unless your team wants it.

---

## Troubleshooting

**Server shows ✘ or "failed" in `/mcp` / `claude mcp list`**

- Run the server command yourself (e.g. `npx -y @modelcontextprotocol/server-filesystem .`) to see its error output
- Check `.mcp.json` is valid JSON (`cat .mcp.json | jq .`)
- Use `/mcp` → **reconnect**, or restart Claude Code
- Start with debug logging filtered to MCP: `claude --debug=mcp`

**Project server never loads**

- Servers from `.mcp.json` need one-time approval. If you declined, run `claude mcp reset-project-choices` and restart

**"Missing environment variable" warning**

- A `${VAR}` in the config isn't set in the shell that launched `claude`. Export it, or add a `${VAR:-default}` fallback

**GitHub MCP authentication errors (401/403)**

- Verify the token is valid, not expired, and has access to the target repo
- Check `echo $GITHUB_MCP_PAT` returns a value in the same shell as `claude`
- If you used `.mcp.json` with `${...}` in the header, make sure the variable name is your own (Claude Code reads some credential variable names as empty for remote servers)

---

**Next**: [Lab 11: Skills](lab-11-skills.md)
