# Milestone 071 - Implement Readable Run HUD Baseline

## Summary
- Added one compact readable run-HUD layer inside the existing combat shell.
- Kept the current auto-battle, post-run, town, and world-map flows unchanged.
- Included the directly related world-map cleanup so path-role/access summaries are rebuilt from current world state instead of relying on constructor-time cached sets.

## What Changed
- Added a small run-HUD presentation seam:
  - `RunHudState`
  - `RunHudStateResolver`
  - `RunHudTextBuilder`
- The active combat shell now shows:
  - current location and node context
  - current auto-battle/run state, outcome, and elapsed time
  - player and enemy health in one compact line
  - tracked progress context when the node supports tracked progress
- Boss/gate runs now show `gate clear` progress wording instead of reusing ordinary node-clear wording.
- `WorldMapScreenController` no longer relies on cached selectable/path-role sets. It now rebuilds access/path-role inputs from current world state each time it builds node options, summary state, or selection metadata.

## Shipped Behavior After 071
- Combat/run flow is easier to read at a glance without changing combat mechanics.
- Tracked combat nodes show progress context in the run HUD.
- Boss/gate runs show gate-clear progress context in the run HUD.
- Non-combat contexts still do not show misleading combat HUD progress text.
- World-map node access, route labels, and summary text still behave the same, but they now stay correct if the same controller instance is reused after world-state changes.

## Intentionally Not Changed
- No combat redesign
- No full final HUD system
- No new gameplay mechanics
- No Milestone 072+ work

## Verification
- Compile/import first:
  - `tools/unity_compile_check.ps1`
  - log: `Logs/unity_compile_check.log`
- EditMode verification:
  - `tools/unity_editmode_verify.ps1`
  - the known helper artifact issue reproduced again and did not leave its expected `Logs/editmode_results.xml` file in this shell
- Fallback verification used the direct waited Unity batch EditMode run without `-quit`
- Final result:
  - `473 passed`
  - `0 failed`
- Artifacts:
  - `Logs/m071_editmode_results.xml`
  - `Logs/m071_editmode.log`
