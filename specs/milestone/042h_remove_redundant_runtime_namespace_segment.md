# Milestone 042h - Remove Redundant Runtime Namespace Segment

## Delivered
- Renamed runtime namespaces from `Survivalon.Runtime.*` to `Survivalon.*` while keeping the existing folder structure unchanged.
- Updated runtime `using` directives and cross-domain references to the new namespace roots.
- Updated EditMode tests to keep their existing `Survivalon.Tests.EditMode.*` namespaces while switching their runtime imports to `Survivalon.*`.
- Kept this milestone behavior-preserving:
  - no gameplay changes
  - no file or folder moves
  - no asmdef changes

## Namespace Convention Change
- `Assets/Scripts/Core/...` -> `Survivalon.Core...`
- `Assets/Scripts/Startup/...` -> `Survivalon.Startup...`
- `Assets/Scripts/Combat/...` -> `Survivalon.Combat...`
- `Assets/Scripts/Run/...` -> `Survivalon.Run...`
- `Assets/Scripts/World/...` -> `Survivalon.World...`
- `Assets/Scripts/State/...` -> `Survivalon.State...`
- `Assets/Scripts/State/Persistence/...` -> `Survivalon.State.Persistence...`
- `Assets/Scripts/Data/...` -> `Survivalon.Data...`

## Intentional Exceptions
- `Assets/Scripts/RuntimeAssemblyMarker.cs` now uses the cross-domain root namespace `Survivalon`.
- `Assets/Tests/EditMode/RuntimeAssemblySmokeTests.cs` remains in `Survivalon.Tests.EditMode` as the intentionally cross-domain root smoke test.
- The runtime assembly name remains `Survivalon.Runtime` because asmdef layout was intentionally left unchanged in this milestone.

## Why This Was Safe
- The refactor changed only namespaces, using directives, and directly affected references.
- No gameplay rules, data flow, UI behavior, or persistence behavior changed.
- The current folder structure, asmdefs, and Unity asset layout were left intact.

## Verification
- Unity EditMode batch run passed.
- Result: `288` passed, `0` failed.
- Artifacts:
  - `Logs/runtime_namespace_trim_editmode_results.xml`
  - `Logs/runtime_namespace_trim_editmode.log`
