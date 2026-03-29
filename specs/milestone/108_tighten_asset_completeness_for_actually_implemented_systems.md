# Milestone 108 - Tighten Asset Completeness For Actually Implemented Systems

## Goal
- Audit only the currently shipped systems for runtime-facing sprite, animation, and music completeness.
- Integrate already available missing assets where the repo honestly contains them.
- Document the remaining honest limits without inventing new runtime systems or placeholder content.

## Delivered
- Audited the current shipped systems that already expose runtime-facing assets:
  - startup / main menu / settings / offline-return flow
  - world map and its live build-preparation surface
  - town / service shell
  - combat shell
  - post-run / UI-system / music seams
- Added one runtime-safe playable-character world-icon seam:
  - `Assets/Scripts/Characters/PlayableCharacterWorldIconRegistry.cs`
  - `Assets/Resources/PlayableCharacterWorldIconRegistry.asset`
- Wired the already-prepared `Vanguard` and `Striker` world icons into the live world-map build-preparation character-selection buttons.
- Updated the art index and build snapshot so they describe the true shipped hookup status after this audit.

### What Was Integrated Vs What Remains Requested
- Integrated:
  - `Assets/Art/Characters/Vanguard/WorldIcon/world_icon.png`
  - `Assets/Art/Characters/Striker/WorldIcon/world_icon.png`
  - runtime hookup through the world-map build-preparation character-selection surface only
- No new external asset request was opened in this milestone.
  - The audit did not find any repo-absent sprite, animation, or music asset that could be honestly requested for a currently shipped system without also implying a broader runtime expansion.

### Remaining Honest Non-Integrated Asset Limits
- Playable-character portraits
  - not integrated now because the current shipped build still has no portrait-specific runtime surface to receive them cleanly
  - current runtime falls back safely by using text-only selected-character summaries plus the newly wired world icons in world-map build preparation
  - canonical future asset location if later needed: `Assets/Art/Characters/<CharacterName>/Portrait/portrait.png`
- Broader location-/service-specific music
  - not integrated now because the repo currently contains only the shipped calm/gameplay loop pair and the runtime intentionally exposes only that two-context music split
  - current runtime falls back safely by continuing to use `music_calm_loop.wav` for safe/planning contexts and `music_gameplay_loop.wav` for active combat
  - canonical future asset location if later needed: `Assets/Audio/Music/` using the current loop-based music asset root
- Clip-driven character/enemy animation assets
  - not integrated now because no shipped system currently uses animation controllers or clip-driven runtime animation; the live combat shell still intentionally relies on state-sprite switching
  - current runtime falls back safely by continuing to resolve `idle` / `attack` / `hit` / `defeat` state sprites from the existing sprite registries
  - canonical future asset location if later needed: `Assets/Art/Characters/<CharacterName>/Sprites/` and `Assets/Art/Enemies/<EnemyName>/Sprites/` alongside the current authored combat-state sprite families

## Behavior Change
- The live world-map build-preparation character-selection buttons now render authored world icons for the current playable `Vanguard` / `Striker` roster instead of remaining text-only.
- Gameplay rules, progression, world routing, combat, save/resume, music-context logic, and town/service behavior remain unchanged.

## Tests
- Added `Assets/Tests/EditMode/Characters/PlayableCharacterWorldIconRegistryTests.cs`
  - verifies the runtime-safe world-icon registry resolves the live `Vanguard` / `Striker` assets
  - verifies the resolver fails closed for unknown character ids
- Updated `Assets/Tests/EditMode/World/WorldMapScreenUiSetupTests.cs`
  - verifies the live world-map character-selection buttons render non-raycast world-icon images for the current playable roster

## Out Of Scope
- Portrait hookup or new portrait-specific UI surfaces
- New music contexts, per-location music routing, service-only themes, boss themes, playlists, or adaptive music logic
- Animation-controller hookup, new animation systems, or changing the current combat-state sprite-switching model
- Any gameplay/system expansion beyond the shipped runtime-facing asset hookup audited here

## Verification
- Compile/import:
  - `powershell -NoLogo -NoProfile -File "tools/unity_compile_check.ps1"`
  - result: success
  - log: `C:\IT_related\myGame\Survivalon\Logs\unity_compile_check.log`
- EditMode helper:
  - `powershell -NoLogo -NoProfile -File "tools/unity_editmode_verify.ps1"`
  - result: the known helper artifact issue reproduced and did not create `C:\IT_related\myGame\Survivalon\Logs\editmode_results.xml`
- Direct Unity batch fallback:
  - `C:\Program Files\Unity\Hub\Editor\6000.3.10f1\Editor\Unity.exe -batchmode -nographics -projectPath C:\IT_related\myGame\Survivalon -runTests -runSynchronously -testPlatform EditMode -testResults C:\IT_related\myGame\Survivalon\Logs\m108_editmode_results.xml -logFile C:\IT_related\myGame\Survivalon\Logs\m108_editmode.log`
  - result: `695 passed`, `0 failed`, `0 inconclusive`, `0 skipped`
  - requested project-local artifacts:
    - `C:\IT_related\myGame\Survivalon\Logs\m108_editmode_results.xml`
    - `C:\IT_related\myGame\Survivalon\Logs\m108_editmode.log`
  - Unity also wrote its default XML artifact:
    - `C:\Users\Happy\AppData\LocalLow\DefaultCompany\Survivalon\TestResults.xml`
- This milestone was asset-completeness hardening for already shipped systems, not feature expansion.
