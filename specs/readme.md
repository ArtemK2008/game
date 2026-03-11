# specs/readme.md

## Path convention
All paths in this spec are relative to the repository root unless explicitly stated otherwise.

## Purpose
This folder is the source of truth for project decisions.

Use these specs before implementing any feature.
If multiple specs apply, align them instead of guessing.

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
- `milestone/` — short records of completed implementation steps

---

## Reading rule
For any task:
1. Read the directly relevant spec.
2. Read its foundational dependencies if needed.
3. Implement only the current small milestone.
4. Add a milestone file in `specs/milestone/`.
5. Stage all project-related created/changed files with `git add`.
6. Do not commit; the user commits manually.

---

## Naming rule
- Use lowercase file names with underscores.
- Keep names stable and descriptive.
- Milestone files should use a consistent ordered format.

---

## Implementation rule
- Work in small complete milestones.
- Follow specs, do not invent major behavior.
- If required sprites, animation, music, or other assets are missing, ask for them clearly.
- Stage all project-related created/changed files with `git add`.
- Add local/editor-generated files that should not be versioned to `.gitignore` instead of staging them.
- No commit is created by Codex; the user commits manually.
