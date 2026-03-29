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
- `Assets/Art/WorldMap/Nodes/locked.png`
- `Assets/Art/WorldMap/Nodes/available.png`
- `Assets/Art/WorldMap/Nodes/current.png`
- `Assets/Art/WorldMap/Nodes/cleared.png`

Retained source:
- `Assets/Art/WorldMap/Nodes/node_states_sheet.png`

Current split order:
- left-to-right source panels -> `locked`, `available`, `current`, `cleared`

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
- The authored world-map presentation sheet was normalized into canonical split node-state files while retaining the source sheet.
- Current combat VFX assets remain intentionally sheet-based canonical source files, and milestone `094` now uses those sheets directly for restrained combat readability cues without splitting them into a broader sequence pipeline.

## Current hookup status
- Milestone `091` now wires the current canonical player and enemy combat-state sprites into the live combat shell.
- Milestone `100` now wires the current canonical `RuinSentinel` state set into live `Sunscorch Ruins` combat content.
- Milestone `092` and `099` now wire the current canonical combat backgrounds for `VerdantFrontier`, `EchoCaverns`, and `SunscorchRuins` into the live combat shell.
- Milestone `093` now wires the current canonical `CavernServiceHub` service background into the live town/service shell.
- Follow-up `106a` now wires the authored world-map background and canonical node-state icons into the live world-map presentation.
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
  - `Assets/Art/WorldMap/Nodes/locked.png`
  - `Assets/Art/WorldMap/Nodes/available.png`
  - `Assets/Art/WorldMap/Nodes/current.png`
  - `Assets/Art/WorldMap/Nodes/cleared.png`
- Portraits and world icons remain prepared assets only and are not wired into runtime yet.
- Milestone `094` now wires the current combat VFX sheets directly into the live combat shell for:
  - baseline impact readability
  - `Burst Strike`
  - low-health danger
  - defeat

## Remaining gaps for later milestones
- No additional world-map node-state variants beyond `locked`, `available`, `current`, and `cleared` are prepared yet.
- No explicit per-frame VFX slicing contract exists yet.
- No additional service/town-safe-space background variants are prepared yet.
- Runtime hookup for broader location backgrounds beyond the current shipped combat/service subset is still pending for later milestones.
