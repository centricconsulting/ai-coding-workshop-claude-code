# Local Setup Guide

For participants who cannot use Dev Containers. If you can use the devcontainer, see `.devcontainer/README.md` instead — it handles all of this automatically.

---

## Prerequisites Summary

| Tool | Required Version | Check Command |
|---|---|---|
| Node.js | 18+ | `node --version` |
| Claude Code CLI | latest | `claude --version` |
| .NET SDK | 9.0+ | `dotnet --version` |
| Java JDK | 21+ | `java -version` |
| Maven | 3.9+ | `mvn --version` |
| Git | any recent | `git --version` |

You only need **.NET** or **Java** — not both. Pick the stack you'll use for the labs.

---

## Step 1: Node.js

Claude Code requires Node.js 18 or later.

**macOS (Homebrew):**
```bash
brew install node
```

**macOS/Linux (nvm — recommended):**
```bash
curl -o- https://raw.githubusercontent.com/nvm-sh/nvm/v0.39.7/install.sh | bash
nvm install 20
nvm use 20
```

**Windows:**
Download the LTS installer from [nodejs.org](https://nodejs.org).

**Verify:**
```bash
node --version   # should print v18.x.x or higher
npm --version
```

---

## Step 2: Claude Code CLI

```bash
npm install -g @anthropic-ai/claude-code
```

**Verify:**
```bash
claude --version
```

---

## Step 3: ANTHROPIC_API_KEY

Claude Code requires an Anthropic API key. Get one at [console.anthropic.com](https://console.anthropic.com).

### macOS / Linux

Add to your shell profile (`~/.zshrc`, `~/.bashrc`, or `~/.bash_profile`):

```bash
export ANTHROPIC_API_KEY="sk-ant-..."
```

Then reload:
```bash
source ~/.zshrc   # or source ~/.bashrc
```

### Windows (PowerShell)

```powershell
[System.Environment]::SetEnvironmentVariable("ANTHROPIC_API_KEY", "sk-ant-...", "User")
```

Restart your terminal after setting it.

### Verify

```bash
echo $ANTHROPIC_API_KEY   # macOS/Linux — should print your key
```

```powershell
echo $env:ANTHROPIC_API_KEY   # Windows PowerShell
```

---

## Step 4: Authenticate Claude Code

```bash
claude
```

On first launch, Claude Code will prompt you to authenticate. Follow the browser flow. Once authenticated, the session persists.

**Quick smoke test:**
```bash
claude --print "Say hello"
```

If this prints a greeting and exits, Claude Code is working correctly.

---

## Step 5: .NET 9 SDK (if using the .NET stack)

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
dotnet build
dotnet test
```

Expected: `Build succeeded`, all tests skipped (not failed).

---

## Step 6: Java 21 JDK + Maven (if using the Spring Boot stack)

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

## Step 7: VS Code Setup

### Required Extension

Install the **Claude Code** extension:
- Open VS Code
- Press `Ctrl+Shift+X` (Extensions)
- Search for "Claude Code" by Anthropic
- Install

Or via command line:
```bash
code --install-extension anthropics.claude-code
```

### Recommended Extension (for presentations)

If you want to view the Marp slide decks:
```bash
code --install-extension marp-team.marp-vscode
```

---

## Step 8: Fork and Clone

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
claude --print "Say hello"

# .NET (if using)
dotnet --version
dotnet build
dotnet test

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
- Run `npm install -g @anthropic-ai/claude-code` again
- Verify npm global bin is on your PATH: `npm bin -g`

**`ANTHROPIC_API_KEY` not found / authentication errors**
- Check `echo $ANTHROPIC_API_KEY` returns your key
- Make sure you reloaded your shell after adding it: `source ~/.zshrc`
- On Windows, restart the terminal after setting the env var

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
