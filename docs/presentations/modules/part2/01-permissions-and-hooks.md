---
marp: true
theme: default
paginate: true
backgroundColor: #fff
---

# Module 09: Permissions, Safety & Hooks

## Taking Deliberate Control of Claude Code's Boundaries

---

# Where We Are

Day 1 covered first contact, CLAUDE.md, TDD, plan mode, refactoring, checkpoints & rewind, and git workflow. Lab 8 just added a security review gate.

So far, you've *experienced* permission prompts — Claude Code pausing to ask before running a command.

**Now you stop experiencing permissions and start *designing* them.**

---

# The Permissions Model

Claude Code separates **suggestion** from **execution**.

Permission prompts are the gap between the two:

```
Bash command
  git push origin workshop/your-name
Do you want to proceed?
❯ 1. Yes
  2. Yes, and don't ask again for: git push *
  3. No
```

The prompt exists because pushing to a remote is irreversible.
The goal isn't to eliminate prompts — it's to eliminate the *wrong* prompts.

Permissions are enforced by **Claude Code**, not the model — CLAUDE.md can't grant or revoke them.

---

# Permission Modes — Your Autonomy Dial

<style scoped>
section { font-size: 22px; }
</style>

| Mode | Runs without asking | Use for |
|---|---|---|
| `manual` (alias `default`) | Reads only | Reviewing every action |
| `acceptEdits` | Reads + file edits + `mkdir`/`mv`/`cp` | Iterating on code you review |
| `plan` | Read-only exploration | Plan before you change (Lab 4) |
| `auto` | Everything, checked by a safety classifier | Long tasks, less prompt fatigue |
| `dontAsk` | Only pre-approved tools; rest denied | CI and scripts (Lab 13) |
| `bypassPermissions` | Everything | Isolated containers/VMs only |

`Shift+Tab` cycles modes · `claude --permission-mode plan` · `"defaultMode"` in settings

**auto** is now the built-in starting mode — use Manual to see your rules in action.

---

# Settings Precedence

```
Managed settings          ← org policy (MDM / managed-settings.json) — can't be overridden
        │
Command line              ← --settings, --permission-mode, --allowedTools
        │
.claude/settings.local.json  ← you, this project (not committed)
        │
.claude/settings.json     ← team, this project (checked in)
        │
~/.claude/settings.json   ← you, every project
```

For ordinary keys, **higher wins**. For permission **rules**, every level is merged and **deny anywhere wins** — a user allow can't undo a project deny.

`/permissions` shows every rule *and the file it came from*.

---

# Allow / Ask / Deny

```json
{
  "permissions": {
    "allow": ["Bash(dotnet *)", "Bash(git diff *)", "Edit(src-dotnet/**)"],
    "ask":   ["Bash(git push *)"],
    "deny":  ["Bash(git push --force*)", "Bash(rm -rf *)",
              "Bash(dotnet ef *)", "Read(./.env)"],
    "defaultMode": "default"
  }
}
```

Evaluated **deny → ask → allow** — first match wins, specificity doesn't.
So `Bash(dotnet *)` never lets `dotnet ef …` through.

`Bash(dotnet:*)` still works; `Bash(dotnet *)` is the current form.
Use `Edit(path)` / `Read(path)` for files — `Write(path)` rules never match.

---

# What Belongs in Each List

<style scoped>
section { font-size: 27px; }
</style>

<style scoped>
section { display: grid; grid-template-columns: minmax(0, 1fr) minmax(0, 1fr); column-gap: 48px; align-content: start; }
h1 { grid-column: 1 / -1; grid-row: 1; }
p:nth-of-type(1) { grid-column: 1; grid-row: 2; }
ul:nth-of-type(1) { grid-column: 1; grid-row: 3; }
p:nth-of-type(2) { grid-column: 1; grid-row: 4; }
ul:nth-of-type(2) { grid-column: 1; grid-row: 5; }
p:nth-of-type(3) { grid-column: 2; grid-row: 2; }
ul:nth-of-type(3) { grid-column: 2; grid-row: 3; }
p:nth-of-type(4) { grid-column: 2; grid-row: 4 / span 2; }
p:nth-of-type(5) { grid-column: 2; grid-row: 6; }
</style>

**Allow (no prompt needed):**
- Build and test commands (`dotnet build`, `mvn test`)
- Routine git (`git add`, `git commit`)
- (`ls`, `cat`, `grep`, read-only `git` are already prompt-free)

**Ask (always confirm):**
- Irreversible but legitimate: `git push`

**Deny (blocked outright):**
- Destructive: `rm -rf`, `git push --force`, `git reset --hard`
- Database migrations (`dotnet ef`, `mvn flyway:migrate`)
- Secrets: `Read(./.env)`

**Everything else:** prompt at runtime (or the auto-mode classifier).

Need OS-level isolation for shell commands? `/sandbox` (macOS, Linux, WSL2).

---

# Hooks: Deterministic Automation

<style scoped>
section { font-size: 27px; }
</style>

Hooks run **your** command at fixed points in Claude Code's lifecycle.
CLAUDE.md *asks* Claude to do something — a hook *guarantees* it.

```json
{
  "hooks": {
    "PostToolUse": [{
      "matcher": "Edit|Write",
      "hooks": [{
        "type": "command",
        "command": "\"${CLAUDE_PROJECT_DIR}\"/.claude/hooks/format-cs.sh"
      }]
    }]
  }
}
```

Handler types: `command` · `http` · `mcp_tool` · `prompt` · `agent` (experimental)

`/hooks` = read-only browser of every hook and where it's defined.

---

# Hook Events

<style scoped>
section { font-size: 22px; }
</style>

<style scoped>
section { display: grid; grid-template-columns: minmax(0, 1fr) minmax(0, 1fr); column-gap: 48px; align-content: start; }
h1 { grid-column: 1 / -1; grid-row: 1; }
table { grid-row: 2; align-self: start; display: table; width: 100%; }
table:nth-of-type(1) { grid-column: 1; }
table:nth-of-type(2) { grid-column: 2; }
p { grid-column: 1 / -1; grid-row: 3; }
</style>

| Event | When it fires |
|---|---|
| `SessionStart` / `SessionEnd` | Session begins (or resumes/clears) / ends |
| `UserPromptSubmit` | You submit a prompt, before Claude sees it |
| `PreToolUse` | Before a tool call — **can block it** |
| `PermissionRequest` | A permission prompt is about to appear |
| `PostToolUse` / `PostToolUseFailure` | After a tool call succeeds / fails |

| Event | When it fires |
|---|---|
| `Notification` | Claude Code needs your attention |
| `SubagentStart` / `SubagentStop` | A subagent starts / finishes |
| `Stop` | Claude finishes responding |
| `PreCompact` / `PostCompact` | Around context compaction |

…plus `Setup`, `PermissionDenied`, `PostToolBatch`, `InstructionsLoaded`, `ConfigChange`, `CwdChanged`, `FileChanged`, `Worktree*`, `Elicitation*` and more.

---

# Hook Input: JSON on stdin

```json
{
  "session_id": "abc123",
  "cwd": "/workspaces/ai-coding-workshop-claude-code",
  "permission_mode": "default",
  "hook_event_name": "PreToolUse",
  "tool_name": "Edit",
  "tool_input": { "file_path": "/…/Task.cs", "old_string": "…", "new_string": "…" }
}
```

Read it with `jq`:

```bash
FILE_PATH=$(jq -r '.tool_input.file_path // empty')
```

There is **no** `$CLAUDE_TOOL_INPUT` variable — input only arrives on stdin.
`$CLAUDE_PROJECT_DIR` points at the project root.

---

# Hook Output: Exit Codes

<style scoped>
section { font-size: 25px; }
</style>

| Exit | Meaning |
|---|---|
| `0` | Success — action proceeds (JSON on stdout is applied) |
| `2` | **Block** — `PreToolUse` stops the tool; **stderr goes to Claude** |
| other | Non-blocking error — notice shown, action **still proceeds** |

⚠️ `exit 1` does **not** block.

Or exit 0 with a JSON decision:

```json
{ "hookSpecificOutput": {
    "hookEventName": "PreToolUse",
    "permissionDecision": "deny",
    "permissionDecisionReason": "Protected file" } }
```

`allow` · `ask` · `deny` · `defer` — a hook `deny` holds in **every** mode, even `bypassPermissions`.

---

# Blocking Example: Protect Files

<style scoped>
section { font-size: 27px; }
</style>

```bash
#!/bin/bash
# .claude/hooks/protect-files.sh — PreToolUse, matcher "Edit|Write"
FILE_PATH=$(jq -r '.tool_input.file_path // empty')

for pattern in ".devcontainer/" ".env" ".git/"; do
  if [[ "$FILE_PATH" == *"$pattern"* ]]; then
    echo "Blocked: $FILE_PATH matches protected pattern '$pattern'" >&2
    exit 2
  fi
done
exit 0
```

Claude sees: `PreToolUse:Edit hook error: … Blocked: … '.devcontainer/'`
— and adjusts instead of editing.

Test without Claude: `echo '{"tool_input":{"file_path":"x/.env"}}' | ./protect-files.sh; echo $?`

---

# Practical Hook Patterns

**Audit log** (shipped in `.claude/settings.json`):
```bash
jq -r '"[\(now | todate)] Bash: \(.tool_input.command)"' >> "${CLAUDE_PROJECT_DIR}/.claude/audit.log"
```

**Format after edit** (`PostToolUse`, `Edit|Write`):
```bash
f=$(jq -r '.tool_input.file_path // empty'); case "$f" in *.cs) cd "$CLAUDE_PROJECT_DIR/src-dotnet" &&
  dotnet format whitespace TaskManager.sln --include "${f#"$CLAUDE_PROJECT_DIR/src-dotnet"/}";; esac
```

**Run tests when a test file changes** — and tell Claude if they fail:
```bash
f=$(jq -r '.tool_input.file_path // empty'); case "$f" in */tests/TaskManager.UnitTests/*.cs)
  cd "$CLAUDE_PROJECT_DIR" && dotnet test src-dotnet/tests/TaskManager.UnitTests/ --nologo -v q >&2 || exit 2;; esac
```

---

# When NOT to Write a Hook

<style scoped>
section { font-size: 26px; }
</style>

**Good hook candidates:**
- Code formatters (idempotent, fast, non-interactive)
- Audit logs (read-only side effect)
- Guards that block clearly forbidden actions (exit 2 + clear message)

**Bad hook candidates:**
- Slow builds on every file change (blocks Claude Code mid-task)
- Hooks that prompt for user input (no one can answer)
- Policy that only *usually* matters — use an `ask` rule instead

Rule: hooks should be **fast, non-interactive, and predictable**,
and should **block only on purpose**.

Hooks run with your user's permissions — review them like code.

---

# Lab 9 Preview

<style scoped>
section { font-size: 26px; }
</style>

## What You'll Do

1. Switch permission modes and inspect rules with `/permissions`
2. Add an allow rule and an ask/deny rule; see deny → ask → allow in action
3. Watch the shipped audit-log hook fire; browse it with `/hooks`
4. Build a `PreToolUse` guard that **blocks** `.devcontainer/` edits with exit 2
5. Add format-after-edit and run-tests `PostToolUse` hooks (`Edit|Write`)

**Prerequisite:** `jq` (`jq --version`)

## The Key Insight

After this lab, permission prompts become **intentional** —
not random interruptions, but the boundary you designed.

---

# Lab 9: Permissions, Safety & Hooks

👉 Open [docs/labs/lab-09-permissions-and-hooks.md](../../../labs/lab-09-permissions-and-hooks.md)

**Goal for the next 25–30 minutes:**
- Understand what's already configured and why
- Add one allow rule and one ask or deny rule
- Write a hook that blocks, and one that automates

**The test:** ask Claude to edit a protected file and watch your hook stop it.

📚 [code.claude.com/docs/en/permissions](https://code.claude.com/docs/en/permissions) · [hooks-guide](https://code.claude.com/docs/en/hooks-guide) · [hooks](https://code.claude.com/docs/en/hooks)

---

<!-- Navigation -->
[← 08 Security Review](00-security-review.md) | **Day 2 Module 09** | [Next: 10 MCP Servers →](02-mcp-servers.md)
