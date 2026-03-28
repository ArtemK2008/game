# Milestone 094 - Implement Combat Effect Readability Under Autobattle

## Goal
- Make important combat moments easier to read in the current autobattle shell without adding clutter or a broad VFX framework.
- Keep the implementation small, explicit, and limited to the current live combat shell.

## Delivered
- Added one small runtime-safe combat-effect seam:
  - `CombatEffectSpriteRegistry`
  - `CombatEffectPresentationStateResolver`
- Added the runtime-safe registry asset:
  - `Assets/Resources/CombatEffectSpriteRegistry.asset`
- Extended `CombatShellPresentationStateResolver` so the combat shell presentation now resolves:
  - current combat background
  - player and enemy combat-state sprites
  - current readable combat effect overlays
- Updated `CombatShellView` to render three fixed overlay slots inside the existing shell:
  - `PlayerCombatEffectArt`
  - `EnemyCombatEffectArt`
  - `CombatShellCenterEffectArt`
- Updated the live node/combat flow so effect-policy resolution stays outside the screen and view:
  - `NodePlaceholderScreen` captures combat snapshots and passes the resolved effect state through the existing presentation path

## Asset Usage
- Used the prepared canonical combat VFX sheets already present in the repo:
  - `Assets/Art/VFX/Combat/basic_combat_cues_sheet.png`
  - `Assets/Art/VFX/Combat/burst_strike_effect_sheet.png`
  - `Assets/Art/VFX/Combat/danger_pulse_sheet.png`
  - `Assets/Art/VFX/Combat/defeat_effect_sheet.png`
- No new art was requested.
- No VFX sheet splitting was needed for this milestone; the current sheets remain the canonical gameplay-facing assets.

## Behavior Change
- Ordinary same-tick damage now shows one restrained target-side impact cue instead of stacking multiple overlapping baseline effects.
- `Burst Strike` now reads as a distinct centered combat cue instead of using the ordinary impact treatment.
- Low-health danger now has one visible threshold-cross pulse cue without adding looping clutter.
- Defeat now has one short side-specific defeat cue.
- These cues stay subordinate to the existing shell:
  - no animation controller
  - no generalized VFX framework
  - no combat balance or reward changes

## SRP Boundary
- `CombatEffectSpriteRegistry` owns authored runtime-safe effect sprite references only.
- `CombatEffectPresentationStateResolver` owns readable combat-effect policy and maps the current tick into a compact overlay state.
- `CombatShellPresentationStateResolver` composes the overall combat shell presentation state.
- `CombatShellView` only renders the resolved effect sprites.
- `NodePlaceholderScreen` remains a requester/composer and does not own effect policy.

## Tests
- Added `Assets/Tests/EditMode/Combat/CombatEffectSpriteRegistryTests.cs`
  - verifies the runtime-safe registry resolves all milestone-094 combat effect sprites
- Added `Assets/Tests/EditMode/Combat/CombatEffectPresentationStateResolverTests.cs`
  - verifies baseline impact readability
  - verifies `Burst Strike` uses the centered cue instead of the ordinary impact cue
  - verifies low-health danger remains threshold-cross only
  - verifies defeat takes precedence over center special cues
- Updated `Assets/Tests/EditMode/World/NodePlaceholderScreenCombatUiTests.cs`
  - verifies the live combat shell surfaces effect cues during automatic combat
  - verifies the live `Burst Strike` flow surfaces the centered effect cue

## Out Of Scope
- Milestone `095` and later
- Animation controllers, timelines, particle systems, shader systems, or a broader VFX pipeline
- Any change to combat balance, save/resume, rewards, progression, audio behavior, or world/town flow
- Portrait/world-icon hookup or broader visual-polish systems

## Verification
- Compile/import first:
  - `powershell -NoLogo -NoProfile -File "tools/unity_compile_check.ps1"`
  - result: success
  - log: `C:\IT_related\myGame\Survivalon\Logs\unity_compile_check.log`
- EditMode helper:
  - `powershell -NoLogo -NoProfile -File "tools/unity_editmode_verify.ps1"`
  - result: the known helper artifact issue reproduced and did not create `C:\IT_related\myGame\Survivalon\Logs\editmode_results.xml`
- Direct Unity batch fallback used for final verification:
  - `-batchmode`
  - `-nographics`
  - `-projectPath C:\IT_related\myGame\Survivalon`
  - `-runTests`
  - `-runSynchronously`
  - `-testPlatform EditMode`
  - `-testResults C:\IT_related\myGame\Survivalon\Logs\m094_editmode_results.xml`
  - `-logFile C:\IT_related\myGame\Survivalon\Logs\m094_editmode.log`
  - final result:
    - `605 passed`
    - `0 failed`
    - `0 inconclusive`
    - `0 skipped`
  - artifacts:
    - `C:\IT_related\myGame\Survivalon\Logs\m094_editmode_results.xml`
    - `C:\IT_related\myGame\Survivalon\Logs\m094_editmode.log`
