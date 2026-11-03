# Day 1 lab validation — .NET path (2026-11-03)

> **Status (2026-11-03):** all nine "Fix first" items and the low items were fixed in #52, except two left for a live check: whether interactive `/effort` accepts `auto`, and whether `/code-review` still has an `ultra` level. Items marked 🖐 are still open for the dry run (#37). The `validate/day1-dotnet` branch referenced below was local and has been deleted; line numbers refer to the lab guides as they were before #52.

## Summary

Labs 1–7 were run on the .NET track on a local branch `validate/day1-dotnet` (not pushed; main untouched). Code steps were executed for real (Lab 3 Parts 1–2, Lab 5 Step 2 + Phase 1 + integration-test extension); Claude-behavior claims were checked with fresh headless `claude -p` sessions (rule loading, plan mode, the Lab 6 over-broad edit, `/code-review`). Interactive UI (Shift+Tab, rewind menu, `/context`, `/memory`, plan approval) is marked 🖐 for the live dry run (#37).

**Every lab's main path works.** No blockers. Final state: 40 unit + 2 integration tests passing.

### Fix first (medium–high)

| # | Lab | Finding | Suggested fix |
|---|---|---|---|
| 1 | 6 (#40) | The over-broad B.3 edit changed behavior (removed `Thread.Sleep`, narrowed `catch`, sealed class) but **all characterization tests still passed**, so "Did anything break?" gets "No". | Make the lesson explicit ("tests only pin what they assert — read the diff"), or use a prompt that breaks a pinned behavior (e.g. "…and trim input whitespace"). |
| 2 | 3 (#24) | Step 5 `git diff` shows **nothing**: all Part 1 files are new/untracked. | Use `git status` + `git add -A && git diff --staged --stat`. |
| 3 | 3 (#24) | Step 2 needs email/SMS channel dependencies that Step 1 never defines; participants' results will diverge (risk of false greens). | Step 1: design `INotificationService` **and** the `IEmailSender`/`ISmsSender` ports. |
| 4 | 3 (#24) | `Priority` collides with `FakeItEasy.Priority` (CS0104) in `UnitTest1.cs` during Step 9; everyone hits it. | Troubleshooting entry + alias `using Priority = TaskManager.Domain.Tasks.Priority;`. |
| 5 | 7 (#26) | Prereq "uncommitted changes from Labs 3, 5, 6" contradicts the commit-per-lab guidance (Labs 4–5, facilitator guide); Steps 3–4 find nothing to stage. | Point Steps 3–4 at Step 2's doc-comment changes or at `git log origin/main..HEAD`. |
| 6 | 5 (#25) | Phase 1 renames break the build until the caller is updated, but caller updates are Step 4. | Update the caller and tests in Phase 1; make Step 4 "confirm no other callers". |
| 7 | 4 (#39), 5 | "ILogger instead of Console" / "SLF4J instead of System.out": neither legacy class has Console/System.out. The real smell is the swallowed exception. | Reword to "log the swallowed file-write exception". |
| 8 | 4 (#39) | A.3 asks to add a doc comment to `TaskId`, which already has one (both stacks). | Pick an undocumented target, or say "replace". |
| 9 | 2 (#23) | C.3's prompt doesn't touch a domain file, so `domain-layer.md` never auto-loads by path; the teaching point is lost. | Prefix: "Look at src-dotnet/src/TaskManager.Domain/Tasks/, then show me…". |

### Low (wording / polish)
`--permission-mode default` → prefer `manual` (still accepted) · `/effort auto` (CLI rejects `auto`; confirm interactive) · `null!` rule vs. null-argument tests · Lab 5 needs a test→Infrastructure project reference · stale "Lab 2/Lab 3" comments in `TaskService.cs`, `Program.cs`, `/health` · Lab 5 extension targets (`string taskId`, `List<Task>`) don't exist · Lab 3 stub names vs. requested tests · Lab 2 D.1 "all Skip stubs" stale since #50 · `springboot.md` glob shown as `pom.xml` · `git reset --hard` is denied in settings.json (Lab 6 Part C) · `ultra` `/code-review` level unconfirmed.

---

Environment: macOS (Darwin 27.2), Claude Code 2.1.317, .NET SDK 10.0.403 (targets net9.0), jq 1.7.1, model Opus 5.5.
Branch: `validate/day1-dotnet` (local only, from main @ 85427a2).
Legend: ✅ verified · ⚠️ issue found · 🖐 manual-only (needs a live interactive session)

## Lab 1 — First Contact (#22)

- ✅ `claude --version` → 2.1.317.
- 🖐 `claude doctor`, `/status`, `/login`, auth flows, trust prompt, Shift+Tab cycling, `/context` display, keyboard shortcuts.
- ✅ Starter domain matches B.2 expectations: .NET `Task` has Title, Description, Status (no Priority/DueDate); Spring `Task.java` exists.
- ✅ `LegacyTaskProcessor` exists in both stacks (B.3).
- ✅ `.claude/` contains agents (3), skills (test-data-generator), rules (3), settings.json (B.2).
- ✅ D.1: `dotnet test src-dotnet` → 1 passed, 11 skipped, 0 failed. `Bash(dotnet *)` is in the allow list, so no prompt — matches the text.
- ✅ D.1: settings.json denies `rm -rf *` and `git push --force*` as stated.
- ✅ D.2: no `Write`/`Edit` allow rule, so a file write in Manual mode will prompt — matches.
- ✅ Audit-log hooks run with jq and write to `.claude/audit.log`, which is gitignored (`*.log`).
- ⚠️ **`--permission-mode default` is a legacy alias.** `claude --help` lists choices `acceptEdits, auto, bypassPermissions, manual, dontAsk, plan`. `default` is still accepted (tested headless), but the docs should lead with `manual`. Affects Lab 1 (L146, L269), Lab 4 (L61), Lab 9 (L51), Lab 13 (L99 table), features-tour deck (L92). Low severity.
- ⚠️ **`/effort ... or auto`** (L121): `claude --effort auto` is rejected ("Valid values: low, medium, high, xhigh, max"). The interactive `/effort` may still accept `auto`; 🖐 confirm, otherwise drop `auto`. Low severity.
- 🖐 "Auto … the default starting mode in recent versions" (C.2 table; tour deck L88) depends on plan/model/admin settings; confirm in a fresh participant account.

## Lab 2 — CLAUDE.md & Project Memory (#23)

- ✅ Three `CLAUDE.md` files exist (root, `src-dotnet/`, `src-springboot/`); `src-dotnet/tests/` has none (Part D premise holds).
- ✅ Rule frontmatter matches the C.1 table (`dotnet.md`, `springboot.md` scoped; `follow-up-questions.md` unscoped). Minor: the table shows `pom.xml`; the actual glob is `src-springboot/**/pom.xml`.
- ✅ **C.2 live test** (fresh headless session): reading `Task.cs` loaded root `CLAUDE.md` + `follow-up-questions.md` at start, then `src-dotnet/CLAUDE.md`, `dotnet.md`, and the new `domain-layer.md` on first read. Exactly as described.
- ⚠️ **C.3 rule doesn't auto-load with the given prompt.** "Show me (don't write it yet) a small Effort value object…" doesn't touch any domain file, so `domain-layer.md` never loads by path. In the headless run Claude read the rule files explicitly, so output still followed the rule (`FromName`, message lists valid values), but the teaching point ("the rule loaded because of the path") isn't demonstrated. **Fix:** prefix the prompt the way B.2 does: "Look at src-dotnet/src/TaskManager.Domain/Tasks/, then show me…". Medium severity.
- ✅ **D.4 live test**: reading `UnitTest1.cs` auto-loaded `src-dotnet/tests/CLAUDE.md` and, through `@../../.claude/rules/domain-layer.md`, the domain rule. The import path in D.3 is correct. The generated test followed the tests file (removed the stub's `Skip`, `MustHaveHappenedOnceExactly`, `TestContext.Current.CancellationToken`).
- ⚠️ D.1 expectation "test methods that are currently `Skip` stubs" is slightly stale since #50: one real test (`HealthEndpointTests`) now passes. Suggest "mostly `Skip` stubs, plus one passing `/health` integration test". Low severity.
- 🖐 `/context` memory-files view, `/memory` (including the auto-memory toggle), auto-memory "Saved … memory" message, `/init`, `CLAUDE_CODE_NEW_INIT=1`, `/doctor prompt-audit`, B.1/B.2 side-by-side comparison (model-dependent).

## Lab 3 — TDD & Requirements → Code (#24)

### Part 1 (NotificationService), executed end to end
- ✅ Step 1–3 work as written: interface in Application → tests → Red (compile error `NotificationService` not found, as 2.2 predicts) → Green (7 tests pass; totals 8 passed, 11 skipped, 0 failed).
- ⚠️ **Step 2 needs channel dependencies nobody defined.** "SendBothAsync throws if either channel fails" + "Use FakeItEasy for any dependencies" forces Claude to invent sender ports (`IEmailSender`/`ISmsSender`) mid-Red-phase; the Step 1 interface prompt doesn't mention them. Expect divergence between participants (some will fake `INotificationService` itself and get a false green). **Fix:** in Step 1, add "…and the IEmailSender and ISmsSender ports it will depend on". Medium.
- ⚠️ **`null!` conflict.** The null-task tests require passing `null` with nullable enabled, but `src-dotnet/CLAUDE.md` says "Do not use `null!` suppress". Claude has to break a rule or contort the test. **Fix:** add "(tests that pass null on purpose are the exception)" to that CLAUDE.md line. Low–medium.
- ⚠️ **Step 5 `git diff` shows nothing.** All three files are new and untracked, so `git diff` is empty, contradicting "You should see: the new interface file, the test file, the implementation file". **Fix:** use `git status` then `git add -A && git diff --staged --stat` (or `git add -N .` before `git diff`). Same issue in Step 11 is avoided because it uses `git status`. Medium (visible, confusing).
- 🖐 Step 4 refactor result is model-dependent (little to do on a 25-line class).

### Part 2 (Requirements → Code), executed end to end
- 🖐 Step 6 plan mode (`/plan`, "No, keep planning", "Yes, manually approve edits") is interactive. Backlog file written to `docs/backlog/task-priorities.md` with the guide's 5-item shape.
- ✅ Step 7 `Priority`: Red (CS0246) → Green; `domain-layer.md` loads for `src-dotnet/src/TaskManager.Domain/**` and the result has `FromName` + valid-values message. `UnitTests` already references Domain, so `tests/.../Domain/PriorityTests.cs` works without project changes.
- ✅ Step 8 `DueDate`/`Priority` on `Task`: Red (CS1061/CS1501) → Green; 2-arg `Create` kept, so `NotificationServiceTests` still compiles. Closing the title TODO works with `ArgumentException.ThrowIfNullOrWhiteSpace`.
- ⚠️ **`Priority` collides with `FakeItEasy.Priority`** (CS0104) in any test file with `using FakeItEasy;` + `using TaskManager.Domain.Tasks;`, i.e. `UnitTest1.cs` in Step 9. Every participant hits it. Claude fixes it with an alias, but it costs a cycle. **Fix:** add to Lab 3 troubleshooting ("CS0104 'Priority' is ambiguous → `using Priority = TaskManager.Domain.Tasks.Priority;`"), or mention it in `src-dotnet/tests/CLAUDE.md` guidance. Medium.
- ✅ Step 9 application: three `AddTaskAsync` stubs replaced; Red (CS1503) → Green (25 passed, 3 skipped). `ITaskRepository.AddTaskAsync` + `SaveChangesAsync` exist as the prompt assumes.
- ⚠️ Minor: the stub names being replaced (`…WithNullTitle…`, `…WithEmptyDescription_ShouldThrowArgumentException`) don't match the three tests the prompt asks for, and "empty description throws" contradicts nothing else in the lab. Claude replaces them fine; just expect participants to ask. Low.
- ✅ Step 9 API: `POST /tasks` + adapted `CreateTaskRequest`; `dotnet run` listens on `http://localhost:5215` (matches launchSettings). curl #1 → `201 Created` + `Location: /tasks/{id}`; curl #2 → `400` ProblemDetails "Valid values are: Low, Medium, High, Critical". Exactly as expected.
- ✅ Step 10 premise: `docs/adr/` doesn't exist yet.
- ⚠️ **Stale lab numbers in starter code comments**: `TaskService.cs` says "Lab 2: Requirements → Backlog → Code" and throws "implemented during Lab 2"; `Program.cs` header says "Lab 3: Code Generation & Refactoring"; `/health` returns "ready for Lab 3!". Participants read these. Low.

## Lab 4 — Plan Mode, Permission Modes & Thinking (#39)

- ✅ **Plan mode blocks edits** (headless `--permission-mode plan`): asked to edit `TaskId.cs`; Claude produced a plan, wrote nothing to the repo (`git status` clean). Plans are auto-saved under the user's Claude config `plans/` folder, which supports B.3's "save it to docs/plans" step being a deliberate extra write.
- ✅ `opusplan` model alias exists (C.3).
- ✅ **B.1 plan quality** (headless plan mode, same prompt): phases were characterize → rename → guards → enum → extract → logging, each with a test gate, and it named the riskiest phase. Matches "Expect a plan along these lines". Saved to `docs/plans/legacy-refactor-plan.md` for Lab 5.
- ⚠️ **A.3 target already has the comment.** `TaskId` already has an XML doc comment (.NET) / Javadoc (Spring), so "Add a one-line XML doc comment to the TaskId class" makes Claude ask whether to replace it, which muddies the Manual vs. accept-edits comparison. **Fix:** pick a target without one (e.g. "…to the `TaskStatus` enum", check first) or say "Replace the XML doc comment on TaskId with a one-line one". Medium.
- ⚠️ **"ILogger instead of Console" / "SLF4J instead of System.out" don't apply.** Neither `LegacyTaskProcessor` (.NET or Spring) contains `Console`/`System.out`; the real logging smell is the swallowed exception (`catch { // ignore }`). Claude noticed and reframed it, but the prompt sets the wrong expectation and Lab 5 likely inherits it. **Fix:** say "log the swallowed file-write exception with ILogger/SLF4J". Medium.
- ⚠️ `default` vs `manual` naming (see Lab 1): the A.1 table labels the mode `default` "(shown as **Manual**)"; the CLI now calls it `manual`.
- ⚠️ `/effort … |auto` and "`/effort auto` resets to the model default" (C.1, C.2): `--effort auto` is rejected by the CLI; 🖐 confirm the interactive command.
- 🖐 Shift+Tab cycle order and status-bar strings, the three plan-approval choices, `Ctrl+G`, `/effort status`, `ultrathink` rainbow, `Option+T`, Plan/Explore subagent use during planning.

## Lab 5 — Multi-File Refactoring & Context (#25)

- ✅ Overview is accurate: `LegacyBatchImportService` is the only caller and uses `Init`, `Chk`, `ProcessTask`, `GetCount`, `GetLast`.
- ⚠️ **Characterization tests need a project reference first.** `TaskManager.UnitTests` references Domain and Application but not Infrastructure, where `LegacyTaskProcessor` lives. Claude adds `<ProjectReference>` (no new package, so it's allowed), but participants may wonder whether that violates "ask before adding dependencies". **Fix:** one line in Step 2: "Claude will need to add a project reference to TaskManager.Infrastructure; that's expected". Low–medium.
- ✅ Step 2: 15 characterization tests written and passing against the current code, including the odd behavior (`Chk` false before `Init`, `Init` resets the count but not the last result, 500-char limit).
- ⚠️ Side effects: type-1 + flag calls write `task_{id}.txt` into the test working directory (`bin/…`, gitignored) and `Thread.Sleep(100)` each. Harmless; worth one line in the facilitator guide so nobody's surprised. Low.
- ✅ Phase 1 renames (`Chk→CanProcess`, `_rdy→_isReady`, `GetCount→GetProcessedCount`, …) across class, caller, and tests: 40 unit tests green.
- ⚠️ **Step order: callers must change during Phase 1, not Step 4.** Renaming public methods breaks the build until `LegacyBatchImportService` and the tests are updated, so "run the characterization tests after each rename" can't pass in Phase 1 as worded. Claude updates the caller in Phase 1 anyway, which leaves Step 4 as a verification step. **Fix:** say so in Phase 1 ("…and update LegacyBatchImportService and the tests to match") and reframe Step 4 as "confirm no other callers exist". Medium.
- ✅ **Extension (new in #50):** implemented `CreateTask_WithValidData_ShouldReturn201` from the stub using `HealthEndpointTests` as the pattern: passes against Lab 3's `POST /tasks` with no production changes (IntegrationTests 2 passed, 4 skipped).
- ⚠️ Extension targets that don't exist: "wrap `string taskId`" (the legacy code uses `int id`) and "wrap `List<Task>`" (no such list in the legacy code). **Fix:** "wrap `int id` in a value object" / "wrap the batch `List<(int, string, int)>` in a first-class collection". Low.
- 🖐 Phases 2–4 not executed (model-dependent; mechanics proven by Phase 1). `/context`, `/compact [instructions]`, `/usage`/`/cost`, auto-compact.

## Lab 6 — Checkpoints & Rewind (#40)

- ✅ Prerequisite state holds: after Lab 5 Phase 1 the refactored processor is committed and the characterization tests pass.
- ⚠️ **B.3's damage isn't caught by the tests.** Ran the exact over-broad prompt headlessly (acceptEdits) on the post-Lab-5 file: Claude rewrote ~166 lines, **removed `Thread.Sleep(100)`, narrowed the swallowed `catch` to `IOException`/`UnauthorizedAccessException`, and sealed the class**, all real behavior changes, yet **all 15 characterization tests still passed**. So B.4's "Run the characterization tests. Did anything break?" will usually get "No", and some participants will conclude there's nothing to rewind. The damage is visible only in `git diff`. **Fix (pick one):** (a) make the lesson explicit: "Tests may still pass. That's the point: characterization tests only pin what they assert. Read the diff."; or (b) use a prompt that reliably breaks a pinned behavior, e.g. "…and trim whitespace from the input before processing" (breaks the type-2 and swap-case expectations). Medium–high; it's the core of the lab.
- ✅ Runtime side effects behave as A.3/B.7 describe: characterization tests leave `task_{id}.txt` in the .NET test project's `bin/` output (verified in Lab 5). 🖐 Spring location (`taskmanager-infrastructure/`) not checked.
- ℹ️ Part C recommends `git reset --hard` as a human action; note that `.claude/settings.json` **denies** `Bash(git reset --hard*)`, so if a participant asks Claude to run it, it's refused. That's a nice Lab 9 tie-in; worth one sentence here so nobody thinks it's broken.
- 🖐 Everything about the rewind UI: `Esc Esc`, `/rewind` (and aliases `/checkpoint`, `/undo`), the restore/summarize options, Bash-made files surviving rewind (B.7), the post-`/clear` `/resume` entry.

## Lab 7 — Git Workflow & PR Preparation (#26)

- ✅ **`/code-review low src-dotnet/src/TaskManager.Infrastructure/Legacy`** runs headlessly with the documented syntax (level + path target), reviews the branch diff for that path, and correctly reports no findings for the rename-only refactor: "No logic changed in any hunk". No files modified.
- ✅ `/code-review`, `/security-review`, and `/simplify` are present as bundled skills in 2.1.317. `gh` is available.
- ⚠️ **Prerequisite contradicts earlier labs.** Lab 7 assumes "Uncommitted changes from Labs 3, 5, and 6", but Lab 4 requires "`git status` clean", Lab 5 requires "Lab 3 work committed", and the facilitator guide says to commit at the end of every lab. Following those, Step 3 ("Stage the files") and Step 4 find nothing to stage. `/code-review` still works (it covers commits ahead of upstream), but the commit steps don't. **Fix:** either have Lab 6 end with uncommitted work, or change Lab 7 Steps 3–4 to "If you've been committing per lab, run Step 4 against `git log origin/main..HEAD` instead, or practice on Step 2's doc-comment changes, which are uncommitted". Medium.
- ✅ Step 2 targets exist (`NotificationService` from Lab 3, the refactored `LegacyTaskProcessor` from Lab 5). Step 2's doc-comment edits do give Steps 3–4 something to stage, so the fix above can lean on that.
- 🖐 `/code-review` running as a background subagent, the `ultra` level (not listed in the skill's own level description, which shows `low|medium|high|xhigh|max`; confirm it still exists), `--fix` editing outside checkpoints, `--comment` on a PR, the `/review` alias history note (v2.1.223), `/pr-comments` removal, `gh pr create` flow.
