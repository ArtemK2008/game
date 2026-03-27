using System;

namespace Survivalon.World
{
    /// <summary>
    /// Описывает компактную player-facing сводку boss-encounter для текущего placeholder-потока.
    /// </summary>
    public sealed class BossEncounterPresentationState
    {
        public static readonly BossEncounterPresentationState None =
            new BossEncounterPresentationState(false, string.Empty, string.Empty);

        public BossEncounterPresentationState(
            bool isBossEncounter,
            string encounterDisplayName,
            string stakesSummary)
        {
            if (isBossEncounter && string.IsNullOrWhiteSpace(encounterDisplayName))
            {
                throw new ArgumentException(
                    "Boss encounter display name cannot be null or whitespace when boss presentation is active.",
                    nameof(encounterDisplayName));
            }

            if (!isBossEncounter && !string.IsNullOrEmpty(encounterDisplayName))
            {
                throw new ArgumentException(
                    "Non-boss presentation cannot expose an encounter display name.",
                    nameof(encounterDisplayName));
            }

            if (!isBossEncounter && !string.IsNullOrEmpty(stakesSummary))
            {
                throw new ArgumentException(
                    "Non-boss presentation cannot expose a stakes summary.",
                    nameof(stakesSummary));
            }

            IsBossEncounter = isBossEncounter;
            EncounterDisplayName = encounterDisplayName ?? string.Empty;
            StakesSummary = stakesSummary ?? string.Empty;
        }

        public bool IsBossEncounter { get; }

        public string EncounterDisplayName { get; }

        public string StakesSummary { get; }

        public bool HasStakesSummary => !string.IsNullOrWhiteSpace(StakesSummary);
    }
}
