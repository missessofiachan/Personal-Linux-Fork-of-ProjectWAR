# Agent Organization

## Leadership

- Project Director (Human)
  - Owns final direction.
  - Only authority that can waive Claude participation.
  - Final escalation point for major disagreements.

## Admin Agents (Process Owners)

- PM Agent (Codex Admin)
  - Writes scope and acceptance criteria.
  - Produces `admin/pm-brief.md`.

- QA Agent (Codex Admin)
  - Reviews consolidator-selected implementation before merge.
  - Produces `admin/qa-review.md`.
  - Must set `Gate Status: PASS` for merge eligibility.

- Release Agent (Codex Admin)
  - Verifies all gates and task records are complete.
  - Produces `admin/release-signoff.md`.
  - Must set `Status: APPROVED` for merge eligibility.

## Engineering Agents (Solution Producers)

- Khorne (Codex)
- Tzeentch (Codex)
- Slaanesh (Codex)
- Nurgle (Codex)
- Claude (Claude AI)

Each engineering agent proposes an independent implementation path.

## Integration

- Consolidation Engineer (Codex)
  - Compares engineering submissions.
  - Builds selected/hybrid solution in `agent/<task_id>/consolidator`.
  - Documents decisions in `consolidation.md`.

## Merge Eligibility Rules

Merge to `Restart` is blocked unless all conditions are true:

- Wait gate passed (or explicit Project Director waiver for Claude recorded).
- QA gate is `PASS`.
- Release signoff is `APPROVED`.
