# Milestone 038 - Implement One Account-Wide Progression Sink

## Delivered
- Added one persistent account-wide progression sink using `PersistentProgressionState`.
- Implemented a small account-wide upgrade board service with one upgrade/project:
  - it consumes `PersistentProgressionMaterial`
  - it can be afforded, purchased once, and rejected cleanly when resources are insufficient
  - it records purchased state persistently as an account-wide progression entry
- Implemented a small account-wide progression effect resolver so the purchased upgrade produces a real permanent benefit in data/model terms.
- Kept the sink structurally separate from combat integration:
  - the purchased upgrade currently resolves into an account-wide combat-baseline effect model
  - wiring that effect into actual combat outcomes is deferred to Milestone 039
- Kept town/service presentation out of scope for this milestone:
  - the sink now exists structurally and persistently
  - dedicated runtime access through a service/town UI remains deferred to later town/service milestones

## Tests
- Added `AccountWideProgressionBoardServiceTests` to verify:
  - upgrade affordability
  - successful purchase and resource spend
  - rejection when resources are insufficient
  - rejection of duplicate repurchase
- Added `AccountWideProgressionEffectResolverTests` to verify:
  - default empty-effect state
  - applied account-wide effect after purchase
  - null guard behavior
- Updated `SafeResumePersistenceServiceTests` to verify purchased account-wide upgrade state and its resolved applied effect survive the existing safe persistence boundary.

## Intentionally Left Out
- Runtime combat integration of the new account-wide effect
- Additional upgrade entries or a broader upgrade board
- Dedicated town/service UI access to the sink
- Character/build-specific progression sinks
- Broader economy redesign or additional spend systems

## Verification
- Unity EditMode batch run passed for the milestone implementation.
