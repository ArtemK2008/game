# Milestone 039 - Connect Progression Sink To Combat Outcomes

## Delivered
- Wired the existing account-wide progression effect into the live combat setup path.
- The current purchased account-wide upgrade now applies its resolved max-health bonus to the player-side combat baseline before future combat runs start.
- Kept the integration small and local:
  - enemy combat baseline stats remain unchanged
  - reward generation, progression rules, and purchase rules remain unchanged
  - replay and world-entry combat flows both reuse the same persisted progression effect

## Tests
- Updated `CombatEntityStateTests` to verify the combat shell context factory applies the purchased account-wide max-health bonus to the player side only.
- Updated `RunLifecycleControllerCombatTests` to verify:
  - the no-upgrade baseline remains unchanged
  - the purchased upgrade increases player max health
  - the purchased upgrade produces a visible survivability improvement in future combat runs
- Updated `BootstrapStartupCombatScreenFlowTests` to verify the purchased upgrade affects real runtime combat entry and replay flow through the existing bootstrap/world/node screen path.

## Intentionally Left Out
- Additional account-wide upgrade entries
- Combat integration for any future progression-sink effects beyond the current max-health bonus
- Dedicated town/service UI access to the progression sink
- Milestone 040 and later progression-system expansion

## Verification
- Unity EditMode batch run passed for the milestone implementation.
