# Local Setup Guide

For participants who cannot use Dev Containers. If you can use the devcontainer, see `.devcontainer/README.md` instead — it handles all of this automatically.

---

## Prerequisites Summary

| Tool | Required Version | Check Command |
|---|---|---|
| Claude Code CLI | latest | `claude --version` |
| Claude account | Pro/Max/Team/Enterprise subscription **or** Console API key | `/status` inside Claude Code |
| .NET SDK | 9.0+ | `dotnet --version` |
| Java JDK | 21+ | `java -version` |
| Maven | 3.9+ | `mvn --version` |
| Git | any recent | `git --version` |

You only need **.NET** or **Java** — not both. Pick the stack you'll use for the labs.

Claude Code runs on macOS 13+, Windows 10 1809+ (natively or under WSL), and Ubuntu 20.04+, Debian 10+, or Alpine 3.19+ with 4 GB+ RAM. **Node.js is not required** unless you choose the npm install option.

---

## Step 1: Install Claude Code

The **native installer** is the recommended option on every OS. It installs a self-contained binary to `~/.local/bin/claude` and auto-updates in the background.

**macOS / Linux / WSL:**
```bash
curl -fsSL https://claude.ai/install.sh | bash
```

**Windows PowerShell:**
```powershell
irm https://claude.ai/install.ps1 | iex
```

**Windows CMD:**
```batch
curl -fsSL https://claude.ai/install.cmd -o install.cmd && install.cmd && del install.cmd
```

> On native Windows, [Git for Windows](https://git-scm.com/downloads/win) is recommended (Claude Code uses Git Bash when available, otherwise PowerShell). WSL 2 is also supported — run the Linux command inside your WSL shell.

### Alternatives

| Option | Command | Notes |
|---|---|---|
| Homebrew (macOS/Linux) | `brew install --cask claude-code` | Does **not** auto-update — run `brew upgrade claude-code` |
| WinGet (Windows) | `winget install Anthropic.ClaudeCode` | Does **not** auto-update — run `winget upgrade Anthropic.ClaudeCode` |
| npm | `npm install -g @anthropic-ai/claude-code` | Requires **Node.js 22+**. Never use `sudo npm install -g` |

**Verify:**
```bash
claude --version
claude doctor      # read-only diagnostics for your installation and settings
```

If `claude` isn't found after the native install, add `~/.local/bin` to your `PATH` and open a new terminal:
```bash
echo 'export PATH="$HOME/.local/bin:$PATH"' >> ~/.zshrc   # or ~/.bashrc
```

---

## Step 2: Choose How You'll Authenticate

Claude Code needs a Claude account. Your facilitator will tell you which option your cohort uses.

| Option | Use when | How |
|---|---|---|
| **Claude subscription** (Pro, Max, Team, Enterprise) | You or your org has a claude.ai plan | Run `claude` and sign in through the browser (or `/login` later) |
| **Claude Console API key** | Your org bills usage through the Anthropic Console | Set `ANTHROPIC_API_KEY` in your shell profile |
| **Cloud provider** (Amazon Bedrock, Google Vertex AI, Microsoft Foundry) | Your org routes model access through its cloud | Set the provider env vars per the [authentication docs](https://code.claude.com/docs/en/authentication) |

### Option A — Subscription login

```bash
claude
```

On first launch Claude Code opens a browser to sign in. If no browser opens (SSH, WSL, containers), copy the URL shown in the terminal into a browser and paste the returned code back. You can switch accounts any time with `/login`.

### Option B — Console API key

Create a key at [platform.claude.com](https://platform.claude.com) (formerly console.anthropic.com).

**macOS / Linux** — add to your shell profile (`~/.zshrc`, `~/.bashrc`, or `~/.bash_profile`) and reload:
```bash
export ANTHROPIC_API_KEY="sk-ant-..."
source ~/.zshrc   # or source ~/.bashrc
```

**Windows (PowerShell)** — then restart your terminal:
```powershell
[System.Environment]::SetEnvironmentVariable("ANTHROPIC_API_KEY", "sk-ant-...", "User")
```

The first time you start `claude` with a key set, it asks you to approve the key. When an API key is set it **takes precedence** over a subscription login — `unset ANTHROPIC_API_KEY` to fall back to your subscription.

---

## Step 3: Smoke Test

```bash
claude -p "Say hello"
```

If this prints a greeting and exits, Claude Code is working. Inside an interactive session, `/status` shows which account and credential are active.

> **Using a Dev Container later?** Run `claude setup-token` on your host to generate a one-year subscription token, and export it as `CLAUDE_CODE_OAUTH_TOKEN` — the workshop devcontainers forward it automatically. See `.devcontainer/README.md`.

---

## Step 4: .NET 9 SDK (if using the .NET stack)

**macOS (Homebrew):**
```bash
brew install --cask dotnet-sdk
```

**macOS/Linux (official installer):**

Download from [dot.net/download](https://dotnet.microsoft.com/download/dotnet/9.0) and follow the installer.

**Windows:**

Download and run the .NET 9 SDK installer from [dot.net/download](https://dotnet.microsoft.com/download/dotnet/9.0).

**Verify:**
```bash
dotnet --version   # should print 9.x.x
```

**Verify the project builds:**
```bash
cd /path/to/ai-coding-workshop-claude-code
dotnet build src-dotnet
dotnet test src-dotnet
```

Expected: `Build succeeded`, and the test summary shows **11 skipped, 0 failed** — the skipped tests are stubs you fill in during the labs.

---

## Step 5: Java 21 JDK + Maven (if using the Spring Boot stack)

### Java 21

**macOS (Homebrew):**
```bash
brew install openjdk@21
echo 'export PATH="/opt/homebrew/opt/openjdk@21/bin:$PATH"' >> ~/.zshrc
source ~/.zshrc
```

**macOS/Linux (SDKMAN — recommended):**
```bash
curl -s "https://get.sdkman.io" | bash
sdk install java 21-tem
```

**Windows:**
Download the Eclipse Temurin 21 installer from [adoptium.net](https://adoptium.net).

**Verify:**
```bash
java -version   # should print openjdk version "21.x.x"
```

### Maven

**macOS (Homebrew):**
```bash
brew install maven
```

**Linux:**
```bash
sudo apt install maven   # Debian/Ubuntu
```

**Windows:**
Download from [maven.apache.org](https://maven.apache.org/download.cgi) and add to PATH.

**Verify:**
```bash
mvn --version   # should print Apache Maven 3.9.x
```

**Verify the project builds:**
```bash
cd /path/to/ai-coding-workshop-claude-code/src-springboot
mvn clean test
```

Expected: `BUILD SUCCESS`, all tests pass.

---

## Step 6: VS Code Setup

### Required Extension

Install the **Claude Code** extension:
- Open VS Code
- Press `Ctrl+Shift+X` (Extensions)
- Search for "Claude Code" by Anthropic
- Install (requires VS Code 1.94+)

The extension bundles its own copy of the CLI for the chat panel, but the labs run `claude` in the terminal, so you still need Step 1.

Or via command line:
```bash
code --install-extension anthropic.claude-code
```

### Recommended Extension (for presentations)

If you want to view the Marp slide decks:
```bash
code --install-extension marp-team.marp-vscode
```

---

## Step 7: Fork and Clone

The workshop requires a **fork** (not just a clone) because Labs 10, 13, and 15 need a GitHub remote you control.

1. Go to [github.com/centricconsulting/ai-coding-workshop-claude-code](https://github.com/centricconsulting/ai-coding-workshop-claude-code)
2. Click **Fork** → create under your account
3. Clone your fork:

```bash
git clone git@github.com:YOUR-USERNAME/ai-coding-workshop-claude-code.git
cd ai-coding-workshop-claude-code
```

4. Add the upstream remote:

```bash
git remote add upstream https://github.com/centricconsulting/ai-coding-workshop-claude-code.git
```

5. Create your personal branch:

```bash
git checkout -b workshop/your-name
```

---

## Verification Checklist

Before the workshop, confirm all of these pass:

```bash
# Claude Code
claude --version
claude doctor
claude -p "Say hello"

# .NET (if using)
dotnet --version
dotnet build src-dotnet
dotnet test src-dotnet

# Java (if using)
java -version
mvn --version
cd src-springboot && mvn clean test

# Git
git remote -v   # should show both origin (your fork) and upstream
git branch      # should show workshop/your-name as current
```

If anything fails, see the troubleshooting section below or ask in the workshop Slack channel before the session.

---

## Troubleshooting

**`claude: command not found`**
- Native install: check `ls ~/.local/bin/claude`, add `~/.local/bin` to `PATH`, and open a new terminal
- npm install: check `node --version` is 22+ and that `npm prefix -g`'s `bin` directory is on your PATH
- Run `claude doctor` for diagnostics

**Authentication errors / wrong account**
- Run `/status` inside Claude Code to see which credential is active
- An `ANTHROPIC_API_KEY` in your environment overrides a subscription login — `unset ANTHROPIC_API_KEY` if you meant to use your subscription
- API keys: check `echo $ANTHROPIC_API_KEY` returns your key and that you reloaded your shell (Windows: restart the terminal)
- Subscriptions: run `/login` to sign in again
- Corporate proxies/firewalls: see the [troubleshooting docs](https://code.claude.com/docs/en/troubleshooting)

**`dotnet: command not found`**
- Verify the SDK is installed: check Add/Remove Programs (Windows) or `ls /usr/local/share/dotnet`
- Add to PATH if needed: `export PATH="$PATH:/usr/local/share/dotnet"`

**`mvn: command not found`**
- Verify Maven is installed and on PATH
- SDKMAN users: run `sdk use java 21-tem` before running Maven

**`BUILD FAILURE` on Spring Boot**
- Make sure you're running from `src-springboot/`, not the project root
- Java 21 is required: `java -version` must show 21

**Port already in use when running the API**
- The API defaults to port 8080 (Spring Boot) or 5000 (.NET)
- Kill the existing process or set a different port in `appsettings.json` / `application.properties`
