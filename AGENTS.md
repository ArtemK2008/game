# Repository Guidelines

## Source of truth
- `specs/` is the source of truth for product behavior.
- Read these first before implementation:
    - `specs/README.md`
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

## Repository hygiene
- Keep changes scoped to the current milestone only.
- Avoid unrelated refactors.
- Do not stage local/editor-generated files such as `.idea/` unless explicitly requested.
- Add local-only files that should not be versioned to `.gitignore`.
