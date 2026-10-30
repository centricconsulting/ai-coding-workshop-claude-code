# Lab 13: Headless & Automation Mode

**Duration**: 25–30 minutes
**Day**: 2 of 2 (Advanced)

**Learning Objectives**:
- Run Claude Code non-interactively with `-p` / `--print`
- Get machine-readable output with `--output-format json` / `stream-json`
- Control headless runs with `--allowedTools`, `--permission-mode`, and `--max-turns`
- Use `--continue` and `--resume` for session persistence
- Pipe data into and out of Claude Code in shell pipelines
- Integrate Claude Code into CI with the official GitHub Action (`anthropics/claude-code-action@v1`)
- Know the safety implications of headless mode

---

## Overview

Everything you've done in the workshop has been interactive — a human in the loop. But Claude Code also runs headlessly: accepting a prompt via CLI flags, doing the work, and exiting. This opens up automation workflows that would otherwise require a human at a terminal.

Programmatic Claude Code is part of the **Claude Agent SDK** (formerly the "Claude Code SDK"). The SDK is available three ways: the `claude -p` CLI you'll use in this lab, plus Python and TypeScript packages for building your own agents with the same tools, permissions, hooks, and sessions.

---

## Prerequisites

- Completed Labs 1–12
- Comfortable with basic shell scripting and pipes
- `jq` installed (for parsing JSON output)
- Authenticated Claude Code. `claude -p` uses the same login as your interactive sessions (Claude subscription or `ANTHROPIC_API_KEY`). The only exception is `--bare` mode, which requires `ANTHROPIC_API_KEY` (or a cloud provider).

---

## Part A: The `-p` Flag (8 min)

`-p` (long form `--print`) runs Claude Code non-interactively: it takes a prompt, does the work, prints the result, and exits with code 0 on success or non-zero on failure.

### A.1 Basic Usage

```bash
claude -p "What are the top 3 issues in src-dotnet/src/TaskManager.Infrastructure/Legacy/LegacyTaskProcessor.cs?"
```

No interactive session opens. Claude Code reads the file, analyzes it, prints the result to stdout, and exits.

> Headless runs load the same project configuration as an interactive session — CLAUDE.md, `.claude/settings.json` permissions and hooks, skills, subagents, and `.mcp.json` servers. There is **no one to answer permission prompts**, so anything that would prompt is denied unless you allow it up front (Part B).

### A.2 Piping In and Out

`claude -p` reads stdin and writes stdout, so it composes like any other Unix tool:

```bash
# Pipe data IN — Claude doesn't need Bash permission to read it
git log --oneline -20 | claude -p "Summarize the themes of these recent commits in 3 bullets."

# Pipe output OUT to a file
claude -p "List all public interfaces in the src-dotnet/ directory." > interfaces.txt

# Use the output in a script
ISSUES=$(claude -p "Are there any TODO comments in the src-dotnet/ directory? List them, or print NONE.")
if [ "$ISSUES" != "NONE" ]; then
  echo "Found TODOs: $ISSUES"
fi
```

### A.3 Structured Output

Use `--output-format` to control the response shape:

| Format | What you get |
|---|---|
| `text` (default) | Plain text — the final answer only |
| `json` | One JSON object: `result`, `session_id`, `total_cost_usd`, usage, and more |
| `stream-json` | Newline-delimited JSON events as they happen (for progress UIs and long runs; add `--verbose`) |

```bash
# Full JSON envelope
claude -p "How many test projects are in src-dotnet/tests/?" --output-format json | jq .

# Just the answer
claude -p "How many test projects are in src-dotnet/tests/?" --output-format json | jq -r '.result'

# What did that cost?
claude -p "Summarize CLAUDE.md in one sentence." --output-format json | jq '.total_cost_usd'
```

> Want output guaranteed to match a shape? Add `--json-schema '<schema>'` with `--output-format json` and read the `structured_output` field. Asking for "JSON" in the prompt alone works, but isn't validated.

---

## Part B: Controlling a Headless Run (6 min)

Headless runs can't ask "May I?", so you decide permissions **before** the run starts.

| Flag | Purpose | Example |
|---|---|---|
| `--allowedTools` | Pre-approve specific tools/commands (permission rule syntax) | `--allowedTools "Read,Bash(dotnet test *)"` |
| `--disallowedTools` | Deny or remove tools for this run | `--disallowedTools "Edit,Write"` |
| `--permission-mode` | Baseline mode: `default`, `acceptEdits`, `plan`, `auto`, `dontAsk`, `bypassPermissions` | `--permission-mode dontAsk` |
| `--max-turns` | Cap the number of agentic turns (exits with an error when reached) | `--max-turns 5` |
| `--max-budget-usd` | Stop once estimated spend reaches a limit | `--max-budget-usd 0.50` |
| `--append-system-prompt` | Add instructions for this run only | `--append-system-prompt "Be terse."` |
| `--bare` | Skip auto-loading hooks, skills, plugins, MCP, and CLAUDE.md for reproducible CI runs | `--bare` |

### B.1 A Read-Only Analysis Run

```bash
claude -p "Run the .NET tests and summarize any failures. Do not change any files." \
  --permission-mode dontAsk \
  --allowedTools "Read,Grep,Glob,Bash(dotnet test *)" \
  --max-turns 10
```

`dontAsk` denies anything that would normally prompt, so only reads and the explicitly allowed `dotnet test` command can run.

### B.2 A Run That Is Allowed to Edit

```bash
claude -p "Fix the naming convention violations in LegacyTaskProcessor.cs, then run dotnet build." \
  --permission-mode acceptEdits \
  --allowedTools "Bash(dotnet build *)" \
  --max-turns 15
```

Review the result with `git diff` — and remember `/rewind` doesn't help you here; use git.

> **Prefix matching:** `Bash(git diff *)` (with a space before `*`) matches `git diff` and `git diff --stat` but not `git diff-index`. The older `Bash(git diff:*)` form still works.

---

## Part C: Session Persistence (5 min)

### C.1 `--continue`

`--continue` (`-c`) picks up the most recent conversation in the current directory:

```bash
# Start a task
claude -p "Read src-dotnet/src/TaskManager.Domain and describe the current domain model."

# Continue in the next command
claude -p "Now suggest 3 improvements to the domain model based on what you read." --continue
```

The second command has the context of the first — Claude Code remembers what it read.

### C.2 `--resume` with a Session ID or Name

When scripts run several conversations, capture the session ID from JSON output:

```bash
SESSION_ID=$(claude -p "Analyze the test coverage in the .NET project." --output-format json | jq -r '.session_id')

claude -p "Now do the same analysis for the Spring Boot project and compare." --resume "$SESSION_ID"
```

You can also name a session with `--name` (`-n`) and resume it by name:

```bash
claude -n coverage-analysis -p "Analyze the test coverage in the .NET project."
claude --resume coverage-analysis -p "Now do the same for Spring Boot."
```

Run `claude --resume` with no arguments (or `/resume` inside a session — `/continue` is an alias) to pick from recent sessions interactively.

### C.3 When to Use Session Persistence

- **Multi-step scripts**: where step 2 needs the output of step 1
- **Daily reports**: resume a session to add today's activity to yesterday's analysis
- **Long-running analysis**: split across multiple terminal sessions

---

## Part D: CI Integration Patterns (8 min)

### D.1 Pre-Commit Code Review Script

A local git hook that reviews staged changes. The diff is piped in, so Claude needs no tool permissions at all:

```bash
#!/bin/bash
# .githooks/pre-commit

STAGED_FILES=$(git diff --cached --name-only --diff-filter=ACM | grep -E "\.(cs|java)$")

if [ -z "$STAGED_FILES" ]; then
  exit 0
fi

echo "Running Claude Code review on staged files..."

REVIEW=$(git diff --cached -- $STAGED_FILES | claude -p \
  --permission-mode dontAsk \
  --max-turns 3 \
  "Review this staged diff for bugs, convention violations, and missing tests. Be concise — flag real issues only, not style preferences. Start your answer with the single word BLOCK if there is a critical bug or security issue, otherwise OK.")

echo "$REVIEW"

if echo "$REVIEW" | head -1 | grep -q "^BLOCK"; then
  echo "⚠ Claude Code found critical issues. Fix them or commit with --no-verify."
  exit 1
fi
```

```
Create the file .githooks/pre-commit with the script above. Make it executable.
```

```bash
chmod +x .githooks/pre-commit
git config core.hooksPath .githooks
```

> Asking for a fixed first token (`BLOCK`/`OK`) is far more reliable than grepping prose for words like "bug".

### D.2 GitHub Actions with `anthropics/claude-code-action@v1`

For GitHub CI, use Anthropic's official action rather than hand-installing the CLI. The quickest setup is from inside Claude Code:

```
/install-github-app
```

It installs the Claude GitHub App on your repo, stores an auth secret, and opens a PR with a starter workflow. (github.com repos only.)

The action has two modes:
- **Interactive mode** — no `prompt` input; Claude responds to `@claude` mentions in issues and PR comments.
- **Automation mode** — a `prompt` input; Claude runs on any event (PR opened, schedule, etc.).

Key inputs:

| Input | Purpose |
|---|---|
| `prompt` | Instructions (plain text or a `/skill-name`). Omit for `@claude` interactive mode |
| `claude_args` | Any Claude Code CLI flags: `--allowedTools`, `--max-turns`, `--model`, `--append-system-prompt`, … |
| `anthropic_api_key` | Claude API key (from a repo secret) |
| `claude_code_oauth_token` | Alternative: Claude subscription token, generated with `claude setup-token` |
| `settings` | Claude Code settings JSON (string or file path) |
| `plugin_marketplaces` / `plugins` | Install plugins before the run |

**Example: automated PR description generator**

```yaml
# .github/workflows/pr-description.yml
name: Generate PR Description
on:
  pull_request:
    types: [opened]

jobs:
  describe-pr:
    runs-on: ubuntu-latest
    permissions:
      contents: read
      pull-requests: write
      id-token: write
    steps:
      - uses: actions/checkout@v4
        with:
          fetch-depth: 0

      - name: Generate PR description
        uses: anthropics/claude-code-action@v1
        env:
          GH_TOKEN: ${{ github.token }}
        with:
          anthropic_api_key: ${{ secrets.ANTHROPIC_API_KEY }}
          prompt: |
            Generate a PR description for pull request #${{ github.event.pull_request.number }}
            in ${{ github.repository }}. Use `git log` and `git diff` against
            origin/${{ github.event.pull_request.base.ref }} to see the changes.
            Include: summary, motivation, testing done, and a reviewer checklist.
            Then update the PR body with `gh pr edit <number> --body "<description>"`.
          claude_args: |
            --max-turns 10
            --allowedTools "Bash(git log *),Bash(git diff *),Bash(gh pr edit *)"
```

Note what changed from a hand-rolled approach:
- No `npm install` step — the action brings Claude Code with it
- One `env:` block per step (YAML doesn't allow duplicate keys)
- Tools are granted explicitly via `claude_args` — in automation mode Claude has **no** shell or GitHub access until you allow it
- `id-token: write` is required for the action's default GitHub App authentication

**Example: `@claude` on demand (interactive mode)**

```yaml
# .github/workflows/claude.yml
name: Claude Code
on:
  issue_comment:
    types: [created]
  pull_request_review_comment:
    types: [created]

jobs:
  claude:
    if: contains(github.event.comment.body, '@claude')
    runs-on: ubuntu-latest
    permissions:
      contents: write
      pull-requests: write
      issues: write
      id-token: write
    steps:
      - uses: actions/checkout@v4
        with:
          fetch-depth: 1
      - uses: anthropics/claude-code-action@v1
        with:
          anthropic_api_key: ${{ secrets.ANTHROPIC_API_KEY }}
          claude_args: "--max-turns 10"
```

Now anyone with write access can comment `@claude add a unit test for TaskService.CompleteTask` on a PR.

Review these workflows with the group. Note: they are patterns to understand, not necessarily to run during the lab. The action reads your repo's `CLAUDE.md`, `.claude/settings.json`, and skills — so everything you configured in earlier labs applies in CI too.

> Not on GitHub? The same `claude -p` patterns work in any CI system (GitLab CI/CD has its own guide). Install the CLI on the runner with the native installer, set `ANTHROPIC_API_KEY` from a secret, and prefer `--bare` for reproducibility.

### D.3 Nightly Code Quality Report

```bash
#!/bin/bash
# scripts/nightly-quality-report.sh

DATE=$(date +%Y-%m-%d)
mkdir -p docs/reports

claude -p "
Generate a code quality summary for this repository. Cover:
1. Top 5 files with the most TODO/FIXME comments
2. Public methods with no corresponding tests
3. Classes that violate single responsibility (too many public methods)
4. Any obvious anti-patterns in the recent git log (last 7 days)

Format as a markdown report.
" \
  --permission-mode dontAsk \
  --allowedTools "Read,Grep,Glob,Bash(git log *)" \
  --max-turns 20 \
  > "docs/reports/quality-$DATE.md"

echo "Report generated: docs/reports/quality-$DATE.md"
```

```
Create the file scripts/nightly-quality-report.sh with this content and make it executable.
```

---

## Part E: Safety in Headless Mode (5 min)

Headless mode removes the human-in-the-loop — which means the safety responsibility shifts entirely to your configuration.

**Essential safety practices for headless:**

1. **Start locked down** — use `--permission-mode dontAsk` (deny anything not explicitly allowed) and grant only what the task needs with `--allowedTools`. Avoid `bypassPermissions` / `--dangerously-skip-permissions` outside an isolated container.

2. **Never allow destructive operations in CI** — add to `deny` in `.claude/settings.json` (deny always wins over allow):
   ```json
   "deny": [
     "Bash(git push *)",
     "Bash(rm -rf *)",
     "Bash(git reset --hard *)"
   ]
   ```

3. **Use read-only configurations where possible** — many CI use cases only need Claude Code to read files and generate text:
   ```bash
   claude -p "..." \
     --permission-mode dontAsk \
     --allowedTools "Read,Grep,Glob,Bash(git log *),Bash(git diff *)" \
     --disallowedTools "Edit,Write"
   ```

4. **Bound the run** — `--max-turns` and `--max-budget-usd` stop a runaway loop from burning time and money.

5. **Know what loads** — without `--bare`, a `-p` run executes the project's hooks and connects its `.mcp.json` servers *without* the trust/approval prompts you'd see interactively. Only run headless against repositories you trust, or use `--bare` and pass exactly the settings/MCP config you want.

6. **Audit logs are mandatory in CI** — the hook from Lab 9 belongs in your CI `.claude/settings.json`.

7. **Test headless scripts locally first** — run them with `--permission-mode plan` (read-only planning, no changes) before wiring them to CI.

---

## Success Criteria

- [ ] You ran at least one `claude -p` command non-interactively
- [ ] You piped data into Claude Code and piped its output to a file or another command
- [ ] You parsed `--output-format json` output with `jq`
- [ ] You used `--continue` or `--resume` for session persistence
- [ ] You can describe how `anthropics/claude-code-action@v1` runs in interactive vs. automation mode
- [ ] You know which flags and deny rules are essential before using Claude Code headlessly

---

## Extension Exercise

Write a shell script that:
1. Uses `claude -p ... --output-format json` to list all public interfaces in the `src-dotnet/` directory and captures the `session_id`
2. Uses `--resume "$SESSION_ID"` to check whether each interface has a corresponding implementation
3. Outputs a summary: "X of Y interfaces have implementations" and the total cost from `total_cost_usd`

**Stretch:** use `--json-schema` so step 1 returns `{"interfaces": ["..."]}` and loop over it with `jq`.

---

## Troubleshooting

**`claude -p` prints an authentication error**

- Run `claude` interactively once and `/login`, or set `ANTHROPIC_API_KEY`
- If you used `--bare`, you **must** set `ANTHROPIC_API_KEY` — bare mode ignores subscription login
- Test: `claude -p "Say hello"` should print a greeting and exit

**The run stops with "permission denied" or does nothing**

- Headless runs can't prompt. Add the needed tools with `--allowedTools`, or check `permission_denials` in `--output-format json` output to see what was blocked

**Session not found with `--resume`**

- Use the exact `session_id` from JSON output, or the exact `--name` you set. Run `claude --resume` without arguments to see an interactive list of recent sessions.

**Run ends with a max-turns error**

- Raise `--max-turns`, or narrow the task so it needs fewer steps

**CI pipeline times out or costs too much**

- Reduce the scope of the task
- Set `--max-turns` and `--max-budget-usd`
- Check for tools that need allowing (denied calls cause retries and extra turns)

---

**Next**: [Lab 14: Output Styles & Custom Personas](lab-14-output-styles-and-personas.md)
