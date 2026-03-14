# Milestone 027 - Verify No-Manual-Combat Core Loop

## Delivered
- Audited the current combat prototype and confirmed the combat resolver itself was already automatic before this milestone:
  - no movement is required in the current MVP combat model
  - player and enemy already auto-targeted and auto-attacked
  - combat outcome already mapped to run success/failure
- Removed the remaining shell-level manual combat dependency for combat nodes:
  - combat-compatible node runs now auto-enter `RunActive` from the node placeholder shell
  - resolved combat runs now auto-enter `PostRun` without needing a manual `Enter Post-Run State` click
- Kept non-combat placeholder nodes on their existing manual shell flow.
- Kept the combat shell as a readable debug-style view while making the no-manual combat loop explicit in `RunLifecycleController`.

## Tests
- Updated `RunLifecycleControllerTests` to prove combat nodes can advance from node entry to `PostRun` automatically for both player-victory and enemy-victory cases, and that automatic combat flow stops after post-run is reached.
- Updated `NodePlaceholderScreenUiTests` to prove:
  - combat nodes auto-start without a `Start Combat Shell` click
  - combat nodes reach post-run without manual combat input
  - replayed combat nodes auto-start again
  - hostile boss combat reaches failed post-run without manual combat input
- Updated `BootstrapStartupScreenFlowTests` to prove the world map -> combat node -> post-run loop and the return-to-world flow work without any manual combat-step clicks.

## User Actions Still Required
- Select a combat node on the world map.
- Click `Enter <node>` on the world map.
- After combat auto-resolves and post-run appears, choose a post-run action such as replay, return to world, or stop.

No manual movement, manual attack input, or manual combat-step button presses are required during combat itself.

## Intentionally Left Out
- Movement systems
- Manual combat actions
- Advanced AI or targeting heuristics
- Multi-entity combat
- Animation, VFX, or combat-presentation polish
- Broader run-system redesign

## Verification
- Unity EditMode batch run passed:
  - `Logs/m027_editmode_results.xml`
  - `Logs/m027_editmode.log`
- Practical Unity Game-view verification passed through a temporary editor-only verifier:
  - `Logs/m027_no_manual_combat_runtime_check.txt`
  - verified actions were limited to:
    - click combat node on the world map
    - click `Enter Selected Node`
    - wait for combat to auto-resolve into post-run
  - verified `Advance button clicks: 0`
  - verified final combat-shell state reached post-run with `Run Lifecycle Complete`
