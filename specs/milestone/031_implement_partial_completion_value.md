# Milestone 031 - Implement Partial Completion Value

## Spec Result
- The specs allow partial completion value structurally:
  - failed or incomplete runs may still matter if they produce persistent map progress, rewards, or account growth
  - standard combat-map progress remains kill-driven
- In the current implemented MVP, combat is still deterministic 1v1 with one hostile enemy per run.
- Because node progress is kill-driven and the single enemy kill also resolves the run as success, the current MVP does **not** expose a failed or incomplete combat case that can grant partial node progress without inventing broader combat or reward systems.

## Delivered
- Kept the existing progress rule unchanged:
  - successful combat runs that defeat the enemy still grant `+1` node progress
  - failed combat runs still grant `0` node progress
  - clear-at-threshold and next-node unlock behavior remain unchanged
- Treated this milestone as a verification/tightening milestone instead of inventing unsupported partial-value behavior.
- Updated the current build snapshot to state the real MVP boundary explicitly.

## Tests
- Updated `RunLifecycleControllerTests` to verify:
  - failed tracked combat runs stay at `0 / 3` progress
  - failed tracked combat runs do not unlock routes
  - repeating failed tracked combat runs does not create phantom progress drift
- Updated `NodePlaceholderScreenUiTests` to verify failed hostile boss post-run summary still shows:
  - `Resolution: Failed`
  - `Node progress total: 0 / 3`
  - `Node progress delta: 0`
  - `Route unlock changed: No`
- Existing successful-progress, clear-threshold, and next-node unlock tests remain the proof that the success path still works exactly as before.

## Intentionally Deferred
- Multi-enemy combat cases where a failed run could still have meaningful kill output
- Reward-based partial completion value
- Early-exit partial completion rules
- Broader progression/reward redesign

## Verification
- Unity EditMode batch run passed:
  - `Logs/m031_editmode_results.xml`
  - `Logs/m031_editmode.log`
