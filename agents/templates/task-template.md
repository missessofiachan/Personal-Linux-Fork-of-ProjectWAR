# Task Brief

- Task Number: `0001`
- Task ID: `TASK-0001-short-name`
- Title:
- Requestor:
- Base Branch: `Restart`
- Created At (UTC):

## Claude Tasking Path

- Start at `agents/TASK-INDEX.md`
- Open this file: `agents/tasks/<task_id>/task.md`

## Objective

Describe the business or engineering goal in one paragraph.

## Scope

- In scope:
- Out of scope:

## Constraints

- Required tools:
- Forbidden changes:
- Performance/security requirements:

## Acceptance Criteria

- [ ] Criterion 1
- [ ] Criterion 2
- [ ] Criterion 3

## Branch Map

- `agent/<task_id>/pm`
- `agent/<task_id>/qa`
- `agent/<task_id>/release`
- `agent/<task_id>/khorne`
- `agent/<task_id>/tzeentch`
- `agent/<task_id>/slaanesh`
- `agent/<task_id>/nurgle`
- `agent/<task_id>/claude`
- `agent/<task_id>/consolidator`

## Required Admin Records

- `admin/pm-brief.md`
- `admin/qa-review.md`
- `admin/release-signoff.md`

## Delivery Requirements (Each Engineer)

- Implementation summary
- Rationale for approach
- Tests executed + results
- Merge complications
- Impact to base branch
- Argument for why approach should be selected

## Wait/Waive Rules

- Wait for all engineer submissions before consolidation.
- Claude can be waived only by explicit Project Director instruction; record in `waiver.md`.
- Codex must not self-authorize waiver.
- If no Claude submission and no explicit waiver, consolidation is blocked pending Project Director decision.

## Branch Cleanup Rule

- After consolidator branch is merged into base, purge worker branches (Khorne/Tzeentch/Slaanesh/Nurgle/Claude) locally and on origin.

## Pre-Merge Gates

- QA gate in `admin/qa-review.md` is `PASS`.
- Release signoff in `admin/release-signoff.md` is `APPROVED`.
- `.\agents\scripts\check-premerge-gates.ps1 -TaskId "<task_id>"` returns `PASS`.
