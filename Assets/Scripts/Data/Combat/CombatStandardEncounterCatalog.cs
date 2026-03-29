namespace Survivalon.Data.Combat
{
    public static class CombatStandardEncounterCatalog
    {
        private static readonly CombatStandardEncounterDefinition EnemyUnitEncounterDefinition =
            new CombatStandardEncounterDefinition(
                "combat_encounter_enemy_unit",
                CombatStandardEnemyProfileCatalog.EnemyUnit);
        private static readonly CombatStandardEncounterDefinition BulwarkRaiderEncounterDefinition =
            new CombatStandardEncounterDefinition(
                "combat_encounter_bulwark_raider",
                CombatStandardEnemyProfileCatalog.BulwarkRaider);
        private static readonly CombatStandardEncounterDefinition RuinSentinelEncounterDefinition =
            new CombatStandardEncounterDefinition(
                "combat_encounter_ruin_sentinel",
                CombatStandardEnemyProfileCatalog.RuinSentinel);

        public static CombatStandardEncounterDefinition EnemyUnitEncounter => EnemyUnitEncounterDefinition;

        public static CombatStandardEncounterDefinition BulwarkRaiderEncounter => BulwarkRaiderEncounterDefinition;

        public static CombatStandardEncounterDefinition RuinSentinelEncounter => RuinSentinelEncounterDefinition;
    }
}
