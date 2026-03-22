using System;
using System.Collections.Generic;

namespace Survivalon.Run
{
    public sealed class RunTimeSkillUpgradeChoiceState
    {
        public RunTimeSkillUpgradeChoiceState(
            string titleDisplayName,
            string summaryDisplayName,
            IReadOnlyList<RunTimeSkillUpgradeChoiceOptionState> options)
        {
            if (string.IsNullOrWhiteSpace(titleDisplayName))
            {
                throw new ArgumentException(
                    "Choice title cannot be null or whitespace.",
                    nameof(titleDisplayName));
            }

            if (string.IsNullOrWhiteSpace(summaryDisplayName))
            {
                throw new ArgumentException(
                    "Choice summary cannot be null or whitespace.",
                    nameof(summaryDisplayName));
            }

            if (options == null)
            {
                throw new ArgumentNullException(nameof(options));
            }

            TitleDisplayName = titleDisplayName;
            SummaryDisplayName = summaryDisplayName;
            Options = new List<RunTimeSkillUpgradeChoiceOptionState>(options).AsReadOnly();
        }

        public string TitleDisplayName { get; }

        public string SummaryDisplayName { get; }

        public IReadOnlyList<RunTimeSkillUpgradeChoiceOptionState> Options { get; }
    }

    public sealed class RunTimeSkillUpgradeChoiceOptionState
    {
        public RunTimeSkillUpgradeChoiceOptionState(
            string upgradeId,
            string displayName,
            string effectSummary,
            string pickHint)
        {
            if (string.IsNullOrWhiteSpace(upgradeId))
            {
                throw new ArgumentException(
                    "Upgrade id cannot be null or whitespace.",
                    nameof(upgradeId));
            }

            if (string.IsNullOrWhiteSpace(displayName))
            {
                throw new ArgumentException(
                    "Display name cannot be null or whitespace.",
                    nameof(displayName));
            }

            if (string.IsNullOrWhiteSpace(effectSummary))
            {
                throw new ArgumentException(
                    "Effect summary cannot be null or whitespace.",
                    nameof(effectSummary));
            }

            if (string.IsNullOrWhiteSpace(pickHint))
            {
                throw new ArgumentException(
                    "Pick hint cannot be null or whitespace.",
                    nameof(pickHint));
            }

            UpgradeId = upgradeId;
            DisplayName = displayName;
            EffectSummary = effectSummary;
            PickHint = pickHint;
        }

        public string UpgradeId { get; }

        public string DisplayName { get; }

        public string EffectSummary { get; }

        public string PickHint { get; }
    }
}
