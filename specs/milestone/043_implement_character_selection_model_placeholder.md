# Milestone 043 - Implement Character Selection Model Placeholder

## Delivered
- Added an explicit character-selection service layer on top of the existing persistent character state:
  - `PlayableCharacterSelectionService`
  - `PlayableCharacterSelectionOption`
- Kept the current roster intentionally small:
  - no second playable character was added
  - `character_vanguard` / `Vanguard` remains the only shipped playable character
- Made current-character handling explicit and extensible instead of relying on the previous implicit "Vanguard always" baseline:
  - selectable playable characters are resolved through one selection-focused service
  - the selected active character is normalized so only one valid playable character remains active
  - invalid or missing active selection falls back cleanly to the first valid selectable playable character
- Updated persistent-character initialization so startup normalization no longer hard-overrides future selection state:
  - the default playable character is still guaranteed to exist, be unlocked, and be selectable
  - active selection is now normalized through the new selection service rather than always forcing the default character active directly
- Added a minimal placeholder selection surface on the world map:
  - the world map now shows the currently selected playable character
  - the world map now shows one small character-selection button per selectable playable character
  - selecting a character updates the persistent active-character state used by future run entry
- Preserved current run-entry behavior:
  - combat entry still resolves the selected persistent playable character into the player-side combat baseline
  - with the current single-character roster, combat behavior remains unchanged while the selection path is now explicit

## Tests
- Added `PlayableCharacterSelectionServiceTests` to verify:
  - selectable known character options are built correctly
  - invalid active-character state normalizes cleanly
  - explicit character selection updates active state correctly
  - unknown character ids are rejected cleanly
- Updated `BootstrapStartupStateFactoryTests` to verify persisted startup state normalizes a missing active playable-character selection.
- Updated `BootstrapStartupCombatScreenFlowTests` to verify the world map shows the selected character placeholder and run entry still uses that selected character.
- Updated `WorldMapScreenPresentationTests` to verify the new character-selection summary/button text builders.
- Updated `WorldMapScreenUiSetupTests` to verify:
  - the world map now shows the selected character placeholder UI
  - the character-selection button is present
  - pressing the current character button keeps the persistent active character valid

## Intentionally Left Out
- A second playable character
- Character selection beyond the current placeholder world-map surface
- Character-specific progression trees
- Character skills/skill-package runtime behavior beyond the existing baseline data linkage
- Gear/build preparation UI
- Any Milestone 044 or later character-system expansion

## Verification
- `tools/run_editmode_tests.ps1` was invoked first, but in this shell it returned before waiting on the Unity editor process and did not produce artifacts.
- Verified the same EditMode batch invocation directly with Unity using a waited process:
  - `Logs/m043_editmode_results.xml`
  - `Logs/m043_editmode.log`
- Result: `299` passed, `0` failed.
