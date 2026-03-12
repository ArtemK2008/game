# Milestone 014 - Implement run lifecycle shell

- Added an explicit run lifecycle model with `RunLifecycleState` covering `RunStart`, `RunActive`, `RunResolved`, and `PostRun`.
- Added a minimal `RunResult` packet plus `RunRewardPayload` and `RunNextActionContext` so resolved runs now produce a dedicated result shape even though combat, rewards, and progression deltas are still placeholder-level.
- Added `RunLifecycleController` as the plain C# state-transition layer for the current placeholder node flow.
- Evolved `NodePlaceholderScreen` into a run-lifecycle shell UI: it now advances from run start -> active -> resolved -> post-run, then returns to the world map with the produced `RunResult`.
- Updated `BootstrapStartup` so returning from the placeholder run happens from post-run state rather than directly from an ad hoc placeholder screen.
- Added Edit Mode coverage for lifecycle state transitions, run-result creation, placeholder-screen lifecycle progression, and bootstrap enter/return flow through the explicit run shell.
- This milestone does not add combat, reward generation, persistent reward application, node-progress updates, or post-run polish. It only proves the explicit run-state flow and result handoff needed before later run/combat work.
