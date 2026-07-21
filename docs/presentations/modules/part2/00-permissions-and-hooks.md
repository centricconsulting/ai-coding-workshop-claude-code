---
marp: true
theme: default
paginate: true
backgroundColor: #fff
---

# Module 06: Permissions, Safety & Hooks

## Taking Deliberate Control of Claude Code's Boundaries

---

# Where We Are

You've completed Part 1: first contact, CLAUDE.md, TDD, refactoring, and git workflow.

In Part 1, you *experienced* permission prompts — Claude Code pausing to ask before running a command.

**Part 2 starts here: you stop experiencing permissions and start *designing* them.**

---

# The Permissions Model

Claude Code separates **suggestion** from **execution**.

Permission prompts are the gap between the two:

```
Claude Code wants to run: git push origin workshop/your-name

Allow? [y/n]
```

The prompt exists because pushing to a remote is irreversible.
The goal isn't to eliminate prompts — it's to eliminate the *wrong* prompts.

---

# Three-Level Hierarchy

```
.claude/settings.json    ← project-level (checked in, team-wide)
        │
        ▼
~/.claude/settings.json  ← user-level (personal overrides)
        │
        ▼
Interactive prompts       ← everything else, at runtime
```

**Most specific wins.** A user-level allow overrides a project-level deny.

The project config encodes team policy. The user config encodes personal workflow.

---

# The Allow/Deny Format

```json
{
  "permissions": {
    "allow": [
      "Bash(dotnet:*)",
      "Bash(git status)",
      "Bash(git diff*)",
      "Write(*)"
    ],
    "deny": [
      "Bash(git push*)",
      "Bash(rm -rf*)"
    ]
  }
}
```

`deny` rules override `allow` rules when both match.

---

# What Belongs in Each List

**Allow (no prompt needed):**
- Build and test commands (`dotnet build`, `mvn test`)
- Read-only git operations (`git log`, `git diff`, `git status`)
- Safe file reads (`cat`, `ls`, `find`)

**Deny (blocked outright):**
- Destructive deletes (`rm -rf`)
- Force operations (`git push --force`, `git reset --hard`)
- Database migrations (`dotnet ef`, `mvn flyway:migrate`)

**Everything else:** prompt at runtime.

---

# Hooks: Side Effects on Tool Calls

Hooks run shell commands automatically when Claude Code uses a tool.

```json
{
  "hooks": {
    "PostToolUse": [{
      "matcher": "Write",
      "hooks": [{
        "type": "command",
        "command": "dotnet format"
      }]
    }]
  }
}
```

This runs `dotnet format` every time Claude Code writes a file.
No prompt. No forgetting. Automatic.

---

# Hook Events

| Event | When it fires |
|---|---|
| `PreToolUse` | Before Claude Code calls a tool |
| `PostToolUse` | After Claude Code calls a tool |
| `Notification` | When Claude Code sends a notification |
| `Stop` | When Claude Code finishes a response |

Hooks receive a JSON blob on stdin describing the tool call —
tool name, arguments, and (for PostToolUse) output.

---

# Hook Input: What You Get on stdin

```json
{
  "tool_name": "Write",
  "tool_input": {
    "file_path": "/path/to/MyService.cs",
    "content": "..."
  }
}
```

Use `jq` or a small script to extract what you need:

```bash
FILE=$(cat - | jq -r '.tool_input.file_path')
if [[ "$FILE" == *.cs ]]; then
  dotnet format "$FILE"
fi
```

---

# Practical Hook Patterns

**Format on write:**
```json
{ "matcher": "Write", "hooks": [{ "type": "command",
  "command": "FILE=$(jq -r '.tool_input.file_path'); dotnet format $FILE" }] }
```

**Run tests when a test file is written:**
```json
{ "matcher": "Write", "hooks": [{ "type": "command",
  "command": "jq -r '.tool_input.file_path' | grep -q 'UnitTests' && dotnet test tests/TaskManager.UnitTests/ --no-build" }] }
```

**Audit log:**
```json
{ "matcher": ".*", "hooks": [{ "type": "command",
  "command": "jq -c '{tool:.tool_name,ts:now}' >> /tmp/claude-audit.log" }] }
```

---

# When NOT to Write a Hook

**Good hook candidates:**
- Code formatters (idempotent, fast, non-interactive)
- Audit logs (read-only side effect)
- Linters that report but don't block

**Bad hook candidates:**
- Slow builds on every file write (blocks Claude Code mid-task)
- Hooks that prompt for user input (Claude Code can't respond)
- Hooks that modify files Claude Code just wrote (creates loops)

Rule: hooks should be **fast, non-interactive, and non-modifying**
unless you specifically want them to gate Claude Code's progress.

---

# Lab 6 Preview

## What You'll Do

1. Ask Claude Code to explain the current `.claude/settings.json`
2. Add a safe allow rule (`cat`) and test it with `/clear`
3. Add a deny rule for database migrations
4. Observe the existing audit log hook fire
5. Write a format-on-save PostToolUse hook
6. Write a test-run PostToolUse hook for test files

## The Key Insight

After this lab, permission prompts become **intentional** —
not random interruptions, but the boundary you designed.

---

# Lab 6: Permissions, Safety & Hooks

👉 Open [docs/labs/lab-06-permissions-and-hooks.md](../../../labs/lab-06-permissions-and-hooks.md)

**Goal for the next 25 minutes:**
- Understand what's already configured and why
- Add one allow rule, one deny rule
- Write and observe at least one working hook

**The test:** run a workflow end-to-end after your hooks are in place
and see them fire automatically.

---

<!-- Navigation -->
[← Part 1 Complete](../part1/05-git-workflow.md) | **Part 2 Module 00** | [Next: 01 MCP Servers →](01-mcp-servers.md)
