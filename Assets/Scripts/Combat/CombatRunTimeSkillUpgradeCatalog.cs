using System;
using System.Collections.Generic;

namespace Survivalon.Combat
{
    public static class CombatRunTimeSkillUpgradeCatalog
    {
        private static readonly IReadOnlyList<CombatRunTimeSkillUpgradeOption> EmptyUpgradeOptions =
            Array.Empty<CombatRunTimeSkillUpgradeOption>();

        public static CombatRunTimeSkillUpgradeOption BurstTempo { get; } = new CombatRunTimeSkillUpgradeOption(
            upgradeId: "combat_run_upgrade_burst_tempo",
            displayName: "Burst Tempo",
            description: "Burst Strike triggers faster during this run.",
            sourceSkillDisplayName: "Burst Strike",
            selectionHint: "Steadier burst pressure.");

        public static CombatRunTimeSkillUpgradeOption BurstPayload { get; } = new CombatRunTimeSkillUpgradeOption(
            upgradeId: "combat_run_upgrade_burst_payload",
            displayName: "Burst Payload",
            description: "Burst Strike hits harder during this run.",
            sourceSkillDisplayName: "Burst Strike",
            selectionHint: "Bigger damage spikes.");

        private static readonly IReadOnlyList<CombatRunTimeSkillUpgradeOption> BurstStrikeUpgradeOptions =
            new[]
            {
                BurstTempo,
                BurstPayload,
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
