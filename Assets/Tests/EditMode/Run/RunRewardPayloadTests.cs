using System;
using System.Collections.Generic;
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
            Assert.That(rewardPayload.HasOrdinaryRewards, Is.True);
            Assert.That(rewardPayload.HasMilestoneRewards, Is.False);
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
                });

            Assert.That(rewardPayload.CurrencyRewards, Is.Empty);
            Assert.That(rewardPayload.MaterialRewards, Has.Count.EqualTo(1));
            Assert.That(rewardPayload.MaterialRewards[0].ResourceCategory, Is.EqualTo(ResourceCategory.RegionMaterial));
            Assert.That(rewardPayload.MaterialRewards[0].Amount, Is.EqualTo(3));
            Assert.That(rewardPayload.HasMaterialRewards, Is.True);
            Assert.That(rewardPayload.HasOrdinaryRewards, Is.True);
            Assert.That(rewardPayload.HasMilestoneRewards, Is.False);
            Assert.That(rewardPayload.HasRewards, Is.True);
        }

        [Test]
        public void ShouldHoldMilestoneMaterialRewards()
        {
            RunRewardPayload rewardPayload = new RunRewardPayload(
                Array.Empty<RunCurrencyReward>(),
                Array.Empty<RunMaterialReward>(),
                Array.Empty<RunCurrencyReward>(),
                new[]
                {
                    new RunMaterialReward(ResourceCategory.PersistentProgressionMaterial, 1),
                });

            Assert.That(rewardPayload.CurrencyRewards, Is.Empty);
            Assert.That(rewardPayload.MaterialRewards, Is.Empty);
            Assert.That(rewardPayload.MilestoneCurrencyRewards, Is.Empty);
            Assert.That(rewardPayload.MilestoneMaterialRewards, Has.Count.EqualTo(1));
            Assert.That(rewardPayload.MilestoneMaterialRewards[0].ResourceCategory, Is.EqualTo(ResourceCategory.PersistentProgressionMaterial));
            Assert.That(rewardPayload.MilestoneMaterialRewards[0].Amount, Is.EqualTo(1));
            Assert.That(rewardPayload.HasOrdinaryRewards, Is.False);
            Assert.That(rewardPayload.HasMilestoneRewards, Is.True);
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
                },
                Array.Empty<RunCurrencyReward>(),
                new[]
                {
                    new RunMaterialReward(ResourceCategory.PersistentProgressionMaterial, 1),
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
            Assert.That(runResult.RewardPayload.MilestoneMaterialRewards, Has.Count.EqualTo(1));
            Assert.That(runResult.RewardPayload.CurrencyRewards[0].Amount, Is.EqualTo(18));
            Assert.That(runResult.RewardPayload.MaterialRewards[0].Amount, Is.EqualTo(4));
            Assert.That(runResult.RewardPayload.MilestoneMaterialRewards[0].Amount, Is.EqualTo(1));
        }

        [Test]
        public void ShouldCopyRewardCollectionsOnConstruction()
        {
            RunCurrencyReward initialCurrencyReward = new RunCurrencyReward(ResourceCategory.SoftCurrency, 12);
            RunMaterialReward initialMaterialReward = new RunMaterialReward(ResourceCategory.RegionMaterial, 3);
            RunMaterialReward initialMilestoneMaterialReward = new RunMaterialReward(ResourceCategory.PersistentProgressionMaterial, 1);
            RunRewardPayload rewardPayload;
            List<RunCurrencyReward> currencyRewards = new List<RunCurrencyReward> { initialCurrencyReward };
            List<RunMaterialReward> materialRewards = new List<RunMaterialReward> { initialMaterialReward };
            List<RunMaterialReward> milestoneMaterialRewards = new List<RunMaterialReward> { initialMilestoneMaterialReward };

            rewardPayload = new RunRewardPayload(
                currencyRewards,
                materialRewards,
                Array.Empty<RunCurrencyReward>(),
                milestoneMaterialRewards);

            currencyRewards.Add(new RunCurrencyReward(ResourceCategory.SoftCurrency, 99));
            materialRewards.Clear();
            milestoneMaterialRewards.Clear();

            Assert.That(rewardPayload.CurrencyRewards, Has.Count.EqualTo(1));
            Assert.That(rewardPayload.CurrencyRewards[0], Is.EqualTo(initialCurrencyReward));
            Assert.That(rewardPayload.MaterialRewards, Has.Count.EqualTo(1));
            Assert.That(rewardPayload.MaterialRewards[0], Is.EqualTo(initialMaterialReward));
            Assert.That(rewardPayload.MilestoneMaterialRewards, Has.Count.EqualTo(1));
            Assert.That(rewardPayload.MilestoneMaterialRewards[0], Is.EqualTo(initialMilestoneMaterialReward));
        }

        [Test]
        public void ShouldExposeReadOnlyRewardCollections()
        {
            RunRewardPayload rewardPayload = new RunRewardPayload(
                new[]
                {
                    new RunCurrencyReward(ResourceCategory.SoftCurrency, 12),
                },
                new[]
                {
                    new RunMaterialReward(ResourceCategory.RegionMaterial, 3),
                },
                Array.Empty<RunCurrencyReward>(),
                new[]
                {
                    new RunMaterialReward(ResourceCategory.PersistentProgressionMaterial, 1),
                });

            IList<RunCurrencyReward> currencyRewards = (IList<RunCurrencyReward>)rewardPayload.CurrencyRewards;
            IList<RunMaterialReward> materialRewards = (IList<RunMaterialReward>)rewardPayload.MaterialRewards;
            IList<RunMaterialReward> milestoneMaterialRewards = (IList<RunMaterialReward>)rewardPayload.MilestoneMaterialRewards;

            Assert.That(
                () => currencyRewards.Add(new RunCurrencyReward(ResourceCategory.SoftCurrency, 1)),
                Throws.TypeOf<NotSupportedException>());
            Assert.That(
                () => materialRewards.Add(new RunMaterialReward(ResourceCategory.RegionMaterial, 1)),
                Throws.TypeOf<NotSupportedException>());
            Assert.That(
                () => milestoneMaterialRewards.Add(new RunMaterialReward(ResourceCategory.PersistentProgressionMaterial, 1)),
                Throws.TypeOf<NotSupportedException>());
        }

        [Test]
        public void ShouldRejectMissingCurrencyRewardCollection()
        {
            Assert.That(
                () => new RunRewardPayload(null, Array.Empty<RunMaterialReward>()),
                Throws.ArgumentNullException.With.Property("ParamName").EqualTo("currencyRewards"));
        }

        [Test]
        public void ShouldRejectMissingMaterialRewardCollection()
        {
            Assert.That(
                () => new RunRewardPayload(Array.Empty<RunCurrencyReward>(), null),
                Throws.ArgumentNullException.With.Property("ParamName").EqualTo("materialRewards"));
        }

        [Test]
        public void ShouldRejectMissingMilestoneCurrencyRewardCollection()
        {
            Assert.That(
                () => new RunRewardPayload(
                    Array.Empty<RunCurrencyReward>(),
                    Array.Empty<RunMaterialReward>(),
                    null,
                    Array.Empty<RunMaterialReward>()),
                Throws.ArgumentNullException.With.Property("ParamName").EqualTo("milestoneCurrencyRewards"));
        }

        [Test]
        public void ShouldRejectMissingMilestoneMaterialRewardCollection()
        {
            Assert.That(
                () => new RunRewardPayload(
                    Array.Empty<RunCurrencyReward>(),
                    Array.Empty<RunMaterialReward>(),
                    Array.Empty<RunCurrencyReward>(),
                    null),
                Throws.ArgumentNullException.With.Property("ParamName").EqualTo("milestoneMaterialRewards"));
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
