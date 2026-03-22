using System;
using System.Collections.Generic;
using Survivalon.Combat;

namespace Survivalon.Run
{
    public sealed class RunTimeSkillUpgradeChoicePresentationCatalog
    {
        private static readonly IReadOnlyDictionary<string, RunTimeSkillUpgradeChoicePresentationDefinition> Definitions =
            new Dictionary<string, RunTimeSkillUpgradeChoicePresentationDefinition>(StringComparer.Ordinal)
            {
                {
                    CombatRunTimeSkillUpgradeCatalog.BurstTempo.UpgradeId,
                    new RunTimeSkillUpgradeChoicePresentationDefinition(
                        "Burst Strike",
                        "Steadier burst pressure.")
                },
                {
                    CombatRunTimeSkillUpgradeCatalog.BurstPayload.UpgradeId,
                    new RunTimeSkillUpgradeChoicePresentationDefinition(
                        "Burst Strike",
                        "Bigger damage spikes.")
                },
            };

        public RunTimeSkillUpgradeChoicePresentationDefinition Resolve(string upgradeId)
        {
            if (string.IsNullOrWhiteSpace(upgradeId))
            {
                throw new ArgumentException("Upgrade id cannot be null or whitespace.", nameof(upgradeId));
            }

            return Definitions.TryGetValue(upgradeId, out RunTimeSkillUpgradeChoicePresentationDefinition definition)
                ? definition
                : new RunTimeSkillUpgradeChoicePresentationDefinition(
                    sourceSkillDisplayName: null,
                    selectionHint: "Useful in this run.");
        }
    }

    public readonly struct RunTimeSkillUpgradeChoicePresentationDefinition
    {
        public RunTimeSkillUpgradeChoicePresentationDefinition(
            string sourceSkillDisplayName,
            string selectionHint)
        {
            if (selectionHint == null)
            {
                throw new ArgumentNullException(nameof(selectionHint));
            }

            SourceSkillDisplayName = sourceSkillDisplayName;
            SelectionHint = selectionHint;
        }

        public string SourceSkillDisplayName { get; }

        public string SelectionHint { get; }
    }
}
