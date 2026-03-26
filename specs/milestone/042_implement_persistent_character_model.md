# Milestone 042 - Implement Persistent Character Model

## Delivered
- Added one explicit playable character baseline to the current build:
  - character id: `character_vanguard`
  - display name: `Vanguard`
  - combat entity id: `player_main`
  - baseline combat stats aligned with the existing prototype player baseline
- Added small runtime character-model support:
  - `PlayableCharacterProfile` for runtime identity + baseline stats
  - `PlayableCharacterCatalog` for the current single shipped character
  - `PlayableCharacterResolver` for resolving the current active persistent character
- Extended persistent game state with explicit character-state storage helpers.
- Added `PersistentPlayableCharacterInitializer` so:
  - bootstrap fallback game-state creation seeds the default playable character
  - persisted older save states are normalized to contain the default playable character
- Connected run entry / combat setup so future runs now use the resolved persistent playable character baseline.
- Preserved the current prototype combat numbers by giving the first character the same baseline stats the anonymous player baseline used previously.
- Preserved the existing account-wide progression sink behavior by applying those effects on top of the character-backed baseline during combat setup.

## Tests
- Added `PersistentPlayableCharacterInitializerTests` to verify default-character seeding and non-duplication behavior.
- Added `PlayableCharacterResolverTests` to verify:
  - active character resolution
  - fallback resolution when no character is marked active
  - rejection when no playable character exists
- Updated `BootstrapStartupStateFactoryTests` to verify startup-created and loaded game states include the default persistent playable character.
- Updated `CombatEntityStateTests` to verify combat context creation now uses the character-backed player identity and stats.
- Updated `RunLifecycleControllerCombatTests` to verify:
  - run entry uses the persistent playable character
  - account-wide progression effects still apply correctly on top of the persistent character baseline
- Updated `BootstrapStartupCombatScreenFlowTests` and `NodePlaceholderScreenCombatUiTests` so the current runtime combat UI assertions reflect the named playable character.
- Updated `CombatShellPresentationTests` so combat-shell summary/card expectations reflect the named playable character.
- Updated `SafeResumePersistenceServiceTests` to verify persistent playable character state survives resolved-world save snapshots.

## Out Of Scope
- Character selection UI
- A second playable character
- Character-specific progression trees
- Character equipment/loadout gameplay
- Skill-package runtime behavior
- Any Milestone 043 or later character-system expansion

## Verification
- Unity EditMode batch run passed.
- Result: `288` passed, `0` failed.
- Artifacts:
  - `Logs/m042_editmode_results.xml`
  - `Logs/m042_editmode.log`
