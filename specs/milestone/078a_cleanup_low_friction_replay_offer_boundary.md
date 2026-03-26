# Milestone 078a - Cleanup Low-Friction Replay Offer Boundary

## Goal
- Keep Milestone 078's low-friction replay/return idea.
- Restrict the world-map quick replay/return shortcut so it is offered only immediately after an explicit return to the world map from post-run or town/service flow.

## What Changed
- Moved quick replay/return availability behind one small session-only return-to-world reentry offer in `SessionContextState`.
- `BootstrapWorldContextTransitionService` now stamps that offer only for explicit `return to world` transitions.
- `WorldMapScreenController` now resolves a quick replay/return target only when that explicit return-to-world offer is present and still matches the current safe-context node.
- `WorldMapScreen` now consumes that temporary offer on node entry so it does not behave like a permanent always-on world-map shortcut.
- Ordinary startup/load and safe resume continue to open the world map without any implicit current-context replay/return shortcut.

## Behavior Change
- Quick replay/return is still available after:
  - post-run -> return to world
  - town/service -> return to world
- Quick replay/return is no longer shown on:
  - first world-map entry
  - ordinary startup/load into the world map
  - safe resume / restart load
- When that temporary shortcut is not offered, the entry button goes back to the normal `Select a reachable node to enter` behavior until the player selects a reachable node.

## Tests
- Updated `Assets/Tests/EditMode/State/SessionContextStateTests.cs`
  - return-to-world reentry offer is session-only and is consumed on node entry
- Updated `Assets/Tests/EditMode/Startup/BootstrapWorldContextTransitionServiceTests.cs`
  - explicit `return to world` sets the temporary reentry offer
  - stop-session flow does not set that offer
- Updated `Assets/Tests/EditMode/World/WorldMapScreenControllerTests.cs`
  - quick replay target resolves only when the explicit return-to-world offer exists
  - ordinary world-map load does not resolve a quick replay target
- Updated `Assets/Tests/EditMode/World/WorldMapScreenUiSetupTests.cs`
  - ordinary world-map show requires node selection
  - quick replay appears only with the explicit return-to-world offer
  - the offer is consumed after use
- Updated `Assets/Tests/EditMode/Startup/BootstrapStartupCombatScreenFlowTests.cs`
  - returning from combat still enables quick replay
  - restart/load after resolved post-run autosave no longer shows that temporary shortcut
- Updated `Assets/Tests/EditMode/Startup/BootstrapStartupScreenFlowTests.cs`
  - ordinary startup world map does not show quick replay/return
  - returning from town/service still enables quick return
  - restart/load after a town/service-origin world save does not show quick return

## Out Of Scope
- Any broader replay-history system
- Any new persistent replay metadata
- Changes to post-run replay behavior
- Changes to autosave behavior
- Changes to safe-resume target routing
- Mid-run save/resume, offline rewards, or broader automation systems

## Verification
- Compile/import first:
  - `powershell -NoLogo -NoProfile -File "tools/unity_compile_check.ps1"`
  - result: success
  - log: `C:\IT_related\myGame\Survivalon\Logs\unity_compile_check.log`
- EditMode helper verification:
  - `powershell -NoLogo -NoProfile -File "tools/unity_editmode_verify.ps1"`
  - compile/import passed again, but the known helper artifact issue reproduced and `Logs/editmode_results.xml` was not created
- Fallback:
  - `powershell -NoLogo -NoProfile -File "tools/run_editmode_tests.ps1" -ResultsPath "C:\IT_related\myGame\Survivalon\Logs\m078a_editmode_results.xml" -LogPath "C:\IT_related\myGame\Survivalon\Logs\m078a_editmode.log"`
- Final result:
  - `523 passed`
  - `0 failed`
  - `0 inconclusive`
  - `0 skipped`
- Artifacts:
  - `C:\IT_related\myGame\Survivalon\Logs\unity_compile_check.log`
  - `C:\IT_related\myGame\Survivalon\Logs\m078a_editmode_results.xml`
  - `C:\IT_related\myGame\Survivalon\Logs\m078a_editmode.log`
