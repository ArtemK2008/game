# Milestone 073a - Cleanup Post-Run Next-Action Recommendation Boundaries

## Summary
- Kept post-run behavior and player-facing actions unchanged.
- Split the growing post-run recommendation logic into smaller focused helpers so replay reasoning, push-target resolution, and service-hub opportunity detection no longer live inside one expanding policy class.
- Tightened the meaning of `forward target` so service hubs are not treated as push targets.

## What Changed
- Extracted focused helpers under `Assets/Scripts/Run/`:
  - `PostRunReplayOpportunityResolver`
  - `PostRunForwardOpportunityResolver`
  - `PostRunServiceOpportunityResolver`
- Added small resolved helper-state models:
  - `PostRunForwardOpportunityState`
  - `PostRunServiceOpportunityState`
- Reduced `PostRunNextActionResolver` to orchestration:
  - combine replay, forward, and service opportunity inputs
  - choose the final recommended action kind
  - build `PostRunNextActionState`
- Made forward-push resolution explicitly ignore service/progression nodes:
  - push guidance now points only at actual forward content/progression targets
  - service-hub value remains its own separate recommendation/input

## Shipped Behavior After 073a
- The post-run panel still shows the same compact next-action block and the same replay / return / stop actions.
- Combined cases still read clearly:
  - push guidance points to a real forward target
  - service guidance points to `Cavern Service Hub`
  - when both exist, the return guidance still mentions both paths clearly
- No combat, reward, persistence, town, or navigation behavior changed.

## Intentionally Not Changed
- No new post-run buttons or flows
- No new progression/economy mechanics
- No new persistence rules
- No Milestone 074+ work

## Verification
- Compile/import first:
  - `tools/unity_compile_check.ps1`
  - log: `Logs/unity_compile_check.log`
- EditMode verification:
  - `tools/unity_editmode_verify.ps1`
  - the known helper artifact issue reproduced again and did not create `Logs/editmode_results.xml`
- Fallback verification:
  - waited Unity batch EditMode run writing:
    - `Logs/m073a_editmode_results.xml`
    - `Logs/m073a_editmode.log`
- Final result:
  - `493 passed`
  - `0 failed`
