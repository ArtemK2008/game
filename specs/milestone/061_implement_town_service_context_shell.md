# Milestone 061 - Implement Town Service Context Shell

## Decision
- The smallest honest 061 step is to reuse the existing cavern `ServiceOrProgression` node and make it open one explicit non-combat town/service shell.
- The shell should be distinct from combat, support short-session use, and surface already-implemented persistent systems without dragging in a broader town simulation or new progression systems.

## Delivered
- Added one explicit town-service content seam:
  - `TownServiceContextDefinition`
  - `TownServiceContextCatalog`
- The cavern service node now carries `Cavern Service Hub` content and enters a dedicated `TownServiceScreen` instead of the generic node placeholder shell.
- Added a focused town/service presentation stack:
  - `TownServiceScreen`
  - `TownServiceScreenState`
  - `TownServiceScreenStateResolver`
  - `TownServiceScreenTextBuilder`
- The shipped service shell now shows:
  - a progression-hub summary using current persistent progression material and account-wide project state
  - a build-preparation summary using the selected character, assigned package, and equipped primary/support gear
  - `Return To World Map`
  - `Stop Session`
- The shared safe-return service was renamed and generalized from post-run-only use to world-context use:
  - `BootstrapPostRunTransitionService` -> `BootstrapWorldContextTransitionService`

## Behavior Change
- Entering `region_002_node_001` now opens a distinct town/service screen.
- That screen is clearly non-combat:
  - no combat shell
  - no run lifecycle advance flow
  - no replay/post-run actions
- The player can review existing persistent progression/build state there and then:
  - return directly to the world map
  - stop the session from that safe service context
- Current build changes remain on the world map in this MVP.
- Current account-wide progression spending remains outside the service shell in this milestone. The shell exposes the progression sink as readable context only.

## SRP Notes
- World/bootstrap content owns which node carries the town-service context.
- `TownServiceScreenStateResolver` owns conversion from persistent game state into service-shell view state.
- `TownServiceScreenTextBuilder` owns player-facing wording.
- `TownServiceScreen` only wires the placeholder UI and callbacks.
- `BootstrapWorldContextTransitionService` now owns safe return/stop handling for both post-run and service contexts, which keeps that responsibility out of `BootstrapStartup`.

## Tests
- Added dedicated town/service tests to cover:
  - service-shell state resolution
  - readable service-shell presentation text
  - service-shell UI structure and callbacks
  - startup/world flow reaching the service shell from the cavern service node
- Updated startup/world/session tests to cover:
  - service-node content attachment in bootstrap world data
  - service context carried into `NodePlaceholderState`
  - startup routing to the town/service shell
  - safe return/stop behavior through the generalized world-context transition service
  - startup test helpers preferring active-screen buttons after the new service shell introduced duplicate button names across hidden screens

## Verification
- `tools/run_editmode_tests.ps1` detached again without producing fresh artifacts in this shell.
- Verification then used the direct waited Unity batch invocation.
- A first direct run exposed startup-flow helper assumptions that were still selecting inactive hidden-screen buttons; that helper was tightened and the suite was rerun.
- Final result: `431 passed`, `0 failed`
- Artifacts:
  - `Logs/m061_editmode_results.xml`
  - `Logs/m061_editmode.log`

## Out Of Scope
- Milestone 062 or later service interaction expansion
- interactive progression-board purchasing inside the service shell
- moving package/gear assignment off the world map
- multiple town buildings, NPC systems, crafting, or town simulation
- broader non-combat context content beyond the single shipped `Cavern Service Hub`
