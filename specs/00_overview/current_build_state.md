# Current Build State

## Purpose
This file is a rolling summary of what is already implemented in the current build. It is intended as a compact handoff/reference for future Codex runs so they can see the current shipped prototype state without rereading the full milestone chain first.

## Completed milestone range
This summary reflects completed work through **Milestone 031**.

## Current playable loop
On startup, the bootstrap scene loads a persisted game state if one exists, otherwise it falls back to the bootstrap demo world state. Startup then routes into the world map safe context or a main-menu placeholder target depending on safe-resume state.

From the world map, the player manually selects a reachable node and confirms entry. Combat-compatible nodes then auto-start their run flow: combat begins automatically, auto-targeting and auto-attacks resolve the 1v1 encounter over time, the run resolves to success or failure, and the screen enters post-run automatically. At post-run, the player can replay the node, return to the world map, or stop the session. Non-combat placeholder nodes still use the simple placeholder run shell rather than real gameplay.

Manual actions currently required:
- select a node on the world map
- confirm node entry
- choose a post-run action after resolution

Manual movement, manual attacks, and manual combat stepping are not required in the current combat loop.

## Implemented systems

### Bootstrap / startup flow
- The project launches through `BootstrapScene` and `BootstrapStartup`.
- Startup resolves into a world-map safe context or a main-menu placeholder target.
- Safe-stop persistence exists for resolved world-level context; startup can reload that safe context.

### World map and node entry
- A runtime world graph exists with regions, nodes, explicit connections, and limited branching.
- Reachability supports forward movement and backtracking from current world context.
- The world map shows node identity, node type, node state, current/selectable status, and a small summary of current context.
- Entering a selected node routes into a placeholder node screen through explicit node-entry flow logic.

### Run lifecycle and post-run flow
- Runs use explicit lifecycle states: `RunStart`, `RunActive`, `RunResolved`, and `PostRun`.
- Post-run shows a compact summary and allows replay, return to world, or stop session.
- Replay re-enters the same node cleanly.
- Return/stop save a world-level safe resume context.

### Combat foundation
- Combat exists as a placeholder shell inside the current node/run flow rather than as a dedicated final combat scene.
- The combat prototype is currently one player-side entity versus one enemy-side entity.
- Combat entities have explicit allegiance, alive/active state, runtime health, and a small shared base stat model.
- Base stats currently include max health, attack power, attack rate, and defense-based survivability.

### Auto-battle / hostility / no-manual-combat loop
- Combat uses deterministic auto-targeting.
- Player and enemy both attack automatically on their own timing.
- Enemy hostility is explicit and can defeat the player.
- Defeated entities become inactive and stop contributing to combat.
- Combat auto-advances until one side wins, then the run resolves automatically into post-run.

### Persistent node progress
- Combat-oriented nodes have persistent per-node progress stored in `PersistentWorldState` / `PersistentNodeState`.
- In the current 1v1 prototype, a successful enemy defeat grants `+1` node progress.
- Progress can accumulate across multiple successful runs before a node reaches its clear threshold.
- Progress totals and per-run delta are surfaced minimally in the post-run summary.

### Map clear threshold behavior
- Tracked combat-oriented nodes become `Cleared` when persistent node progress reaches the node threshold.
- The current threshold rule is simple and kill-driven.
- Cleared state persists and is reflected on the world map because the world map reads persistent node state when present.

### Next-node unlock behavior
- When a tracked node reaches `Cleared`, directly connected locked next nodes become `Available`.
- The unlocked state is persisted in `PersistentWorldState`, not only in transient world-map view state.
- World-map reachability now respects persistent unlocked node state, so newly unlocked connected nodes become selectable after returning from the cleared node flow.

## Important current rules / constraints
- Combat is currently **1v1 only**.
- Movement is **not** part of the current MVP combat model.
- The combat shell is still a debug-style placeholder presentation, not a final combat HUD.
- `BossOrGate` currently shares the same tracked-progress rule and default threshold as ordinary combat nodes as a temporary MVP placeholder.
- Current unlock behavior is limited to direct connected-node unlock on clear; advanced branch and gate semantics are still deferred.
- Failed or incomplete combat runs do **not** currently grant node progress in the MVP, because node progress is still kill-driven and the single-enemy combat prototype has no failed partial-kill case.
- Rewards and economy are still placeholder-level; runs currently produce minimal reward/result structure without a real reward pipeline.
- Non-combat nodes still use placeholder run behavior.

## Not implemented yet
- Broader partial-completion outputs beyond the current 1v1 kill-driven MVP
- Structured reward economy beyond placeholder reward payloads
- Real persistent progression sinks and upgrade spending
- Expanded character/build systems beyond the current placeholders
- Multi-entity combat, skills, advanced AI, and broader combat content

## Known temporary placeholders / technical shortcuts
- The world graph and initial persistent state are currently seeded by `BootstrapWorldMapFactory`.
- The world map, node screen, post-run panel, and combat shell are runtime-generated placeholder UI.
- Main menu flow is still a placeholder target rather than a real menu system.
- Combat nodes use a minimal direct-engagement model with no movement, range, animation, or final presentation systems.
- Boss/gate node progress behavior is intentionally temporary and should be revisited in the later progression/boss milestones.

## Source note
This file is derived from the milestone notes in `specs/milestone/` plus the current codebase entry flow. It should be updated after each completed milestone so it remains the compact source for current implemented build state.
