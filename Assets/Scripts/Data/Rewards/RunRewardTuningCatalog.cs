using Survivalon.Core;

namespace Survivalon.Data.Rewards
{
    /// <summary>
    /// Хранит текущий authored catalog базовых reward-значений для shipped runtime loop.
    /// </summary>
    public static class RunRewardTuningCatalog
    {
        private static readonly RunRewardTuningDefinition CurrentDefinition =
            new RunRewardTuningDefinition(
                new RewardAmountDefinition(ResourceCategory.SoftCurrency, 1),
                new RewardAmountDefinition(ResourceCategory.RegionMaterial, 1),
                new RewardAmountDefinition(ResourceCategory.PersistentProgressionMaterial, 1),
                new RewardAmountDefinition(ResourceCategory.PersistentProgressionMaterial, 2));

        public static RunRewardTuningDefinition Current => CurrentDefinition;
    }
}
