# Milestone 065 - implement one conversion/refinement mechanic

## Goal
Add one small refinement/conversion flow inside the existing `Cavern Service Hub` so persistent resources can be turned into another lasting progression input without expanding into a broader crafting or town-navigation system.

## Decision
- The smallest honest 065 step is one fixed town-side refinement recipe, not a crafting framework.
- The shipped conversion is:
  - `Region material x3 -> Persistent progression material x1`
- It is exposed through the existing town/service progression surface.
- It persists immediately on success and refreshes the service screen in place.

This adds clear value because it lets repeatable region-material gains feed back into the existing account-wide project sink, which makes the town/service shell more useful between runs without introducing new currencies or larger hub systems.

## Delivered
- Added one tiny town conversion data seam:
  - `TownServiceConversionId`
  - `TownServiceConversionDefinition`
  - `TownServiceConversionCatalog`
- Added one focused interaction seam:
  - `TownServiceConversionInteractionService`
- Extended `TownServiceScreenStateResolver` and `TownServiceScreenState` so the service shell now resolves:
  - current `Region material` balance
  - one visible conversion option with affordability state
- Extended `TownServiceScreenTextBuilder` and `TownServiceScreen` so the conversion is:
  - shown in the progression hub summary
  - invoked with one button
  - refreshed immediately after a successful conversion

## Behavior Change
- `Cavern Service Hub` still remains the only shipped town/service shell.
- Existing direct project purchasing still works there.
- Existing build-preparation editing still works there.
- The new conversion behavior is:
  - if the player has at least `Region material x3`, they can run `Region Material Refinement`
  - successful conversion immediately spends `Region material x3`
  - successful conversion immediately grants `Persistent progression material x1`
  - the updated balances persist immediately
  - the service screen refreshes immediately
- If the player has fewer than `3` region material, the conversion remains visible but unavailable.

## SRP Notes
- SRP stayed clean in the touched area.
- Conversion rule/data ownership lives in the small town conversion definition/catalog seam.
- Persistent resource mutation and persistence handoff live in `TownServiceConversionInteractionService`.
- `TownServiceScreenStateResolver` still resolves presentation state only.
- `TownServiceScreenTextBuilder` still formats player-facing text only.
- `TownServiceScreen` still only wires buttons, invokes focused interaction seams, and refreshes the view.

## Verification
- Followed the mandatory compile/import-first workflow from `AGENTS.md`.
- Ran:
  - `tools/unity_compile_check.ps1`
  - log: `Logs/unity_compile_check.log`
- Then ran:
  - `tools/unity_editmode_verify.ps1`
  - compile/import passed again, but the helper still failed to create its expected `Logs/editmode_results.xml` artifact in this shell
- Fallback verification used a direct waited Unity batch EditMode run without `-quit`.
- Final EditMode result:
  - `453 passed`
  - `0 failed`
- Artifacts:
  - `Logs/unity_compile_check.log`
  - `Logs/m065_editmode_results.xml`
  - `Logs/m065_editmode.log`

## Out Of Scope
- No Milestone 066+ work
- No broader crafting/refinement system
- No recipe list, tabs, timers, or queueing
- No new currencies
- No removal of existing world-map or town interactions
