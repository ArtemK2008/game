# Milestone 060 - Add Boss Reward Differentiation

## Decision
- The smallest honest 060 step is to keep the current reward pipeline and extend it with one separate boss-reward bucket.
- Successful boss defeat should feel more important than an ordinary clear without introducing a broader loot redesign or a new currency.

## Delivered
- Extended `RunRewardPayload` with separate boss reward collections:
  - `BossCurrencyRewards`
  - `BossMaterialRewards`
- `RunRewardResolutionService` now grants one small boss-only reward bundle on successful boss defeat:
  - `Persistent progression material x2`
- Ordinary combat rewards and clear-threshold milestone rewards remain structurally separate and unchanged.
- `RunRewardGrantService` now applies boss reward entries into persistent balances.
- `PostRunSummaryTextBuilder` now shows a compact separate line for boss rewards:
  - `Boss rewards: Persistent progression material x2`

## Behavior Change
- Successful standard combat still grants the current ordinary reward baseline:
  - `Soft currency x1`
  - region material when the node qualifies for it
- Clear-threshold milestone rewards still grant the existing milestone reward:
  - `Persistent progression material x1`
- Successful boss defeat now grants a more important reward moment than ordinary clears:
  - ordinary reward line still shows the baseline combat reward
  - boss reward line now adds `Persistent progression material x2`
- The current forest boss flow still unlocks `Cavern Gate` separately from reward handling.

## SRP Notes
- Boss reward policy remains in `RunRewardResolutionService`, not in UI or combat classes.
- Reward payload structure remains the cross-run contract for granted rewards.
- `RunRewardGrantService` still only applies already-resolved reward entries to persistent balances.
- `PostRunSummaryTextBuilder` only formats the structured reward data for display.

## Tests
- Updated reward-resolution tests to prove:
  - successful boss defeat grants differentiated boss rewards
  - ordinary combat does not receive boss reward differentiation
  - ordinary milestone reward behavior is unchanged
- Updated reward-payload and reward-grant tests to cover the new boss reward bucket directly.
- Updated combat/run flow tests to prove a successful boss run grants the differentiated reward into persistent balances.
- Updated startup/post-run flow coverage to prove the boss reward line is visible in the shipped placeholder loop.

## Verification
- `tools/run_editmode_tests.ps1` detached again without producing artifacts in this shell.
- Verification then used the direct waited Unity batch invocation.
- Result: `424 passed`, `0 failed`
- Artifacts:
  - `Logs/m060_editmode_results.xml`
  - `Logs/m060_editmode.log`

## Out Of Scope
- Milestone 061 or later loot expansion work
- new currencies
- boss-specific loot tables
- broader first-clear/replay reward differentiation
- boss ability or AI changes
