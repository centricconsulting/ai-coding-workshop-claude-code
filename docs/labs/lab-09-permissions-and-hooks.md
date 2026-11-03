# Lab 9: Permissions, Safety & Hooks

**Duration**: 25–30 minutes
**Day**: 2 of 2 (Advanced)

**Learning Objectives**:
- Understand Claude Code's permission modes and permission rules, and why they exist
- Read and modify `.claude/settings.json` (and know when to use `.claude/settings.local.json` instead)
- Write `PreToolUse` and `PostToolUse` hooks that read their input from stdin
- Block an action with a hook (exit code 2) and see Claude Code react to it
- Know which permissions to allow, which to keep as ask, and which to deny

---

## Overview

Claude Code's permissions model is a safety boundary between "Claude Code can suggest this" and "Claude Code can execute this." In Day 1 you experienced permission prompts firsthand. In this lab you'll take deliberate control of that boundary — first with **permission modes and rules**, then with **hooks**, which run your own scripts at fixed points in Claude Code's lifecycle.

> 📚 Reference docs: [Permissions](https://code.claude.com/docs/en/permissions) · [Permission modes](https://code.claude.com/docs/en/permission-modes) · [Settings](https://code.claude.com/docs/en/settings) · [Hooks guide](https://code.claude.com/docs/en/hooks-guide) · [Hooks reference](https://code.claude.com/docs/en/hooks)

---

## Prerequisites

- Completed Day 1 (Labs 1–7) and Lab 8
- Familiar with the permission prompts from Lab 1
- **`jq` installed** — every hook in this lab parses its JSON input with `jq`. Check with `jq --version`. If it's missing: `brew install jq` (macOS), `sudo apt-get install -y jq` (Debian/Ubuntu/dev container), or `winget install jqlang.jq` (Windows). Without `jq`, the hooks fail with a non-blocking `jq: command not found` error and do nothing.

---

## Part A: Understanding the Permissions Model (8 min)

### A.1 Permission Modes

A **permission mode** sets the overall level of autonomy for a session. Press **`Shift+Tab`** to cycle modes; the current mode is shown below the prompt.

| Mode | What runs without asking | Typical use |
|---|---|---|
| `manual` (shown as **Manual**; older alias `default`) | Reads only — everything else prompts | Reviewing every action, sensitive work |
| `acceptEdits` | Reads, file edits, and common filesystem commands (`mkdir`, `mv`, `cp`…) in the working directory | Iterating on code you're reviewing |
| `plan` | Reads and read-only exploration; no source edits | Exploring before changing (Lab 4) |
| `auto` | Everything, with a background safety classifier reviewing actions | Long tasks, less prompt fatigue |
| `dontAsk` | Only pre-approved tools; anything that would prompt is **denied** | Locked-down CI and scripts (Lab 13) |
| `bypassPermissions` | Everything (except a few protected actions) | Isolated containers/VMs only |

You can also start in a specific mode with `claude --permission-mode plan`, or set a starting mode in settings with `"permissions": { "defaultMode": "acceptEdits" }`.

> **Note**: On current Claude Code versions, **auto** is the built-in starting mode for interactive sessions. To *see* the permission rules in this lab take effect, run this lab in **Manual** mode:
>
> ```bash
> claude --permission-mode manual
> ```

### A.2 Read the Workshop's Settings File

```
Read .claude/settings.json and explain what each section does: what's allowed, what requires confirmation, what's denied, and what the hooks are doing.
```

Claude Code will read the file and explain the structure. Then open the interactive view:

```
/permissions
```

`/permissions` lists every active rule and **which settings file it came from**. You can add or remove rules here too; changes apply immediately.

### A.3 Where Settings Come From

Settings are layered. When the same key is set in more than one place, the higher level wins:

1. **Managed settings** — deployed by your organization (MDM, `managed-settings.json`, or server-managed). Cannot be overridden.
2. **Command line** — e.g. `--settings`, `--permission-mode`, `--allowedTools` for one session
3. **`.claude/settings.local.json`** — *your* personal settings for this project (kept out of git)
4. **`.claude/settings.json`** — the project's shared settings, checked into the repo (team-wide)
5. **`~/.claude/settings.json`** — your user settings for every project

Permission **rules** are different: they are **merged** from every level, and a **deny anywhere wins**. A user-level allow can't override a project-level deny, and nothing overrides a managed deny.

This means:
- `.claude/settings.json` encodes team policy for everyone who clones the repo
- `.claude/settings.local.json` is where you put personal exceptions — no commit needed. When you answer a prompt with "Yes, and don't ask again", Claude Code writes the rule there for you.
- Interactive prompts (or the auto-mode classifier) handle everything no rule covers

> **Workspace trust**: `allow` rules in a project's `.claude/settings.json` only take effect after you accept the trust dialog for that folder. `ask` and `deny` rules always apply, because they only restrict.

### A.4 The Allow / Ask / Deny Format

Rules are written as `Tool` or `Tool(specifier)`:

```json
"allow": [
  "Bash(dotnet *)",        ← any dotnet command (trailing " *" also matches bare "dotnet")
  "Bash(git status)",      ← exact command only
  "Bash(git diff *)",      ← git diff with any arguments
  "Edit(src-dotnet/**)"           ← edit any file under src-dotnet/
],
"ask": [
  "Bash(git push *)"       ← always confirm, even in modes that wouldn't otherwise ask
],
"deny": [
  "Bash(git push --force*)",  ← never force-push
  "Bash(rm -rf *)",           ← never recursive-delete
  "Read(./.env)"              ← never read secrets (also blocks editing them)
]
```

Rules are evaluated **deny → ask → allow**; the first match wins and specificity doesn't matter. So `Bash(dotnet *)` in `allow` does **not** let `dotnet ef …` through when `Bash(dotnet ef *)` is in `deny`.

A few gotchas worth knowing:
- `Bash(dotnet:*)` is the older spelling of `Bash(dotnet *)` — both work; the space form is preferred.
- Use `Edit(path)` / `Read(path)` for file rules. `Write(path)` rules are accepted but **never match** anything.
- Read-only commands like `ls`, `cat`, `grep`, `find`, and read-only `git` already run without a prompt — you don't need allow rules for them.
- Bash rules match command text, so they're a convenience boundary, not a security sandbox. For OS-level isolation of shell commands, see `/sandbox` (macOS, Linux, WSL2).

**Key principle**: Deny the genuinely dangerous operations (force push, hard reset, destructive delete, migrations, secrets), `ask` for the irreversible-but-legitimate ones (push), and allow the noisy-but-safe ones (build, test). Don't try to allow-list everything upfront — you'll miss things.

---

## Part B: Modifying Permissions (7 min)

### B.1 Add a Safe Allow

The workshop's settings don't cover the GitHub CLI, so read-only `gh` commands prompt every time. Add an allow for listing pull requests and issues:

```
Add Bash(gh pr list *) and Bash(gh issue list *) to the allow list in .claude/settings.json.
```

Verify:

```bash
jq .permissions .claude/settings.json
```

### B.2 Test the Change

Settings files are watched, so the change applies to the running session — no restart needed. Run `/permissions` to confirm the new rules are listed, then:

```
List the open pull requests in this repo using the gh CLI.
```

This time there should be no permission prompt for `gh pr list` — Claude Code runs it directly. (If `gh` isn't authenticated, the command itself fails; that's fine — what matters is that you weren't prompted.)

### B.3 Ask vs. Deny

Look at how `git push` is handled:

```
Explain why .claude/settings.json puts "git push *" under ask but "git push --force*" under deny. What would happen if I asked you to force-push?
```

Then add one more protection of your own — for example, a deny for `Bash(git clean -fd*)`, or an `ask` for `Bash(git rebase *)`.

### B.4 Personal Overrides (Optional)

Create a `.claude/settings.local.json` that starts *your* sessions in `acceptEdits` mode without changing it for the team:

```json
{
  "permissions": {
    "defaultMode": "acceptEdits"
  }
}
```

Start a new session and check the mode indicator. Delete the file (or the key) when you're done if you prefer Manual mode.

---

## Part C: Writing Hooks (12 min)

Hooks run your own commands automatically at specific points in Claude Code's lifecycle. They're defined in the `hooks` section of a settings file and are **deterministic** — unlike instructions in CLAUDE.md, a hook *always* runs.

### C.1 Hook Events

The events you'll use most:

| Event | When it fires | Can block? |
|---|---|---|
| `SessionStart` | A session begins, resumes, or is cleared/compacted | — |
| `UserPromptSubmit` | You submit a prompt, before Claude sees it | Yes |
| `PreToolUse` | Before a tool call executes | **Yes** |
| `PermissionRequest` | A tool call is about to show you a permission prompt | Via JSON decision |
| `PostToolUse` | After a tool call succeeds | Feeds back to Claude |
| `PostToolUseFailure` | After a tool call fails | — |
| `Notification` | Claude Code needs your attention (permission prompt, idle) | — |
| `SubagentStart` / `SubagentStop` | A subagent starts / finishes | `SubagentStop`: yes |
| `Stop` | Claude finishes responding | Yes (keeps Claude working) |
| `PreCompact` / `PostCompact` | Around context compaction | `PreCompact`: yes |
| `SessionEnd` | The session terminates | — |

There are more (`Setup`, `PermissionDenied`, `PostToolBatch`, `InstructionsLoaded`, `ConfigChange`, `CwdChanged`, `FileChanged`, `WorktreeCreate`, `Elicitation`, …) — see the [hooks reference](https://code.claude.com/docs/en/hooks) for the full list.

Each hook entry has:
- a **`matcher`** — for tool events, the tool name: `Bash`, `Edit|Write` (exact names separated by `|`), or a regex such as `mcp__.*`. Matchers are case-sensitive.
- one or more **handlers** with a `type`: `command` (shell command — what this lab uses), `http` (POST to a URL), `mcp_tool`, `prompt` (single-turn LLM check), or `agent` (experimental subagent check).

### C.2 How a Command Hook Talks to Claude Code

**Input** — Claude Code sends a JSON object on the hook's **stdin**. (There are no `$CLAUDE_TOOL_*` environment variables; `$CLAUDE_PROJECT_DIR` is the one you'll use most.) For a `PreToolUse` on `Bash`:

```json
{
  "session_id": "abc123",
  "cwd": "/workspaces/ai-coding-workshop-claude-code",
  "permission_mode": "default",
  "hook_event_name": "PreToolUse",
  "tool_name": "Bash",
  "tool_input": { "command": "dotnet test", "description": "Run tests" }
}
```

For `Edit`/`Write`, `tool_input.file_path` holds the file's path. Extract fields with `jq -r '.tool_input.command'`.

**Output** — the exit code decides what happens:

| Exit code | Meaning |
|---|---|
| `0` | Success. Action proceeds. Optional JSON on stdout is applied. |
| `2` | **Blocking error.** On `PreToolUse` the tool call is blocked, and your **stderr** is sent to Claude as the reason. On `PostToolUse` the tool already ran, but stderr is shown to Claude. |
| anything else | **Non-blocking error.** You see a `hook error` notice; the action still proceeds. |

> ⚠️ `exit 1` does **not** block. Use `exit 2` (or the JSON decision below) for anything that must stop an action.

For finer control, a `PreToolUse` hook can exit `0` and print JSON instead:

```json
{
  "hookSpecificOutput": {
    "hookEventName": "PreToolUse",
    "permissionDecision": "deny",
    "permissionDecisionReason": "Use the repository script instead"
  }
}
```

`permissionDecision` can be `allow` (skip the prompt), `ask` (force a prompt), `deny` (block), or `defer` (headless only). A hook's `deny` holds in **every** permission mode — even `bypassPermissions` — while a hook's `allow` can never override a `deny` or `ask` rule from settings.

### C.3 Existing Hook: Audit Log

The workshop settings already include an audit-log hook. Look at it:

```bash
jq .hooks .claude/settings.json
```

The `PreToolUse` hook on `Bash` reads the command from stdin and appends it to `.claude/audit.log` (git-ignored by `*.log`); the `PostToolUse` hook on `Edit|Write` logs each changed file. Test it:

```
Run "dotnet --version", then create a file named hook-test.txt in the repo root containing "hook test".
```

Then check the audit log:

```bash
cat .claude/audit.log
```

You should see a `Bash: dotnet --version` line and a `Write: …/hook-test.txt` line. Delete `hook-test.txt` when you're done.

Now type `/hooks`. It opens a **read-only browser** of every configured hook, labeled with where it comes from (user, project, local settings, plugin, or session). Select a hook to see its full command and source file. To change hooks, edit the settings file (or ask Claude to) — changes are picked up automatically.

> **Tip**: Test a hook command outside Claude Code by piping sample JSON into it:
> ```bash
> echo '{"tool_name":"Bash","tool_input":{"command":"ls"}}' \
>   | CLAUDE_PROJECT_DIR=$PWD sh -c "$(jq -r '.hooks.PreToolUse[0].hooks[0].command' .claude/settings.json)"
> echo $?
> tail -1 .claude/audit.log
> ```

### C.4 Write a Blocking Hook (exit 2)

Project rules say the `.devcontainer/` files must not change during labs. A CLAUDE.md instruction *asks* Claude not to; a hook *guarantees* it. Ask Claude Code to build the guard:

```
Create .claude/hooks/protect-files.sh: a PreToolUse hook script that reads the hook JSON from stdin with jq, gets .tool_input.file_path, and if the path contains ".devcontainer/", ".env", or ".git/", prints a clear reason to stderr and exits 2. Otherwise exit 0. Make it executable, then register it in .claude/settings.json under PreToolUse with matcher "Edit|Write", using "${CLAUDE_PROJECT_DIR}/.claude/hooks/protect-files.sh" as the command. Show me the script and the settings change before writing.
```

The result should look like this:

```bash
#!/bin/bash
# .claude/hooks/protect-files.sh — PreToolUse hook for Edit|Write
FILE_PATH=$(jq -r '.tool_input.file_path // empty')

for pattern in ".devcontainer/" ".env" ".git/"; do
  if [[ "$FILE_PATH" == *"$pattern"* ]]; then
    echo "Blocked by protect-files hook: $FILE_PATH matches protected pattern '$pattern'. Ask the user to make this change by hand." >&2
    exit 2
  fi
done

exit 0
```

```json
"PreToolUse": [
  { "matcher": "Bash", "hooks": [ { "type": "command", "command": "…audit log…" } ] },
  {
    "matcher": "Edit|Write",
    "hooks": [
      { "type": "command", "command": "\"${CLAUDE_PROJECT_DIR}\"/.claude/hooks/protect-files.sh" }
    ]
  }
]
```

**Test it in isolation first:**

```bash
echo '{"tool_name":"Edit","tool_input":{"file_path":"/repo/.devcontainer/README.md"}}' | .claude/hooks/protect-files.sh; echo "exit=$?"
echo '{"tool_name":"Edit","tool_input":{"file_path":"/repo/src-dotnet/src/TaskManager.Api/Program.cs"}}' | .claude/hooks/protect-files.sh; echo "exit=$?"
```

You should see the block message with `exit=2`, then `exit=0`.

**Then test it live:**

```
Add a line at the end of .devcontainer/README.md saying "edited by Claude".
```

Watch what happens: the edit is blocked, Claude receives your stderr message (`PreToolUse:Edit hook error: … Blocked by protect-files hook …`), and it should stop and tell you rather than retrying. Run `git status` to confirm `.devcontainer/` is untouched.

> Notice that the block happened **regardless of permission mode**. Try it again after switching to `acceptEdits` with `Shift+Tab` — still blocked.

### C.5 Write a Format-After-Edit Hook

A common team use case: automatically format code after Claude Code changes a file. Claude changes files with **both** `Edit` (modify) and `Write` (create/overwrite), so match both:

```
Add a PostToolUse hook to .claude/settings.json with matcher "Edit|Write". It should read the hook JSON from stdin with jq, get .tool_input.file_path, and only if it ends in .cs, run "dotnet format whitespace TaskManager.sln --include <path>" from $CLAUDE_PROJECT_DIR/src-dotnet. dotnet format needs the path relative to the solution, so strip the "$CLAUDE_PROJECT_DIR/src-dotnet/" prefix. Put the logic in .claude/hooks/format-cs.sh. Show me what you'd add before writing.
```

Review the proposed hook. A working script looks like:

```bash
#!/bin/bash
# .claude/hooks/format-cs.sh — PostToolUse hook for Edit|Write
FILE_PATH=$(jq -r '.tool_input.file_path // empty')
case "$FILE_PATH" in
  *.cs)
    cd "$CLAUDE_PROJECT_DIR/src-dotnet" &&
      dotnet format whitespace TaskManager.sln --include "${FILE_PATH#"$CLAUDE_PROJECT_DIR/src-dotnet"/}"
    ;;
esac
exit 0
```

Confirm and write. Then ask Claude to make a small change to `src-dotnet/src/TaskManager.Domain/Tasks/Task.cs` and check `git diff` — the file should come back consistently formatted.

> **Spring Boot track**: use the same pattern with `*.java` and your formatter of choice (for example `mvn spotless:apply` if the project uses Spotless). Don't add new Maven plugins without asking.

### C.6 Write a Test-Run Hook

Add a `PostToolUse` hook that runs the unit tests whenever a unit test file changes, and tells Claude if they fail:

```
Add another PostToolUse hook with matcher "Edit|Write". Read .tool_input.file_path from stdin with jq. If the path is under src-dotnet/tests/TaskManager.UnitTests/ and ends in .cs, run "dotnet test src-dotnet/tests/TaskManager.UnitTests/ --nologo -v q" from $CLAUDE_PROJECT_DIR with its output sent to stderr, and exit 2 if the tests fail so Claude sees the failure. Exit 0 otherwise.
```

A one-line version of the command:

```bash
f=$(jq -r '.tool_input.file_path // empty'); case "$f" in */tests/TaskManager.UnitTests/*.cs) cd "$CLAUDE_PROJECT_DIR" && dotnet test src-dotnet/tests/TaskManager.UnitTests/ --nologo -v q >&2 || exit 2 ;; esac
```

Now every time Claude Code changes a test file, the tests run automatically, and a failure comes straight back to Claude via exit code 2 — no prompt needed.

> Don't add `--no-build` here: the file just changed, so the tests must be rebuilt.

---

## Part D: When NOT to Automate (3 min)

Hooks are powerful — and can create friction if overused.

**Good hook candidates:**
- Code formatters (idempotent, fast)
- Audit logs (read-only side effect)
- Guards that block clearly forbidden actions (exit 2 with a helpful message)
- Linters/tests that report back to Claude

**Bad hook candidates:**
- Slow build steps on every file change (blocks Claude Code mid-task — hooks run synchronously and default to a 10-minute timeout)
- Hooks that prompt for user input (there's no terminal to answer them)
- Hooks that rewrite files in unpredictable ways (Claude's next edit is based on what it *thinks* the file contains)
- Policy that only *usually* matters — use an `ask` rule instead of a hard block

The rule: hooks should be **fast, non-interactive, and predictable**, and should **block only on purpose** (exit 2), never by accident.

> **Security note**: hooks run arbitrary shell commands with your user's permissions. Review hook changes in `.claude/settings.json` like any other code, and use `/hooks` to see exactly what will run.

---

## Success Criteria

- [ ] You can name the permission modes and switch between them with `Shift+Tab`
- [ ] You can explain the settings precedence (managed → command line → local → project → user) and why deny always wins
- [ ] You added at least one allow rule and one ask or deny rule to `.claude/settings.json`
- [ ] You observed the audit-log hook fire and viewed it in `/hooks`
- [ ] You wrote a `PreToolUse` hook that blocks with exit code 2 and saw Claude Code react to the block
- [ ] You wrote a working `PostToolUse` hook matching `Edit|Write`
- [ ] You understand why some automations make bad hooks

---

## Extension Exercise

Add a `Stop` hook that logs a line each time Claude Code finishes responding. (`Stop` doesn't use a matcher, so omit it.)

```json
"Stop": [
  {
    "hooks": [{
      "type": "command",
      "command": "jq -r '\"[\\(now | todate)] Stop in \\(.cwd) (session \\(.session_id))\"' >> \"${CLAUDE_PROJECT_DIR}/.claude/sessions.log\""
    }]
  }
]
```

Then run a few prompts and check `.claude/sessions.log` to see the session timeline.

**Stretch**: add a `Notification` hook with matcher `permission_prompt|idle_prompt` that triggers a desktop notification (`osascript -e 'display notification "Claude Code needs you"'` on macOS, `notify-send` on Linux) so you can switch windows while Claude works.

---

## Troubleshooting

**Hooks not firing**

- Run `/hooks` and confirm the hook appears under the right event and source
- Check the `matcher` — it's case-sensitive and matches the tool name (`Edit|Write`, not `edit|write`)
- Check that `.claude/settings.json` is valid JSON: `jq . .claude/settings.json`
- Hooks fire only for matched tools: if Claude uses the `Read` tool instead of `cat`, a `Bash` matcher won't fire

**`PreToolUse:… hook error` in the transcript**

- `jq: command not found` → install `jq` (see Prerequisites)
- `No such file or directory` / `Permission denied` → check the script path and run `chmod +x .claude/hooks/*.sh`
- Test the hook by piping sample JSON into it and checking `echo $?`

**My "block" didn't block**

- The hook must exit with **code 2** (or print a JSON `permissionDecision: "deny"`). `exit 1` is a non-blocking error.
- The block reason must go to **stderr** (`>&2`), or Claude won't see why.

**Permission allow not working**

- Run `/permissions` to see whether the rule loaded and which file it came from
- Check for typos in the tool name or pattern (`Bash(gh pr list *)` — note the space before `*`)
- Remember: `deny` and `ask` rules are checked before `allow`, at every settings level
- Project `allow` rules need workspace trust — accept the trust dialog when you start Claude Code in the repo
- If you're in **auto** mode, the classifier may approve actions anyway; switch to Manual with `Shift+Tab` to see rules in isolation

**Seeing hook stderr**

Claude Code doesn't show stderr from a hook that exits 0. Start with `claude --debug` to see hook execution details, or temporarily append `2>> .claude/hook-errors.log` to the command.

---

**Next**: [Lab 10: MCP Servers](lab-10-mcp-servers.md)
