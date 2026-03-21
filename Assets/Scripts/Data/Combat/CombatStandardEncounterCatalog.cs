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

        public static CombatStandardEncounterDefinition EnemyUnitEncounter => EnemyUnitEncounterDefinition;

        public static CombatStandardEncounterDefinition BulwarkRaiderEncounter => BulwarkRaiderEncounterDefinition;
    }
}
