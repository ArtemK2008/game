# Milestone 016 - Implement safe stopping/resume baseline

- Added a minimal persistence baseline for safe stop/resume using `SafeResumePersistenceService` and `FilePersistentGameStateStorage`.
- Added `PersistentSafeResumeState` and `SafeResumeTargetType` so persisted game state now records an explicit world-level safe resume target instead of relying on unresolved run context.
- `BootstrapStartup` now loads persisted game state on startup and saves resolved world-level context when the player returns to the world map or stops from post-run.
- The persistence boundary stays explicit: only persistent game state is saved, and the save point is the resolved post-run world-level context.
- Temporary run state and unresolved node flow are not persisted as a safe resume snapshot.
- Added Edit Mode coverage for persisted safe-resume markers, saved-state reload behavior, startup routing from saved safe-resume context, and post-run return/stop persistence updates.
- This milestone does not add offline progress, mid-run suspend/resume, multiple save slots, or broader profile/save management.
