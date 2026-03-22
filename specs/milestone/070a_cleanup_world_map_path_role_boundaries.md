# Milestone 070a - Cleanup World-Map Path-Role Boundaries

## Summary
- Tightened the world-map readability model so the UI no longer implies that all non-forward selectable nodes are real routes.
- Kept gameplay behavior unchanged.
- Split true backtrack connectivity from replayable cleared-node farm access in the world-map projection layer.

## What Changed
- Added one small access-side distinction:
  - `WorldNodeAccessResolver.GetPathEnterableNodes(...)`
- Refined `WorldMapPathRole` so world-map node labels can now distinguish:
  - `Current anchor`
  - `Forward route`
  - `Backtrack route`
  - `Replayable farm node`
  - `Blocked path`
- `WorldMapWorldStateSummaryResolver` now projects separate deterministic lists for:
  - forward routes
  - backtrack routes
  - replayable farm nodes
  - blocked linked paths
- `WorldMapScreenTextBuilder` now uses that split state so the summary wording stays accurate and no longer labels farm-only replay access as a route.

## Shipped Behavior After 070a
- Real graph-linked path information is still shown clearly.
- Cleared-node replay/farm access is still selectable and enterable exactly as before.
- The world map now presents replayable farm access as its own concept instead of implying it is a backtrack route.
- Ordering of the shown forward/backtrack/replayable/blocked node lists is now deterministic.

## Intentionally Not Changed
- No new gameplay systems
- No world-map redesign
- No changes to persistence, rewards, progression, combat, or town logic
- No Milestone 071+ work

## Verification
- Compile/import first:
  - `tools/unity_compile_check.ps1`
  - log: `Logs/unity_compile_check.log`
- `tools/unity_editmode_verify.ps1` again reproduced the known shell artifact issue and did not leave its expected `Logs/editmode_results.xml` file in this shell.
- Verification then used the direct waited Unity batch EditMode fallback without `-quit`.
- Final result:
  - `469 passed`
  - `0 failed`
- Artifacts:
  - `Logs/m070a_editmode_results.xml`
  - `Logs/m070a_editmode.log`
