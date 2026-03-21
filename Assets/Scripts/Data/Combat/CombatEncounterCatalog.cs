namespace Survivalon.Data.Combat
{
    public static class CombatEncounterCatalog
    {
        private static readonly CombatEncounterDefinition EnemyUnitEncounterDefinition = new CombatEncounterDefinition(
            "combat_encounter_enemy_unit",
            CombatStandardEnemyProfileCatalog.EnemyUnit);
        private static readonly CombatEncounterDefinition BulwarkRaiderEncounterDefinition = new CombatEncounterDefinition(
            "combat_encounter_bulwark_raider",
            CombatStandardEnemyProfileCatalog.BulwarkRaider);
        private static readonly CombatEncounterDefinition GatePlaceholderEncounterDefinition = new CombatEncounterDefinition(
            "combat_encounter_gate_placeholder",
            CombatBossPlaceholderProfileCatalog.GateEnemy);

        public static CombatEncounterDefinition EnemyUnitEncounter => EnemyUnitEncounterDefinition;

        public static CombatEncounterDefinition BulwarkRaiderEncounter => BulwarkRaiderEncounterDefinition;

        public static CombatEncounterDefinition GatePlaceholderEncounter => GatePlaceholderEncounterDefinition;
    }
}
