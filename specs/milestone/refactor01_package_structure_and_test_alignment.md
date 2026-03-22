# Refactor Milestone 01 - Package Structure And Test Alignment

## Summary
- Kept all player-facing behavior unchanged.
- Tightened one clear ownership mismatch in the post-run slice:
  - post-run text builders now live under the `Run` runtime domain beside the rest of the post-run state/resolver classes
  - their direct unit tests now live under `Tests/EditMode/Run`
- Reduced misplaced direct builder tests from the world-screen presentation test file so the main tests now mirror runtime ownership more closely.

## Runtime Boundary Changes
- Moved runtime post-run presentation builders:
  - `Assets/Scripts/World/PostRunSummaryTextBuilder.cs`
  - `Assets/Scripts/World/PostRunNextActionTextBuilder.cs`
- New runtime ownership:
  - `Assets/Scripts/Run/PostRunSummaryTextBuilder.cs`
  - `Assets/Scripts/Run/PostRunNextActionTextBuilder.cs`
- Namespace ownership now matches that placement:
  - `Survivalon.Run`

## Test Alignment Changes
- Moved direct unit tests to mirror the runtime package:
  - `Assets/Tests/EditMode/World/PostRunSummaryTextBuilderTests.cs`
    -> `Assets/Tests/EditMode/Run/PostRunSummaryTextBuilderTests.cs`
  - `Assets/Tests/EditMode/World/PostRunNextActionTextBuilderTests.cs`
    -> `Assets/Tests/EditMode/Run/PostRunNextActionTextBuilderTests.cs`
- Removed duplicate direct builder-only assertions from `NodePlaceholderScreenPresentationTests` so that file stays focused on world/node screen presentation ownership.

## Boundary Decision
- The non-obvious decision was whether post-run text builders belonged under `World` because `NodePlaceholderScreen` renders them.
- I kept them under `Run` because their direct reason to change is post-run/run-state wording, not world-graph or world-map behavior:
  - they depend on `RunResult`, `PostRunStateController`, and post-run recommendation state
  - they are presentation for the run/post-run domain
  - `NodePlaceholderScreen` only consumes them as part of its world-screen wiring

## Verification
- Compile/import first:
  - `tools/unity_compile_check.ps1`
  - log: `Logs/unity_compile_check.log`
- EditMode verification:
  - `tools/unity_editmode_verify.ps1`
  - the known helper artifact issue reproduced again and did not create `Logs/editmode_results.xml`
- Fallback verification:
  - waited Unity batch EditMode run writing:
    - `Logs/refactor01_editmode_results.xml`
    - `Logs/refactor01_editmode.log`
- Final result:
  - `491 passed`
  - `0 failed`
