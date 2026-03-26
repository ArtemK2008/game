# Milestone 052a - Cleanup World Map Build Summary And Gear Assignment Shape

## Purpose
- Tighten the small 052 pre-run gear assignment seam without changing any shipped behavior.
- Clean up stale naming in the world-map build summary path.
- Remove the hardcoded single-item option source from selected-character gear assignment.

## Delivered
- Renamed `WorldMapScreenTextBuilder.BuildSkillPackageAssignmentText(...)` to `BuildBuildAssignmentText(...)` so the method name matches its current responsibility.
- Updated directly related tests and call sites to use the new build-summary naming.
- Refactored `PlayableCharacterGearAssignmentService` so primary-gear options now come from:
  - current shipped `GearCatalog` entries
  - current owned gear ids in persistent state
  - the selected character's equipped state for `PrimaryCombat`
- Added focused test coverage that proves primary-gear equip state remains character-specific across selection changes.

## Behavior Change
- No gameplay behavior changed.
- The same one shipped `PrimaryCombat` item, `Training Blade`, remains the only current owned/equippable option.
- The world-map placeholder still shows the same build summary and the same equip or unequip control.
- Save/load/bootstrap/safe-resume behavior remains unchanged.

## SRP Notes
- `GearCatalog` still owns shipped gear definitions only.
- `PlayableCharacterGearAssignmentService` still owns selected-character gear option resolution and equip or clear rules.
- `WorldMapScreen` still only wires placeholder UI behavior.
- The cleanup only moved option sourcing and naming clarity into the correct seams.

## Tests
- Updated world-map presentation tests for the renamed build-summary method.
- Added service coverage for character-specific gear persistence across selection changes.
- Added service coverage that unknown owned gear ids do not become bogus build options.
