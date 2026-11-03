---
marp: true
theme: default
paginate: true
backgroundColor: #fff
---

# Module 14: Output Styles & Custom Personas

## Changing How Claude Code Communicates, Not What It Knows

---

# Where We Are

You've built the automation toolkit: MCP, Skills, Sub-Agents, headless mode (Module 13).

**Module 14 is about tone, not capability.** It's **self-study** in the default agenda — work through it after the workshop (20–25 min).

Every mechanism so far changes what Claude Code knows or can do. An output style changes how it talks — persistently, across the whole session.

---

# One-Off Instruction vs. Output Style

**One-off** (works for a single response):
```
Answer tersely — one paragraph, no preamble.
```
Say it again next turn, or Claude Code drifts back to default.

**Output style** (persists at the system-prompt level):
```
/output-style code-reviewer
```
Applies to every response until you switch it.

---

# Where Output Styles Live

<style scoped>
section { font-size: 26px; }
</style>

```
.claude/output-styles/
  <style-name>.md
```

Activate:
```
/output-style code-reviewer
```

Back to default:
```
/output-style default
```

Or `/config` → **Output style**. Choice saved as `outputStyle` in `.claude/settings.local.json`; applies from your next message.

Also: `~/.claude/output-styles/` (personal), managed policy, or a plugin's `output-styles/`.

---

# Built-In Styles

| Style | What changes |
|---|---|
| **Default** | Standard software-engineering behavior |
| **Proactive** | Starts work right away; assumes instead of asking about routine decisions |
| **Concise** | Result first — no preamble, narration, or recap |
| **Explanatory** | Adds `★ Insight` blocks explaining the *why* |
| **Learning** | Insights + leaves `TODO(human)` pieces for you to write |

Try a built-in before writing your own — it may already be what you need.

---

# Team Use Cases

| Style | Use Case |
|---|---|
| **Terse CI-log mode** | Headless scripts (Module 13) — no conversational filler |
| **Teaching / explain-everything** | Onboarding — every response explains *why* |
| **Strict-reviewer persona** | Code review passes — blunt, opinionated |
| **Customer-facing summary** | Non-technical stakeholders — no jargon |

Good fit when the *same tone* should apply across many prompts and many sessions. If it's a one-off, just say it in the prompt.

---

# Anatomy of a Custom Style

<style scoped>
section { font-size: 23px; }
</style>

```markdown
---
name: code-reviewer
description: Terse, blunt senior-reviewer persona for code review passes.
keep-coding-instructions: true
---

You are a blunt senior engineer doing a code review. Rules:

- No preamble, no restating the question
- Lead with the most severe issue first
- One line per issue: what's wrong, why it matters, the fix
- No hedging language ("might", "could potentially")
- If the code is fine, say so in one sentence and stop
```

`keep-coding-instructions: true` keeps Claude Code's engineering instructions (default `false` replaces them).
Restart Claude Code after creating a style file — styles load at startup.

Concrete rules beat vague ones — "no sentence over 15 words" changes behavior; "try to be concise" often doesn't.

---

# Standardize or Leave Personal?

<style scoped>
section { font-size: 23px; }
</style>

Output styles raise a governance question Skills don't: this is a *default persona for the whole session*, not a narrow procedure.

| Question | Consider |
|---|---|
| Should the team share one default style? | A CI-log style: probably yes — consistency matters for scripts |
| Should everyone use the same review persona? | Maybe not — reviewers have different styles for good reason |
| Where does a shared style live? | `.claude/output-styles/` (project) vs. `~/.claude/output-styles/` (personal) vs. a team plugin |
| Make it available, or activate it? | Committing the file = available; committing `outputStyle` in `.claude/settings.json` = everyone's default |
| Who decides the default? | Same ownership question as CLAUDE.md and settings.json |

---

# Setting Up the Capstone

<style scoped>
section { font-size: 27px; }
</style>

By the end of Module 14, you have the **complete customization toolkit**:

| Mechanism | What It Changes |
|---|---|
| CLAUDE.md | Context Claude Code always has |
| Hooks | Side effects on tool calls |
| MCP | What Claude Code can reach |
| Skills | Procedures on standby |
| Subagents | How work gets parallelized/isolated |
| Output Styles | How Claude Code communicates |

**Lab 15 (Capstone): design all six together as one team configuration — and package it as a plugin.**

---

# Lab 14 Preview

## What You'll Do

1. Run a baseline prompt under default behavior; try a built-in style
2. Create `.claude/output-styles/code-reviewer.md` — a terse, blunt persona
3. Activate it with `/output-style code-reviewer`
4. Re-run the same prompt and compare responses side by side
5. Discuss: standardize at the team level, or leave it personal?

## The Key Insight

An output style is a persistent decision about *how* Claude Code talks —
made once, applied every session, until you change it.

---

# Lab 14: Output Styles & Custom Personas

👉 Open [docs/labs/lab-14-output-styles-and-personas.md](../../../labs/lab-14-output-styles-and-personas.md)

**Goal for the next 20 minutes:**
- Custom output style created and activated
- Side-by-side comparison of default vs. custom behavior
- Clear on the team-vs-personal tradeoff — feeds directly into Lab 15

---

<!-- Navigation -->
[← 13 Headless & Automation](05-headless-automation.md) | **Day 2 Module 14** | [Next: Capstone →](07-capstone-team-config.md)
