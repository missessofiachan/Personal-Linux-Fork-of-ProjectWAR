# Agent Workflow Quickstart

This folder contains the operational files for the ProjectWAR multi-agent team.

Role definitions are in `agents/ORG.md`.

## Task numbering

- Use numbered IDs: `TASK-0001-short-name`.
- Register every task in `agents/TASK-INDEX.md`.
- Claude should be directed to:
  - `agents/TASK-INDEX.md`
  - `agents/tasks/<task_id>/task.md`

## 1. Create or copy a task file

- Use `agents/templates/task-template.md` as the base.
- Save the task as `agents/tasks/<task_id>/task.md`.

## 2. Create agent branches

Run:

```powershell
.\agents\scripts\create-agent-branches.ps1 -TaskId "<task_id>" -BaseBranch "Restart"
```

## 3. PM briefing

- PM agent writes:
  - `agents/tasks/<task_id>/admin/pm-brief.md`

## 4. Execute coder agents in parallel

- Assign the same coding task to:
  - Khorne
  - Tzeentch
  - Slaanesh
  - Nurgle
  - Claude

## 5. Collect required coder submissions

- Save each result in `agents/tasks/<task_id>/submissions/`.
- Use `agents/templates/submission-template.md`.

## 6. Apply wait/waive gate

- Do not consolidate until all submissions are present.
- If Claude is waived by Project Director, record it in `agents/tasks/<task_id>/waiver.md`.
- Codex must not apply waiver without explicit Project Director instruction.
- If Claude is missing and no explicit waiver is given, pause and ask Project Director.

## 7. Consolidate

- Use `agents/templates/consolidation-template.md`.
- Consolidator branch: `agent/<task_id>/consolidator`.
- Document chosen path, rejected paths, risks, and merge strategy.

## 8. Mandatory QA gate before merge

- QA agent must review consolidator output and write:
  - `agents/tasks/<task_id>/admin/qa-review.md`
- QA gate must be explicitly `PASS` before merge to `Restart`.

## 9. Release signoff before merge

- Release agent writes:
  - `agents/tasks/<task_id>/admin/release-signoff.md`
- Release status must be `APPROVED` before merge.

## 10. Run pre-merge gate check

```powershell
.\agents\scripts\check-premerge-gates.ps1 -TaskId "<task_id>"
```

## 11. Purge Worker Branches

- After consolidator is merged into base, purge worker/coder branches.
- Run:

```powershell
.\agents\scripts\purge-worker-branches.ps1 -TaskId "<task_id>" -BaseBranch "Restart" -DeleteRemote
```
