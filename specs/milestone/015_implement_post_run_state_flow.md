# Milestone 015 - Implement post-run state flow

## Delivered
- Added a minimal post-run summary flow on top of the run lifecycle shell, centered on `PostRunStateController` and a visible post-run summary panel inside `NodePlaceholderScreen`.
- The post-run summary now communicates that the run is finished, shows the resolved run result context, and surfaces the three next actions required at this milestone: replay, return to world, and stop.
- Replay now restarts the placeholder run shell for the same node without leaving the node screen.
- Return to world now exits the post-run panel back to the world map through `BootstrapStartup`.
- Stop now exits the post-run panel to the existing startup placeholder as a minimal safe session-end target, without introducing save/load or a broader menu system.

## Tests
- Added Edit Mode coverage for post-run action availability, replay reset behavior, return-to-world flow, stop flow, and bootstrap routing after stop.

## Out Of Scope
- This milestone does not add a real reward screen, persistent reward application, save/load, or final menu UX. It only proves the minimal readable post-run decision state and its three explicit actions.
