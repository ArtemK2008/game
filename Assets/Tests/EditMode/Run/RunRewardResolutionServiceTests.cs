using NUnit.Framework;
using Survivalon.Core;
using Survivalon.Data.Combat;
using Survivalon.Data.Gear;
using Survivalon.Data.World;
using Survivalon.Run;
using Survivalon.State.Persistence;
using Survivalon.Tests.EditMode.World;
using Survivalon.World;

namespace Survivalon.Tests.EditMode.Run
{
    /// <summary>
    /// Проверяет, что runtime reward resolver сохраняет shipped поведение после выноса тюнинга в Data.
    /// </summary>
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
            Assert.That(rewardPayload.BossCurrencyRewards, Is.Empty);
            Assert.That(rewardPayload.BossMaterialRewards, Is.Empty);
        }

        [Test]
        public void ShouldGrantHigherRegionMaterialRewardForFrontierFarmNode()
        {
            RunRewardResolutionService service = new RunRewardResolutionService();

            RunRewardPayload rewardPayload = service.Resolve(
                NodePlaceholderTestData.CreateFrontierFarmPlaceholderState(),
                RunResolutionState.Succeeded,
                BootstrapWorldTestData.CreateWorldGraph());

            Assert.That(rewardPayload.CurrencyRewards, Has.Count.EqualTo(1));
            Assert.That(rewardPayload.CurrencyRewards[0].ResourceCategory, Is.EqualTo(ResourceCategory.SoftCurrency));
            Assert.That(rewardPayload.CurrencyRewards[0].Amount, Is.EqualTo(1));
            Assert.That(rewardPayload.MaterialRewards, Has.Count.EqualTo(1));
            Assert.That(rewardPayload.MaterialRewards[0].ResourceCategory, Is.EqualTo(ResourceCategory.RegionMaterial));
            Assert.That(rewardPayload.MaterialRewards[0].Amount, Is.EqualTo(2));
            Assert.That(rewardPayload.MilestoneMaterialRewards, Is.Empty);
            Assert.That(rewardPayload.BossMaterialRewards, Is.Empty);
        }

        [Test]
        public void ShouldGrantBoundedEliteChallengeRewardBundleForOptionalChallengeCombatRun()
        {
            RunRewardResolutionService service = new RunRewardResolutionService();

            RunRewardPayload rewardPayload = service.Resolve(
                NodePlaceholderTestData.CreateEliteChallengePlaceholderState(),
                RunResolutionState.Succeeded,
                BootstrapWorldTestData.CreateWorldGraph());

            Assert.That(rewardPayload.CurrencyRewards, Has.Count.EqualTo(1));
            Assert.That(rewardPayload.CurrencyRewards[0].ResourceCategory, Is.EqualTo(ResourceCategory.SoftCurrency));
            Assert.That(rewardPayload.CurrencyRewards[0].Amount, Is.EqualTo(1));
            Assert.That(rewardPayload.MaterialRewards, Has.Count.EqualTo(1));
            Assert.That(rewardPayload.MaterialRewards[0].ResourceCategory, Is.EqualTo(ResourceCategory.RegionMaterial));
            Assert.That(rewardPayload.MaterialRewards[0].Amount, Is.EqualTo(3));
            Assert.That(rewardPayload.MilestoneMaterialRewards, Is.Empty);
            Assert.That(rewardPayload.BossMaterialRewards, Is.Empty);
            Assert.That(rewardPayload.BossGearRewards, Is.Empty);
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
        public void ShouldGrantBossRewardBundleForSuccessfulBossCombatRun()
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
            Assert.That(rewardPayload.BossCurrencyRewards, Is.Empty);
            Assert.That(rewardPayload.BossMaterialRewards, Has.Count.EqualTo(1));
            Assert.That(rewardPayload.BossMaterialRewards[0].ResourceCategory, Is.EqualTo(ResourceCategory.PersistentProgressionMaterial));
            Assert.That(rewardPayload.BossMaterialRewards[0].Amount, Is.EqualTo(2));
            Assert.That(rewardPayload.BossGearRewards, Has.Count.EqualTo(1));
            Assert.That(rewardPayload.BossGearRewards[0].GearId, Is.EqualTo(GearIds.GatebreakerBlade));
            Assert.That(rewardPayload.BossGearRewards[0].DisplayName, Is.EqualTo("Gatebreaker Blade"));
            Assert.That(rewardPayload.MilestoneMaterialRewards, Is.Empty);
        }

        [Test]
        public void ShouldGrantHigherBossProgressionMaterialRewardForEchoCavernsBossRun()
        {
            RunRewardResolutionService service = new RunRewardResolutionService();

            RunRewardPayload rewardPayload = service.Resolve(
                NodePlaceholderTestData.CreateCavernGateBossPlaceholderState(),
                RunResolutionState.Succeeded,
                BootstrapWorldTestData.CreateWorldGraph());

            Assert.That(rewardPayload.CurrencyRewards, Has.Count.EqualTo(1));
            Assert.That(rewardPayload.CurrencyRewards[0].ResourceCategory, Is.EqualTo(ResourceCategory.SoftCurrency));
            Assert.That(rewardPayload.CurrencyRewards[0].Amount, Is.EqualTo(1));
            Assert.That(rewardPayload.MaterialRewards, Is.Empty);
            Assert.That(rewardPayload.BossCurrencyRewards, Is.Empty);
            Assert.That(rewardPayload.BossMaterialRewards, Has.Count.EqualTo(1));
            Assert.That(
                rewardPayload.BossMaterialRewards[0].ResourceCategory,
                Is.EqualTo(ResourceCategory.PersistentProgressionMaterial));
            Assert.That(rewardPayload.BossMaterialRewards[0].Amount, Is.EqualTo(3));
            Assert.That(rewardPayload.MilestoneMaterialRewards, Is.Empty);
        }

        [Test]
        public void ShouldNotGrantCavernBossBonusWhenEchoCavernsBossNodeLacksSpecificBossRewardContent()
        {
            RunRewardResolutionService service = new RunRewardResolutionService();
            NodePlaceholderState placeholderState = new NodePlaceholderState(
                BootstrapWorldScenario.CavernGateNodeId,
                BootstrapWorldScenario.CavernRegionId,
                NodeType.BossOrGate,
                NodeState.Available,
                BootstrapWorldScenario.CavernServiceNodeId,
                CombatBossEncounterCatalog.GateBossEncounter,
                locationIdentity: LocationIdentityCatalog.EchoCaverns);

            RunRewardPayload rewardPayload = service.Resolve(
                placeholderState,
                RunResolutionState.Succeeded,
                BootstrapWorldTestData.CreateWorldGraph());

            Assert.That(rewardPayload.BossMaterialRewards, Has.Count.EqualTo(1));
            Assert.That(rewardPayload.BossMaterialRewards[0].ResourceCategory, Is.EqualTo(ResourceCategory.PersistentProgressionMaterial));
            Assert.That(rewardPayload.BossMaterialRewards[0].Amount, Is.EqualTo(2));
            Assert.That(rewardPayload.BossGearRewards, Is.Empty);
        }

        [Test]
        public void ShouldKeepBossRewardValueWhenDuplicateBossGearRewardIsAlreadyOwned()
        {
            RunRewardResolutionService service = new RunRewardResolutionService();

            RunRewardPayload rewardPayload = service.Resolve(
                NodePlaceholderTestData.CreateBossCombatPlaceholderState(),
                RunResolutionState.Succeeded,
                BootstrapWorldTestData.CreateWorldGraph(),
                ownedGearIds: new[] { GearIds.TrainingBlade, GearIds.GuardCharm, GearIds.GatebreakerBlade });

            Assert.That(rewardPayload.BossMaterialRewards, Has.Count.EqualTo(1));
            Assert.That(rewardPayload.BossMaterialRewards[0].Amount, Is.EqualTo(2));
            Assert.That(rewardPayload.BossGearRewards, Is.Empty);
            Assert.That(rewardPayload.HasBossRewards, Is.True);
            Assert.That(rewardPayload.HasRewards, Is.True);
        }

        [Test]
        public void ShouldIncreaseBossRewardBundleWhenBossSalvageProjectIsPurchased()
        {
            RunRewardResolutionService service = new RunRewardResolutionService();

            RunRewardPayload rewardPayload = service.Resolve(
                NodePlaceholderTestData.CreateBossCombatPlaceholderState(),
                RunResolutionState.Succeeded,
                BootstrapWorldTestData.CreateWorldGraph(),
                progressionEffects: new AccountWideProgressionEffectState(
                    playerMaxHealthBonus: 0,
                    playerAttackPowerBonus: 0,
                    ordinaryRegionMaterialRewardBonus: 0,
                    bossProgressionMaterialRewardBonus: 1));

            Assert.That(rewardPayload.CurrencyRewards, Has.Count.EqualTo(1));
            Assert.That(rewardPayload.CurrencyRewards[0].Amount, Is.EqualTo(1));
            Assert.That(rewardPayload.BossCurrencyRewards, Is.Empty);
            Assert.That(rewardPayload.BossMaterialRewards, Has.Count.EqualTo(1));
            Assert.That(rewardPayload.BossMaterialRewards[0].ResourceCategory, Is.EqualTo(ResourceCategory.PersistentProgressionMaterial));
            Assert.That(rewardPayload.BossMaterialRewards[0].Amount, Is.EqualTo(3));
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
            Assert.That(rewardPayload.BossMaterialRewards, Is.Empty);
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
        public void ShouldKeepOrdinaryCombatRewardsUnchangedWhenOnlyBossSalvageProjectIsPurchased()
        {
            RunRewardResolutionService service = new RunRewardResolutionService();

            RunRewardPayload rewardPayload = service.Resolve(
                NodePlaceholderTestData.CreateCombatPlaceholderState(),
                RunResolutionState.Succeeded,
                BootstrapWorldTestData.CreateWorldGraph(),
                progressionEffects: new AccountWideProgressionEffectState(
                    playerMaxHealthBonus: 0,
                    playerAttackPowerBonus: 0,
                    ordinaryRegionMaterialRewardBonus: 0,
                    bossProgressionMaterialRewardBonus: 1));

            Assert.That(rewardPayload.CurrencyRewards, Has.Count.EqualTo(1));
            Assert.That(rewardPayload.CurrencyRewards[0].Amount, Is.EqualTo(1));
            Assert.That(rewardPayload.MaterialRewards, Has.Count.EqualTo(1));
            Assert.That(rewardPayload.MaterialRewards[0].ResourceCategory, Is.EqualTo(ResourceCategory.RegionMaterial));
            Assert.That(rewardPayload.MaterialRewards[0].Amount, Is.EqualTo(1));
            Assert.That(rewardPayload.MilestoneMaterialRewards, Is.Empty);
            Assert.That(rewardPayload.BossMaterialRewards, Is.Empty);
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
            Assert.That(rewardPayload.BossMaterialRewards, Is.Empty);
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
            Assert.That(rewardPayload.BossMaterialRewards, Is.Empty);
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

