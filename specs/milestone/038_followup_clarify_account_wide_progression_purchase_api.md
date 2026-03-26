# Milestone 038 Follow-Up - Clarify Account-Wide Progression Purchase API

## Delivered
- Clarified the account-wide progression sink API so resource availability and actual purchase eligibility are no longer conflated.
- Renamed the resource-only check from `CanAfford(...)` to `HasRequiredResources(...)`.
- Added `CanPurchase(...)` for the real buyability rule:
  - the upgrade must not already be purchased
  - and the required resources must be available
- Kept runtime behavior unchanged; this is an API-clarity fix inside the existing Milestone 038 sink.

## Tests
- Updated `AccountWideProgressionBoardServiceTests` to verify:
  - resource-only availability
  - true purchase eligibility
  - already-purchased upgrades are not buyable
  - insufficient resources are not buyable

## Out Of Scope
- Any Milestone 039 combat integration
- Additional upgrade entries
- UI or service-layer access changes

## Verification
- Unity EditMode batch run passed for the follow-up fix.
