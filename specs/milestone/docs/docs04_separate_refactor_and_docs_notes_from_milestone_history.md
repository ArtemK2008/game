# Docs Milestone 04 - Separate Refactor And Docs Notes From Milestone History

## Goal
- Separate refactor notes and docs-only notes from the main milestone-history folder so the milestone history is easier to navigate without changing any repository behavior.

## Delivered
- Moved docs-only notes into `specs/milestone/docs/`.
- Moved pure refactor notes into `specs/milestone/refactors/`.
- Kept numbered milestone notes and suffix follow-up milestone notes in `specs/milestone/`.
- Updated the main navigation docs so the new folder split is explicit and easy to follow.

## Behavior Change
- No gameplay, runtime, tooling, Unity-project, or verification behavior changed.
- This was a docs-only repository-organization change for milestone-history navigation clarity.

## Tests
- No automated tests were added or changed.
- No Unity compile/import or EditMode run was required because only docs changed.

## Out Of Scope
- Renaming numbered milestone files or suffix follow-up milestone files.
- Rewriting existing milestone content beyond the smallest path and navigation updates required by the move.
- Any runtime, tooling, test, or project-structure changes outside `specs/`.

## Verification
- Verified the milestone by reviewing the moved note locations and the updated navigation docs.
- Confirmed the change set stayed docs-only.
