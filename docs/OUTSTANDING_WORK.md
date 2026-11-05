# Outstanding Work: Claude Code Workshop

_Status as of 2026-11-05 (updated after #55, with #56 and #57 open). Pick up from here in a new session._

## Where things stand
- **Merged to main:** #38 (full refresh: current Claude Code features, alignment with the Copilot workshop, dev container fix, xunit.v3), #47 (.NET solution moved into `src-dotnet/`), #48 (slide overflow fixes and "directing an agent" framing across 16 decks), #49 (Day 1 timing: Lab 8 moved to Day 2, Lab 14 self-study, decks renumbered), #50 (.NET integration tests enabled with `Microsoft.AspNetCore.Mvc.Testing`), #51 (Day 2 wrap-up deck), #52, #53 and #55 (lab validation fixes).
- **Open PRs:** #56 (repo hygiene files, #35) and #57 (Spring Boot 3.2.4 → 4.1.2, #46).
- **Issues:** audited and realigned to the 15-lab / Day 1–Day 2 layout. Milestones: Day 1 Ready, Day 2 Ready, Full Workshop Ready.
- **Slide changes (merged in #48):**
  - Kickoff slides 2–4 and 13 reworded around "Claude Code is an agentic harness" and "suggestion-based vs. directing an agent".
  - Overflow fixed on all 17 decks (255 slides checked by rendering). No content cut: 5 slides were split, which shifts slide numbers in the kickoff, 02, 07, 09 and capstone decks.

## Next steps (in order)
1. ~~**Finish the deck review**~~ ✅ done (`docs/presentations/modules/`).
   - [x] Reworded the Lab 1 takeaway in `part1/09-wrapup-day1.md` to "Direct an agent that reads, plans, and acts — not one suggestion at a time".
   - [x] Re-render after any edits to confirm there's no overflow (Marp CLI `--images png`).
2. ~~**Commit the slide fixes**~~ ✅ merged as #48 (merge commit). Marp CLI is allowed in `.claude/settings.local.json` for re-renders.
3. ~~**Day 1 timing** (#44)~~ ✅ merged as #49: Lab 3 stays whole on Day 1 (ends 16:45); Lab 8 opens Day 2; Lab 14 is self-study (Day 2 ends 16:35). Decks renumbered (`part1/08-wrapup-day1`, `part2/00-security-review` … `part2/07-capstone`).
4. ~~**Lab 5 .NET integration tests** (#45)~~ ✅ merged as #50: `Microsoft.AspNetCore.Mvc.Testing` 9.0.22 approved and added, plus a passing `/health` example test. Fresh checkout now reports 1 passed, 11 skipped, 0 failed.
5. ~~**Day 2 wrap-up deck** (#14)~~ ✅ merged as #51: `part2/08-wrapup-day2.md` (13 slides), linked from the capstone deck and indexes.
6. **Validate each lab end to end** in a live Claude Code session:
   - Day 1 .NET pass done 2026-11-03 (code steps + headless checks): report in `docs/validation/day1-dotnet.md`, fixes merged in #52. Still to do: interactive 🖐 items, Spring Boot track, Day 2. 
   - Day 2 .NET pass done 2026-11-03: report in `docs/validation/day2-dotnet.md` (one blocker: Lab 8 `/security-review` doesn't report the planted bug). Fixes, including the Lab 8 restructure, merged in #53. 
   - Spring Boot pass done 2026-11-03 in the `springboot-participant` dev container: report in `docs/validation/spring.md`, fixes merged in #55. Remaining: the interactive 🖐 items in all three reports (dry run, #37).
   - [ ] Day 1: #22 Lab 1, #23 Lab 2, #24 Lab 3, #39 Lab 4, #25 Lab 5, #40 Lab 6, #26 Lab 7
   - [ ] Day 2: #41 Lab 8, #27 Lab 9, #28 Lab 10 (also confirm GitHub MCP server tool names), #42 Lab 11 (also confirm the `/skills` `t` sort key), #29 Lab 12, #30 Lab 13, #43 Lab 14 (self-study), #31 Lab 15
7. **Full dry run** (#37): facilitator runs all 15 labs end to end in the dev containers (.NET and Spring Boot).
8. **Nice to have:**
   - #35 repo hygiene: files added in PR #56. Still to do: Centric legal confirms the license (`LICENSE.md` is a placeholder) and fill in the Code of Conduct reporting contact (`TODO`).
   - #46 Spring Boot upgrade: `src-springboot` moved to 4.1.2 in PR #57 (102 tests pass; verified in the `springboot-participant` dev container). Still to do: the same upgrade in the Copilot workshop (centricconsulting/ai-coding-workshop). Boot 4.0 OSS support ends 2026-12-31 and 4.1 on 2027-07-31 (from third-party sources; confirm on spring.io).

## Notes
- The dev containers (dotnet-participant, springboot-participant, maintainer) all build. The lock files were regenerated in #38 because of the GitHub CLI signing-key rotation.
- The Copilot workshop (centricconsulting/ai-coding-workshop) is the reference for directional alignment, not exact parity.
- Merge PRs with merge commits (no squash), and don't auto-approve PRs.
