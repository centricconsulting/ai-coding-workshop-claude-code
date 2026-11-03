# TaskManager Workshop — Claude Code Project Memory

## What This Project Is

This is a hands-on workshop codebase for learning AI-assisted software development with Claude Code. The application is a **TaskManager API** used as the exercise substrate for all labs.

There are two parallel implementations:
- `src-dotnet/` — .NET 9, Clean Architecture, Minimal APIs, xUnit v3 + FakeItEasy
- `src-springboot/` — Spring Boot 3.x, Clean Architecture, REST, JUnit 5 + Mockito

Both implementations model the same domain. The starter code has the basics: a `Task` aggregate (`TaskId`, title, description, `TaskStatus`) and a repository. Priorities, due dates, notifications, and assignments are added by participants during the labs — read the current code rather than assuming they exist.

## How to Build and Test

```bash
# .NET — a fresh checkout reports 1 passed (a /health integration test), 11 skipped stub tests (implemented during the labs), 0 failed
dotnet build src-dotnet
dotnet test src-dotnet

# Spring Boot
cd src-springboot
mvn clean test
```

## Directory Layout

```
CLAUDE.md                    ← you are here (project root)
src-dotnet/                  ← .NET solution: TaskManager.sln, src/, tests/ (see src-dotnet/CLAUDE.md)
src-springboot/              ← Spring Boot solution (see src-springboot/CLAUDE.md)
docs/labs/                   ← Step-by-step lab guides
docs/presentations/          ← Marp slide decks
docs/guides/                 ← Reference guides (agent design, customization)
.claude/settings.json        ← Permissions and hooks (Lab 9 teaching artifact; hooks need jq)
.claude/agents/              ← Subagents: architecture-reviewer, backlog-generator, test-strategist
.claude/skills/              ← Skills: test-data-generator
.claude/rules/               ← Path-scoped rules (dotnet.md, springboot.md) + follow-up-questions.md
```

## Key Conventions (Both Stacks)

- **TDD first**: write tests before implementation
- **Clean Architecture**: changes flow inward (API → Application → Domain); never the reverse
- **Conventional Commits**: `feat:`, `fix:`, `test:`, `refactor:`, `docs:`, `chore:`
- **No dead code**: remove unused code rather than commenting it out

## What NOT to Do

- Do not add NuGet packages or Maven dependencies without asking first
- Do not modify `.devcontainer/` files during labs
- Do not commit to `main` directly — work on a personal branch
- Do not run `dotnet ef database update` or `mvn flyway:migrate` — no database migrations in this project

## Lab Context

Workshop participants work on a personal branch (`workshop/your-name`). Each lab builds on the previous. The `LegacyTaskProcessor` in both stacks is intentionally bad code — it exists as a refactoring exercise target.

If asked to implement a feature, check `docs/labs/` for the relevant lab guide before proceeding. Lab guides contain the specific prompts and expected outputs for each exercise.
