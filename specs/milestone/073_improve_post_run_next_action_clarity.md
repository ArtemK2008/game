# Milestone 073 - Improve Post-Run Next-Action Clarity

## Summary
- Kept the existing post-run actions and gameplay flow unchanged.
- Added one compact post-run next-action guidance layer so the player can quickly read the practical decision after a run:
  - replay
  - return to world to push
  - return to world, then visit the service hub
  - stop safely
- Kept the work small by deriving that guidance from existing run result, world state, resource state, and progression state.

## What Changed
- Added a small post-run next-action presentation seam:
  - `PostRunNextActionState`
  - `PostRunNextActionResolver`
  - `PostRunNextActionTextBuilder`
- Updated the post-run panel so it now shows:
  - the existing compact run summary
  - a new compact next-action area beside the existing replay / return / stop buttons
- Improved visible action wording without adding new navigation:
  - replay uses the friendly node name
  - return stays the world-return action
  - stop is framed explicitly as a safe exit
- Switched post-run summary node naming to use the friendly display-name path instead of raw internal node ids.

## Shipped Behavior After 073
- After a run resolves, the current build now clearly tells the player whether the most useful next step is to:
  - replay the same node for progress or farming
  - return to world and push to the next forward node
  - return to world, then visit `Cavern Service Hub` for current project/refinement value
  - stop safely
- The guidance is still based only on already-shipped systems:
  - current replayability
  - current forward world access
  - current service-hub availability
  - current affordable project / ready-refinement state
- No new direct service-hub navigation was added from post-run.

## Intentionally Not Changed
- No new gameplay systems
- No new rewards, currencies, upgrade mechanics, or persistence rules
- No new post-run buttons beyond replay / return / stop
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
    - `Logs/m073_editmode_results.xml`
    - `Logs/m073_editmode.log`
- Final result:
  - `488 passed`
  - `0 failed`
