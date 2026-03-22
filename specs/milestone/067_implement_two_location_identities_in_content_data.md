# Milestone 067 - Implement Two Location Identities In Content Data

## Summary
- Added two explicit shipped location identities in content data:
  - `Verdant Frontier`
  - `Echo Caverns`
- Attached those identities to the current bootstrap world regions so node/location identity is now explicit in shipped content instead of being inferred only from raw region ids.
- Carried the resolved location identity through world-map node options and node-entry placeholder state.
- Made current shipped presentation location-aware in three small places:
  - world-map node labels
  - node placeholder summary text
  - post-run summary text
- Tied reward/source presentation to location identity through data-owned reward-source labels, so the current MVP flow now makes it clearer where repeated farming rewards are coming from.

## What Changed
- Added a small location-identity data seam with definition/catalog ownership in the world/data layer.
- Updated bootstrap world content so current shipped regions map to those identities:
  - forest content uses `Verdant Frontier`
  - cavern content uses `Echo Caverns`
- Updated world-map state resolution so node labels include the location display name.
- Updated placeholder node/post-run presentation so the player can see:
  - the current location identity
  - the current reward focus or reward source

## Current Shipped Effect
- The prototype now has two explicit place identities in live content, visible to the player:
  - `Verdant Frontier` for the current forest-side combat/boss path
  - `Echo Caverns` for the current cavern-side service context
- Reward/source wording is now location-aware without changing the actual reward mechanics.
- Core combat, town, build, progression, and conversion behavior stayed the same.

## Intentionally Not Added
- No biome system
- No faction system
- No new currencies
- No large map/navigation redesign
- No authored external location content pipeline
- No milestone 068+ work

## Verification
- Compile/import first:
  - `tools/unity_compile_check.ps1`
  - log: `Logs/unity_compile_check.log`
- `tools/unity_editmode_verify.ps1` reproduced the known shell issue and did not leave its expected `Logs/editmode_results.xml` artifact in this shell.
- Verification then used the direct waited Unity batch EditMode fallback.
- Final result:
  - `454 passed`
  - `0 failed`
- Artifacts:
  - `Logs/m067_editmode_results.xml`
  - `Logs/m067_editmode.log`
