# Milestone 104 - Implement Offline Summary Claim Flow

## Goal
- Add the smallest honest offline-progress claim flow on top of the shipped Milestone 103 eligibility model.
- Keep rewards limited to conservative farming-support value only.

## Delivered
- Added `OfflineProgressClaimState` as the minimal resolved claim payload.
- Added `OfflineProgressClaimResolver` to turn:
  - persisted stable-save UTC anchor
  - persisted offline-progress eligibility kind
  - current authored world data
  - current persistent progression effects
  into one conservative offline-farming claim when the saved context is clearly eligible.
- Added `OfflineProgressClaimService` so claim application:
  - grants the resolved resource gain once
  - immediately persists the updated game state through the existing safe-resume save seam
  - refreshes the stable-save anchor so the same elapsed gap cannot be claimed twice on the next startup
- Updated `BootstrapStartup` so `Continue` now:
  - resolves the normal safe-resume target first
  - checks for a claimable offline summary only on that continue path
  - shows a compact claim surface before resuming when a claim exists
  - otherwise resumes directly as before
- Extended `StartupPlaceholderView` with one compact `Offline Summary` surface and one `Claim And Continue` action.

## Behavior Change
- The build now shows one compact offline-farming summary only when all of the following are true:
  - the persisted safe context is a world-map safe resume
  - the persisted offline eligibility kind is `FarmReadyWorldNode`
  - the saved anchor node still resolves through authored world data
  - at least one whole offline hour has elapsed since the stable-save anchor
  - the saved node supports ordinary region-material output
- Current offline rewards are intentionally narrow:
  - `Region material` only
  - whole-hour counting only
  - capped at 8 counted hours
  - per-hour value is derived from the existing authored ordinary region-material reward plus any authored node yield bonus and the shipped farm-yield progression bonus when owned
- Claim applies the gain once, persists immediately, and prevents an immediate double-claim on the next startup.
- No summary interrupts the player when:
  - the safe context is town/service
  - the safe context is boss/gate, unresolved, ambiguous, or not farm-ready
  - less than one whole offline hour has elapsed
  - the saved node does not support ordinary region-material output

## Tests
- Added `OfflineProgressClaimResolverTests` to cover:
  - successful eligible claim resolution
  - whole-hour cap behavior
  - progression bonus inclusion
  - fail-closed behavior for short gaps and unsupported saved nodes
- Added `OfflineProgressClaimServiceTests` to cover:
  - one-time claim application
  - immediate persistence
  - stable-save anchor refresh
  - no immediate double-claim
- Updated `BootstrapStartupScreenFlowTests` to cover:
  - offline summary shown before eligible continue resume
  - claim applying through the startup flow
  - no repeat summary immediately after claim

## Out Of Scope
- Continuous offline simulation while the game is open.
- Town/service offline rewards.
- Boss/gate or unresolved-run offline rewards.
- Offline progression materials, gear, unlocks, or character growth.
- Inbox/history UI, timer dashboards, or broader offline-economy systems.

## Verification
- Requested spec path note:
  - `specs/09_progression/offline_progress.md` does not exist in this repo
  - the governing equivalent used here was `specs/08_interface/saving_persistence_offline_progress.md`
- Compile/import check:
  - `powershell -NoLogo -NoProfile -File "tools/unity_compile_check.ps1"`
  - passed
  - log: `C:\IT_related\myGame\Survivalon\Logs\unity_compile_check.log`
- Standard EditMode verification helper:
  - `powershell -NoLogo -NoProfile -File "tools/unity_editmode_verify.ps1"`
  - reproduced the known missing-results helper artifact
  - expected results file was not created: `C:\IT_related\myGame\Survivalon\Logs\editmode_results.xml`
- Direct Unity batch fallback:
  - `C:\Program Files\Unity\Hub\Editor\6000.3.10f1\Editor\Unity.exe -batchmode -nographics -projectPath C:\IT_related\myGame\Survivalon -runTests -runSynchronously -testPlatform EditMode -testResults C:\IT_related\myGame\Survivalon\Logs\m104_editmode_results.xml -logFile C:\IT_related\myGame\Survivalon\Logs\m104_editmode.log`
  - passed: `662 passed`, `0 failed`, `0 inconclusive`, `0 skipped`
  - artifacts:
    - `C:\IT_related\myGame\Survivalon\Logs\m104_editmode_results.xml`
    - `C:\IT_related\myGame\Survivalon\Logs\m104_editmode.log`
