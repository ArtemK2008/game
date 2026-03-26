# Milestone 054 - Implement Second Gear Slot Category

## Decision
- Milestone 053 already proved the first gear slot was useful, so 054 adds exactly one more live category and stops there.
- The smallest honest second category is `SecondarySupport`, with one survivability-focused starter item instead of a broader inventory matrix.
- The shipped support item is `gear_secondary_guard_charm` / `Guard Charm`, and it grants a flat `+40` max-health bonus when equipped.

## Delivered
- Extended the shipped gear catalog from one live item to two:
  - `Training Blade` in `PrimaryCombat`
  - `Guard Charm` in `SecondarySupport`
- Extended gear definitions and combat resolution so the second slot changes combat through one small survivability stat bonus.
- Generalized the selected-character gear assignment service just enough to support two categories cleanly instead of a primary-only public API.
- Kept the current world-map build area compact by reusing the existing placeholder gear row and build summary instead of adding a new screen.
- Updated startup/persistence normalization so both shipped starter gear ids are present in owned gear data and both categories survive save/load safely.

## Behavior Change
- Gameplay changed only in the intended 054 way.
- The build now has exactly two live gear categories, not a broader multi-slot inventory system.
- The currently selected character can equip:
  - one `PrimaryCombat` item
  - one `SecondarySupport` item
- Those assignments remain character-specific and persistent between runs.
- `Training Blade` still grants `+2` attack power.
- `Guard Charm` now grants `+40` max health.
- Equipping `Guard Charm` makes combat visibly different by increasing run-entry max health and leaving the player with more remaining health after standard autobattle encounters.
- World-map package assignment, node selection/entry behavior, and node-list scrolling remain unchanged.

## SRP Notes
- `GearCatalog` and `GearProfile` still own shipped gear-definition data only.
- `PersistentGearStateInitializer` still owns starter-gear ownership and loadout normalization.
- `PlayableCharacterGearAssignmentService` now owns selected-character gear assignment rules for both live categories instead of only `PrimaryCombat`.
- `PlayableCharacterGearCombatEffectResolver` still owns gear-to-combat stat interpretation.
- `WorldMapScreen` remains UI wiring only; it orchestrates the existing selection/build services without absorbing gear rules or combat policy.

## Tests
- Updated catalog coverage to assert both shipped gear categories and item definitions.
- Added secondary-slot coverage for:
  - owned/equippable option resolution
  - selected-character assignment
  - persistent startup/safe-resume normalization
  - combat stat resolution
  - end-to-end run-result difference from the support slot
- Updated world-map placeholder UI tests so they verify both gear buttons exist, both slots can be equipped/unequipped, and the scrollable node-list viewport remains usable after the extra build control is present.

## Out Of Scope
- more than two live gear categories
- additional support-slot items
- loot flow or inventory browsing
- rarity systems or instance-based gear
- any Milestone 055 or later work
