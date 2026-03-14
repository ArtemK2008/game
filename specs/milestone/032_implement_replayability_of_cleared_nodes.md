# Milestone 032 - Implement Replayability Of Cleared Nodes

## Delivered
- Confirmed and tightened the current MVP rule that cleared nodes remain replayable.
- Verified that a `Cleared` node can still be selected and entered from the world map when it is otherwise reachable in the current world flow.
- Locked in that replaying or re-entering a cleared node:
  - preserves the node's persistent `Cleared` state
  - does not re-lock already unlocked connected nodes
  - keeps the real placeholder world-map -> node-entry -> post-run -> return flow working
- Kept the farming meaning minimal and MVP-aligned:
  - cleared nodes remain low-friction repeatable content
  - no new reward or economy system was introduced in this milestone

## Tests
- Updated `WorldNodeEntryFlowControllerTests` to verify a reachable cleared node can be entered without regressing its persistent state.
- Updated `BootstrapStartupScreenFlowTests` to verify the real flow:
  - clear a tracked node
  - unlock the next connected node
  - move through the world flow until the cleared node is reachable again
  - re-enter the cleared node
  - confirm the cleared state persists and unlock state is not corrupted on return

## Intentionally Left Out
- New reward or economy outputs for farming cleared nodes
- Advanced farm-mode UX or automation
- Boss/gate replay semantics beyond the current shared placeholder rule
- Broader progression redesign beyond preserving cleared-node replayability

## Verification
- Unity EditMode batch run passed:
  - `Logs/m032_editmode_results.xml`
  - `Logs/m032_editmode.log`
