# Milestone 056a - Cleanup Enemy Content And Profile Ownership

## Decision
- The smallest honest follow-up after 055 and 056 is to clean up enemy-content ownership without changing shipped combat results.
- Enemy selection should come from small encounter/content data attached to combat-capable nodes, not from bootstrap node-id branching inside `CombatEnemyProfileResolver`.
- Standard enemy profile data and boss/gate placeholder profile data should no longer share one combined catalog.

## Delivered
- Added a small combat-encounter content seam:
  - `CombatEncounterDefinition`
  - `CombatEncounterCatalog`
- Bootstrap world nodes now carry their current shipped combat encounter content directly:
  - forest entry and forest farm use the `Enemy Unit` encounter
  - forest push uses the `Bulwark Raider` encounter
  - current gate/boss placeholder nodes use the gate placeholder encounter
- `WorldNodeEntryFlowController` now carries that encounter content into `NodePlaceholderState`.
- `CombatEnemyProfileResolver` now resolves the enemy profile from `NodePlaceholderState.CombatEncounter` instead of branching on bootstrap node ids.
- Split enemy profile data ownership into:
  - `CombatStandardEnemyProfileCatalog`
  - `CombatBossPlaceholderProfileCatalog`
- Added one tiny future-facing behavior seam on enemy profiles through `CombatEnemyBehaviorType` so later enemy behavior differences have an explicit ownership slot without changing runtime combat logic yet.

## Behavior Change
- Gameplay behavior is unchanged.
- Current farm node, push node, and boss/gate placeholder encounters still resolve to the same enemy profiles and produce the same current combat results.
- World-map flow, run flow, rewards, progression, and UI behavior remain unchanged.

## SRP Notes
- `CombatEncounterCatalog` owns current shipped combat encounter content only.
- `CombatStandardEnemyProfileCatalog` owns shipped standard-enemy data only.
- `CombatBossPlaceholderProfileCatalog` owns gate/boss placeholder enemy data only.
- `CombatEnemyProfileResolver` now owns only placeholder-node-to-enemy-profile resolution from already supplied encounter content.
- `BootstrapWorldGraphBuilder` owns the current bootstrap-node-to-encounter mapping as content data instead of leaking that mapping into combat-resolution code.

## Tests
- Added direct data coverage for the new encounter catalog and the split boss/standard profile ownership.
- Updated bootstrap world graph tests to prove encounter content is attached to the expected shipped nodes.
- Updated world-node entry tests to prove bootstrap encounter content is carried into placeholder state.
- Updated resolver tests to prove:
  - standard and gate encounters still resolve correctly
  - missing encounter content is rejected explicitly for combat nodes
  - the tiny enemy behavior-type seam is exposed on the shipped profiles

## Out Of Scope
- any broader authored encounter-data system
- enemy abilities or AI systems
- broader faction/content pipelines
- any Milestone 057 or later work
