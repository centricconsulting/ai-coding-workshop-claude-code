# Spring Boot lab validation (2026-11-03)

> **Status (2026-11-03):** every ⚠️/❌ item below is fixed in the PR that added this report, except the IDE/CLI build race, which is environmental and now documented in Lab 3 troubleshooting and the facilitator guide. Items marked 🖐 in the .NET reports also apply here and are open for the dry run (#37). The `validate/spring` branch referenced below was local.

## Summary

The Spring-specific steps of Labs 1–11 were run in the `springboot-participant` dev container: real Maven builds and tests for Lab 3 (both parts), Lab 5 (characterization tests + Phase 1), Lab 8 (B.1 + the `/security-checklist` skill), and Lab 9 (a Spring test-run hook). Stack-agnostic steps (Labs 10, 12–15) were covered in the .NET reports.

**The dev container works and the Spring path works end to end**, ending at 132 passing tests. Fixes:

| # | Lab | Finding | Fix |
|---|---|---|---|
| 1 | 3 | Step 3 asked for a `NotificationService` class implementing the `NotificationService` interface (impossible in Java) | Name it `DefaultNotificationService` in Steps 2–3 |
| 2 | 3 | Annotating it `@Service` (natural in Spring) breaks every `@SpringBootTest`: no `EmailSender`/`SmsSender` beans | Step 3: keep it a plain class |
| 3 | 3 | Adding fields to `CreateTaskRequest` breaks ~10 existing two-argument constructor calls | Step 9: keep a two-argument constructor |
| 4 | 9 | No working Spring path for C.5 (no formatter configured) or C.6 (.NET-only) | Spring note for C.5; tested `run-java-tests.sh` for C.6 |
| 5 | 1 | No expected Spring test result anywhere | "about 100 tests, all passing" in Lab 1, README, LOCAL_SETUP, CLAUDE.md, kickoff deck, guide |
| 6 | 3 | VS Code's Java extension shares `target/` with Maven, causing `Unresolved compilation problem` errors | Troubleshooting + guide: `mvn clean test` |
| 7 | 5 | `ITaskOutputWriter` is a .NET name | `TaskOutputWriter` for Spring |
| 8 | — | Stale `src-springboot/coverage-summary.txt` | Removed |

---


## Dev container + Lab 1

- ✅ `devcontainer up` with `.devcontainer/springboot-participant/` succeeds; `postCreateCommand` installs Claude Code and `mvn clean install -DskipTests` passes (BUILD SUCCESS, ~47 s).
- ✅ All tools the Spring track uses are present: Java 21, Maven 3.9, `claude`, `jq` (Lab 9 hooks), Node (Lab 10 `npx`), `gh`.
- ✅ Baseline `mvn clean test`: **102 tests, 0 failures, 0 errors, 0 skipped**.
- ⚠️ Lab 1 D.1, the README, LOCAL_SETUP, and the kickoff deck give the .NET expected result ("1 passed, 11 skipped, 0 failed") but nothing for Spring. Add "Spring Boot: about 100 tests, all passing (none skipped)". Low.

## Lab 2 — Spring paths

- ✅ Path-scoped rules load for Java files: reading the domain `Task.java` in a fresh headless session pulled in `src-springboot/CLAUDE.md`, `.claude/rules/springboot.md`, and the C.3 `domain-layer.md` (glob `src-springboot/taskmanager-domain/**/*.java`).

## Lab 3 — TDD & Requirements → Code (Spring), executed end to end

- ✅ Starter assumptions hold: `create(title)`, `create(title, description)`, `reconstitute(...)`, MockMvc `TaskApiIntegrationTest`, `GlobalExceptionHandler` maps `IllegalArgumentException` → 400, `dev` profile uses H2 `create-drop`.
- ❌ **Step 1/3 name clash.** Step 1 designs a `NotificationService` **interface**; Step 3 says "Create a NotificationService **class** that implements the NotificationService interface". Java can't have both in one package, so Claude must invent a name (`DefaultNotificationService`, `…Impl`) and participants diverge. **Fix:** name the class in Step 3 (`DefaultNotificationService`). Medium.
- ❌ **`@Service` on the implementation breaks every `@SpringBootTest`.** Annotating it like `TaskService` (the natural Spring move) fails context startup with `NoSuchBeanDefinitionException: EmailSender`, so 8 API integration tests error, which only shows up when the *whole* build runs. **Fix:** Step 3 says "keep it a plain class (no `@Service`) until there are sender adapters". Medium.
- ✅ Steps 1–3 otherwise work: Red (cannot find symbol) → Green; Part 1 adds 7 tests.
- ✅ Step 7 `Priority` enum + `PriorityTest`: Red → Green with the lab's own `mvn -q -pl taskmanager-domain test`.
- ✅ Step 8: `priority`/`dueDate` on `Task`, `reconstitute` extended, ripple into `TaskEntity` (Lombok `@Data`) and `TaskMapper` exactly as the Spring note says; H2 `create-drop` needed no migration. 119 tests green.
- ⚠️ **Step 9 ripple not mentioned:** adding `priority`/`dueDate` to the `CreateTaskRequest` record breaks ~10 existing `new CreateTaskRequest(title, description)` calls in `TaskApiIntegrationTest`. **Fix:** tell Claude to keep a two-argument constructor (or to update the existing tests). Low–medium.
- ✅ Step 9: service overload + DTOs + controller; MockMvc tests for HIGH/future → 201 echoing both fields, `URGENT` → 400 ProblemDetail listing valid values, omitted → `MEDIUM`. **124 tests green.** Optional manual check: `mvn -q install -DskipTests` + `spring-boot:run -Dspring-boot.run.profiles=dev` + the guide's `curl` → **201 Created**.
- ⚠️ **IDE/CLI build race (participant hazard).** With VS Code's Java extension (`redhat.java`, installed by this dev container) open on the repo, it compiles into the same `target/` with the Eclipse compiler on every save. `mvn` then skips "up-to-date" classes and tests fail with `java.lang.Error: Unresolved compilation problem` or odd `ClassNotFoundException`s. Seen here because a VS Code dev container for this repo was running alongside. **Fix:** Lab 3 / facilitator troubleshooting: "Strange compile errors from `mvn`? Run `mvn clean test`; the IDE's Java extension shares `target/`". Medium (confusing when it hits).

## Lab 4 — Plan Mode (Spring)

- ✅ The updated B.1 prompt ("log the swallowed exceptions with SLF4J") matches the code: the Spring `LegacyTaskProcessor` swallows both `InterruptedException` and `IOException`. A.3's Spring Javadoc target (`TaskId`) has an existing Javadoc to replace. Plan-mode mechanics were validated on .NET (stack-agnostic).

## Lab 5 — Refactoring (Spring), executed

- ✅ Characterization tests under `taskmanager-infrastructure/src/test/java/` compile and run with no project changes (unlike .NET, no new module reference is needed): 8 tests green against the current code.
- ✅ Phase 1 renames (`chk→canProcess`, `rdy→ready`, `getCount→getProcessedCount`, …) with `LegacyBatchImportService` and the tests updated in the same step (as the #52 wording now says): **132 tests green**.
- ℹ️ Spring's `LegacyTaskProcessor` is a `@Component` singleton with mutable state, so the shared bean leaks `processedCount`/`lastResult` across callers. That's a good extra smell for Step 1's review; no change needed.
- ⚠️ Phase 4 says "Extract the file write into an `ITaskOutputWriter` abstraction"; the `I` prefix is a .NET convention. **Fix:** "`ITaskOutputWriter` (.NET) / `TaskOutputWriter` (Spring)". Low.

## Lab 6 — Checkpoints (Spring)

- ✅ B.7's note is accurate: Spring characterization tests leave `task_{id}.txt` in the `taskmanager-infrastructure/` module directory (3 files after one run).

## Lab 8 — Security Review (Spring), executed with the #53 flow

- ✅ B.1 Spring prompt implemented as written: `organizationId` from the `X-Organization-Id` header on `POST /api/tasks`, `TaskService.assignTask(taskId, userId, organizationId)` with no ownership check, `PUT /api/tasks/{id}/assign`, 404 via the existing `TaskNotFoundException` handler. Entity + mapper carry both fields; 132 tests green.
- ✅ **C.1**: `/security-checklist src-springboot` (skill built from the C prompt) reported, as finding 1.1, "`assignTask` takes `organizationId` but never uses it… checks that the task exists, not who owns it", with file:line and a fix. The restructured lab works on the Spring track too. (`/security-review` itself was validated on .NET; its exploitability reasoning is stack-independent.)

## Lab 9 — Hooks (Spring)

- ⚠️ **C.5 and C.6 have no working Spring path.** C.5 suggests `mvn spotless:apply` "if the project uses Spotless", but no module configures Spotless; C.6's test-run hook is .NET-only. **Fix:** add a Spring C.6 variant. Validated in the container: a `run-java-tests.sh` PostToolUse hook that maps a changed `*/src/test/java/*.java` path to its module and runs `mvn -q -pl <module> -am test` exits 0 on pass, 0 for non-test files, and **2 with the assertion message** when a test fails. Tell Spring participants to skip C.5 (no formatter configured) or treat it as read-only. Medium.

## Lab 11 — Skills (Spring)

- ✅ `/test-data-generator Task 3 java` read the current `Task.java` (including the lab additions `Priority`, `dueDate`, `assign`, `inOrganization`) and built data only through `Task.create(...)` + business methods.

## Labs 10, 12–15

- ℹ️ Stack-agnostic; validated in the .NET pass (`docs/validation/day2-dotnet.md`). Spring-specific paths they mention (`src-springboot/taskmanager-api`, `taskmanager-infrastructure`) exist.

## Repo hygiene

- ⚠️ `src-springboot/coverage-summary.txt` is a stale generated file from the initial import (unexpanded `$(date)` placeholder, referenced nowhere). Removed.
