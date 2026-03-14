# Milestone 024 - Implement Basic Auto-Attack Loop

## Delivered
- Added `CombatAutoAdvanceLoop` as the thin runtime auto-progression layer above `CombatEncounterResolver`.
- Kept combat deterministic by advancing the existing resolver in fixed `0.25s` ticks while the run remains in `RunActive` combat.
- Removed the need for manual combat stepping during normal combat flow.
- Updated the node placeholder shell so combat starts once, then progresses automatically until one side wins.
- Kept the combat shell visible as a debug-style runtime view while changing the active combat button state to `Combat Auto-Running`.

## Tests
- Added `CombatAutoAdvanceLoopTests` for:
  - automatically advancing combat to a resolved outcome
  - deterministic repeated progression across small fixed ticks
  - preventing extra combat advancement after resolution
- Updated `NodePlaceholderScreenUiTests` to verify combat-node UI shows auto-running behavior and that combat can reach post-run without manual advance clicks.
- Updated `BootstrapStartupScreenFlowTests` so the real node-entry flow drives automatic combat progression correctly.

## Intentionally Left Out
- Manual player attack input
- Pause, speed, or time-scale controls
- Multi-entity combat auto-loops
- Animation, VFX, movement, or broader game-loop orchestration
- Any combat-engine redesign beyond the thin shell/runtime auto-advance layer

## Verification
- Unity EditMode batch run passed:
  - `Logs/m024_editmode_results.xml`
  - `Logs/m024_editmode.log`
- Practical runtime Game-view verification passed:
  - `Logs/m024_auto_attack_runtime_check.txt`
  - `Logs/m024_auto_attack_runtime_check.png`
  - Verified flow:
    - entered the combat node from the world map
    - started the combat shell once
    - observed combat progress to `PlayerVictory` without manual advance input
