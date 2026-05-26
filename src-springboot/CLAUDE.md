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
web/            ← @RestController endpoints, request/response DTOs
```

Dependencies flow **inward only**: web → application → domain. Infrastructure implements domain interfaces.

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

- `src/main/java/.../domain/Task.java` — core aggregate
- `src/main/java/.../application/tasks/commands/` — command handlers
- `src/main/java/.../application/tasks/queries/` — query handlers
- `src/main/java/.../infrastructure/repositories/` — in-memory implementations
- `src/main/java/.../web/TaskController.java` — REST endpoints
- `src/main/java/.../infrastructure/legacy/LegacyTaskProcessor.java` — intentionally messy, used for Lab 4

## What NOT to Do

- Do not use field injection (`@Autowired` on fields) — use constructor injection
- Do not use `Optional.get()` without `isPresent()` check — use `orElseThrow()`
- Do not add `@Transactional` unless the lab explicitly calls for it
- Do not use raw types or unchecked casts
