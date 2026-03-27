# Milestone 085 - Expand Boss Readability And Structure

## Goal
- Make the shipped boss encounter flow read more clearly than ordinary combat without adding new boss mechanics or a broader combat subsystem.
- Keep the change small and centered on the existing node/combat presentation seam.

## Delivered
- Added one compact boss-presentation seam:
  - `BossEncounterPresentationState`
  - `BossEncounterPresentationStateResolver`
- The new resolver derives boss-readable encounter role and encounter stakes from already-shipped boss node data:
  - current boss role tag
  - current gate-clear stake when present
  - current boss reward stake when present
  - current boss gear reward stake when present
- Updated the existing compact boss node header to show:
  - `Encounter: Gate boss`
  - `Boss stakes: ...`
- Updated the existing run HUD / combat shell text to show:
  - `Boss encounter | <location> | <node>` as the context title
  - `Boss role: Gate boss | Stakes: ...` in the active/resolved combat summary
- Kept the change on the presentation side only; reward, unlock, save, replay, and combat-resolution logic remain unchanged.

## Behavior Change
- Boss encounters are now more visibly distinct from ordinary combat in the live node/combat flow.
- Ordinary combat presentation stays the same.
- Bosses now read as explicit boss encounters with explicit stakes instead of looking like ordinary combat plus a stronger enemy name only.
- Save/resume, autosave, reward grants, boss unlock flow, gear reward ownership, replay flow, and combat logic all stay the same.

## Tests
- Added `Assets/Tests/EditMode/World/BossEncounterPresentationStateResolverTests.cs`
  - verifies ordinary combat does not produce boss presentation state
  - verifies the forest gate boss resolves `Gate boss` plus `Gate clear, Boss rewards, Gear reward`
  - verifies the cavern gate boss resolves a reward-focused boss presentation without a gate-clear stake
- Updated `Assets/Tests/EditMode/Run/RunHudPresentationTests.cs`
  - verifies boss run HUD title becomes `Boss encounter | ...`
  - verifies boss role/stakes line appears in the compact combat HUD summary
- Updated `Assets/Tests/EditMode/World/NodePlaceholderScreenCombatUiTests.cs`
  - verifies the live boss node screen shows the boss encounter label and stakes before auto-resolved failure/post-run
- Updated `Assets/Tests/EditMode/Startup/BootstrapStartupProgressionScreenFlowTests.cs`
  - verifies the shipped forest gate boss entry flow exposes the clearer boss presentation before resolving into the existing boss reward/unlock result

## Out Of Scope
- Any new boss mechanics, phases, support adds, or AI patterns
- Any new boss reward type, currency, or broader loot redesign
- Any combat rebalance or persistence change
- Any new modal, dedicated boss screen, or broader UI redesign

## Verification
- Compile/import first:
  - `powershell -NoLogo -NoProfile -File "tools/unity_compile_check.ps1"`
  - result: success
  - log: `C:\IT_related\myGame\Survivalon\Logs\unity_compile_check.log`
- EditMode helper:
  - `powershell -NoLogo -NoProfile -File "tools/unity_editmode_verify.ps1"`
  - result: the known helper artifact issue reproduced and did not create `C:\IT_related\myGame\Survivalon\Logs\editmode_results.xml`
- Direct waited fallback:
  - inline PowerShell `Start-Process` invocation of Unity EditMode batch run with:
    - `-batchmode`
    - `-nographics`
    - `-projectPath C:\IT_related\myGame\Survivalon`
    - `-runTests`
    - `-runSynchronously`
    - `-testPlatform EditMode`
    - `-testResults C:\IT_related\myGame\Survivalon\Logs\m085_editmode_results.xml`
    - `-logFile C:\IT_related\myGame\Survivalon\Logs\m085_editmode.log`
  - result: `553 passed`, `0 failed`, `0 inconclusive`, `0 skipped`
  - artifacts:
    - `C:\IT_related\myGame\Survivalon\Logs\m085_editmode_results.xml`
    - `C:\IT_related\myGame\Survivalon\Logs\m085_editmode.log`
