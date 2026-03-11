# Codex Delivery Workflow

## Path convention
All paths in this spec are relative to the repository root unless explicitly stated otherwise.

## Purpose
Define how Codex should work on this project, how results should be delivered, how milestones should be structured, and what repository/spec workflow must be followed.

---

## Scope
This spec defines:
- how Codex should use project specs
- how implementation steps should be sized
- how results should be delivered in the repository
- git-related rules
- milestone documentation rules
- asset dependency rules
- completion criteria for each milestone
- constraints for all future implementation work

This spec does not define:
- gameplay design itself
- exact code architecture
- exact CI/CD behavior
- exact branch naming strategy

---

## Core statement
Codex must work in small, complete, spec-driven milestones.

Each milestone must:
- be based on the specs in the `specs/` folder,
- implement one small coherent slice of the project,
- include all project-related code/assets/work needed for that slice to be actually usable,
- include tests for the milestone behavior,
- produce a milestone file in `specs/milestone/`,
- stop before the next unrelated slice begins.

Codex must stage all project-related created/changed files with `git add` before stopping.
Codex must not commit changes.
The user will commit manually.

---

## Core workflow rule
Default workflow:

`read relevant specs in specs/ -> choose one small milestone -> implement the complete milestone -> add/update all required project files -> add milestone note in specs/milestone/ -> git add all project-related created/changed files -> stop without committing`

Do not skip spec reading.
Do not skip milestone documentation.
Do not commit.

---

## Source of truth rule
The source of truth for project decisions is:
- `specs/` directory

Required behavior:
- before implementing a feature, Codex should read the relevant specs
- if multiple specs apply, Codex should align them rather than guessing
- if specs appear to conflict, Codex should surface the conflict instead of silently inventing a new rule

Milestone files are records of completed work, not replacements for specs.

---

## Repository output rule
All project-related implementation artifacts created or changed for a milestone must exist in the working tree and be staged with `git add` before completion.

Required behavior:
- create/update code files
- create/update test files
- create/update Unity asset metadata/files when needed
- create/update spec milestone record
- stage all project-related created/changed files with `git add`
- do not stage local/editor-generated files such as `.idea/` unless explicitly requested
- add local-only files that should not be versioned to `.gitignore`

### Git rule
- **Stage project-related created/changed files with `git add`**
- **Do not create commits**
- **Do not auto-commit**
- **Do not rewrite git history**
- **Do not decide commit boundaries**
- the user performs commits manually

---

## Milestone sizing rule
Milestones must be small.

Small means:
- one coherent slice of functionality
- limited blast radius
- easy to review
- easy to test
- easy to revert or adjust

Milestones must not try to implement a broad system family all at once.

Examples of acceptable milestone scope:
- one stable world-node selection flow
- one basic autobattle combat loop
- one node-progress and unlock rule
- one basic reward screen with connected data flow

Examples of bad milestone scope:
- all combat + all progression + all UI + all assets together
- full character/gear/skill/town system in one step
- broad cleanup/refactor plus new features plus asset overhaul in one step

---

## Milestone completeness rule
Even though milestones must be small, each milestone must be complete for its intended slice.

Complete means:
- the gameplay/code path works end-to-end for the scope of that milestone
- all directly required project files are included
- all directly required supporting assets for that milestone are included if available
- tests for the implemented behavior are included where applicable
- the result is not left intentionally half-wired

Do not leave a milestone in a state where a feature is “almost there” but unusable because obvious directly-related pieces were skipped.

---

## Asset completeness rule
If a milestone requires assets to be meaningfully complete, Codex should ensure the milestone includes them.

Examples:
- sprites
- animations
- sound/music
- prefab hookups
- ScriptableObject data if needed

Required principle:
- if an asset is necessary to make the milestone genuinely complete and the asset is missing, Codex should ask the user for that asset with a clear description of what is needed
- do not silently ignore the missing asset if it makes the milestone incomplete
- do not pretend the milestone is complete when an obvious required asset is absent

---

## Missing asset request rule
When required assets are missing, Codex should ask specifically for them.

The request should include:
- what asset type is needed
- what it will be used for
- minimum required characteristics
- preferred format if relevant

Examples:
- missing sprite sheet for a basic enemy
- missing idle/walk/attack animation frames for a character
- missing music loop for combat or town context
- missing SFX for important combat feedback

Codex should not ask for assets that are optional for the milestone if simple placeholders are acceptable and do not invalidate the milestone.

---

## Placeholder rule
Placeholders are allowed only when they do not undermine the milestone goal.

Allowed placeholder use:
- temporary simple art/audio placeholder for non-critical polish
- temporary primitive visual for testing and implementation flow
- temporary default UI layout if the feature remains usable and reviewable

Disallowed placeholder use:
- placeholder instead of a required asset when the milestone’s purpose depends on that asset being real
- placeholder used to hide that a milestone is functionally incomplete

If the milestone is about a system, placeholders may be acceptable.
If the milestone is about presentation/feel/readability, required presentation assets may need to be requested.

---

## Milestone record rule
After each completed milestone, Codex must create a milestone file in:
- `specs/milestone/`

Each milestone file must contain a short description of:
- what was implemented
- what files/systems were changed at a high level
- what the milestone now enables
- any important limitation or dependency still remaining

The milestone record should be short, factual, and review-friendly.

---

## Milestone file naming rule
Milestone files should be easy to order and review.

Recommended format:
- sequential numbering or date + short slug

Examples:
- `001_basic_world_navigation.md`
- `002_node_progress_and_unlock.md`
- `2026-03-11_basic_autobattle_loop.md`

The naming scheme should stay consistent once chosen.

---

## Spec usage rule per task
For each task, Codex should read only the relevant subset of specs, but must not ignore dependencies.

Required behavior:
- identify the directly relevant spec(s)
- also read any foundational specs they depend on when needed
- implement according to those specs
- avoid unrelated scope creep from non-relevant specs

Example:
If implementing node unlock flow, relevant specs likely include:
- `specs/02_core/core_gameplay_loop.md`
- `specs/02_core/progression/progression_structure_unlock_flow.md`
- `specs/03_world/world_structure.md`
- `specs/03_world/moving_between_maps.md`
- `specs/03_world/what_does_it_mean_to_beat_a_map.md`

---

## Implementation stopping rule
After finishing one milestone, Codex should stop.

Required behavior:
- do not automatically start the next milestone
- do not continue expanding scope because more related work is possible
- wait for the next user instruction after the milestone is completed and documented

This preserves reviewability and control.

---

## Testing rule
Every milestone must include tests for the behavior introduced or changed in that milestone, where testing is applicable.

Required behavior:
- add or update unit tests for milestone behavior
- keep tests focused on the implemented slice
- do not leave milestone logic untested if it can reasonably be tested

If Unity-specific integration/play mode coverage is truly needed, add it when appropriate, but do not replace straightforward unit tests with heavy scene tests unnecessarily.

---

## Refactoring rule during milestone work
Refactoring is allowed only when it directly supports the milestone.

Allowed:
- small refactor to make the milestone implementable
- small refactor to improve clarity of touched code
- small refactor to keep tests sane

Not allowed:
- broad unrelated cleanup
- architecture rewrite because it “might help later”
- opportunistic large refactor unrelated to the current milestone

Milestones should stay focused.

---

## Completion criteria rule
A milestone is complete only if:
- the scoped feature works end-to-end
- directly related code is present
- directly related tests are present where applicable
- directly required assets are present or explicitly requested from the user
- milestone documentation file is created in `specs/milestone/`
- no commit is created

If one of these is missing, the milestone is not complete.

---

## Communication rule for blocked work
If Codex cannot complete a milestone because something essential is missing, it should say so clearly.

Valid blockers include:
- required missing asset
- conflicting specs
- missing repository context
- missing scene/prefab dependency needed for the requested slice

When blocked by missing assets, Codex should ask for the exact asset and describe what is needed.

---

## Repository hygiene rule
Codex should keep repository changes coherent.

Required behavior:
- change only files relevant to the milestone
- avoid accidental unrelated edits
- keep milestone output reviewable
- keep asset/code/spec changes aligned

Do not create noise changes unrelated to the milestone.

---

## Unity-specific workflow rule
Because this is a Unity project, milestone completeness may include:
- scripts
- prefabs
- scenes
- ScriptableObjects
- sprite/audio asset hookups
- animations/controllers
- related meta files where needed

If the milestone depends on Unity asset wiring, Codex should include that wiring as part of the milestone rather than stopping at only raw C# code.

---

## MVP implementation rule
For early milestones, optimize for:
- one small usable slice at a time
- spec compliance
- clean, staged project changes with no commit
- test coverage
- reviewable milestone notes
- minimal but complete asset wiring

Do not optimize early for:
- huge vertical slices
- speculative future systems
- polishing multiple unrelated systems in one milestone

---

## Constraints for all future implementation work
- Stage all project-related created/changed files with `git add`.
- Do not commit.
- Do not skip creating milestone files.
- Do not ignore the `specs/` folder as the source of truth.
- Do not make milestones broad.
- Do not leave directly related work half-done.
- Do not silently ignore missing required assets.
- Do not stage local/editor-generated files such as `.idea/` unless explicitly requested.
- Do not continue to the next milestone without user instruction.
- Do not treat placeholder-only presentation as complete if the milestone requires real presentation assets.

---

## Extension points
This workflow must remain compatible with later addition of:
- stricter milestone templates
- automated changelog generation
- spec-to-milestone traceability links
- checklists per milestone
- richer asset request templates
- review gates or QA notes

These additions must extend the same small-step, spec-driven workflow.

---

## Out of scope
This spec does not define:
- exact git branching model
- exact commit-message format
- exact CI requirements
- exact release packaging workflow

---

## Validation checklist
- [ ] Codex reads relevant specs from `specs/` before implementing.
- [ ] Work is delivered in small milestones.
- [ ] Each milestone is complete for its intended slice.
- [ ] All project-related created/changed files for the milestone are staged with `git add`.
- [ ] No commit is created.
- [ ] A milestone file is created in `specs/milestone/` after each completed step.
- [ ] Missing required assets are explicitly requested from the user.
- [ ] Unity-specific asset wiring is included when required for milestone completeness
