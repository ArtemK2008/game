using System;
using System.Collections.Generic;
using Survivalon.Combat;

namespace Survivalon.Run
{
    public sealed class RunTimeSkillUpgradeChoiceStateResolver
    {
        public RunTimeSkillUpgradeChoiceState Resolve(
            IReadOnlyList<CombatRunTimeSkillUpgradeOption> upgradeOptions)
        {
            if (upgradeOptions == null)
            {
                throw new ArgumentNullException(nameof(upgradeOptions));
            }

            string sourceSkillDisplayName = ResolveSourceSkillDisplayName(upgradeOptions);
            List<RunTimeSkillUpgradeChoiceOptionState> optionStates =
                new List<RunTimeSkillUpgradeChoiceOptionState>(upgradeOptions.Count);

            for (int index = 0; index < upgradeOptions.Count; index++)
            {
                CombatRunTimeSkillUpgradeOption upgradeOption = upgradeOptions[index];
                optionStates.Add(new RunTimeSkillUpgradeChoiceOptionState(
                    upgradeOption.UpgradeId,
                    upgradeOption.DisplayName,
                    upgradeOption.Description,
                    upgradeOption.SelectionHint));
            }

            return new RunTimeSkillUpgradeChoiceState(
                "Run-only skill choice",
                BuildSummaryDisplayName(sourceSkillDisplayName),
                optionStates);
        }

        private static string ResolveSourceSkillDisplayName(
            IReadOnlyList<CombatRunTimeSkillUpgradeOption> upgradeOptions)
        {
            if (upgradeOptions.Count == 0)
            {
                return null;
            }

            string sourceSkillDisplayName = upgradeOptions[0].SourceSkillDisplayName;
            if (string.IsNullOrWhiteSpace(sourceSkillDisplayName))
            {
                return null;
            }

            for (int index = 1; index < upgradeOptions.Count; index++)
            {
                if (!string.Equals(
                    sourceSkillDisplayName,
                    upgradeOptions[index].SourceSkillDisplayName,
                    StringComparison.Ordinal))
                {
                    return null;
                }
            }

            return sourceSkillDisplayName;
        }

        private static string BuildSummaryDisplayName(string sourceSkillDisplayName)
        {
            return string.IsNullOrWhiteSpace(sourceSkillDisplayName)
                ? "Choose 1 upgrade before auto-battle starts. This choice affects the current run only."
                : $"Choose 1 {sourceSkillDisplayName} upgrade before auto-battle starts. This choice lasts for the current run only.";
        }
    }
}
