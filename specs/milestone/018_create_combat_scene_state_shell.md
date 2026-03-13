# Milestone 018 - Create combat scene/state shell

- Added a minimal combat shell under `Assets/Scripts/Combat` with `CombatShellContext`, `CombatShellParticipant`, `CombatSide`, and `CombatShellView`.
- Combat shell entry is driven from the existing run lifecycle: when a combat-compatible node enters `RunActive`, a combat context is initialized with one player-side participant and one enemy-side participant.
- `NodePlaceholderScreen` now shows a visible combat panel for combat-compatible nodes during the active run state while keeping non-combat node flow on the existing placeholder path.
- The combat shell uses thin MonoBehaviour wiring and plain C# context creation so later combat rules can replace the placeholder layer without redesigning the world-to-run flow.
- Added Edit Mode coverage for combat-shell lifecycle initialization, combat-shell UI visibility, participant side distinction, and world-map entry into a combat node shell.
- Verified with a real Unity batch Edit Mode run. `Logs/m018_editmode_results.xml` was produced and the run completed with `60` tests passed and exit code `0`.
- This milestone does not add targeting, attacks, damage, defeat handling, rewards, or a full combat HUD yet. It only proves combat context entry and side-distinct spawned participants.
