# Milestone 005 - Create Core Identifiers And Enums

## Implemented
- Added minimal runtime identifiers for `NodeId` and `RegionId` in `Assets/Scripts/`.
- Added stable runtime enums for `NodeState`, `NodeType`, `ProgressionLayerType`, `RewardCategory`, `RewardSourceCategory`, and `ResourceCategory`.
- Added Edit Mode tests covering identifier validation/value equality and the minimum spec-required node/resource category sets.

## Changed at a high level
- Expanded the runtime assembly with foundational identifiers and category enums only.
- Expanded the Edit Mode test assembly with one focused test file for Milestone 005 behavior.

## Enabled now
- Later milestones can reference shared world/progression/economy category types without inventing duplicate strings or ad hoc constants.
- World data can use explicit `NodeId` and `RegionId` value objects instead of raw strings.

## Important limitations
- Optional early node roles such as `elite`, `loot`, `town`, and `special` were left out to keep the enum set limited to the minimum stable world-compatible types required now.
- No gameplay logic, data containers, world graph logic, or reward payload objects were added in this milestone.
- Unity batch verification imported the new files and compiled `Survivalon.Runtime` plus `Survivalon.EditModeTests`, but repeated `-runTests` executions still did not emit `Logs/m005_editmode_results.xml` or any test summary lines in `Logs/m005_editmode.log`.
