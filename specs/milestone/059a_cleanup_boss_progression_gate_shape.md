# Milestone 059a - Cleanup Boss Progression Gate Shape

## Decision
- The smallest honest follow-up after 059 is to keep boss-gate behavior unchanged while separating ordinary route unlock state from boss-gate unlock state.
- Boss progression domain flow should return structured unlock data, not player-facing summary strings.

## Delivered
- `DidUnlockRoute` now means only the ordinary clear-threshold route-unlock rule again.
- Added `BossProgressionGateUnlockResult` as the small structured boss-gate outcome type.
- `RunProgressResolution` and `RunResult` now carry boss-gate unlock data separately from ordinary route unlock state.
- `BossProgressionGateDefinition` now stores only world/progression mapping data:
  - unlocked target node id
- `BossProgressionGateUnlockService` now returns structured boss-gate unlock data instead of summary text.
- `PostRunSummaryTextBuilder` now builds the readable boss-gate line from structured domain data in the presentation layer.

## Behavior Change
- Gameplay behavior is unchanged.
- Defeating the forest `Gate Boss` still unlocks `region_002_node_002` / `Cavern Gate`.
- `Cavern Gate` still becomes reachable on return to the world map.
- Post-run summary still shows:
  - `Boss gate unlock: Cavern gate opened`
- Ordinary clear-threshold route unlock behavior remains unchanged and is now tracked separately from boss-gate unlock state.

## SRP Notes
- World/bootstrap content still owns boss-node-to-target mapping.
- `BossProgressionGateUnlockService` owns only boss-gate application.
- `RunProgressResolutionService` still orchestrates progression outcomes but no longer carries presentation wording through domain flow.
- `PostRunSummaryTextBuilder` owns player-facing boss-gate wording.

## Tests
- Added direct coverage proving boss-gate unlock is represented as structured domain data rather than a presentation-text field in the world/progression model.
- Updated run progress and run result tests to prove:
  - ordinary route unlock and boss-gate unlock stay separate
  - boss-gate unlock still exposes the unlocked node structurally
- Existing bootstrap/startup/post-run flow coverage still proves:
  - boss defeat unlocks `Cavern Gate`
  - the post-run summary shows the readable boss-gate line
  - the unlocked target is available on return to the world map

## Verification
- `tools/run_editmode_tests.ps1` detached again without producing artifacts in this shell.
- Verification then used the direct waited Unity batch invocation.
- Result: `418 passed`, `0 failed`
- Artifacts:
  - `Logs/m059a_editmode_results.xml`
  - `Logs/m059a_editmode.log`

## Out Of Scope
- Milestone 060 boss reward differentiation
- broader boss reward bundles
- boss-specific ability systems
- broader boss clear-state redesign
