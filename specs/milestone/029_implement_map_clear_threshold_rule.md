# Milestone 029 - Implement Map Clear Threshold Rule

## Delivered
- Confirmed and locked in the persistent clear-at-threshold rule for tracked combat-oriented nodes.
- Tracked node progress now explicitly counts as a map clear when the persistent progress meter reaches its threshold:
  - progress remains kill-driven
  - the node state becomes `Cleared`
  - the cleared state persists through the existing world-state flow
- Reused the existing `PersistentNodeState` threshold transition instead of introducing a second clear-tracking path.

## Tests
- Updated `NodeProgressMeterServiceTests` to verify:
  - tracked combat nodes stay capped at the threshold once cleared
  - progress does not drift past the clear state after threshold is reached
- Updated `BootstrapStartupScreenFlowTests` to verify:
  - repeated successful combat runs can clear the tracked combat node in the real placeholder flow
  - the saved persistent node state becomes `Cleared`
  - the world map reflects the cleared node state through the existing UI flow
- Failed-run non-clear coverage remains in the existing node-progress and lifecycle tests.

## Out Of Scope
- Unlocking connected nodes or routes
- Branch unlock logic
- Reward/economy expansion
- Boss/gate-specific clear semantics

This milestone only implements node clear-at-threshold.
Unlocking connected nodes is deferred to Milestone 030.
`BossOrGate` continues to use the temporary shared tracked-progress rule and threshold placeholder until later progression/boss milestones revisit it explicitly.

## Verification
- Unity EditMode batch run passed:
  - `Logs/m029_editmode_results.xml`
  - `Logs/m029_editmode.log`
