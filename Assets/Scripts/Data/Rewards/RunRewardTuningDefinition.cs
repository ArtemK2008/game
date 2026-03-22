using System;

namespace Survivalon.Data.Rewards
{
    /// <summary>
    /// Описывает статический shipped тюнинг базовых наград текущего прототипа.
    /// </summary>
    public sealed class RunRewardTuningDefinition
    {
        public RunRewardTuningDefinition(
            RewardAmountDefinition ordinaryCombatCurrencyReward,
            RewardAmountDefinition ordinaryCombatRegionMaterialReward,
            RewardAmountDefinition successfulClearMilestoneMaterialReward,
            RewardAmountDefinition successfulBossMaterialReward)
        {
            OrdinaryCombatCurrencyReward = ordinaryCombatCurrencyReward ??
                throw new ArgumentNullException(nameof(ordinaryCombatCurrencyReward));
            OrdinaryCombatRegionMaterialReward = ordinaryCombatRegionMaterialReward ??
                throw new ArgumentNullException(nameof(ordinaryCombatRegionMaterialReward));
            SuccessfulClearMilestoneMaterialReward = successfulClearMilestoneMaterialReward ??
                throw new ArgumentNullException(nameof(successfulClearMilestoneMaterialReward));
            SuccessfulBossMaterialReward = successfulBossMaterialReward ??
                throw new ArgumentNullException(nameof(successfulBossMaterialReward));
        }

        public RewardAmountDefinition OrdinaryCombatCurrencyReward { get; }

        public RewardAmountDefinition OrdinaryCombatRegionMaterialReward { get; }

        public RewardAmountDefinition SuccessfulClearMilestoneMaterialReward { get; }

        public RewardAmountDefinition SuccessfulBossMaterialReward { get; }
    }
}
