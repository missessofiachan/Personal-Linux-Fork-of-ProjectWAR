# Task Brief

- Task Number: `0002`
- Task ID: `TASK-0002-workflow-guardrails`
- Title: `Enforce protected-branch agent workflow guardrails`
- Requestor: `Project Director`
- Base Branch: `Restart`
- Created At (UTC): `2026-02-24`

## Claude Tasking Path

- Start at `agents/TASK-INDEX.md`
- Open this file: `agents/tasks/TASK-0002-workflow-guardrails/task.md`

## Objective

Prevent future direct implementation pushes to protected branches by adding explicit workflow policy and a local pre-push enforcement guard.

## Scope

- In scope:
  - Add explicit no-direct-implementation guardrails to repository workflow docs.
  - Add a pre-push guard that blocks protected-branch code pushes lacking `TASK-####` or `DIRECTOR-OVERRIDE`.
  - Add a one-command hook setup script.
- Out of scope:
  - Gameplay feature implementation.
  - Retrofitting old commits.

## Acceptance Criteria

- [x] `AGENTS.md` explicitly states Codex cannot directly implement on protected branches without Director override.
- [x] `agents/README.md` includes hook setup instructions.
- [x] `.githooks/pre-push` and `agents/scripts/pre-push-guard.ps1` block non-compliant protected-branch pushes.
- [x] `agents/scripts/setup-git-hooks.ps1` configures `core.hooksPath` to `.githooks`.
