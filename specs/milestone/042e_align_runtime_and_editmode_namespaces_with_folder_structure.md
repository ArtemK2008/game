# Milestone 042e - Align Runtime And EditMode Namespaces With Folder Structure

## Delivered
- Aligned runtime namespaces with the current script folders:
  - `Assets/Scripts/Core/...` -> `Survivalon.Runtime.Core...`
  - `Assets/Scripts/Startup/...` -> `Survivalon.Runtime.Startup...`
  - `Assets/Scripts/Combat/...` -> `Survivalon.Runtime.Combat...`
  - `Assets/Scripts/Run/...` -> `Survivalon.Runtime.Run...`
  - `Assets/Scripts/World/...` -> `Survivalon.Runtime.World...`
  - `Assets/Scripts/State/...` -> `Survivalon.Runtime.State...`
  - `Assets/Scripts/State/Persistence/...` -> `Survivalon.Runtime.State.Persistence...`
  - `Assets/Scripts/Data/...` -> `Survivalon.Runtime.Data...`
- Aligned EditMode test namespaces with the current test folders:
  - `Assets/Tests/EditMode/Core/...` -> `Survivalon.Tests.EditMode.Core...`
  - `Assets/Tests/EditMode/Startup/...` -> `Survivalon.Tests.EditMode.Startup...`
  - `Assets/Tests/EditMode/Combat/...` -> `Survivalon.Tests.EditMode.Combat...`
  - `Assets/Tests/EditMode/Run/...` -> `Survivalon.Tests.EditMode.Run...`
  - `Assets/Tests/EditMode/World/...` -> `Survivalon.Tests.EditMode.World...`
  - `Assets/Tests/EditMode/State/...` -> `Survivalon.Tests.EditMode.State...`
  - `Assets/Tests/EditMode/State/Persistence/...` -> `Survivalon.Tests.EditMode.State.Persistence...`
  - `Assets/Tests/EditMode/Data/...` -> `Survivalon.Tests.EditMode.Data...`
- Updated runtime and test files to use explicit cross-domain `using` directives after the flat namespace went away.
- Kept this milestone behavior-preserving:
  - no gameplay changes
  - no file moves
  - no asmdef rewrite

## Intentional Exceptions
- `Assets/Scripts/RuntimeAssemblyMarker.cs` remains in `Survivalon.Runtime` because it is an assembly-level marker rather than a domain-owned gameplay type.
- `Assets/Tests/EditMode/RuntimeAssemblySmokeTests.cs` remains in `Survivalon.Tests.EditMode` because it is intentionally cross-domain smoke coverage rather than domain-owned behavior coverage.

## Why This Improves Readability
- Folder ownership and namespace ownership now tell the same story.
- Cross-domain dependencies are explicit in each file instead of being hidden by the old flat namespace.
- Runtime and test navigation now stays consistent between physical folders and code references.

## Verification
- Unity EditMode batch run passed.
- Result: `288` passed, `0` failed.
- Artifacts:
  - `Logs/namespace_alignment_editmode_results.xml`
  - `Logs/namespace_alignment_editmode.log`
