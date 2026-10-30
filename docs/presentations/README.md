# Workshop Presentations

Presentations are written in [Marp](https://marp.app/) format — standard Markdown with a frontmatter block that Marp renders as slides.

---

## Viewing Presentations

### VS Code (recommended)

1. Install the [Marp for VS Code](https://marketplace.visualstudio.com/items?itemName=marp-team.marp-vscode) extension
2. Open any `.md` file in `modules/`
3. Click the **Marp preview icon** in the top-right corner of the editor (or use `Ctrl+Shift+P` → "Marp: Open Preview")

The preview updates live as you edit.

### Browser (export first)

See the export section below.

---

## Exporting Presentations

### Export all Day 1 slides to PDF

```bash
npx @marp-team/marp-cli docs/presentations/modules/part1/*.md --pdf --output-dir docs/presentations/pdf/part1
```

### Export all Day 2 slides to PDF

```bash
npx @marp-team/marp-cli docs/presentations/modules/part2/*.md --pdf --output-dir docs/presentations/pdf/part2
```

### Export a single module to HTML

```bash
npx @marp-team/marp-cli docs/presentations/modules/part1/01-claude-code-features-tour.md --html
```

### Export a single module to PDF

```bash
npx @marp-team/marp-cli docs/presentations/modules/part1/02-claude-md-memory.md --pdf
```

> **Note:** `npx @marp-team/marp-cli` requires Node.js 18+. If you use the maintainer devcontainer, Node.js LTS and Marp CLI are pre-installed, so you can run `marp` directly.

---

## Module Index

See [index.md](index.md) for the full list of modules, durations, and delivery patterns (full 2-day, two-session, and focused short sessions). Facilitators should also read the [Facilitator Guide](../../FACILITATOR_GUIDE.md).

### Day 1: Fundamentals

| File | Title | Duration |
|---|---|---|
| [00-kickoff-and-setup.md](modules/part1/00-kickoff-and-setup.md) | Kickoff & Setup | 15 min |
| [01-claude-code-features-tour.md](modules/part1/01-claude-code-features-tour.md) | Claude Code Features Tour | 15 min |
| [02-claude-md-memory.md](modules/part1/02-claude-md-memory.md) | CLAUDE.md & Project Memory | 20 min |
| [03-tdd-with-claude-code.md](modules/part1/03-tdd-with-claude-code.md) | TDD & Requirements → Code | 20 min |
| [04-plan-mode-extended-thinking.md](modules/part1/04-plan-mode-extended-thinking.md) | Plan Mode, Permission Modes & Thinking | 20 min |
| [05-refactoring-and-context.md](modules/part1/05-refactoring-and-context.md) | Refactoring & Context Management | 15 min |
| [06-checkpoints-and-rewind.md](modules/part1/06-checkpoints-and-rewind.md) | Checkpoints & Rewind | 15 min |
| [07-git-workflow.md](modules/part1/07-git-workflow.md) | Git Workflow & PR Preparation | 10 min |
| [08-security-review.md](modules/part1/08-security-review.md) | Security Review Workflows | 15 min |
| [09-wrapup-day1.md](modules/part1/09-wrapup-day1.md) | Day 1 Wrap-Up & Day 2 Preview | 15 min |

### Day 2: Advanced

| File | Title | Duration |
|---|---|---|
| [00-permissions-and-hooks.md](modules/part2/00-permissions-and-hooks.md) | Permissions, Safety & Hooks | 25 min |
| [01-mcp-servers.md](modules/part2/01-mcp-servers.md) | MCP Servers | 20 min |
| [02-skills.md](modules/part2/02-skills.md) | Skills | 20 min |
| [03-sub-agents.md](modules/part2/03-sub-agents.md) | Custom Subagents & Agent Workflows | 20 min |
| [04-headless-automation.md](modules/part2/04-headless-automation.md) | Headless & Automation | 15 min |
| [05-output-styles.md](modules/part2/05-output-styles.md) | Output Styles & Custom Personas | 15 min |
| [06-capstone-team-config.md](modules/part2/06-capstone-team-config.md) | Capstone — Team Configuration & Plugins | 15 min |

---

## Slide Format Reference

Each module follows this structure:

```markdown
---
marp: true
theme: default
paginate: true
backgroundColor: #fff
---

# Module Title

## Subtitle

---

# Slide Title

Content here.

---

<!-- Navigation -->
[← Previous](prev.md) | **Module N** | [Next →](next.md)
```

Slides are separated by `---`. The last slide always has navigation links.

---

## Facilitator Notes

Presenter notes (visible only in presenter view) use the HTML comment syntax:

```markdown
# Slide Title

Slide content here.

<!-- 
Presenter note: pause here for questions before moving on.
Expected question: "Does this work with my existing editor setup?"
-->
```

In Marp VS Code, use `Ctrl+Shift+P` → "Marp: Open Presenter View" to see notes during delivery.
