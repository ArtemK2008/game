# Repository Guidelines

## Source of truth
- `specs/` is the source of truth for product behavior.
- Read these first before implementation:
    - `specs/readme.md`
    - `specs/01_workflow/code_style.md`
    - `specs/01_workflow/codex_delivery_workflow.md`
    - `specs/00_overview/implementation_milestone_plan.md`
    - `specs/00_overview/current_build_state.md`
- Then read only the directly relevant spec files for the requested milestone.
- Use `specs/00_overview/current_build_state.md` as the primary compact reference for what is already implemented.
- Use `specs/milestone/*.md` as historical detail when recent implementation context is needed.
- If specs conflict, surface the conflict instead of guessing.
- If specs, milestone notes, and current code appear inconsistent, surface the inconsistency instead of guessing.

## Working mode
- Implement only one small milestone at a time.
- Keep changes complete for that milestone.
- Stop after finishing the milestone.
- Do not continue into the next milestone automatically.
- Implement exactly the requested milestone and nothing beyond it.
- If the requested milestone depends on missing prerequisite infrastructure or missing assets, stop and say exactly what is missing.
- Stage all project-related created/changed files with `git add`.
- Do not create commits; the user commits manually.
- Add a milestone note in `specs/milestone/` after completion.

## Code quality and refactoring rules
- Follow `specs/01_workflow/code_style.md` strictly.
- Respect Single Responsibility Principle in practice, not only in naming.
- Every class should have one clear reason to change.
- When adding a feature to an existing class, check whether that class still has one clear responsibility.
- If a touched class has grown beyond a clear single responsibility, perform a small local refactor in the same milestone to restore clarity.
- Keep such refactors scoped to the touched area and behavior-preserving unless the milestone explicitly changes behavior.
- Prefer extracting small focused classes/services over adding more branches and responsibilities to an already mixed-responsibility class.
- Do not use milestone work as an excuse for broad unrelated cleanup, but do not leave obvious SRP violations in touched code unaddressed.

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
- Preserve the project’s current strong testing standard; do not reduce useful coverage without a clear reason.
- When changing behavior, add or update tests so the changed behavior remains explicitly covered.
- When refactoring touched classes for SRP/clarity, keep or improve test coverage around that area.
- Do not remove useful tests just to make a milestone easier to deliver.
- Use clear behavior-style test names such as `ShouldUnlockNextNodeWhenProgressReachesThreshold`.
- Run batch Edit Mode tests with `tools/run_editmode_tests.ps1`.
- For coverage checks, use `tools/run_editmode_coverage.ps1`.
- Coverage guidance is documented in `tools/code_coverage.md`.
- Use coverage as a validation tool for touched milestone-critical logic, especially when refactoring or splitting responsibilities in existing code.
- Do not chase coverage numbers by writing shallow tests; prefer meaningful behavior coverage for the touched area.
- Do not pass `-quit` to Unity when using `-runTests`; let the Unity Test Framework exit after writing results.

## Repository hygiene
- Keep changes scoped to the current milestone only.
- Avoid unrelated refactors outside the touched milestone area.
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