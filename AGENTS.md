# Repository Guidelines

## Source of truth
- `specs/` is the source of truth for product behavior.
- Read these first before implementation:
    - `specs/readme.md`
    - `specs/01_workflow/code_style.md`
    - `specs/01_workflow/codex_delivery_workflow.md`
    - `specs/00_overview/implementation_milestone_plan.md`
- If specs conflict, surface the conflict instead of guessing.

## Working mode
- Implement only one small milestone at a time.
- Keep changes complete for that milestone.
- Stop after finishing the milestone.
- Do not continue into the next milestone automatically.
- Stage all project-related created/changed files with `git add`.
- Do not create commits; the user commits manually.
- Add a milestone note in `specs/milestone/` after completion.

## Unity rules
- This repo uses Unity `6000.3.10f1`; confirm the current editor version in `ProjectSettings/ProjectVersion.txt` before editor-specific changes.
- Prefer checking `ProjectSettings/ProjectVersion.txt` before changing package wiring, asmdefs, scenes, serialized assets, or other editor-version-sensitive files.
- Do not rely on deprecated built-in Unity fonts/resources such as `Arial.ttf` for runtime UI creation.
- Prefer project-owned font assets for runtime UI; temporary placeholder UI may use Unity-6-compatible fallback resources only when necessary.
- Keep `MonoBehaviour` scripts thin.
- Put gameplay logic in small testable C# classes where possible.
- When changing scenes, prefabs, ScriptableObjects, or assets, include related `.meta` files.
- Use Edit Mode tests for domain logic unless Play Mode is genuinely needed.

## Asset rule
- If a milestone depends on missing art, audio, animation, or other assets, stop and ask clearly for the missing asset.
- Do not treat asset-dependent work as complete without required assets.

## Testing
- Every milestone must add or update tests for introduced behavior.
- Use clear behavior-style test names such as `ShouldUnlockNextNodeWhenProgressReachesThreshold`.
- Run batch Edit Mode tests with `tools/run_editmode_tests.ps1`.
- Do not pass `-quit` to Unity when using `-runTests`; let the Unity Test Framework exit after writing results.

## Repository hygiene
- Keep changes scoped to the current milestone only.
- Avoid unrelated refactors.
- Do not stage local/editor-generated files such as `.idea/` unless explicitly requested.
- Add local-only files that should not be versioned to `.gitignore`.

## Implementation snapshot upkeep
- After completing each milestone, update `specs/00_overview/current_build_state.md` in addition to creating the milestone note in `specs/milestone/`.
- Treat `specs/00_overview/current_build_state.md` as the compact snapshot of the build’s current behavior.
- Treat `specs/milestone/*.md` as detailed historical notes for each milestone.
- When a milestone changes current behavior, reflect that behavior in `current_build_state.md`.
- When a milestone only adds historical detail but does not change current behavior, keep the `current_build_state.md` edit minimal.
- Keep `current_build_state.md` concise:
    - summarize only currently true behavior
    - list important temporary placeholder decisions
    - list major not-yet-implemented areas that matter for upcoming milestones
- Do not merge milestone history into one large file.
- Do not remove milestone notes after updating `current_build_state.md`.
- If milestone notes and current code appear inconsistent, surface the inconsistency instead of guessing.