# Milestone 011 - Implement basic world map screen

- Added a minimal runtime world map screen with `WorldMapScreen`, `WorldMapScreenController`, and `WorldMapNodeOption`.
- The bootstrap flow now creates a small in-memory demo world graph and world-state context through `BootstrapWorldMapFactory` and shows the world map when entering the world-view placeholder.
- The screen renders a simple full-screen canvas with one button row per node, showing node identity, node type, node state, and whether the node is current, selectable, selected, or locked.
- Node selection uses the existing reachability logic from `NodeReachabilityResolver` through `WorldMapScreenController`, so only reachable non-locked nodes can be selected.
- Added Edit Mode tests covering selectable node options, valid reachable-node selection, and rejection of locked or unreachable nodes.
- This milestone does not add node entry flow, route-line visualization, save/load, or polished final UI.
