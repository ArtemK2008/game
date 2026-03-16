# Milestone 042g - Remove Test-Only Runtime Accessors

## Delivered
- Removed a very small set of clearly dead runtime-visible convenience accessors that were only referenced by EditMode tests:
  - `StartupPlaceholderView.ActiveTarget`
  - `PostRunStateController.NodeContext`
  - `RunLifecycleController.HasCombatContext`
- Updated the affected tests to assert the same behavior through real runtime state instead of those convenience accessors.
- Kept the cleanup behavior-preserving:
  - no gameplay changes
  - no folder changes
  - no namespace changes
  - no asmdef changes

## Why It Was Safe
- `StartupPlaceholderView.ActiveTarget`
  - had no code references outside a single EditMode test
  - was not serialized
  - was not a Unity lifecycle entry point
  - was not used by scene, prefab, button, or inspector wiring
  - was not intentionally preserved as a meaningful runtime contract because runtime flow already exposes the shown target through the view name/text set by `Show(...)`
- `PostRunStateController.NodeContext`
  - had no code references outside a single EditMode test
  - was on a plain C# controller, not a Unity-wired type
  - was not part of a required runtime contract because replay behavior already uses the private stored node context internally
- `RunLifecycleController.HasCombatContext`
  - had no code references outside a single EditMode test
  - was not serialized and not Unity-wired
  - was not part of a required runtime contract because callers that need the real combat context already use `CombatContext`

## Tests Updated
- `BootstrapStartupScreenFlowTests`
- `PostRunStateControllerTests`
- `RunLifecycleControllerCombatTests`

## Intentionally Preserved For Safety
- Unity lifecycle methods, serialized members, assembly markers, and smoke-test exceptions
- Public/runtime-facing APIs whose usage could still be indirect through Unity wiring or meaningful runtime contracts
- Ambiguous controller/view accessors that are still used by real runtime flow

## Verification
- Unity EditMode batch run passed.
- Result: `288` passed, `0` failed.
- Artifacts:
  - `Logs/dead_code_cleanup_editmode_results.xml`
  - `Logs/dead_code_cleanup_editmode.log`
