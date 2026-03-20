# Milestone 049 - Implement Build-Facing Skill Selection Or Assignment

## Decision
- Combat already resolved skill packages from persistent character state, but package assignment was still implicit in code and not player-facing.
- The smallest honest next step was to expose one minimal persistent package-assignment path on the existing world-map placeholder instead of building a separate skill screen or broader loadout editor.
- To keep scope tight while still proving the assignment matters, only `Vanguard` gained one additional valid package option. `Striker` stayed on its current single shipped package.

## Delivered
- Added a small character-facing package-assignment layer on top of the existing `skillPackageId` seam:
  - `PlayableCharacterSkillPackageCatalog` now defines the valid assignable package options per playable character
  - `PlayableCharacterSkillPackageAssignmentService` now resolves valid options for the selected character, applies requested assignments, and normalizes invalid persisted package ids
- Added one meaningful alternate package:
  - `Vanguard` now supports `Standard Guard` as the default package
  - `Vanguard` can also switch to `Burst Drill`, which adds the current periodic active skill `Burst Strike`
  - `Striker` keeps the current `Relentless Burst` package with `Relentless Assault` plus `Burst Strike`
- Extended the current placeholder world map UI with a minimal package-assignment section:
  - it shows the selected character's currently assigned package and summary
  - it only shows valid package buttons for the currently selected character
  - clicking a package button updates the selected character's persistent assignment immediately
- Preserved current combat integration:
  - future run entry already resolves skill packages from persistent character state, so assigned packages now flow into combat automatically
  - baseline attack, passive skill behavior, and current auto-triggered active skill behavior all remain unchanged

## SRP Notes
- The new assignment policy was kept out of `CombatSkillPackageCatalog` on purpose.
- `CombatSkillPackageCatalog` still only answers combat-side questions about what a package does in combat.
- Character-facing package validity, normalization, and assignment now live in the dedicated `PlayableCharacterSkillPackageAssignmentService`, while the world-map `MonoBehaviour` only handles button wiring and refresh.

## Tests
- Added `PlayableCharacterSkillPackageAssignmentServiceTests` to verify:
  - valid package options resolve for the selected character
  - invalid package assignment is rejected
  - valid assignment updates persistent character state
  - invalid persisted package ids normalize safely back to valid character-owned defaults
- Updated `PersistentPlayableCharacterInitializerTests` to verify startup normalization now also fixes invalid persisted package assignments
- Updated `CombatEntityStateTests` and `RunLifecycleControllerCombatTests` to verify:
  - assigned package overrides are used on combat entry
  - `Vanguard` with `Burst Drill` gains `Burst Strike`
  - the current boss/gate placeholder encounter outcome changes meaningfully when `Vanguard` is assigned the alternate package
- Updated world-map placeholder tests to verify:
  - the package-assignment summary is shown
  - package buttons reflect the current assignment
  - switching character selection refreshes the visible valid package set
  - pressing a package button updates persistent assignment state and placeholder text

## Intentionally Left Out
- A dedicated skill screen
- Drag/drop loadouts or inventory-style skill management
- More than one additional package option
- Broader package matrices across the roster
- Cooldown UI, mana UI, or broader combat UX work
- Any Milestone 050 or later work

## Verification
- Run batch Edit Mode tests with `tools/run_editmode_tests.ps1`.
- If that shell returns before Unity finishes, use the direct waited Unity batch invocation and report that explicitly.
