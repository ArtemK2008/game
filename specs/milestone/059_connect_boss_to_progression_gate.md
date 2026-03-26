# Milestone 059 - Connect Boss To Progression Gate

## Decision
- The smallest honest 059 step is to keep boss progression mapping in world/bootstrap content instead of pushing route-target logic into combat resolvers or UI code.
- Boss defeat now produces one explicit major progression-gate result in the shipped bootstrap world.

## Delivered
- Added one small world-content seam for boss-gate output:
  - `BossProgressionGateDefinition`
  - `BossProgressionGateUnlockService`
- `WorldNode` and `NodePlaceholderState` now carry optional boss-gate progression content for boss/gate nodes.
- The bootstrap world now marks the forest gate node with one boss-gate unlock target:
  - defeating the forest `Gate Boss` opens `region_002_node_002` / `Cavern Gate`
- The bootstrap route graph now includes the minimal forward connection needed for that newly opened target to be reachable after returning from the forest boss run.
- Run progression resolution now applies the boss-gate unlock separately from the ordinary clear-threshold route-unlock rule.
- Post-run summary output now includes a compact explicit line when a boss gate opens:
  - `Boss gate unlock: Cavern gate opened`

## Behavior Change
- Ordinary combat-node clears still use the existing kill-progress threshold rule to unlock directly connected next nodes.
- Boss defeat is now more meaningful than an ordinary standard-node success:
  - the forest `Gate Boss` can unlock a new deeper reachable target before the placeholder boss node reaches its current tracked-progress threshold
  - the unlocked `Cavern Gate` becomes visible and enterable through the normal world-map flow after returning from the successful boss run
- Current shipped combat, world-map layout, node-entry flow, rewards, gear, and skill-package assignment behavior otherwise remain unchanged.

## SRP Notes
- World/bootstrap content owns the boss-gate target mapping.
- The new boss-gate unlock service owns only boss progression-gate application.
- `RunProgressResolutionService` still orchestrates run-result progression outcomes, but boss-gate policy is delegated instead of smeared across UI or combat classes.
- UI code only displays the boss-gate result carried in `RunResult`.

## Tests
- Added isolated coverage for boss-gate unlock application and reachability.
- Updated run-progress resolution tests to prove:
  - successful boss defeat unlocks the intended gate target
  - ordinary node clears do not emit the boss-gate unlock summary
- Updated bootstrap world/content tests to prove:
  - the forest gate node carries boss-gate progression content
  - the bootstrap graph includes the forward connection needed for the unlocked target
- Updated startup flow coverage to prove the shipped placeholder loop now:
  - defeats the forest boss
  - shows the explicit boss-gate unlock line in post-run summary
  - returns to a world map where `Cavern Gate` is available

## Verification
- `tools/run_editmode_tests.ps1` detached again without producing artifacts in this shell.
- Verification then used the direct waited Unity batch invocation.
- Result: `416 passed`, `0 failed`
- Artifacts:
  - `Logs/m059_editmode_results.xml`
  - `Logs/m059_editmode.log`

## Out Of Scope
- Milestone 060 boss reward differentiation
- broader boss reward bundles
- boss-specific ability systems
- boss clear-state redesign beyond the current small progression-gate connection
