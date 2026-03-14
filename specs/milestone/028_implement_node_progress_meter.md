# Milestone 028 - Implement Node Progress Meter

## Delivered
- Added persistent per-node progress tracking to `PersistentWorldState.NodeStates` through `PersistentNodeState`.
- Added `NodeProgressMeterService` as the small accumulation layer for node progress updates.
- Wired successful combat-node runs to grant progress from defeated enemies in the current 1v1 prototype:
  - one defeated enemy grants `+1` node progress
  - repeated successful runs accumulate progress on the same node
- Kept `BossOrGate` on the same tracked progress rule and default threshold as ordinary combat nodes as a temporary MVP placeholder.
- Seeded the bootstrap world state with persistent node progress values so the current placeholder world map flow has readable baseline data.
- Surfaced node progress minimally in the post-run summary:
  - current total
  - current threshold
  - per-run delta

## Tests
- Added `NodeProgressMeterServiceTests` to verify:
  - combat-node progress starts at the expected default
  - successful combat progress is applied
  - repeated successful runs accumulate progress
  - failed runs do not grant the same success progress
  - reaching the current clear threshold is reported correctly
- Updated `RunLifecycleControllerTests` to verify successful combat resolution grants node progress and failed runs do not.
- Updated `NodePlaceholderScreenUiTests` and `BootstrapStartupScreenFlowTests` to verify node progress appears in the current placeholder flow and persists across repeat runs/return-to-world flow.
- Updated `PostRunStateControllerTests` for the extended `RunResult` shape.

## Intentionally Left Out
- Dedicated boss clear semantics
- Dedicated gate-opening semantics
- Route unlock application
- Full reward tables or economy
- Multi-enemy combat progress rules
- Broad progression services beyond the per-node progress meter
- World-map UI redesign for dedicated progress presentation
- Revisit of `BossOrGate` progress behavior, which is deferred to Milestone 029 / 030 and later boss-focused milestones

## Verification
- Unity EditMode batch run passed:
  - `Logs/m028_editmode_results.xml`
  - `Logs/m028_editmode.log`
- Result: `111` passed, `0` failed
