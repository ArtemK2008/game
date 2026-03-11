# Menu and Settings

## Purpose
Define the menu and settings structure for the first playable version and establish the structural rules that later UX, saving, automation, audio, visual style, and technical specs must follow.

---

## Scope
This spec defines:
- what menu and settings systems exist in this project
- the role of menu/settings in the overall player flow
- menu categories
- settings categories
- relationship between menu/settings and the core loop, save model, and UX
- minimal MVP menu/settings requirements
- constraints for later specs

This spec does not define:
- exact final layout
- exact button placement
- exact visual styling
- exact platform-specific integrations
- exact input-binding implementation details
- exact localization implementation

---

## Core statement
Menus and settings are support systems that must reduce friction, improve clarity, and preserve player control over the game experience.

They must:
- provide clean access to the main gameplay loop,
- expose only the settings that materially improve play comfort or readability,
- preserve the game’s low-friction session structure,
- avoid overwhelming the player with configuration complexity in MVP,
- remain extensible for future systems.

Menus and settings are not content.
They are support layers for accessing, controlling, and understanding the game.

---

## Core menu/settings rule
Default structure:

`player enters game -> main menu provides access to core play/profile/settings flows -> in-game menus provide access to pause/settings/system actions -> settings persist where appropriate -> player returns quickly to the core loop`

The system must support quick entry, quick re-entry, and low-friction configuration.

---

## Design principles
- Menus must minimize friction between launching the game and playing it.
- Settings must prioritize meaningful usability over quantity.
- Frequently used actions must be easy to access.
- Rarely used system options must not clutter core play flows.
- The first version should keep menu depth shallow.
- Settings must persist predictably.
- In-game menus must not interrupt the core loop more than necessary.
- The system must remain extensible for future features.

---

## Menu system role in the game
Menus are one of the main support layers for:
- entering the game
- resuming the game
- reaching settings
- pausing or safely exiting
- navigating between core system contexts when not in active combat

If menu structure is weak:
- the loop feels slower to re-enter,
- common actions become tedious,
- settings become hard to trust,
- short sessions become less comfortable.

---

## Menu categories
The menu system should support a small set of clear categories.

### 1. Main menu
Purpose:
- entry point into the game
- access to play/resume/profile/settings/quit flows

Required MVP role:
- support starting or continuing play
- support opening settings
- support quitting cleanly

### 2. Pause / in-game system menu
Purpose:
- allow safe access to system actions while in a run or in a service context

Required MVP role:
- pause when appropriate
- access settings
- return to play
- exit to safe context if allowed

### 3. Profile/progression access menu or screen
Purpose:
- expose persistent build/progression/service layers outside combat

This may overlap structurally with town/service contexts in MVP and does not need to be a separate top-level menu if unnecessary.

### 4. Settings menu
Purpose:
- expose configurable player-facing options

This is required in MVP.

---

## Settings categories
The settings system should support a small set of meaningful categories.

### 1. Audio settings
Purpose:
- control comfort and clarity of game sound

Examples:
- master volume
- music volume
- SFX volume
- mute behaviors later

This category is required in MVP.

### 2. Display / visual comfort settings
Purpose:
- improve readability and comfort

Examples:
- fullscreen/window mode if relevant
- resolution/display mode if relevant
- screen shake or visual intensity reduction later
- simple visual readability toggles later

MVP should expose only what is relevant to the target platform/build.

### 3. Gameplay / automation settings
Purpose:
- control comfort and convenience in the autobattle loop

Examples:
- auto-pick preference later
- pause behavior later
- confirmation behavior later
- low-friction farming options later

MVP may keep this minimal.

### 4. Interface/settings for readability
Purpose:
- improve information clarity

Examples:
- HUD scale later
- compact/detailed reward summary later
- notification intensity later

This is future-compatible and may remain minimal in MVP.

### 5. System settings
Purpose:
- expose broader application-level options

Examples:
- language later
- account/profile options later
- cloud-save-related options later

MVP does not need broad system complexity.

---

## Relationship between menus and the core loop
Menus must support the gameplay loop without competing with it.

Required behavior:
- main menu should lead quickly into world/session flow
- in-game menus should not obscure current context unnecessarily
- settings access should not force deep navigation during common play
- common loop actions should usually happen through world/service screens, not through detached menu structures

Menus are support infrastructure, not the main navigation layer for progression content.

---

## Relationship between settings and persistence
Settings must persist predictably where appropriate.

Required behavior:
- changed settings save or apply in a reliable way
- player should not have to repeatedly reconfigure common preferences
- settings persistence must align with the broader save/persistence model

MVP should prefer simple, trustworthy settings persistence over many partially persistent options.

---

## Relationship between menus and run/session structure
Menus must support short-session and long-session play.

Required behavior:
- resume/continue paths should be quick
- pausing and safe exiting should be understandable
- the player should be able to leave after a stable point without confusion
- re-entry into the game should return the player to a clear context

This is especially important for a game designed to support comfortable repeated sessions.

---

## Relationship between menus and towns/service layers
Town/service layers are gameplay contexts, not just menu layers.

Required principle:
- important progression/service actions should remain grounded in their gameplay context
- menus may provide access to broader system functions, but should not replace all town/service identity

If a feature is part of the game world/service loop, it should not be forced into a generic menu without reason.

---

## Relationship between menus and UX readability
Menu structure must reinforce the information hierarchy defined in `ux_hud_information_flow`.

Required behavior:
- menus should not hide key progression actions behind excessive nesting
- settings should not be the main place where players discover ordinary gameplay meaning
- the player should be able to distinguish game-state screens from system/settings screens

The player should always know whether they are:
- playing,
- preparing,
- reviewing progress,
- configuring the application.

---

## Main menu responsibilities
The main menu should provide at least:
- continue / start play
- access to settings
- exit/quit

Optional later responsibilities:
- profile selection
- credits
- patch notes/news
- save-slot management
- language selection

### MVP rule
The main menu should stay compact and should not front-load non-essential options.

---

## Pause menu responsibilities
The pause/in-game system menu should provide at least:
- resume
- settings access
- exit/back to safe context if allowed

Optional later responsibilities:
- quick help
- automation status panel
- HUD/readability toggles

### Required principle
The pause menu must not become the primary place for ordinary progression decisions.

---

## Settings interaction model
Settings changes should be easy to understand.

Required behavior:
- settings names should be clear
- effect of settings should be understandable
- applied changes should feel predictable
- settings that require confirmation should do so clearly

The first version should avoid many technical settings whose impact is not obvious to most players.

---

## Low-friction rule
Because the game is built around repeatable lazy/autobattle sessions, menus/settings must minimize re-entry friction.

Required behavior:
- fast path from launch to current progression context
- fast path from pause back to play
- no deep mandatory menu traversal before ordinary runs
- common support actions should require few steps

This is one of the most important support-layer goals.

---

## Complexity control rule
Do not overbuild menus/settings in MVP.

### MVP rule
Provide:
- one compact main menu
- one compact pause menu
- one simple settings structure

Do not require:
- many top-level categories
- many rarely used toggles
- many overlapping support screens
- large profile-management flows before they are needed

The first version should prove usability before configurability.

---

## Accessibility / comfort direction
The architecture should remain compatible with later comfort/accessibility additions.

Future-compatible examples:
- reduced visual intensity options
- reduced motion/screen shake options
- text size or HUD readability options
- audio clarity controls
- simplified feedback intensity

MVP does not need a large accessibility suite, but the system should not block it.

---

## Control/input settings direction
Control/input settings depend on the final input model and platform.

### MVP-compatible rule
If the game requires little direct manual control, input settings may remain minimal.

Possible later settings:
- keybinds if needed
- controller support options later
- confirm/cancel behavior settings later

Do not over-prioritize deep input customization before the real input needs are proven.

---

## MVP requirements
The first playable version must support:
- a main menu with start/continue and settings access
- an in-game pause/system menu with resume and settings access
- basic audio settings
- any essential display/window settings relevant to the target platform/build
- persistent saving of settings where appropriate
- clean return from menus to the core game flow

MVP may omit:
- many gameplay toggles
- advanced accessibility layers
- multiple profiles/save-slot UI if not needed
- deep input remapping
- large support/help/codex menu systems

---

## MVP priorities
Focus on:
- fast entry into play
- clear pause/resume behavior
- reliable settings persistence
- a small number of high-value settings
- minimal friction between game launch and current progression context
- clear distinction between gameplay flow and system configuration

Avoid in MVP:
- many low-value toggles
- deeply nested menu trees
- menu features with no clear gameplay support value
- large non-core front-end systems before the main loop is proven
- settings categories that overlap heavily or confuse the player

---

## Data model requirements
Minimum required menu/settings-related data:
- current/resumable play context availability
- settings values
- settings persistence state
- allowable pause/exit actions by context
- menu state routing data

Optional later data:
- save-slot/profile metadata
- accessibility settings state
- advanced automation preference settings
- recent activity shortcuts
- language/localization state

Exact schema belongs to implementation.

---

## Constraints for later specs
Later specs must not violate the following:
- Do not make main progression depend on deep menu navigation.
- Do not make common support actions slower than they need to be.
- Do not expose many settings with unclear player value.
- Do not make menu and service/town contexts blur together without reason.
- Do not require manual save/menu workflows for ordinary session safety.
- Do not make pause/settings interaction disrupt the low-friction loop more than necessary.
- Do not overload the MVP with front-end complexity before gameplay usability is proven.

---

## Extension points
The menu/settings layer must support later addition of:
- more gameplay convenience settings
- accessibility settings
- localization/language support
- profile/save-slot management
- deeper automation preferences
- support/help/codex panels
- advanced audio/visual tuning
- cloud/profile systems

These additions must extend the low-friction support layer, not replace it.

---

## Out of scope
This spec does not define:
- exact screen art/layout
- exact platform integration details
- exact localization workflow
- exact control-remapping UI
- exact cloud-account flow

---

## Validation checklist
- [ ] The game has a compact main menu.
- [ ] The game has a compact pause/system menu.
- [ ] The game has basic settings access.
- [ ] Important settings persist predictably.
- [ ] Menus allow fast entry and re-entry into the core game loop.
- [ ] Menus do not replace gameplay/service contexts unnecessarily.
- [ ] The system can later expand to accessibility, language, and profile features withou