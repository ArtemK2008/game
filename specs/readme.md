# specs/readme.md

## Path convention
All paths in this spec are relative to the repository root unless explicitly stated otherwise.

## Purpose
This folder is the source of truth for project decisions.

Use these specs before implementing any feature.
If multiple specs apply, align them instead of guessing.
When docs overlap, use this precedence:
1. the directly relevant spec files in `specs/`
2. `specs/00_overview/current_build_state.md` for the compact current-state snapshot
3. `specs/milestone/` notes for implementation history and recent detail

---

## Folder structure
- `00_overview/` — high-level project overview and implementation roadmap
- `01_workflow/` — Codex workflow, code style, and process rules
- `02_core/` — core gameplay loop, sessions, modes, progression foundations
- `03_world/` — world structure, locations, movement, map completion
- `04_combat/` — combat, automation, stats, enemies, bosses, skills
- `05_player/` — characters and gear
- `06_economy/` — resources, loot, powerup mechanics
- `07_progression_hubs/` — towns and service-layer systems
- `08_interface/` — UX, menus, settings, persistence/offline flow
- `09_presentation/` — visual style and sound/music
- `10_art/` — gameplay-facing art production rules, asset pipeline specs, sprite/animation delivery contracts
- `milestone/` — numbered milestone notes, suffix follow-up notes, and the milestone status index
- `milestone/refactors/` — behavior-preserving refactor milestone notes
- `milestone/docs/` — docs-only milestone notes

---

## Reading rule
For any task:
1. Read the directly relevant spec.
2. Read its foundational dependencies if needed.
3. Implement only the current small milestone.
4. Add the milestone file in the appropriate location under `specs/milestone/`.
5. Stage all project-related created/changed files with `git add`.
6. Do not commit; the user commits manually.

When a task depends on art assets, also read the relevant files in `10_art/` together with the gameplay-facing spec they support.
Open questions are not permission to invent behavior during implementation; surface the gap or conflict instead.
Use `specs/milestone/status_index.md` to find the current next numbered target, milestone classifications, and the milestone-note folder split.
Use `specs/01_workflow/milestone_note_template.md` when writing a new milestone note.

---

## Naming rule
- Use lowercase file names with underscores.
- Keep names stable and descriptive.
- Milestone files should use a consistent ordered format.

---

## Implementation rule
- Work in small complete milestones.
- Follow specs, do not invent major behavior.
- If required sprites, animation, music, models, or other assets are missing, ask for them clearly.
- Stage all project-related created/changed files with `git add`.
- Add local/editor-generated files that should not be versioned to `.gitignore` instead of staging them.
- No commit is created by Codex; the user commits manually.
