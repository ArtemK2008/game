# Milestone 056 - Implement Enemy Profile Differences In Combat

## Decision
- The smallest honest 056 step is to deepen the current stat identity of the shipped standard enemy profiles instead of adding a broader enemy ability system.
- `Enemy Unit` now acts as the sharper early-pressure baseline enemy through faster attack cadence and lighter defenses.
- `Bulwark Raider` now acts as the sturdier push-node pressure source through higher survivability plus slower, heavier hits.

## Delivered
- Reused the existing enemy-profile seam from 055 without broadening combat architecture:
  - `CombatEnemyProfile`
  - `CombatEnemyProfileCatalog`
  - `CombatEnemyProfileResolver`
- Updated the shipped combat profiles so the current standard enemies differ through real combat-facing pressure:
  - `Enemy Unit`: `75` max health, `7` attack power, `1.25` attack rate, `2` defense
  - `Bulwark Raider`: `105` max health, `9` attack power, `0.85` attack rate, `6` defense
- Kept current boss/gate placeholder behavior unchanged.
- Preserved the current content mapping:
  - farm combat node keeps `Enemy Unit`
  - forest push combat node keeps `Bulwark Raider`

## Behavior Change
- Gameplay changed only in the intended 056 way.
- The two shipped standard enemies now create different combat pressure in practice:
  - `Enemy Unit` threatens the player sooner through faster repeated attacks
  - `Bulwark Raider` survives longer, deals heavier hits, and produces stronger total attrition over the full fight
- The current farm-node fight still resolves successfully and quickly for the baseline build.
- The current push-node fight still resolves successfully, but it now feels different for two reasons at once:
  - higher survivability
  - different incoming-pressure pacing
- World-map flow, run flow, gear flow, skill flow, and current boss/gate placeholder behavior remain unchanged.

## SRP Notes
- `CombatEnemyProfileCatalog` still owns shipped enemy-definition data only.
- `CombatEnemyProfileResolver` still owns node-context-to-profile selection logic only.
- No new enemy-behavior abstraction was added because current stat-driven differentiation already solves the 056 requirement cleanly.
- Combat execution, UI, and run orchestration classes did not absorb enemy-specific selection or behavior branches.

## Tests
- Updated direct resolver coverage so both shipped standard profiles assert their current combat-facing stats.
- Added direct coverage that the shipped standard enemy profiles now differ by both threat cadence and survivability.
- Updated combat-context coverage for the new `Enemy Unit` and `Bulwark Raider` profile stats.
- Added runtime combat-flow coverage proving:
  - `Enemy Unit` creates sharper early pressure after the same elapsed time window
  - `Bulwark Raider` still creates stronger full-fight attrition through a longer sturdier encounter
- Updated startup-flow coverage so the push-node placeholder still surfaces the current `Bulwark Raider` combat card text.

## Out Of Scope
- enemy-specific skills or special attacks
- broader faction or enemy-family systems
- reward/loot differentiation by enemy
- any Milestone 057 or later work
