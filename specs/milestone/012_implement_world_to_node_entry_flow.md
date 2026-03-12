# Milestone 012 - Implement world-to-node entry flow

- Added explicit node entry from the world map: `WorldMapScreen` now requires selecting a reachable node and then confirming entry through an `EnterSelectedNodeButton`.
- Added `WorldNodeEntryFlowController` and `NodePlaceholderState` so node entry uses plain C# flow logic to validate the selected destination, update persistent world context, and create a minimal placeholder node-state payload.
- `BootstrapStartup` now swaps between the world map and a new `NodePlaceholderScreen`, which acts as the minimal placeholder node state for this milestone.
- The placeholder node screen shows the entered node context and provides an explicit `Resolve Placeholder Node and Return` action that returns safely to the world map.
- Returning to the world map reuses the updated world context, so the entered node becomes the current anchor while prior reachable nodes remain available for backtracking.
- Added Edit Mode coverage for world-map entry confirmation, placeholder-screen return handling, and node-entry world-context updates.
- This milestone does not add a separate combat scene, real node gameplay, rewards, or post-run resolution systems yet; it only proves the entry and return loop through a controlled placeholder node state.
