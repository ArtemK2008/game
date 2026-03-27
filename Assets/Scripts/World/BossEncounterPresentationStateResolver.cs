using System;
using System.Collections.Generic;
using Survivalon.Core;
using Survivalon.Data.Combat;

namespace Survivalon.World
{
    /// <summary>
    /// Собирает компактную читаемую boss-сводку из уже существующего node/boss content без новой gameplay-логики.
    /// </summary>
    public static class BossEncounterPresentationStateResolver
    {
        public static BossEncounterPresentationState Resolve(NodePlaceholderState placeholderState)
        {
            if (placeholderState == null)
            {
                throw new ArgumentNullException(nameof(placeholderState));
            }

            CombatBossEncounterDefinition bossEncounterDefinition =
                placeholderState.CombatEncounter as CombatBossEncounterDefinition;
            if (placeholderState.NodeType != NodeType.BossOrGate || bossEncounterDefinition == null)
            {
                return BossEncounterPresentationState.None;
            }

            string encounterDisplayName = ResolveEncounterDisplayName(bossEncounterDefinition.BossRoleType);
            string stakesSummary = BuildStakesSummary(placeholderState);

            return new BossEncounterPresentationState(true, encounterDisplayName, stakesSummary);
        }

        private static string ResolveEncounterDisplayName(CombatBossRoleType bossRoleType)
        {
            switch (bossRoleType)
            {
                case CombatBossRoleType.GateBoss:
                    return "Gate boss";
                default:
                    throw new InvalidOperationException(
                        $"Unknown boss role type '{bossRoleType}'.");
            }
        }

        private static string BuildStakesSummary(NodePlaceholderState placeholderState)
        {
            List<string> stakes = new List<string>();

            if (placeholderState.BossProgressionGate != null)
            {
                stakes.Add("Gate clear");
            }

            if (placeholderState.BossRewardContent != null)
            {
                stakes.Add("Boss rewards");
            }

            if (!string.IsNullOrWhiteSpace(placeholderState.BossRewardContent?.GearRewardId))
            {
                stakes.Add("Gear reward");
            }

            return stakes.Count == 0
                ? string.Empty
                : string.Join(", ", stakes);
        }
    }
}
