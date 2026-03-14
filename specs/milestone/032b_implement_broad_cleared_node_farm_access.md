# Milestone 032b - Implement Broad Cleared Node Farm Access

## Delivered
- Added `WorldNodeAccessResolver` as the single rule layer for world-map enterability.
- Kept the existing path-based reachability rules for normal progression nodes.
- Extended world-map access so persistently `Cleared` nodes remain enterable for farming even when they are no longer reachable through the normal forward/backtrack path rules.
- Reused the same access rule in both:
  - `WorldMapScreenController` for node selection
  - `WorldNodeEntryFlowController` for node-entry validation
- Kept the current MVP progression behavior unchanged:
  - locked nodes still stay locked
  - uncleared available / in-progress nodes still depend on normal reachability
  - cleared node replay does not regress persistent state
  - already unlocked connected nodes are not re-locked or corrupted

## Tests
- Added `WorldNodeAccessResolverTests` to verify:
  - a persistently cleared node becomes enterable even when old path rules would not reach it
  - uncleared or locked nodes do not gain the same farm access
- Updated `WorldMapScreenControllerTests` to verify a cleared farm node is selectable while an unreachable uncleared node is not.
- Updated `WorldNodeEntryFlowControllerTests` to verify:
  - a persistently cleared unreachable node can still be entered
  - an unreachable uncleared node still cannot be entered
  - locked nodes still cannot be entered
- Updated `BootstrapStartupScreenFlowTests` to verify the real placeholder flow:
  - load a world state where a cleared node is not normally reachable
  - enter it from the world map anyway
  - resolve and return cleanly
  - preserve cleared state and unlocked route state

## Intentionally Left Out
- Reward or economy expansion for farming
- Any redesign of world graph structure or route rules
- Boss/gate redesign beyond the current shared placeholder behavior
- Automation improvements beyond broad world-map farm access for cleared nodes

## Verification
- Unity EditMode batch run passed:
  - `Logs/m032b_editmode_results.xml`
  - `Logs/m032b_editmode.log`
