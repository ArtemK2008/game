namespace Survivalon.Data.Combat
{
    public static class CombatBossEncounterCatalog
    {
        private static readonly CombatBossEncounterDefinition GateBossEncounterDefinition =
            new CombatBossEncounterDefinition(
                "combat_encounter_gate_boss",
                "boss_gate_001",
                CombatBossRoleType.GateBoss,
                CombatBossProfileCatalog.GateBoss);

        public static CombatBossEncounterDefinition GateBossEncounter => GateBossEncounterDefinition;
    }
}
