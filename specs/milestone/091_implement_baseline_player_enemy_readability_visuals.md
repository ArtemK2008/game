# Milestone 091 - Implement Baseline Player/Enemy Readability Visuals

## Goal
- Make the current combat shell visually readable at a glance by using the prepared canonical player-side and enemy-side combat sprites.
- Keep the implementation small, state-based, and subordinate to the current autobattle prototype.

## Delivered
- Added one small runtime-safe combat sprite seam:
  - `CombatEntitySpriteRegistry`
  - `CombatShellVisualStateResolver`
  - `CombatShellPresentationStateResolver`
- Added the runtime-safe registry asset:
  - `Assets/Resources/CombatEntitySpriteRegistry.asset`
- Updated the live combat shell to render separate player-side and enemy-side sprites instead of text-only entity cards.
- Wired the current shipped entity ids to the prepared canonical combat-state sprites:
  - `player_main` -> `Assets/Art/Characters/Vanguard/Sprites/`
  - `player_striker` -> `Assets/Art/Characters/Striker/Sprites/`
  - `enemy_001` -> `Assets/Art/Enemies/EnemyUnit/Sprites/`
  - `enemy_002` -> `Assets/Art/Enemies/BulwarkRaider/Sprites/`
  - `boss_001` -> `Assets/Art/Enemies/GateBoss/Sprites/`
- Kept sprite-state switching intentionally small and readable:
  - `idle`
  - `attack`
  - `hit`
  - `defeat`
- Updated the art index so later visual milestones can see which prepared asset families are already hooked up and which remain prep-only.

## Behavior Change
- Live combat now shows clearly distinct player-side and enemy-side entity visuals instead of text-only entity blocks.
- The current autobattle shell now switches between `idle`, `attack`, `hit`, and `defeat` sprites for the active player and enemy entity.
- Current shipped player-side hookup covers the playable combat characters already used in runtime:
  - `Vanguard`
  - `Striker`
- Current shipped enemy-side hookup covers the current live enemy families:
  - `Enemy Unit`
  - `Bulwark Raider`
  - `Gate Boss`
- This milestone changes combat readability only. It does not add backgrounds, VFX hookup, animation controllers, or broader visual systems.

## Tests
- Added `Assets/Tests/EditMode/Combat/CombatEntitySpriteRegistryTests.cs`
  - verifies the runtime-safe sprite registry resolves all current player and enemy combat-state sprite sets
- Added `Assets/Tests/EditMode/Combat/CombatShellVisualStateResolverTests.cs`
  - verifies combat transitions resolve readable `idle` / `attack` / `hit` / `defeat` visual states
- Updated `Assets/Tests/EditMode/World/NodePlaceholderScreenCombatUiTests.cs`
  - verifies live combat UI shows distinct non-null player and enemy sprites
  - verifies current character selection changes the player-side sprite set
- Updated `Assets/Tests/EditMode/Startup/BootstrapStartupCombatScreenFlowTests.cs`
  - verifies the bootstrap combat flow renders the expected initial combat-state sprites

## Out Of Scope
- Milestones `092`, `093`, and `094`
- Location backgrounds, service/town visuals, combat VFX hookup, or broader scene polish
- Animation controllers, frame-by-frame animation systems, or a broader visual-manager framework
- Any change to combat balance, rewards, save/resume, replay, progression, or audio behavior

## Verification
- Compile/import first:
  - `powershell -NoLogo -NoProfile -File "tools/unity_compile_check.ps1"`
  - result: success
  - log: `C:\IT_related\myGame\Survivalon\Logs\unity_compile_check.log`
- EditMode helper:
  - `powershell -NoLogo -NoProfile -File "tools/unity_editmode_verify.ps1"`
  - result: the known helper artifact issue reproduced and did not create `C:\IT_related\myGame\Survivalon\Logs\editmode_results.xml`
- Direct waited fallback used for final verification:
  - inline PowerShell `Start-Process` invocation of Unity EditMode batch run with:
    - `-batchmode`
    - `-nographics`
    - `-projectPath C:\IT_related\myGame\Survivalon`
    - `-runTests`
    - `-runSynchronously`
    - `-testPlatform EditMode`
    - `-testResults C:\IT_related\myGame\Survivalon\Logs\m091_editmode_results.xml`
    - `-logFile C:\IT_related\myGame\Survivalon\Logs\m091_editmode.log`
  - final result:
    - `592 passed`
    - `0 failed`
    - `0 inconclusive`
    - `0 skipped`
  - artifacts:
    - `C:\IT_related\myGame\Survivalon\Logs\m091_editmode_results.xml`
    - `C:\IT_related\myGame\Survivalon\Logs\m091_editmode.log`
