# Agent Governance Guide

## Managing Claude Code Configuration at Enterprise Scale

This guide covers how to maintain and govern Claude Code configuration across a team or organization — ownership, versioning, onboarding, secrets, plugins, organization-wide policy, and evolution over time.

---

## Who Owns What

Claude Code configuration lives in a handful of files and directories. Each has a different owner.

| File | Scope | Owner |
|---|---|---|
| `CLAUDE.md` (root and subdirs), `.claude/rules/` | Project conventions and context | Engineering team via PR process |
| `.claude/settings.json` | Permissions, hooks, enabled plugins, shared output style | Tech lead or platform team |
| `.mcp.json` | Project-scoped MCP servers (external tool integrations) | Platform/DevX team |
| `.claude/agents/`, `.claude/skills/`, `.claude/output-styles/` | Project subagents, skills, and output styles | Engineering team via PR process |
| Team plugin + marketplace repo | Shared agents, skills, hooks, MCP across many repos | Platform/DevX team |
| Managed settings (`managed-settings.json`, MDM, or server-managed) | Organization-wide policy that users can't override | Security / IT / platform team |
| `CLAUDE.local.md`, `.claude/settings.local.json` | Personal, per-project overrides (gitignored) | Individual developer |
| `~/.claude/settings.json`, `~/.claude.json` | Personal settings and user/local-scoped MCP servers | Individual developer |

**The project files belong to the codebase.** Changes go through the same PR review as code. Personal files belong to the developer and are never committed.

---

## The PR Process for Configuration Changes

Treat `.claude/settings.json`, `.mcp.json`, hook scripts, and plugins like infrastructure code — changes need review before they reach the team.

**Proposing a change:**
1. Open a branch and modify the file
2. In the PR description, answer:
   - What does this change allow or deny that wasn't allowed or denied before?
   - What's the motivation? (New workflow, security tightening, team feedback)
   - Have you tested the change locally? (`/permissions`, `/hooks`, `/mcp`, `claude plugin validate`)
3. At least one reviewer should verify the allow/deny logic — especially any new `allow` rules, new hook scripts, and new MCP servers

**Who can approve:**
- Tech lead or designated DevX engineer for `settings.json` and hook script changes
- Platform team for `.mcp.json` changes (external system access) and team plugin releases
- Any senior engineer for `CLAUDE.md`, rules, agent, and skill content changes

**Fast-track exceptions:**
- Adding a `deny` rule for a newly-discovered risk: merge without full review cycle if the tech lead approves
- Fixing a broken hook command: treat as a hotfix

---

## Settings Scopes and Precedence

Claude Code reads settings from several levels. Highest precedence first:

1. **Managed settings** — deployed by your organization; nothing in a user's files or `--settings` overrides a managed key
2. **Command line** — `--settings <file-or-json>` and flags for one session
3. **Project local** — `.claude/settings.local.json` (personal, gitignored)
4. **Shared project** — `.claude/settings.json` (committed)
5. **User** — `~/.claude/settings.json`

Scalar keys use the highest level that sets them. **List keys such as `permissions.allow` and `permissions.deny` merge across levels**, so each file can add entries without removing another's.

**Permission rules are evaluated deny → ask → allow.** A `deny` from *any* level wins over an `allow` from any other level, so a developer can't locally re-allow something the project denies. If they need an exception to a project deny rule, that's a conversation — not a local override. Organizations that want to go further can set `allowManagedPermissionRulesOnly` in managed settings so only managed allow/ask/deny rules apply.

**What belongs in project settings:**
- Deny rules for dangerous operations (these must be consistent across the team)
- Allow rules for standard build/test commands, e.g. `Bash(dotnet test *)`, `Bash(mvn test *)`
- Ask rules for consequential actions, e.g. `Bash(git push *)`
- Audit log hooks and hooks that enforce team conventions (format-on-edit, test-on-write)
- `enabledPlugins` / `extraKnownMarketplaces` for the team plugin

**What belongs in user or local settings:**
- Personal allow rules for tools the individual uses (e.g., a specific linter)
- Personal hooks and output style preferences
- Anything that would be noise for teammates

> **Workspace trust:** project `allow` rules, `extraKnownMarketplaces`, and most `env` values only take effect after each developer trusts the folder. Project `deny` and `ask` rules apply immediately.

---

## Hooks as Policy

Hooks let you enforce rules that need logic. When you write or review a hook, check:

- **Input arrives as JSON on stdin** — scripts should parse it with `jq` (e.g. `.tool_input.command`, `.tool_input.file_path`).
- **Exit 2 blocks** (for blocking events such as `PreToolUse`), and stderr is shown to Claude as the reason. **Exit 1 is a non-blocking error** — the action still runs. A "blocking" hook that uses `exit 1` is a silent policy gap.
- For richer decisions, a `PreToolUse` hook can exit 0 and print JSON with `hookSpecificOutput.permissionDecision` set to `"allow"`, `"ask"`, or `"deny"` plus a `permissionDecisionReason`. Deny and ask *rules* are still evaluated regardless of what the hook returns.
- Reference scripts with `"$CLAUDE_PROJECT_DIR"/.claude/hooks/...` (or `${CLAUDE_PLUGIN_ROOT}` inside a plugin) so they work from any working directory.
- Hooks run with the developer's credentials. Review hook scripts with the same care as CI scripts.

Organizations can set `allowManagedHooksOnly` in managed settings to restrict which hooks run to those delivered by managed policy (and approved plugins).

---

## Handling Secrets and Tokens

MCP servers and some hooks require credentials (API tokens, connection strings). Never put secrets in committed files.

**Pattern: environment variable expansion in `.mcp.json`**

`.mcp.json` supports `${VAR}` and `${VAR:-default}` in `command`, `args`, `env`, `url`, and `headers`. Example using GitHub's official MCP server:

```json
{
  "mcpServers": {
    "github": {
      "type": "http",
      "url": "https://api.githubcopilot.com/mcp/",
      "headers": {
        "Authorization": "Bearer ${GITHUB_MCP_PAT}"
      }
    }
  }
}
```

The token is read from the developer's environment at startup — it's never in the file. (The old `@modelcontextprotocol/server-github` npm package is archived; use GitHub's official server, either the hosted endpoint above or the `ghcr.io/github/github-mcp-server` Docker image.)

**Where developers store their tokens:**
- Shell profile (`~/.zshrc`, `~/.bashrc`) or a secrets manager CLI for interactive use
- CI secrets store (GitHub Actions secrets, Azure Key Vault) for pipeline use
- A local `.env` file loaded by a dev script (never committed — add to `.gitignore`)

**Protect secrets from Claude itself** with deny rules such as `Read(./.env)`, `Read(./.env.*)`, and `Read(./secrets/**)`.

**Tokens should have minimum required scope.** A GitHub token for MCP read access doesn't need `write` or `admin` scope — prefer fine-grained PATs limited to specific repositories.

---

## Governing MCP Servers

- **Scopes:** `local` (default, just you, this project — stored in `~/.claude.json`), `project` (`.mcp.json`, committed), and `user` (you, every project). Only `project` scope is shared through the repo.
- **Approval:** each developer approves project-scoped servers from `.mcp.json` the first time they're seen. `claude mcp reset-project-choices` resets those choices.
- **Permissions:** MCP tools are named `mcp__<server>__<tool>` and can be allowed, asked, or denied like any other tool, e.g. `"deny": ["mcp__github__merge_pull_request"]`.
- **Organization control:** managed policy can supply a fixed set of servers (`managed-mcp.json`) and allowlist/denylist servers users may add (`allowedMcpServers` / `deniedMcpServers`).
- **Transports:** prefer `http` for remote servers; `sse` is deprecated.

---

## Distributing Configuration with Plugins

A single repo's `.claude/` folder doesn't scale across dozens of repositories. **Plugins** bundle skills, subagents, hooks, MCP servers, and output styles into one installable unit, and **marketplaces** catalog them.

- A plugin is a directory with `.claude-plugin/plugin.json` plus `skills/`, `agents/`, `hooks/hooks.json`, `.mcp.json`, and/or `output-styles/` at its root.
- A marketplace is a git repo (or directory) with `.claude-plugin/marketplace.json` listing plugins and their sources.
- Developers install with `claude plugin marketplace add your-org/your-marketplace` and `claude plugin install <plugin>@<marketplace>` (or `/plugin` in a session).
- Validate in CI with `claude plugin validate <path>`.
- Plugin skills and agents are namespaced (`/<plugin>:<skill>`), so they don't collide with repo-local ones.

**Enable a team plugin for a repository** by committing to `.claude/settings.json`:

```json
{
  "extraKnownMarketplaces": {
    "your-org-tools": {
      "source": { "source": "github", "repo": "your-org/claude-plugins" }
    }
  },
  "enabledPlugins": {
    "team-standards@your-org-tools": true
  }
}
```

**Plugins run code** (hooks and MCP servers). Treat third-party plugins like any other dependency: review before installing, pin versions where possible, and restrict which marketplaces are allowed (see below).

---

## Organization-Wide Policy: Managed Settings

Managed settings are deployed by IT/security and sit at the top of the precedence stack. They can be delivered as a `managed-settings.json` file in a system directory, through MDM/OS-level policy, or as server-managed settings from the Claude admin console.

Common managed controls:

| Goal | Managed setting |
|---|---|
| Enforce deny rules everywhere | `permissions.deny` (can't be overridden by users) |
| Only allow org-defined permission rules | `allowManagedPermissionRulesOnly` |
| Prevent `--dangerously-skip-permissions` / bypass mode | `permissions.disableBypassPermissionsMode` |
| Only run org-approved hooks | `allowManagedHooksOnly` |
| Restrict which plugin marketplaces are allowed | `strictKnownMarketplaces` (allowlist), `blockedMarketplaces` (blocklist) |
| Pre-install and require team plugins | `extraKnownMarketplaces` + `enabledPlugins` |
| Block a specific plugin | `enabledPlugins: { "plugin@marketplace": false }` |
| Control MCP servers | `managed-mcp.json`, `allowedMcpServers`, `deniedMcpServers` |
| Enforce an output style | `outputStyle` |

Keep managed settings minimal — policy that must hold for everyone — and leave team-specific workflow choices to project configuration. See the [settings](https://code.claude.com/docs/en/settings) and [plugin management for organizations](https://code.claude.com/docs/en/plugins/org) docs for the full key list.

---

## Onboarding a New Developer

A new developer joining a project with an existing Claude Code configuration needs:

**Step 1: Install and authenticate Claude Code**
```bash
curl -fsSL https://claude.ai/install.sh | bash   # macOS/Linux/WSL (native installer, auto-updates)
# Windows PowerShell: irm https://claude.ai/install.ps1 | iex
# npm install -g @anthropic-ai/claude-code still works as an alternative
claude  # follow the authentication prompts
```

Run `claude doctor` if anything looks wrong.

**Step 2: Read the project configuration**
```
Read CLAUDE.md, .claude/rules/, .claude/settings.json, and .mcp.json and explain what Claude Code is configured to do in this project.
```

This is the fastest way to understand what's already set up. `/memory`, `/permissions`, `/hooks`, and `/mcp` show the live state.

**Step 3: Set up required environment variables**
Check `.mcp.json` for any `${VAR_NAME}` references and ensure those variables are set in the developer's shell.

**Step 4: Trust the folder, approve MCP servers, and install the team plugin**
Approve project MCP servers when prompted. If the repo declares a team marketplace, install the plugin when prompted (or via `/plugin`).

**Step 5: Optionally configure personal settings**
Developers can add preferences to `~/.claude/settings.json` or `.claude/settings.local.json`. These don't affect teammates.

**Checklist to include in your team onboarding doc:**
- [ ] Claude Code installed and authenticated (`claude doctor` passes)
- [ ] Required environment variables set
- [ ] CLAUDE.md, rules, and `.claude/settings.json` reviewed and understood
- [ ] MCP servers connected (`/mcp` shows them as connected)
- [ ] Team plugin installed (`/plugin` → Installed)
- [ ] First task run successfully

---

## Versioning and Rollback

Because configuration files live in the repo, versioning is free — it's just git.

**Tagging a stable configuration:**
```bash
git tag claude-config-v1.0 -m "Baseline team configuration after workshop"
```

**Rolling back a bad change:**
```bash
git revert <commit-that-broke-things>
```

**Auditing what changed:**
```bash
git log --oneline -- .claude/ .mcp.json CLAUDE.md
```

**Team plugins** have their own version (`version` in `plugin.json`). Bump it on each release so teammates pick up updates deliberately; roll back by reverting the marketplace repo or pinning the previous version.

**What "broke" looks like:**
- Claude Code starts prompting for permissions it didn't before (a missing `allow` rule — check `/permissions`)
- A hook starts failing or stops blocking (validate with `jq . .claude/settings.json`, check `/hooks`, and run `claude --debug`)
- An MCP server shows as failed in `/mcp` (missing environment variable or expired token)
- Claude Code produces output that doesn't match team conventions (a stale `CLAUDE.md` or rule)

---

## Keeping CLAUDE.md Current

CLAUDE.md drifts from reality as the codebase evolves. A quarterly review is enough for most teams.

**Signs CLAUDE.md needs updating:**
- Claude Code suggests patterns you've moved away from
- New developers ask questions that CLAUDE.md should answer but doesn't
- A command in CLAUDE.md no longer works (package renamed, script moved)
- The root file has grown long — move stack-specific detail into path-scoped `.claude/rules/` files

**Practical approach:**
When you notice a gap, fix it immediately — open CLAUDE.md (or `/memory`) and add or correct the relevant section. A small, continuous update habit beats a quarterly cleanup sprint.

---

## Scale Considerations

For organizations with many teams using Claude Code:

**Shared base configuration via plugins**
Publish a baseline team plugin (standard subagents, skills, guardrail hooks) through an internal marketplace. Teams enable it in their repos and add project-specific configuration on top. Use managed settings only for the non-negotiable policy layer (deny rules, allowed marketplaces).

**MCP server hosting**
Internal MCP servers (Jira, Confluence, internal APIs) should be hosted centrally as remote `http` servers rather than run as local processes. Teams configure `.mcp.json` to point at the shared server URL rather than running `npx` locally.

**Audit logs**
The audit hook from Lab 9 becomes more important at scale. Route audit logs to a central collector (Splunk, Datadog) for visibility into what Claude Code is doing across the organization. Claude Code's OpenTelemetry support is another option for usage and cost monitoring.

**Configuration drift detection**
Add a CI step that validates `.claude/settings.json` (against the published schema at `https://json.schemastore.org/claude-code-settings.json`), runs `claude plugin validate` on team plugins, and checks that required deny rules are present. A missing `"Bash(rm -rf *)"` deny rule in a team's config is a risk worth catching in CI.

---

## Related Resources

- [Lab 15: Capstone — Design a Team Configuration](../labs/lab-15-capstone-team-config.md)
- [Customization Decision Guide](customization-decision-guide.md)
- [Claude Code settings](https://code.claude.com/docs/en/settings)
- [Claude Code permissions](https://code.claude.com/docs/en/permissions)
- [Claude Code hooks reference](https://code.claude.com/docs/en/hooks)
- [MCP in Claude Code](https://code.claude.com/docs/en/mcp)
- [Plugins](https://code.claude.com/docs/en/plugins) and [plugin marketplaces](https://code.claude.com/docs/en/plugin-marketplaces)
- [Managing plugins for your organization](https://code.claude.com/docs/en/plugins/org)
