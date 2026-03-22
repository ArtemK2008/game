# Milestone 072 - Implement Upgrade Choice UI

## Summary
- Turned the existing run-start skill-upgrade pause into one compact readable choice panel.
- Kept the shipped choice logic, effects, and run flow unchanged.
- Kept the work scoped to presentation/state formatting only for the current `Burst Strike` choice.

## What Changed
- Added a small run-choice presentation seam under `Assets/Scripts/Run/`:
  - `RunTimeSkillUpgradeChoiceState`
  - `RunTimeSkillUpgradeChoiceStateResolver`
  - `RunTimeSkillUpgradeChoiceTextBuilder`
- Extended the existing run-only upgrade option data with compact presentation-facing fields:
  - source skill display name
  - quick pick hint
- Updated `NodePlaceholderScreen` so the run-start choice panel now:
  - clearly says the player must choose one upgrade before auto-battle starts
  - shows each option with a readable name, effect summary, and quick pick hint
  - uses the existing click-through upgrade selection flow without changing upgrade application
- Kept the panel compatible with the current 071 readable run HUD baseline by continuing to hide the active combat shell until the pending choice is made.

## Shipped Behavior After 072
- Runs that expose the current `Burst Strike` upgrade choice now show a faster-to-parse panel before auto-battle starts.
- The player can quickly see:
  - what each choice is
  - what it changes
  - why they might pick it
- Runs with no current run-only upgrade choice continue straight into the existing auto-battle flow with no extra panel.
- Choosing `Burst Tempo` or `Burst Payload` still applies the same current run-only effect as before.

## Intentionally Not Changed
- No broader upgrade system
- No new upgrade pools or mechanics
- No rerolls, rarity, currencies, or persistent upgrade state
- No Milestone 073+ work

## Verification
- Compile/import first:
  - `tools/unity_compile_check.ps1`
  - log: `Logs/unity_compile_check.log`
- EditMode verification:
  - `tools/unity_editmode_verify.ps1`
  - the known helper artifact issue reproduced again and did not leave `Logs/editmode_results.xml`
- Fallback verification:
  - `tools/run_editmode_tests.ps1 -ResultsPath Logs/m072_editmode_results.xml -LogPath Logs/m072_editmode.log`
- Final result:
  - `476 passed`
  - `0 failed`
- Artifacts:
  - `Logs/m072_editmode_results.xml`
  - `Logs/m072_editmode.log`
