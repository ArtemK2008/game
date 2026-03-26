# Milestone 078 - Implement Low-Friction Replay Flow

## Goal
- Reduce friction for repeated useful node runs in the current shipped prototype.
- Keep replay/repeat behavior aligned with the current post-run flow, world-map flow, and world-map-only safe-resume model.

## Delivered
- Reused the existing world-map entry button as a low-friction current-context repeat action when no new node is selected.
- The world map now resolves one quick-repeat target from the current safe world context:
  - combat and boss nodes show `Replay <node>`
  - service nodes show `Return to <node>`
- Selecting a different reachable node still overrides that shortcut and keeps the normal `Enter <selected node>` behavior.
- Tightened node-entry flow so re-entering the current safe-context node is allowed for this explicit repeat path without loosening the normal selectable-node list rules.

## Behavior Change
- Repeating the same useful node is now lower-friction from the world map.
- After returning from a resolved combat run, the player can immediately replay that current node from the world map without reselecting it first.
- After returning from the current service node, the player can immediately return to that service context from the world map without reselecting it first.
- Post-run replay, resolved-post-run autosave, world-map safe resume, and temporary run-only state behavior remain unchanged.

## Tests
- Updated `Assets/Tests/EditMode/World/WorldMapScreenControllerTests.cs`
  - quick-repeat target resolves from the current world context
- Updated `Assets/Tests/EditMode/World/WorldMapScreenPresentationTests.cs`
  - entry-button wording distinguishes normal entry, combat replay, and service return
- Updated `Assets/Tests/EditMode/World/WorldMapScreenUiSetupTests.cs`
  - world map exposes an interactable quick-repeat button with no new selection
  - quick-repeat path invokes the entry callback for the current context node
- Updated `Assets/Tests/EditMode/World/WorldNodeEntryFlowControllerTests.cs`
  - current-context node can be re-entered for the low-friction repeat path
- Updated `Assets/Tests/EditMode/Startup/BootstrapStartupCombatScreenFlowTests.cs`
  - returning from combat shows `Replay Forest Farm`
  - quick replay from the world map works without reselecting the node
  - restart/load after resolved post-run autosave still restores to the world map and exposes the same quick replay action
- Updated `Assets/Tests/EditMode/Startup/BootstrapStartupScreenFlowTests.cs`
  - returning from town/service shows `Return to Cavern Service Hub`
  - quick return from the world map re-enters that service context without reselecting the node

## Out Of Scope
- Auto-repeat or unattended repeat loops
- New automation systems
- Mid-run save/resume
- Offline rewards or offline farming
- World-map redesign
- Broader town/service navigation changes
- New content, new currencies, or new combat systems

## Verification
- Compile/import first:
  - `powershell -NoLogo -NoProfile -File "tools/unity_compile_check.ps1"`
  - result: success
  - log: `C:\IT_related\myGame\Survivalon\Logs\unity_compile_check.log`
- EditMode helper verification:
  - `powershell -NoLogo -NoProfile -File "tools/unity_editmode_verify.ps1"`
  - compile/import passed again, but the known helper artifact issue reproduced and `Logs/editmode_results.xml` was not created
- Fallback if the known helper artifact issue reproduces:
  - `powershell -NoLogo -NoProfile -File "tools/run_editmode_tests.ps1" -ResultsPath "C:\IT_related\myGame\Survivalon\Logs\m078_editmode_results.xml" -LogPath "C:\IT_related\myGame\Survivalon\Logs\m078_editmode.log"`
- Final result:
  - `518 passed`
  - `0 failed`
  - `0 inconclusive`
  - `0 skipped`
- Artifacts:
  - `C:\IT_related\myGame\Survivalon\Logs\unity_compile_check.log`
  - `C:\IT_related\myGame\Survivalon\Logs\m078_editmode_results.xml`
  - `C:\IT_related\myGame\Survivalon\Logs\m078_editmode.log`
