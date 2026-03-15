# Milestone 039 Follow-Up - Reduce Run Persistent Context Parameter Growth

## Delivered
- Added `RunPersistentContext` as a small value object grouping the persistent run-entry state used across the combat-entry flow:
  - `PersistentWorldState`
  - `ResourceBalancesState`
  - `PersistentProgressionState`
- Updated the Milestone 039 combat-entry path to use that grouped context through:
  - `BootstrapStartup`
  - `NodePlaceholderScreen`
  - `PostRunStateController`
  - `RunLifecycleController`
- Kept runtime behavior unchanged:
  - combat baseline integration from the purchased account-wide upgrade still works exactly as before
  - replay, reward application, progression updates, and post-run flow remain unchanged

## Tests
- Updated replay and combat lifecycle tests to pass the grouped persistent context instead of the previous parallel parameter lists.
- Kept the existing Milestone 039 combat-outcome coverage intact.

## Intentionally Left Out
- Any new progression behavior
- Additional run-context grouping outside the touched Milestone 039 flow
- Milestone 040 and later work

## Verification
- Unity EditMode batch run passed for the follow-up refactor.
