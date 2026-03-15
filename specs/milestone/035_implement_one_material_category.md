# Milestone 035 - Implement One Material Category

## Delivered
- Added one live material reward path using `ResourceCategory.RegionMaterial`.
- Successful standard combat runs in regions whose resource identity is `RegionMaterial` now resolve with a structured reward payload that includes a small region-material reward alongside the existing soft-currency reward.
- The current bootstrap world now uses that rule to make ordinary forest combat/farm nodes produce repeatable region-linked material value without changing service-node or broader milestone reward behavior.
- Granted region material is applied into persistent resource balances during run resolution and is saved through the existing resolved world-context persistence boundary.
- Post-run summary text now shows the aggregated material reward when it is present.

## Tests
- Updated `RunRewardResolutionServiceTests` to verify:
  - region combat nodes grant region material
  - boss/gate combat nodes do not grant the same material reward
  - failed and non-combat runs still keep their existing reward behavior
- Updated `RunRewardGrantServiceTests` to verify region material is applied into persistent balances.
- Updated `RunLifecycleControllerCombatTests` to verify the automatic combat loop now carries and grants the region-material reward during the real run-result flow.
- Updated `BootstrapStartupCombatScreenFlowTests` to verify the material reward appears in the post-run summary and persists after returning to the world map.
- Updated `NodePlaceholderScreenPresentationTests` to verify reward-summary formatting with both soft currency and region material.

## Intentionally Left Out
- Material spending/sinks
- Persistent progression material rewards
- Broader region-by-region material differentiation
- Boss/milestone reward differentiation
- Gear loot
- Reward-balance tuning beyond the small readable placeholder amounts

## Verification
- Unity EditMode batch run passed:
  - `Logs/m035_editmode_results.xml`
  - `Logs/m035_editmode.log`
