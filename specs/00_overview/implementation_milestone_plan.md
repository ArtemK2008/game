# Implementation Milestone Plan

## Purpose
Define the step-by-step implementation plan for the project as a sequence of small atomic milestones.

---

## Scope
This spec defines:
- milestone order
- milestone size expectations
- dependency-aware implementation sequence
- practical build-up from initialized Unity project to full implementation of current specs

This spec does not define:
- exact prompts for Codex
- exact dates or time estimates
- exact git branching workflow

---

## Starting assumptions
- Unity project already exists and opens successfully.
- `agent.md` already exists.
- specs are stored in `specs/`.
- milestone notes will be stored in `specs/milestone/`.
- Codex follows `codex_delivery_workflow`.
- Codex follows `code_style`.
- Milestones are small and complete.
- Project-related created/changed files are staged with `git add`.
- No commits are created by Codex; the user commits manually.

---

## Milestone design rules
- Each milestone should implement one coherent slice.
- Each milestone should be testable or verifiable.
- Each milestone should leave the project in a runnable state.
- Each milestone should include directly related Unity wiring.
- Each milestone should create a milestone note.
- If required assets are missing, Codex should ask for them.

---

## Phase 0 — repository and project baseline

### Milestone 001 — verify project baseline
- Open the Unity project and verify it runs.
- Verify folder structure exists for scripts, scenes, prefabs, sprites, audio, ScriptableObjects, tests, and specs.
- Add missing top-level folders if needed.
- Add initial milestone note.

### Milestone 002 — align spec folder structure
- Ensure all current specs exist inside `specs/`.
- Ensure `specs/milestone/` exists.
- Ensure naming consistency for existing spec files.
- Add milestone note.

### Milestone 003 — create core assembly/test structure
- Create initial runtime and test assembly structure if needed.
- Create basic test project setup for unit tests.
- Verify tests can run.
- Add milestone note.

### Milestone 004 — create bootstrap scene
- Create one minimal playable bootstrap scene.
- Ensure game can launch into a controlled startup flow.
- Add milestone note.

---

## Phase 1 — foundational data model and application shell

### Milestone 005 — create core identifiers and enums
- Implement core enums and identifiers for node state, node type, progression layer types, reward categories, and other stable categories already defined by specs.
- Keep them minimal.
- Add tests for core enum-dependent rules where useful.
- Add milestone note.

### Milestone 006 — create base data containers
- Implement ScriptableObject or serializable data definitions for regions, nodes, rewards, characters, and gear placeholders.
- Do not implement gameplay yet.
- Add milestone note.

### Milestone 007 — create persistent game state model
- Implement plain C# state models for world state, node state, progression state, resource balances, character state, and loadout state.
- Keep runtime state and persistent state separate.
- Add tests for state transitions if applicable.
- Add milestone note.

### Milestone 008 — create game bootstrap flow
- Implement initial game startup controller/state entry flow.
- Route startup into main menu or world view placeholder.
- Keep logic thin and testable where possible.
- Add milestone note.

---

## Phase 2 — world structure and navigation foundation

### Milestone 009 — implement world graph model
- Implement region, node, and node-connection runtime model.
- Support locked and available nodes.
- Support connected routes.
- Add tests for graph reachability basics.
- Add milestone note.

### Milestone 010 — implement node reachability rules
- Implement logic for determining currently reachable nodes.
- Support forward movement and backtracking rules.
- Add tests for locked/available/reachable behavior.
- Add milestone note.

### Milestone 011 — implement basic world map screen
- Create a minimal world map UI.
- Show nodes and their basic state.
- Allow selecting an available node.
- Add milestone note.

### Milestone 012 — implement world-to-node entry flow
- Allow entering a selected node from the world map.
- Route into a placeholder node scene/state.
- Return safely to world map.
- Add milestone note.

### Milestone 013 — implement route-choice support
- Support at least one case of branch selection in the world graph.
- Keep route logic small and readable.
- Add tests for branch availability.
- Add milestone note.

---

## Phase 3 — run/session structure and safe loop skeleton

### Milestone 014 — implement run lifecycle shell
- Implement run start, run active, run resolved, and post-run states.
- No full combat yet.
- Ensure run result object exists.
- Add milestone note.

### Milestone 015 — implement post-run state flow
- Create post-run summary state/panel.
- Allow replay, return to world, or stop.
- Keep it minimal and readable.
- Add milestone note.

### Milestone 016 — implement safe stopping/resume baseline
- Persist enough state to return to a world-level safe context after a resolved run.
- Do not implement offline progress yet.
- Add tests for basic persistence boundaries.
- Add milestone note.

### Milestone 017 — implement session-context helpers
- Track recent node, recent push target, or similar lightweight session context if helpful.
- Use only if it improves loop readability.
- Add milestone note.

---

## Phase 4 — combat foundation

### Milestone 018 — create combat scene/state shell
- Create a minimal combat scene or combat context.
- Spawn one player-side entity and one enemy-side entity.
- No full combat rules yet.
- Add milestone note.

### Milestone 019 — implement combat entity base model
- Implement common combat entity data/state structures.
- Separate player-side and enemy-side allegiance.
- Add tests for basic combat entity setup.
- Add milestone note.

### Milestone 020 — implement base stat model
- Implement minimal stats from `stats_and_formulas`.
- Include health, damage, attack speed/timing, and survivability rule.
- Add tests for effective stat calculation.
- Add milestone note.

### Milestone 021 — implement basic attack resolution
- Implement basic target selection and attack execution for one player entity and one enemy.
- Resolve damage over time.
- Add unit tests for attack and defeat outcomes.
- Add milestone note.

### Milestone 022 — implement enemy defeat and run resolve baseline
- Remove defeated enemy from active combat.
- Resolve run success/failure in a minimal combat scenario.
- Add milestone note.

---

## Phase 5 — automation / AI baseline

### Milestone 023 — implement basic auto-targeting
- Add automated target selection for player-side combat.
- Keep rules simple and deterministic.
- Add tests if the logic is isolated.
- Add milestone note.

### Milestone 024 — implement basic auto-attack loop
- Make the player-side entity fight without manual attack input.
- Ensure stable repeated combat behavior.
- Add milestone note.

### Milestone 025 — implement basic enemy hostility behavior
- Make enemies threaten or attack the player-side entity.
- Keep enemy behavior minimal but functional.
- Add milestone note.

### Milestone 026 — implement movement if movement exists in MVP
- Add simple automated movement behavior only if the chosen combat model needs it.
- Keep it readable and testable.
- Add milestone note.

### Milestone 027 — verify no-manual-combat core loop
- Confirm node runs can resolve without manual movement or attacks.
- Tighten behavior until the core autobattle loop is usable.
- Add milestone note.

---

## Phase 6 — map completion and unlock backbone

### Milestone 028 — implement node progress meter
- Add persistent node progress value.
- Connect enemy kills to progress gain.
- Add tests for progress accumulation.
- Add milestone note.

### Milestone 029 — implement map clear threshold rule
- Mark node as cleared when threshold is reached.
- Keep threshold rule simple and kill-driven.
- Add tests for state transitions.
- Add milestone note.

### Milestone 030 — implement next-node unlock rule
- Unlock next connected node when current node clears.
- Update world map state accordingly.
- Add tests for unlock behavior.
- Add milestone note.

### Milestone 031 — implement partial completion value
- Ensure incomplete or failed runs can still grant progress where allowed.
- Keep this aligned with specs.
- Add milestone note.

### Milestone 032 — implement replayability of cleared nodes
- Allow re-entering cleared nodes.
- Preserve their world value for farming.
- Add milestone note.

---

## Phase 7 — rewards, economy, and post-run meaning

### Milestone 033 — implement reward payload model
- Add structured run reward payloads.
- Support currencies/materials at minimum.
- Add milestone note.

### Milestone 034 — implement basic soft currency
- Add one soft currency resource.
- Grant it from runs.
- Persist it safely.
- Add tests for gain/spend basics.
- Add milestone note.

### Milestone 035 — implement one material category
- Add one region/material type or equivalent non-generic reward layer.
- Grant it from appropriate nodes.
- Add milestone note.

### Milestone 036 — implement post-run reward summary UI
- Show rewards and progress deltas after runs.
- Keep it aggregated and readable.
- Add milestone note.

### Milestone 037 — differentiate ordinary vs milestone rewards
- Make clear/map unlock or other milestone reward moments feel distinct.
- Add milestone note.

---

## Phase 8 — progression sink baseline

### Milestone 038 — implement one account-wide progression sink
- Add one simple persistent upgrade board or project sink.
- Spend resources for permanent benefit.
- Add tests for upgrade purchase/application.
- Add milestone note.

### Milestone 039 — connect progression sink to combat outcomes
- Ensure upgrades change future run performance visibly.
- Keep the upgrade count small.
- Add milestone note.

### Milestone 040 — implement one push-oriented upgrade
- Add an upgrade that clearly helps harder content.
- Example: survivability or damage baseline.
- Add milestone note.

### Milestone 041 — implement one farm-oriented upgrade
- Add an upgrade that clearly helps farming comfort or efficiency.
- Example: reward efficiency or automation comfort later if already available.
- Add milestone note.

---

## Phase 9 — characters baseline

### Milestone 042 — implement persistent character model
- Add one playable character with persistent state.
- Connect it to stats and run entry.
- Add milestone note.

### Milestone 043 — implement character selection model placeholder
- If only one character exists, still create extensible selection handling.
- Keep UI minimal.
- Add milestone note.

### Milestone 044 — implement character-linked progression
- Add one persistent way for the character to become stronger over time.
- Keep it simple.
- Add milestone note.

### Milestone 045 — implement second character only if needed
- Add a second character only when the first one is stable and the milestone goal is to prove character differentiation.
- Request art/animation assets if required.
- Add milestone note.

---

## Phase 10 — skills baseline

### Milestone 046 — implement baseline attack as skill-compatible system
- Make the basic attack compatible with the future skill model.
- Keep it simple and automated.
- Add milestone note.

### Milestone 047 — implement one passive skill layer
- Add one passive skill effect that changes combat meaningfully.
- Add tests for its rule if isolated.
- Add milestone note.

### Milestone 048 — implement one auto-triggered active skill
- Add one triggered or periodic skill.
- Keep activation fully automation-compatible.
- Add milestone note.

### Milestone 049 — implement build-facing skill selection or assignment
- Add minimal skill/loadout assignment flow if needed by current design.
- Keep it readable and small.
- Add milestone note.

### Milestone 050 — implement run-time upgrade affecting skills
- Add at least one run-time choice that modifies a skill or attack pattern.
- Add milestone note.

---

## Phase 11 — gear baseline

### Milestone 051 — implement persistent gear data model
- Add one minimal gear category.
- Make it persist between runs.
- Add milestone note.

### Milestone 052 — implement equipping gear before runs
- Add a minimal equip/assignment flow.
- Keep it compatible with one character.
- Add milestone note.

### Milestone 053 — implement gear stat/effect impact
- Make equipped gear visibly change combat results.
- Add tests for gear effect calculation if isolated.
- Add milestone note.

### Milestone 054 — implement second gear slot/category only if needed
- Add one more gear category only when the first proves useful.
- Keep slot count low.
- Add milestone note.

---

## Phase 12 — enemies expansion

### Milestone 055 — implement standard enemy data variety
- Add at least one more standard enemy profile.
- Differentiate through durability or threat.
- Add milestone note.

### Milestone 056 — implement enemy profile differences in combat
- Make different enemies produce visibly different combat pressure.
- Add milestone note.

### Milestone 057 — map enemy profiles to nodes/locations
- Connect enemy type selection to node/location content.
- Keep initial mapping simple.
- Add milestone note.

---

## Phase 13 — bosses baseline

### Milestone 058 — implement boss entity/encounter model
- Add one gate-boss encounter model.
- Keep it compatible with the existing combat foundation.
- Add milestone note.

### Milestone 059 — connect boss to progression gate
- Make boss defeat unlock meaningful new progression.
- Keep output visible.
- Add tests for gate unlock if isolated.
- Add milestone note.

### Milestone 060 — add boss reward differentiation
- Make boss rewards feel more important than ordinary clears.
- Add milestone note.

---

## Phase 14 — towns and service hub baseline

### Milestone 061 — implement town/service context shell
- Add one town-equivalent service context.
- It can be a service screen or a world node depending on the chosen MVP structure.
- Add milestone note.

### Milestone 062 — connect progression sink to town/service layer
- Move or expose the persistent upgrade sink through the town/service context.
- Keep interaction short.
- Add milestone note.

### Milestone 063 — connect gear/build preparation to town/service layer
- Allow pre-run build/gear management through the town/service context.
- Add milestone note.

---

## Phase 15 — powerup mechanics baseline

### Milestone 064 — implement one project-style powerup mechanic
- Add one persistent project/upgrade mechanic beyond direct node unlocks.
- Connect resource inputs to lasting benefits.
- Add milestone note.

### Milestone 065 — implement one conversion/refinement mechanic
- Add one simple conversion/refinement flow if it adds clear value.
- Keep it small.
- Add milestone note.

### Milestone 066 — connect materials to powerup mechanic
- Ensure repeated farming materials now have a clear use path.
- Add milestone note.

---

## Phase 16 — world identity and location differentiation

### Milestone 067 — implement at least two location identities in content data
- Add two distinct region/location identities.
- Tie them to nodes and rewards.
- Add milestone note.

### Milestone 068 — implement basic location-based enemy/resource differentiation
- Make locations differ through enemy emphasis, material emphasis, or both.
- Add milestone note.

### Milestone 069 — implement old-location relevance rule in content
- Ensure earlier locations still provide useful value after deeper progression opens.
- Add milestone note.

---

## Phase 17 — UX / HUD baseline

### Milestone 070 — implement readable world-state UI
- Show current region, node states, and reachable paths clearly.
- Keep the view simple.
- Add milestone note.

### Milestone 071 — implement readable run HUD baseline
- Show health, node/run context, and progress context.
- Keep it minimal and readable.
- Add milestone note.

### Milestone 072 — implement upgrade choice UI
- Add one readable in-run choice panel.
- Keep decisions quick to parse.
- Add milestone note.

### Milestone 073 — improve post-run next-action clarity
- Make replay/push/upgrade/stop decisions obvious.
- Add milestone note.

---

## Phase 18 — save/persistence hardening

### Milestone 074 — implement autosave at resolved post-run boundary
- Persist world and progression after safe milestones.
- Add milestone note.

### Milestone 075 — persist build/character/gear state cleanly
- Ensure build preparation survives restarts.
- Add milestone note.

### Milestone 076 — validate safe resume flow
- Confirm the game restores into a clear world/service context.
- Add milestone note.

### Milestone 077 — add minimal offline-progress compatibility hooks
- Prepare save state for future offline progress even if offline rewards are not yet enabled.
- Add milestone note.

---

## Phase 19 — farming comfort and automation improvements

### Milestone 078 — implement low-friction replay flow
- Add quick replay or repeat-node support.
- Keep it aligned with current session flow.
- Add milestone note.

### Milestone 079 — implement auto-pick baseline if desired
- Add minimal auto-pick for in-run upgrade choices if the current core loop is stable enough.
- Add milestone note.

### Milestone 080 — mark or detect farm-ready content
- Add one simple rule for content that is suitable for low-friction farming.
- Add milestone note.

### Milestone 081 — add one automation comfort upgrade
- Connect progression to better farming comfort without breaking push/farm distinction.
- Add milestone note.

---

## Phase 20 — loot and gear loop expansion

### Milestone 082 — implement gear-as-loot acquisition if not already present
- Add gear drop/reward support in a controlled simple form.
- Add milestone note.

### Milestone 083 — implement basic duplicate/conversion handling if needed
- Add only if repeated gear/material rewards are becoming dead value.
- Keep it simple.
- Add milestone note.

### Milestone 084 — implement milestone reward presentation polish
- Differentiate boss/clear/reward spikes more clearly in UI and data flow.
- Add milestone note.

---

## Phase 21 — boss and challenge expansion

### Milestone 085 — expand boss readability and structure
- Improve boss encounter clarity without adding unnecessary complexity.
- Add milestone note.

### Milestone 086 — add one optional challenge/elite path if the core gate-boss works
- Keep it subordinate to the main progression loop.
- Add milestone note.

---

## Phase 22 — audio baseline

### Milestone 087 — implement basic UI/system feedback sounds
- Add clicks, confirmations, and important state-change sounds.
- Request assets if required and missing.
- Add milestone note.

### Milestone 088 — implement readable combat SFX baseline
- Add core combat audio for attacks, hits, defeats, and danger feedback.
- Request assets if required and missing.
- Add milestone note.

### Milestone 089 — implement basic music context split
- Add at least one gameplay music context and one calmer town/menu/world context.
- Request assets if required and missing.
- Add milestone note.

### Milestone 090 — implement milestone audio cues
- Add stronger audio confirmation for unlocks, clears, and boss rewards.
- Add milestone note.

---

## Phase 23 — visual readability pass

### Milestone 091 — implement baseline player/enemy readability visuals
- Ensure player-side and enemy-side entities are clearly distinguishable.
- Request sprites/animations if required and missing.
- Add milestone note.

### Milestone 092 — implement two distinct location visual identities
- Add at least two visually distinct location/region looks.
- Request missing sprites/backgrounds if required.
- Add milestone note.

### Milestone 093 — implement service/town visual contrast
- Make towns/service spaces feel calmer and distinct from combat.
- Add milestone note.

### Milestone 094 — improve combat effect readability under autobattle
- Reduce clutter and make important actions legible.
- Add milestone note.

---

## Phase 24 — menu/settings completion

### Milestone 095 — implement compact main menu
- Add start/continue/settings/quit flow.
- Add milestone note.

### Milestone 096 — implement compact pause/system menu
- Add resume/settings/exit flow where appropriate.
- Add milestone note.

### Milestone 097 — implement basic audio/display settings persistence
- Persist essential settings reliably.
- Add milestone note.

---

## Phase 25 — content breadth expansion

### Milestone 098 — add more nodes to prove world structure depth
- Expand node count while keeping graph readable.
- Add milestone note.

### Milestone 099 — add another region/location with distinct value
- Expand world progression breadth.
- Request assets if required and missing.
- Add milestone note.

### Milestone 100 — add another enemy family/profile set
- Strengthen location/world identity through enemy diversity.
- Add milestone note.

### Milestone 101 — add one more meaningful progression sink or powerup branch
- Expand long-term goals without fragmenting systems.
- Add milestone note.

### Milestone 102 — add second character only if the game now benefits from roster differentiation
- Request required sprites/animations/audio if missing.
- Add milestone note.

---

## Phase 26 — offline progress (optional after core loop stability)

### Milestone 103 — implement minimal offline-progress eligibility model
- Tie offline progress only to clearly farm-ready contexts.
- Add milestone note.

### Milestone 104 — implement offline summary claim flow
- Show readable offline gains on return.
- Keep outputs limited to farming-support value.
- Add milestone note.

### Milestone 105 — tune offline limits to preserve active push value
- Ensure offline systems do not replace active progression.
- Add milestone note.

---

## Phase 27 — quality and cohesion pass

### Milestone 106 — tighten spec/code alignment
- Review implemented systems against specs.
- Fix mismatches without broad redesign.
- Add milestone note.

### Milestone 107 — tighten test coverage around milestone-critical rules
- Add missing unit coverage in core systems.
- Focus on brittle or branching logic.
- Add milestone note.

### Milestone 108 — tighten asset completeness for actually implemented systems
- Request or integrate missing sprites, animations, and music for completed systems.
- Add milestone note.

### Milestone 109 — tighten UX clarity for current system breadth
- Remove friction and ambiguity in world map, HUD, rewards, and progression access.
- Add milestone note.

### Milestone 110 — stabilize first full spec-covered playable version
- Ensure the current implemented scope forms one coherent playable build.
- Ensure all milestone notes exist.
- Ensure no half-implemented major systems remain in the delivered scope.
- Add milestone note.

---

## Asset request checkpoints
At these milestone groups, Codex should explicitly request missing assets if placeholders are no longer sufficient:
- character readability milestones
- enemy readability milestones
- location visual identity milestones
- combat VFX/readability milestones
- audio/music milestones
- animation-dependent presentation milestones

If a milestone’s purpose depends on those assets, the milestone is blocked until they are provided or the scope is reduced explicitly.

---

## Review checkpoints
Pause for review after these groups:
- Phase 2 complete
- Phase 6 complete
- Phase 10 complete
- Phase 15 complete
- Phase 19 complete
- Phase 24 complete
- Phase 27 complete

These are good moments to re-check specs before continuing.

---

## Milestone completion rule
Each milestone is complete only when:
- the scoped feature works end-to-end
- directly related code is added
- directly related tests are added where applicable
- directly required assets are wired or explicitly requested
- the milestone note is created in `specs/milestone/`
- no commit is created

---

## Final rule
Do not skip ahead because a later milestone seems related.
Implement one small complete slice.
Document it.
Stop.
Wait for the next instruction.
