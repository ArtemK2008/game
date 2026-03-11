# Milestone 009 - Implement world graph model

- Added the plain runtime world-graph layer in `Assets/Scripts/World/` with `WorldRegion`, `WorldNode`, `WorldNodeConnection`, and `WorldGraph`.
- Represented regions as persistent containers of node ids plus entry node, progression order, resource identity, and difficulty band.
- Represented nodes with explicit `RegionId`, `NodeId`, `NodeType`, and `NodeState`, keeping locked versus available handling in the graph reachability rules.
- Represented routes as explicit source-to-target `WorldNodeConnection` entries; no implicit reverse links are created.
- Added Edit Mode reachability tests covering connected available nodes, locked nodes, explicit connection direction, and disconnected nodes.
- This milestone only adds the runtime graph model and basic reachability traversal. It does not add world-map UI, node entry flow, unlock progression services, or save/load changes.
