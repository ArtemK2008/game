# Milestone 013 - Implement route choice support

- Added one real branch case to the demo world graph: the current frontier push node now leads to both a side farming combat node and a service/progression node.
- `WorldMapScreenController` now tracks forward-selectable destinations separately from all reachable destinations and exposes whether the current node offers a real branch choice.
- Branch availability is derived from the existing graph and reachability model through `NodeReachabilityResolver.GetForwardReachableNodes`, so locked or disconnected nodes are not offered as forward route options.
- `WorldMapScreen` now surfaces forward route count in its summary text so the branch case remains readable in the current placeholder UI.
- Added Edit Mode tests covering multiple forward options on a branch node, exclusion of locked branch destinations, exclusion of disconnected available nodes, and readable branch summary on the world map.
- This milestone does not add route-visualization polish, branch-specific rewards, new unlock systems, or broader world-map redesign.
- Batch Unity verification was blocked while the project was already open in another Unity editor instance, so `-runTests` returned before execution without a test summary in that environment.
