# Milestone 037 - Differentiate Ordinary vs Milestone Rewards

## Delivered
- Kept the existing ordinary reward flow intact:
  - successful combat runs still grant soft currency
  - successful standard combat runs in region-material regions still grant region material
- Added a distinct milestone reward path for a real milestone moment already present in the build:
  - when a tracked node reaches its clear threshold, the run now also grants `PersistentProgressionMaterial x1`
- Kept the distinction explicit in the existing reward pipeline by extending `RunRewardPayload` with separate milestone reward buckets instead of hiding milestone meaning in UI-only string logic.
- Applied milestone rewards through the same persistent resource-balance flow used by ordinary rewards, so milestone rewards are saved through the normal resolved world-context boundary.
- Updated the post-run summary to keep the output compact and readable:
  - ordinary rewards stay on the main `Rewards gained` line
  - milestone rewards appear on a separate `Milestone rewards` line only when present

## Tests
- Updated reward-payload tests to cover milestone reward buckets and combined ordinary-plus-milestone payloads.
- Updated reward-resolution tests to verify milestone rewards are granted only when a tracked node actually reaches clear threshold.
- Updated reward-grant tests to verify milestone rewards apply into persistent balances.
- Updated run-lifecycle progression tests to verify a real clear-threshold run grants persistent progression material and persists it into resource balances.
- Updated post-run summary tests and world-flow tests to verify milestone rewards appear distinctly in the summary while ordinary runs remain unchanged.

## Out Of Scope
- Boss-specific or map-depth-specific milestone reward tiers
- Broader milestone reward bundles
- Spending or sink behavior for persistent progression material
- Reward rarity, gear loot, or broader economy redesign

## Verification
- Unity EditMode batch run passed for the milestone implementation.
