using NUnit.Framework;
using Survivalon.Core;
using Survivalon.Run;
using Survivalon.World;

namespace Survivalon.Tests.EditMode.World
{
    public sealed class PostRunSummaryTextBuilderTests
    {
        [Test]
        public void ShouldBuildAggregatedRewardAndProgressSummaryForTrackedRun()
        {
            RunResult runResult = new RunResult(
                new NodeId("region_001_node_004"),
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
                    }),
                1,
                3,
                3,
                0,
                false,
                new RunNextActionContext(
                    canReplayNode: true,
                    canChooseAnotherNode: true,
                    canStopSession: true),
                BossProgressionGateUnlockResult.CreateUnlocked(BootstrapWorldScenario.CavernGateNodeId));
            PostRunStateController postRunStateController = new PostRunStateController(
                NodePlaceholderTestData.CreateCombatPlaceholderState(),
                runResult);

            string summaryText = PostRunSummaryTextBuilder.Build(postRunStateController, runResult);

            Assert.That(summaryText, Is.EqualTo(
                "Run finished.\n" +
                "Location: Verdant Frontier\n" +
                "Node: Forest Farm\n" +
                "Resolution: Succeeded\n" +
                "Rewards gained: Soft currency x1, Region material x1\n" +
                "Reward source: Frontier salvage\n" +
                "Milestone rewards: Persistent progression material x1\n" +
                "Progress changes: node +1 this run; tracked total 3 / 3; persistent +0; route unlock No\n" +
                "Boss gate unlock: Cavern gate opened\n"));
        }

        [Test]
        public void ShouldShowNoneAndUntrackedWhenRunHasNoRewardsOrTrackedProgress()
        {
            RunResult runResult = new RunResult(
                new NodeId("region_002_node_001"),
                RunResolutionState.Failed,
                RunRewardPayload.Empty,
                0,
                0,
                0,
                0,
                false,
                new RunNextActionContext(
                    canReplayNode: true,
                    canChooseAnotherNode: true,
                    canStopSession: false));
            PostRunStateController postRunStateController = new PostRunStateController(
                NodePlaceholderTestData.CreateServicePlaceholderState(),
                runResult);

            string summaryText = PostRunSummaryTextBuilder.Build(postRunStateController, runResult);

            Assert.That(summaryText, Does.Contain("Rewards gained: None"));
            Assert.That(summaryText, Does.Contain("Location: Echo Caverns"));
            Assert.That(summaryText, Does.Contain("Node: Cavern Service Hub"));
            Assert.That(summaryText, Does.Not.Contain("Reward source:"));
            Assert.That(summaryText, Does.Contain("Progress changes: node not tracked; persistent +0; route unlock No"));
            Assert.That(summaryText, Does.Not.Contain("Next actions:"));
        }

        [Test]
        public void ShouldDistinguishNodeProgressGainedThisRunFromTrackedTotal()
        {
            RunResult runResult = new RunResult(
                new NodeId("region_001_node_004"),
                RunResolutionState.Succeeded,
                RunRewardPayload.Empty,
                1,
                2,
                3,
                0,
                false,
                new RunNextActionContext(
                    canReplayNode: true,
                    canChooseAnotherNode: true,
                    canStopSession: true));
            PostRunStateController postRunStateController = new PostRunStateController(
                NodePlaceholderTestData.CreateCombatPlaceholderState(),
                runResult);

            string summaryText = PostRunSummaryTextBuilder.Build(postRunStateController, runResult);

            Assert.That(summaryText, Does.Contain("Progress changes: node +1 this run; tracked total 2 / 3; persistent +0; route unlock No"));
            Assert.That(summaryText, Does.Not.Contain("Milestone rewards:"));
        }

        [Test]
        public void ShouldOmitBossGateUnlockLineWhenNoBossProgressionGateWasUnlocked()
        {
            RunResult runResult = CreateSucceededRunResult();
            PostRunStateController postRunStateController = new PostRunStateController(
                NodePlaceholderTestData.CreateCombatPlaceholderState(),
                runResult);

            string summaryText = PostRunSummaryTextBuilder.Build(postRunStateController, runResult);

            Assert.That(summaryText, Does.Not.Contain("Boss gate unlock:"));
        }

        [Test]
        public void ShouldShowBossRewardsOnSeparateLineWhenPresent()
        {
            RunResult runResult = new RunResult(
                BootstrapWorldScenario.ForestGateNodeId,
                RunResolutionState.Succeeded,
                new RunRewardPayload(
                    new[]
                    {
                        new RunCurrencyReward(ResourceCategory.SoftCurrency, 1),
                    },
                    System.Array.Empty<RunMaterialReward>(),
                    System.Array.Empty<RunCurrencyReward>(),
                    System.Array.Empty<RunMaterialReward>(),
                    System.Array.Empty<RunCurrencyReward>(),
                    new[]
                    {
                        new RunMaterialReward(ResourceCategory.PersistentProgressionMaterial, 2),
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
                NodePlaceholderTestData.CreateForestGateBossPlaceholderState(),
                runResult);

            string summaryText = PostRunSummaryTextBuilder.Build(postRunStateController, runResult);

            Assert.That(summaryText, Does.Contain("Rewards gained: Soft currency x1"));
            Assert.That(summaryText, Does.Contain("Reward source: Frontier salvage"));
            Assert.That(summaryText, Does.Contain("Boss rewards: Persistent progression material x2"));
            Assert.That(summaryText, Does.Not.Contain("Milestone rewards:"));
        }

        [Test]
        public void ShouldShowCavernBossRewardBundleWithEchoCavernsRewardSource()
        {
            RunResult runResult = new RunResult(
                BootstrapWorldScenario.CavernGateNodeId,
                RunResolutionState.Succeeded,
                new RunRewardPayload(
                    new[]
                    {
                        new RunCurrencyReward(ResourceCategory.SoftCurrency, 1),
                    },
                    System.Array.Empty<RunMaterialReward>(),
                    System.Array.Empty<RunCurrencyReward>(),
                    System.Array.Empty<RunMaterialReward>(),
                    System.Array.Empty<RunCurrencyReward>(),
                    new[]
                    {
                        new RunMaterialReward(ResourceCategory.PersistentProgressionMaterial, 3),
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
                NodePlaceholderTestData.CreateCavernGateBossPlaceholderState(),
                runResult);

            string summaryText = PostRunSummaryTextBuilder.Build(postRunStateController, runResult);

            Assert.That(summaryText, Does.Contain("Location: Echo Caverns"));
            Assert.That(summaryText, Does.Contain("Reward source: Cavern relic caches"));
            Assert.That(summaryText, Does.Contain("Boss rewards: Persistent progression material x3"));
            Assert.That(summaryText, Does.Not.Contain("Milestone rewards:"));
        }

        [Test]
        public void ShouldRejectMissingPostRunStateController()
        {
            Assert.That(
                () => PostRunSummaryTextBuilder.Build(null, CreateSucceededRunResult()),
                Throws.ArgumentNullException.With.Property("ParamName").EqualTo("postRunStateController"));
        }

        [Test]
        public void ShouldRejectMissingRunResult()
        {
            Assert.That(
                () => PostRunSummaryTextBuilder.Build(
                    new PostRunStateController(
                        NodePlaceholderTestData.CreateCombatPlaceholderState(),
                        CreateSucceededRunResult()),
                    null),
                Throws.ArgumentNullException.With.Property("ParamName").EqualTo("runResult"));
        }

        private static RunResult CreateSucceededRunResult()
        {
            return new RunResult(
                new NodeId("region_001_node_004"),
                RunResolutionState.Succeeded,
                RunRewardPayload.Empty,
                1,
                1,
                3,
                0,
                false,
                new RunNextActionContext(
                    canReplayNode: true,
                    canChooseAnotherNode: true,
                    canStopSession: true));
        }
    }
}

