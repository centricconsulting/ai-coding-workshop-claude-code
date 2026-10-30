# DevContainer Selection Guide

This repository contains **3 devcontainer configurations** to support different workshop roles and technology stacks.

---

## ⚠️ Before You Open the Container: Decide How Claude Code Will Authenticate

Claude Code supports two first-class sign-in paths (plus cloud providers). Your facilitator will tell you which one your cohort uses.

| Path | Who it's for | How it reaches the container |
|---|---|---|
| **Claude subscription** (Pro, Max, Team, Enterprise) | Most participants | Run `claude` inside the container and use `/login`, **or** forward a long-lived token (`CLAUDE_CODE_OAUTH_TOKEN`) from your host |
| **Claude Console API key** (pay-as-you-go) | Teams billed through the Anthropic Console | Forward `ANTHROPIC_API_KEY` from your host |
| Amazon Bedrock / Google Vertex AI / Microsoft Foundry | Organizations routing through a cloud provider | Pass the provider's env vars (see the [authentication docs](https://code.claude.com/docs/en/authentication)) |

Every container forwards **`ANTHROPIC_API_KEY`** and **`CLAUDE_CODE_OAUTH_TOKEN`** from your host via `remoteEnv`. Set at most one of them **on your host before opening the devcontainer**.

### Option A — Sign in inside the container (simplest)

Don't set anything on the host. After the container builds, open a terminal and run `claude`, then follow the login prompt. If no browser opens (common in containers), copy the login URL shown in the terminal into your host browser, sign in, and paste the code back into the terminal.

> The container's home directory is discarded on rebuild, so you'll need to sign in again after a rebuild. Options B and C avoid that.

### Option B — Forward a subscription token

On your **host** (where Claude Code is already installed and you're logged in to a subscription):

```bash
claude setup-token          # opens a browser, then prints a one-year OAuth token
```

Add the token to your shell profile (`~/.zshrc`, `~/.bashrc`, etc.):
```bash
export CLAUDE_CODE_OAUTH_TOKEN=<token-from-setup-token>
```

### Option C — Forward a Console API key

Create a key at [platform.claude.com](https://platform.claude.com) (formerly console.anthropic.com) and add it to your host shell profile:
```bash
export ANTHROPIC_API_KEY=sk-ant-...
```

**Windows (PowerShell)** — persist either variable for your user, then restart VS Code:
```powershell
[System.Environment]::SetEnvironmentVariable("CLAUDE_CODE_OAUTH_TOKEN", "<token>", "User")
# or
[System.Environment]::SetEnvironmentVariable("ANTHROPIC_API_KEY", "sk-ant-...", "User")
```

### Verify on the host before opening the container (Options B/C)
```bash
echo ${CLAUDE_CODE_OAUTH_TOKEN:+token set} ${ANTHROPIC_API_KEY:+api key set}
```

> If both an API key and a subscription login are present, Claude Code prefers the API key once you approve it. Run `/status` inside Claude Code to see which credential is active.

---

## 🎯 Which Container Should I Use?

| Role | Container |
|---|---|
| Workshop participant — .NET track | `.devcontainer/dotnet-participant/` |
| Workshop participant — Spring Boot track | `.devcontainer/springboot-participant/` |
| Facilitator or content maintainer | `.devcontainer/maintainer/` |

---

## Container Details

### .NET Participant

**Location:** `.devcontainer/dotnet-participant/`

**Includes:**
- .NET 9 SDK
- Claude Code CLI (native installer → `~/.local/bin/claude`, auto-updates)
- Claude Code VS Code extension
- C# Dev Kit
- GitHub CLI

**Builds on startup:** `src-dotnet/TaskManager.sln`

---

### Spring Boot Participant

**Location:** `.devcontainer/springboot-participant/`

**Includes:**
- Java 21 JDK + Maven 3.9 + Gradle
- Claude Code CLI (native installer → `~/.local/bin/claude`, auto-updates)
- Claude Code VS Code extension
- Extension Pack for Java, Spring Boot Dashboard
- GitHub CLI

**Builds on startup:** `src-springboot/`

---

### Maintainer (Full Toolchain)

**Location:** `.devcontainer/maintainer/`

**Includes:**
- .NET 9 SDK + Java 21 JDK + Maven + Gradle
- Claude Code CLI (native installer) + Node.js LTS + Marp CLI
- Claude Code VS Code extension
- All .NET and Java extensions
- GitHub CLI

**Builds on startup:** Both `src-dotnet/TaskManager.sln` and `src-springboot/`

---

## 📋 Container Comparison

| Feature | .NET Participant | Spring Boot Participant | Maintainer |
|---|---|---|---|
| .NET 9 SDK | ✅ | ❌ | ✅ |
| Java 21 + Maven | ❌ | ✅ | ✅ |
| Claude Code CLI | ✅ | ✅ | ✅ |
| Node.js (needed for Lab 10's `npx` MCP server) | ✅ | ✅ | ✅ |
| Marp CLI | ❌ | ❌ | ✅ |
| Build time | ~1–2 min | ~2–3 min | ~3–5 min |

---

## 🚀 How to Open

1. Decide on your authentication option (see above) and, for Options B/C, set the variable on your host
2. Open the repository in VS Code
3. When prompted, select **"Reopen in Container"**
4. Choose the configuration for your role
5. Wait for the container to build (~1–5 min depending on container)

### Switching containers

`F1` → **"Dev Containers: Rebuild and Reopen in Container"** → select new configuration.

---

## Verifying Claude Code Inside the Container

Once the container is running, open a terminal and verify:

```bash
claude --version            # Should print a version number
claude doctor               # Read-only install/settings diagnostics
claude -p "Say hello"       # Should get a response (prompts for login first if you chose Option A)
```

If `claude -p` reports it isn't logged in, either run `claude` and `/login` (Option A), or set `CLAUDE_CODE_OAUTH_TOKEN` / `ANTHROPIC_API_KEY` on the host and rebuild (Options B/C).

### How Claude Code is installed

Each `postCreateCommand` runs Anthropic's **native installer** (`curl -fsSL https://claude.ai/install.sh | bash`) as the `vscode` user. It installs to `~/.local/bin/claude`, auto-updates in the background, and doesn't need Node.js. `remoteEnv` puts `/home/vscode/.local/bin` on `PATH`. The Claude Code VS Code extension (`anthropic.claude-code`) is installed via `customizations.vscode.extensions`.

> Anthropic also publishes a [Claude Code Dev Container Feature](https://code.claude.com/docs/en/devcontainer) (`ghcr.io/anthropics/devcontainer-features/claude-code:1.0`). It's a good choice for your own projects; this workshop uses the native installer so Claude Code can auto-update without a Node.js toolchain. The same page covers persisting `~/.claude` across rebuilds with a named volume and `CLAUDE_CONFIG_DIR`.

### Feature lock files

All three containers pin their Dev Container Features in `devcontainer-lock.json`. Stale pins can break builds (an old `github-cli` feature pin failed against GitHub CLI's rotated apt signing key). If you change a `features` block, or periodically, regenerate the matching lock file:

```bash
devcontainer upgrade --workspace-folder . --config .devcontainer/<name>/devcontainer.json
```

`jq` is preinstalled in all three images — the audit-log hooks in `.claude/settings.json` (Lab 9) need it.

---

## Troubleshooting

**`ANTHROPIC_API_KEY` / `CLAUDE_CODE_OAUTH_TOKEN` is empty inside the container**
- Set the variable on your host machine, restart VS Code, and rebuild the container
- `remoteEnv` only forwards variables that exist in VS Code's environment when the container starts

**Container won't build**
- Ensure Docker Desktop is running
- Check available disk space (need 5+ GB free)
- Try: `F1` → "Dev Containers: Rebuild Container"
- Clear Docker cache: `docker system prune -a`

**`claude: command not found`**
- The `postCreateCommand` installs Claude Code — wait for it to finish and check its output in the VS Code terminal
- Check the binary exists: `ls ~/.local/bin/claude`
- Re-run the installer manually: `curl -fsSL https://claude.ai/install.sh | bash`, then `export PATH="$HOME/.local/bin:$PATH"`
- Run `claude doctor` for install diagnostics

**`npx: command not found` in Lab 10**
- All three containers include Node.js LTS via the `node` feature. If you're on an older container build, rebuild it (**Dev Containers: Rebuild Container**)

**Login opens no browser / `localhost` connection error**
- Copy the login URL shown above the paste prompt into your host browser, sign in, and paste the code back into the terminal

**Authentication error despite a credential being set**
- Run `/status` inside Claude Code to see which credential is active
- An API key takes precedence over a subscription login; `unset ANTHROPIC_API_KEY` to fall back to your subscription
- API keys: verify the key at [platform.claude.com](https://platform.claude.com) and that it starts with `sk-ant-`
- Subscription tokens: regenerate with `claude setup-token` on the host

---

## Related Documentation

- [Workshop README](../README.md) — setup and getting started
- [Lab 1: First Contact](../docs/labs/lab-01-first-contact.md) — first steps with Claude Code
- [Claude Code: Development containers](https://code.claude.com/docs/en/devcontainer)
- [Claude Code: Authentication](https://code.claude.com/docs/en/authentication)
