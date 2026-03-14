# Milestone 026 - Implement Movement If Movement Exists In MVP

## Decision
- No movement system was added in this milestone.
- The current MVP combat model does not require distance, range, lane, or positioning logic yet.

## Why Movement Was Intentionally Omitted
- The relevant specs treat movement as conditional, not mandatory:
  - `specs/04_combat/stats_and_formulas.md` only requires movement stats and movement-related timing **if movement exists in MVP**
  - `specs/04_combat/combat_rules.md` only requires automated movement **if movement exists in MVP**
  - `specs/04_combat/automation_ai_behavior.md` allows a simple movement model but does not force movement when the chosen MVP combat space does not need range closure
- The current implementation has no combat-space concept that would require movement:
  - `CombatShellContext` contains only node id plus player/enemy entities
  - `CombatEntityState` and `CombatEntityRuntimeState` contain no position, range, lane, or approach state
  - `CombatStatBlock` has no movement or range fields in the current MVP stat set
  - `CombatAutoTargetSelector` and `CombatEncounterResolver` resolve combat directly through valid target selection and attack timing, with no engagement-distance gate

## Current MVP Model
- The chosen MVP combat model is a deterministic direct-engagement autobattle loop:
  - valid hostile target exists
  - attacks resolve on timing
  - damage and defeat resolve directly
  - run resolves without any manual movement input
- Adding movement at this stage would introduce speculative combat-space complexity rather than satisfy an actual MVP requirement.

## Tests
- No new tests were added.
- Existing combat tests already verify that the current MVP combat loop resolves successfully without movement-specific state or input.

## Verification
- Unity EditMode batch run passed:
  - `Logs/m026_editmode_results.xml`
  - `Logs/m026_editmode.log`
