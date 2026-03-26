# Milestone Note Template

## Core Guidance
- Capture the smallest honest step for the milestone.
- State what changed, what did not change, and what was intentionally left out.
- Keep current behavior in `specs/00_overview/current_build_state.md` and keep milestone notes focused on the specific history step.

## Default Feature Milestone
```md
# Milestone NNN - Short Title

## Goal
- What this milestone set out to prove or deliver.

## Delivered
- Concrete shipped changes in this milestone.

## Behavior Change
- What is now true for the player or the runtime flow.

## Tests
- Tests added or updated for the milestone behavior.

## Out Of Scope
- Explicitly left out work and later milestones not consumed here.

## Verification
- Exact verification path used and exact artifact paths.
```

## Short Variants

### Follow-Up Milestone
- Keep the same sections.
- Keep `Behavior Change` explicit, even if it says behavior stayed unchanged and the note only tightened boundaries, naming, or a recent fix.
- Reference the parent milestone in `Goal` or `Out Of Scope`.

### Refactor Milestone
- Replace `Delivered` with the structural ownership/boundary changes if that reads more clearly.
- Make it explicit that player-facing/runtime behavior stayed unchanged unless it truly changed.
- Keep verification focused on the touched area and exact artifact paths.

### Acceptance / Verification-Only Milestone
- Keep `Goal`, `Delivered`, `Out Of Scope`, and `Verification`.
- `Behavior Change` can say that the milestone confirmed existing behavior rather than introducing new behavior.
- `Tests` can describe the verification suite run or say that no code/test changes were required.
