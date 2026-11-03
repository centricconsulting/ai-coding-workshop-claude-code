# Lab 1: First Contact — Claude Code CLI & the Agent Model

**Duration**: 20–25 minutes
**Day**: 1 of 2 (Fundamentals)

**Learning Objectives**:
- Install and authenticate Claude Code
- Understand the agentic model and how it differs from completion-based tools
- Navigate the terminal UI: slash commands, permission prompts, session management
- Ask Claude Code to explore a codebase you've never described

> **Note**: This lab starts with *understanding the tool*, not writing code — because Claude Code's agentic nature means you need to understand what it can do (and what it will ask permission for) before you hand it a task.

---

## Prerequisites

- Claude Code installed (native installer recommended — see [docs/LOCAL_SETUP.md](../LOCAL_SETUP.md), or use a devcontainer)
- A Claude subscription (Pro, Max, Team, Enterprise) **or** a Claude Console API key — your facilitator will tell you which
- Repository **forked** on GitHub and cloned from your fork
- Personal branch created: `git checkout -b workshop/your-name`

> **Why fork?** Later labs (GitHub MCP in Lab 10, CI workflows in Lab 13, `/security-review` which needs an `origin` remote) require a remote you can push to and query. The fork is the foundation — set it up now.

---

## Part A: Installation & Authentication (5 min)

### A.1 Verify Installation

```bash
claude --version
claude doctor
```

You should see a version number, then a read-only diagnostic report of your installation and settings. If you get "command not found":
- **In a devcontainer**: the `postCreateCommand` installs Claude Code with the native installer — wait for container startup to finish, then open a new terminal. To reinstall manually: `curl -fsSL https://claude.ai/install.sh | bash`
- **Locally (macOS/Linux/WSL)**: `curl -fsSL https://claude.ai/install.sh | bash`, then make sure `~/.local/bin` is on your `PATH` and open a new terminal
- **Locally (Windows PowerShell)**: `irm https://claude.ai/install.ps1 | iex`

> npm still works (`npm install -g @anthropic-ai/claude-code`, Node.js 22+), but the native installer is the recommended path and auto-updates.

### A.2 Authenticate

Start Claude Code from the repository root:

```bash
claude
```

What happens next depends on how your cohort authenticates:

| You are using | What you'll see |
|---|---|
| **Claude subscription** | A browser opens to sign in. If it doesn't (common in containers/SSH), copy the URL from the terminal into a browser and paste the code back. |
| **Console API key** (`ANTHROPIC_API_KEY` set) | A one-time prompt asking you to approve the key. |
| **Devcontainer + forwarded token** (`CLAUDE_CODE_OAUTH_TOKEN`) | Nothing — you're already signed in. |

You may also be asked whether you trust the files in this folder — say yes; this is your own fork.

Once you're at the prompt, check which account is active:

```
/status
```

> **Credential precedence**: an `ANTHROPIC_API_KEY` in your environment wins over a subscription login. If `/status` shows the wrong account, exit, run `unset ANTHROPIC_API_KEY`, and start `claude` again — or run `/login` to switch accounts.

Type `/exit` to leave, then continue below.

---

## Part B: First Conversation — Codebase Exploration (8 min)

### B.1 Open Claude Code in the Workshop Repository

Navigate to your fork's clone and start Claude Code:

```bash
cd ai-coding-workshop-claude-code
claude
```

### B.2 Ask Claude Code to Explore the Project

Type this prompt:

```
What is this project? Give me an overview of the architecture, the main domain concepts, and how the .NET and Spring Boot versions relate to each other.
```

**Watch what happens**: Claude Code will read multiple files before responding. You'll see tool calls appear as it explores the `src-dotnet/` directory and builds understanding — `CLAUDE.md` is already in its context because it loads automatically at session start. This is the agentic model in action — it's not guessing from a prompt, it's reading your actual code.

**Expected behavior**: Claude Code should accurately describe:
- TaskManager as a workshop codebase with two parallel implementations
- Clean Architecture with Domain/Application/Infrastructure/API layers
- The starter domain model: a `Task` with title, description, and status (priorities, due dates, and notifications are added during the labs)
- The `.claude/` folder of shipped agents, skills, rules, and settings you'll use on Day 2

### B.3 Follow-Up: Ask About a Specific File

```
Show me the LegacyTaskProcessor and explain what's wrong with it.
```

Claude Code will find and read `LegacyTaskProcessor` in whichever stack it finds first. Notice it gives you a specific, code-grounded answer — not a generic response.

---

## Part C: Slash Commands & the Terminal UI (5 min)

### C.1 Essential Slash Commands

Type `/` at the prompt to see a filterable menu of commands and skills. Try each of these:

| Command | What it does |
|---|---|
| `/help` | Lists available commands |
| `/context` | Shows what's using your context window — system prompt, `CLAUDE.md`, tools, conversation |
| `/model` | Opens the model picker (←/→ adjusts effort for models that support it) |
| `/effort` | Shows or sets reasoning effort: `low`, `medium`, `high`, `xhigh`, `max`, or `auto` |
| `/status` | Shows version, model, account, and active credential |
| `/compact` | Summarizes the conversation to free context. Use proactively when sessions grow long |
| `/clear` | Starts a fresh conversation. Use this when switching to an unrelated task |

Two more you'll use later — **don't run them now**:

| Command | What it does | Lab |
|---|---|---|
| `/init` | Analyzes the codebase and generates (or improves) a `CLAUDE.md` | Lab 2 |
| `/code-review` | Reviews your current changes, a branch, or a PR (`/review` is an alias) | Labs 7–8 |

> `/context` is worth a second look: notice that `CLAUDE.md` is already loaded before you've typed anything. That's the "project memory" you'll write in Lab 2.

### C.2 Permission Modes (Shift+Tab)

Press **Shift+Tab** a few times and watch the mode indicator below the prompt change. Claude Code cycles through its permission modes:

| Mode | Behavior |
|---|---|
| **Auto** | Claude acts without asking; a safety classifier blocks risky actions. The default starting mode in recent versions |
| **Manual** (`default`) | Claude asks before editing files or running commands that aren't pre-approved |
| **Accept edits** (`acceptEdits`) | File edits are auto-approved; other commands still ask |
| **Plan** (`plan`) | Claude researches and proposes a plan without changing anything (Lab 4) |

For Part D, cycle to **Manual** mode so you can see permission prompts. (You can also start a session in Manual mode with `claude --permission-mode manual`.)

### C.3 Keyboard Shortcuts

| Action | Shortcut |
|---|---|
| Submit prompt | `Enter` |
| New line in prompt | `Shift+Enter` (or `\` then `Enter`) |
| Previous prompts | `↑` / `↓` |
| Cycle permission modes | `Shift+Tab` |
| Interrupt mid-response | `Esc` |
| Rewind to an earlier point | `Esc` `Esc` (on an empty prompt) |
| Run a shell command directly | Start the prompt with `!` (e.g. `!git status`) |
| Exit Claude Code | `/exit`, or `Ctrl+C` twice |

### C.4 Understanding the Output Format

When Claude Code takes an action, you see lines like:

```
● Read(src-dotnet/src/TaskManager.Domain/Tasks/Task.cs)
● Bash(dotnet build)
● Write(src-dotnet/src/TaskManager.Application/Services/NotificationService.cs)
```

These are **tool calls** — real actions Claude Code takes on your filesystem or in your terminal. You control which of these are allowed (Day 2, Lab 9).

---

## Part D: Your First Agentic Task (5 min)

### D.1 Ask Claude Code to Run the Tests

```
Run the .NET tests and tell me which ones pass and which ones fail.
```

*(Spring Boot track: ask it to run the Maven tests instead. Expect `BUILD SUCCESS` with about 100 tests, all passing and none skipped; the Spring starter ships with real tests rather than stubs.)*

**Expected result**: the build succeeds and the summary reports **1 passed, 11 skipped, 0 failed**. The passing test is a `/health` check; the skipped tests are stubs that you'll implement during the labs.

**Did you get a permission prompt?** Probably not — and that's worth understanding. This repository ships a `.claude/settings.json` that **pre-approves** `dotnet` and `mvn` commands. That file is the teaching artifact for Lab 9; it also denies dangerous commands like `rm -rf` and `git push --force`. Open it and take a quick look.

### D.2 Trigger a Permission Prompt

Make sure you're in **Manual** mode (Shift+Tab), then ask for something that isn't pre-approved:

```
Create a file called scratch-notes.md containing a three-bullet summary of this project.
```

Claude Code shows the proposed change and asks before writing. The prompt offers numbered choices along the lines of:
- **Yes** — allow this once
- **Yes, and don't ask again** for this kind of action this session
- **No** — and tell Claude what to do differently

Choose **Yes**, then ask Claude to delete `scratch-notes.md` (or delete it yourself with `!rm scratch-notes.md`).

### D.3 Understand What Just Happened

Claude Code:
1. Determined the correct test command from `CLAUDE.md` (or by exploring `src-dotnet/`)
2. Checked the command against your permission rules and mode — `dotnet` was pre-approved, the file write was not
3. Ran the action
4. Parsed the output and gave you a human-readable summary

This is fundamentally different from a completion-based tool — Claude Code **executed code on your machine** and reasoned about the results.

---

## Reflection: The Agentic Model

Before moving to Lab 2, take a moment to understand the mental model shift:

| | Completion-based | Agentic (Claude Code) |
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
- [ ] `dotnet test` (or `mvn clean test`) ran through Claude Code — .NET shows 1 passed, 11 skipped, 0 failed; Spring Boot shows about 100 passing tests and `BUILD SUCCESS`
- [ ] You saw at least one permission prompt and understand why `dotnet` didn't trigger one
- [ ] You can cycle permission modes with Shift+Tab
- [ ] You can use `/help`, `/context`, `/model`, `/effort`, `/compact`, and `/clear`

---

## Extension Exercise

Ask Claude Code to compare the .NET and Spring Boot implementations of the Task entity:

```
Compare how Task is modeled in src-dotnet/ (.NET) vs src-springboot/ (Java). What are the key similarities and differences?
```

Notice that Claude Code will read files from both directories before responding.

---

## Troubleshooting

**"claude: command not found"**
- Native install: check `ls ~/.local/bin/claude`, add `~/.local/bin` to `PATH`, and open a new terminal
- Run `claude doctor` for install diagnostics

**Authentication failed / wrong account**
- Run `/status` to see the active credential
- Run `/login` to sign in again or switch accounts
- If `ANTHROPIC_API_KEY` is set it overrides your subscription — `unset ANTHROPIC_API_KEY` if that's not what you want
- In a devcontainer, see `.devcontainer/README.md` for forwarding `CLAUDE_CODE_OAUTH_TOKEN` or `ANTHROPIC_API_KEY`

**Never see a permission prompt**
- You're probably in **Auto** mode — press Shift+Tab until the indicator shows Manual, or restart with `claude --permission-mode manual`
- `dotnet`/`mvn` commands are pre-approved in `.claude/settings.json`; try a file write instead

**Hook errors mentioning `jq`**
- The shipped `.claude/settings.json` has audit-log hooks that need `jq`. Install it (`brew install jq`, `sudo apt install jq`, or `winget install jqlang.jq`). The hooks don't block anything if `jq` is missing — Lab 9 covers them

**No tool calls visible**
- Tool calls appear inline; you may need to scroll up
- Press `Ctrl+O` to toggle the detailed transcript view

---

**Next**: [Lab 2: CLAUDE.md & Project Memory](lab-02-claude-md-memory.md)
