# Sound and Music

## Purpose
Define the sound and music direction for the first playable version and establish the structural rules that later combat, locations, UX, towns, rewards, automation, and menu/settings specs must follow.

---

## Scope
This spec defines:
- what sound and music must achieve in this project
- audio priorities for a lazy/autobattle progression game
- relationship between audio and readability
- relationship between audio and world/location identity
- minimal MVP sound/music requirements
- constraints for later specs

This spec does not define:
- final soundtrack list
- exact compositions
- exact sound library
- exact middleware setup
- exact mixing pipeline
- exact implementation technology
- exact voiceover content

---

## Core statement
Sound and music must support a grind-heavy world-progression autobattler by improving readability, reinforcing world identity, and making repeated play comfortable over long sessions.

Audio must:
- support gameplay readability,
- reinforce location and progression identity,
- make combat outcomes easier to understand,
- remain comfortable during repeated farming,
- provide contrast between combat, world navigation, and service/town contexts,
- stay scalable as more content is added.

Audio is not only mood.
It is also a functional feedback layer.

---

## Core audio rule
Default audio rule:

`important gameplay state changes must be audible enough to support player understanding without forcing constant visual focus`

If audio design conflicts with clarity or long-session comfort, clarity and comfort win.

---

## Audio design principles
- Readability is more important than spectacle.
- Repeated play must remain comfortable.
- Important state changes should be audible.
- Audio should reinforce, not duplicate or clutter, visual feedback.
- The first version should prefer a small number of strong audio roles.
- Music should support session rhythm without becoming tiring.
- Sound layers must remain scalable for future content growth.
- Audio density must remain controlled in autobattle contexts.

---

## Audio system role in the game
Sound and music are one of the main ways the game expresses:
- combat state and impact
- reward and progression significance
- world/location identity
- contrast between danger and safety
- long-session pacing and comfort

If audio is weak or poorly structured:
- autobattle feels less readable,
- locations feel less memorable,
- repeated farming becomes more monotonous,
- milestone moments feel less important.

---

## Primary audio goals
The sound/music layer must support these goals.

### 1. Combat readability
The player should hear whether combat is active, successful, dangerous, or resolved.

### 2. World/location identity
Different locations and contexts should feel distinct through audio mood.

### 3. Session comfort
Repeated farming should remain sonically comfortable during long sessions.

### 4. Milestone emphasis
Bosses, unlocks, clear states, and major rewards should feel more significant than ordinary actions.

### 5. Context contrast
World map, combat, town/service, reward, and menu contexts should feel different enough to support orientation.

---

## Relationship between sound/music and core loop
Audio must support the loop:

`choose node -> run autobattle -> gain rewards/progress -> upgrade -> continue`

Required behavior:
- world-level navigation should feel different from combat
- combat should provide readable action feedback
- post-run reward/progress state should have clear audio confirmation
- service/town states should feel calmer and more planning-oriented

The audio layer should reinforce the emotional rhythm between:
- push
- stabilize
- upgrade
- repeat

---

## Relationship between sound/music and UX readability
Audio must support the information hierarchy from `ux_hud_information_flow`.

Required behavior:
- important feedback should be audible even if the player is not focused on every visual detail
- routine repeated events should not flood the player with noise
- major state changes should have stronger audible emphasis than ordinary repeated events

Audio should help the player answer:
- is this run going well?
- did something important happen?
- was something unlocked or completed?
- am I in danger or in a safe planning context?

---

## Relationship between sound/music and combat
Combat audio must make autobattle easier to understand.

Required behavior:
- offensive actions should have readable feedback
- enemy hits / player damage / danger states should be perceptible
- enemy defeats should be recognizable
- milestone combat moments should feel heavier than ordinary farming kills
- audio density should not become chaotic during high-efficiency farming

The player should not have to manually act, but should still be able to hear whether the build is functioning well.

---

## Relationship between sound/music and automation
Because combat is automated, sound must help build trust in the system.

Required behavior:
- repeated combat loops should sound intentional rather than random
- strong automation should still produce clear audible rhythm
- sound spam should be controlled so the player can tolerate long farming sessions
- the player should be able to hear when an unusual or important event interrupts ordinary flow

Autobattle-friendly audio clarity is a core requirement.

---

## Relationship between sound/music and locations
Locations are one of the main anchors of audio identity.

Required behavior:
- different locations/regions should be able to support different musical or ambient moods
- location audio should reinforce the feeling that places matter
- earlier locations should remain recognizable when revisited

Examples of location audio identity anchors:
- ambient texture
- instrumentation bias
- rhythmic intensity
- tonal palette
- environmental sound motifs

Exact composition and production choices belong later.

---

## Relationship between sound/music and towns/service spaces
Towns/service contexts should contrast clearly with combat.

Required behavior:
- towns/service spaces should sound calmer and lower-pressure
- town/service audio should support planning and readability
- entering a service context should feel like a change of state, not more combat pressure

This contrast is important for session rhythm and fatigue reduction.

---

## Relationship between sound/music and progression
Audio should help progression feel tangible.

Required behavior:
- node clears, unlocks, milestone rewards, and boss defeats should have stronger audible confirmation than routine actions
- deeper or more important content may later have stronger or distinct musical identity
- stronger builds or stronger content may feel different through sound emphasis, not only visuals

Audio should help the player feel forward movement.

---

## Relationship between sound/music and rewards
Rewards should have readable audio treatment.

Required behavior:
- routine gains may use light confirmation
- milestone rewards should feel more significant
- reward audio should not become annoying during repeated farming

The first version should prefer restrained and readable reward sound design over constant reward noise.

---

## Push vs farm audio rhythm
Audio should help distinguish:

### Push context
Expected feel:
- more tension
- stronger danger cues
- more weight on important combat beats

### Farm context
Expected feel:
- more comfort
- more repetition tolerance
- less stress-heavy emphasis

This distinction should come from audio weighting and music mood, not only explicit labels.

---

## Audio layer categories
The system should support a small number of clear audio layers.

### 1. Music layer
Purpose:
- support world/combat/town/session mood
- shape high-level emotional pacing

### 2. Combat SFX layer
Purpose:
- communicate attacks, hits, defeats, danger, and combat activity

### 3. UI/system feedback layer
Purpose:
- communicate clicks, selections, confirmations, unlocks, and progression events

### 4. Ambience/environment layer
Purpose:
- reinforce locations and calm/world identity

### 5. Milestone emphasis layer
Purpose:
- highlight bosses, clears, unlocks, and important rewards

MVP does not need deep layering complexity, but these roles must be structurally possible.

---

## Music direction principles
Music should:
- support location identity
- support mode/context contrast
- remain listenable over repeated sessions
- avoid overpowering moment-to-moment feedback
- support gradual progression into deeper content

Music should not:
- demand constant attention
- become exhausting during long farming cycles
- blur all locations and contexts into one mood

MVP can use a small set of tracks if they map cleanly to major contexts.

---

## Sound-effect direction principles
Sound effects should:
- be short and readable
- differentiate routine vs important events
- avoid excessive overlap noise
- support quick state understanding
- remain comfortable during repeated loops

SFX should not:
- make the screen feel louder than it is readable
- treat every ordinary event as equally important
- produce fatigue through constant high-intensity repetition

---

## Audio density rule
Do not overfill the game with simultaneous high-priority sounds.

Required principles:
- routine events should have lighter audio weight than milestone events
- repeated farming loops should remain sonically tolerable
- overlapping sounds should remain controllable in density and prominence
- the first version should prefer fewer stronger cues over many weak noisy cues

---

## Silence / calm-space rule
Not every context needs constant intense audio.

Required principle:
- calm/service/menu/world states may intentionally use lighter audio density or calmer music
- contrast is useful for pacing, fatigue reduction, and emphasis

The game should not feel sonically “busy” all the time.

---

## MVP requirements
The first playable version must support:
- at least one music context for gameplay and one contrasting calmer context for service/menu/world use, or an equivalent minimal structure
- readable combat sound feedback
- readable UI/system confirmation sounds
- clear audible distinction for important events such as node clear, unlock, or major reward
- audio settings support through menu/settings
- audio presentation that remains tolerable during repeated farming

MVP may omit:
- large soundtrack variety
- layered adaptive music systems
- full environmental ambience per location
- voiceover
- advanced dynamic mixing systems
- many separate boss themes

---

## MVP priorities
Focus on:
- combat readability
- comfortable repetition
- clear milestone feedback
- context contrast between combat and non-combat states
- simple scalable audio role structure

Avoid in MVP:
- too many sound layers
- fatigue-heavy music loops
- excessive combat sound spam
- soundtrack quantity before core role clarity is proven
- cinematic audio ambitions that slow core iteration

---

## Data/pipeline direction
The audio system should remain compatible with structured content growth.

Required principle:
- contexts such as world/combat/town/boss/reward should be able to map cleanly to audio behavior
- locations and major content types should be able to add distinct audio identity later without replacing the whole system

Exact implementation details belong to technical/audio production planning.

---

## Constraints for later specs
Later specs must not violate the following:
- Do not prioritize audio spectacle over readability.
- Do not make repeated farming sonically exhausting by default.
- Do not make important progression events sound the same as ordinary repeated actions.
- Do not make autobattle feel acoustically chaotic.
- Do not rely on constant high-intensity music to create importance.
- Do not add many audio layers before role clarity is proven.
- Do not let UI or reward sound spam overpower useful combat/world cues.

---

## Extension points
The sound/music layer must support later addition of:
- region-specific music identity
- boss-specific themes
- richer ambience systems
- stronger adaptive combat music
- accessibility-oriented audio options
- more detailed automation/state indicators through sound
- more differentiated milestone/reward sound language

These additions must extend the readable audio foundation, not replace it.

---

## Out of scope
This spec does not define:
- exact soundtrack compositions
- exact SFX asset lists
- exact implementation middleware
- exact mixing values
- exact VO pipeline
- exact audio production workflow

---

## Validation checklist
- [ ] The game has readable combat audio feedback.
- [ ] The game has audible contrast between combat and non-combat/service contexts.
- [ ] Important progression/reward moments can be audibly emphasized.
- [ ] Repeated farming remains sonically tolerable.
- [ ] Audio supports world/location identity at least at a basic level.
- [ ] The sys

## Current audio asset source of truth
Exact currently available shipped audio files and paths are tracked in:

`specs/09_presentation/audio_asset_manifest.json`

Implementation milestones that add or wire audio must use that manifest as the source of truth for available assets and exact repository paths.
If a required asset is missing from the manifest, implementation should request it clearly instead of inventing filenames or placeholder assets unless the milestone explicitly allows placeholders.