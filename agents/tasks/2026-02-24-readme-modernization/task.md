# Task Brief

- Task ID: `2026-02-24-readme-modernization`
- Title: Modernize project README for complete beginners
- Requestor: Project Director
- Base Branch: `Restart`
- Created At (UTC): `2026-02-24`

## Objective

Rewrite the root `README.md` so a complete computer novice can understand what the emulator is and how to run it locally.

## Scope

- In scope:
  - Explain what the emulator is in plain language.
  - Provide beginner-safe setup and run instructions.
  - Include prerequisites, database import, zone data setup, build/run steps, and basic troubleshooting.
  - Archive the previous README to `docs/archive/README-legacy-2026-02-24.md`.
- Out of scope:
  - Code changes to runtime behavior.
  - New emulator features.
  - Deployment/production hardening.

## Constraints

- Use clear, plain English intended for non-technical users.
- Keep instructions accurate to repository scripts/configs.
- Preserve historical content by archiving old README before replacement.

## Acceptance Criteria

- [ ] Old README is archived at `docs/archive/README-legacy-2026-02-24.md`.
- [ ] New README explains emulator purpose for beginners.
- [ ] New README includes step-by-step setup and run flow.
- [ ] New README includes troubleshooting section.
- [ ] Instructions reflect current repo scripts and config locations.

## Branch Map

- `agent/2026-02-24-readme-modernization/khorne`
- `agent/2026-02-24-readme-modernization/tzeentch`
- `agent/2026-02-24-readme-modernization/slaanesh`
- `agent/2026-02-24-readme-modernization/nurgle`
- `agent/2026-02-24-readme-modernization/claude`
- `agent/2026-02-24-readme-modernization/consolidator`

## Delivery Requirements (Each Engineer)

- Summary of proposed README changes
- Why this documentation strategy is best
- Validation performed (links checked, command sanity checks, readability checks)
- Merge complications anticipated
- Expected impact to base branch
- Argument for selection

## Wait/Waive Rules

- Consolidation waits for all five submissions by default.
- Claude submission can be waived only by Project Director; record decision in `waiver.md`.
