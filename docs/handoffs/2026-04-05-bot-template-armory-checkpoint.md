# Bot Template Armory Checkpoint

Date: 2026-04-05

## Summary

This checkpoint captures the ProjectWAR-side status of the bot editor API after the armory redesign and the template-aware route cleanup.

## Current State

- `WorldServer` exposes both bot-sheet and career-template bot editor routes.
- Base-template patching and per-bot override patching are both supported.
- Runtime toolkit usage has moved beyond inspection into active template and override editing.
- The standalone toolkit viewer now depends on the API's template-aware route surface instead of only the older per-bot override workflow.

## Key API Surfaces

- `PATCH /api/bot-editor/bots/{characterId}/template`
- `GET /api/bot-editor/career-templates`
- `GET /api/bot-editor/career-templates/{careerLine}/{tier}/{variantIndex}/items`
- `PATCH /api/bot-editor/career-templates/{careerLine}/{tier}/{variantIndex}`
- existing `PUT/DELETE /api/bot-editor/bots/{characterId}/gear` routes remain the per-bot override path

## Remaining Gaps

- tactic-slot editing is not exposed
- mastery-template editing is not exposed
- the API is still gear-focused; tactic/mastery state needs explicit response and mutation models before the toolkit can surface it cleanly

## Cleanup Included In This Checkpoint

- outdated docs that still described the six-route, per-bot-only editor milestone were updated
- generated build/temp artifacts under the repo root are purged as part of the checkpoint cleanup
