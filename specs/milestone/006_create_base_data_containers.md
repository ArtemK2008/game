# Milestone 006 - Create Base Data Containers

## Implemented
- Added ScriptableObject definitions for regions, nodes, rewards, characters, and gear placeholders.
- Added serializable nested data classes for node connections, reward resource amounts, and combat stat blocks.
- Added Edit Mode tests covering container defaults and typed world identifier access.

## Changed at a high level
- Expanded the runtime assembly with static/configurable data definitions only.
- Expanded the Edit Mode test assembly with one focused test file for the new container layer.

## Enabled now
- Later milestones can create world, reward, character, and gear assets without inventing ad hoc schema per feature.
- World data now has a clean region/node container foundation that is compatible with the existing `NodeId`, `RegionId`, `NodeType`, `NodeState`, `RewardCategory`, `RewardSourceCategory`, and `ResourceCategory` types.

## Important limitations
- No runtime state, traversal logic, services, combat logic, progression logic, or reward resolution logic was added.
- Region forward-output modeling, node unlock rules, character progression state, and gear ownership/equip state were intentionally left for later milestones because those are runtime/persistent-state concerns rather than static definition concerns.
- Character skill linkage remains a string reference because the skill data model is not implemented yet.
- Unity batch verification imported the new data-container folders and compiled `Survivalon.Runtime` plus `Survivalon.EditModeTests`, but `-runTests` still did not emit `Logs/m006_editmode_results.xml` or any test summary lines in `Logs/m006_editmode.log`.
