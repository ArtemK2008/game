# Current Build State

## Purpose
This file is a rolling summary of what is already implemented in the current build. It is intended as a compact handoff/reference for future Codex runs so they can see the current shipped prototype state without rereading the full milestone chain first.

## Completed milestone range
This summary reflects completed work through **Milestone 049a**, plus the accepted cleanup/refactor milestones **042b** through **042h** and **047a**.

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
- The placeholder world map now keeps its node list inside a simple scrollable viewport with stable full-width node-button alignment, so lower node buttons remain reachable and readable as the header/selection area grows.
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
- The baseline attack now resolves through a small explicit skill-compatible combat structure instead of a one-off direct resolver path:
  - combat entities carry a baseline attack definition
  - the automated combat loop triggers that baseline attack on its existing interval timing
  - attack effect application now routes through a dedicated combat-skill executor seam
- One passive skill layer now exists on top of that baseline attack seam:
  - current playable-character skill packages can resolve always-on passive combat skills into the player combat entity
  - the current shipped passive is `Relentless Assault`, which increases direct-damage skill output by `20%`
  - in the current prototype, that passive affects both the automated baseline attack and the current auto-triggered active skill because both use the direct-damage combat path
- One auto-triggered active skill layer now exists on top of the same combat seam:
  - current playable-character skill packages can resolve one periodic triggered active skill into the player combat entity
  - the current shipped active skill is `Burst Strike`, which auto-triggers every `2.5` seconds and deals boosted direct damage through the existing combat-skill executor seam
  - only `Striker`'s default skill package currently grants that active skill
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

### Persistent character baseline, selection placeholder, and linked progression
- The build now has two explicit playable characters stored in persistent game state:
  - `character_vanguard` / `Vanguard`
  - `character_striker` / `Striker`
- Bootstrap fallback state creation and persisted-state startup normalization both ensure these shipped playable characters exist, are unlocked, and selectable.
- The build now also has an explicit character-selection placeholder model:
  - selectable playable characters are resolved through a dedicated selection service
  - the active/current playable character is normalized so one valid selectable character remains active
  - the current world map shows the selected character and one small placeholder selection button per selectable playable character
- The current world map also now exposes one minimal build-facing skill-package assignment placeholder:
  - the selected character's currently assigned package is shown next to the existing character-selection area
  - only valid package choices for the currently selected character are shown
  - changing the package updates that character's persistent `skillPackageId`, and future run entry uses the assigned package instead of only the profile default
- Run entry now resolves the current selected persistent playable character into the player-side combat baseline instead of relying only on an anonymous hardcoded player concept.
- The current player combat identity and base stats come from that character-backed baseline, and existing account-wide progression effects continue to layer on top of it before combat begins.
- The selected playable character now also has one live character-linked progression path:
  - successful combat-compatible runs increase that character's persistent `ProgressionRank` by `+1`
  - each rank currently grants `+5` max health to that same character on future combat run entry
  - this character-owned bonus is resolved separately from the account-wide progression sink and stacks with existing account-wide combat bonuses
- The current shipped character baselines are intentionally small but meaningfully different in the 1v1 autobattle prototype:
  - `Vanguard` is the sturdier default baseline
  - `Striker` trades durability/defense for higher offense and faster attack cadence
  - `Striker`'s default skill package now carries both the current passive skill layer, `Relentless Assault`, and the current periodic active skill, `Burst Strike`
- The current shipped skill-package assignment options are intentionally tiny:
  - `Vanguard` can use `Standard Guard` (default, no passive or active skill) or `Burst Drill` (adds `Burst Strike`)
  - `Striker` currently has one valid package, `Relentless Burst`, which carries `Relentless Assault` plus `Burst Strike`
- In the current prototype, that differentiation is already visible and testable:
  - selecting `Striker` changes future combat entry stats
  - selecting `Striker` also adds the current always-on passive direct-damage bonus and periodic active skill to future combat entry
  - the current boss/gate placeholder encounter that defeats `Vanguard` can be cleared by `Striker` without needing account-wide offense upgrades
- The new assignment flow is also already visible and testable:
  - assigning `Vanguard` to `Burst Drill` adds the same current periodic active skill to `Vanguard`'s future combat entry
  - that package assignment is persistent and can turn the current boss/gate placeholder encounter from `Vanguard` failure into success without changing `Vanguard`'s base character identity
- There is no dedicated character selection screen yet and no dedicated character-progression UI exists yet.

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
- Character selection now exists only as a minimal world-map placeholder path; broader roster UI and deeper multi-character systems are still deferred.
- Character-linked progression now exists only as a simple rank-like max-health bonus; broader character trees, multiple progression axes, and dedicated character progression UI are still deferred.
- The skill layer is still intentionally small: one passive skill, one periodic auto-triggered active skill, and one minimal world-map package-assignment placeholder now exist, while additional skill variety, cooldown/UI complexity, and broader skill-package/loadout systems are still deferred.
- Non-combat nodes still use placeholder run behavior.

## Not implemented yet
- Broader partial-completion outputs beyond the current 1v1 kill-driven MVP
- Real reward generation and reward persistence beyond the current soft-currency, one region-material path, and one clear-threshold milestone reward
- Additional progression sinks and dedicated sink access through the service/town layer
- Expanded multi-character/build systems beyond the current two-character placeholder roster and simple rank-based character growth
- Multi-entity combat, broader skill systems, advanced AI, and broader combat content

## Known temporary placeholders / technical shortcuts
- The world graph and initial persistent state are currently seeded by `BootstrapWorldMapFactory`.
- The world map, node screen, post-run panel, and combat shell are runtime-generated placeholder UI.
- Main menu flow is still a placeholder target rather than a real menu system.
- Combat nodes use a minimal direct-engagement model with no movement, range, animation, or final presentation systems.
- The current passive skill layer is still interpreted through a small hardcoded resolver path for the single shipped passive, `Relentless Assault`.
- The current auto-triggered active skill layer is still interpreted through small hardcoded resolver paths for the single shipped active skill, `Burst Strike`.
- Boss/gate node progress behavior is intentionally temporary and should be revisited in the later progression/boss milestones.

## Source note
This file is derived from the milestone notes in `specs/milestone/` plus the current codebase entry flow. It should be updated after each completed milestone so it remains the compact source for current implemented build state.
- Shared cross-domain identifiers and category types are now physically grouped under `Assets/Scripts/Core/`, with matching EditMode ownership tests under `Assets/Tests/EditMode/Core/`.
- EditMode tests are now physically grouped much more closely to runtime domain ownership through `Core`, `Startup`, `Combat`, `Run`, `World`, `State`, `State/Persistence`, and `Data` test folders.
- Runtime and EditMode test namespaces now mirror that domain folder structure under `Survivalon.*` and `Survivalon.Tests.EditMode.*`, with only the intentionally cross-domain root assembly marker/smoke-test exceptions left unchanged.
- The follow-up namespace cleanup removed the broad redundant cross-domain `using` noise from runtime and EditMode files; runtime behavior stayed unchanged.
- A small follow-up dead-code pass removed a few runtime convenience accessors that existed only for EditMode tests; runtime behavior stayed unchanged.
