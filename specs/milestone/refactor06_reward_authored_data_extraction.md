# Refactor milestone 06 — reward authored-data extraction

## Scope
- extract shipped static run-reward tuning out of runtime reward logic
- keep reward eligibility, payload construction, and reward grant flow unchanged
- keep the refactor narrow and behavior-preserving

## What proved the boundary issue
- `Assets/Scripts/Run/RunRewardResolutionService.cs` contained shipped static reward values directly inside runtime logic:
  - ordinary soft-currency reward: `SoftCurrency x1`
  - ordinary region-material base reward: `RegionMaterial x1`
  - clear-threshold milestone reward: `PersistentProgressionMaterial x1`
  - baseline boss reward: `PersistentProgressionMaterial x2`
- those values are authored/static tuning, not runtime resolution policy

## Refactor
- added `Assets/Scripts/Data/Rewards/RewardAmountDefinition.cs`
- added `Assets/Scripts/Data/Rewards/RunRewardTuningDefinition.cs`
- added `Assets/Scripts/Data/Rewards/RunRewardTuningCatalog.cs`
- updated `Assets/Scripts/Run/RunRewardResolutionService.cs` to read shipped static reward values from `RunRewardTuningCatalog`
- added `Assets/Tests/EditMode/Data/Rewards/RunRewardTuningCatalogTests.cs`

## Resulting ownership split
- `Assets/Scripts/Data/Rewards/`
  - owns shipped static run-reward tuning values
- `Assets/Scripts/Run/`
  - owns reward eligibility checks
  - owns runtime payload construction
  - owns runtime reward application flow
- node-specific and boss-specific reward modifiers still stay with world content where they belong

## Behavior check
- reward values stayed unchanged
- ordinary region material and boss bonuses still layer exactly as before
- no persistence, UI, or gameplay flow changed
