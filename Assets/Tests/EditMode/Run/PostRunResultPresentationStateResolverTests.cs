using NUnit.Framework;
using Survivalon.Core;
using Survivalon.Data.Gear;
using Survivalon.Run;
using Survivalon.Tests.EditMode.World;
using Survivalon.World;

namespace Survivalon.Tests.EditMode.Run
{
    public sealed class PostRunResultPresentationStateResolverTests
    {
        [Test]
        public void ShouldResolveOrdinaryRunPresentationWithoutSpikeOrUnlockLines()
        {
            PostRunResultPresentationStateResolver resolver = new PostRunResultPresentationStateResolver();
            RunResult runResult = new RunResult(
                BootstrapWorldScenario.ForestFarmNodeId,
                RunResolutionState.Succeeded,
                new RunRewardPayload(
                    new[]
                    {
                        new RunCurrencyReward(ResourceCategory.SoftCurrency, 1),
                    },
                    new[]
                    {
                        new RunMaterialReward(ResourceCategory.RegionMaterial, 2),
                    }),
                1,
                1,
                3,
                0,
                false,
                new RunNextActionContext(
                    canReplayNode: true,
                    canChooseAnotherNode: true,
                    canStopSession: true));
            PostRunStateController postRunStateController = new PostRunStateController(
                NodePlaceholderTestData.CreateFrontierFarmPlaceholderState(),
                runResult);

            PostRunResultPresentationState presentationState =
                resolver.Resolve(postRunStateController, runResult);

            Assert.That(presentationState.OrdinaryRewardSummary, Is.EqualTo("Soft currency x1, Region material x2"));
            Assert.That(presentationState.RewardSourceSummary, Is.EqualTo("Frontier salvage"));
            Assert.That(presentationState.HasClearSpikeRewardSummary, Is.False);
            Assert.That(presentationState.HasBossSpikeRewardSummary, Is.False);
            Assert.That(presentationState.HasBossGearRewardSummary, Is.False);
            Assert.That(presentationState.HasUnlockOutcomeSummary, Is.False);
            Assert.That(presentationState.ProgressSummary, Is.EqualTo("node +1 this run; tracked total 1 / 3; persistent +0"));
        }

        [Test]
        public void ShouldResolveDistinctSpikeAndUnlockPresentationForCombinedRunResult()
        {
            PostRunResultPresentationStateResolver resolver = new PostRunResultPresentationStateResolver();
            RunResult runResult = new RunResult(
                BootstrapWorldScenario.ForestGateNodeId,
                RunResolutionState.Succeeded,
                new RunRewardPayload(
                    new[]
                    {
                        new RunCurrencyReward(ResourceCategory.SoftCurrency, 1),
                    },
                    new[]
                    {
                        new RunMaterialReward(ResourceCategory.RegionMaterial, 1),
                    },
                    System.Array.Empty<RunCurrencyReward>(),
                    new[]
                    {
                        new RunMaterialReward(ResourceCategory.PersistentProgressionMaterial, 1),
                    },
                    System.Array.Empty<RunCurrencyReward>(),
                    new[]
                    {
                        new RunMaterialReward(ResourceCategory.PersistentProgressionMaterial, 2),
                    },
                    new[]
                    {
                        new RunGearReward(GearIds.GatebreakerBlade),
                    }),
                1,
                3,
                3,
                0,
                true,
                new RunNextActionContext(
                    canReplayNode: true,
                    canChooseAnotherNode: true,
                    canStopSession: true),
                BossProgressionGateUnlockResult.CreateUnlocked(BootstrapWorldScenario.CavernGateNodeId));
            PostRunStateController postRunStateController = new PostRunStateController(
                NodePlaceholderTestData.CreateForestGateBossPlaceholderState(),
                runResult);

            PostRunResultPresentationState presentationState =
                resolver.Resolve(postRunStateController, runResult);

            Assert.That(presentationState.OrdinaryRewardSummary, Is.EqualTo("Soft currency x1, Region material x1"));
            Assert.That(presentationState.ClearSpikeRewardSummary, Is.EqualTo("Persistent progression material x1"));
            Assert.That(presentationState.BossSpikeRewardSummary, Is.EqualTo("Persistent progression material x2"));
            Assert.That(presentationState.BossGearRewardSummary, Is.EqualTo("Gatebreaker Blade"));
            Assert.That(
                presentationState.UnlockOutcomeSummary,
                Is.EqualTo("Forward route opened; Cavern gate opened"));
            Assert.That(presentationState.ProgressSummary, Is.EqualTo("node +1 this run; tracked total 3 / 3; persistent +0"));
        }
    }
}
