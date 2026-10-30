---
paths:
  - "src-springboot/**/*.java"
  - "src-springboot/**/pom.xml"
  - "src-springboot/**/*.{yml,yaml,properties}"
---

# Spring Boot Rules (Java 21 / Clean Architecture / DDD)

These rules complement `src-springboot/CLAUDE.md`, which holds the module layout, build commands, and testing conventions. They load only when Claude reads or edits files under `src-springboot/`.

## Before Writing Code

- Write or update the failing test first (TDD). Run `mvn clean test` from `src-springboot/`.
- Check the module: `taskmanager-domain` → `taskmanager-application` → `taskmanager-api`; `taskmanager-infrastructure` implements domain interfaces. Dependencies point inward only.
- Do not add Maven dependencies without asking.

## Domain Module

- Pure Java: no Spring, JPA, Jackson, or validation annotations. No `@Entity`, `@Service`, or `@Component`.
- Aggregates are `final`, created through static factories (`Task.create(...)`), rebuilt from storage with `reconstitute(...)`, and changed only through business methods (`start()`, `complete()`, `cancel()`).
- Value objects and IDs are `record`s that validate in the compact constructor or factory (`TaskId.of(...)`).
- Throw `IllegalArgumentException` for invalid input and `IllegalStateException` for invalid transitions.
- Repository interfaces (`TaskRepository`) live in the domain; implementations live in infrastructure.

## Application, Infrastructure, API

- Constructor injection only, with `final` fields. No field `@Autowired`.
- Application services orchestrate; they do not hold business rules and do not use Spring Data interfaces directly.
- JPA entities (`TaskEntity`) and mappers stay in infrastructure. Never return them from the API.
- Controllers are thin: request `record` DTO with Bean Validation → application service → response DTO. Map exceptions with `@RestControllerAdvice` to Problem Details.
- Return `Optional` from lookups; never call `Optional.get()` without a check. No `null` returns from public methods.

## Java Style

- Java 21: records, `var` when the type is obvious, switch expressions, and pattern matching for `instanceof`.
- SLF4J with placeholders (`log.info("Created task {}", id)`), never `System.out` or string concatenation in log calls.
- Guard clauses over nested `if`/`else`; small methods; one level of abstraction per method.

## Verification

Run `mvn clean test` (Testcontainers needs Docker for integration tests) before declaring a change done. Commit with a Conventional Commit message.
