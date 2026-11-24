# Pre-Workshop Participant Checklist

Complete everything on this list **before** the workshop session. Each item takes 2–15 minutes. Arriving with these done means you spend the workshop learning, not troubleshooting setup.

If you get stuck, contact your facilitator. Do not wait until the day of the session.

---

> **Two pre-workshop sessions:** session 1 is a primer for people new to the terminal, Git, and VS Code ([Lab 0](labs/lab-00-non-technical-crash-course.md)). Session 2 is for everyone: we walk through the [Local Setup Guide](LOCAL_SETUP.md) (or the [devcontainer guide](../.devcontainer/README.md)), and this checklist is how you confirm you're ready. If you attended session 1, items 1, 2, and 7 (fork, upstream remote, personal branch) are already done: just confirm with `git remote -v` and `git branch`. Unfamiliar word? See the [terminology glossary](terminology.md).

## The Checklist

### 1. Fork the repository

- [ ] Go to [github.com/centricconsulting/ai-coding-workshop-claude-code](https://github.com/centricconsulting/ai-coding-workshop-claude-code)
- [ ] Click **Fork** and create a copy under your GitHub account
- [ ] Clone **your fork** (not the original):
  ```bash
  git clone git@github.com:YOUR-USERNAME/ai-coding-workshop-claude-code.git
  ```

> **Why a fork?** Lab 7 (PRs), Lab 8 (`/security-review` needs an `origin` remote), Lab 10 Part C (optional GitHub MCP server), Lab 13 (CI workflows) and Lab 15 (capstone) all need a GitHub remote you can push to and query.

---

### 2. Add the upstream remote

- [ ] In your cloned repo:
  ```bash
  git remote add upstream https://github.com/centricconsulting/ai-coding-workshop-claude-code.git
  ```
- [ ] Verify:
  ```bash
  git remote -v
  ```
  You should see two remotes: `origin` (your fork) and `upstream` (the original).

---

### 3. Install Claude Code

- [ ] Run the **native installer** (recommended — auto-updates, no Node.js needed):
  ```bash
  # macOS / Linux / WSL
  curl -fsSL https://claude.ai/install.sh | bash
  ```
  ```powershell
  # Windows PowerShell
  irm https://claude.ai/install.ps1 | iex
  ```
  *(Alternatives: `brew install --cask claude-code`, `winget install Anthropic.ClaudeCode`, or `npm install -g @anthropic-ai/claude-code` with Node.js 22+. See [docs/LOCAL_SETUP.md](LOCAL_SETUP.md).)*
- [ ] Verify the install:
  ```bash
  claude --version
  claude doctor
  ```
  If `claude` isn't found, add `~/.local/bin` to your `PATH` and open a new terminal.

> **Using a Dev Container?** Skip this item — the container installs Claude Code for you. Read `.devcontainer/README.md` for the authentication options instead.

---

### 4. Authenticate

Your facilitator will tell you which option your cohort uses.

- [ ] **Option A — Claude subscription (Pro, Max, Team, Enterprise):** run `claude` and follow the browser sign-in. If no browser opens, copy the URL from the terminal into a browser and paste the code back.
- [ ] **Option B — Console API key:** create a key at [platform.claude.com](https://platform.claude.com), add it to your shell profile, and reload your shell:
  ```bash
  export ANTHROPIC_API_KEY="sk-ant-..."
  ```
  Approve the key the first time `claude` asks.
- [ ] Verify authentication works:
  ```bash
  claude -p "Say hello"
  ```
  Expected: Claude Code prints a greeting and exits. Inside an interactive session, `/status` shows which account is active.

> **Dev Container users:** either sign in inside the container with `/login`, or set `CLAUDE_CODE_OAUTH_TOKEN` (from `claude setup-token`) or `ANTHROPIC_API_KEY` on your **host** before opening the container — it's forwarded automatically.

---

### 5. Verify your stack builds

Pick the stack you'll use during the workshop (.NET or Spring Boot). You don't need both.

**Option A — .NET:**
- [ ] .NET 9 SDK installed: `dotnet --version` prints `9.x.x`
- [ ] Build passes:
  ```bash
  dotnet build src-dotnet
  ```
- [ ] Tests run (they'll be skipped, not failed):
  ```bash
  dotnet test src-dotnet
  ```
  Expected: 1 passed, 11 skipped, 0 failed (the skipped tests are stubs you implement in the labs)

**Option B — Spring Boot:**
- [ ] Java 21 installed: `java -version` prints `21.x.x`
- [ ] Maven installed: `mvn --version` prints `3.9.x`
- [ ] Build and tests pass:
  ```bash
  cd src-springboot
  mvn clean test
  ```
  Expected: `BUILD SUCCESS` with about 100 tests, all passing

---

### 6. Install the VS Code Claude Code extension

- [ ] Open VS Code
- [ ] Install **Claude Code** by Anthropic:
  - Extensions panel (`Ctrl+Shift+X`) → search "Claude Code"
  - Or: `code --install-extension anthropic.claude-code`
- [ ] Verify: a Claude Code (spark) icon appears in the editor toolbar or Activity Bar (requires VS Code 1.94+)

> Optional but recommended: install [Marp for VS Code](https://marketplace.visualstudio.com/items?itemName=marp-team.marp-vscode) to view workshop slide decks.

---

### 7. Create your personal branch

- [ ] In your cloned repo:
  ```bash
  git checkout -b workshop/your-name
  ```
  Replace `your-name` with your first name or GitHub username.
- [ ] Verify:
  ```bash
  git branch   # should show workshop/your-name with an asterisk
  ```

---

### 8. Node.js and jq (Day 2 labs)

Skip this item if you're using a Dev Container — both are preinstalled.

- [ ] Node.js 18+ with `npx` (Lab 10): `node --version`
- [ ] `jq` (Labs 9 and 13): `jq --version` (macOS: `brew install jq`; Windows: `winget install jqlang.jq`; Debian/Ubuntu: `sudo apt install jq`)

---

### 9. Day 2 preparation

Day 2 labs reach outside your laptop, so set these up now rather than on the day.

- [ ] **Confirm your fork has a default branch Claude Code can compare against** (Lab 8 `/security-review`):
  ```bash
  git rev-parse --abbrev-ref origin/HEAD   # should print origin/main
  ```
  If it fails, run `git fetch origin && git remote set-head origin --auto`.
- [ ] **Optional — GitHub only: create a fine-grained personal access token** (Lab 10 Part C, GitHub MCP server). Skip this if your team uses another git provider or you'd rather watch the demo. At [github.com/settings/personal-access-tokens](https://github.com/settings/personal-access-tokens), choose **Only select repositories → your fork**, and grant read-only access to **Issues**, **Pull requests**, and **Contents**. Copy it somewhere safe; don't commit it. If you skip this, you'll watch the facilitator demo that part.
- [ ] **Windows only:** confirm **Git for Windows** (which includes Git Bash) is installed. Lab 9's hook scripts run in Git Bash.
- [ ] **Optional:** Python 3 (`python3 --version`) for the `security-guidance` plugin in Lab 8 Part F, and network access to GitHub for installing plugins.

---

### 10. (Optional) GitHub CLI

Only needed for the optional PR steps in Lab 7, and only on GitHub. The devcontainers already include it.

- [ ] Install `gh` (macOS: `brew install gh`; Windows: `winget install --id GitHub.cli`; Linux: [cli.github.com](https://cli.github.com))
- [ ] Sign in: `gh auth login`
- [ ] Verify: `gh auth status`

---

## Final Smoke Test

Run this sequence to confirm everything works end-to-end:

```bash
# From the repo root
claude -p "What is this project? Give me a one-sentence summary."
```

Claude Code should:
1. Read one or more files in the repo
2. Print a one-sentence summary
3. Exit cleanly

If this works, you're ready for the workshop.

---

## Troubleshooting

See [docs/LOCAL_SETUP.md](LOCAL_SETUP.md) for detailed installation instructions and a troubleshooting section.

For issues you can't resolve yourself, contact your facilitator with:
- The command you ran
- The full error output
- Your OS and terminal

---

## What to Bring to the Workshop

- Laptop with all the above installed
- Claude Code authenticated (subscription login or API key verified with `claude -p "Say hello"`)
- Your fork cloned, upstream remote added, personal branch created
- Questions — the first 10 minutes are Q&A from pre-work
