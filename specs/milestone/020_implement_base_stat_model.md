# Milestone 020 - Implement base stat model

- Added `CombatStatBlock` as the minimal shared runtime stat model for combat entities, covering max health, attack power, attack rate, and defense.
- Added `CombatStatCalculator` to provide the first effective stat calculations needed by combat: attack interval from attack rate and a bounded defense-based mitigation rule for incoming damage.
- Updated `CombatEntityState` so both player-side and enemy-side combat entities now carry shared base stats, and `CombatShellContextFactory` now seeds simple player/enemy baseline stat profiles for the combat shell.
- `CombatShellView` now surfaces the placeholder combat stats directly so the combat shell shows health, offense, speed, and survivability context in runtime UI.
- Added Edit Mode coverage for stat-block validation, attack timing calculation, defense mitigation behavior, and combat-shell/entity integration with shared stats.
- Verified with a real Unity batch Edit Mode run. `Logs/m020_editmode_results.xml` was produced and the run completed with `68` tests passed and exit code `0`.
- This milestone does not add attack resolution, damage exchange loops, targeting, movement, crit, penetration, status effects, or reward formulas yet. It only establishes the shared base stat layer needed for later combat milestones.
