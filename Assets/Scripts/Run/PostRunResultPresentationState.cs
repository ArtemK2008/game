using System;

namespace Survivalon.Run
{
    /// <summary>
    /// Хранит уже сгруппированное post-run представление наград, спайков и исходов без UI-логики.
    /// </summary>
    public sealed class PostRunResultPresentationState
    {
        public PostRunResultPresentationState(
            string ordinaryRewardSummary,
            string progressSummary,
            string rewardSourceSummary = "",
            string clearSpikeRewardSummary = "",
            string bossSpikeRewardSummary = "",
            string bossGearRewardSummary = "",
            string unlockOutcomeSummary = "")
        {
            OrdinaryRewardSummary = ordinaryRewardSummary ?? throw new ArgumentNullException(nameof(ordinaryRewardSummary));
            ProgressSummary = progressSummary ?? throw new ArgumentNullException(nameof(progressSummary));
            RewardSourceSummary = rewardSourceSummary ?? string.Empty;
            ClearSpikeRewardSummary = clearSpikeRewardSummary ?? string.Empty;
            BossSpikeRewardSummary = bossSpikeRewardSummary ?? string.Empty;
            BossGearRewardSummary = bossGearRewardSummary ?? string.Empty;
            UnlockOutcomeSummary = unlockOutcomeSummary ?? string.Empty;
        }

        public string OrdinaryRewardSummary { get; }

        public string ProgressSummary { get; }

        public string RewardSourceSummary { get; }

        public string ClearSpikeRewardSummary { get; }

        public string BossSpikeRewardSummary { get; }

        public string BossGearRewardSummary { get; }

        public string UnlockOutcomeSummary { get; }

        public bool HasRewardSourceSummary => !string.IsNullOrWhiteSpace(RewardSourceSummary);

        public bool HasClearSpikeRewardSummary => !string.IsNullOrWhiteSpace(ClearSpikeRewardSummary);

        public bool HasBossSpikeRewardSummary => !string.IsNullOrWhiteSpace(BossSpikeRewardSummary);

        public bool HasBossGearRewardSummary => !string.IsNullOrWhiteSpace(BossGearRewardSummary);

        public bool HasUnlockOutcomeSummary => !string.IsNullOrWhiteSpace(UnlockOutcomeSummary);
    }
}
