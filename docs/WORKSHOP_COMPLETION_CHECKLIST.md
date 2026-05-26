# Workshop Completion Checklist

Tracks everything needed before this workshop is ready to deliver. Items are grouped by category and roughly ordered by dependency.

---

## 1. Presentation Modules

13 of 14 modules are missing. The kickoff (`part1/00`) exists; everything else needs to be written.

### Part 1 Modules (5 missing)
- [ ] `docs/presentations/modules/part1/01-claude-code-features-tour.md` — CLI tour: slash commands, keyboard shortcuts, output format, IDE integration options
- [ ] `docs/presentations/modules/part1/02-claude-md-memory.md` — CLAUDE.md hierarchy, what belongs in it, live comparison demo
- [ ] `docs/presentations/modules/part1/03-tdd-with-claude-code.md` — TDD workflow walkthrough, propose-before-write discipline, diff review
- [ ] `docs/presentations/modules/part1/04-refactoring-and-context.md` — characterization tests, refactoring phases, `/compact` timing
- [ ] `docs/presentations/modules/part1/05-git-workflow.md` — `/review`, conventional commits, PR descriptions from session context
- [ ] `docs/presentations/modules/part1/06-wrapup-part1.md` — key takeaways, anti-patterns, bridge to Part 2

### Part 2 Modules (8 missing)
- [ ] `docs/presentations/modules/part2/00-welcome-recap.md` — Part 1 recap, Part 2 roadmap
- [ ] `docs/presentations/modules/part2/01-permissions-safety.md` — permissions model, allow/deny hierarchy, safety principles
- [ ] `docs/presentations/modules/part2/02-hooks-automation.md` — hook events, writing hooks, good vs. bad hook candidates
- [ ] `docs/presentations/modules/part2/03-mcp-servers.md` — MCP protocol, available servers, when to build custom
- [ ] `docs/presentations/modules/part2/04-sub-agents.md` — when sub-agents appear, structuring for parallelism, scoping large tasks
- [ ] `docs/presentations/modules/part2/05-headless-automation.md` — `--print`, `--continue`, CI patterns, safety in headless mode
- [ ] `docs/presentations/modules/part2/06-capstone-governance.md` — capstone framing, team config governance, ownership models
- [ ] `docs/presentations/modules/part2/07-wrapup-next-steps.md` — key takeaways, next steps, resources

### Presentation Infrastructure
- [ ] `docs/presentations/README.md` — how to view with Marp, export to PDF/HTML, delivery patterns

---

## 2. Facilitator Guides

- [ ] `docs/FACILITATOR_GUIDE.md` — Part 1: timing, talking points, demo scripts, common participant issues, dual-stack delivery notes
- [ ] `docs/FACILITATOR_GUIDE_PART2.md` — Part 2: same structure, advanced topic facilitation notes

---

## 3. Supporting Participant Docs

- [ ] `docs/LOCAL_SETUP.md` — manual setup guide for participants who can't use devcontainers (install Node, Claude Code, .NET/.Java locally)
- [ ] `docs/PRE_WORKSHOP_CHECKLIST.md` — participant checklist to complete before attending: API key, fork, build verified, Claude Code authenticated

---

## 4. Referenced Guides (linked from labs but not yet written)

- [ ] `docs/guides/agent-governance.md` — linked from Lab 10; governance model for team Claude Code configurations: ownership, versioning, change process
- [ ] `docs/guides/claude-code-decision-guide.md` — when to use CLAUDE.md vs. hooks vs. MCP vs. headless; parallel to the Copilot customization decision guide

---

## 5. Content Validation (test the labs)

Each lab needs to be walked through end-to-end with actual Claude Code before the workshop is deliverable.

- [ ] **Lab 1** — verify all slash commands (`/status`, `/cost`, `/compact`, `/review`, `/clear`) behave as described
- [ ] **Lab 2** — verify CLAUDE.md hierarchy loads correctly; test the `tests/CLAUDE.md` exercise produces meaningfully better output
- [ ] **Lab 3** — run the full TDD cycle; verify Claude Code proposes before writing when asked; verify test-run loop works
- [ ] **Lab 4** — verify `/compact` timing and behavior; confirm `LegacyTaskProcessor` has enough problems to fill 45 minutes
- [ ] **Lab 5** — verify `/review` command behavior and output format
- [ ] **Lab 6** — verify hook variable names (`$CLAUDE_TOOL_INPUT_FILE_PATH`, etc.) are correct; test all hook examples fire correctly
- [ ] **Lab 7** — verify MCP package names: `@modelcontextprotocol/server-filesystem` and `@modelcontextprotocol/server-github`; verify `.claude/mcp.json` is the correct config location
- [ ] **Lab 8** — confirm sub-agent behavior is observable in output; verify parallel task examples actually parallelize
- [ ] **Lab 9** — verify `--print`, `--continue`, `--resume` flag names and behavior; check `--output-format text` flag exists; verify `--session` flag syntax
- [ ] **Lab 10** — walk through capstone scenario end-to-end; verify the peer review exercise works in group settings

### Specific things to verify in Claude Code docs
- [ ] Confirm the Claude Code VS Code extension ID is `anthropics.claude-code`
- [ ] Confirm `--session` flag syntax for named sessions
- [ ] Confirm `--output-format` flag exists and accepted values
- [ ] Confirm hook environment variable names (`$CLAUDE_TOOL_INPUT`, `$CLAUDE_TOOL_INPUT_FILE_PATH`)
- [ ] Confirm `.claude/mcp.json` vs. `~/.claude/claude_desktop_config.json` — which takes precedence, which is project-level

---

## 6. Code / Codebase Review

- [ ] **Verify `LegacyTaskProcessor` is sufficiently problematic** for Lab 4 — needs naming issues, nested conditionals, long methods, missing tests. Check both `.NET` (`src/TaskManager.Infrastructure/Legacy/LegacyTaskProcessor.cs`) and Java (`src-springboot/.../LegacyTaskProcessor.java`) versions.
- [ ] **Add `tests/CLAUDE.md` reference answer** — Lab 2 has participants write it themselves, but a reference version in a `docs/guides/` or separate branch would help facilitators confirm participant output is reasonable
- [ ] **Verify both stacks build and test cleanly** from a fresh clone: `dotnet build && dotnet test` and `mvn clean test`

---

## 7. Repo Hygiene

- [ ] `LICENSE.md` — add appropriate license
- [ ] `CODE_OF_CONDUCT.md`
- [ ] `CONTRIBUTING.md` — how to propose changes, lab feedback process
- [ ] `.github/PULL_REQUEST_TEMPLATE/` — PR templates for participant branches (feature, fix, chore — mirrors Copilot workshop)
- [ ] `.gitignore` — verify it covers Claude Code artifacts (`.claude/` session data if any, audit logs at `/tmp/` are fine)

---

## 8. GitHub Repository Configuration

- [ ] **Branch protection on `main`** — require PR, no direct push (in progress — see conversation)
- [ ] **Repository description** — set on GitHub: "Hands-on workshop for AI-assisted development with Claude Code"
- [ ] **Topics** — add: `claude-code`, `anthropic`, `workshop`, `ai-assisted-development`, `dotnet`, `spring-boot`
- [ ] **README preview** — verify README renders well on the GitHub repo landing page

---

## 9. Pre-Delivery Dry Run

Before the first live delivery:

- [ ] Facilitator completes all 10 labs end-to-end (not just reviews content)
- [ ] Devcontainer tested: open each of the 3 containers, verify Claude Code authenticates and runs
- [ ] Confirm timing estimates in `docs/labs/README.md` are realistic
- [ ] Test fork workflow: fork the repo, clone the fork, add upstream, verify Labs 7 and 9 work against the fork
- [ ] Verify MCP lab works with a real GitHub token against a real repo

---

## Summary Counts

| Category | Total Items | Done |
|---|---|---|
| Presentation modules | 14 | 1 |
| Facilitator guides | 2 | 0 |
| Supporting participant docs | 2 | 0 |
| Referenced guides | 2 | 0 |
| Lab content validation | 15 | 0 |
| Code/codebase review | 3 | 0 |
| Repo hygiene | 5 | 1 (`.gitignore`) |
| GitHub configuration | 4 | 0 |
| Pre-delivery dry run | 5 | 0 |
| **Total** | **52** | **2** |
