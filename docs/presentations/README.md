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

### Export all Part 1 slides to PDF

```bash
npx @marp-team/marp-cli docs/presentations/modules/part1/*.md --pdf --output-dir docs/presentations/pdf/part1
```

### Export all Part 2 slides to PDF

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

> **Note:** `npx @marp-team/marp-cli` requires Node.js 18+. If you use the maintainer devcontainer, Marp CLI is pre-installed as `marp`.

---

## Module Index

See [index.md](index.md) for the full list of modules, durations, and delivery patterns (full-day, two-session, lunch & learn).

### Part 1: Fundamentals

| File | Title | Duration |
|---|---|---|
| [00-kickoff-and-setup.md](modules/part1/00-kickoff-and-setup.md) | Kickoff & Setup | 15 min |
| [01-claude-code-features-tour.md](modules/part1/01-claude-code-features-tour.md) | Claude Code Features Tour | 15 min |
| [02-claude-md-memory.md](modules/part1/02-claude-md-memory.md) | CLAUDE.md & Project Memory | 20 min |
| [03-tdd-with-claude-code.md](modules/part1/03-tdd-with-claude-code.md) | TDD with Claude Code | 20 min |
| [04-refactoring-and-context.md](modules/part1/04-refactoring-and-context.md) | Refactoring & Context Management | 15 min |
| [05-git-workflow.md](modules/part1/05-git-workflow.md) | Git Workflow & PR Preparation | 10 min |

### Part 2: Advanced (coming soon)

| File | Title | Duration |
|---|---|---|
| `00-welcome-recap.md` | Welcome & Part 1 Recap | 10 min |
| `01-permissions-safety.md` | Permissions & Safety | 15 min |
| `02-hooks-automation.md` | Hooks for Automation | 10 min |
| `03-mcp-servers.md` | MCP Servers | 15 min |
| `04-sub-agents.md` | Sub-Agents & Parallelism | 15 min |
| `05-headless-automation.md` | Headless & Automation | 10 min |
| `06-capstone-governance.md` | Capstone & Governance | 10 min |

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
Presenter note: mention the comparison to GitHub Copilot here.
Expected question: "Can I use both tools?"
-->
```

In Marp VS Code, use `Ctrl+Shift+P` → "Marp: Open Presenter View" to see notes during delivery.
