# Current Build State

## Purpose
This file is a rolling summary of what is already implemented in the current build. It is intended as a compact handoff/reference for future Codex runs so they can see the current shipped prototype state without rereading the full milestone chain first.

## Completed milestone range
This summary reflects completed work through **Milestone 073**, plus the accepted cleanup/refactor milestones **042b** through **042h**, **047a**, **050a**, **052a**, **056a**, **059a**, **061a**, **063a**, **065a**, **067a**, **068a**, **069a**, **070a**, **072a**, and **073a**.

## Current playable loop
On startup, the bootstrap scene loads a persisted game state if one exists, otherwise it falls back to the bootstrap demo world state. Startup then routes into the world map safe context or a main-menu placeholder target depending on safe-resume state.

From the world map, the player manually selects an enterable node and confirms entry. Combat-compatible nodes then auto-start their run flow: combat begins automatically, auto-targeting and auto-attacks resolve the 1v1 encounter over time, the run resolves to success or failure, and the screen enters post-run automatically. At post-run, the player can replay the node, return to the world map, or stop the session. Cleared nodes remain replayable through both post-run replay and later world-map re-entry, including farm access when they are no longer reachable through the normal forward/backtrack path rules. The current cavern service node now opens a distinct town/service shell instead of the generic node placeholder, while broader non-combat content remains placeholder-level.

Manual actions currently required:
- select a node on the world map
- confirm node entry
- choose one run-only skill upgrade before combat auto-starts when the selected current package exposes that choice
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
- The shipped bootstrap content now has two explicit location identities attached through world content data:
  - `Verdant Frontier`
  - `Echo Caverns`
- World regions/nodes now carry that location identity explicitly through node-entry state instead of relying only on raw region ids.
- The shipped location identities now also carry small gameplay-facing emphasis data:
  - `Verdant Frontier` is the region-material farming / raider-emphasis location
  - `Echo Caverns` is the progression-focused / gate-guardian-emphasis location
- Reachability supports forward movement and backtracking from current world context.
- The world map shows friendly node identity, node type, node state, current/selectable status, and a small summary of current context.
- The shipped bootstrap nodes now carry authored friendly player-facing names, and the main world-map surfaces use those names instead of raw internal node ids:
  - node button labels
  - selected-node summary
  - route/readability summary lines
  - entry button text
- World-map node labels now surface the shipped location identity alongside those friendly names so the current regions read as distinct places in the prototype flow.
- The world map now also exposes one compact readable world-state summary:
  - current location identity and region
  - current node and current node state
  - forward routes, true backtrack routes, replayable farm nodes, and blocked linked paths from the current context
  - compact state/status legends so the placeholder node list reads more clearly at a glance
- World-map reachability/path-role projection is now rebuilt from the current world state each refresh instead of relying on constructor-time cached access sets, so reused world-map controllers cannot go stale after world-state changes.
- World-map node labels now also carry a small path-role line:
  - `Current anchor`
  - `Forward route`
  - `Backtrack route`
  - `Replayable farm node`
  - `Blocked path`
- The placeholder world map now keeps its node list inside a simple scrollable viewport with stable full-width node-button alignment, so lower node buttons remain reachable and readable as the header, character-selection, and package-assignment area grows.
- The same placeholder world map now also exposes one minimal two-slot gear equip/unequip area for the currently selected character without changing the overall placeholder screen structure.
- Entering a selected node routes into a placeholder node screen through explicit node-entry flow logic.
- The placeholder node screen now also surfaces the current location identity and reward focus for the entered content.
- The placeholder node screen and live town/service shell now also surface the current location's enemy emphasis, keeping location identity visible beyond world-map labels.
- The shipped frontier farm node now also carries one explicit revisit-value content rule:
  - it surfaces `Revisit value: Region material yield +1` in the node placeholder summary
  - this keeps `Verdant Frontier` visibly useful even after deeper cavern progression is available
  - invalid authored region-material-yield combinations are now rejected at world-graph validation, and revisit-value text is only shown when the entered node actually supports region-material rewards
- `ServiceOrProgression` entry currently routes into one explicit town/service shell only when node content supplies a town-service context definition; otherwise it falls back to the generic placeholder node shell.

### Run lifecycle and post-run flow
- Runs use explicit lifecycle states: `RunStart`, `RunActive`, `RunResolved`, and `PostRun`.
- Post-run shows a compact summary and allows replay, return to world, or stop session.
- The post-run summary now presents rewards and progress changes in a more clearly grouped aggregated format.
- The post-run summary now also shows a compact explicit boss-gate unlock line when a boss defeat opens new forward progression.
- The post-run panel now also exposes one compact next-action guidance area that clarifies the current practical decision using existing actions only:
  - replay the same node
  - return to world to push forward
  - return to world, then visit `Cavern Service Hub` for current upgrade/refinement opportunities
  - stop safely after the resolved run
- That next-action guidance uses friendly node/service names and refreshes from current world/progression/resource state, so the recommendation changes when forward paths or service opportunities change.
- The post-run recommendation logic now also keeps push-target resolution and service-hub opportunity resolution separate, so utility/service nodes are not treated as forward push targets.
- Ordinary route unlock state and boss-gate unlock state are now tracked separately in run results, so the summary no longer relies on one overloaded unlock flag.
- Replay re-enters the same node cleanly.
- Return/stop save a world-level safe resume context.

### Town / service shell
- The current build now has one explicit town-equivalent service context:
  - `Cavern Service Hub`
- It is reached through the existing cavern service node in the bootstrap world graph and opens a dedicated non-combat screen instead of the generic node placeholder shell.
- The service shell currently exposes two MVP-readable sections:
  - a progression-hub summary tied to the existing account-wide progression sink and current persistent progression material balance
  - a build-preparation summary tied to the currently selected character, assigned skill package, and equipped primary/support gear
- The service shell overview now also surfaces the current authored location identity and its reward focus/source, so the live cavern service flow reads explicitly as `Echo Caverns` rather than only as a raw region id.
- The service shell now also supports one short direct progression interaction:
  - affordable account-wide projects can be purchased directly from the town/service screen
  - purchase spends `Persistent progression material`
  - purchased state and balances persist immediately
  - the service screen refreshes in place after purchase
- The service shell now also supports one short direct conversion/refinement interaction:
  - `Region material x3 -> Persistent progression material x1`
  - the conversion is visible in the progression-hub section
  - successful conversion persists immediately and refreshes the service screen in place
  - insufficient region material leaves the conversion visible but unavailable
- The progression-hub section now also makes the current material-to-power loop explicit:
  - it shows how repeated region-material farming feeds refinement and then future project purchases
  - it shows current refinement readiness or next-refinement progress
  - it shows which project targets are already affordable and which additional project targets the current refinement path would enable
- The service shell now also supports one short direct build-preparation interaction for the currently selected character:
  - skill package assignment can be changed there using the existing persistent character-assignment rules
  - primary and support gear can be equipped or unequipped there using the existing persistent loadout rules
  - successful changes persist immediately and the service screen refreshes in place
- The world map still keeps its existing build controls in parallel for now; the town/service shell is an additional safe-context build-preparation surface, not a replacement hub yet.
- The service shell currently supports two safe actions:
  - return to world map
  - stop session
- The service shell now uses a safer scrollable content area for its readable overview, progression, and build sections.

### Combat foundation
- Combat exists as a placeholder shell inside the current node/run flow rather than as a dedicated final combat scene.
- Combat-capable node screens now keep a compact persistent run-context header visible during both the pending run-start upgrade choice and the later combat flow instead of repeating the larger generic node placeholder summary block in combat contexts.
- The combat placeholder shell now also exposes one compact readable run HUD layer during active/resolved combat:
  - current location identity and node context
  - current auto-battle/run state, outcome, and elapsed combat time
  - player and enemy health at a glance
  - tracked progress context when applicable, including boss gate-clear progress
- The combat prototype is currently one player-side entity versus one enemy-side entity.
- Combat entities have explicit allegiance, alive/active state, runtime health, and a small shared base stat model.
- Base stats currently include max health, attack power, attack rate, and defense-based survivability.
- Standard enemies now have two shipped combat profiles instead of one:
  - `Enemy Unit` is the faster lighter-pressure baseline standard enemy
  - `Bulwark Raider` is the slower sturdier push-oriented standard enemy currently used by the forest push combat node
- The build now also has one explicit boss encounter model:
  - `Gate Boss` is the first shipped gate-boss encounter and uses the same current 1v1 deterministic auto-battle foundation as other encounters
  - boss encounter content is modeled explicitly through separate boss encounter/profile data instead of reusing only standard-enemy encounter assumptions
- Current shipped combat-node enemy selection now comes from small bootstrap encounter-content data attached at node entry rather than hardcoded node-id branching inside the combat resolver.
- The initial shipped node/location mapping remains intentionally small and bootstrap-seeded:
  - forest entry and forest farm use `Enemy Unit`
  - forest push uses `Bulwark Raider`
  - current gate/boss nodes use the explicit `Gate Boss` encounter
- That location split now has one small live gameplay difference beyond labels:
  - the specific shipped `Cavern Gate` boss node grants `+1` extra persistent progression material compared with the same baseline boss reward in `Verdant Frontier`
  - this keeps `Echo Caverns` as the more progression-focused shipped location while leaving the bonus owned by specific boss-node content rather than all bosses in that location
- The first shipped boss progression gate now exists in live bootstrap content:
  - defeating the forest `Gate Boss` unlocks `region_002_node_002` / `Cavern Gate`
  - that result is surfaced explicitly in post-run summary text from structured boss-gate unlock data, separate from ordinary route-unlock state
  - the unlocked `Cavern Gate` becomes reachable on return to the world map through the existing bootstrap route graph
  - successful boss defeat now also grants a distinct boss reward bundle using `Persistent progression material x2`, separate from both ordinary rewards and clear-threshold milestone rewards

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
- One run-time skill-upgrade choice layer now exists on top of the current triggered active skill seam:
  - when the selected current package grants `Burst Strike`, run start now pauses for one small run-only choice before combat auto-starts
  - the current shipped choices are temporary run-only modifiers on `Burst Strike`: `Burst Tempo` (faster cadence for the current run) and `Burst Payload` (harder hit for the current run)
  - that run-start choice now appears through one compact readable choice panel with a clear header, short effect summaries, quick-pick hints, and one click target per option
  - the readable choice copy is now resolved on the run/UI presentation side rather than being stored directly on the combat upgrade option data
  - that choice applies only to the current run and is not persisted into later runs or character/package state
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
- The shipped `Verdant Frontier` farm node now also grants one extra region material through node-owned content, so that run currently pays `Region material x2` before any purchased farm-yield project bonus.
- A purchased farm-oriented account-wide upgrade can increase that ordinary region-material reward output on repeatable standard combat runs.
- Granted region material is applied into persistent resource balances during run resolution and is saved through the existing resolved world-context persistence boundary when the player returns to world or stops the session.
- The current post-run summary aggregates the region-material reward alongside soft currency when both are granted.
- Reward/source presentation is now location-aware in the shipped bootstrap content, so the current post-run flow names the reward source based on the entered location identity rather than showing only generic reward text.
- In the current shipped loop, that means earlier frontier farming now has one explicit lasting-value path after deeper progression opens:
  - replay the frontier farm node for higher region material
  - refine region material into persistent progression material in town
  - spend that persistent progression material on account-wide projects

### Ordinary vs milestone rewards
- Ordinary run rewards currently remain small and repeatable: successful combat runs grant soft currency, and standard combat runs in region-material regions also grant region material.
- A tracked node reaching its clear threshold now grants a distinct milestone reward using `ResourceCategory.PersistentProgressionMaterial`.
- That milestone reward is applied into persistent resource balances during run resolution and is saved through the existing resolved world-context persistence boundary when the player returns to world or stops the session.
- Successful boss defeat now also grants a separate boss reward bundle:
  - `Persistent progression material x2`
  - this is surfaced on its own compact `Boss rewards` line in the post-run summary so boss clears feel more important than ordinary clears without expanding into a broader loot screen
- Current shipped boss-node content now differentiates that boss bundle:
  - the forest gate boss keeps the baseline `Persistent progression material x2`
  - the cavern gate boss grants `Persistent progression material x3`

### One account-wide progression sink
- The build now has one persistent account-wide upgrade sink stored in `PersistentProgressionState`.
- It currently contains four small upgrades/projects that consume `ResourceCategory.PersistentProgressionMaterial` and permanently record account-wide benefits in persistent data.
- Purchased upgrade state persists through the normal saved game-state flow and resolves into a small account-wide effect model that now feeds both player combat baseline stats and ordinary reward efficiency before future runs start.
- The current account-wide upgrades increase player max health, player attack power, and ordinary region-material reward output in future runs, without changing enemy baseline stats or milestone reward amounts.
- The new push-oriented offense upgrade helps harder combat more directly by increasing player-side baseline damage enough to visibly improve tougher future encounters.
- The new farm-oriented yield upgrade helps repeatable farming more directly by increasing ordinary region-material rewards on standard region-material combat clears.
- The new project-style powerup layer now also includes one explicit boss-focused efficiency project:
  - `Boss Salvage Project`
  - it consumes persistent progression material through the same town/service progression sink
  - once purchased, successful future boss clears grant `+1` extra persistent progression material in the existing boss reward bundle
  - this gives the current prototype one persistent project mechanic that is clearly separate from direct node unlock progression
- The town/service shell now exposes that sink as the current live purchase surface:
  - affordable projects can be bought directly from `Cavern Service Hub`
  - purchases spend persistent progression material and persist immediately
  - already-purchased projects are no longer buyable there
  - unaffordable projects remain visible but unavailable
- The town/service shell now also exposes one small refinement path into that same sink:
  - `Region material x3 -> Persistent progression material x1`
  - this gives repeatable region farming a direct safe-context conversion path into future account-wide project purchasing
- The same town/service progression summary now makes that loop explicit as a current MVP powerup path by showing:
  - current refinement readiness/progress
  - current already-affordable projects
  - new project targets the current refinement path would fund

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
- The current town/service shell now exposes the same selected-character skill-package assignment through a second safe-context MVP interaction surface, while still using the same persistent assignment rules and selected-character state.
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

### Persistent gear baseline
- The build now has two live persistent gear categories:
  - `PrimaryCombat`
  - `SecondarySupport`
- Two shipped starter gear items currently exist in the code-driven gear catalog:
  - `gear_primary_training_blade` / `Training Blade`
- `gear_secondary_guard_charm` / `Guard Charm`
- Persistent game state now carries account-owned gear ids, and startup/bootstrap normalization ensures the starter gear is present in owned gear data.
- Each persistent playable character already carries a `PersistentLoadoutState` with equipped-gear entries by category.
- The current world map now exposes one minimal gear equip placeholder area for the selected character:
  - it shows the currently equipped `PrimaryCombat` and `SecondarySupport` gear
  - it allows equipping or unequipping the shipped starter items before future runs
  - the change is persistent and follows the currently selected character
- The current town/service shell now also exposes the same selected-character primary/support gear equip state and allows equipping or unequipping those shipped starter items before future runs through the same persistent loadout seam.
- Current startup/bootstrap normalization now validates persisted equipped-gear entries against:
  - owned gear state
  - current shipped gear ids
  - category match
  - one equipped item per category
- The current default bootstrap state still starts with no gear equipped on the shipped characters.
- The selected character's persistent loadout data now flows into run context and now affects future combat through two small live gear effects.
- The shipped `Training Blade` currently grants a flat `+2` attack-power bonus when equipped, which improves the selected character's future combat output and shortens ordinary autobattle runs without redesigning the current combat model.
- The shipped `Guard Charm` currently grants a flat `+40` max-health bonus when equipped, which improves the selected character's future survivability and leaves standard-run outcomes with higher remaining health without redesigning the current combat model.

### Post-run reward summary UI
- The current post-run panel surfaces run rewards, progress changes, and next actions in a compact aggregated text summary.
- Ordinary reward output stays grouped into one readable reward line rather than a noisy detailed breakdown.
- Milestone rewards are shown on a separate compact line when present, so clear-threshold runs feel distinct without expanding into a detailed reward panel.
- The summary now also surfaces the current location identity and reward source for shipped content, making the region/material loop more legible in the MVP flow.
- Progress changes are grouped into one readable line that distinguishes node progress gained this run from the current tracked total, while still surfacing persistent progression delta and route-unlock result.
- A separate compact next-action block now makes the existing post-run buttons easier to read in practice by distinguishing:
  - replay same node
  - return to world to push
  - return to world, then visit `Cavern Service Hub`
  - safe stop

## Important current rules / constraints
- Combat is currently **1v1 only**.
- Movement is **not** part of the current MVP combat model.
- The combat shell now has one minimal readable run-HUD baseline, but it is still placeholder-level and not a final combat HUD.
- `BossOrGate` currently shares the same tracked-progress rule and default threshold as ordinary combat nodes as a temporary MVP placeholder.
- Current unlock behavior is limited to direct connected-node unlock on clear; advanced branch and gate semantics are still deferred.
- Broad farm access applies only to persistently `Cleared` nodes; uncleared nodes still follow the normal reachability rules.
- Failed or incomplete combat runs do **not** currently grant node progress in the MVP, because node progress is still kill-driven and the single-enemy combat prototype has no failed partial-kill case.
- Rewards and economy are still early and placeholder-level beyond the new soft-currency, one region-material path, one clear-threshold milestone reward, and one structural account-wide progression sink; broader reward differentiation and additional sinks are not implemented yet.
- The new account-wide sink is now wired into live player combat baseline stats and can now be spent directly through the current town/service shell, while broader sink interaction depth is still deferred.
- Character selection now exists only as a minimal world-map placeholder path; broader roster UI and deeper multi-character systems are still deferred.
- Character-linked progression now exists only as a simple rank-like max-health bonus; broader character trees, multiple progression axes, and dedicated character progression UI are still deferred.
- The skill layer is still intentionally small: one passive skill, one periodic auto-triggered active skill, and one minimal world-map package-assignment placeholder now exist, while additional skill variety, cooldown/UI complexity, and broader skill-package/loadout systems are still deferred.
- The first run-time skill choice now exists only as one compact readable choice panel for current `Burst Strike` users; broader in-run upgrade pools, repeated level-up chains, and upgrade UI depth are still deferred.
- Gear now exists for two categories with a minimal pre-run equip/unequip placeholder and two simple live stat effects; broader gear UI, gear acquisition, additional categories beyond the current two-slot baseline, and richer gear effects are still deferred.
- The current town/service shell is intentionally MVP-small: it provides a distinct safe context with direct progression purchasing, one fixed region-material refinement action, selected-character build preparation, and return/stop actions, but not a full service interaction suite.

## Not implemented yet
- Broader partial-completion outputs beyond the current 1v1 kill-driven MVP
- Real reward generation and reward persistence beyond the current soft-currency, one region-material path, and one clear-threshold milestone reward
- Additional progression sinks and broader service/town interaction beyond the current direct project purchase flow and one fixed refinement action
- Deeper town/build navigation beyond the current short direct selected-character package and gear controls
- Expanded multi-character/build systems beyond the current two-character placeholder roster and simple rank-based character growth
- Additional gear categories beyond the current two-slot baseline, richer live gear effects, and broader itemization/loot systems
- Multi-entity combat, broader skill systems, advanced AI, and broader combat content

## Known temporary placeholders / technical shortcuts
- The world graph, town-service context mapping, and initial persistent state are currently seeded by `BootstrapWorldMapFactory`.
- The first shipped location identities are still bootstrap-seeded content data rather than authored external content assets.
- The fallback location-identity seam still exists as a defensive default for missing world data, but current shipped bootstrap regions and their entered placeholder states are explicitly covered as authored identities rather than fallback-generated ones.
- The first location-based differentiation layer is intentionally small and data-owned: location identity owns place flavor/emphasis/presentation data, while the current cavern gate's extra boss reward is carried by specific boss-node content rather than the whole location identity.
- The first old-location relevance rule is intentionally just as narrow and data-owned:
  - the frontier farm's extra region-material yield is carried by specific combat-node content rather than the whole location identity
- The world map, node screen, town/service shell, post-run panel, and combat shell are runtime-generated placeholder UI.
- Main menu flow is still a placeholder target rather than a real menu system.
- Combat nodes use a minimal direct-engagement model with no movement, range, animation, or final presentation systems.
- The current passive skill layer is still interpreted through a small hardcoded resolver path for the single shipped passive, `Relentless Assault`.
- The current auto-triggered active skill layer is still interpreted through small hardcoded resolver paths for the single shipped active skill, `Burst Strike`.
- Standard enemy and boss variety are still intentionally small: one faster baseline standard enemy, one slower sturdier push-oriented standard enemy variant, and one explicit gate-boss profile/encounter currently exist, while broader enemy rosters and faction/content variety are still deferred.
- Standard-enemy encounter/profile data and boss encounter/profile data now live in separate small catalogs, while current encounter content remains bootstrap-seeded.
- Boss/gate node progress behavior is still intentionally temporary beyond the new forest-gate unlock result; boss defeat now opens the next gate target and grants one small differentiated boss reward bundle, but broader boss clear semantics and richer boss reward systems are still deferred.

## Source note
This file is derived from the milestone notes in `specs/milestone/` plus the current codebase entry flow. It should be updated after each completed milestone so it remains the compact source for current implemented build state.
- Shared cross-domain identifiers and category types are now physically grouped under `Assets/Scripts/Core/`, with matching EditMode ownership tests under `Assets/Tests/EditMode/Core/`.
- EditMode tests are now physically grouped much more closely to runtime domain ownership through `Core`, `Startup`, `Combat`, `Run`, `World`, `State`, `State/Persistence`, and `Data` test folders.
- Runtime and EditMode test namespaces now mirror that domain folder structure under `Survivalon.*` and `Survivalon.Tests.EditMode.*`, with only the intentionally cross-domain root assembly marker/smoke-test exceptions left unchanged.
- The follow-up namespace cleanup removed the broad redundant cross-domain `using` noise from runtime and EditMode files; runtime behavior stayed unchanged.
- A small follow-up dead-code pass removed a few runtime convenience accessors that existed only for EditMode tests; runtime behavior stayed unchanged.
