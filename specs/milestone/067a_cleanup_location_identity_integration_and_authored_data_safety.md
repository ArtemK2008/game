# Milestone 067a - Cleanup Location Identity Integration And Authored Data Safety

## Summary
- Kept Milestone 067 behavior intact and tightened two seams:
  - the live town/service flow now surfaces the current location identity clearly
  - shipped bootstrap content is now covered explicitly as using authored location identities rather than fallback-generated ones
- Reward mechanics did not change.
- The location/reward tie-in remains presentation/state-driven only.

## What Changed
- Extended the town/service presentation state so the live `Cavern Service Hub` screen now shows:
  - `Location: Echo Caverns`
  - the location reward focus
  - the location reward source
- Kept that logic out of `TownServiceScreen`; the screen still only wires UI and refresh.
- Added a small structural authored-vs-fallback flag to `LocationIdentityDefinition`.
- Kept the fallback identity seam in place for safety, but made authored bootstrap content tests assert that shipped forest/cavern regions and entered node states are not fallback identities.

## Behavior
- Player-visible gameplay is unchanged except for the intended service-screen clarity improvement.
- `Verdant Frontier` and `Echo Caverns` remain the two shipped location identities.
- World map, node placeholder, and post-run location-aware presentation continue to work.
- Town/service flow now exposes `Echo Caverns` clearly in the real startup -> world -> service path.

## Intentionally Not Changed
- No reward redesign
- No economy/system expansion
- No biome/faction/content-authoring framework
- No milestone 068+ work

## Verification
- Compile/import first:
  - `tools/unity_compile_check.ps1`
  - log: `Logs/unity_compile_check.log`
- `tools/unity_editmode_verify.ps1` again reproduced the known shell artifact issue and did not leave its expected `Logs/editmode_results.xml` file.
- Verification then used the direct waited Unity batch EditMode fallback.
- Final result:
  - `455 passed`
  - `0 failed`
- Artifacts:
  - `Logs/m067a_editmode_results.xml`
  - `Logs/m067a_editmode.log`
