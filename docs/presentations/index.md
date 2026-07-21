# Workshop Presentations — Claude Code Edition

Presentations use [Marp](https://marp.app/) format. View with the [Marp VS Code extension](https://marketplace.visualstudio.com/items?itemName=marp-team.marp-vscode) or export to PDF.

---

## Day 1: Fundamentals (~6 hours)

| Module | File | Duration | Labs Covered |
|---|---|---|---|
| 00 Kickoff & Setup | [00-kickoff-and-setup.md](modules/part1/00-kickoff-and-setup.md) | 15 min | Lab 1 intro |
| 01 Claude Code Features Tour | [01-claude-code-features-tour.md](modules/part1/01-claude-code-features-tour.md) | 15 min | — |
| 02 CLAUDE.md & Project Memory | [02-claude-md-memory.md](modules/part1/02-claude-md-memory.md) | 20 min | Lab 2 |
| 03 TDD with Claude Code | [03-tdd-with-claude-code.md](modules/part1/03-tdd-with-claude-code.md) | 20 min | Lab 3 |
| 04 Plan Mode & Extended Thinking | [04-plan-mode-extended-thinking.md](modules/part1/04-plan-mode-extended-thinking.md) | 20 min | Lab 4 |
| 05 Refactoring & Context | [05-refactoring-and-context.md](modules/part1/05-refactoring-and-context.md) | 15 min | Lab 5 |
| 06 Checkpoints & Rewind | [06-checkpoints-and-rewind.md](modules/part1/06-checkpoints-and-rewind.md) | 15 min | Lab 6 |
| 07 Git Workflow | [07-git-workflow.md](modules/part1/07-git-workflow.md) | 10 min | Lab 7 |
| 08 Security Review | [08-security-review.md](modules/part1/08-security-review.md) | 15 min | Lab 8 |
| 09 Day 1 Wrap-Up | [09-wrapup-day1.md](modules/part1/09-wrapup-day1.md) | 15 min | — |

**Day 1 Total**: ~6 hours including lab time

---

## Day 2: Advanced Claude Code (~6 hours)

| Module | File | Duration | Labs Covered |
|---|---|---|---|
| 00 Permissions, Safety & Hooks | [00-permissions-and-hooks.md](modules/part2/00-permissions-and-hooks.md) | 25 min | Lab 9 |
| 01 MCP Servers | [01-mcp-servers.md](modules/part2/01-mcp-servers.md) | 20 min | Lab 10 |
| 02 Skills | [02-skills.md](modules/part2/02-skills.md) | 20 min | Lab 11 |
| 03 Sub-Agents & Parallel Workflows | [03-sub-agents.md](modules/part2/03-sub-agents.md) | 20 min | Lab 12 |
| 04 Headless & Automation | [04-headless-automation.md](modules/part2/04-headless-automation.md) | 15 min | Lab 13 |
| 05 Output Styles | [05-output-styles.md](modules/part2/05-output-styles.md) | 15 min | Lab 14 |
| 06 Capstone — Team Configuration | [06-capstone-team-config.md](modules/part2/06-capstone-team-config.md) | 15 min | Lab 15 |

**Day 2 Total**: ~6 hours including lab time

---

## Workshop Delivery Patterns

### Full 2-Day Workshop (12 hours)
Deliver Day 1 and Day 2 on consecutive days. Recommended default — matches the labs' Day 1/Day 2 framing and gives each day room for breaks and Q&A.

### Two Separate Sessions
Day 1 one week, Day 2 the following week. Participants can practice CLAUDE.md, Plan Mode, and checkpoint/rewind skills between sessions.

### Lunch & Learn: CLAUDE.md Essentials (1.5 hours)
- Module 00 Kickoff (15 min)
- Lab 1: First Contact (20 min)
- Module 02 CLAUDE.md (20 min)
- Lab 2: CLAUDE.md (30 min)
- Discussion (15 min)

### Advanced Session Only (6 hours)
For teams already comfortable with Claude Code basics. Skip Day 1, start with Day 2. Requires participants to have CLAUDE.md experience.

### Hooks & Automation Focus (2 hours)
- Module 00 Permissions & Hooks (25 min)
- Lab 9: Permissions & Hooks (25 min)
- Module 04 Headless (15 min)
- Lab 13: Headless Mode (25 min)
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
