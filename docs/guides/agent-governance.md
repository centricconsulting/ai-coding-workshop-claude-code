# Agent Governance Guide

## Managing Claude Code Configuration at Enterprise Scale

This guide covers how to maintain and govern Claude Code configuration across a team or organization — ownership, versioning, onboarding, secrets, and evolution over time.

---

## Who Owns What

Claude Code configuration lives in three files. Each has a different owner.

| File | Scope | Owner |
|---|---|---|
| `CLAUDE.md` (root and subdirs) | Project conventions and context | Engineering team via PR process |
| `.claude/settings.json` | Permissions, hooks, safety rules | Tech lead or platform team |
| `.mcp.json` | External tool integrations | Platform/DevX team |
| `~/.claude/settings.json` | Personal overrides | Individual developer |

**The project files belong to the codebase.** Changes go through the same PR review as code. The personal file belongs to the developer and is never committed.

---

## The PR Process for Configuration Changes

Treat `.claude/settings.json` and `.mcp.json` like infrastructure code — changes need review before they reach the team.

**Proposing a change:**
1. Open a branch and modify the file
2. In the PR description, answer:
   - What does this change allow or deny that wasn't allowed or denied before?
   - What's the motivation? (New workflow, security tightening, team feedback)
   - Have you tested the change locally?
3. At least one reviewer should verify the allow/deny logic — especially any new `allow` rules

**Who can approve:**
- Tech lead or designated DevX engineer for `settings.json` changes
- Platform team for `.mcp.json` changes (external system access)
- Any senior engineer for `CLAUDE.md` content changes

**Fast-track exceptions:**
- Adding a `deny` rule for a newly-discovered risk: merge without full review cycle if the tech lead approves
- Fixing a broken hook command: treat as a hotfix

---

## User-Level vs. Project-Level Settings

Project-level (`.claude/settings.json`) sets the floor for the whole team. User-level (`~/.claude/settings.json`) lets individuals extend it for their personal workflow.

**What belongs in project settings:**
- Deny rules for dangerous operations (these must be consistent across the team)
- Allow rules for standard build/test commands
- Audit log hooks
- Hooks that enforce team conventions (format-on-save, test-on-write)

**What belongs in user settings:**
- Personal allow rules for tools the individual uses (e.g., a specific linter)
- Personal hooks for local workflow preferences
- Anything that would be noise for teammates

**Key rule:** user settings can *add* permissions but should not override project `deny` rules. If a developer needs an exception to a project deny rule, that's a conversation — not a local override.

---

## Handling Secrets and Tokens

MCP servers and some hooks require credentials (API tokens, connection strings). Never put secrets in committed files.

**Pattern: environment variables in `.mcp.json`**

```json
{
  "mcpServers": {
    "github": {
      "command": "npx",
      "args": ["-y", "@modelcontextprotocol/server-github"],
      "env": {
        "GITHUB_PERSONAL_ACCESS_TOKEN": "${GITHUB_PERSONAL_ACCESS_TOKEN}"
      }
    }
  }
}
```

The `${VAR_NAME}` syntax reads from the shell environment — the token is never in the file.

**Where developers store their tokens:**
- Shell profile (`~/.zshrc`, `~/.bashrc`) for interactive use
- CI secrets store (GitHub Actions secrets, Azure Key Vault) for pipeline use
- A local `.env` file loaded by a dev script (never committed — add to `.gitignore`)

**Tokens should have minimum required scope.** A GitHub token for MCP read access doesn't need `write` or `admin` scope.

---

## Onboarding a New Developer

A new developer joining a project with an existing Claude Code configuration needs:

**Step 1: Install and authenticate Claude Code**
```bash
npm install -g @anthropic-ai/claude-code
claude  # follow the authentication prompts
```

**Step 2: Read the project configuration**
```
Read .claude/settings.json and CLAUDE.md and explain what Claude Code is configured to do in this project.
```

This is the fastest way to understand what's already set up.

**Step 3: Set up required environment variables**
Check `.mcp.json` for any `${VAR_NAME}` references and ensure those variables are set in the developer's shell.

**Step 4: Optionally configure user-level settings**
Developers can add personal preferences to `~/.claude/settings.json`. These don't affect teammates.

**Checklist to include in your team onboarding doc:**
- [ ] Claude Code installed and authenticated
- [ ] Required environment variables set
- [ ] `.claude/settings.json` reviewed and understood
- [ ] MCP servers reachable (restart Claude Code and verify with `List the available MCP tools`)
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
git log --oneline -- .claude/settings.json .mcp.json CLAUDE.md
```

**What "broke" looks like:**
- Claude Code starts prompting for permissions it didn't before (a missing `allow` rule)
- A hook starts failing silently (check hook command syntax with `jq . .claude/settings.json`)
- Claude Code produces output that doesn't match team conventions (a stale `CLAUDE.md`)

---

## Keeping CLAUDE.md Current

CLAUDE.md drifts from reality as the codebase evolves. A quarterly review is enough for most teams.

**Signs CLAUDE.md needs updating:**
- Claude Code suggests patterns you've moved away from
- New developers ask questions that CLAUDE.md should answer but doesn't
- A command in CLAUDE.md no longer works (package renamed, script moved)

**Practical approach:**
When you notice a gap, fix it immediately — open CLAUDE.md and add or correct the relevant section. A small, continuous update habit beats a quarterly cleanup sprint.

---

## Scale Considerations

For organizations with many teams using Claude Code:

**Shared base configuration**
Maintain a shared internal template repository with a baseline `settings.json` and root `CLAUDE.md` structure. Teams fork it and customize. This gives you consistent safety defaults (deny rules) without forcing identical workflows.

**MCP server hosting**
Internal MCP servers (Jira, Confluence, internal APIs) should be hosted centrally rather than run as local processes. Teams configure `.mcp.json` to point at the shared server URL rather than running `npx` locally.

**Audit logs**
The audit hook from Lab 9 becomes more important at scale. Route audit logs to a central collector (Splunk, Datadog) for visibility into what Claude Code is doing across the organization.

**Configuration drift detection**
Add a CI step that validates `.claude/settings.json` against a schema and checks that required deny rules are present. A missing `"Bash(rm -rf*)"` deny rule in a team's config is a risk worth catching in CI.

---

## Related Resources

- [Lab 15: Capstone — Design a Team Configuration](../labs/lab-15-capstone-team-config.md)
- [Claude Code Hooks documentation](https://docs.anthropic.com/en/docs/claude-code/hooks)
- [MCP documentation](https://docs.anthropic.com/en/docs/claude-code/mcp)
- [Claude Code Settings reference](https://docs.anthropic.com/en/docs/claude-code/settings)
