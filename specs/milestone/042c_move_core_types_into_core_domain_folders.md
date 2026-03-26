# Milestone 042c - Move Core Types Into Core Domain Folders

## Delivered
- Created `Assets/Scripts/Core/` for shared cross-domain identifiers, enums, and lightweight value-like types.
- Moved these runtime files from the script root into `Assets/Scripts/Core/`:
  - `NodeId`
  - `RegionId`
  - `NodeState`
  - `NodeType`
  - `ProgressionLayerType`
  - `ResourceCategory`
  - `RewardCategory`
  - `RewardSourceCategory`
- Created `Assets/Tests/EditMode/Core/` and moved the matching ownership test:
  - `CoreIdentifierAndCategoryTests`
- Preserved Unity asset integrity by moving each `.meta` file with its script.
- Kept namespaces unchanged in this milestone so the refactor stayed purely structural and low-risk.

## Why This Improves Readability
- The script root now contains fewer ordinary domain types.
- Shared cross-domain primitives are easier to find in one predictable `Core` location.
- The matching test now lives in the mirrored `Core` test folder, so runtime ownership and test ownership are easier to follow together.

## Tests
- No new behavior tests were needed because this was a behavior-preserving folder-placement refactor.
- Existing EditMode coverage remained in place and was validated through the batch EditMode run.

## Out Of Scope
- Runtime namespaces for the moved core types
- Other domain test files still sitting at the EditMode root
- Broader runtime domain moves for `World`, `Run`, `State`, `Combat`, and `Data`
- Any asmdef or assembly-boundary changes

## Verification
- Unity EditMode batch run passed.
- Result: `288` passed, `0` failed.
- Artifacts:
  - `Logs/core_folder_structure_refactor_editmode_results.xml`
  - `Logs/core_folder_structure_refactor_editmode.log`
