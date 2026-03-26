# Refactor milestone 05 — dormant authoring-model cleanup

## Goal
Keep the refactor behavior-preserving while making the dormant ScriptableObject authoring-model slice explicit instead of leaving it mixed into active runtime-facing data folders.

## What proved the slice is dormant
- Code search found no runtime references to:
  - `CharacterDefinition`
  - `GearDefinition`
  - `RegionDefinition`
  - `NodeDefinition`
  - `NodeConnectionData`
  - `RewardDefinition`
- The only references were inside the old `BaseDataContainerTests` fixture.

## What changed
- Moved the dormant authoring-model classes into `Assets/Scripts/Prototype/AuthoringData/`.
- Updated their namespaces and CreateAssetMenu paths so they no longer look like active shipped data.
- Moved and renamed the matching EditMode test to:
  - `Assets/Tests/EditMode/Prototype/AuthoringData/PrototypeAuthoringDataContainerTests.cs`

## Why isolation instead of deletion
- The slice is clearly unused by the shipped runtime flow today.
- But because it is editor-facing ScriptableObject code, deletion would be a stronger assumption about future authoring plans.
- Isolation is the safest honest cleanup: the code remains available without implying active runtime ownership.

## BaseDataContainerTests decision
- The old `BaseDataContainerTests` name was too vague and made the dormant slice look like active shared data infrastructure.
- It was moved and renamed to `PrototypeAuthoringDataContainerTests` so test ownership matches the final isolated slice.
