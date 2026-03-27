# Milestone 084 - Implement Milestone Reward Presentation Polish

## Goal
- Make post-run reward spikes feel more clearly distinct without changing reward amounts, grant timing, progression behavior, or save/resume flow.
- Keep the change small and limited to the existing post-run result presentation seam.

## Delivered
- Added one small presentation-oriented resolver/model pair:
  - `PostRunResultPresentationState`
  - `PostRunResultPresentationStateResolver`
- Moved post-run reward/result grouping out of ad hoc string assembly and into that focused presentation seam.
- Updated `PostRunSummaryTextBuilder` so the existing post-run summary now renders explicit compact lines for:
  - ordinary rewards
  - clear-threshold spike rewards
  - boss spike rewards
  - boss gear rewards
  - unlock outcomes
- Kept `NodePlaceholderScreen` unchanged as a UI renderer of the already-built summary string.

## Behavior Change
- Player-facing post-run presentation is clearer, but underlying gameplay behavior is unchanged.
- Reward amounts, reward sources, boss gear ownership, route unlock rules, autosave behavior, safe resume behavior, replay flow, and combat logic all stay the same.
- The main visible polish change is that boss spikes, clear-threshold spikes, boss gear spikes, and unlock outcomes no longer read like one blended reward block or one overloaded progress line.
- Route unlock state now appears as an explicit `Unlock outcomes:` line instead of being embedded as `route unlock Yes/No` inside `Progress changes:`.

## Tests
- Added `Assets/Tests/EditMode/Run/PostRunResultPresentationStateResolverTests.cs`
  - verifies ordinary-only presentation state stays clean
  - verifies combined ordinary + clear spike + boss spike + boss gear + unlock presentation stays explicitly separated
- Updated `Assets/Tests/EditMode/Run/PostRunSummaryTextBuilderTests.cs`
  - verifies the new labels and separation across ordinary-only, clear-threshold, boss, boss-gear, and combined cases
- Updated `Assets/Tests/EditMode/World/NodePlaceholderScreenCombatUiTests.cs`
  - verifies post-run combat UI still shows the new compact progress text cleanly
- Updated `Assets/Tests/EditMode/Startup/BootstrapStartupCombatScreenFlowTests.cs`
  - verifies ordinary combat post-run presentation still reads cleanly in the shipped startup flow
- Updated `Assets/Tests/EditMode/Startup/BootstrapStartupProgressionScreenFlowTests.cs`
  - verifies clear-threshold, boss, boss-gear, and unlock spike presentation in the shipped world-map progression flow

## Out Of Scope
- Any new reward type, currency, or progression rule
- Any change to reward grant timing or autosave timing
- Any new loot screen, modal, or inventory presentation
- Any animation system or broader post-run UI redesign

## Verification
- Compile/import first:
  - `powershell -NoLogo -NoProfile -File "tools/unity_compile_check.ps1"`
  - result: success
  - log: `C:\IT_related\myGame\Survivalon\Logs\unity_compile_check.log`
- EditMode helper:
  - `powershell -NoLogo -NoProfile -File "tools/unity_editmode_verify.ps1"`
  - result: the known helper artifact issue reproduced and did not create `C:\IT_related\myGame\Survivalon\Logs\editmode_results.xml`
- Direct waited fallback:
  - `powershell -NoLogo -NoProfile -File "tools/run_editmode_tests.ps1" -ResultsPath "C:\IT_related\myGame\Survivalon\Logs\m084_editmode_results.xml" -LogPath "C:\IT_related\myGame\Survivalon\Logs\m084_editmode.log"`
  - result: `550 passed`, `0 failed`, `0 inconclusive`, `0 skipped`
  - artifacts:
    - `C:\IT_related\myGame\Survivalon\Logs\m084_editmode_results.xml`
    - `C:\IT_related\myGame\Survivalon\Logs\m084_editmode.log`
