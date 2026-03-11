# Milestone 007 - Create Persistent Game State Model

## Implemented
- Added plain persistent state models for world state, node state, progression state, resource balances, character state, and loadout state.
- Added a root `PersistentGameState` aggregate plus small supporting models for progression entries, resource balances, and equipped gear entries.
- Updated `NodeState` to include `InProgress` so the persistent node model matches the unlock-flow spec.
- Added Edit Mode tests covering node-progress state transitions and resource balance spend rules.

## Changed at a high level
- Expanded the runtime assembly with serializable plain C# persistent-state models only.
- Expanded the Edit Mode test assembly with one focused test file for Milestone 007 state transitions.

## Enabled now
- Later milestones can persist world progression, resources, characters, loadouts, and broad progression state without mixing them into temporary run or combat state.
- World persistence now has explicit space for node progress, reachable-node state, unlocked-region state, and last safe world-level resume context.

## Important limitations
- No save/load infrastructure, serialization backend, repositories, managers, or autosave orchestration was added.
- No temporary run state, combat runtime state, world traversal logic, or unlock-application services were added.
- Gear ownership is intentionally represented only as a simple owned-gear id list on `PersistentGameState`; richer gear progression state remains for later milestones.
- Character/build references that do not yet have dedicated identifier types remain string ids.
- Unity batch verification imported the new persistence-state folders and compiled `Survivalon.Runtime` plus `Survivalon.EditModeTests`, but `-runTests` still did not emit `Logs/m007_editmode_results.xml` or any test summary lines in `Logs/m007_editmode.log`.
