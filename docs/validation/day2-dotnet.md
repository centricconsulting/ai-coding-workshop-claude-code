# Day 2 lab validation — .NET path (2026-11-03)

> **Status (2026-11-03):** every "Fix first" and low item was fixed in #53, including the Lab 8 restructure. Items marked 🖐 are still open for the dry run (#37), notably Lab 8's new flow run interactively, Lab 10 MCP approval and GitHub MCP, and the Lab 15 marketplace install. The `validate/day2-dotnet` branch referenced below was local and has been deleted.

## Summary

Labs 8–15 were run on the .NET track on a local branch `validate/day2-dotnet` (not pushed; main untouched), starting from the participant state left by the Day 1 run. Code and config steps were executed for real; Claude-behavior claims were tested with fresh headless `claude -p` sessions (`/security-review`, `/security-checklist`, hooks, skills, subagent chain, output style, plugin load). Steps that write to the user's global config or need the user's GitHub token were not run (marked 🖐).

**One blocker (Lab 8). Everything else works**, with a handful of wording fixes. Hooks, skills, subagents, headless flags, session persistence, and plugin packaging all behaved as documented.

### Fix first

| # | Lab | Finding | Suggested fix |
|---|---|---|---|
| 1 | 8 (#41) | **`/security-review` doesn't report the planted cross-tenant bug.** Dropped at 3/10 (as written) and 7/10 (with a stronger, header-based tenant), below its 8/10 bar: no auth, GUID IDs, no listing endpoint. The lab's B.2/B.4 and a success criterion depend on it. The Part C skill **does** catch it, and so does Lab 13's BLOCK/OK diff review. | Reframe Lab 8 around the gap: run `/security-review` and read why it drops the finding → build `/security-checklist` → it finds the gap → fix → re-run both. Keep an `X-Organization-Id` caller in B.1. Route B.1 through `TaskService` to match the API rules. |
| 2 | 12 (#29) | **A.4 auto-delegation is preempted by the Lab 11 skill.** The exact prompt invokes `clean-architecture-boundary-check` instead of `architecture-reviewer`. | Say so in A.4 and use it as the skill-vs-subagent discussion; @-mention to force the agent. |
| 3 | 11 (#42) | Spring layer called `web/` in Lab 11 and `src-springboot/CLAUDE.md`, but the module/package is `taskmanager-api` / `…api`. | Use `api` (web) in both. |

### Low / polish
Lab 11: tell `test-data-generator` to use Glob/Grep (it reached for Bash `find`, outside its `allowed-tools`) · Lab 13: note `--max-turns` isn't in `--help` (but works) · Lab 15: add `description` to the example `marketplace.json` (validator warning); `block-git-push.sh` regex also matches `git pushx` · Lab 14: tip that `--settings '{"outputStyle":…}'` tries a style without persisting it · Lab 12: mention the chain's runtime/cost (~7 min, ~$1.50 headless).

### Worth adding as facilitator talking points (verified)
- **Headless runs skip `.mcp.json` approval.** After Lab 10, a plain `claude -p` connected the still-"pending approval" project server with all 14 tools (Lab 13 Part E point 5 is correct).
- Hooks block **regardless of mode**: the protect-files hook stopped an `acceptEdits` edit to `.devcontainer/`, and Claude stopped instead of retrying.
- The repo's own deny rules bite during the capstone (`rm -rf` blocked while building the plugin), which makes a good live Lab 9 callback.

---

Environment: Claude Code 2.1.317, .NET SDK 10.0.403 (net9.0), model Opus 5.5. Branch `validate/day2-dotnet` (local only) = participant's Day 1 state (Labs 1–7 validation run) merged with main @ b187757.
Legend: ✅ verified · ⚠️ issue · ❌ step doesn't work as written · 🖐 manual-only

## Lab 8 — Security Review Workflows (#41)

- ✅ Prerequisites: `origin/HEAD` → `origin/main`; branch has commits ahead.
- ✅ B.1 as written builds and commits (assign endpoint with existence check only, `OrganizationId` defaulted).
- ❌ **B.2: `/security-review` does not report the planted vulnerability.** Ran it twice headlessly:
  1. **As written:** the candidate was raised and **dropped at 3/10 confidence**. Reasons: the whole API is unauthenticated, `OrganizationId` is always the constant `"default-org"` ("no tenant to cross into"), and task IDs are unguessable GUIDs.
  2. **With a stronger B.1** (tasks stamped with a caller-supplied `X-Organization-Id`; assign reads the header but never compares it): **dropped at 7/10**, below the tool's 8/10 reporting bar, because GUID IDs can't be enumerated (no list endpoint) and the header is client-controlled anyway.
  `/security-review` deliberately filters low-exploitability findings, so in an app with no auth it will usually treat a missing tenant check as hardening rather than a vulnerability. B.2, B.4, and the success criterion "confirmed `/security-review` found it" won't hold reliably. **High; this is the core of the lab.**
- ✅ **Part C skill catches it.** Created `.claude/skills/security-checklist/SKILL.md` per the prompt (frontmatter with `disable-model-invocation: true` and scoped `allowed-tools` is accepted); `/security-checklist` ran headlessly and reported, as finding #1: "`AssignTaskAsync` checks that the task exists (`FindByIdAsync`) but never compares `organizationId` with `task.OrganizationId`", with file:line and a fix. It also correctly reported "No findings" for notification content (no sender implementations exist yet).
- **Recommended redesign:** make the gap the motivation for Part C, which matches how the tools actually behave. (1) B.2: run `/security-review` and **read why it drops or doesn't raise the assign endpoint**: generic review weighs exploitability, not your tenancy rules. (2) Move Part C (the skill) before the fix. (3) Run `/security-checklist`; it finds the existence-without-ownership gap. (4) B.3 fix, then re-run `/security-checklist` to confirm, and `/security-review` for a clean baseline. Keep the `X-Organization-Id` header in B.1 so the tenant concept is concrete. Update the deck, facilitator guide, and success criteria to match.
- ⚠️ B.1's .NET prompt has the endpoint call `ITaskRepository` directly, which contradicts `.claude/rules/dotnet.md` ("Endpoints map request DTO → command → Application service. No business logic in handlers"). The skill flagged it too. Claude may push back or route through `TaskService`. **Fix:** have B.1 add a `TaskService.AssignTaskAsync` (still without the ownership check). Low–medium.
- 🖐 Part A baseline run (skipped; same command as B.2), `/skills` listing, `security-guidance` / `claude-security` plugin install, the `claude-plugins-official` marketplace name, the GitHub Action.

## Lab 9 — Permissions, Safety & Hooks (#27)

- ✅ C.3 tip works as written: piping sample JSON into the shipped audit hook appends `Bash: ls` to `.claude/audit.log`.
- ✅ **B.1–B.2 live**: in headless Manual mode, `gh pr list` was **denied** without a rule and **ran** after adding `Bash(gh pr list *)` to `.claude/settings.json`, with no restart (settings are watched).
- ✅ **C.2 payload claims** (captured real stdin from a PreToolUse hook): fields `session_id`, `cwd`, `permission_mode`, `hook_event_name`, `tool_name`, `tool_input` are present (plus `effort`, `prompt_id`, `tool_use_id`, `transcript_path`). `tool_input.file_path` is absolute. There are **no `CLAUDE_TOOL_*` env vars**. Note: the sample JSON's `"permission_mode": "default"` is plausible (the live run showed `acceptEdits`); 🖐 confirm whether Manual reports `"default"` or `"manual"`.
- ✅ **C.4 protect-files hook**: the script from the guide passes the isolation test (`exit=2` / `exit=0`). **Live** in `acceptEdits` mode: the edit to `.devcontainer/README.md` was blocked, Claude reported the hook reason and stopped instead of retrying, and `git status .devcontainer` stayed clean. This confirms "regardless of permission mode".
- ✅ **C.5 format hook**: the script from the guide strips the absolute prefix correctly; `dotnet format whitespace … --include` fixed a deliberately misindented line (and normalized other whitespace in the file). About 2 s per `.cs` edit.
- ✅ **C.6 test-run one-liner**: runs the unit tests for a test-file path (exit 0, 40 passed) and is a no-op for other paths.
- ✅ Extension `Stop` hook jq expression is valid.
- 🖐 `/permissions`, `/hooks` browser, Shift+Tab cycle, trust dialog, `/sandbox`, B.4 `defaultMode` in `settings.local.json`, `Write(path)` rules "never match", `Notification` hook.

## Lab 10 — MCP Servers (#28)

- ✅ B.1: `claude mcp add --scope project filesystem -- npx -y @modelcontextprotocol/server-filesystem .` works; B.2's generated `.mcp.json` matches the guide **exactly** (`type: stdio`, args, empty `env`).
- ✅ B.3: `claude mcp list` reports `⏸ Pending approval (run claude to approve)` for the project-scoped server, which matches "asks you to approve it the first time".
- 🖐 **Not run: approving and using the server.** Approval is interactive; my attempt to pre-approve it via `enabledMcpjsonServers` in `settings.local.json` for a headless run was blocked by the session's safety classifier (it would download and run an external package). Needs the live dry run: approve, `/mcp` tool list (`read_text_file`, `list_directory`, `search_files`, `directory_tree`), the directory-tree prompt, and `mcp__filesystem__*` permission rules.
- ✅ C.4/C.5 targets exist: `centricconsulting/ai-coding-workshop` has `enhancement` issues; this repo has closed issues.
- 🖐 **Part C not run by design** (needs the user's PAT and writes to user-level config): hosted endpoint `https://api.githubcopilot.com/mcp/` with a PAT, Docker option, GitHub MCP tool names (#28's open question), `mcp__github__create_issue` / `merge_pull_request` names in the extension, resources/prompts (`@`, `/mcp__…`), `mcp-server-dev` plugin.
- ℹ️ Node v25 is fine for B (guide says 18+).

## Lab 11 — Skills (#42)

- ✅ B.1: the shipped `test-data-generator` matches the description: `arguments: [entity, count, format]`, `$entity/$count/$format/$ARGUMENTS` in the body, `allowed-tools: Read Grep Glob`, `when_to_use`, and both templates (`TaskTestData.cs`, `.java`).
- ✅ **B.2 live**: `/test-data-generator Task 5 csharp` read the *current* domain model (it picked up Day 1's `Priority`/`DueDate` and Lab 8's `Assign`/`OrganizationId`) and produced data built only with `Task.Create(...)` + business methods. Works as described.
- ✅ **B.3 live**: the plain-language prompt auto-invoked the skill (`Skill` tool call confirmed in the transcript).
- ⚠️ In one headless run the skill then tried a Bash `cd src-dotnet && find …`, which isn't covered by its `allowed-tools`; headless denied it and Claude fell back to hand-written data, describing it as "the skill errored". Interactively that's a permission prompt. **Fix:** in SKILL.md, say "use Glob/Grep to find the domain files" so it stays inside its pre-approved tools. Low.
- ✅ **C.4 live**: a `clean-architecture-boundary-check` skill built to the guide's frontmatter/body spec auto-activated on **both** prompts and got both right: **FAIL** for Domain calling Infrastructure (with the fix: Application service + Domain interface), **PASS** for Application calling the Domain `ITaskRepository`. Direct `/clean-architecture-boundary-check src-dotnet/src/TaskManager.Application` → PASS with csproj/`using` evidence. A new skill under an existing `.claude/skills/` was picked up without restarting.
- ⚠️ Spring layer naming: Lab 11 C.1 and `src-springboot/CLAUDE.md` call the outer layer `web/`, but the module and package are `taskmanager-api` / `com.example.taskmanager.api`. A skill keyed on "web" will misclassify paths. **Fix:** say `api` (web) in both. Low–medium.
- 🖐 `/skills` menu and the `t` token-cost sort (#42's open question), `/reload-skills`, `context: fork` extension, the `disable-model-invocation` extension.

## Lab 12 — Custom Subagents & Agent Workflows (#29)

- ✅ The A.1 table matches the shipped frontmatter (tools and models for all three agents); `architecture-reviewer` has the documented output headings.
- ✅ **B.1 chain live** (exact prompt, `acceptEdits` as the stand-in for approving the write): Claude delegated `backlog-generator` → `architecture-reviewer` → `test-strategist` **in order**, `docs/backlog/task-assignment-notifications.md` was written (6 stories, 24 pts), the review placed Assignee/notification port/endpoint per layer and caught the existing Lab 8 assign-handler issues, and the test plan followed. **It took ~7 min and cost ~$1.47** in one headless run, so it's worth telling facilitators (and making sure participants have quota). The 12-minute slot fits.
- ⚠️ **A.4 automatic delegation doesn't happen after Lab 11.** With the guide's exact prompt, Claude invoked Lab 11's `clean-architecture-boundary-check` **skill** instead of delegating to `architecture-reviewer`. Without the skill trigger (a shorter variant), it simply answered directly. The two descriptions now cover the same trigger, which is exactly what Lab 12's Troubleshooting warns about ("Make sure no other agent's description covers the same ground"). **Fix:** set the expectation in A.4: "If your Lab 11 skill answers instead, that's the overlap problem. Compare the two descriptions, or @-mention the agent." That turns it into a C.3 skill-vs-subagent discussion. Medium.
- ✅ **C.2 custom agent live**: `api-contract-checker` built to the spec (Read/Grep/Glob, `haiku`, fixed table output) was picked up by a new session and ran via `@agent-api-contract-checker`; it reported 8 real drifts between the stacks.
- 🖐 Part 0 Explore (`very thorough`) + `/tasks`, `@"name (agent)"` typeahead, background/Ctrl+B, Part D parallel Explore agents, `/agents` reminder text, extension items (`skills:`, `--agent`, `memory`, `isolation: worktree`).

## Lab 13 — Headless & Automation (#30)

- ✅ Flags in Part B exist in `claude --help`: `--allowedTools`, `--disallowedTools`, `--max-budget-usd`, `--append-system-prompt`, `--bare`, `--json-schema`, `-n/--name`.
- ⚠️ `--max-turns` is **not listed in `claude --help`** (2.1.317), although it's accepted and works (used throughout this run). Fine to teach; one parenthetical ("not shown in `--help`, but supported") would head off questions. Low.
- ✅ A.3 JSON envelope has `result`, `session_id`, `total_cost_usd`, `permission_denials` (the Troubleshooting reference).
- ✅ **C.1/C.2 live**: `--continue`, `--resume <session_id>`, and `-n <name>` + `--resume <name>` each recalled a code word from the previous run.
- ✅ **B.1 live**: `--permission-mode dontAsk --allowedTools "Read,Grep,Glob,Bash(dotnet test *)"` ran the tests and summarized them without edits.
- ✅ **D.1 pattern live**: piping the Lab 8 insecure-endpoint commit into the BLOCK/OK prompt returned **`BLOCK`** on line 1 (notably, where `/security-review` dropped the same change; worth citing in the Lab 8 redesign).
- ✅ **Part E point 5 confirmed (important):** with the Lab 10 `filesystem` server still "⏸ Pending approval" in `claude mcp list`, a plain `claude -p` run reported it **`connected` (source: project) with 14 `mcp__` tools**. Headless runs do skip the `.mcp.json` approval gate, exactly as the guide warns. Consider making this a facilitator talking point: "after Lab 10, every `claude -p` in the repo starts your project MCP servers".
- 🖐 `/install-github-app`, the GitHub Action workflows, `claude setup-token`, `--bare` auth requirement, `--json-schema` stretch.

## Lab 14 — Output Styles (#43, self-study)

- ✅ Premise holds: no `.claude/output-styles/` exists yet.
- ✅ **B.2 sample style works as written**: copied verbatim to `.claude/output-styles/code-reviewer.md` (with `keep-coding-instructions: true`) and activated for one headless run with `--settings '{"outputStyle":"code-reviewer"}'`. The review came back with no preamble, issues ranked by severity, one fix each, and still code-aware (cited rules, tests, conventions).
- ℹ️ `--settings '{"outputStyle":"…"}'` is a handy way to try a style without `/output-style` persisting it to `settings.local.json`; worth a tip in A.2 or Troubleshooting ("The style persists unexpectedly").
- 🖐 `/output-style` list/switch, `/config` picker, built-in style names (Default, Proactive, Concise, Explanatory, Learning), `Insight` blocks, persistence to `settings.local.json`.

## Lab 15 — Capstone: Team Config & Plugins (#31)

- ✅ **C.2 `block-git-push.sh`** (extracted verbatim): blocks `git push origin main` and `echo hi && git push` (exit 2), allows `git status` (exit 0). Minor: also blocks `git pushx` (regex not anchored at the end); harmless. Low.
- ✅ **F.1–F.2 plugin layout** built exactly as written (plugin.json, `agents/`, `skills/`, `hooks/hooks.json` using `${CLAUDE_PLUGIN_ROOT}`, `scripts/`): `claude plugin validate ./team-plugin` → **✔ Validation passed**.
- ✅ **F.3 namespacing**: a session-only `--plugin-dir` load reported the plugin (`taskmanager-team@inline`, v0.1.0) with agent `taskmanager-team:api-contract-checker` and command `taskmanager-team:clean-architecture-boundary-check`, exactly as described.
- ⚠️ **F.4 marketplace.json** from the guide validates **with a warning**: "No marketplace description provided". **Fix:** add a top-level `"description": "TaskManager team plugins"` to the example. Low.
- ℹ️ Building the plugin from Claude Code tripped the repo's own `Bash(rm -rf *)` deny rule when cleaning up a scratch folder, a good live example of Lab 9's rules in action.
- 🖐 Not run (they write to the user's plugin config): `claude plugin marketplace add`, `plugin install`, `plugin list`, uninstall/remove, `extraKnownMarketplaces`/`enabledPlugins` in project settings, the trust-prompted install. Also the handler `if` field (C.2), `/hooks`, `/plugin`, `/reload-plugins`, presentations.
