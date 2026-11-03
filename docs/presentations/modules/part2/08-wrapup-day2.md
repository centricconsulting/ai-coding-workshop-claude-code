---
marp: true
theme: default
paginate: true
backgroundColor: #fff
---

# Day 2 Wrap-Up

## Workshop Complete. Now Make It Your Team's.

---

# What You Practiced on Day 2 (1/2)

| Lab | Skill | Key Takeaway |
|---|---|---|
| Lab 8 | Security review | A dedicated security gate, plus a skill with your domain's risks |
| Lab 9 | Permissions & hooks | The deny list matters as much as the allow list; hooks are guarantees |
| Lab 10 | MCP servers | MCP adds capabilities Claude doesn't have; hooks add side effects |
| Lab 11 | Skills | A packaged, on-demand procedure — not always-on context |

---

# What You Practiced on Day 2 (2/2)

| Lab | Skill | Key Takeaway |
|---|---|---|
| Lab 12 | Subagents | Each specialist gets its own context and tools; chain the handoffs |
| Lab 13 | Headless & CI | No human in the loop, so permissions must be restrictive |
| Lab 14 | Output styles *(self-study)* | Changes *how* Claude communicates, not what it knows |
| Lab 15 | Team config & plugins | One shared configuration multiplies value across the team |

---

# Day 2 Command Cheat Sheet

<style scoped>
section { font-size: 22px; }
</style>

| Need | Command |
|---|---|
| Security pass on your branch | `/security-review`, `/security-checklist` (your skill) |
| Permission rules & modes | `/permissions`, `Shift+Tab`, `claude --permission-mode plan` |
| Hooks | `/hooks` (defined in `.claude/settings.json`) |
| MCP servers | `/mcp`, `claude mcp add`, `claude mcp list` |
| Skills | `/skills`, `/<skill-name>`, `/reload-skills` |
| Subagents | `/agents`, `/tasks`, `claude --agent <name>` |
| Headless & CI | `claude -p`, `--output-format json`, `anthropics/claude-code-action@v1` |
| Output style | `/output-style`, `/output-style default` |
| Plugins | `/plugin`, `claude plugin validate`, `claude --plugin-dir <dir>` |

---

# Which Mechanism? Five Questions

1. **Guarantee or suggestion?** If it must always hold → **permission rule** or **hook**
2. **New capability?** Claude needs to reach a new system → **MCP server**
3. **How often does it apply?** Always → **CLAUDE.md** · some files → **rule with `paths:`** · one job → **skill** or **subagent**
4. **Needs isolation?** Own context, fewer tools, other model → **subagent**
5. **Who else needs it?** Other repos or teams → **plugin**

Full flowchart: [Customization Decision Guide](../../../guides/customization-decision-guide.md#decision-flow)

---

# The Mental Model Shift

**After Day 1:**
> "I direct a workflow. Claude Code executes it."

**After Day 2:**
> "I design the environment the agent works in — its memory, boundaries, tools, and specialists — and share it with my team."

Day 1 made *you* effective. Day 2 makes that repeatable for everyone who opens the repo.

---

# Rolling It Out to a Team

<style scoped>
section { font-size: 27px; }
</style>

**Start in the repo.** Commit `CLAUDE.md`, `.claude/rules/`, `.claude/settings.json`, agents, and skills. Review changes to them like code.

**Share across repos with a plugin.** Package the specialists that proved useful, and publish them in a team marketplace.

**Put must-hold policy in managed settings.** Deny rules, a disabled bypass mode, and approved marketplaces belong at the org level.

**Assign an owner.** Someone keeps `CLAUDE.md` current and reviews `.claude/` changes.

See the [Agent Governance Guide](../../../guides/agent-governance.md).

---

# Where Does Each Setting Belong?

| Scope | Put here | Example |
|---|---|---|
| **Managed** (org) | Policy that must hold for everyone | Deny reading `.env`; block bypass mode; allowed marketplaces |
| **Project** (`.claude/`) | Team workflow for this repo | Conventions, path rules, format-on-edit hook, shared agents |
| **Personal** (`~/.claude/`) | Your own preferences | Output style, extra allow rules, personal skills |

Keep managed settings minimal. Most of what you built in this workshop is project configuration.

---

# Discussion

- Which customization would you adopt on your team **on Monday**?
- What would you put in **managed** settings vs. project settings?
- Which repetitive task on your team deserves a skill or a subagent first?

---

# Your First Week Back

1. Add a `CLAUDE.md` and one path-scoped rule to a real project
2. Add one guard: a deny rule or a blocking hook for something that must never happen
3. Run `/code-review` and `/security-review` before every PR
4. Ship one specialist — a skill or subagent — in a team plugin
5. Work through **Lab 14 (Output Styles)** on your own

Lab 15's plugin is your starting point.

---

# Resources

**This repo**
- [Customization Decision Guide](../../../guides/customization-decision-guide.md) · [Agent Design Guide](../../../guides/agent-design-guide.md) · [Agent Governance Guide](../../../guides/agent-governance.md)
- [Lab guides](../../../labs/README.md) — every lab works as self-study

**Claude Code docs**
- [Overview](https://code.claude.com/docs/en/overview) · [Settings](https://code.claude.com/docs/en/settings) · [Hooks](https://code.claude.com/docs/en/hooks) · [Plugins](https://code.claude.com/docs/en/plugins) · [Plugin marketplaces](https://code.claude.com/docs/en/plugin-marketplaces)
- CI: the `anthropics/claude-code-action@v1` GitHub Action (Lab 13)

---

# Thank You

Over two days you went from first contact to a team configuration you can ship:
project memory, TDD from user story to endpoint, planning, refactoring, git and
security gates, permissions and hooks, MCP, skills, subagents, automation, and plugins.

**Questions?**

---

<!-- Navigation -->
[← 15 Capstone](07-capstone-team-config.md) | **Day 2 Wrap-Up** | Workshop Complete
