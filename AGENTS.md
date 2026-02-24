# ProjectWAR Agent Team Charter

This repository uses a multi-agent engineering workflow.

## Team Structure

- Project Director (Human): final authority, can waive Claude input, escalation target.
- Project Manager Agent (Codex Admin): defines scope, acceptance criteria, and delivery checklist per task.
- QA Agent (Codex Admin): independently reviews consolidator output and issues pass/fail gate decision.
- Release Manager Agent (Codex Admin): verifies all required gates/records and authorizes merge readiness.
- Consolidation Engineer (Codex): reviews all coder outputs, integrates the chosen path, and prepares the merge candidate.
- Parallel Engineering Agents:
  - Khorne (Codex)
  - Tzeentch (Codex)
  - Slaanesh (Codex)
  - Nurgle (Codex)
  - Claude (Claude AI, external contributor)

## Operating Model

- The five engineering agents are the primary implementation workforce.
- Khorne, Tzeentch, Slaanesh, and Nurgle are intentionally not fixed to permanent specialties.
- Each task is assigned to all five engineers in parallel to encourage diverse solutions.
- Claude is managed like an independent employee: tasks can be assigned, but execution is autonomous.

## Branch Model

Default base branch: `Restart` (unless the task file says otherwise).

Per task `<task_id>`, create:

- `agent/<task_id>/pm`
- `agent/<task_id>/qa`
- `agent/<task_id>/release`
- `agent/<task_id>/khorne`
- `agent/<task_id>/tzeentch`
- `agent/<task_id>/slaanesh`
- `agent/<task_id>/nurgle`
- `agent/<task_id>/claude`
- `agent/<task_id>/consolidator`

## Task Numbering

- Every task must use a numbered ID format: `TASK-0001-short-name`.
- Task directories must match the ID: `agents/tasks/<task_id>/`.
- The canonical task list is tracked in `agents/TASK-INDEX.md`.
- Claude tasking entry point is always:
  - `agents/TASK-INDEX.md` (find task)
  - `agents/tasks/<task_id>/task.md` (full brief)

## Required Task Records

For each task folder `agents/tasks/<task_id>/`:

- `task.md` (single source of truth for scope and acceptance criteria)
- `submissions/<agent>.md` for each engineer
- `consolidation.md` (integration decision and merge plan)
- `waiver.md` (Claude waiver record when applicable)
- `admin/pm-brief.md` (PM scope/acceptance record)
- `admin/qa-review.md` (mandatory QA gate result for consolidator output)
- `admin/release-signoff.md` (release readiness record)

## Required Submission Content

Every engineer submission must include:

- Summary of changes
- Why this approach was chosen
- Tests run and results
- Potential merge complications
- Expected impact on the base branch
- Argument for why this path is best

## Execution Workflow

1. Create task file from template.
2. Create per-agent branches.
3. PM Agent writes `admin/pm-brief.md`.
4. Run all five engineers in parallel.
5. Collect all submissions.
6. Wait gate: consolidation is blocked until all submissions are present.
7. Claude waiver: consolidation can proceed without Claude only when Project Director gives explicit waiver instruction and it is recorded in `waiver.md`.
8. Consolidation Engineer compares outputs, implements selected path on `agent/<task_id>/consolidator`, and updates `consolidation.md`.
9. QA Agent reviews consolidator output and writes `admin/qa-review.md` with explicit gate status.
10. Release Manager verifies gates and writes `admin/release-signoff.md`.
11. If disagreement is substantial, escalate to Project Director for a direction call.
12. Run pre-merge gate check: `.\agents\scripts\check-premerge-gates.ps1 -TaskId "<task_id>"`.
13. Merge to base branch is allowed only after all pre-merge gates pass.

## Pre-Merge Gates

No code is merged into the base branch until all conditions are true:

- Wait gate passed (or explicit Claude waiver recorded by Project Director).
- QA gate is `PASS` in `admin/qa-review.md`.
- Release signoff is `APPROVED` in `admin/release-signoff.md`.

## Branch Lifecycle

- After `agent/<task_id>/consolidator` is merged into the base branch, purge worker branches:
  - `agent/<task_id>/khorne`
  - `agent/<task_id>/tzeentch`
  - `agent/<task_id>/slaanesh`
  - `agent/<task_id>/nurgle`
  - `agent/<task_id>/claude`
- Keep or delete the consolidator branch based on Project Director instruction.
- Preferred cleanup command:
  - `.\agents\scripts\purge-worker-branches.ps1 -TaskId "<task_id>" -BaseBranch "Restart" -DeleteRemote`

## Waiver Authority

- Only Project Director can authorize waiver.
- Codex cannot self-authorize, infer, or auto-apply waiver.
- If Claude submission is missing and no explicit waiver instruction exists, Codex must pause and request Project Director decision.
- `waiver.md` must include the explicit authorization source (user instruction text/date).

## Repo Locations

- Team workflow quickstart: `agents/README.md`
- Organization chart and role definitions: `agents/ORG.md`
- Task index: `agents/TASK-INDEX.md`
- Templates: `agents/templates/`
- Branch helper: `agents/scripts/create-agent-branches.ps1`
- Branch purge helper: `agents/scripts/purge-worker-branches.ps1`
- Pre-merge gate checker: `agents/scripts/check-premerge-gates.ps1`
