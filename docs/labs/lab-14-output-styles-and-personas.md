# Lab 14: Output Styles & Custom Personas

**Duration**: 20–25 minutes
**Day**: 2 of 2 (Advanced)

**Learning Objectives**:
- Understand output styles as a persistent, system-prompt-level customization
- Distinguish an output style from a one-off prompt instruction
- Create and activate a custom output style
- Evaluate when to standardize an output style at the team level

---

## Overview

Every customization mechanism so far — CLAUDE.md, hooks, MCP, Skills — changes what Claude Code *knows* or *can do*. An **output style** changes something different: how Claude Code *communicates* — its tone, verbosity, and default behavior — persistently, across every response, until you switch it off.

This is the last new mechanism before the capstone. After this lab you'll have the complete toolkit — CLAUDE.md, hooks, MCP, Skills, Sub-Agents, and output styles — to design the full team configuration in Lab 15.

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

Activate a style with the `/output-style` command:

```
/output-style code-reviewer
```

Switch back to default the same way:

```
/output-style default
```

### A.3 Team Use Cases

| Style | Use Case |
|---|---|
| **Terse CI-log mode** | Headless scripts (Lab 13) where output feeds a log or another script — no conversational filler |
| **Teaching / explain-everything** | Onboarding a junior developer — every response explains *why*, not just *what* |
| **Strict-reviewer persona** | Code review passes — blunt, opinionated, no hedging |
| **Customer-facing summary** | Non-technical stakeholders — no jargon, outcome-focused |

Output styles are a good fit when the *same* tone should apply across many different prompts and many different sessions. If it's a one-off, just say it in the prompt.

---

## Part B: Build the First Output Style in This Repo (10 min)

No `.claude/output-styles/` directory exists anywhere in this repo yet.

### B.1 Establish a Baseline

Before creating the style, run a prompt under default behavior so you have something to compare against:

```
Review src/TaskManager.Infrastructure/Legacy/LegacyTaskProcessor.cs and tell me what's wrong with it.
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
---

You are a blunt senior engineer doing a code review. Rules:

- No preamble, no restating the question, no "Great question!"
- Lead with the most severe issue first
- One line per issue: what's wrong, why it matters, the fix
- No hedging ("might", "could potentially") — state the problem directly
- If the code is fine, say so in one sentence and stop
- Never apologize or soften criticism with praise sandwiches
```

### B.3 Activate It and Re-Run the Same Prompt

```
/output-style code-reviewer
```

```
Review src/TaskManager.Infrastructure/Legacy/LegacyTaskProcessor.cs and tell me what's wrong with it.
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
- **Where should a shared style live?** Project-level `.claude/output-styles/` (shared) vs. an individual's own local configuration — the same question the [Agent Governance Guide](../guides/agent-governance.md) raises for project vs. user settings.
- **Who decides the default?** If nobody activates a custom style, everyone gets Claude Code's default tone — is that itself a decision that needs to be made deliberately for a team, or is it fine to leave unset?

This is direct prep for Lab 15: when you design a full team configuration, you'll decide whether output styles are part of it, and if so, which ones and why.

---

## Success Criteria

- [ ] `.claude/output-styles/code-reviewer.md` exists and defines a clear persona
- [ ] You activated it with `/output-style code-reviewer` and confirmed Claude Code's tone changed
- [ ] You ran the same prompt under default and custom styles and can point to concrete differences
- [ ] You can articulate the team-vs-individual tradeoff for standardizing an output style

---

## Extension Challenge

Create a second output style, `.claude/output-styles/teaching-mode.md`, that does the opposite of `code-reviewer`: verbose, explains reasoning step by step, defines any technical term on first use. Run the same LegacyTaskProcessor.cs prompt under this style and compare all three responses (default, code-reviewer, teaching-mode) side by side.

---

## Troubleshooting

**`/output-style code-reviewer` doesn't do anything**

- Confirm the file exists at `.claude/output-styles/code-reviewer.md` and the name matches what you passed to `/output-style`
- Run `/output-style` with no arguments to list available styles and confirm Claude Code sees the file

**Responses don't seem different from default**

- The style body may be too soft — instructions like "try to be concise" are weaker than concrete rules ("no sentence over 15 words," "no hedging words: might, could, potentially")
- Confirm you didn't switch back to `/output-style default` accidentally between runs

**Style persists across unrelated sessions and that's surprising**

- This is expected — output styles are meant to persist. Switch back explicitly with `/output-style default` when you're done

---

**Next**: [Lab 15: Capstone — Team Configuration](lab-15-capstone-team-config.md)
