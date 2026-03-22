# Milestone 065a - fix town conversion persistence wiring

## Goal
Fix the missing live persistence injection risk in the town conversion flow and harden tests so they verify real persistence instead of passing through shared in-memory object references.

## Decision
- Keep player-facing town conversion behavior exactly the same.
- Fix the real startup wiring by explicitly injecting a persistence-backed `TownServiceConversionInteractionService`.
- Tighten startup and isolated conversion tests so they:
  - track real save-call count
  - use cloned persisted snapshots
  - fail if conversion mutates only runtime memory without saving

## Delivered
- Updated `BootstrapStartup` so `TownServiceScreen` now receives:
  - `conversionInteractionService: new TownServiceConversionInteractionService(persistenceService)`
- Hardened the shared startup flow storage double to:
  - clone seeded state
  - clone returned loaded state
  - clone saved state
  - count explicit save calls
- Hardened isolated town conversion test storage doubles with the same snapshot/save-count behavior.
- Updated the real startup -> town conversion flow test so it proves:
  - no save happened before conversion
  - conversion triggered exactly one save
  - persisted stored balances changed only after that save

## Behavior
- No player-facing behavior changed.
- `Cavern Service Hub` still shows the same conversion and town sections.
- Successful `Region material x3 -> Persistent progression material x1` conversion still refreshes immediately and now has explicit real startup persistence wiring behind it.
- Existing progression purchase, build-prep, return, and stop flows remain unchanged.

## SRP notes
- SRP improved in the touched area.
- `BootstrapStartup` now only performs the dependency injection it should have performed already.
- `TownServiceConversionInteractionService` remains the owner of town conversion orchestration and immediate persistence handoff.
- `TownServiceScreen` remains UI wiring and refresh only.
- The stricter storage doubles keep persistence verification inside tests instead of leaking storage shortcuts into behavior assertions.

## Verification
- Followed the mandatory compile/import-first workflow from `AGENTS.md`.
- Ran:
  - `tools/unity_compile_check.ps1`
  - log: `Logs/unity_compile_check.log`
- Then ran:
  - `tools/unity_editmode_verify.ps1`
  - compile/import passed again, but the helper still failed to create its expected `Logs/editmode_results.xml` artifact in this shell
- Fallback verification used a direct waited Unity batch EditMode run.
- Final EditMode result:
  - `453 passed`
  - `0 failed`
- Artifacts:
  - `Logs/unity_compile_check.log`
  - `Logs/m065a_editmode_results.xml`
  - `Logs/m065a_editmode.log`

## Intentionally left out
- No Milestone 066+ work
- No town UI expansion
- No conversion recipe expansion
- No broader persistence-architecture refactor
