# Docs Milestone 01 - Milestone Navigation And Workflow Cleanup

## Goal
- Add small documentation aids for milestone navigation, milestone-note writing, and Unity verification workflow clarity without changing gameplay/runtime behavior or consuming the next numbered gameplay milestone.

## Delivered
- Added `specs/milestone/status_index.md` as a compact navigation index for completed numbered milestones, follow-ups, refactors, and the current next numbered target.
- Added `specs/01_workflow/unity_verification_notes.md` to document the compile-first, EditMode-second, and direct waited batch fallback verification sequence already used in the repo.
- Added `specs/01_workflow/milestone_note_template.md` to give one compact default milestone-note template plus short guidance for follow-up, refactor, and acceptance notes.
- Updated `specs/readme.md` with explicit documentation precedence, a non-invention rule for open questions, and pointers to the new navigation/template aids.
- Updated `AGENTS.md` with a minimal pointer to the new milestone index and verification note.
- Added one small source-note pointer in `specs/00_overview/current_build_state.md`.

## Behavior Change
- No gameplay, runtime, scene, asset, test, tooling, or Unity-project behavior changed.
- This milestone only improves repository navigation and documentation workflow clarity.

## Tests
- No automated tests were added or changed.
- No Unity compile/test run was required because no `.cs`, `.asmdef`, scene, runtime UI wiring, or test files were changed.

## Out Of Scope
- Any gameplay milestone work, including Milestone 074.
- Rewriting milestone history into `current_build_state.md`.
- Modifying `tools/` scripts or verification behavior.
- Rewriting `specs/01_workflow/codex_delivery_workflow.md`, even though its current content appears inconsistent with its filename and overlaps code-style material.

## Verification
- Verified the milestone by reviewing the added/updated documentation files and keeping the change set docs-only.
- Unity compile/import and EditMode verification were intentionally not run because the touched files were limited to repository documentation.
