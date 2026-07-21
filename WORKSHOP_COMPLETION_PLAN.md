# Workshop Completion Plan

**Created**: 2026-06-10  
**Status**: In progress  
**Goal**: Ship a complete, link-consistent, facilitator-ready workshop

---

## Current State Summary

### What's Complete
- All 10 lab guides (`docs/labs/lab-01` through `lab-10`)
- Part 1 presentation modules 00–05 (`docs/presentations/modules/part1/`)
- Part 2 presentation modules 00–04 (`docs/presentations/modules/part2/`)
- `docs/labs/README.md`
- `docs/LOCAL_SETUP.md`
- `docs/PRE_WORKSHOP_CHECKLIST.md`
- Root `CLAUDE.md`, `src/CLAUDE.md`, `src-springboot/CLAUDE.md`
- `.claude/settings.json`
- Root `README.md`

### What's Missing (Broken Links / Empty Dirs)
| Item | Referenced From | Priority |
|---|---|---|
| `docs/presentations/modules/part1/06-wrapup-part1.md` | index.md, module 05 nav footer | High |
| `docs/guides/agent-governance.md` | lab-10 closing section | High |
| `docs/presentations/modules/roles/` | (empty dir, unknown intent) | Medium |
| `docs/guides/` | (empty dir, only agent-governance.md needed?) | High |
| `docs/notes/`, `docs/adr/`, `docs/design/` | (empty dirs, likely placeholders) | Low |

### What Needs Updating
| Item | Issue | Priority |
|---|---|---|
| `docs/presentations/README.md` | Part 2 section still says "coming soon" with old filenames | High |
| `docs/presentations/index.md` | Part 2 total duration not updated | Low |

---

## Work Items

### P0 — Broken Links (must fix before delivering)

- [x] **1. Create `docs/presentations/modules/part1/06-wrapup-part1.md`**  
  A Part 1 wrap-up slide deck (Marp format). Should:
  - Recap the 5 labs and what was learned
  - Preview Part 2 topics
  - Include a break / logistics slide
  - Nav footer linking back to module 05 and forward to Part 2 module 00
  - ~8–10 slides

- [x] **2. Create `docs/guides/agent-governance.md`**  
  Referenced at the bottom of lab-10 as: *"Read the Agent Governance Guide for how to manage Claude Code configuration at enterprise scale."*  
  Should cover:
  - Who owns configuration files (CLAUDE.md, settings.json, .mcp.json)
  - PR process for proposing config changes
  - How user-level vs. project-level settings interact at scale
  - Handling secrets and tokens across a team
  - Onboarding new developers to an existing Claude Code configuration
  - Version management and rollback for config
  - ~4–6 sections, practical/reference format (not a lab, not slides)

### P1 — Stale Content (should fix before delivering)

- [x] **3. Update `docs/presentations/README.md`**  
  Part 2 table updated to actual filenames (00–04), "coming soon" note removed, Part 1 wrap-up row added.

- [x] **4. Clarify or populate `docs/presentations/modules/roles/`**  
  Directory doesn't exist — plan was stale on this point. Nothing to do.

### P2 — Cleanup / Polish (nice to have)

- [x] **5. Update `docs/presentations/index.md` Part 2 total duration**  
  Verified: 25+20+20+15+15 = 95 min of slides; "~3 hours including lab time" is consistent with Part 1's framing. Left as is.

- [x] **6. Decide on `docs/notes/`, `docs/adr/`, `docs/design/`**  
  None of these directories exist — plan was stale. Nothing to do.

- [ ] **7. Commit and push the 6 unpushed commits**  
  Current branch is 6 commits ahead of `origin/main`.  
  Also: `.devcontainer/springboot-participant/devcontainer-lock.json` is untracked — decide whether to commit or `.gitignore` it.

---

## Recommended Execution Order

1. Part 1 wrap-up slide deck (`06-wrapup-part1.md`) — fixes the broken nav chain
2. Agent governance guide — fixes the broken lab-10 link
3. Update `presentations/README.md` — removes stale "coming soon"
4. Decision on `roles/` directory — delete or stub
5. Clean up empty directories (`notes/`, `adr/`, `design/`)
6. Final link audit across all markdown files
7. Commit and push everything

---

## Open Questions

- **`roles/` directory**: Was role-specific content (developer/architect/manager variants) intended? If so, what form? If not, delete.
- **`docs/notes/`, `docs/adr/`, `docs/design/`**: Intentional placeholder scaffolding, or accidental leftovers? Safe to delete?
- **Devcontainer lock file**: Commit `.devcontainer/springboot-participant/devcontainer-lock.json` or add to `.gitignore`?
