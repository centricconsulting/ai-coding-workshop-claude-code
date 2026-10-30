---
marp: true
theme: default
paginate: true
backgroundColor: #fff
---

# Module 13: Headless & Automation Mode

## Claude Code Without a Human in the Loop

---

# Where We Are

Everything you've done in this workshop has been **interactive** —
a human at the terminal, reading output, deciding what to do next.

**Lab 13 removes the human.**

Claude Code accepts a prompt, does the work, prints the result, and exits.
No session. No REPL. No waiting.

---

# The `-p` Flag

`-p` (long form `--print`) is how you run Claude Code non-interactively:

```bash
claude -p "What are the top 3 issues in LegacyTaskProcessor.cs?"
```

- No interactive session opens
- Output goes to stdout; exit code 0 on success, non-zero on failure
- Same login, CLAUDE.md, settings, hooks, skills, and MCP as interactive mode

Works anywhere a shell command works: scripts, CI pipelines, cron jobs, git hooks.

Part of the **Claude Agent SDK** (formerly "Claude Code SDK") — also available as Python and TypeScript packages.

---

# Piping In and Out

`claude -p` reads stdin and writes stdout — standard Unix tooling works:

```bash
# Pipe data in (no Bash permission needed to read it)
git diff main | claude -p "Report any typos in this diff as file:line."

# Save to a file
claude -p "List all public interfaces in src-dotnet/." > interfaces.txt

# Structured output for scripts
claude -p "How many test projects are in src-dotnet/tests/?" --output-format json | jq -r '.result'
```

`--output-format text | json | stream-json` — JSON includes `result`, `session_id`, `total_cost_usd`.
Add `--json-schema` for validated output in `structured_output`.

---

# Controlling a Headless Run

Nobody can answer a permission prompt — decide **before** the run:

| Flag | Purpose |
|---|---|
| `--allowedTools "Read,Bash(dotnet test *)"` | Pre-approve specific tools |
| `--disallowedTools "Edit,Write"` | Remove/deny tools |
| `--permission-mode dontAsk` | Deny anything not explicitly allowed |
| `--permission-mode acceptEdits` | Allow file edits without prompting |
| `--max-turns 10` / `--max-budget-usd 0.50` | Bound the run |
| `--bare` | Skip auto-loaded hooks/skills/MCP/CLAUDE.md (reproducible CI) |

---

# Session Persistence

`--continue` picks up the most recent conversation:

```bash
claude -p "Describe the current domain model in src-dotnet/src/TaskManager.Domain."
claude -p "Suggest 3 improvements based on what you read." --continue
```

`--resume` a specific session by ID (or by `--name`):

```bash
SID=$(claude -p "Analyze .NET test coverage." --output-format json | jq -r '.session_id')
claude -p "Now do the same for Spring Boot." --resume "$SID"
```

Interactive: `/resume` (alias `/continue`) opens the session picker.

---

# CI Integration: Pre-Commit Review

```bash
#!/bin/bash
# .githooks/pre-commit

STAGED=$(git diff --cached --name-only --diff-filter=ACM | grep -E "\.(cs|java)$")
[ -z "$STAGED" ] && exit 0

REVIEW=$(git diff --cached -- $STAGED | claude -p --permission-mode dontAsk \
  "Review this staged diff for bugs and convention violations. Flag real
   issues only. Start with BLOCK if there is a critical issue, else OK.")

echo "$REVIEW"
echo "$REVIEW" | head -1 | grep -q "^BLOCK" && exit 1
exit 0
```

---

# CI Integration: `anthropics/claude-code-action@v1`

Fastest setup: run **`/install-github-app`** inside Claude Code
(installs the GitHub App, adds the secret, opens a PR with a workflow).

```yaml
jobs:
  describe-pr:
    runs-on: ubuntu-latest
    permissions: { contents: read, pull-requests: write, id-token: write }
    steps:
      - uses: actions/checkout@v4
        with: { fetch-depth: 0 }
      - uses: anthropics/claude-code-action@v1
        env:
          GH_TOKEN: ${{ github.token }}
        with:
          anthropic_api_key: ${{ secrets.ANTHROPIC_API_KEY }}
          prompt: "Write a PR description for PR #${{ github.event.pull_request.number }} and apply it with gh pr edit."
          claude_args: '--max-turns 10 --allowedTools "Bash(git log *),Bash(git diff *),Bash(gh pr edit *)"'
```

No `prompt` → **interactive mode**: Claude answers `@claude` mentions in issues/PRs.

---

# CI Integration: Nightly Quality Report

```bash
#!/bin/bash
# scripts/nightly-quality-report.sh

DATE=$(date +%Y-%m-%d)

claude -p "
Generate a code quality summary. Cover:
1. Top 5 files with the most TODO/FIXME comments
2. Public methods with no corresponding tests
3. Classes that violate single responsibility
4. Anti-patterns in the last 7 days of git log

Format as a markdown report.
" --permission-mode dontAsk --allowedTools "Read,Grep,Glob,Bash(git log *)" \
  --max-turns 20 > docs/reports/quality-$DATE.md

echo "Report: docs/reports/quality-$DATE.md"
```

Runs in cron. Generates a report. No human required.

---

# Safety in Headless Mode

Headless removes the human-in-the-loop confirmation layer.
**Safety responsibility shifts entirely to your configuration.**

Essential practices:

1. **Start locked down** — `--permission-mode dontAsk` + a minimal `--allowedTools` list

2. **Deny destructive operations** (deny always beats allow):
   ```json
   "deny": ["Bash(git push *)", "Bash(rm -rf *)", "Bash(git reset --hard *)"]
   ```

3. **Bound every run** — `--max-turns`, `--max-budget-usd`

4. **Know what loads** — without `--bare`, `-p` runs project hooks and `.mcp.json` servers with no trust prompt. Only run against trusted repos.

5. **Audit logs are mandatory** — the hook from Lab 9 belongs in CI.

---

# Headless Mode Anti-Patterns

**Don't run headless without a deny list.**
If `git push` is allowed and you ask Claude Code to "finalize and publish," it may push.

**Don't chain `--continue` indefinitely in scripts.**
Session state accumulates. Long chains degrade in quality.

**Don't parse prose output — use structured output.**
`--output-format json` + `jq`, or `--json-schema` for a validated shape.

**Don't run headless on a task that needs interactive judgment.**
If Claude Code would normally ask a clarifying question, `-p` will guess.
Scope the task tightly enough that no clarification is needed.

---

# Lab 13 Preview

## What You'll Do

1. Run your first `claude -p` command — see output go straight to terminal
2. Pipe data in and out; parse `--output-format json` with `jq`
3. Lock a run down with `--allowedTools`, `--permission-mode`, `--max-turns`
4. Use `--continue` / `--resume` across commands with shared context
5. Create the pre-commit review script and test it
6. Review `anthropics/claude-code-action@v1` workflows (demo or hands-on)

## The Key Insight

`claude -p` is Claude Code for scripts.
Everything you've learned in this workshop applies — just without the REPL.

---

# Lab 13: Headless & Automation Mode

👉 Open [docs/labs/lab-13-headless-automation.md](../../../labs/lab-13-headless-automation.md)

**Goal for the next 25 minutes:**
- Run at least one `claude -p` command successfully
- Pipe output to a file
- Create at least one automation script

**Before you do anything:** confirm `claude -p "Say hello"` works.
`-p` uses your normal login; only `--bare` mode requires `ANTHROPIC_API_KEY`.

---

<!-- Navigation -->
[← 12 Sub-Agents](03-sub-agents.md) | **Day 2 Module 13** | [Next: 14 Output Styles →](05-output-styles.md)
