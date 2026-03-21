# Milestone 049c - Restore World Map Scroll After Package Assignment

## Goal
- Fix the placeholder world-map usability regression introduced after the package-assignment section was added, without changing world-map behavior or starting Milestone 050 work.

## Delivered
- Restored usable remaining height for the node-list `ScrollRect` in `WorldMapScreen`.
- Kept the existing placeholder structure intact:
  - title and summary remain at the top
  - character selection remains visible
  - package assignment remains visible
  - node selection and entry flow remain unchanged
- Compact choice-row layout now keeps the small character-selection and package-assignment button groups from consuming unnecessary vertical space.
- Tightened the local layout rebuild order so the panel and node-list scroll viewport recalculate correctly after dynamic character/package button refreshes.

## Cause
- The package-assignment section pushed the fixed-height stack above the node list past the available panel height budget.
- That left the node-list scroll viewport with too little usable remaining height, and the existing refresh pass did not fully restabilize the scroll-view layout after dynamic button rebuilds.

## Tests
- Updated `WorldMapScreenUiSetupTests` to verify:
  - the world map still creates a contained scrollable node-list viewport after the package-assignment UI is present
  - the viewport remains tall enough to show a full node button
  - overflowing node content stays reachable after character-selection and package-assignment refreshes
  - node buttons and labels remain horizontally contained and readable inside the viewport

## Blur Check
- No code-side runtime blur cause was identified in the placeholder world-map UI path.
- No font, sprite, or other UI asset changes were made for this follow-up.

## Intentionally Left Out
- Any redesign of the world-map placeholder
- Broader UI polish or visual cleanup
- Any Milestone 050 or later work
