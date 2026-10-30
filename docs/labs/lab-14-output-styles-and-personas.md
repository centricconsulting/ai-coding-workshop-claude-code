# Lab 14: Output Styles & Custom Personas

**Duration**: 20–25 minutes
**Day**: 2 of 2 (Advanced)

**Learning Objectives**:
- Understand output styles as a persistent, system-prompt-level customization
- Distinguish an output style from a one-off prompt instruction
- Try the built-in output styles (Proactive, Concise, Explanatory, Learning)
- Create and activate a custom output style, including the `keep-coding-instructions` frontmatter
- Evaluate when to standardize an output style at the team level

---

## Overview

Every customization mechanism so far — CLAUDE.md, hooks, MCP, Skills — changes what Claude Code *knows* or *can do*. An **output style** changes something different: how Claude Code *communicates* — its tone, verbosity, and default behavior — persistently, across every response, until you switch it off.

This is the last new mechanism before the capstone. After this lab you'll have the complete toolkit — CLAUDE.md, hooks, MCP, Skills, Subagents, and output styles — to design the full team configuration (and package it as a plugin) in Lab 15.

---

## Prerequisites

- Completed Labs 1–13

---

## Part A: What Is an Output Style? (6 min)

### A.1 Persistent vs. One-Off

You already know how to change Claude Code's behavior for a single prompt:

```
Answer tersely — one paragraph, no preamble.
```

That works for one response. Say it again next turn, or Claude Code drifts back to default behavior. An **output style** is the same kind of instruction, but written once and applied at the system-prompt level for the whole session — and every session after, once activated — until you explicitly switch styles.

### A.2 Where Output Styles Live

```
.claude/output-styles/
  <style-name>.md      ← one file per style
```

Styles can also live in `~/.claude/output-styles/` (personal, all projects), in managed policy settings (organization-wide), or inside a plugin.

Activate a style with the `/output-style` command (the name is case-insensitive):

```
/output-style code-reviewer
```

Run `/output-style` with no argument to list styles and see the current one, or use `/config` → **Output style** for a picker. Switch back to default the same way:

```
/output-style default
```

Your choice is saved as `"outputStyle"` in `.claude/settings.local.json` for this project, and takes effect from your **next message**. To make a style your default everywhere, set `outputStyle` in `~/.claude/settings.json`.

### A.3 Built-In Styles

Claude Code ships with four styles besides **Default**:

| Built-in | What changes |
|---|---|
| **Proactive** | Starts work immediately and makes reasonable assumptions instead of asking about routine decisions (doesn't change your permission mode) |
| **Concise** | Leads with the result; no preamble, narration, or recap |
| **Explanatory** | Adds short `★ Insight` blocks explaining the choices behind the code |
| **Learning** | Like Explanatory, but leaves `TODO(human)` spots for *you* to write the interesting parts |

Try one now:

```
/output-style explanatory
```

```
Add a guard clause to TaskService so a completed task can't be completed twice.
```

Note the `Insight` blocks, then switch back with `/output-style default` and discard the change (`/rewind` or `git checkout .`).

### A.4 Team Use Cases

| Style | Use Case |
|---|---|
| **Terse CI-log mode** | Headless scripts (Lab 13) where output feeds a log or another script — no conversational filler (start from the built-in **Concise**; `/output-style` also works in `claude -p`) |
| **Teaching / explain-everything** | Onboarding a junior developer — every response explains *why*, not just *what* |
| **Strict-reviewer persona** | Code review passes — blunt, opinionated, no hedging |
| **Customer-facing summary** | Non-technical stakeholders — no jargon, outcome-focused |

Output styles are a good fit when the *same* tone should apply across many different prompts and many different sessions. If it's a one-off, just say it in the prompt.

**Output style vs. the other mechanisms:**

| You want | Use |
|---|---|
| Every response in a certain voice/format, or Claude in a different role | Output style |
| Claude to know project conventions | CLAUDE.md / `.claude/rules/` |
| Instructions for one kind of task (e.g. a review checklist) | Skill |
| Something guaranteed to happen every time | Hook |
| A focused helper with its own prompt, tools, and model | Subagent |

> Output styles apply to the main conversation only — subagents use their own system prompts, so a style doesn't change how, say, the `architecture-reviewer` subagent responds.

---

## Part B: Build the First Output Style in This Repo (10 min)

No `.claude/output-styles/` directory exists anywhere in this repo yet.

### B.1 Establish a Baseline

Before creating the style, run a prompt under default behavior so you have something to compare against:

```
Review src-dotnet/src/TaskManager.Infrastructure/Legacy/LegacyTaskProcessor.cs and tell me what's wrong with it.
```

Note the tone: default Claude Code responses tend to be balanced, hedge with caveats, and explain reasoning at moderate length.

### B.2 Create the Style

```
Create .claude/output-styles/code-reviewer.md. It should define a terse, blunt senior-reviewer persona: short sentences, no hedging, no restating the question, issues ranked by severity, and a one-line fix suggested for each issue. Show me the file before writing it.
```

Expect something in the neighborhood of:

```markdown
---
name: code-reviewer
description: Terse, blunt senior-reviewer persona for code review passes.
keep-coding-instructions: true
---

You are a blunt senior engineer doing a code review. Rules:

- No preamble, no restating the question, no "Great question!"
- Lead with the most severe issue first
- One line per issue: what's wrong, why it matters, the fix
- No hedging ("might", "could potentially") — state the problem directly
- If the code is fine, say so in one sentence and stop
- Never apologize or soften criticism with praise sandwiches
```

**Frontmatter fields** (all optional):

| Field | Meaning |
|---|---|
| `name` | Style name shown in `/output-style` and `/config`. Defaults to the file name |
| `description` | Shown in the picker |
| `keep-coding-instructions` | `true` keeps Claude Code's built-in software-engineering instructions (how to scope changes, verify work, etc.) alongside your style. **Default `false`** — a custom style *replaces* them. Set `true` whenever Claude will still be writing or reviewing code |

> Without `keep-coding-instructions: true`, a custom style turns Claude into a general assistant with your persona — great for a writing or data-analysis style, risky for a coding persona.

### B.3 Activate It and Re-Run the Same Prompt

Claude Code reads style files at startup, so **restart Claude Code** (`/exit`, then `claude --continue`) after creating the file. Then:

```
/output-style code-reviewer
```

```
Review src-dotnet/src/TaskManager.Infrastructure/Legacy/LegacyTaskProcessor.cs and tell me what's wrong with it.
```

### B.4 Compare Side by Side

Put the two responses next to each other and note concretely what changed: length, hedging language, structure, whether it restates the question. This is the evidence that the style is a real, measurable behavior change — not just a cosmetic label.

```
/output-style default
```

---

## Part C: Standardizing Output Styles at the Team Level (5 min)

Like Skills, an output style saved in `.claude/output-styles/` is versioned with the repo and shared with everyone who clones it. Unlike Skills, an output style is a *default persona for the whole session* — which raises a genuinely different governance question than "should this skill exist."

Discuss with your table:

- **Should the team have one default output style, or is this purely individual preference?** A terse CI mode makes sense to standardize (Lab 13's headless scripts benefit from everyone using the same one). A teaching-mode persona might be better left as a personal, user-level choice.
- **Where should a shared style live?** Project-level `.claude/output-styles/` (shared) vs. `~/.claude/output-styles/` (personal) vs. a team plugin — the same question the [Agent Governance Guide](../guides/agent-governance.md) raises for project vs. user settings.
- **Should the style be *activated* for everyone?** Committing the style file only makes it *available*. Activating it for the whole team means committing `"outputStyle": "code-reviewer"` in `.claude/settings.json` — a much stronger decision. (Plugins can even force a style with `force-for-plugin: true`.)
- **Who decides the default?** If nobody activates a custom style, everyone gets Claude Code's default tone — is that itself a decision that needs to be made deliberately for a team, or is it fine to leave unset?

This is direct prep for Lab 15: when you design a full team configuration, you'll decide whether output styles are part of it, and if so, which ones and why.

---

## Success Criteria

- [ ] You tried at least one built-in style (Proactive, Concise, Explanatory, or Learning)
- [ ] `.claude/output-styles/code-reviewer.md` exists, defines a clear persona, and sets `keep-coding-instructions: true`
- [ ] You activated it with `/output-style code-reviewer` and confirmed Claude Code's tone changed
- [ ] You ran the same prompt under default and custom styles and can point to concrete differences
- [ ] You can articulate the team-vs-individual tradeoff for standardizing an output style

---

## Extension Challenge

Create a second output style, `.claude/output-styles/teaching-mode.md`, that does the opposite of `code-reviewer`: verbose, explains reasoning step by step, defines any technical term on first use. Run the same LegacyTaskProcessor.cs prompt under this style and compare all four responses (default, built-in Explanatory, code-reviewer, teaching-mode) side by side. What does your custom style add that Explanatory/Learning don't?

**Stretch:** create a non-coding style (e.g. `release-notes-writer.md`) *without* `keep-coding-instructions` and observe how Claude's behavior shifts away from software-engineering habits.

---

## Troubleshooting

**`/output-style code-reviewer` doesn't do anything**

- Confirm the file exists at `.claude/output-styles/code-reviewer.md` and the name matches the file name or the `name:` frontmatter
- Restart Claude Code — style files are read at startup, so a file created mid-session isn't listed yet
- Run `/output-style` with no arguments to list available styles and confirm Claude Code sees the file
- If the frontmatter YAML doesn't parse, the style loads under its file name with no fields set — run `claude --debug` to see the error

**Responses don't seem different from default**

- The style body may be too soft — instructions like "try to be concise" are weaker than concrete rules ("no sentence over 15 words," "no hedging words: might, could, potentially")
- Confirm you didn't switch back to `/output-style default` accidentally between runs
- If you set `outputStyle` by hand in a settings file, the value is case-sensitive and must match the style's name exactly

**Style persists across unrelated sessions and that's surprising**

- This is expected — your choice is saved to `.claude/settings.local.json` for this project (or `~/.claude/settings.json` if you set it there). Switch back explicitly with `/output-style default` when you're done

---

**Next**: [Lab 15: Capstone — Team Configuration](lab-15-capstone-team-config.md)
