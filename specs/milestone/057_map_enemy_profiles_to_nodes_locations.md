# Milestone 057 - Map Enemy Profiles To Nodes Locations

## Goal
- The current branch already satisfies the core 057 requirement.
- Enemy profile selection is already driven by node/location content instead of hardcoded resolver branching because 056a moved current shipped enemy mapping into encounter content attached to world nodes and carried into `NodePlaceholderState`.
- The smallest honest 057 delivery is therefore a documentation/acceptance milestone, not another runtime system.

## Delivered
- Verified that current enemy profile selection is content-driven in the shipped bootstrap world:
  - combat-capable `WorldNode` content carries `CombatEncounterDefinition`
  - `WorldNodeEntryFlowController` carries that encounter content into `NodePlaceholderState`
  - `CombatEnemyProfileResolver` resolves the enemy profile from `NodePlaceholderState.CombatEncounter`
- Verified that current shipped content mapping is already simple and location-aware enough for the prototype:
  - forest entry and forest farm resolve `Enemy Unit`
  - forest push resolves `Bulwark Raider`
  - current gate/boss placeholder nodes resolve `Gate Enemy`
- Updated the compact build snapshot so Milestone 057 is reflected explicitly in the current implemented state.

## Behavior Change
- No gameplay behavior changed.
- No combat, world-map, reward, progression, or UI behavior changed.
- Current shipped enemy mapping stays exactly the same.

## SRP Notes
- Combat resolver classes do not own bootstrap/world-specific enemy mapping rules anymore.
- Content ownership remains in the world/content seam through node-attached encounter data.
- UI and run-flow classes still do not absorb enemy-selection logic.

## Tests
- No runtime code change was needed for 057.
- Existing current coverage already verifies the shipped content-driven mapping through:
  - bootstrap world graph encounter attachment tests
  - world node entry encounter carry-through tests
  - combat enemy profile resolution tests

## Out Of Scope
- any broader authored encounter pipeline
- broader region/faction frameworks
- enemy behavior controllers or ability systems
- any Milestone 058 or later work

## Verification
- The milestone was verified by rerunning the existing EditMode suite against the current branch state.
