# Lab Validation Reports

Results of running the labs end to end with Claude Code, to catch steps that don't behave as written. Each report lists what was verified (✅), what was wrong (⚠️/❌), and what still needs a live interactive session (🖐).

| Report | Track | Labs | Fixes |
|---|---|---|---|
| [day1-dotnet.md](day1-dotnet.md) | .NET | 1–7 | #52 |
| [day2-dotnet.md](day2-dotnet.md) | .NET | 8–15 | #53 |
| [spring.md](spring.md) | Spring Boot | 1–11 (Spring-specific steps) | this PR |

**Method:** code and config steps were executed for real on a throwaway branch; claims about Claude's behavior were tested in fresh headless `claude -p` sessions. Steps that need the interactive UI, a personal token, or changes to user-level config were left for the facilitator dry run (#37).

**Not yet covered:** the 🖐 items in all three reports (interactive UI, personal tokens, user-level config).
