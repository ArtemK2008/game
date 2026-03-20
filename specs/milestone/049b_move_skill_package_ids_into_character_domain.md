# Milestone 049b - Move Skill Package Ids Into Character Domain

## Decision
- Milestone 049a correctly deduplicated the shipped playable-character skill package ids, but the new shared holder lived under the generic `Core` domain.
- That location was broader than needed because the identifiers belong specifically to the playable-character skill-package / build-assignment seam.
- The smallest correct cleanup was to move the id holder into `Assets/Scripts/Data/Characters/` and update the directly related runtime/test references.

## Delivered
- Moved `PlayableCharacterSkillPackageIds` from:
  - `Assets/Scripts/Core/PlayableCharacterSkillPackageIds.cs`
  to:
  - `Assets/Scripts/Data/Characters/PlayableCharacterSkillPackageIds.cs`
- Updated its runtime namespace from `Survivalon.Core` to `Survivalon.Data.Characters`.
- Updated the directly related runtime/test references and the small guard test to use the new location.
- Kept the class name and constant values unchanged.

## Behavior
- No gameplay behavior changed.
- No UI behavior changed.
- No package-option behavior changed.
- The current `Vanguard` / `Striker` package behavior remains exactly the same.

## SRP Notes
- `PlayableCharacterSkillPackageIds` still owns only stable shipped package identifiers.
- Combat behavior remains in combat-side classes.
- Package-assignment and availability behavior remains in character-side classes.
- The move improves ownership clarity by placing the identifier source next to the character/build domain it supports.

## Tests
- Preserved the existing guard test coverage for the shared package-id source.
- Updated only the directly related runtime/test references needed for the new namespace/path.

## Intentionally Left Out
- No new package abstraction
- No redesign of the assignment or combat package systems
- No Milestone 050 work

## Verification
- Run batch Edit Mode tests with `tools/run_editmode_tests.ps1`.
- If that shell returns before Unity finishes, use the direct waited Unity batch invocation and report that explicitly.
