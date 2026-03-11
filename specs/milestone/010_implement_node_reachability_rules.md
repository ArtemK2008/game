# Milestone 010 - Implement node reachability rules

- Added `NodeReachabilityResolver` as the current-world-context rule layer on top of the raw `WorldGraph`.
- Forward reachable nodes are resolved from the current anchor node through direct outbound graph connections, excluding locked destinations.
- Backtrack reachable nodes are resolved from previously reachable world-state context, using last-safe and remembered reachable node ids while still excluding locked destinations.
- Combined current reachability returns the union of forward and backtrack destinations without duplicating the current node.
- Added Edit Mode tests covering forward reachability, locked-node exclusion, backtracking, disconnected-node exclusion, and forward-versus-backtrack distinction.
- This milestone does not add world-map UI, node entry flow, unlock application services, or save/load infrastructure.
