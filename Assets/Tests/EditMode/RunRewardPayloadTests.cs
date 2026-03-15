using System;
using NUnit.Framework;
using Survivalon.Runtime;

namespace Survivalon.Tests.EditMode
{
    public sealed class RunRewardPayloadTests
    {
        [Test]
        public void ShouldHoldCurrencyRewards()
        {
            RunRewardPayload rewardPayload = new RunRewardPayload(
                new[]
                {
                    new RunCurrencyReward(ResourceCategory.SoftCurrency, 12),
                },
                Array.Empty<RunMaterialReward>());

            Assert.That(rewardPayload.CurrencyRewards, Has.Count.EqualTo(1));
            Assert.That(rewardPayload.CurrencyRewards[0].ResourceCategory, Is.EqualTo(ResourceCategory.SoftCurrency));
            Assert.That(rewardPayload.CurrencyRewards[0].Amount, Is.EqualTo(12));
            Assert.That(rewardPayload.MaterialRewards, Is.Empty);
            Assert.That(rewardPayload.HasCurrencyRewards, Is.True);
            Assert.That(rewardPayload.HasRewards, Is.True);
        }

        [Test]
        public void ShouldHoldMaterialRewards()
        {
            RunRewardPayload rewardPayload = new RunRewardPayload(
                Array.Empty<RunCurrencyReward>(),
                new[]
                {
                    new RunMaterialReward(ResourceCategory.RegionMaterial, 3),
                    new RunMaterialReward(ResourceCategory.PersistentProgressionMaterial, 1),
                });

            Assert.That(rewardPayload.CurrencyRewards, Is.Empty);
            Assert.That(rewardPayload.MaterialRewards, Has.Count.EqualTo(2));
            Assert.That(rewardPayload.MaterialRewards[0].ResourceCategory, Is.EqualTo(ResourceCategory.RegionMaterial));
            Assert.That(rewardPayload.MaterialRewards[0].Amount, Is.EqualTo(3));
            Assert.That(rewardPayload.MaterialRewards[1].ResourceCategory, Is.EqualTo(ResourceCategory.PersistentProgressionMaterial));
            Assert.That(rewardPayload.MaterialRewards[1].Amount, Is.EqualTo(1));
            Assert.That(rewardPayload.HasMaterialRewards, Is.True);
            Assert.That(rewardPayload.HasRewards, Is.True);
        }

        [Test]
        public void ShouldCarryStructuredRewardPayloadInRunResult()
        {
            RunRewardPayload rewardPayload = new RunRewardPayload(
                new[]
                {
                    new RunCurrencyReward(ResourceCategory.SoftCurrency, 18),
                },
                new[]
                {
                    new RunMaterialReward(ResourceCategory.RegionMaterial, 4),
                });

            RunResult runResult = new RunResult(
                new NodeId("region_001_node_001"),
                RunResolutionState.Succeeded,
                rewardPayload,
                0,
                0,
                0,
                0,
                false,
                new RunNextActionContext(
                    canReplayNode: true,
                    canChooseAnotherNode: true,
                    canStopSession: true));

            Assert.That(runResult.RewardPayload.CurrencyRewards, Has.Count.EqualTo(1));
            Assert.That(runResult.RewardPayload.MaterialRewards, Has.Count.EqualTo(1));
            Assert.That(runResult.RewardPayload.CurrencyRewards[0].Amount, Is.EqualTo(18));
            Assert.That(runResult.RewardPayload.MaterialRewards[0].Amount, Is.EqualTo(4));
        }

        [Test]
        public void ShouldRejectMaterialResourceAsCurrencyReward()
        {
            Assert.That(
                () => new RunCurrencyReward(ResourceCategory.RegionMaterial, 1),
                Throws.ArgumentException);
        }

        [Test]
        public void ShouldRejectCurrencyResourceAsMaterialReward()
        {
            Assert.That(
                () => new RunMaterialReward(ResourceCategory.SoftCurrency, 1),
                Throws.ArgumentException);
        }
    }
}
