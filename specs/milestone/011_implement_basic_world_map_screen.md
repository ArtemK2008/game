# Milestone 011 - Implement basic world map screen

- Added a minimal runtime world map screen with `WorldMapScreen`, `WorldMapScreenController`, and `WorldMapNodeOption`.
- The bootstrap flow now creates a small in-memory demo world graph and world-state context through `BootstrapWorldMapFactory` and shows the world map when entering the world-view placeholder.
- The screen renders a simple full-screen runtime canvas with one button row per node, showing node identity, node type, node state, and whether the node is current, selectable, selected, or locked.
- EventSystem setup now uses `InputSystemUIInputModule` so the world map stays compatible with the active Input System package and does not rely on `StandaloneInputModule`.
- Runtime text now uses Unity 6 compatible `LegacyRuntime.ttf` instead of deprecated built-in `Arial.ttf`, so the placeholder world map can render without throwing during startup.
- Node selection uses the existing reachability logic from `NodeReachabilityResolver` through `WorldMapScreenController`, so only reachable non-locked nodes can be selected.
- Added Edit Mode tests covering selectable node options, valid reachable-node selection, rejection of locked or unreachable nodes, and world map UI bootstrap setup.
- This milestone does not add node entry flow, route-line visualization, save/load, or polished final UI.
