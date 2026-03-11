# Milestone 004 - Create Bootstrap Scene

## Implemented
- Renamed the default scene to `Assets/Scenes/BootstrapScene.unity` and made it the project startup scene.
- Added a minimal `BootstrapRoot` object with `BootstrapStartup` as the explicit startup-flow entry point.
- Kept the scene intentionally minimal: only the existing camera/light setup plus the bootstrap component.

## Changed
- Added `Assets/Scripts/BootstrapStartup.cs`.
- Updated `ProjectSettings/EditorBuildSettings.asset` so the enabled startup scene is `Assets/Scenes/BootstrapScene.unity`.
- Added this milestone note.

## Verification
- Verified the startup scene path in `ProjectSettings/EditorBuildSettings.asset`.
- Ran a batch-mode Unity smoke open:
  `& 'C:\Program Files\Unity\Hub\Editor\6000.3.10f1\Editor\Unity.exe' -batchmode -nographics -projectPath 'C:\IT_related\myGame\Survivalon' -quit -logFile 'C:\IT_related\myGame\Survivalon\Logs\m004_bootstrap_smoke.log'`
- Unity recompiled scripts, imported `Assets/Scenes/BootstrapScene.unity`, and exited batchmode successfully with return code `0`.

## Enables
- Provides one clear, controlled startup entry point for future menu or application-flow work.

## Limitations
- The bootstrap flow currently only logs initialization from `BootstrapStartup`; no menu, world, combat, save, or progression systems are started yet.
