# Spring Boot TaskManager — Claude Code Context

## Stack

- **Java 21** with records and sealed interfaces
- **Spring Boot 3.x** with Spring Web (REST)
- **JUnit 5** + **Mockito** for testing
- **Maven** for build and dependency management
- **Clean Architecture** with four packages

## Architecture Layers

```
domain/         ← Entities, value objects, domain interfaces (no Spring dependencies)
application/    ← Use cases, command/query handlers, application services
infrastructure/ ← Repository implementations, external service adapters
api/            ← @RestController endpoints, request/response DTOs (the web layer)
```

Dependencies flow **inward only**: api → application → domain. Infrastructure implements domain interfaces.

Each layer is a Maven module: `taskmanager-domain`, `taskmanager-application`, `taskmanager-infrastructure`, `taskmanager-api`.

## Java Conventions

- Java records for value objects and DTOs
- Sealed interfaces for domain concepts with a closed set of variants
- Guard clauses (no `else` branches): throw or return early
- `CompletableFuture` or synchronous — no mixing
- SLF4J (`LoggerFactory.getLogger(...)`) for structured logging, never `System.out`
- No abbreviations in identifiers
- Constructor injection — no field injection with `@Autowired`

## Testing Conventions

- JUnit 5 with `@Test`, `@ParameterizedTest`
- Mockito with `@ExtendWith(MockitoExtension.class)`
- Arrange / Act / Assert sections with blank line separators
- Test method naming: `methodName_stateUnderTest_expectedBehavior`
- Never test implementation details — test observable behavior
- Integration tests use `@SpringBootTest` with `MockMvc`

## Common Commands

```bash
mvn clean test                          # Build and run all tests
mvn test -Dtest="*UnitTest"             # Run unit tests only
mvn spring-boot:run                     # Run the application
mvn clean package                       # Build JAR
```

## Key Files

- `taskmanager-domain/.../domain/tasks/Task.java` — core aggregate (`TaskId`, `TaskStatus`, and the `TaskRepository` port alongside)
- `taskmanager-application/.../application/services/TaskService.java` — application service (commands/handlers are added during labs)
- `taskmanager-infrastructure/.../infrastructure/persistence/` — JPA entities, mappers, and the `JpaTaskRepositoryAdapter`
- `taskmanager-api/.../api/controllers/TaskController.java` — REST endpoints (`GlobalExceptionHandler` in `api/exception/`)
- `taskmanager-infrastructure/.../infrastructure/legacy/LegacyTaskProcessor.java` — intentionally messy, used for Lab 5

## What NOT to Do

- Do not use field injection (`@Autowired` on fields) — use constructor injection
- Do not use `Optional.get()` without `isPresent()` check — use `orElseThrow()`
- Do not add `@Transactional` unless the lab explicitly calls for it
- Do not use raw types or unchecked casts
