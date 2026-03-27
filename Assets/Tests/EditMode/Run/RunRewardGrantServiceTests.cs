using NUnit.Framework;
using Survivalon.Core;
using Survivalon.Data.Gear;
using Survivalon.Run;
using Survivalon.State.Persistence;

namespace Survivalon.Tests.EditMode.Run
{
    public sealed class RunRewardGrantServiceTests
    {
        [Test]
        public void ShouldApplySoftCurrencyRewardToPersistentBalances()
        {
            RunRewardGrantService service = new RunRewardGrantService();
            ResourceBalancesState resourceBalances = new ResourceBalancesState();
            RunRewardPayload rewardPayload = new RunRewardPayload(
                new[]
                {
                    new RunCurrencyReward(ResourceCategory.SoftCurrency, 1),
                },
                System.Array.Empty<RunMaterialReward>());

            service.Grant(resourceBalances, rewardPayload);

            Assert.That(resourceBalances.GetAmount(ResourceCategory.SoftCurrency), Is.EqualTo(1));
        }

        [Test]
        public void ShouldApplyMaterialRewardToPersistentBalances()
        {
            RunRewardGrantService service = new RunRewardGrantService();
            ResourceBalancesState resourceBalances = new ResourceBalancesState();
            RunRewardPayload rewardPayload = new RunRewardPayload(
                System.Array.Empty<RunCurrencyReward>(),
                new[]
                {
                    new RunMaterialReward(ResourceCategory.RegionMaterial, 2),
                });

            service.Grant(resourceBalances, rewardPayload);

            Assert.That(resourceBalances.GetAmount(ResourceCategory.RegionMaterial), Is.EqualTo(2));
        }

        [Test]
        public void ShouldApplyMilestoneMaterialRewardToPersistentBalances()
        {
            RunRewardGrantService service = new RunRewardGrantService();
            ResourceBalancesState resourceBalances = new ResourceBalancesState();
            RunRewardPayload rewardPayload = new RunRewardPayload(
                System.Array.Empty<RunCurrencyReward>(),
                System.Array.Empty<RunMaterialReward>(),
                System.Array.Empty<RunCurrencyReward>(),
                new[]
                {
                    new RunMaterialReward(ResourceCategory.PersistentProgressionMaterial, 1),
                });

            service.Grant(resourceBalances, rewardPayload);

            Assert.That(resourceBalances.GetAmount(ResourceCategory.PersistentProgressionMaterial), Is.EqualTo(1));
        }

        [Test]
        public void ShouldApplyBossMaterialRewardToPersistentBalances()
        {
            RunRewardGrantService service = new RunRewardGrantService();
            ResourceBalancesState resourceBalances = new ResourceBalancesState();
            RunRewardPayload rewardPayload = new RunRewardPayload(
                System.Array.Empty<RunCurrencyReward>(),
                System.Array.Empty<RunMaterialReward>(),
                System.Array.Empty<RunCurrencyReward>(),
                System.Array.Empty<RunMaterialReward>(),
                System.Array.Empty<RunCurrencyReward>(),
                new[]
                {
                    new RunMaterialReward(ResourceCategory.PersistentProgressionMaterial, 2),
                });

            service.Grant(resourceBalances, rewardPayload);

            Assert.That(resourceBalances.GetAmount(ResourceCategory.PersistentProgressionMaterial), Is.EqualTo(2));
        }

        [Test]
        public void ShouldLeaveBalancesUnchangedWhenRewardPayloadIsEmpty()
        {
            RunRewardGrantService service = new RunRewardGrantService();
            ResourceBalancesState resourceBalances = new ResourceBalancesState();

            service.Grant(resourceBalances, RunRewardPayload.Empty);

            Assert.That(resourceBalances.GetAmount(ResourceCategory.SoftCurrency), Is.EqualTo(0));
            Assert.That(resourceBalances.GetAmount(ResourceCategory.PersistentProgressionMaterial), Is.EqualTo(0));
        }

        [Test]
        public void ShouldGrantBossGearRewardIntoOwnedGearWithoutDuplicatingEntries()
        {
            RunRewardGrantService service = new RunRewardGrantService();
            PersistentGameState gameState = new PersistentGameState();
            RunRewardPayload rewardPayload = new RunRewardPayload(
                System.Array.Empty<RunCurrencyReward>(),
                System.Array.Empty<RunMaterialReward>(),
                System.Array.Empty<RunCurrencyReward>(),
                System.Array.Empty<RunMaterialReward>(),
                System.Array.Empty<RunCurrencyReward>(),
                new[]
                {
                    new RunMaterialReward(ResourceCategory.PersistentProgressionMaterial, 2),
                },
                new[]
                {
                    new RunGearReward(GearIds.GatebreakerBlade),
                });

            gameState.EnsureOwnedGearId(GearIds.TrainingBlade);
            gameState.EnsureOwnedGearId(GearIds.GuardCharm);
            service.Grant(gameState, rewardPayload);
            service.Grant(gameState, rewardPayload);

            Assert.That(gameState.ResourceBalances.GetAmount(ResourceCategory.PersistentProgressionMaterial), Is.EqualTo(4));
            Assert.That(gameState.OwnedGearIds, Does.Contain(GearIds.GatebreakerBlade));
            Assert.That(gameState.OwnedGearIds, Has.Count.EqualTo(3));
        }
    }
}

