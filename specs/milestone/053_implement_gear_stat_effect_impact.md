# Milestone 053 - Implement Gear Stat Effect Impact

## Decision
- The smallest honest 053 step is one flat stat effect on the already shipped `PrimaryCombat` item instead of a broader gear-effect framework.
- `Training Blade` now grants a persistent flat `+2` attack-power bonus when it is equipped on the selected character.
- The world-map build-summary cleanup is included in the same milestone by renaming the awkward `BuildBuildAssignmentText(...)` seam to `BuildAssignmentText(...)`.

## Delivered
- Extended `GearProfile` and `GearCatalog` so the shipped `Training Blade` definition now carries one explicit combat-relevant stat bonus.
- Added `PlayableCharacterGearCombatEffectResolver` so gear combat interpretation stays in one focused resolver rather than leaking into UI or run-flow code.
- Wired that resolver into `CombatShellContextFactory`, so equipped persistent gear now contributes to player combat baseline stats before the encounter starts.
- Kept the effect intentionally small:
  - no new gear category
  - no new item
  - no gear UI redesign
  - no inventory/loot system expansion
- Renamed the world-map build-summary helper from `BuildBuildAssignmentText(...)` to `BuildAssignmentText(...)` and updated the direct callers/tests.

## Behavior Change
- Gameplay changed only in the intended 053 way.
- Unequipped characters keep their previous combat baseline.
- Equipping `Training Blade` increases the selected character's future run-entry attack power by `+2`.
- That effect is persistent between runs because it comes from the existing equipped-gear state on the selected character.
- The effect is visible in combat outcomes: the equipped character clears ordinary autobattle encounters faster.
- World-map gear equip flow, package assignment flow, and scroll/layout behavior remain unchanged.

## SRP Notes
- `GearCatalog` still owns shipped gear definitions only.
- `PlayableCharacterGearCombatEffectResolver` owns gear-to-combat stat interpretation.
- `CombatShellContextFactory` still only assembles combat baseline state from already resolved modifier sources.
- `WorldMapScreenTextBuilder` naming is now aligned with its actual broader build-summary responsibility.

## Tests
- Added direct coverage for gear attack-power effect resolution.
- Updated catalog coverage to assert the shipped gear stat definition.
- Updated combat-shell and run-context tests to verify equipped gear changes attack power at run entry.
- Added end-to-end run-flow coverage proving equipped `Training Blade` shortens standard combat while unequipped baseline behavior remains intact.
- Updated world-map presentation coverage to follow the cleaner build-summary helper name.

## Out Of Scope
- additional gear items
- additional gear categories
- gear-driven skill modification
- inventory browsing or loot flow
- broader gear-effect systems
- any Milestone 054 or later work
