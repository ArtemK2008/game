# Milestone 036 - Implement Post-Run Reward Summary UI

## Delivered
- Tightened the existing post-run summary into a more clearly grouped aggregated result panel.
- Post-run summary text now explicitly groups:
  - rewards gained this run
  - progress changes from this run
  - next available actions
- Reward presentation remains aggregated and readable, showing compact combined reward output instead of a noisy breakdown.
- Progress presentation now stays compact while still surfacing:
  - node progress delta
  - tracked node progress total when applicable
  - persistent progression delta
  - route unlock result
- Extracted post-run summary formatting into `PostRunSummaryTextBuilder` so post-run result presentation has one clear owner instead of remaining mixed into the broader node-screen text builder.

## Tests
- Added `PostRunSummaryTextBuilderTests` to verify:
  - aggregated reward/progress formatting for tracked rewarded runs
  - readable fallback output for empty rewards and untracked node progress
  - null guard behavior
- Updated `NodePlaceholderScreenPresentationTests` to verify the node screen still surfaces the grouped reward/progress summary through its existing UI text builder path.
- Updated `BootstrapStartupCombatScreenFlowTests` to verify the real post-run screen now shows the grouped aggregated reward/progress summary after a combat run.

## Intentionally Left Out
- Detailed inventory or reward-history screens
- Reward rarity or milestone differentiation
- Spending UI
- Gear loot presentation
- Expanded run analytics or combat breakdown panels

## Verification
- Unity EditMode batch run passed:
  - `Logs/m036_editmode_results.xml`
  - `Logs/m036_editmode.log`
