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

# The `--print` Flag

`--print` (or `-p`) is how you run Claude Code non-interactively:

```bash
claude --print "What are the top 3 issues in LegacyTaskProcessor.cs?"
```

- No interactive session opens
- Output goes to stdout
- Claude Code exits when done

Works anywhere a shell command works: scripts, CI pipelines, cron jobs, git hooks.

---

# Piping Output

Because output is stdout, standard Unix tooling works:

```bash
# Save to a file
claude --print "List all public interfaces in src/." > interfaces.txt

# Count findings
claude --print "List every naming convention violation in LegacyTaskProcessor.cs." | wc -l

# Use in a script
ISSUES=$(claude --print "List TODO comments in src/. JSON array.")
if [ -n "$ISSUES" ]; then
  echo "Found TODOs"
fi
```

Ask for JSON explicitly when you need structured output you can parse.

---

# Session Persistence

`--continue` picks up where the last session left off:

```bash
# Step 1
claude --print "Describe the current domain model in src/TaskManager.Domain."

# Step 2 (remembers step 1)
claude --continue --print "Suggest 3 improvements based on what you read."
```

`--name` + `--resume` for named sessions:

```bash
claude --name coverage-analysis --print "Analyze .NET test coverage."
claude --resume coverage-analysis --print "Now do the same for Spring Boot."
```

---

# CI Integration: Pre-Commit Review

```bash
#!/bin/bash
# .githooks/pre-commit

STAGED=$(git diff --cached --name-only --diff-filter=ACM | grep -E "\.(cs|java)$")
[ -z "$STAGED" ] && exit 0

REVIEW=$(claude --print "Review these staged changes for bugs and convention
violations. Flag real issues only.

Files: $STAGED
Diff: $(git diff --cached -- $STAGED)")

echo "$REVIEW"

if echo "$REVIEW" | grep -qi "critical\|security\|bug"; then
  echo "Claude Code found issues. Review before committing."
  exit 1
fi
```

---

# CI Integration: Automated PR Descriptions

```yaml
# .github/workflows/pr-description.yml
jobs:
  describe-pr:
    runs-on: ubuntu-latest
    steps:
      - uses: actions/checkout@v4
        with: { fetch-depth: 0 }
      - run: npm install -g @anthropic-ai/claude-code
      - name: Generate PR description
        env:
          ANTHROPIC_API_KEY: ${{ secrets.ANTHROPIC_API_KEY }}
        run: |
          DESCRIPTION=$(claude --print "Generate a PR description.
          Branch: ${{ github.head_ref }}
          Commits: $(git log origin/${{ github.base_ref }}..HEAD --oneline)")
          gh pr edit ${{ github.event.pull_request.number }} --body "$DESCRIPTION"
        env:
          GH_TOKEN: ${{ github.token }}
```

---

# CI Integration: Nightly Quality Report

```bash
#!/bin/bash
# scripts/nightly-quality-report.sh

DATE=$(date +%Y-%m-%d)

claude --print "
Generate a code quality summary. Cover:
1. Top 5 files with the most TODO/FIXME comments
2. Public methods with no corresponding tests
3. Classes that violate single responsibility
4. Anti-patterns in the last 7 days of git log

Format as a markdown report.
" > docs/reports/quality-$DATE.md

echo "Report: docs/reports/quality-$DATE.md"
```

Runs in cron. Generates a report. No human required.

---

# Safety in Headless Mode

Headless removes the human-in-the-loop confirmation layer.
**Safety responsibility shifts entirely to your configuration.**

Essential practices:

1. **Deny destructive operations** — headless Claude Code can't ask for confirmation:
   ```json
   "deny": ["Bash(git push*)", "Bash(rm -rf*)", "Bash(git reset --hard*)"]
   ```

2. **Use read-only allows in CI** — most CI tasks only need reads:
   ```json
   "allow": ["Bash(git log*)", "Bash(git diff*)", "Read(*)"],
   "deny": ["Write(*)", "Edit(*)", "Bash(*)"]
   ```

3. **Audit logs are mandatory** — the hook from Lab 9 belongs in CI.

---

# Headless Mode Anti-Patterns

**Don't run headless without a deny list.**
If `git push` is allowed and you ask Claude Code to "finalize and publish," it may push.

**Don't chain `--continue` indefinitely in scripts.**
Session state accumulates. Long chains degrade in quality.

**Don't parse prose output — ask for JSON.**
`"List X"` produces varying formats. `"List X as JSON array [{name, path}]"` is stable.

**Don't run headless on a task that needs interactive judgment.**
If Claude Code would normally ask a clarifying question, `--print` will guess.
Scope the task tightly enough that no clarification is needed.

---

# Lab 13 Preview

## What You'll Do

1. Run your first `--print` command — see output go straight to terminal
2. Pipe output to a file and to another command
3. Use `--continue` across two commands with shared context
4. Create the pre-commit review script and test it
5. Review the GitHub Actions PR description workflow (demo or hands-on)
6. Set up the nightly quality report script

## The Key Insight

`--print` is Claude Code for scripts.
Everything you've learned in this workshop applies — just without the REPL.

---

# Lab 13: Headless & Automation Mode

👉 Open [docs/labs/lab-13-headless-automation.md](../../../labs/lab-13-headless-automation.md)

**Goal for the next 25 minutes:**
- Run at least one `--print` command successfully
- Pipe output to a file
- Create at least one automation script

**Before you do anything:** confirm `ANTHROPIC_API_KEY` is set in your shell.
Headless mode requires it — there's no browser auth fallback.

---

<!-- Navigation -->
[← 12 Sub-Agents](03-sub-agents.md) | **Day 2 Module 13** | [Next: 14 Output Styles →](05-output-styles.md)
