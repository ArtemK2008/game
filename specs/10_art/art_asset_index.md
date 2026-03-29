# Art Asset Index

## Purpose
Provide one compact source-of-truth index for the current gameplay-facing art assets prepared under `Assets/Art/` for upcoming visual hookup milestones.

This index is intentionally small.
Use it together with the focused pipeline specs in `specs/10_art/`.

## Current prepared asset categories

### Playable characters
Pipeline spec:
- `specs/10_art/character_sprite_and_animation_pipeline.md`

Root:
- `Assets/Art/Characters/`

Current characters:
- `Vanguard`
- `Striker`
- `Duelist`
- `Arcanist`

Canonical outputs per character:
- `Portrait/portrait.png`
- `WorldIcon/world_icon.png`
- `Sprites/idle.png`
- `Sprites/attack.png`
- `Sprites/hit.png`
- `Sprites/defeat.png`

Optional retained source:
- `Sprites/combat_states_sheet.png`

### Enemies
Pipeline spec:
- `specs/10_art/enemy_sprite_pipeline.md`

Root:
- `Assets/Art/Enemies/`

Current enemy families:
- `EnemyUnit`
- `BulwarkRaider`
- `GateBoss`
- `RuinSentinel`

Canonical gameplay-facing outputs per enemy:
- `Sprites/idle.png`
- `Sprites/attack.png`
- `Sprites/hit.png`
- `Sprites/defeat.png`

Retained source per enemy:
- `Sprites/combat_states_sheet.png`

### Location and service backgrounds
Pipeline spec:
- `specs/10_art/environment_and_background_pipeline.md`

Root:
- `Assets/Art/Locations/`

Current canonical files:
- `Assets/Art/Locations/VerdantFrontier/Backgrounds/combat_background.png`
- `Assets/Art/Locations/EchoCaverns/Backgrounds/combat_background.png`
- `Assets/Art/Locations/SunscorchRuins/Backgrounds/combat_background.png`
- `Assets/Art/Locations/CavernServiceHub/Backgrounds/service_background.png`

### World map presentation art
Pipeline spec:
- `specs/10_art/environment_and_background_pipeline.md`

Root:
- `Assets/Art/WorldMap/`

Current canonical files:
- `Assets/Art/WorldMap/Backgrounds/world_map_background.png`
- `Assets/Art/WorldMap/Nodes/ordinary_combat.png`
- `Assets/Art/WorldMap/Nodes/farm.png`
- `Assets/Art/WorldMap/Nodes/elite.png`
- `Assets/Art/WorldMap/Nodes/boss_gate.png`
- `Assets/Art/WorldMap/Nodes/service.png`
- `Assets/Art/WorldMap/Nodes/locked.png`
- `Assets/Art/WorldMap/Nodes/current.png`
- `Assets/Art/WorldMap/Nodes/region_transition.png`

Retained source:
- `Assets/Art/WorldMap/Source/world_nodes_sheet_v2.png`

Current split order:
- strict 4x2 equal-cell grid, left-to-right then top-to-bottom:
  - `ordinary_combat`
  - `farm`
  - `elite`
  - `boss_gate`
  - `service`
  - `locked`
  - `current`
  - `region_transition`

### Combat VFX
Pipeline spec:
- `specs/10_art/combat_vfx_pipeline.md`

Root:
- `Assets/Art/VFX/Combat/`

Current canonical sheet assets:
- `Assets/Art/VFX/Combat/basic_combat_cues_sheet.png`
- `Assets/Art/VFX/Combat/burst_strike_effect_sheet.png`
- `Assets/Art/VFX/Combat/danger_pulse_sheet.png`
- `Assets/Art/VFX/Combat/defeat_effect_sheet.png`

## Current prep status
- Player character assets were already in canonical gameplay-facing form.
- Enemy combat sheets were normalized into canonical split state files while retaining the source sheets, with `RuinSentinel` now promoted from prepared art to live runtime use.
- Current location/service backgrounds were left in place and documented as the canonical gameplay-facing files.
- The authored world-map presentation sheet was normalized into canonical split node-icon files while retaining the source sheet.
- Current combat VFX assets remain intentionally sheet-based canonical source files, and milestone `094` now uses those sheets directly for restrained combat readability cues without splitting them into a broader sequence pipeline.

## Current hookup status
- Milestone `091` now wires the current canonical player and enemy combat-state sprites into the live combat shell.
- Milestone `100` now wires the current canonical `RuinSentinel` state set into live `Sunscorch Ruins` combat content.
- Milestone `092` and `099` now wire the current canonical combat backgrounds for `VerdantFrontier`, `EchoCaverns`, and `SunscorchRuins` into the live combat shell.
- Milestone `093` now wires the current canonical `CavernServiceHub` service background into the live town/service shell.
- Follow-up `106a` wires the authored world-map background into the live world-map presentation.
- Follow-up `106b` upgrades the live world-map node icons to the authored eight-slice meaning-first set.
- That runtime hookup currently uses only the combat-state files under:
  - `Assets/Art/Characters/<CharacterName>/Sprites/`
  - `Assets/Art/Enemies/<EnemyName>/Sprites/`
- plus the combat background files under:
  - `Assets/Art/Locations/VerdantFrontier/Backgrounds/combat_background.png`
  - `Assets/Art/Locations/EchoCaverns/Backgrounds/combat_background.png`
  - `Assets/Art/Locations/SunscorchRuins/Backgrounds/combat_background.png`
- plus the current service background file under:
  - `Assets/Art/Locations/CavernServiceHub/Backgrounds/service_background.png`
- plus the world-map presentation files under:
  - `Assets/Art/WorldMap/Backgrounds/world_map_background.png`
  - `Assets/Art/WorldMap/Nodes/ordinary_combat.png`
  - `Assets/Art/WorldMap/Nodes/farm.png`
  - `Assets/Art/WorldMap/Nodes/elite.png`
  - `Assets/Art/WorldMap/Nodes/boss_gate.png`
  - `Assets/Art/WorldMap/Nodes/service.png`
  - `Assets/Art/WorldMap/Nodes/locked.png`
  - `Assets/Art/WorldMap/Nodes/current.png`
- `Assets/Art/WorldMap/Nodes/region_transition.png` currently remains prepared canonical art only and is not runtime-used yet because the current world graph does not expose a separate honest region-transition node meaning.
- Milestone `108` now wires the authored `Vanguard` / `Striker` world icons into the live world-map build-preparation character-selection surface through `Assets/Resources/PlayableCharacterWorldIconRegistry.asset`.
- Character portraits remain prepared-only because the current shipped build still does not expose a portrait-specific runtime surface.
- Milestone `094` now wires the current combat VFX sheets directly into the live combat shell for:
  - baseline impact readability
  - `Burst Strike`
  - low-health danger
  - defeat

## Remaining gaps for later milestones
- `region_transition.png` is prepared but not runtime-used yet because the current shipped world graph does not expose a dedicated region-transition node meaning separate from service/combat/boss semantics.
- No explicit per-frame VFX slicing contract exists yet.
- No additional service/town-safe-space background variants are prepared yet.
- Runtime hookup for broader location backgrounds beyond the current shipped combat/service subset is still pending for later milestones.
- No additional repo-contained music clips exist beyond the current shipped calm/gameplay split, so broader location- or service-specific music remains unavailable and the runtime safely falls back to `music_calm_loop.wav` / `music_gameplay_loop.wav`.
- No shipped runtime system currently uses animation controllers or clip-driven character/enemy animation assets; the live combat shell still intentionally relies on state-sprite switching.
