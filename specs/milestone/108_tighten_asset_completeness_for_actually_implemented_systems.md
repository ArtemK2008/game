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
  - Character portraits remain prepared-only because the current shipped build still has no portrait-specific runtime surface.
  - Broader location-/service-specific music is still unavailable in-repo, but the shipped runtime intentionally only exposes the current calm/gameplay music split and safely falls back to those two loops.
  - No shipped system currently uses animation controllers or clip-driven character/enemy animation assets; the live combat shell still intentionally uses state-sprite switching.

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
  - result: see final milestone report for whether the known helper artifact reproduced and which fallback artifacts were used
- This milestone was asset-completeness hardening for already shipped systems, not feature expansion.
