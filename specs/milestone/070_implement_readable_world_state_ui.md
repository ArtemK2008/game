# Milestone 070 - Implement Readable World-State UI

## Summary
- Added one small readable world-state presentation layer to the existing runtime-generated world map.
- Kept the placeholder world map structure and existing interactions intact.
- Reused the real world graph, node reachability, and node access rules instead of adding UI-side route guesses.

## What Changed
- Added one focused world-map summary seam:
  - `WorldMapWorldStateSummary`
  - `WorldMapWorldStateSummaryResolver`
- The world-map screen now shows:
  - current location identity and region
  - current node and current node state
  - reachable destination counts
  - forward routes
  - backtrack / farm routes
  - blocked linked paths from the current node
  - compact state/status legends
- `WorldMapNodeOption` now also carries a `WorldMapPathRole` so each node button can show whether it is:
  - `Current anchor`
  - `Forward route`
  - `Backtrack / farm route`
  - `Blocked path`
- The world-map node button labels now surface that path-role line directly in the existing node list.
- The world-map summary text was compacted so the richer state view still fits cleanly in the current placeholder layout.

## Shipped Behavior After 070
- On the real world map, the player can now tell at a glance:
  - where they are now
  - which region they are in
  - which nodes are currently reachable
  - which routes are forward vs backtrack/farm
  - which linked path is still blocked
  - what `Available`, `InProgress`, `Cleared`, and `Locked` mean in the current placeholder presentation
- Existing node selection, entry, build-prep controls, town flow, combat flow, and post-run flow remain unchanged.

## Intentionally Not Changed
- No graph rendering system
- No minimap
- No new scene or asset work
- No HUD/run-screen changes from Milestone 071+
- No changes to combat, rewards, progression rules, or persistence rules

## Verification
- Compile/import first:
  - `tools/unity_compile_check.ps1`
  - log: `Logs/unity_compile_check.log`
- `tools/unity_editmode_verify.ps1` again reproduced the known shell artifact issue and did not leave its expected `Logs/editmode_results.xml` file in this shell.
- Verification then used the direct waited Unity batch EditMode fallback without `-quit`.
- Final result:
  - `468 passed`
  - `0 failed`
- Artifacts:
  - `Logs/m070_editmode_results.xml`
  - `Logs/m070_editmode.log`
