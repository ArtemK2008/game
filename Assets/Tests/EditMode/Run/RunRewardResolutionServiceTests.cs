using NUnit.Framework;
using Survivalon.Runtime;
using Survivalon.Runtime.Combat;
using Survivalon.Runtime.Core;
using Survivalon.Runtime.Run;
using Survivalon.Runtime.State;
using Survivalon.Runtime.State.Persistence;
using Survivalon.Runtime.World;
using Survivalon.Tests.EditMode.World;

namespace Survivalon.Tests.EditMode.Run
{
    public sealed class RunRewardResolutionServiceTests
    {
        [Test]
        public void ShouldGrantSoftCurrencyAndRegionMaterialForSuccessfulRegionCombatRun()
        {
            RunRewardResolutionService service = new RunRewardResolutionService();

            RunRewardPayload rewardPayload = service.Resolve(
                NodePlaceholderTestData.CreateCombatPlaceholderState(),
                RunResolutionState.Succeeded,
                BootstrapWorldTestData.CreateWorldGraph());

            Assert.That(rewardPayload.CurrencyRewards, Has.Count.EqualTo(1));
            Assert.That(rewardPayload.CurrencyRewards[0].ResourceCategory, Is.EqualTo(ResourceCategory.SoftCurrency));
            Assert.That(rewardPayload.CurrencyRewards[0].Amount, Is.EqualTo(1));
            Assert.That(rewardPayload.MaterialRewards, Has.Count.EqualTo(1));
            Assert.That(rewardPayload.MaterialRewards[0].ResourceCategory, Is.EqualTo(ResourceCategory.RegionMaterial));
            Assert.That(rewardPayload.MaterialRewards[0].Amount, Is.EqualTo(1));
            Assert.That(rewardPayload.MilestoneCurrencyRewards, Is.Empty);
            Assert.That(rewardPayload.MilestoneMaterialRewards, Is.Empty);
        }

        [Test]
        public void ShouldReturnEmptyPayloadForFailedCombatRun()
        {
            RunRewardResolutionService service = new RunRewardResolutionService();

            RunRewardPayload rewardPayload = service.Resolve(
                NodePlaceholderTestData.CreateBossCombatPlaceholderState(),
                RunResolutionState.Failed);

            Assert.That(rewardPayload, Is.SameAs(RunRewardPayload.Empty));
        }

        [Test]
        public void ShouldGrantOnlySoftCurrencyForSuccessfulBossCombatRun()
        {
            RunRewardResolutionService service = new RunRewardResolutionService();

            RunRewardPayload rewardPayload = service.Resolve(
                NodePlaceholderTestData.CreateBossCombatPlaceholderState(),
                RunResolutionState.Succeeded,
                BootstrapWorldTestData.CreateWorldGraph());

            Assert.That(rewardPayload.CurrencyRewards, Has.Count.EqualTo(1));
            Assert.That(rewardPayload.CurrencyRewards[0].ResourceCategory, Is.EqualTo(ResourceCategory.SoftCurrency));
            Assert.That(rewardPayload.CurrencyRewards[0].Amount, Is.EqualTo(1));
            Assert.That(rewardPayload.MaterialRewards, Is.Empty);
            Assert.That(rewardPayload.MilestoneMaterialRewards, Is.Empty);
        }

        [Test]
        public void ShouldIncreaseOrdinaryRegionMaterialRewardWhenFarmYieldProjectIsPurchased()
        {
            RunRewardResolutionService service = new RunRewardResolutionService();

            RunRewardPayload rewardPayload = service.Resolve(
                NodePlaceholderTestData.CreateCombatPlaceholderState(),
                RunResolutionState.Succeeded,
                BootstrapWorldTestData.CreateWorldGraph(),
                progressionEffects: new AccountWideProgressionEffectState(
                    playerMaxHealthBonus: 0,
                    playerAttackPowerBonus: 0,
                    ordinaryRegionMaterialRewardBonus: 1));

            Assert.That(rewardPayload.CurrencyRewards, Has.Count.EqualTo(1));
            Assert.That(rewardPayload.CurrencyRewards[0].Amount, Is.EqualTo(1));
            Assert.That(rewardPayload.MaterialRewards, Has.Count.EqualTo(1));
            Assert.That(rewardPayload.MaterialRewards[0].ResourceCategory, Is.EqualTo(ResourceCategory.RegionMaterial));
            Assert.That(rewardPayload.MaterialRewards[0].Amount, Is.EqualTo(2));
            Assert.That(rewardPayload.MilestoneMaterialRewards, Is.Empty);
        }

        [Test]
        public void ShouldReturnEmptyPayloadForSuccessfulNonCombatRun()
        {
            RunRewardResolutionService service = new RunRewardResolutionService();

            RunRewardPayload rewardPayload = service.Resolve(
                NodePlaceholderTestData.CreateServicePlaceholderState(),
                RunResolutionState.Succeeded);

            Assert.That(rewardPayload, Is.SameAs(RunRewardPayload.Empty));
        }

        [Test]
        public void ShouldGrantPersistentProgressionMaterialWhenTrackedNodeClears()
        {
            RunRewardResolutionService service = new RunRewardResolutionService();

            RunRewardPayload rewardPayload = service.Resolve(
                NodePlaceholderTestData.CreatePushCombatPlaceholderState(),
                RunResolutionState.Succeeded,
                BootstrapWorldTestData.CreateWorldGraph(),
                CreateClearThresholdProgressResolution(didUnlockRoute: true));

            Assert.That(rewardPayload.CurrencyRewards, Has.Count.EqualTo(1));
            Assert.That(rewardPayload.CurrencyRewards[0].ResourceCategory, Is.EqualTo(ResourceCategory.SoftCurrency));
            Assert.That(rewardPayload.MaterialRewards, Has.Count.EqualTo(1));
            Assert.That(rewardPayload.MaterialRewards[0].ResourceCategory, Is.EqualTo(ResourceCategory.RegionMaterial));
            Assert.That(rewardPayload.MilestoneCurrencyRewards, Is.Empty);
            Assert.That(rewardPayload.MilestoneMaterialRewards, Has.Count.EqualTo(1));
            Assert.That(rewardPayload.MilestoneMaterialRewards[0].ResourceCategory, Is.EqualTo(ResourceCategory.PersistentProgressionMaterial));
            Assert.That(rewardPayload.MilestoneMaterialRewards[0].Amount, Is.EqualTo(1));
        }

        [Test]
        public void ShouldKeepMilestoneRewardDistinctWhenFarmYieldProjectIsPurchased()
        {
            RunRewardResolutionService service = new RunRewardResolutionService();

            RunRewardPayload rewardPayload = service.Resolve(
                NodePlaceholderTestData.CreatePushCombatPlaceholderState(),
                RunResolutionState.Succeeded,
                BootstrapWorldTestData.CreateWorldGraph(),
                CreateClearThresholdProgressResolution(didUnlockRoute: true),
                new AccountWideProgressionEffectState(
                    playerMaxHealthBonus: 0,
                    playerAttackPowerBonus: 0,
                    ordinaryRegionMaterialRewardBonus: 1));

            Assert.That(rewardPayload.CurrencyRewards, Has.Count.EqualTo(1));
            Assert.That(rewardPayload.CurrencyRewards[0].Amount, Is.EqualTo(1));
            Assert.That(rewardPayload.MaterialRewards, Has.Count.EqualTo(1));
            Assert.That(rewardPayload.MaterialRewards[0].ResourceCategory, Is.EqualTo(ResourceCategory.RegionMaterial));
            Assert.That(rewardPayload.MaterialRewards[0].Amount, Is.EqualTo(2));
            Assert.That(rewardPayload.MilestoneCurrencyRewards, Is.Empty);
            Assert.That(rewardPayload.MilestoneMaterialRewards, Has.Count.EqualTo(1));
            Assert.That(
                rewardPayload.MilestoneMaterialRewards[0].ResourceCategory,
                Is.EqualTo(ResourceCategory.PersistentProgressionMaterial));
            Assert.That(rewardPayload.MilestoneMaterialRewards[0].Amount, Is.EqualTo(1));
        }

        private static RunProgressResolution CreateClearThresholdProgressResolution(bool didUnlockRoute)
        {
            return new RunProgressResolution(
                1,
                new NodeProgressUpdateResult(
                    isTracked: true,
                    currentProgress: 3,
                    progressThreshold: 3,
                    didReachClearThreshold: true,
                    nodeStateAfterUpdate: NodeState.Cleared),
                didUnlockRoute);
        }
    }
}
