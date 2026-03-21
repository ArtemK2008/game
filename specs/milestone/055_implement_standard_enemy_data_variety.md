# Milestone 055 - Implement Standard Enemy Data Variety

## Decision
- The smallest honest 055 step is one additional standard enemy profile, not a broader faction or encounter system.
- The new shipped standard enemy is `Bulwark Raider`, a durability-focused variant that keeps the current 1v1 autobattle readable.
- The current forest push combat node now resolves to `Bulwark Raider`, while the farm combat node keeps the existing `Enemy Unit`.

## Delivered
- Added a small combat-enemy profile data seam:
  - `CombatEnemyProfile`
  - `CombatEnemyProfileCatalog`
  - `CombatEnemyProfileResolver`
- Moved standard enemy definition data out of `CombatShellContextFactory` into the new data catalog and resolver seam.
- Added one additional standard enemy profile:
  - `Enemy Unit` remains the baseline standard enemy
  - `Bulwark Raider` is the new more durable standard enemy with higher max health
- Kept the existing boss/gate placeholder behavior intact through the same resolver seam.
- Wired the existing forest push combat node into the new `Bulwark Raider` profile so current shipped content can actually encounter the new enemy type.

## Behavior
- Gameplay changed only in the intended 055 way.
- Standard combat now has two shipped enemy profiles:
  - `Enemy Unit`
  - `Bulwark Raider`
- `Bulwark Raider` is currently differentiated through durability:
  - `Enemy Unit` stays at `75` max health
  - `Bulwark Raider` uses `105` max health
- The forest push combat node now takes longer to clear and leaves the player with less remaining health than the current farm combat node, while still resolving successfully in the current prototype.
- Current boss/gate placeholder behavior, run rewards, node progress rules, and world-map flow stay unchanged.

## SRP Notes
- `CombatEnemyProfileCatalog` owns shipped enemy definition data only.
- `CombatEnemyProfileResolver` owns node-context-to-enemy-profile selection logic.
- `CombatShellContextFactory` now assembles combat context from already resolved enemy profiles instead of storing standard enemy data branches directly.
- UI and run orchestration code still do not own enemy-profile selection rules.

## Tests
- Added direct resolver coverage for:
  - the new `Bulwark Raider` mapping on the forest push node
  - baseline standard enemy fallback on the farm node
  - boss/gate enemy resolution
  - non-combat node rejection
- Added direct combat-context coverage for the new durable standard enemy profile.
- Added run-flow coverage proving the push-node enemy now resolves more slowly than the farm-node enemy because of its higher durability.
- Added startup-flow coverage proving current shipped world-map content can actually enter the `Bulwark Raider` encounter.

## Intentionally Left Out
- broader enemy faction systems
- more than one additional standard enemy profile
- new boss content
- loot drops or enemy-specific rewards
- any Milestone 056 or later work
