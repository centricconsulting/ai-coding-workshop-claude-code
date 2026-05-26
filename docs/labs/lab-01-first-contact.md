# Lab 1: First Contact — Claude Code CLI & the Agent Model

**Duration**: 20–25 minutes
**Part**: 1 of 2 (Fundamentals)

**Learning Objectives**:
- Install and authenticate Claude Code
- Understand the agentic model and how it differs from completion-based tools
- Navigate the terminal UI: slash commands, permission prompts, session management
- Ask Claude Code to explore a codebase you've never described

> **Compare to Copilot**: In the Copilot workshop, Lab 1 started with writing code immediately. Here, Lab 1 starts with *understanding the tool* — because Claude Code's agentic nature means you need to understand what it can do (and what it will ask permission for) before you hand it a task.

---

## Prerequisites

- Claude Code installed: `npm install -g @anthropic-ai/claude-code`
- Authenticated: `claude` → follow login prompt
- Repository **forked** on GitHub and cloned from your fork
- Personal branch created: `git checkout -b workshop/your-name`

> **Why fork?** Later labs (GitHub MCP in Lab 7, CI workflows in Lab 9) require a remote you can push to and query. The fork is the foundation — set it up now.

---

## Part A: Installation & Authentication (5 min)

### A.1 Verify Installation

```bash
claude --version
```

You should see a version number. If you get "command not found":
- **In a devcontainer**: the `postCreateCommand` installs Claude Code — wait for the container startup to finish, or run `npm install -g @anthropic-ai/claude-code` manually
- **Locally**: run `npm install -g @anthropic-ai/claude-code` then restart your terminal

### A.2 Authenticate

**In a devcontainer**: authentication is handled via `ANTHROPIC_API_KEY` forwarded from your host machine. Verify it arrived:

```bash
echo $ANTHROPIC_API_KEY   # Should print your key — not empty
```

If empty, the key wasn't set on your host before the container started. See `.devcontainer/README.md` for how to fix it.

**Locally (no devcontainer)**:

```bash
claude
```

If this is your first run, Claude Code will prompt you to authenticate with your Anthropic account. Follow the browser flow. When authenticated, you'll see the Claude Code REPL prompt:

```
Claude Code — type /help for commands
>
```

Type `/exit` to leave, then continue below.

---

## Part B: First Conversation — Codebase Exploration (8 min)

### B.1 Open Claude Code in the Workshop Repository

Navigate to your fork's clone and start Claude Code:

```bash
cd ai-code-workshop-claude-code
claude
```

### B.2 Ask Claude Code to Explore the Project

Type this prompt:

```
What is this project? Give me an overview of the architecture, the main domain concepts, and how the .NET and Spring Boot versions relate to each other.
```

**Watch what happens**: Claude Code will read multiple files before responding. You'll see tool calls appear as it reads `CLAUDE.md`, explores the `src/` directory, and builds understanding. This is the agentic model in action — it's not guessing from a prompt, it's reading your actual code.

**Expected behavior**: Claude Code should accurately describe:
- TaskManager as a workshop codebase with two parallel implementations
- Clean Architecture with Domain/Application/Infrastructure/API layers
- The domain model (Tasks with priorities, due dates, notifications)

### B.3 Follow-Up: Ask About a Specific File

```
Show me the LegacyTaskProcessor and explain what's wrong with it.
```

Claude Code will find and read `LegacyTaskProcessor` in whichever stack it finds first. Notice it gives you a specific, code-grounded answer — not a generic response.

---

## Part C: Slash Commands & the Terminal UI (5 min)

### C.1 Essential Slash Commands

Try each of these:

```
/help
```

Lists all available slash commands with descriptions.

```
/clear
```

Clears conversation history and starts fresh. Use this when starting a new unrelated task.

```
/compact
```

Compresses conversation history to free up context. Use proactively when sessions grow long.

```
/review
```

Runs a structured code review on your current changes.

### C.2 Keyboard Shortcuts

| Action | Shortcut |
|---|---|
| Submit prompt | `Enter` |
| New line in prompt | `Shift+Enter` |
| Previous prompts | `↑` / `↓` |
| Interrupt mid-response | `Escape` |
| Exit Claude Code | `/exit` or `Ctrl+C` (at empty prompt) |

### C.3 Understanding the Output Format

When Claude Code takes an action, you see:

```
● Reading file: src/TaskManager.Domain/Entities/Task.cs
● Bash: dotnet build
● Writing file: src/TaskManager.Application/Services/NotificationService.cs
```

These are **tool calls** — real actions Claude Code takes on your filesystem or in your terminal. You control which of these are allowed (Part 2, Lab 6).

---

## Part D: Your First Agentic Task (5 min)

### D.1 Ask Claude Code to Run the Tests

```
Run the .NET tests and tell me which ones pass and which ones fail.
```

**Observe the permission prompt**: The first time Claude Code tries to run a bash command (like `dotnet test`), it will ask for permission. This is the **permissions model** — Claude Code won't silently execute commands without your approval.

You can respond:
- **`y`** (yes, once)
- **`a`** (always allow this command pattern)
- **`n`** (deny this time)
- **`d`** (deny and don't ask again for this pattern)

For workshop purposes, choose `a` to allow `dotnet` commands.

### D.2 Understand What Just Happened

Claude Code:
1. Determined the correct build command from `CLAUDE.md` (or by exploring `src/`)
2. Asked permission before running it
3. Ran `dotnet test`
4. Parsed the output and gave you a human-readable summary

This is fundamentally different from Copilot — Claude Code **executed code on your machine** and reasoned about the results.

---

## Reflection: The Agentic Model

Before moving to Lab 2, take a moment to understand the mental model shift:

| | Completion-based (Copilot) | Agentic (Claude Code) |
|---|---|---|
| **What it sees** | Your current file + nearby context | Any file it reads; your whole repo |
| **What it can do** | Suggest code at cursor | Read files, write files, run commands |
| **Who initiates** | You trigger a completion | You describe a task; Claude Code plans and acts |
| **How to guide it** | Specific prompts + `#file` + slash commands | Good CLAUDE.md + clear task scope |
| **Safety model** | Suggestions you accept/reject | Permission prompts + diff review |

The key insight: **Claude Code's output is actions, not suggestions.** You review a diff or a test result, not just a code completion.

---

## Success Criteria

- [ ] `claude --version` shows a version number
- [ ] You authenticated and ran at least one conversation
- [ ] Claude Code accurately described the project architecture from reading the codebase
- [ ] You saw at least one permission prompt and understand what it means
- [ ] You can use `/help`, `/compact`, `/review`, and `/clear`

---

## Extension Exercise

Ask Claude Code to compare the .NET and Spring Boot implementations of the Task entity:

```
Compare how Task is modeled in src/ (.NET) vs src-springboot/ (Java). What are the key similarities and differences?
```

Notice that Claude Code will read files from both directories before responding.

---

## Troubleshooting

**"claude: command not found"**
- Run `npm install -g @anthropic-ai/claude-code` then restart your terminal

**"Authentication failed"**
- Run `claude` and follow the re-authentication flow
- Ensure you have a valid Anthropic account with API access

**"Permission denied" on dotnet/mvn**
- Claude Code's permissions model prevents running commands you haven't approved
- Type `a` to always allow the command, or add it to `.claude/settings.json` (Lab 6)

**No tool calls visible**
- Tool calls appear inline; you may need to scroll up
- Try `/help` to confirm the session is active and see available commands

---

**Next**: [Lab 2: CLAUDE.md & Project Memory](lab-02-claude-md-memory.md)
