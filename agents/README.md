# Agent Workflow Quickstart

This folder contains the operational files for the ProjectWAR multi-agent team.

## 1. Create or copy a task file

- Use `agents/templates/task-template.md` as the base.
- Save the task as `agents/tasks/<task_id>/task.md`.

## 2. Create agent branches

Run:

```powershell
.\agents\scripts\create-agent-branches.ps1 -TaskId "<task_id>" -BaseBranch "Restart"
```

## 3. Execute in parallel

- Assign the same task to:
  - Khorne
  - Tzeentch
  - Slaanesh
  - Nurgle
  - Claude

## 4. Collect required submissions

- Save each result in `agents/tasks/<task_id>/submissions/`.
- Use `agents/templates/submission-template.md`.

## 5. Apply wait gate

- Do not consolidate until all submissions are present.
- If Claude is waived by Project Director, record it in `agents/tasks/<task_id>/waiver.md`.

## 6. Consolidate

- Use `agents/templates/consolidation-template.md`.
- Consolidator branch: `agent/<task_id>/consolidator`.
- Document chosen path, rejected paths, risks, and merge strategy.
