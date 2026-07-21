# Workshop Completion Plan

**Created**: 2026-07-21
**Status**: In progress
**Goal**: Verify and polish the 2-day restructure (commit `4c50590`) before pushing to `origin/main`. Work through this checklist together; delete this file once everything is checked off and pushed.

---

## Context

The workshop was just restructured from 1-day/6-hours to 2-days/6-hours-each, adding 5 new labs+modules (Plan Mode, Checkpoints & Rewind, Security Review, Skills, Output Styles) and renumbering everything 1–15. Automated checks already passed: no dangling links, nav-footer chains walk cleanly, lab/module numbers agree everywhere, duration arithmetic checks out. What's left is things automation can't catch — human review of AI-generated content, and a couple of real bugs found during a follow-up audit.

---

## P0 — Bugs found, block push

- [x] **1. Stale `LegacyTaskProcessor` path in `docs/labs/lab-05-refactoring-and-context.md`** (line 45, .NET prompt) — fixed to `src/TaskManager.Infrastructure/Legacy/LegacyTaskProcessor.cs`.

- [x] **2. Same stale path in `docs/labs/lab-13-headless-automation.md`** (line 35, `--print` example) — fixed.

- [ ] **3. Decide on `.devcontainer/maintainer/devcontainer-lock.json`**
  Modified in the working tree, unrelated to this session's work (was already dirty before we started). Commit it, revert it, or find out why it keeps changing — shouldn't ride along in the restructure commit either way.

---

## P1 — Human review (nothing automated can check this)

- [ ] **4. Read through the 5 new labs** for tone/accuracy/consistency with the other 10 — they were written by background agents and haven't had human eyes on them yet:
  - `docs/labs/lab-04-plan-mode-and-extended-thinking.md`
  - `docs/labs/lab-06-checkpoints-and-rewind.md`
  - `docs/labs/lab-08-security-review-workflows.md`
  - `docs/labs/lab-11-skills.md`
  - `docs/labs/lab-14-output-styles-and-personas.md`

- [ ] **5. Read through the 5 new modules** (matching slide decks) for the same reason:
  - `docs/presentations/modules/part1/04-plan-mode-extended-thinking.md`
  - `docs/presentations/modules/part1/06-checkpoints-and-rewind.md`
  - `docs/presentations/modules/part1/08-security-review.md`
  - `docs/presentations/modules/part2/02-skills.md`
  - `docs/presentations/modules/part2/05-output-styles.md`

- [ ] **6. Sanity-checked, not yet dry-run**: Lab 8's exercise adds `AssignedToUserId`/`OrganizationId` to the `Task` entity. Confirmed no naming collision with the existing `Task.cs`/`Task.java` (both currently have no such fields) — but nobody has actually run the lab's prompts against a live Claude Code session yet to confirm they produce what's described.

- [ ] **7. Spring Boot prompt precision**: a few prompts (e.g. Lab 5 Step 1) say "the LegacyTaskProcessor in `src-springboot/`" rather than the full path, unlike the .NET prompts which are always exact. Not wrong, just less precise — worth a pass to see if it's worth tightening or if it's intentionally loose because Spring Boot participants are expected to navigate by class name.

---

## P2 — Polish, nice to have before push

- [ ] **8. Marp render check**: export at least one new module to HTML/PDF and eyeball it — confirm frontmatter, slide breaks, and tables render as expected (nobody has actually opened these in Marp yet, only read the raw markdown).
  ```bash
  npx @marp-team/marp-cli docs/presentations/modules/part1/04-plan-mode-extended-thinking.md --html
  ```

- [ ] **9. Facilitator timing sanity check**: read through Day 1 and Day 2 schedules once more against the new content — does 20 min feel right for Plan Mode, 30 for Security Review, etc., or does anything need adjusting now that the actual lab content exists (durations were originally estimated before the labs were written)?

- [ ] **10. Final link/numbering re-check** after any edits from items 1–9 land — rerun the grep-based verification pass used during the restructure (dangling links, `Lab [0-9]` consistency, nav-footer chain walk) since any content edits could introduce new stale references.

---

## Done (from the restructure pass, verified automated)

- [x] No dangling internal markdown links
- [x] No stale "Part 1"/"Part 2" prose outside folder paths
- [x] No orphaned old filenames referenced anywhere
- [x] Lab and module nav-footer chains walk 1→15 cleanly, no gaps/loops
- [x] Module H1 numbers are gap-free 1–15
- [x] Schedule-table duration arithmetic agrees across README, labs/README, presentations/index
- [x] Zero "Copilot" references anywhere in the repo (`.md`/`.cs`/`.java`)
- [x] Peripheral files (`CLAUDE.md` x3, `.claude/settings.json`, test stubs, agent-governance guide, checklists) all updated to match new lab numbers

## Done (this session)

- [x] **README `## Schedule` reworked to foreground modules, not labs.** Each row now leads with **Module N**, notes the paired lab as `(+ Lab N)`, and an intro line clarifies the duration shown is the combined module+slide-deck-plus-lab block — with a pointer to `docs/presentations/index.md` for slide-only durations. Module numbers used match each file's global H1 number, which is identical to its paired lab number for every module except the two Day 1 intro modules (Kickoff, Features Tour) and the two wrap-ups, none of which have a lab of their own. Worth a quick recheck against item 9 (facilitator timing) once that's done, since this table is the most likely one to drift if durations change.

---

## When this is done

1. Check off everything above
2. Delete this file
3. Commit the deletion + any fixes together (or as a final small commit)
4. Push to `origin/main`
