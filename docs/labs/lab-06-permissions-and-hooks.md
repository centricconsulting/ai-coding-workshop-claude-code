# Lab 6: Permissions, Safety & Hooks

**Duration**: 25–30 minutes
**Part**: 2 of 2 (Advanced)

**Learning Objectives**:
- Understand Claude Code's permissions model and why it exists
- Read and modify `.claude/settings.json`
- Write pre- and post-tool hooks for workflow automation
- Know which permissions to allow vs. keep as prompt-and-approve

---

## Overview

Claude Code's permissions model is a safety boundary between "Claude Code can suggest this" and "Claude Code can execute this." In Part 1 you experienced permission prompts firsthand. In this lab you'll take deliberate control of that boundary.

---

## Prerequisites

- Completed Part 1 (Labs 1–5)
- Familiar with the permission prompts from Lab 1

---

## Part A: Understanding the Permissions Model (8 min)

### A.1 Read the Workshop's Settings File

```bash
claude
```

```
Read .claude/settings.json and explain what each section does: what's allowed, what's denied, and what the hooks are doing.
```

Claude Code will read the file and explain the structure.

### A.2 The Permissions Hierarchy

Claude Code resolves permissions in this order (most specific wins):

1. **`.claude/settings.json`** (project-level, checked into the repo — team-wide settings)
2. **`~/.claude/settings.json`** (user-level — personal overrides)
3. **Interactive prompts** (anything not covered by 1 or 2)

This means:
- The project settings in `.claude/settings.json` apply to everyone who works in this repo
- A developer's personal `~/.claude/settings.json` can add more allowances or restrictions for their local workflow
- Interactive prompts handle everything else at runtime

### A.3 The Allow/Deny Format

Permissions are specified as `Tool(pattern)`:

```json
"allow": [
  "Bash(dotnet:*)",        ← any dotnet subcommand
  "Bash(git status)",      ← exact command only
  "Bash(git diff*)",       ← git diff with any arguments
  "Write(*)"               ← write any file
],
"deny": [
  "Bash(git push*)",       ← deny git push entirely
  "Bash(rm -rf*)"          ← deny destructive deletes
]
```

**Key principle**: Start with a restrictive deny list for genuinely dangerous operations (push, force reset, destructive delete), then allow everything else through prompts. Don't try to allow-list everything upfront — you'll miss things.

---

## Part B: Modifying Permissions (7 min)

### B.1 Add a Safe Allow

The workshop's `.claude/settings.json` requires interactive approval for `cat`. That's inconvenient for read-only exploration. Add it:

```
Add Bash(cat:*) to the allow list in .claude/settings.json.
```

Verify:

```bash
cat .claude/settings.json
```

### B.2 Test the Change

Start a fresh Claude Code session:

```
/clear
```

```
Read the contents of src/TaskManager.Domain/Entities/Task.cs using cat.
```

This time there should be no permission prompt — Claude Code runs `cat` directly.

### B.3 Add a Safety Deny

Add a protection against accidentally running database migrations:

```
Add a deny rule for any dotnet ef command and any mvn flyway command to .claude/settings.json. These should never run during workshop labs.
```

---

## Part C: Writing Hooks (12 min)

Hooks run shell commands automatically when Claude Code uses a tool. They're defined in the `hooks` section of `.claude/settings.json`.

### C.1 Hook Events

| Event | When it fires |
|---|---|
| `PreToolUse` | Before Claude Code calls a tool |
| `PostToolUse` | After Claude Code calls a tool |
| `Notification` | When Claude Code sends a notification |
| `Stop` | When Claude Code finishes a response |

### C.2 Existing Hook: Audit Log

The workshop settings already include a basic audit log hook. Test it:

```
Write a small test file at /tmp/hook-test.txt with the content "hook test".
```

Then check the audit log:

```bash
cat /tmp/claude-audit.log
```

You should see entries for the Write and any Bash commands used.

### C.3 Write a Format-on-Save Hook

A common team use case: automatically format code after Claude Code writes a file.

Add a PostToolUse hook on the Write tool that runs `dotnet format` after any C# file is written:

```
Add a PostToolUse hook to .claude/settings.json. It should match the Write tool and run: if the file path ends in .cs, run "dotnet format src/TaskManager.sln --include $CLAUDE_TOOL_INPUT_FILE_PATH". Show me what you'd add before writing the file.
```

Review the proposed hook. Confirm and write.

> **Note**: Hook commands receive context via environment variables. `$CLAUDE_TOOL_INPUT_FILE_PATH` contains the path of the file just written. See the Claude Code documentation for the full list of available hook variables.

### C.4 Write a Test-Run Hook

Add a PostToolUse hook that runs the relevant tests whenever a test file is written:

```
Add another PostToolUse hook that matches Write tool calls where the file path contains "UnitTests". When triggered, it should run: dotnet test tests/TaskManager.UnitTests/ --no-build
```

Now every time Claude Code writes a test file, the tests run automatically. You see failures immediately without having to prompt for them.

---

## Part D: When NOT to Automate (3 min)

Hooks are powerful — and can create friction if overused.

**Good hook candidates:**
- Code formatters (idempotent, fast)
- Audit logs (read-only side effect)
- Linters that report but don't block

**Bad hook candidates:**
- Slow build steps on every file write (blocks Claude Code mid-task)
- Hooks that prompt for user input (Claude Code can't respond)
- Hooks that modify files Claude Code just wrote (creates loops)

The rule: hooks should be **fast, non-interactive, and non-modifying** unless you specifically want them to gate Claude Code's progress.

---

## Success Criteria

- [ ] You can explain the three-level permissions hierarchy
- [ ] You added at least one allow rule and one deny rule to `.claude/settings.json`
- [ ] You wrote a working PostToolUse hook and observed it fire
- [ ] You understand why some automations make bad hooks

---

## Extension Exercise

Add a `Stop` hook that prints a summary line when Claude Code finishes any response:

```json
{
  "matcher": ".*",
  "hooks": [{
    "type": "command",
    "command": "echo '[claude] Session activity logged at $(date)' >> /tmp/claude-sessions.log"
  }]
}
```

Then run a few prompts and check `/tmp/claude-sessions.log` to see the session timeline.

---

## Troubleshooting

**Hooks not firing**

- Check the `matcher` pattern — it's a regex against the tool name
- Check that `.claude/settings.json` is valid JSON (`jq . .claude/settings.json`)
- Hooks fire only for tools matched; if Claude Code uses `Read` instead of `Bash(cat)`, a `Bash` matcher won't fire

**Permission allow not working**

- `/clear` and start a new session — settings load at startup
- Check for typos in the tool name or pattern
- Remember: `deny` rules override `allow` rules if both match

**Hook command failing silently**

Add stderr redirection to see errors:
```json
"command": "your-command 2>> /tmp/claude-hook-errors.log"
```

---

**Next**: [Lab 7: MCP Servers](lab-07-mcp-servers.md)
