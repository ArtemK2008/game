# Milestone 033 - Implement Reward Payload Model

## Delivered
- Replaced the flat placeholder run reward payload shape with a structured `RunRewardPayload`.
- Added explicit reward entry types for:
  - currency rewards
  - material rewards
- Kept the current run result flow wired through the structured reward payload model.
- Kept actual reward assignment placeholder-level for now:
  - resolved runs still use `RunRewardPayload.Empty`
  - reward values and granting are deferred to later economy milestones

## Tests
- Added `RunRewardPayloadTests` to verify:
  - structured payloads hold currency rewards
  - structured payloads hold material rewards
  - `RunResult` carries the structured payload
  - invalid category usage is rejected
- Updated `RunResultFactoryTests` to verify the default run result still carries an empty structured payload.
- Updated `RunLifecycleControllerTests` to verify the current run flow still produces a valid structured reward payload.

## Intentionally Left Out
- Actual reward generation values
- Soft-currency granting
- Material granting
- Reward persistence into balances or inventory
- Post-run reward summary UI redesign
- Gear loot
- Milestone reward differentiation

## Verification
- Unity EditMode batch run passed:
  - `Logs/m033_editmode_results.xml`
  - `Logs/m033_editmode.log`
