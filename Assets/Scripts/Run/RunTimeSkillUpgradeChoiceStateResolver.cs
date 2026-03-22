using System;
using System.Collections.Generic;
using Survivalon.Combat;

namespace Survivalon.Run
{
    public sealed class RunTimeSkillUpgradeChoiceStateResolver
    {
        private readonly RunTimeSkillUpgradeChoicePresentationCatalog presentationCatalog;

        public RunTimeSkillUpgradeChoiceStateResolver(
            RunTimeSkillUpgradeChoicePresentationCatalog presentationCatalog = null)
        {
            this.presentationCatalog = presentationCatalog ?? new RunTimeSkillUpgradeChoicePresentationCatalog();
        }

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
                RunTimeSkillUpgradeChoicePresentationDefinition presentation =
                    presentationCatalog.Resolve(upgradeOption.UpgradeId);
                optionStates.Add(new RunTimeSkillUpgradeChoiceOptionState(
                    upgradeOption.UpgradeId,
                    upgradeOption.DisplayName,
                    upgradeOption.Description,
                    presentation.SelectionHint));
            }

            return new RunTimeSkillUpgradeChoiceState(
                "Run-only skill choice",
                BuildSummaryDisplayName(sourceSkillDisplayName),
                optionStates);
        }

        private string ResolveSourceSkillDisplayName(
            IReadOnlyList<CombatRunTimeSkillUpgradeOption> upgradeOptions)
        {
            if (upgradeOptions.Count == 0)
            {
                return null;
            }

            string sourceSkillDisplayName = ResolveSourceSkillDisplayName(
                presentationCatalog.Resolve(upgradeOptions[0].UpgradeId));
            if (string.IsNullOrWhiteSpace(sourceSkillDisplayName))
            {
                return null;
            }

            for (int index = 1; index < upgradeOptions.Count; index++)
            {
                RunTimeSkillUpgradeChoicePresentationDefinition presentation =
                    presentationCatalog.Resolve(upgradeOptions[index].UpgradeId);
                if (!string.Equals(
                    sourceSkillDisplayName,
                    ResolveSourceSkillDisplayName(presentation),
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

        private static string ResolveSourceSkillDisplayName(
            RunTimeSkillUpgradeChoicePresentationDefinition presentation)
        {
            return string.IsNullOrWhiteSpace(presentation.SourceSkillDisplayName)
                ? null
                : presentation.SourceSkillDisplayName;
        }
    }
}
