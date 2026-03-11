# Milestone 008 - Create Game Bootstrap Flow

## Implemented
- Added a thin startup-flow resolver that chooses the initial entry target from persistent world context.
- Updated the bootstrap scene entry script to resolve startup target and activate a runtime placeholder view.
- Added Edit Mode tests covering the startup entry decision logic.

## Changed at a high level
- Expanded the runtime assembly with one startup target enum, one resolver class, and one placeholder MonoBehaviour.
- Kept scene wiring thin by reusing the existing bootstrap scene and creating the placeholder target object at runtime when needed.

## Enabled now
- Game startup now follows a controlled entry path instead of only logging bootstrap initialization.
- The flow is prepared to route into a world-level placeholder when a resumable persistent world context exists later, while currently falling back to a compact main-menu placeholder target.

## Important limitations
- No real main menu, no real world map, and no save/load infrastructure were added in this milestone.
- The startup flow currently uses a default empty `PersistentGameState`, so runtime startup enters the main-menu placeholder until persistent resume context is wired in a later milestone.
- Placeholder entry is represented by a runtime-created child object with logging rather than a full UI screen.
- Unity batch verification compiled `Survivalon.Runtime` and `Survivalon.EditModeTests`, but `-runTests` still did not emit `Logs/m008_editmode_results.xml` or any test summary lines in `Logs/m008_editmode.log`.
