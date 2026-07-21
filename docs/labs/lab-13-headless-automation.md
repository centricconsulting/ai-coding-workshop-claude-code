# Lab 13: Headless & Automation Mode

**Duration**: 25–30 minutes
**Day**: 2 of 2 (Advanced)

**Learning Objectives**:
- Run Claude Code non-interactively with `--print`
- Use `--continue` and `--resume` for session persistence
- Pipe Claude Code output into shell pipelines
- Integrate Claude Code into a CI workflow
- Know the safety implications of headless mode

---

## Overview

Everything you've done in the workshop has been interactive — a human in the loop. But Claude Code also runs headlessly: accepting a prompt via CLI flags, doing the work, and exiting. This opens up automation workflows that would otherwise require a human at a terminal.

---

## Prerequisites

- Completed Labs 1–12
- Comfortable with basic shell scripting and pipes

---

## Part A: The `--print` Flag (8 min)

`--print` (or `-p`) runs Claude Code non-interactively: it takes a prompt, does the work, prints the result, and exits.

### A.1 Basic Usage

```bash
claude --print "What are the top 3 issues in src/TaskManager.Domain/LegacyTaskProcessor.cs?"
```

No interactive session opens. Claude Code reads the file, analyzes it, prints the result to stdout, and exits.

### A.2 Piping Output

Because output goes to stdout, you can pipe it:

```bash
# Save the analysis to a file
claude --print "List all public interfaces in the src/ directory." > interfaces.txt

# Count the issues found
claude --print "List every naming convention violation in LegacyTaskProcessor.cs." | wc -l

# Use the output in a script
ISSUES=$(claude --print "Are there any TODO comments in the src/ directory? List them.")
if [ -n "$ISSUES" ]; then
  echo "Found TODOs: $ISSUES"
fi
```

### A.3 JSON Output

For structured output, ask for JSON explicitly:

```bash
claude --print "List all test files in the tests/ directory as JSON. Format: [{file: string, testCount: number}]"
```

This makes it easy to parse Claude Code output in scripts.

---

## Part B: Session Persistence (7 min)

### B.1 `--continue`

`--continue` picks up where the last session left off:

```bash
# Start a task
claude --print "Read src/TaskManager.Domain/Entities/Task.cs and describe the current domain model."

# Continue in the next command
claude --continue --print "Now suggest 3 improvements to the domain model based on what you read."
```

The second command has the context of the first — Claude Code remembers what it read.

### B.2 `--resume` and `--name`

Use `--name` to give a session a display name, then `--resume` to return to it:

```bash
# Start a named session
claude --name my-analysis --print "Analyze the test coverage in the .NET project."

# Resume it later by name
claude --resume my-analysis --print "Now do the same analysis for the Spring Boot project."
```

Session names are set with `--name` and resumed with `--resume <name>`. If you run `--resume` without arguments, Claude Code shows an interactive picker of recent sessions.

### B.3 When to Use Session Persistence

- **Multi-step scripts**: where step 2 needs the output of step 1
- **Daily reports**: resume a session to add today's activity to yesterday's analysis
- **Long-running analysis**: split across multiple terminal sessions

---

## Part C: CI Integration Patterns (10 min)

Headless mode is most valuable in CI pipelines.

### C.1 Pre-Commit Code Review Script

Create a script that runs a code review on every staged change:

```bash
# .githooks/pre-commit
#!/bin/bash

STAGED_FILES=$(git diff --cached --name-only --diff-filter=ACM | grep -E "\.(cs|java)$")

if [ -z "$STAGED_FILES" ]; then
  exit 0
fi

echo "Running Claude Code review on staged files..."

REVIEW=$(claude --print "Review the following staged changes for bugs, convention violations, and missing tests. Be concise — flag real issues only, not style preferences.

Files changed:
$STAGED_FILES

Diff:
$(git diff --cached -- $STAGED_FILES)")

echo "$REVIEW"

# Optionally fail the commit on critical issues
if echo "$REVIEW" | grep -qi "critical\|security\|bug"; then
  echo "⚠ Claude Code found potential issues. Review before committing."
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

### C.2 Automated PR Description Generator

A GitHub Actions workflow that uses Claude Code to generate PR descriptions:

```yaml
# .github/workflows/pr-description.yml
name: Generate PR Description
on:
  pull_request:
    types: [opened]

jobs:
  describe-pr:
    runs-on: ubuntu-latest
    steps:
      - uses: actions/checkout@v4
        with:
          fetch-depth: 0

      - name: Install Claude Code
        run: npm install -g @anthropic-ai/claude-code

      - name: Generate PR description
        env:
          ANTHROPIC_API_KEY: ${{ secrets.ANTHROPIC_API_KEY }}
        run: |
          DESCRIPTION=$(claude --print "
          Generate a PR description for these changes. Include: summary, motivation, testing done, and reviewer checklist.
          
          Branch: ${{ github.head_ref }}
          Commits: $(git log origin/${{ github.base_ref }}..HEAD --oneline)
          Diff summary: $(git diff origin/${{ github.base_ref }}...HEAD --stat)
          ")
          
          gh pr edit ${{ github.event.pull_request.number }} --body "$DESCRIPTION"
        env:
          GH_TOKEN: ${{ github.token }}
```

Review this workflow with the group. Note: this is a pattern to understand, not necessarily to run during the lab.

### C.3 Nightly Code Quality Report

```bash
#!/bin/bash
# scripts/nightly-quality-report.sh

DATE=$(date +%Y-%m-%d)

claude --print "
Generate a code quality summary for this repository. Cover:
1. Top 5 files with the most TODO/FIXME comments
2. Public methods with no corresponding tests
3. Classes that violate single responsibility (too many public methods)
4. Any obvious anti-patterns in the recent git log (last 7 days)

Format as a markdown report.
" > docs/reports/quality-$DATE.md

echo "Report generated: docs/reports/quality-$DATE.md"
```

```
Create the file scripts/nightly-quality-report.sh with this content and make it executable.
```

---

## Part D: Safety in Headless Mode (5 min)

Headless mode removes the human-in-the-loop — which means the safety responsibility shifts entirely to your configuration.

**Essential safety practices for headless:**

1. **Use a restrictive `.claude/settings.json`** — headless Claude Code can't ask for confirmation. If `git push` is allowed, it may push.

2. **Never allow destructive operations in CI** — add to `deny`:
   ```json
   "Bash(git push*)",
   "Bash(rm -rf*)",
   "Bash(git reset --hard*)"
   ```

3. **Use read-only scopes where possible** — many CI use cases only need Claude Code to read files and generate text. Lock it down:
   ```json
   "allow": ["Bash(git log*)", "Bash(git diff*)", "Read(*)"],
   "deny": ["Write(*)", "Edit(*)", "Bash(*)" ]
   ```

4. **Audit logs are mandatory in CI** — the hook from Lab 9 belongs in your CI `.claude/settings.json`.

5. **Test headless scripts locally first** — `--print` with `--dry-run` (if available) before wiring to CI.

---

## Success Criteria

- [ ] You ran at least one `--print` command non-interactively
- [ ] You piped Claude Code output to a file or another command
- [ ] You understand `--continue` and `--resume` for session persistence
- [ ] You can describe at least one CI integration pattern
- [ ] You know which deny rules are essential before using Claude Code headlessly

---

## Extension Exercise

Write a shell script that:
1. Uses `--print` to list all public interfaces in the `src/` directory
2. For each interface, uses `--continue` to check if a corresponding implementation exists
3. Outputs a summary: "X of Y interfaces have implementations"

---

## Troubleshooting

**`--print` exits immediately without doing work**

- Check `ANTHROPIC_API_KEY` is set in the environment
- Verify with: `echo $ANTHROPIC_API_KEY`
- Test: `claude --print "Say hello"` should print a greeting and exit

**Output contains interactive UI elements (colors, spinners)**

Add `--output-format text` to disable terminal formatting:
```bash
claude --print --output-format text "your prompt"
```

**Session not found with `--resume`**

Use `--name <name>` when starting the session, then `--resume <name>` later. Names must match exactly. Run `claude --resume` without arguments to see an interactive list of recent sessions.

**CI pipeline times out**

Headless Claude Code tasks should complete in under 2 minutes for most code tasks. If timing out:
- Reduce the scope of the task
- Add `--max-tokens` to limit response length
- Check for permission prompts that are blocking (add allow rules)

---

**Next**: [Lab 14: Output Styles & Custom Personas](lab-14-output-styles-and-personas.md)
