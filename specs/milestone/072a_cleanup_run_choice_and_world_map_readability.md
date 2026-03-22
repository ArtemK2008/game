# Milestone 072a - Clean Up Run-Choice Presentation Boundaries And World-Map Readability

## Summary
- Kept gameplay behavior unchanged while cleaning presentation ownership and making the current placeholder combat/world-map UI easier to read.
- Removed avoidable UI-only copy from the combat/runtime upgrade option model.
- Preserved the 070a world-map stale-access fix and kept access/path-role projection derived from current world state at refresh time.

## What Changed
- Moved run-start upgrade-choice presentation copy out of `CombatRunTimeSkillUpgradeOption` and into a small run-side presentation seam:
  - `RunTimeSkillUpgradeChoicePresentationCatalog`
  - `RunTimeSkillUpgradeChoiceStateResolver`
- Kept combat/runtime upgrade option data gameplay-focused:
  - upgrade id
  - display name
  - authored effect description
- Cleaned the combat-capable node screen so it no longer repeats the larger generic placeholder summary block above the run HUD:
  - combat contexts now keep a compact persistent header using the friendly node name plus short location/encounter context
  - the existing readable run HUD remains responsible for health, run state, elapsed time, outcome, and tracked progress
  - the pending run-start upgrade choice still shows that context so the player is not choosing blind
- Added authored friendly display names for the shipped bootstrap nodes and threaded them through world/node presentation:
  - world-map node buttons
  - selected-node display
  - entry button text
  - node placeholder title/state
  - run HUD combat title
- Refined the world-map summary to stay compact and readable while keeping the 070a forward/backtrack/replayable/blocked distinction intact.

## Shipped Behavior After 072a
- The current `Burst Strike` run-start choice still appears at the same time and applies the same effects as before.
- The choice panel still shows:
  - a clear title
  - a short summary
  - option names
  - short effect summaries
  - quick pick hints
- Combat-capable node screens now keep less duplicated text and a clearer persistent context header.
- The main world-map UI now reads in authored friendly names such as `Forest Farm`, `Raider Trail`, and `Cavern Service Hub` instead of raw internal node ids.
- World-map access/path-role output still recomputes from current world state when the same controller instance is refreshed.

## Intentionally Not Changed
- No new run-upgrade mechanics
- No change to upgrade timing, selection count, or effect behavior
- No change to combat rules, rewards, persistence, town flow, or node access behavior
- No Milestone 073+ work

## Verification
- Compile/import first:
  - `tools/unity_compile_check.ps1`
  - log: `Logs/unity_compile_check.log`
- EditMode verification:
  - `tools/unity_editmode_verify.ps1`
  - the known helper artifact issue reproduced again and did not leave `Logs/editmode_results.xml`
- Fallback verification:
  - `tools/run_editmode_tests.ps1 -ResultsPath Logs/m072a_editmode_results.xml -LogPath Logs/m072a_editmode.log`
- Final result:
  - `481 passed`
  - `0 failed`
- Artifacts:
  - `Logs/m072a_editmode_results.xml`
  - `Logs/m072a_editmode.log`
