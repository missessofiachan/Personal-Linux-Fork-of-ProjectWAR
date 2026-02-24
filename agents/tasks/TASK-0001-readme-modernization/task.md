# Task Brief

- Task Number: `0001`
- Task ID: `TASK-0001-readme-modernization`
- Title: Modernize project README for complete beginners
- Requestor: Project Director
- Base Branch: `Restart`
- Created At (UTC): `2026-02-24`

## Claude Tasking Path

- Start at `agents/TASK-INDEX.md`
- Open this file: `agents/tasks/TASK-0001-readme-modernization/task.md`

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

- [x] Old README is archived at `docs/archive/README-legacy-2026-02-24.md`.
- [x] New README explains emulator purpose for beginners.
- [x] New README includes step-by-step setup and run flow.
- [x] New README includes troubleshooting section.
- [x] Instructions reflect current repo scripts and config locations.

## Branch Map

- `agent/TASK-0001-readme-modernization/pm`
- `agent/TASK-0001-readme-modernization/qa`
- `agent/TASK-0001-readme-modernization/release`
- `agent/TASK-0001-readme-modernization/khorne` (purged after merge)
- `agent/TASK-0001-readme-modernization/tzeentch` (purged after merge)
- `agent/TASK-0001-readme-modernization/slaanesh` (purged after merge)
- `agent/TASK-0001-readme-modernization/nurgle` (purged after merge)
- `agent/TASK-0001-readme-modernization/claude` (purged after merge)
- `agent/TASK-0001-readme-modernization/consolidator`

## Delivery Requirements (Each Engineer)

- Summary of proposed README changes
- Why this documentation strategy is best
- Validation performed (links checked, command sanity checks, readability checks)
- Merge complications anticipated
- Expected impact to base branch
- Argument for selection

## Wait/Waive Rules

- Consolidation waits for all five submissions by default.
- Claude submission can be waived only by explicit Project Director instruction; record decision in `waiver.md`.
- Codex cannot self-authorize waiver.

## Pre-Merge Gates

- QA gate must be `PASS` in `admin/qa-review.md`.
- Release signoff must be `APPROVED` in `admin/release-signoff.md`.

## Task Status

- `Completed via consolidation branch and merged to Restart`

