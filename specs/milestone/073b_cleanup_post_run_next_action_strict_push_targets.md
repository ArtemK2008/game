# Milestone 073b - Cleanup Post-Run Next-Action Strict Push Targets

## Summary
- Kept player-facing post-run behavior unchanged.
- Tightened the post-run helper seams so push-target resolution is semantically strict in every path.
- Cleaned one suspicious type reference and removed `var` from the touched cleanup area.

## What Changed
- Updated `PostRunReplayOpportunityResolver` to use an explicit `ResourceCategory` reference instead of the suspicious `Core.ResourceCategory` form.
- Tightened `PostRunForwardOpportunityResolver` so service/progression nodes are never surfaced as push targets:
  - not during normal forward-node scanning
  - not during the boss-gate unlock shortcut path
- Replaced the introduced `var` usage in the 073a helper area with explicit readable typing.
- Added focused test coverage for the strict boss-unlock/service-node boundary.

## Shipped Behavior After 073b
- Current bootstrap behavior is unchanged:
  - the forest boss still recommends pushing to `Cavern Gate`
  - service-hub opportunity still resolves separately
  - replay reasoning still behaves the same
- The cleanup only hardens future safety:
  - if a future unlock points at a service/progression node, it will not be treated as a push target

## Intentionally Not Changed
- No new actions, buttons, flows, mechanics, or persistence rules
- No UI wording changes
- No Milestone 074+ work

## Verification
- Compile/import first:
  - `tools/unity_compile_check.ps1`
  - log: `Logs/unity_compile_check.log`
- EditMode verification:
  - `tools/unity_editmode_verify.ps1`
  - the known helper artifact issue reproduced again and did not create `Logs/editmode_results.xml`
- Fallback verification:
  - waited Unity batch EditMode run writing:
    - `Logs/m073b_editmode_results.xml`
    - `Logs/m073b_editmode.log`
- Final result:
  - `494 passed`
  - `0 failed`
