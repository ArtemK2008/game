# Milestone 030 - Implement Next-Node Unlock Rule

## Delivered
- Added `NextNodeUnlockService` as the small progression rule layer for unlocking directly connected next nodes when a tracked source node clears.
- Wired the clear-to-unlock transition into the existing run resolution flow:
  - successful tracked combat runs still grant kill-driven node progress
  - when that progress reaches the node threshold and the node becomes `Cleared`, directly connected locked next nodes become `Available`
  - the unlocked state is persisted in `PersistentWorldState`, not only in transient UI state
- Updated reachability and node-entry flow to respect persistent node state when present, so newly unlocked nodes become reachable/selectable on the world map after returning from the cleared node flow.
- Kept the rule minimal and milestone-scoped:
  - direct connected-node unlock only
  - no branch-specific advanced rule layer
  - no boss/gate redesign

## Tests
- Added `NextNodeUnlockServiceTests` to verify:
  - a locked directly connected node unlocks when the source node reaches `Cleared`
  - repeated unlock application does not duplicate or corrupt the unlocked state
- Updated `NodeReachabilityResolverTests` to verify persistently unlocked nodes become forward reachable.
- Updated `RunLifecycleControllerTests` to verify:
  - a tracked combat node unlocks its connected next node when it clears
  - replaying an already cleared node does not re-trigger duplicate unlock behavior
- Updated `BootstrapStartupScreenFlowTests` to verify the real placeholder flow:
  - clear the tracked combat node
  - return to the world map
  - see the newly unlocked next node as `Available` and selectable

## Out Of Scope
- Advanced branch unlock rules
- Dedicated gate semantics beyond the current shared placeholder rule
- Reward/economy expansion
- Broader progression redesign

This milestone only implements direct next-node unlock-on-clear.
Connected-node unlocking is now in place, but broader progression routing behavior remains deferred.
`BossOrGate` continues to use the temporary shared tracked-progress rule until later progression and boss milestones revisit it explicitly.

## Verification
- Unity EditMode batch run passed:
  - `Logs/m030_editmode_results.xml`
  - `Logs/m030_editmode.log`
- Result: `120` passed, `0` failed
