# Pre-Workshop Participant Checklist

Complete everything on this list **before** the workshop session. Each item takes 2–15 minutes. Arriving with these done means you spend the workshop learning, not troubleshooting setup.

If you get stuck, ask in the workshop Slack channel or email the facilitator. Do not wait until the day of the session.

---

## The Checklist

### 1. Fork the repository

- [ ] Go to [github.com/centricconsulting/ai-coding-workshop-claude-code](https://github.com/centricconsulting/ai-coding-workshop-claude-code)
- [ ] Click **Fork** and create a copy under your GitHub account
- [ ] Clone **your fork** (not the original):
  ```bash
  git clone git@github.com:YOUR-USERNAME/ai-coding-workshop-claude-code.git
  ```

> **Why a fork?** Labs 10, 13, and 15 need a GitHub remote you can push to and query via the GitHub MCP server.

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

### 3. Set your ANTHROPIC_API_KEY

- [ ] Get an API key from [console.anthropic.com](https://console.anthropic.com) (create an account if needed)
- [ ] Add to your shell profile (`~/.zshrc` or `~/.bashrc`):
  ```bash
  export ANTHROPIC_API_KEY="sk-ant-..."
  ```
- [ ] Reload your shell:
  ```bash
  source ~/.zshrc
  ```
- [ ] Verify:
  ```bash
  echo $ANTHROPIC_API_KEY   # should print your key (not empty)
  ```

> If using a Dev Container: set the key on your **host** machine before opening the container. The devcontainer forwards it automatically.

---

### 4. Install Claude Code and authenticate

- [ ] Install Claude Code CLI:
  ```bash
  npm install -g @anthropic-ai/claude-code
  ```
  *(Requires Node.js 18+. See [docs/LOCAL_SETUP.md](LOCAL_SETUP.md) for Node.js installation.)*

- [ ] Run the authentication flow:
  ```bash
  claude
  ```
  Follow the browser prompt to log in with your Anthropic account.

- [ ] Verify authentication works:
  ```bash
  claude --print "Say hello"
  ```
  Expected: Claude Code prints a greeting and exits. If it hangs or errors, check your API key.

---

### 5. Verify your stack builds

Pick the stack you'll use during the workshop (.NET or Spring Boot). You don't need both.

**Option A — .NET:**
- [ ] .NET 9 SDK installed: `dotnet --version` prints `9.x.x`
- [ ] Build passes:
  ```bash
  dotnet build
  ```
- [ ] Tests run (they'll be skipped, not failed):
  ```bash
  dotnet test
  ```
  Expected: `Skipped: 11, Failed: 0`

**Option B — Spring Boot:**
- [ ] Java 21 installed: `java -version` prints `21.x.x`
- [ ] Maven installed: `mvn --version` prints `3.9.x`
- [ ] Build and tests pass:
  ```bash
  cd src-springboot
  mvn clean test
  ```
  Expected: `BUILD SUCCESS`

---

### 6. Install the VS Code Claude Code extension

- [ ] Open VS Code
- [ ] Install **Claude Code** by Anthropic:
  - Extensions panel (`Ctrl+Shift+X`) → search "Claude Code"
  - Or: `code --install-extension anthropics.claude-code`
- [ ] Verify: a Claude Code icon appears in the VS Code sidebar

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

## Final Smoke Test

Run this sequence to confirm everything works end-to-end:

```bash
# From the repo root
claude --print "What is this project? Give me a one-sentence summary."
```

Claude Code should:
1. Read one or more files in the repo
2. Print a one-sentence summary
3. Exit cleanly

If this works, you're ready for the workshop.

---

## Troubleshooting

See [docs/LOCAL_SETUP.md](LOCAL_SETUP.md) for detailed installation instructions and a troubleshooting section.

For issues you can't resolve yourself, post in the workshop Slack channel with:
- The command you ran
- The full error output
- Your OS and terminal

---

## What to Bring to the Workshop

- Laptop with all the above installed
- API key accessible (environment variable set and verified)
- Your fork cloned, upstream remote added, personal branch created
- Questions — the first 10 minutes are Q&A from pre-work
