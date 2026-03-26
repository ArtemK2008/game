# Milestone 058 - Implement Boss Entity Encounter Model

## Decision
- The smallest honest boss-baseline step is one explicit gate-boss encounter model on top of the current encounter-content seam from 056a.
- The current combat runtime remains the same 1v1 deterministic auto-battle loop.
- Boss content ownership should stay separate from standard-enemy content ownership.

## Delivered
- Added one explicit boss encounter model through:
  - `CombatEncounterType`
  - `CombatBossRoleType`
  - `CombatHostileEntityType`
  - `CombatStandardEncounterDefinition`
  - `CombatBossEncounterDefinition`
- Split encounter content ownership into:
  - `CombatStandardEncounterCatalog`
  - `CombatBossEncounterCatalog`
- Split boss profile ownership into:
  - `CombatBossProfileCatalog`
- The shipped gate node mapping now uses the explicit `Gate Boss` encounter model instead of a boss/gate placeholder encounter.
- `CombatEnemyProfileResolver` now resolves the primary hostile profile from the encounter definition without needing any standard-only assumptions.

## Behavior Change
- Current shipped gameplay remains on the same combat foundation:
  - one player entity versus one hostile entity
  - deterministic auto-targeting
  - automated attacks and skill execution
- The first boss encounter is still stat-driven for now, but it is now modeled explicitly as boss content rather than only as a renamed standard-enemy-style entry.
- Current shipped gate-node combat outcomes remain the same as before this milestone because the gate boss keeps the existing hostile stats.
- The only visible content-level change is that the current gate hostile now presents as `Gate Boss`.

## SRP Notes
- World/content ownership still maps nodes to encounters in `BootstrapWorldGraphBuilder`.
- Standard encounter data stays in `CombatStandardEncounterCatalog`.
- Boss encounter data stays in `CombatBossEncounterCatalog`.
- Boss profile data stays in `CombatBossProfileCatalog`.
- Combat runtime still only resolves from already-supplied encounter content and does not absorb world-specific boss rules.

## Tests
- Added direct encounter-model coverage for:
  - shipped standard encounter exposure
  - shipped boss encounter exposure
  - rejecting construction of a boss encounter from a standard-enemy profile
- Updated resolver tests to prove boss encounter resolution now returns the explicit boss profile.
- Updated world-graph and node-entry tests to prove bootstrap boss nodes carry boss encounter content.
- Updated combat/run tests to prove the gate node still enters the existing 1v1 shell with the explicit boss model and expected hostile stats.

## Out Of Scope
- boss ability systems
- boss-specific AI behavior
- broader boss encounter pipelines
- Milestone 059 or later work
