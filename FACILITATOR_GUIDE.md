# Facilitator Guide — AI-Assisted Development with Claude Code

This guide is for facilitators running the 2-day, 15-lab Claude Code workshop. It covers pre-workshop setup, a timed agenda for each day, goals and talking points for each lab, common pitfalls, troubleshooting, and ways to compress or extend the material.

**Participant-facing material:** [README](README.md) · [Lab guides](docs/labs/README.md) · [Slide decks](docs/presentations/index.md) · [Local setup](docs/LOCAL_SETUP.md) · [Devcontainer setup](.devcontainer/README.md) · [Pre-workshop checklist](docs/PRE_WORKSHOP_CHECKLIST.md)

**Reference guides:** [Customization Decision Guide](docs/guides/customization-decision-guide.md) · [Agent Design Guide](docs/guides/agent-design-guide.md) · [Agent Governance Guide](docs/guides/agent-governance.md)

---

## Timing and Flow Tips

- **Each module = short slide intro + hands-on lab.** Keep intros to 5–10 minutes. Participants learn Claude Code by running it, not by watching slides.
- **Demo live, in a terminal.** Show real output, including mistakes. Claude Code is non-deterministic, so your results will differ from the lab text, and that's a useful teaching moment.
- **Tell participants to pick one stack** (🔷 .NET or 🟩 Spring Boot) and stay with it. Mixed pairs work well: one drives, one reviews diffs.
- **Pair weaker setups with working ones.** If someone's install or auth fails, pair them with a neighbor right away and fix it at the next break.
- **Remind people to commit at the end of each lab** (Lab 7 teaches the workflow; before that, `git add -A && git commit -m "lab N"` is fine). Later labs build on earlier ones, and commits make Lab 6 (rewind vs. git) and Lab 8 (`/security-review` on branch commits) work.
- **Protect Lab 3 Part 2.** It's the longest lab (two parts, 90–100 min), and Part 2 is Day 1's requirements-to-code arc. If the morning runs long, drop Part 2's ADR step (Step 10) first, then use the other [Timing Adjustments](#timing-adjustments-and-fast-track-options) before cutting Part 2 itself.
- **Expect variance in speed.** Every lab has an *Extension Exercise* or *Extension Challenge* for fast finishers.

---

## Pre-Workshop Setup Checklist

### 2+ weeks before

- [ ] **Decide how participants will authenticate** and tell them in the invitation:

  | Option | Good for | Notes |
  |---|---|---|
  | Claude Pro / Max / Team / Enterprise subscription | Most participants | Sign in with `/login`. Team/Enterprise admins must assign Claude Code seats. |
  | Claude Console API key (`ANTHROPIC_API_KEY`) | Company-funded workshops, CI demos (Lab 13) | Create keys at [platform.claude.com](https://platform.claude.com). Set a spend limit per participant. An approved key **takes precedence** over a subscription login. |
  | Amazon Bedrock / Google Vertex AI / Microsoft Foundry | Orgs that must use their cloud provider | Needs provider env vars; test well in advance. See [third-party integrations](https://code.claude.com/docs/en/third-party-integrations). |

  Free Claude.ai plans do not include Claude Code.

- [ ] **Decide on environment: devcontainer or local.**
  - **Devcontainer / Codespaces** (recommended for mixed groups): `.NET` and `Spring Boot` participant containers install Claude Code with the native installer and have `jq` and `gh`. Containers don't share your host login, so participants run `/login` inside the container, or forward `ANTHROPIC_API_KEY` / `CLAUDE_CODE_OAUTH_TOKEN` (from `claude setup-token`). See [.devcontainer/README.md](.devcontainer/README.md).
  - **Local**: participants follow [docs/LOCAL_SETUP.md](docs/LOCAL_SETUP.md): the native installer (`curl -fsSL https://claude.ai/install.sh | bash`, or `irm https://claude.ai/install.ps1 | iex` on Windows), plus .NET 9 SDK or JDK 21 + Maven, `git`, and `jq`.
- [ ] **Decide on Lab 10 MCP prerequisites.** Part B runs the filesystem server with `npx`, so it needs **Node.js 18+**. All three devcontainers include Node.js LTS; local-install participants need Node 18+ (or you demo Part B yourself). Part C needs a GitHub fine-grained personal access token (read-only Issues, Pull requests, Contents).
- [ ] **Send the [Pre-Workshop Checklist](docs/PRE_WORKSHOP_CHECKLIST.md).** Ask participants to complete it and reply with the output of `claude doctor` and `dotnet test` / `mvn test`.

### 1 week before

- [ ] Each participant **forks** `centricconsulting/ai-coding-workshop-claude-code` and clones *their fork*. They need an `origin` remote with a default branch for `/security-review` (Lab 8) and for PR prep in Lab 7.
- [ ] Each participant creates a personal branch: `git switch -c workshop/<name>`.
- [ ] Run every lab yourself on the current Claude Code version. Claude Code updates frequently; check the [changelog](https://code.claude.com/docs/en/changelog) for renamed commands or changed defaults.
- [ ] Confirm network access to `claude.ai`, `api.anthropic.com`, `platform.claude.com`, GitHub, NuGet, Maven Central, and npm (Lab 10). Corporate proxies are the most common blocker; see [network configuration](https://code.claude.com/docs/en/network-config).

### Day of

- [ ] Projector terminal with a large font. Turn on a light theme if the room is bright.
- [ ] Your own fork, already through Lab 15, on a separate branch as a fallback "answer key".
- [ ] A spare API key or two for participants whose subscription login fails.
- [ ] Slides: `docs/presentations/modules/` (Marp; see [docs/presentations/README.md](docs/presentations/README.md)).

### Verify a participant environment (2 minutes)

```bash
claude --version
claude doctor                 # install type, PATH, auto-update status
claude -p "Reply with OK"     # proves auth works without opening the UI
dotnet test src-dotnet        # .NET: builds, 1 passed, 11 skipped, 0 failed
# or
cd src-springboot && mvn -q test
```

The 11 skipped .NET tests are intentional stubs that later labs fill in. They are not failures. The 1 passing test is a `/health` integration test that proves `WebApplicationFactory` works.

---

## Agenda

Module durations combine a condensed slide intro with the upper end of the lab's stated duration. Breaks and lunch are extra. Start times assume a 9:00 start.

### Day 1: Fundamentals (~6 hours of content)

| Time | Duration | Block | Lab |
|---|---|---|---|
| 9:00 | 15 min | Kickoff & Setup | — |
| 9:15 | 10 min | Claude Code Features Tour | — |
| 9:25 | 25 min | First Contact | Lab 1 (20–25) |
| 9:50 | 55 min | Module 2: CLAUDE.md & Project Memory | Lab 2 (40–45) |
| 10:45 | 15 min | ☕ Break | |
| 11:00 | 55 min | Module 3, Part 1: TDD fundamentals | Lab 3 Part 1 (45–50) |
| 11:55 | 60 min | 🍽️ Lunch | |
| 12:55 | 50 min | Module 3, Part 2: Requirements → Code | Lab 3 Part 2 (~45) |
| 13:45 | 35 min | Module 4: Plan Mode, Permission Modes & Thinking | Lab 4 (25–30) |
| 14:20 | 60 min | Module 5: Multi-File Refactoring & Context | Lab 5 (45–50) |
| 15:20 | 15 min | ☕ Break | |
| 15:35 | 25 min | Module 6: Checkpoints & Rewind | Lab 6 (20) |
| 16:00 | 30 min | Module 7: Git Workflow & PR Preparation | Lab 7 (20–25) |
| 16:30 | 15 min | Day 1 Wrap-Up & Discussion | — |
| **16:45** | **375 min + 90 min breaks** | | |

### Day 2: Advanced Claude Code (~6 hours of content)

| Time | Duration | Block | Lab |
|---|---|---|---|
| 9:00 | 10 min | Day 2 Kickoff & Recap | — |
| 9:10 | 45 min | Module 8: Security Review Workflows | Lab 8 (30–35) |
| 9:55 | 40 min | Module 9: Permissions, Safety & Hooks | Lab 9 (25–30) |
| 10:35 | 15 min | ☕ Break | |
| 10:50 | 45 min | Module 10: MCP Servers | Lab 10 (30–35) |
| 11:35 | 45 min | Module 11: Skills | Lab 11 (30–35) |
| 12:20 | 60 min | 🍽️ Lunch | |
| 13:20 | 50 min | Module 12: Custom Subagents & Agent Workflows | Lab 12 (35–40) |
| 14:10 | 40 min | Module 13: Headless & Automation Mode | Lab 13 (25–30) |
| 14:50 | 15 min | ☕ Break | |
| 15:05 | 75 min | Module 15: Capstone — Team Configuration & Plugins | Lab 15 (60–65) |
| 16:20 | 15 min | Day 2 Wrap-Up & Next Steps | — |
| **16:35** | **365 min + 90 min breaks** | | |

**Self-study:** Module 14 / Lab 14, Output Styles & Custom Personas (20–25 min). It's the least dependent lab. If Day 2 runs ahead, run it live between Modules 13 and 15 (the 16:35 finish becomes about 17:05).

Both days end before 17:00 with about 15 minutes of slack. Day 1 keeps all of Lab 3, so it ends on a feature taken from user story to a committed, PR-ready endpoint. If a day still runs late, use the [fast-track options](#timing-adjustments-and-fast-track-options).

---

## Day 1 — Lab by Lab

### Kickoff & Features Tour (25 min)

**Goal:** Explain the agentic model and get everyone's environment confirmed.

**Talking points**
- Claude Code is an **agent**: it reads files, runs commands, and edits code in a loop, asking permission along the way. That's different from completion-style tools.
- The same agent runs in the terminal, the VS Code / JetBrains extensions, the desktop app, and on the web. The workshop uses the terminal CLI because every other surface builds on it.
- The memory and customization stack: `CLAUDE.md` → `.claude/rules/` → skills → subagents → hooks / permissions → MCP → plugins. Today covers the foundations; tomorrow covers the customization layers.
- Coming from GitHub Copilot? Show the [mapping table](README.md#relationship-to-the-github-copilot-workshop).

**Checkpoint:** everyone has `claude doctor` passing and `claude -p "Reply with OK"` working. Anyone who doesn't gets paired up immediately.

---

### Lab 1: First Contact (20–25 min)

**Goal:** Install, authenticate, explore the codebase, learn the UI and permission modes.

**Talking points**
- `/status` shows which credential is active. If `ANTHROPIC_API_KEY` is set, it wins over a subscription login.
- `/context`, `/model`, `/effort`: what's loaded, which model, how hard it thinks.
- **Shift+Tab** cycles permission modes. Newer versions start in **Auto** mode (a classifier approves safe actions), so participants may see fewer prompts than the lab describes. Switch to **Manual** to see prompts.
- `dotnet *` and `mvn *` are pre-approved in `.claude/settings.json`, which is why running tests doesn't prompt. Lab 1 Part D creates a file in Manual mode to trigger a prompt deliberately.

**Common pitfalls**
- `claude: command not found` after a native install: `~/.local/bin` isn't on PATH. Open a new terminal or follow the installer's PATH hint.
- Expecting `/init` to be run here. It's introduced in Lab 2, and the repo already has a `CLAUDE.md`.

**Checkpoint:** each participant has asked Claude to explain the architecture and approved or denied at least one permission prompt.

---

### Lab 2: CLAUDE.md & Project Memory (40–45 min)

**Goal:** Understand the memory hierarchy and path-scoped rules.

**Talking points**
- Memory layers: managed policy → project `CLAUDE.md` → subdirectory `CLAUDE.md` (loaded when Claude works there) → `.claude/rules/` → user `~/.claude/CLAUDE.md` → `CLAUDE.local.md`, plus **auto memory**.
- **Rules with `paths:`** load only for matching files. The shipped `dotnet.md` and `springboot.md` keep C# conventions out of Java sessions. `follow-up-questions.md` has no `paths:`, so it always loads.
- Use `/memory` to see and edit loaded files and `/context` to see their token cost. Keep CLAUDE.md short.
- CLAUDE.md is *guidance*, not enforcement. Anything that must always happen belongs in hooks or permissions (Lab 9).

**Common pitfalls**
- A path rule "never loads": it only loads when Claude reads or edits a matching file, not at session start.
- CLAUDE.md edits mid-session: start a new session or ask Claude to re-read the file.
- Conflicting instructions across layers: the more specific file generally wins, but conflicts make behavior unpredictable, so resolve them.

**Checkpoint:** a subdirectory CLAUDE.md or a new rule exists, and the participant has seen it change Claude's output.

---

### Lab 3: TDD & Requirements → Code (90–100 min, two parts)

**Goal (Part 1):** Run Red → Green → Refactor on a `NotificationService` with Claude writing files directly.
**Goal (Part 2):** Turn a user story into a backlog in plan mode, then implement `Priority`, `DueDate`, and `POST /tasks` test-first across every layer, and record an ADR.

**Talking points**
- **Make Claude stop at red.** Ask for failing tests only, run them, and confirm they fail for the right reason before implementing.
- Claude writes and runs tests itself. Review the **diff**, not the chat summary.
- Part 2: requirements, not prompts, are the source of truth. The backlog file is the contract; each step references it.
- This is where the Day 2 shipped subagents come from: `backlog-generator`, `architecture-reviewer`, and `test-strategist` automate exactly what participants do by hand here.

**Common pitfalls**
- Claude writes the implementation alongside the tests. Restate "tests only — do not implement" and reject the edit.
- **False green**: tests pass against a stub. Ask Claude to show a test failing first.
- Plan mode can't save the backlog file. Exit plan mode (Shift+Tab) before asking Claude to write it.
- .NET `CS0104: 'Priority' is an ambiguous reference`: FakeItEasy also has a `Priority` type. Alias it: `using Priority = TaskManager.Domain.Tasks.Priority;`.
- .NET `POST /tasks` returns 404: the route is still commented out in `EndpointExtensions.cs`, or the API wasn't restarted.
- Spring: mapping errors after adding `priority` need the JPA entity and mapper updated together.

**Checkpoint (end of Part 1, before lunch):** green tests and a commit. **(End of Part 2):** `POST /tasks` with priority and due date works end-to-end, and the ADR exists.

---

### Lab 4: Plan Mode, Permission Modes & Thinking (25–30 min)

**Goal:** Use permission modes as interaction models; plan the `LegacyTaskProcessor` refactor read-only; control reasoning depth.

**Talking points**
- Modes: **Manual** (ask before edits and commands) · **Accept edits** · **Plan** (read-only research and proposal) · **Auto** (classifier-approved). Also `/plan <task>` to jump straight into plan mode.
- Plan mode is for *agreeing on the approach* before any file changes. Push back on vague plans.
- Thinking: `/effort low|medium|high|xhigh|max` sets reasoning depth for the session. **`ultrathink`** in a prompt requests deeper reasoning for one turn. "think hard" and similar phrases are no longer special keywords.

**Common pitfalls**
- Auto mode doesn't appear in the Shift+Tab cycle: it depends on plan, model, and admin settings.
- Plan mode still asks to run commands: read-only commands may still need approval in some setups. That's expected.
- Vague plans: ask for file-by-file steps, risks, and a test strategy.

**Checkpoint:** a written plan for the legacy refactor (saved for Lab 5).

---

### Lab 5: Multi-File Refactoring & Context Management (45–50 min)

**Goal:** Refactor `LegacyTaskProcessor` safely behind characterization tests, in phases, while managing context.

**Talking points**
- **Characterization tests first.** They lock in current behavior, including bugs, before anything moves.
- Refactor in small phases (rename → extract → restructure), running tests after each.
- `/context` shows what's using the window. `/compact` summarizes it with optional focus instructions. `/clear` starts fresh. Subagents (Day 2) are another way to keep the main context small.

**Common pitfalls**
- Claude changes logic during a "rename only" phase. Reject the change and restate the constraint.
- Characterization tests fail after refactoring: that's the point. Investigate whether behavior really changed.
- Context overflow on long sessions: compact with a focus ("keep the refactor plan and test status").

**Checkpoint:** characterization tests pass after the refactor, committed.

---

### Lab 6: Checkpoints & Rewind (20 min)

**Goal:** Know when to use `/rewind` (session-level undo) vs. git.

**Talking points**
- Claude Code checkpoints **its own file edits** at each prompt. Press Esc twice (with an empty prompt) or run `/rewind` to restore code, conversation, or both.
- **Changes made by Bash commands, by you in the IDE, or outside the session aren't tracked.** Use git for those.
- Rewind is for "that last turn went badly"; git is for durable history and sharing.

**Common pitfalls**
- `/rewind` shows no checkpoints: no file edits happened yet in this session.
- A rewound file still breaks tests: a Bash command or build output changed something else.
- "The tests still pass, so nothing broke": the over-broad edit usually changes behavior the characterization tests don't assert (in testing it removed `Thread.Sleep` and narrowed the `catch`). Point them at `git diff`. That gap is part of the lesson.

**Checkpoint:** each participant restored a bad edit with `/rewind` and saw that a Bash-created file survived.

---

### Lab 7: Git Workflow & PR Preparation (20–25 min)

**Goal:** Pre-commit review, conventional commits, and a PR description, all through Claude.

**Talking points**
- **`/code-review`** reviews the current changes before commit (`/review` still works as an alias).
- Ask for conventional commit messages that explain *why*. The repo's `.claude/settings.json` pre-approves `git add` and `git commit` but **asks before `git push`** and denies force-push and `reset --hard`.
- Claude can draft the PR description from the branch diff. `gh pr create` works if participants have the GitHub CLI.

**Common pitfalls**
- Committing to `main`. Remind everyone to work on `workshop/<name>`.

**Checkpoint:** a reviewed commit with a conventional message and a drafted PR description.

---

### Day 1 Wrap-Up (15 min)

- What surprised you about working with an agent rather than completions?
- Which permission mode did you settle on, and why?
- Ask everyone to **commit and push** their branch. Day 2 opens with Lab 8, which reviews those branch commits.

---

## Day 2 — Lab by Lab

### Day 2 Kickoff & Recap (10 min)

Recap Day 1 in one slide: memory → TDD → requirements → code → plan → refactor → rewind → git. Then frame Day 2: a **security review** of Day 1's branch, then **making Claude Code yours and your team's** with permissions, hooks, MCP, skills, subagents, automation, and plugins (output styles are self-study). Show the [decision flow](docs/guides/customization-decision-guide.md#decision-flow).

---

### Lab 8: Security Review Workflows (30–35 min)

**Goal:** See why `/security-review` misses a project-specific rule, build a `/security-checklist` skill that catches it, and fix the vulnerability.

**Talking points**
- `/code-review` covers general quality; **`/security-review`** is a single pass focused on vulnerability classes (injection, secrets, broken auth, unsafe input).
- `/security-review` reviews the **commits on your branch** compared with `origin`'s default branch. Uncommitted work isn't included.
- **The planted bug usually isn't reported by `/security-review`.** In validation it was dropped at 3/10 and 7/10 confidence (it reports at 8/10+): no auth, GUID IDs, no list endpoint. That's the lesson: a generic reviewer weighs today's exploitability; your team's rule ("every task operation checks ownership") is something only a project skill knows. Have participants ask it *why* it didn't report the assign path.
- A custom skill adds *your* domain knowledge (field names, invariants) on top of a generic review. This previews Lab 11 later in the day. The skill prompt's "report every match even without auth" line is what makes it catch the gap.
- CI options: the Claude Code GitHub Action and headless runs (Lab 13).
- It opens Day 2 on purpose: participants review the branch they built on Day 1, and the move from *reviewing* for risk to *preventing* it leads straight into Lab 9.

**Common pitfalls**
- `ambiguous argument` or "nothing to review": no `origin` remote, or `origin/HEAD` isn't set. Fix with `git fetch origin && git remote set-head origin --auto`. Participants who cloned upstream instead of a fork need to add their fork as `origin`.
- "`/security-review` found nothing, so I'm stuck": that's expected; move on to Part C. If it says "nothing to review", the code isn't committed yet.
- `/security-checklist` misses the gap: the skill body is missing the "report every match even without authentication" instruction.
- Claude fixes the vulnerability before the participant reviews it. Ask for findings only first.

**Checkpoint:** `/security-checklist` found the gap `/security-review` didn't report; the gap is fixed and both checks re-run clean.

---

### Lab 9: Permissions, Safety & Hooks (25–30 min)

**Goal:** Read and modify `.claude/settings.json`; write hooks that log, guard, and react.

**Talking points**
- Permission rules: `allow` / `ask` / `deny`; **deny always wins**. Settings scopes: managed → user → project (`.claude/settings.json`, committed) → local (`.claude/settings.local.json`, personal).
- Hooks are **deterministic**. They run on events like `PreToolUse`, `PostToolUse`, `Stop`, and `SessionStart`, and receive JSON on stdin. **Exit code 2 blocks** the action and returns stderr to Claude; other non-zero codes don't block.
- The repo ships an audit hook that writes to `.claude/audit.log`. Show it working.
- Part D: when *not* to automate (slow hooks on every edit, hooks that hide failures).

**Common pitfalls**
- Hooks not firing: invalid JSON in settings, wrong matcher (`Edit|Write`, case-sensitive), or the session didn't pick up the change. Check with `/hooks`.
- `jq: command not found`: install `jq` locally. It's preinstalled in all devcontainers.
- "My block didn't block": the hook exited 1 instead of 2.

**Checkpoint:** a working `PreToolUse` guard that blocks a command, and a `PostToolUse` hook.

---

### Lab 10: MCP Servers (30–35 min)

**Goal:** Add MCP servers with the right scope; connect GitHub's official server.

**Talking points**
- MCP adds **capabilities** (tools) Claude doesn't have built in. Transports: `stdio` (local process) and `http` (remote, recommended); `sse` is deprecated.
- Scopes: `local` (default, just you) · `project` (written to `.mcp.json`, shared) · `user` (you, every project).
- Use `/mcp` to check status and to authenticate servers that support OAuth.
- Part D: build a custom server only when no existing server fits.

**Common pitfalls**
- **No Node in .NET/Spring devcontainers**: Part B's `npx` command fails. See [Pre-Workshop Setup](#2-weeks-before).
- Project server never loads: `.mcp.json` servers need a one-time approval. If it was declined, run `claude mcp reset-project-choices` and restart.
- GitHub 401/403: PAT scope or expiry. Fine-grained tokens must include the target repositories.
- Absolute paths committed to `.mcp.json`: use `${VAR}` expansion instead.

**Checkpoint:** `claude mcp list` shows connected servers, and Claude answered a question using a GitHub MCP tool.

---

### Lab 11: Skills (30–35 min)

**Goal:** Understand skills (which now include custom slash commands), use the shipped `/test-data-generator`, and build a boundary-check skill.

**Talking points**
- A skill is a folder with `SKILL.md` and optional templates or scripts. Only the `description` loads until the skill is used (progressive disclosure).
- **Invoke** with `/skill-name [args]`, or let Claude pick it by description. `disable-model-invocation: true` makes it manual-only.
- `.claude/commands/*.md` still works, but skills are the recommended format. Skills follow the open [Agent Skills](https://agentskills.io) standard, so they're portable.
- Scope: project (`.claude/skills/`), personal (`~/.claude/skills/`), or plugin (`/plugin:skill`).

**Common pitfalls**
- Skill never activates: the `description` is vague. Add *when to use it*.
- Skill fires on everything: the description is too broad, or use `disable-model-invocation`.
- Arguments don't show up: reference `$ARGUMENTS` (or named arguments) in the body.

**Checkpoint:** `/test-data-generator Task 5 csharp` (or `java`) produced data, and the participant's own skill works.

---

### Lab 12: Custom Subagents & Agent Workflows (35–40 min)

**Goal:** Use built-in and shipped subagents, chain a feature workflow, and design a new subagent.

**Talking points**
- Built-ins: **Explore**, **Plan**, **general-purpose**.
- Shipped in `.claude/agents/`: `backlog-generator` → `architecture-reviewer` (read-only) → `test-strategist` (read-only). Each has a focused role, least-privilege tools, a model choice, an output contract, and a **Handoff** section.
- Invoke by natural language, `@agent-<name>` (guaranteed), automatic delegation, or `claude --agent <name>`.
- Subagents have **their own context window**, so only the summary returns. That's the main reason to use them.
- Walk through the [Agent Design Guide](docs/guides/agent-design-guide.md) for Part C.

**Common pitfalls**
- No automatic delegation: sharpen the `description` with "Use proactively when…".
- A.4 runs the Lab 11 `clean-architecture-boundary-check` skill instead of `architecture-reviewer`: both descriptions claim the same trigger. Use it to discuss skill vs. subagent; `@agent-architecture-reviewer` forces the agent.
- The B.1 chain takes several minutes (about 7 min and roughly $1.50 in validation). Start it, then discuss while it runs.
- The subagent can't do something: check its `tools` list. The architecture reviewer deliberately can't run git, so pass it file paths.
- Chain loses information: make each agent's final message contain its full output.

**Checkpoint:** a backlog in `docs/backlog/`, an architecture review, and a test plan produced by the chain; a new subagent file drafted.

---

### Lab 13: Headless & Automation Mode (25–30 min)

**Goal:** Run Claude non-interactively for scripts and CI.

**Talking points**
- `claude -p "…"` prints and exits. Pipe input in and choose `--output-format text|json|stream-json`.
- Control runs with `--allowedTools` / `--disallowedTools`, `--permission-mode`, `--max-turns`, `--max-budget-usd`, and `--append-system-prompt`. `--bare` skips local customizations for reproducible CI runs.
- `--continue` and `--resume <id>` keep context across runs.
- CI: the Claude Code GitHub Action, or `claude -p` in any pipeline with `ANTHROPIC_API_KEY` as a secret.

**Common pitfalls**
- Auth error in `-p`: headless runs use the same credentials. In CI, set `ANTHROPIC_API_KEY` or `CLAUDE_CODE_OAUTH_TOKEN`.
- Does nothing or "permission denied": headless runs can't answer prompts. Pre-approve tools with `--allowedTools`.
- Cost or time overruns in CI: always set `--max-turns` and/or `--max-budget-usd`.
- "My MCP server ran without asking": headless runs connect `.mcp.json` servers **without** the approval prompt. In validation, a still-pending Lab 10 `filesystem` server connected (14 tools) in a plain `claude -p`. Make this the Part E safety point, and suggest `claude mcp remove filesystem --scope project` after Lab 10.

**Checkpoint:** a scripted run producing JSON output, plus a sketched CI job.

---

### Lab 14: Output Styles & Custom Personas (20–25 min, self-study by default)

**Goal:** Change *how* Claude communicates with built-in and custom output styles.

**Talking points**
- Built-in styles: Default, Proactive, Concise, Explanatory, Learning. Switch with `/output-style`.
- Custom styles live in `.claude/output-styles/*.md` (project) or `~/.claude/output-styles/` (personal). `keep-coding-instructions: true` keeps Claude's default engineering behavior.
- Output style vs. CLAUDE.md vs. subagent: style changes the *voice* for the whole session; CLAUDE.md adds project facts; a subagent is a separate specialist.

**Common pitfalls**
- `/output-style code-reviewer` does nothing: check the file location and `name` frontmatter, then restart.
- The style persists unexpectedly: `/output-style` saves the choice to `.claude/settings.local.json`. Switch back with `/output-style default`.

**Checkpoint:** a custom style that visibly changes responses.

---

### Lab 15: Capstone — Team Configuration & Plugins (60–65 min)

**Goal:** Design a complete team configuration, build a subagent or skill, package it as a plugin, and present it.

**Suggested flow**
1. Groups of 2–4 pick a scenario (5 min).
2. Design the CLAUDE.md hierarchy, rules, permissions, hooks, and MCP (25 min).
3. Build and test one subagent or skill (15 min).
4. Package as a plugin: `.claude-plugin/plugin.json` at the plugin root, with `skills/` and `agents/` beside it. Validate with `claude plugin validate ./team-plugin` and test with `claude --plugin-dir ./team-plugin` (10 min).
5. Present (10 min total; about 2–3 min per group).

**Talking points**
- Governance: who owns `.claude/`, how changes are reviewed, and what goes in managed settings. See the [Agent Governance Guide](docs/guides/agent-governance.md).
- Plugins are the distribution unit: install them from a marketplace with `/plugin`. Plugin subagents ignore `hooks`, `mcpServers`, and `permissionMode` frontmatter for safety.

**Common pitfalls**
- Components placed inside `.claude-plugin/`: only `plugin.json` goes there.
- Groups over-design and never build. Enforce the 15-minute build timebox.

**Checkpoint:** a validated plugin that loads with `--plugin-dir`, and a short presentation.

---

### Day 2 Wrap-Up (15 min)

Deck: [`part2/08-wrapup-day2.md`](docs/presentations/modules/part2/08-wrapup-day2.md). It recaps Labs 8–15, covers where each setting belongs (managed, project, personal), and ends with a first-week plan.

- Which customization would you adopt on your team on Monday?
- What would you put in **managed** settings vs. project settings?
- Point to the [Customization Decision Guide](docs/guides/customization-decision-guide.md), [Agent Design Guide](docs/guides/agent-design-guide.md), and the [Claude Code docs](https://code.claude.com/docs/en/overview).

---

## Troubleshooting

### Installation

| Symptom | Fix |
|---|---|
| `claude: command not found` after the native installer | `~/.local/bin` isn't on PATH. Add `export PATH="$HOME/.local/bin:$PATH"` to the shell profile and open a new terminal. The devcontainers set this in `remoteEnv`. |
| Multiple installs conflict (npm + native) | `claude doctor` shows the install type. Remove the old one (`npm uninstall -g @anthropic-ai/claude-code`). |
| npm install fails | npm installs need Node.js 22+. Prefer the native installer. |
| Windows: commands behave oddly | Install Git for Windows (recommended), or use WSL. Without Git Bash, Claude uses PowerShell. |
| Corporate proxy / TLS errors | Set `HTTPS_PROXY` and, if needed, `NODE_EXTRA_CA_CERTS`. See [network configuration](https://code.claude.com/docs/en/network-config). |

### Authentication

| Symptom | Fix |
|---|---|
| Unsure which account is being billed | Run `/status`. An approved `ANTHROPIC_API_KEY` takes precedence over a subscription. `unset ANTHROPIC_API_KEY` to fall back. |
| `/login` browser doesn't open (devcontainer, SSH) | Copy the URL Claude prints into a host browser and paste the code back. |
| Container keeps asking to log in | Use `claude setup-token` on the host and forward `CLAUDE_CODE_OAUTH_TOKEN`, or forward `ANTHROPIC_API_KEY`. See [.devcontainer/README.md](.devcontainer/README.md). |
| "Claude Code not enabled" on a Team/Enterprise account | An admin must assign a Claude Code seat. |

### Workshop-specific

| Symptom | Fix |
|---|---|
| `dotnet test` shows 1 passed, 11 skipped | Expected. The skipped tests are stubs completed during labs. |
| `/security-review` fails with `ambiguous argument` | No `origin` remote or `origin/HEAD` unset: `git fetch origin && git remote set-head origin --auto`. It also only reviews **committed** branch changes. |
| Hooks fail with `jq: command not found` | Install `jq` (`brew install jq`, `sudo apt-get install jq`, `winget install jqlang.jq`). It's preinstalled in the devcontainers. |
| Hooks don't fire | Validate the settings JSON (`jq . .claude/settings.json`) and check `/hooks`. |
| MCP server shows failed | Run `claude mcp list` and `/mcp`. For `npx` servers, check Node is installed (not included in the .NET/Spring devcontainers). |
| GitHub MCP 401/403 | Regenerate the PAT with the needed repositories and read permissions, or re-authenticate via `/mcp`. |
| No permission prompts appear | The session is in **Auto** or **Accept edits** mode, or the command is pre-approved in `.claude/settings.json`. Shift+Tab to **Manual**. |
| Subagent or skill doesn't appear | Check its location and frontmatter. If its `.claude/agents/` or `.claude/skills/` directory was just created, restart the session. |
| Claude ignores CLAUDE.md conventions | Check `/memory` to confirm the file is loaded. Keep the file short and specific. Move "must" rules to hooks or permissions. |
| `.NET` build issues | `dotnet --version` should be 9.x. Run `dotnet restore src-dotnet`, then `dotnet build src-dotnet`. |
| Spring Boot build issues | `java -version` should be 21. Run `mvn -v`, then `mvn clean test` from `src-springboot/`. |

---

## Timing Adjustments and Fast-Track Options

### If you're running behind

| Cut / move | Saves | Notes |
|---|---|---|
| Skip **Lab 3 Part 2, Step 10** (the ADR) and assign it as homework | ~5 min on Day 1 | Keeps the user-story-to-endpoint arc intact. Try this first. |
| Demo **Lab 6** instead of hands-on | ~15 min | Show `/rewind` live, then discuss the decision table. |
| Shorten **Lab 7**: skip the PR description | ~10 min | Keep `/code-review` and the conventional commit. |
| Demo **Lab 10 Part B**; participants do Part C only | ~10 min | Also avoids the Node prerequisite in devcontainers. |
| Shorten **Lab 13** to Parts A, B, and E | ~10 min | Keep the safety discussion; skip session persistence and CI. |
| Capstone: build **either** a subagent **or** a skill, and present in a gallery walk | ~15 min | |

### Fast-track for experienced participants

- **Skip Lab 1** (verify environment only) and **compress Lab 2** to Parts C–D (path rules and a subdirectory CLAUDE.md).
- Run Labs 4 and 6 as 10-minute demos.
- Spend the time saved on Lab 12 (design and test a custom subagent) and Lab 15.

### Alternative formats

| Format | Content |
|---|---|
| Half-day intro (~4 h) | Kickoff, Labs 1, 2, 3 Part 1, 4, 7 |
| Customization day (~6 h) | Day 2 only. Participants need basic CLAUDE.md experience. |
| Skills, Subagents & Plugins focus (~3.5 h) | Labs 11, 12, 15 with intros. See [docs/presentations/index.md](docs/presentations/index.md#workshop-delivery-patterns). |
| Two separate sessions | Day 1 one week, Day 2 the next. Ask participants to use CLAUDE.md and plan mode on real work in between. |

---

## Deliverables Recap

By the end of the workshop each participant's branch should contain:

- Updated or new `CLAUDE.md` files and at least one `.claude/rules/` file (Lab 2)
- `NotificationService` with tests, plus `Priority`, `DueDate`, `POST /tasks`, a backlog file, and an ADR (Lab 3)
- Refactored `LegacyTaskProcessor` with characterization tests (Lab 5)
- Conventional commits and a PR description (Lab 7)
- A fixed vulnerability and a `/security-checklist` skill (Lab 8)
- Custom permission rules and hooks (Lab 9), and an MCP config (Lab 10)
- A custom skill (Lab 11), a custom subagent and a backlog → review → test-plan chain (Lab 12)
- A headless script or CI sketch (Lab 13), and a custom output style if they did the Lab 14 self-study
- A team plugin (Lab 15)

---

## Resources

- [Claude Code documentation](https://code.claude.com/docs/en/overview)
- [Setup](https://code.claude.com/docs/en/setup) · [Authentication](https://code.claude.com/docs/en/authentication) · [Troubleshooting](https://code.claude.com/docs/en/troubleshooting)
- [Commands reference](https://code.claude.com/docs/en/commands) · [CLI reference](https://code.claude.com/docs/en/cli-reference)
- [Changelog](https://code.claude.com/docs/en/changelog)
