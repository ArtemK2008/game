# Milestone 051 - Implement Persistent Gear Data Model

## Decision
- The smallest honest 051 shape was implemented as a persistence-only gear baseline.
- `PrimaryCombat` is the only live gear category in the shipped build.
- The build now has one shipped starter gear entry, `gear_primary_training_blade` / `Training Blade`.
- Gear still has no combat effect and no equip UI in this milestone.

## Delivered
- Added a small code-driven gear catalog under `Survivalon.Data.Gear`:
  - `GearIds`
  - `GearProfile`
  - `GearCatalog`
- Used the existing persistent gear seams instead of inventing a parallel system:
  - account-owned gear ids remain on `PersistentGameState`
  - per-character equipped gear remains on `PersistentLoadoutState` / `EquippedGearState`
- Added `PersistentGearStateInitializer` to make the placeholder gear model coherent during bootstrap and persisted-state normalization:
  - starter owned gear is ensured
  - owned gear ids are de-duplicated and cleaned of blank entries
  - equipped gear is normalized against owned gear, current shipped gear ids, category match, and one item per category
- `PersistentPlayableCharacterInitializer` now delegates gear-state initialization to that focused gear initializer after ensuring the shipped playable-character state exists.
- `PersistentCharacterState.LoadoutState`, `PersistentLoadoutState`, and `PersistentGameState.OwnedGearIds` are now null-safe for the touched gear persistence path.

## Behavior
- Gameplay behavior is unchanged in this milestone.
- Characters still start with no gear equipped in the bootstrap state.
- The build now persistently owns one starter primary-gear item and preserves valid equipped-gear data through bootstrap/save-load/safe-resume paths.
- Run entry now carries the selected character's persistent loadout data as part of the existing character state, but gear still has no live combat/stat effect.

## SRP Notes
- `GearCatalog` owns only the currently shipped gear definitions.
- `PersistentGameState` owns account-wide gear ownership state, not gear validation rules.
- `PersistentLoadoutState` owns equipped-gear data storage, not catalog policy.
- `PersistentGearStateInitializer` owns the startup/persistence normalization rules for current gear data.
- Combat/stat effect logic was intentionally not added in this milestone.

## Tests
- Added `GearCatalogTests` for the shipped starter gear data.
- Added `PersistentGearStateInitializerTests` for starter ownership and loadout normalization.
- Added `RunPersistentContextTests` to verify selected-character persistent gear data reaches run context without changing current combat baseline.
- Updated startup and safe-resume tests to verify starter gear ownership and equipped-gear persistence through the current save/load path.
- Updated persistent playable-character initialization tests to verify default bootstrap gear state stays valid and empty-equipped by default.

## Intentionally Left Out
- gear equip/assignment UI and flow
- gear combat/stat effects
- gear loot acquisition
- additional gear categories
- broader inventory/itemization systems
- any Milestone 052 or later work
