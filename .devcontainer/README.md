# DevContainer Selection Guide

This repository contains **3 devcontainer configurations** to support different workshop roles and technology stacks.

---

## ⚠️ Before You Open the Container: Set Your API Key

Claude Code authenticates using your `ANTHROPIC_API_KEY`. The containers forward this variable from your host machine — **you must set it on your host before opening the devcontainer**, or Claude Code won't be able to connect.

### Set the key on your host machine

**macOS / Linux** — add to your shell profile (`~/.zshrc`, `~/.bashrc`, etc.):
```bash
export ANTHROPIC_API_KEY=sk-ant-...
```
Then reload: `source ~/.zshrc` (or open a new terminal).

**Windows (PowerShell)** — add to your PowerShell profile:
```powershell
$env:ANTHROPIC_API_KEY = "sk-ant-..."
```

> Get your API key from [console.anthropic.com](https://console.anthropic.com) under **API Keys**.

### Verify before opening the container
```bash
echo $ANTHROPIC_API_KEY   # Should print your key (not empty)
```

Once the key is set on your host, the devcontainer picks it up automatically via `remoteEnv`. You won't need to set it again inside the container.

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
- Node.js 20 + Claude Code CLI (`@anthropic-ai/claude-code`)
- Claude Code VS Code extension
- C# Dev Kit
- GitHub CLI

**Builds on startup:** `TaskManager.sln`

---

### Spring Boot Participant

**Location:** `.devcontainer/springboot-participant/`

**Includes:**
- Java 21 JDK + Maven 3.9 + Gradle
- Node.js 20 + Claude Code CLI (`@anthropic-ai/claude-code`)
- Claude Code VS Code extension
- Extension Pack for Java, Spring Boot Dashboard
- GitHub CLI

**Builds on startup:** `src-springboot/`

---

### Maintainer (Full Toolchain)

**Location:** `.devcontainer/maintainer/`

**Includes:**
- .NET 9 SDK + Java 21 JDK + Maven + Gradle
- Node.js 20 + Claude Code CLI + Marp CLI
- Claude Code VS Code extension
- All .NET and Java extensions
- GitHub CLI

**Builds on startup:** Both `TaskManager.sln` and `src-springboot/`

---

## 📋 Container Comparison

| Feature | .NET Participant | Spring Boot Participant | Maintainer |
|---|---|---|---|
| .NET 9 SDK | ✅ | ❌ | ✅ |
| Java 21 + Maven | ❌ | ✅ | ✅ |
| Claude Code CLI | ✅ | ✅ | ✅ |
| Marp CLI | ❌ | ❌ | ✅ |
| Build time | ~1–2 min | ~2–3 min | ~3–5 min |

---

## 🚀 How to Open

1. Ensure `ANTHROPIC_API_KEY` is set on your host (see above)
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
claude --version          # Should print a version number
echo $ANTHROPIC_API_KEY   # Should print your key
claude --print "Say hello"  # Should get a response from Claude
```

If `ANTHROPIC_API_KEY` is empty inside the container, it wasn't set on the host before the container started. Set it on the host and rebuild the container.

---

## Troubleshooting

**`ANTHROPIC_API_KEY` is empty inside the container**
- Set the variable on your host machine and rebuild the container
- `remoteEnv` only forwards variables that exist at container start time

**Container won't build**
- Ensure Docker Desktop is running
- Check available disk space (need 5+ GB free)
- Try: `F1` → "Dev Containers: Rebuild Container"
- Clear Docker cache: `docker system prune -a`

**`claude: command not found`**
- The `postCreateCommand` installs Claude Code via npm — wait for it to finish
- Check the VS Code terminal output during container startup for npm errors
- Manually install: `npm install -g @anthropic-ai/claude-code`

**Claude Code authentication error despite key being set**
- Verify the key is valid at [console.anthropic.com](https://console.anthropic.com)
- Check the key starts with `sk-ant-`
- Try re-exporting on the host and rebuilding the container

---

## Related Documentation

- [Workshop README](../README.md) — setup and getting started
- [Lab 1: First Contact](../docs/labs/lab-01-first-contact.md) — first steps with Claude Code
