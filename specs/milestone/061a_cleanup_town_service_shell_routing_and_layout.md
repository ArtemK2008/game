# Milestone 061a - cleanup town/service shell routing and layout

## Goal
Tighten the MVP town/service shell shape after Milestone 061 without expanding the feature scope.

## What changed
- `BootstrapStartup` now opens `TownServiceScreen` only when the entered `NodePlaceholderState` carries an explicit `TownServiceContext`.
- `ServiceOrProgression` nodes without `TownServiceContext` now fall back to the existing generic `NodePlaceholderScreen` instead of being routed by node type alone.
- `TownServiceScreen` now uses a safer content-driven layout with a scrollable content area for the overview, progression, and build sections.
- `TownServiceScreenTextBuilder` no longer owns hardcoded account-wide upgrade display-name mapping.
- Account-wide upgrade display names now come from `AccountWideProgressionUpgradeDefinition` data and flow through `TownServiceProgressionOptionState` into presentation formatting.

## Behavior Change
- Current shipped behavior is preserved:
  - `Cavern Service Hub` still opens the town/service shell
  - combat nodes still use the existing combat/run flow
  - the town/service shell remains non-combat and read-only
  - gear and skill-package editing still stay on the world map
- The only routing behavior change is the intended cleanup:
  - service/progression nodes without explicit town-service content now use the generic placeholder shell safely

## What was intentionally left out
- no interactive progression purchasing
- no movement of gear/package editing into the town shell
- no new town buildings, NPCs, or broader hub systems
- no Milestone 062+ work

## Verification
- Ran the required compile/import-first workflow.
- `tools/unity_compile_check.ps1` was run first. In this environment the script reported a false-positive compile failure because it matched non-compiler Unity log text, but the generated Unity log completed successfully without compiler errors.
- `tools/run_editmode_tests.ps1` detached again without producing artifacts in this shell.
- Followed up with a direct waited Unity EditMode batch run instead.
- Result: `432 passed`, `0 failed`
- Artifacts:
  - `Logs/m061a_editmode_results.xml`
  - `Logs/m061a_editmode.log`
