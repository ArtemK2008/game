using System;
using NUnit.Framework;
using Survivalon.Core;
using Survivalon.Data.Rewards;

namespace Survivalon.Tests.EditMode.Data.Rewards
{
    /// <summary>
    /// Проверяет shipped authored reward-тюнинг, вынесенный из runtime-логики.
    /// </summary>
    public sealed class RunRewardTuningCatalogTests
    {
        [Test]
        public void ShouldExposeCurrentShippedRunRewardTuning()
        {
            RunRewardTuningDefinition rewardTuning = RunRewardTuningCatalog.Current;

            Assert.That(rewardTuning.OrdinaryCombatCurrencyReward.ResourceCategory, Is.EqualTo(ResourceCategory.SoftCurrency));
            Assert.That(rewardTuning.OrdinaryCombatCurrencyReward.Amount, Is.EqualTo(1));
            Assert.That(rewardTuning.OrdinaryCombatRegionMaterialReward.ResourceCategory, Is.EqualTo(ResourceCategory.RegionMaterial));
            Assert.That(rewardTuning.OrdinaryCombatRegionMaterialReward.Amount, Is.EqualTo(1));
            Assert.That(
                rewardTuning.SuccessfulClearMilestoneMaterialReward.ResourceCategory,
                Is.EqualTo(ResourceCategory.PersistentProgressionMaterial));
            Assert.That(rewardTuning.SuccessfulClearMilestoneMaterialReward.Amount, Is.EqualTo(1));
            Assert.That(
                rewardTuning.SuccessfulBossMaterialReward.ResourceCategory,
                Is.EqualTo(ResourceCategory.PersistentProgressionMaterial));
            Assert.That(rewardTuning.SuccessfulBossMaterialReward.Amount, Is.EqualTo(2));
        }

        [Test]
        public void ShouldThrowWhenRewardAmountDefinitionUsesNegativeAmount()
        {
            Assert.That(
                () => new RewardAmountDefinition(ResourceCategory.SoftCurrency, -1),
                Throws.TypeOf<ArgumentOutOfRangeException>());
        }
    }
}
