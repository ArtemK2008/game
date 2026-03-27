# Milestone 080 - Mark Or Detect Farm-Ready Content

## Goal
- Add one small honest rule for content that is suitable for low-friction farming in the current shipped prototype.
- Keep the rule derived from already-shipped content/state instead of inventing a broader authored farming taxonomy.

## Delivered
- Added one focused world-side rule seam:
  - `WorldNodeFarmReadinessResolver`
- The current shipped rule is:
  - ordinary `Combat` nodes become `Farm-ready` when their persistent node state is completed (`Cleared` or `Mastered`)
  - `BossOrGate` and `ServiceOrProgression` nodes are not marked farm-ready by this rule
- `WorldMapScreenController` now resolves that derived state into `WorldMapNodeOption`.
- `WorldMapScreenTextBuilder` now appends `Farm-ready` to the existing world-map node label when that derived state is true.

## Behavior Change
- The world map now visibly marks completed ordinary combat content as `Farm-ready`.
- This keeps push vs farm readability clearer in the existing node list without adding a new screen, save concept, or automation system.
- Replay flow, rewards, progression, persistence, safe resume, and autosave behavior remain unchanged.

## Tests
- Added `Assets/Tests/EditMode/World/WorldNodeFarmReadinessResolverTests.cs`
  - completed combat content is farm-ready
  - uncleared combat content is not farm-ready
  - boss/service content is not farm-ready
- Updated `Assets/Tests/EditMode/World/WorldMapScreenControllerTests.cs`
  - world-map node options now carry the derived farm-ready state
- Updated `Assets/Tests/EditMode/World/WorldMapScreenPresentationTests.cs`
  - node labels show `Farm-ready` only when the derived state is true
- Updated `Assets/Tests/EditMode/World/WorldMapScreenUiSetupTests.cs`
  - the live world-map screen surfaces the farm-ready marker for completed combat content

## Out Of Scope
- No authored farming-role taxonomy
- No automation system or auto-repeat loop
- No reward redesign
- No offline farming rewards or claim flow
- No save/resume or safe-context routing changes
- No new UI screen or menu

## Verification
- Compile/import first:
  - `powershell -NoLogo -NoProfile -File "tools/unity_compile_check.ps1"`
  - result: success
  - log: `C:\IT_related\myGame\Survivalon\Logs\unity_compile_check.log`
- EditMode helper verification:
  - `powershell -NoLogo -NoProfile -File "tools/unity_editmode_verify.ps1"`
  - result: reproduced the known helper artifact issue and did not create `Logs/editmode_results.xml`
- Direct waited fallback:
  - `powershell -NoLogo -NoProfile -File "tools/run_editmode_tests.ps1" -ResultsPath "C:\IT_related\myGame\Survivalon\Logs\m080_editmode_results.xml" -LogPath "C:\IT_related\myGame\Survivalon\Logs\m080_editmode.log"`
  - result: `531 passed`, `0 failed`, `0 inconclusive`, `0 skipped`
  - artifacts:
    - `C:\IT_related\myGame\Survivalon\Logs\m080_editmode_results.xml`
    - `C:\IT_related\myGame\Survivalon\Logs\m080_editmode.log`
