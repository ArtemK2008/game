# Milestone 034 - Implement Basic Soft Currency

## Delivered
- Added one live soft-currency reward path using `ResourceCategory.SoftCurrency`.
- Successful combat-compatible runs now resolve with a structured reward payload containing a small soft-currency reward.
- Granted run rewards are now applied into persistent resource balances during run resolution, so replay/return/stop all work from the updated in-memory persistent state.
- The existing safe persistence boundary now saves the updated soft-currency balance through the normal resolved-world-context save flow.
- Post-run summary text now shows the actual aggregated reward summary instead of a generic placeholder string.

## Tests
- Added `RunRewardResolutionServiceTests` to verify the soft-currency reward rule for successful combat runs and empty payload behavior for failed/non-combat runs.
- Added `RunRewardGrantServiceTests` to verify reward payload application into persistent balances.
- Updated combat lifecycle, startup flow, persistence, placeholder summary, and resource-balance tests to verify:
  - successful combat runs carry soft currency
  - soft currency is applied to persistent balances
  - saved game-state snapshots preserve the updated balance
  - soft-currency spend succeeds when affordable and fails cleanly when overspent

## Out Of Scope
- Region/material reward granting
- Broad reward-balance tuning
- Soft-currency spending UI
- Town/service sinks and upgrade-board spending
- Material persistence and spending
- Milestone reward differentiation
- Gear loot

## Verification
- Unity EditMode batch run passed:
  - `Logs/m034_editmode_results.xml`
  - `Logs/m034_editmode.log`
