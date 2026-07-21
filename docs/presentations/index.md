# Workshop Presentations — Claude Code Edition

Presentations use [Marp](https://marp.app/) format. View with the [Marp VS Code extension](https://marketplace.visualstudio.com/items?itemName=marp-team.marp-vscode) or export to PDF.

---

## Part 1: Fundamentals (~3 hours)

| Module | File | Duration | Labs Covered |
|---|---|---|---|
| 00 Kickoff & Setup | [00-kickoff-and-setup.md](modules/part1/00-kickoff-and-setup.md) | 15 min | Lab 1 intro |
| 01 Claude Code Features Tour | [01-claude-code-features-tour.md](modules/part1/01-claude-code-features-tour.md) | 15 min | — |
| 02 CLAUDE.md & Project Memory | [02-claude-md-memory.md](modules/part1/02-claude-md-memory.md) | 20 min | Lab 2 |
| 03 TDD with Claude Code | [03-tdd-with-claude-code.md](modules/part1/03-tdd-with-claude-code.md) | 20 min | Lab 3 |
| 04 Refactoring & Context | [04-refactoring-and-context.md](modules/part1/04-refactoring-and-context.md) | 15 min | Lab 4 |
| 05 Git Workflow | [05-git-workflow.md](modules/part1/05-git-workflow.md) | 10 min | Lab 5 |
| 06 Part 1 Wrap-Up | [06-wrapup-part1.md](modules/part1/06-wrapup-part1.md) | 15 min | — |

**Part 1 Total**: ~3 hours including lab time

---

## Part 2: Advanced Claude Code (~3 hours)

| Module | File | Duration | Labs Covered |
|---|---|---|---|
| 00 Permissions, Safety & Hooks | [00-permissions-and-hooks.md](modules/part2/00-permissions-and-hooks.md) | 25 min | Lab 6 |
| 01 MCP Servers | [01-mcp-servers.md](modules/part2/01-mcp-servers.md) | 20 min | Lab 7 |
| 02 Sub-Agents & Parallel Workflows | [02-sub-agents.md](modules/part2/02-sub-agents.md) | 20 min | Lab 8 |
| 03 Headless & Automation | [03-headless-automation.md](modules/part2/03-headless-automation.md) | 15 min | Lab 9 |
| 04 Capstone — Team Configuration | [04-capstone-team-config.md](modules/part2/04-capstone-team-config.md) | 15 min | Lab 10 |

**Part 2 Total**: ~3 hours including lab time

---

## Workshop Delivery Patterns

### Full 6-Hour Workshop
Deliver both parts in one day with a break between. Recommended for dedicated workshop days.

### Two 3-Hour Sessions
Part 1 one week, Part 2 the following week. Participants can practice CLAUDE.md skills between sessions.

### Lunch & Learn: CLAUDE.md Essentials (1.5 hours)
- Module 00 Kickoff (15 min)
- Lab 1: First Contact (20 min)
- Module 02 CLAUDE.md (20 min)
- Lab 2: CLAUDE.md (30 min)
- Discussion (15 min)

### Advanced Session Only (3 hours)
For teams already comfortable with Claude Code basics. Skip Part 1, start with Part 2. Requires participants to have CLAUDE.md experience.

### Hooks & Automation Focus (2 hours)
- Module 01 Permissions (15 min)
- Lab 6: Permissions & Hooks (30 min)
- Module 05 Headless (10 min)
- Lab 9: Headless Mode (25 min)
- Discussion: CI Integration patterns (20 min)

---

## How to Use the Presentations

**In VS Code:**
1. Install [Marp extension](https://marketplace.visualstudio.com/items?itemName=marp-team.marp-vscode)
2. Open any `.md` file in `modules/`
3. Click the Marp preview icon (top-right of editor)

**Export to PDF:**
```bash
npx @marp-team/marp-cli docs/presentations/modules/part1/*.md --pdf
npx @marp-team/marp-cli docs/presentations/modules/part2/*.md --pdf
```

**Export to HTML:**
```bash
npx @marp-team/marp-cli docs/presentations/modules/part1/00-kickoff-and-setup.md --html
```
