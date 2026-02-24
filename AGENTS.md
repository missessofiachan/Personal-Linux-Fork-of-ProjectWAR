# ProjectWAR Agent Team Charter

This repository uses a multi-agent engineering workflow.

## Team Structure

- Project Director (Human): final authority, can waive Claude input, escalation target.
- Consolidation Engineer (Codex): reviews all agent outputs, integrates the chosen path, and prepares the final merge recommendation.
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
3. Run all five engineers in parallel.
4. Collect all submissions.
5. Wait gate: consolidation is blocked until all submissions are present.
6. Claude waiver: if Project Director records a waiver in `waiver.md`, consolidation can proceed without Claude's submission.
7. Consolidation Engineer compares outputs, implements the integrated path on `agent/<task_id>/consolidator`, and documents the decision.
8. If disagreement is substantial, escalate to Project Director for a direction call.
9. Merge recommendation is issued only after validation.

## Gate Rule

No code is merged into the base branch until the wait gate passes, or a Claude waiver is explicitly recorded.

## Repo Locations

- Team workflow quickstart: `agents/README.md`
- Task index: `agents/TASK-INDEX.md`
- Templates: `agents/templates/`
- Branch helper: `agents/scripts/create-agent-branches.ps1`
