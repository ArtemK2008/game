using System;
using System.Collections.Generic;

namespace Survivalon.Combat
{
    public static class CombatRunTimeSkillUpgradeCatalog
    {
        private static readonly IReadOnlyList<CombatRunTimeSkillUpgradeOption> EmptyUpgradeOptions =
            Array.Empty<CombatRunTimeSkillUpgradeOption>();

        private static readonly IReadOnlyList<CombatRunTimeSkillUpgradeOption> BurstStrikeUpgradeOptions =
            new[]
            {
                new CombatRunTimeSkillUpgradeOption(
                    CombatSkillCatalog.BurstTempo,
                    "Burst Strike triggers faster during this run."),
                new CombatRunTimeSkillUpgradeOption(
                    CombatSkillCatalog.BurstPayload,
                    "Burst Strike hits harder during this run."),
            };

        public static IReadOnlyList<CombatRunTimeSkillUpgradeOption> GetTriggeredActiveSkillUpgradeOptions(
            CombatSkillDefinition triggeredActiveSkill)
        {
            if (triggeredActiveSkill == null)
            {
                return EmptyUpgradeOptions;
            }

            return triggeredActiveSkill.SkillId == CombatSkillCatalog.BurstStrike.SkillId
                ? BurstStrikeUpgradeOptions
                : EmptyUpgradeOptions;
        }
    }
}
