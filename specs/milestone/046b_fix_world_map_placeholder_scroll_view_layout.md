# Milestone 046b - Fix World Map Placeholder Scroll View Layout

## Goal
- Correct the follow-up world-map placeholder scroll-view layout so the node list stays horizontally aligned inside the viewport.
- Preserve the existing placeholder structure and world-map behavior while fixing left-side clipping and unstable horizontal positioning.

## Delivered
- Normalized the scroll-content rect in `WorldMapScreen` so the node-list content is explicitly kept at full viewport width with zero horizontal offset.
- Kept vertical scrolling behavior unchanged.
- Updated dynamically created node buttons so they stretch cleanly across the available node-list width.
- Ensured layout refresh now re-applies the node-list content width/alignment normalization after dynamic rebuilds, which keeps the placeholder layout stable after both `Show()` and later `Refresh()` calls.
- Preserved existing behavior:
  - node selection is unchanged
  - character selection is unchanged
  - entry-button behavior is unchanged
  - node ordering and label content are unchanged

## Tests
- Updated `WorldMapScreenUiSetupTests` to verify:
  - scroll content aligns horizontally with the viewport
  - node buttons and their labels remain horizontally contained within the viewport
  - horizontal alignment remains stable after a `Refresh()` triggered by node selection
  - vertical scrolling still exposes overflowing lower node buttons

## Out Of Scope
- Any redesign of the placeholder world map
- Additional polish such as custom scrollbars or visual styling work
- Changes to world-map behavior, flow, or interaction logic
- Any Milestone 047 work

## Verification
- Run batch Edit Mode tests with `tools/run_editmode_tests.ps1`.
- If that shell returns before Unity finishes, use the direct waited Unity batch invocation and report that explicitly.
