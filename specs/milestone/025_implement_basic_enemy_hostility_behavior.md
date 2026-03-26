# Milestone 025 - Implement Basic Enemy Hostility Behavior

## Delivered
- Audited the current combat prototype and confirmed that basic enemy hostility already existed before this milestone:
  - enemy-side targeting of the player-side entity
  - enemy attacks resolving on enemy timing
  - enemy-driven player defeat resolving the run as failure
- Kept that working behavior intact instead of duplicating it.
- Made the hostility path more explicit in `CombatEncounterResolver` by separating player attack resolution from enemy-hostility resolution.
- Updated the combat shell status text so active combat clearly states that enemy hostility and player attacks resolve automatically.

## Tests
- Updated `CombatEncounterResolverTests` to prove enemy hostility against the player side explicitly.
- Added `CombatAutoAdvanceLoopTests.ShouldResolveAutoAdvancedBossCombatAsFailedWhenHostileEnemyWins` to verify hostile enemies can defeat the player through the normal auto-combat loop.
- Updated `NodePlaceholderScreenUiTests` to verify the combat shell shows explicit hostile targeting and that the player loses health automatically during combat.

## Out Of Scope
- Advanced AI or behavior trees
- Aggro, threat, or priority systems
- Movement/pathing changes
- Skills, formations, or multi-enemy coordination
- Combat-flow redesign

## Verification
- Unity EditMode batch run passed:
  - `Logs/m025_editmode_results.xml`
  - `Logs/m025_editmode.log`
- Practical runtime Game-view verification passed:
  - `Logs/m025_enemy_hostility_runtime_check.txt`
  - `Logs/m025_enemy_hostility_runtime_check.png`
  - Verified flow:
    - entered a combat node from the world map
    - started the combat shell once
    - observed explicit hostile targeting in the combat shell
    - observed the player lose health automatically from enemy hostility without manual attack input
