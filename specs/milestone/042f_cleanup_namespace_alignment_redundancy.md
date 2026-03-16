# Milestone 042f - Cleanup Namespace Alignment Redundancy

## Delivered
- Removed the broad redundant `using` directives that had been added during the namespace-alignment refactor.
- Kept the cleanup behavior-preserving:
  - no folder moves
  - no namespace changes
  - no asmdef changes
- Deleted a small set of genuinely unused runtime members:
  - the one-argument `CombatShellContextFactory.Create(...)` overload
  - the two-argument `CombatShellContextFactory.Create(...)` overload
  - `PlayableCharacterCatalog.All`
  - `RunRewardResourceCategoryRules.IsCurrencyCategory(...)`
  - `RunRewardResourceCategoryRules.IsMaterialCategory(...)`
- Updated the affected tests to stop depending on those test-only runtime convenience members.

## Tests Updated
- `CombatEntityStateTests`
- `CombatShellPresentationTests`
- `RunProgressResolutionServiceTests`
- `NodePlaceholderScreenPresentationTests`

## Intentionally Preserved For Safety
- Unity lifecycle methods, serialized members, assembly markers, and the cross-domain smoke-test exception
- Any runtime members whose usage could be indirect through Unity wiring or meaningful runtime contracts
- Ambiguous public/runtime-facing APIs that are not clearly dead

## Verification
- Unity EditMode batch run passed.
- Result: `288` passed, `0` failed.
- Artifacts:
  - `Logs/namespace_cleanup_editmode_results.xml`
  - `Logs/namespace_cleanup_editmode.log`
