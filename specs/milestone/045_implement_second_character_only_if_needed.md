# Milestone 045 - Implement Second Character Only If Needed

## Decision
- A second character was needed for this milestone.
- Before this milestone, the branch already had:
  - one persistent playable character
  - minimal character selection handling
  - one simple character-linked progression path
- That was not yet enough to prove that character identity changed gameplay in the current prototype.
- The smallest honest way to prove character differentiation without speculative systems or new external assets was to add exactly one second playable character with a clearly different combat baseline.

## Delivered
- Added one second shipped playable character through the existing persistent-character path:
  - `character_striker` / `Striker`
  - combat entity id: `player_striker`
  - default skill package id: `skill_package_striker_default`
- Kept the roster intentionally small:
  - `Vanguard` remains the default sturdier baseline
  - `Striker` is now the single offense-leaning alternative
- Extended the shipped playable-character catalog so both characters now exist as first-class playable profiles.
- Extended persistent playable-character initialization so bootstrap fallback state creation and persisted-state normalization both ensure:
  - both shipped characters exist
  - both are unlocked and selectable
  - one valid character remains active
  - an already valid persisted selection is preserved
- Reused the existing placeholder selection path instead of adding a broader character UI:
  - the world map continues to show one small button per selectable playable character
  - selection now has meaningful prototype-level gameplay consequences because future run entry can resolve either `Vanguard` or `Striker`
- Preserved the existing character-linked progression seam:
  - character rank remains stored per character
  - selecting `Striker` applies future rank growth to `Striker`, not `Vanguard`
- Proved meaningful runtime differentiation in the current combat prototype:
  - `Striker` enters combat with different baseline stats than `Vanguard`
  - the current boss/gate placeholder encounter that still defeats baseline `Vanguard` can now be cleared by baseline `Striker`

## Tests
- Updated `PersistentPlayableCharacterInitializerTests` to verify both shipped characters are seeded and that a valid existing selection is preserved.
- Updated `BootstrapStartupStateFactoryTests` to verify startup fallback creation and persisted-state startup normalization now produce the two-character shipped roster cleanly.
- Updated `PlayableCharacterSelectionServiceTests` to verify:
  - two known selectable playable characters are surfaced
  - invalid active state normalizes to one valid selection
  - explicit selection can switch from `Vanguard` to `Striker`
  - unknown selection ids are still rejected cleanly
- Updated `PlayableCharacterResolverTests` to verify the active second character resolves into the correct combat identity and baseline stats.
- Updated `WorldMapScreenPresentationTests` and `WorldMapScreenUiSetupTests` to verify the placeholder world-map selection surface now reflects a two-character roster and can switch the current selected character.
- Updated `BootstrapStartupCombatScreenFlowTests` to verify selecting `Striker` on the world map changes future combat run entry.
- Updated `CombatEntityStateTests` to verify combat shell setup can resolve the second character into the player-side combat entity.
- Updated `RunLifecycleControllerCombatTests` to verify:
  - selected-character run entry resolves `Striker` correctly
  - `Striker` progression rank increases without mutating inactive `Vanguard`
  - the current boss placeholder encounter fails on `Vanguard` and succeeds on `Striker`
- Updated `SafeResumePersistenceServiceTests` to verify two-character persistent state survives resolved-world snapshot saving.

## Intentionally Left Out
- Any third character
- A dedicated character-selection screen
- Character portraits, animation, audio, or other external character assets
- Character-specific talent trees or broader progression branches
- Skill-system redesign
- Loadout or gear redesign
- Any Milestone 046 or later work

## Verification
- `tools/run_editmode_tests.ps1` should still be run first for milestone verification.
- If that shell returns before Unity finishes again, verification should fall back to the direct waited Unity batch invocation and report that explicitly.
