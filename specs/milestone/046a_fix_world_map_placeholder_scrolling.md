# Milestone 046a - Fix World Map Placeholder Scrolling

## Goal
- Restore full reachability of the current placeholder world map UI after recent vertical growth made lower node buttons unreachable.
- Keep the change small, behavior-preserving, and scoped only to the active world-map placeholder screen.

## Delivered
- Added a minimal scrollable node-list viewport to `WorldMapScreen`.
- Kept the existing placeholder structure intact:
  - title/header remains readable at the top
  - summary text remains visible
  - character selection remains visible
  - entry button remains visible
  - node buttons remain in the same placeholder list format
- Moved only the node list into a `ScrollRect`-backed viewport so long lists can be scrolled instead of overflowing past the visible panel.
- Preserved existing world-map behavior:
  - node selection logic is unchanged
  - character selection logic is unchanged
  - entry-button behavior is unchanged
  - node-button presentation and ordering are unchanged
- Added an explicit layout refresh pass after rebuilding dynamic character and node buttons so the placeholder UI recalculates content size correctly in EditMode/runtime setup.

## Tests
- Updated `WorldMapScreenUiSetupTests` to verify:
  - the world map now creates a vertical node-list `ScrollRect`
  - overflowing node content becomes reachable by scrolling the viewport
  - the existing world-map controls still render and behave correctly

## Out Of Scope
- Any redesign of the placeholder world map
- Scrollbars, polish animation, or visual cleanup beyond the usability fix
- Changes to world-map behavior, node flow, or character-selection behavior
- Any Milestone 047 work

## Verification
- Run batch Edit Mode tests with `tools/run_editmode_tests.ps1`.
- If that shell returns before Unity finishes, use the direct waited Unity batch invocation and report that explicitly.
