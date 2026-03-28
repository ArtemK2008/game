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
- `Assets/Art/Locations/CavernServiceHub/Backgrounds/service_background.png`

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
- Enemy combat sheets were normalized into canonical split state files while retaining the source sheets.
- Current location/service backgrounds were left in place and documented as the canonical gameplay-facing files.
- Current combat VFX assets were intentionally left as sheets because later hookup still needs explicit cue/frame mapping.

## Remaining gaps for later milestones
- No visual runtime hookup is implemented yet for milestones `091` through `094`.
- No dedicated world-map art assets are prepared yet.
- No explicit per-frame VFX slicing contract exists yet.
- No additional service/town-safe-space background variants are prepared yet.
