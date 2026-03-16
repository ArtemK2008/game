# Current Build State

## Purpose
This file is a rolling summary of what is already implemented in the current build. It is intended as a compact handoff/reference for future Codex runs so they can see the current shipped prototype state without rereading the full milestone chain first.

## Completed milestone range
This summary reflects completed work through **Milestone 042**.

## Current playable loop
On startup, the bootstrap scene loads a persisted game state if one exists, otherwise it falls back to the bootstrap demo world state. Startup then routes into the world map safe context or a main-menu placeholder target depending on safe-resume state.

From the world map, the player manually selects an enterable node and confirms entry. Combat-compatible nodes then auto-start their run flow: combat begins automatically, auto-targeting and auto-attacks resolve the 1v1 encounter over time, the run resolves to success or failure, and the screen enters post-run automatically. At post-run, the player can replay the node, return to the world map, or stop the session. Cleared nodes remain replayable through both post-run replay and later world-map re-entry, including farm access when they are no longer reachable through the normal forward/backtrack path rules. Non-combat placeholder nodes still use the simple placeholder run shell rather than real gameplay.

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
- The startup/bootstrap runtime flow is now grouped under a dedicated `Startup` domain folder/namespace for clearer ownership and navigation.

### World map and node entry
- A runtime world graph exists with regions, nodes, explicit connections, and limited branching.
- Reachability supports forward movement and backtracking from current world context.
- The world map shows node identity, node type, node state, current/selectable status, and a small summary of current context.
- Entering a selected node routes into a placeholder node screen through explicit node-entry flow logic.

### Run lifecycle and post-run flow
- Runs use explicit lifecycle states: `RunStart`, `RunActive`, `RunResolved`, and `PostRun`.
- Post-run shows a compact summary and allows replay, return to world, or stop session.
- The post-run summary now presents rewards and progress changes in a more clearly grouped aggregated format.
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

### Cleared-node replayability
- Cleared nodes remain selectable and enterable when they are otherwise reachable in the world flow.
- Cleared nodes are also enterable from the world map for farming even when they are no longer reachable through the normal forward/backtrack path rules.
- Re-entering a cleared node preserves its `Cleared` state and does not re-lock already unlocked connected nodes.
- In the current MVP, cleared-node farming value remains minimal: the node stays replayable and low-friction, but no broader reward economy exists yet.

### Reward payload model
- Resolved runs now carry a structured `RunRewardPayload`.
- The payload currently supports:
  - currency reward entries
  - material reward entries

### Basic soft currency
- The current economy model now has one live soft currency using `ResourceCategory.SoftCurrency`.
- Successful combat-compatible runs currently grant a small soft-currency reward through the structured run reward payload.
- Granted soft currency is applied into persistent resource balances during run resolution and is saved through the existing resolved world-context persistence boundary when the player returns to world or stops the session.
- The current post-run summary now surfaces the granted soft-currency reward in readable aggregated form.
- A minimal domain spending path exists through persistent resource balances, so soft currency can be added, spent, and rejected cleanly on overspend.

### One region material category
- The current economy model now has one live region material using `ResourceCategory.RegionMaterial`.
- Successful standard combat runs in regions whose resource identity is `RegionMaterial` currently grant a small region-material reward through the structured run reward payload.
- A purchased farm-oriented account-wide upgrade can increase that ordinary region-material reward output on repeatable standard combat runs.
- Granted region material is applied into persistent resource balances during run resolution and is saved through the existing resolved world-context persistence boundary when the player returns to world or stops the session.
- The current post-run summary aggregates the region-material reward alongside soft currency when both are granted.

### Ordinary vs milestone rewards
- Ordinary run rewards currently remain small and repeatable: successful combat runs grant soft currency, and standard combat runs in region-material regions also grant region material.
- A tracked node reaching its clear threshold now grants a distinct milestone reward using `ResourceCategory.PersistentProgressionMaterial`.
- That milestone reward is applied into persistent resource balances during run resolution and is saved through the existing resolved world-context persistence boundary when the player returns to world or stops the session.

### One account-wide progression sink
- The build now has one persistent account-wide upgrade sink stored in `PersistentProgressionState`.
- It currently contains three small upgrades/projects that consume `ResourceCategory.PersistentProgressionMaterial` and permanently record account-wide benefits in persistent data.
- Purchased upgrade state persists through the normal saved game-state flow and resolves into a small account-wide effect model that now feeds both player combat baseline stats and ordinary reward efficiency before future runs start.
- The current account-wide upgrades increase player max health, player attack power, and ordinary region-material reward output in future runs, without changing enemy baseline stats or milestone reward amounts.
- The new push-oriented offense upgrade helps harder combat more directly by increasing player-side baseline damage enough to visibly improve tougher future encounters.
- The new farm-oriented yield upgrade helps repeatable farming more directly by increasing ordinary region-material rewards on standard region-material combat clears.
- Dedicated service-hub or town-style runtime access to this sink is not implemented yet; the sink currently exists through persistent domain/state logic and combat integration rather than through a new UI flow.

### Persistent character baseline
- The build now has one explicit playable character stored in persistent game state: `character_vanguard` / `Vanguard`.
- Bootstrap fallback state creation and persisted-state startup normalization both ensure this default playable character exists, is unlocked, selectable, and active.
- Run entry now resolves the current persistent playable character into the player-side combat baseline instead of relying only on an anonymous hardcoded player concept.
- The current player combat identity and base stats come from that character-backed baseline, and existing account-wide progression effects continue to layer on top of it before combat begins.
- There is no character selection screen yet, and no second playable character exists yet.

### Post-run reward summary UI
- The current post-run panel surfaces run rewards, progress changes, and next actions in a compact aggregated text summary.
- Ordinary reward output stays grouped into one readable reward line rather than a noisy detailed breakdown.
- Milestone rewards are shown on a separate compact line when present, so clear-threshold runs feel distinct without expanding into a detailed reward panel.
- Progress changes are grouped into one readable line that distinguishes node progress gained this run from the current tracked total, while still surfacing persistent progression delta and route-unlock result.

## Important current rules / constraints
- Combat is currently **1v1 only**.
- Movement is **not** part of the current MVP combat model.
- The combat shell is still a debug-style placeholder presentation, not a final combat HUD.
- `BossOrGate` currently shares the same tracked-progress rule and default threshold as ordinary combat nodes as a temporary MVP placeholder.
- Current unlock behavior is limited to direct connected-node unlock on clear; advanced branch and gate semantics are still deferred.
- Broad farm access applies only to persistently `Cleared` nodes; uncleared nodes still follow the normal reachability rules.
- Failed or incomplete combat runs do **not** currently grant node progress in the MVP, because node progress is still kill-driven and the single-enemy combat prototype has no failed partial-kill case.
- Rewards and economy are still early and placeholder-level beyond the new soft-currency, one region-material path, one clear-threshold milestone reward, and one structural account-wide progression sink; broader reward differentiation and additional sinks are not implemented yet.
- The new account-wide sink is now wired into live player combat baseline stats, but it is not yet exposed through a dedicated service/town UI.
- Non-combat nodes still use placeholder run behavior.

## Not implemented yet
- Broader partial-completion outputs beyond the current 1v1 kill-driven MVP
- Real reward generation and reward persistence beyond the current soft-currency, one region-material path, and one clear-threshold milestone reward
- Additional progression sinks and dedicated sink access through the service/town layer
- Expanded multi-character/build systems beyond the current single-character baseline
- Multi-entity combat, skills, advanced AI, and broader combat content

## Known temporary placeholders / technical shortcuts
- The world graph and initial persistent state are currently seeded by `BootstrapWorldMapFactory`.
- The world map, node screen, post-run panel, and combat shell are runtime-generated placeholder UI.
- Main menu flow is still a placeholder target rather than a real menu system.
- Combat nodes use a minimal direct-engagement model with no movement, range, animation, or final presentation systems.
- Boss/gate node progress behavior is intentionally temporary and should be revisited in the later progression/boss milestones.

## Source note
This file is derived from the milestone notes in `specs/milestone/` plus the current codebase entry flow. It should be updated after each completed milestone so it remains the compact source for current implemented build state.
- Shared cross-domain identifiers and category types are now physically grouped under `Assets/Scripts/Core/`, with matching EditMode ownership tests under `Assets/Tests/EditMode/Core/`.
- EditMode tests are now physically grouped much more closely to runtime domain ownership through `Core`, `Startup`, `Combat`, `Run`, `World`, `State`, `State/Persistence`, and `Data` test folders.
- Runtime and EditMode test namespaces now mirror that domain folder structure under `Survivalon.Runtime.*` and `Survivalon.Tests.EditMode.*`, with only the intentionally cross-domain root assembly marker/smoke-test exceptions left unchanged.
