# Milestone 049a - Deduplicate Skill Package Ids

## Decision
- Milestone 049 introduced a valid character-side package-assignment seam, but the three shipped package ids were still duplicated as raw strings across both the combat-facing package catalog and the character-facing package catalog.
- The smallest correct cleanup was to add one tiny shared constant holder for the currently shipped playable-character skill package ids and update the directly related runtime/tests to use it.

## Delivered
- Added `PlayableCharacterSkillPackageIds` under `Assets/Scripts/Core/` as the shared source of truth for:
  - `skill_package_vanguard_default`
  - `skill_package_vanguard_burst_drill`
  - `skill_package_striker_default`
- Updated the directly related runtime code to use those constants:
  - `CombatSkillPackageCatalog`
  - `PlayableCharacterCatalog`
  - `PlayableCharacterSkillPackageCatalog`
- Updated directly related EditMode tests that compare or construct those ids so they now reference the shared constants instead of repeating raw literals.

## Behavior Change
- No gameplay behavior changed.
- No UI behavior changed.
- No package-option behavior changed.
- `Vanguard` and `Striker` package behavior remains exactly the same as in Milestone 049.

## SRP Notes
- The new shared id source is intentionally narrow and only centralizes stable shipped package identifiers.
- It does not own package behavior, assignment rules, or combat resolution logic.
- This keeps identifier ownership separate from both combat-side package interpretation and character-side package-assignment policy.

## Tests
- Added `PlayableCharacterSkillPackageIdsTests` to lock the current shipped package ids to stable expected values.
- Updated directly related runtime-behavior tests to use the shared constants while preserving their existing behavior coverage.

## Out Of Scope
- No new package system abstraction
- No value-object hierarchy for package ids
- No gameplay/UI follow-up beyond the string deduplication cleanup
- No Milestone 050 work

## Verification
- Run batch Edit Mode tests with `tools/run_editmode_tests.ps1`.
- If that shell returns before Unity finishes, use the direct waited Unity batch invocation and report that explicitly.
