using NUnit.Framework;
using Survivalon.Core;
using Survivalon.Data.Gear;
using Survivalon.Run;
using Survivalon.Tests.EditMode.World;
using Survivalon.World;

namespace Survivalon.Tests.EditMode.Run
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
                runResult,
                BootstrapWorldTestData.CreateWorldGraph());

            string summaryText = PostRunSummaryTextBuilder.Build(postRunStateController, runResult);

            Assert.That(summaryText, Is.EqualTo(
                "Run complete.\n" +
                "Location: Verdant Frontier\n" +
                "Node: Forest Farm\n" +
                "Result: Succeeded\n" +
                "Rewards: Soft currency x1, Region material x1\n" +
                "Source: Frontier salvage\n" +
                "Clear bonus: Persistent progression material x1\n" +
                "Unlocks: Cavern Gate opened\n" +
                "Progress: node +1 this run; tracked total 3 / 3; persistent +0\n"));
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

            Assert.That(summaryText, Does.Contain("Rewards: None"));
            Assert.That(summaryText, Does.Contain("Location: Echo Caverns"));
            Assert.That(summaryText, Does.Contain("Node: Cavern Service Hub"));
            Assert.That(summaryText, Does.Not.Contain("Source:"));
            Assert.That(summaryText, Does.Contain("Progress: node not tracked; persistent +0"));
            Assert.That(summaryText, Does.Not.Contain("Unlocks:"));
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

            Assert.That(summaryText, Does.Contain("Progress: node +1 this run; tracked total 2 / 3; persistent +0"));
            Assert.That(summaryText, Does.Not.Contain("Clear bonus:"));
        }

        [Test]
        public void ShouldOmitBossGateUnlockLineWhenNoBossProgressionGateWasUnlocked()
        {
            RunResult runResult = CreateSucceededRunResult();
            PostRunStateController postRunStateController = new PostRunStateController(
                NodePlaceholderTestData.CreateCombatPlaceholderState(),
                runResult);

            string summaryText = PostRunSummaryTextBuilder.Build(postRunStateController, runResult);

            Assert.That(summaryText, Does.Not.Contain("Unlocks:"));
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
                runResult,
                BootstrapWorldTestData.CreateWorldGraph());

            string summaryText = PostRunSummaryTextBuilder.Build(postRunStateController, runResult);

            Assert.That(summaryText, Does.Contain("Rewards: Soft currency x1"));
            Assert.That(summaryText, Does.Contain("Source: Frontier salvage"));
            Assert.That(summaryText, Does.Contain("Boss bonus: Persistent progression material x2"));
            Assert.That(summaryText, Does.Not.Contain("Boss gear:"));
            Assert.That(summaryText, Does.Not.Contain("Clear bonus:"));
        }

        [Test]
        public void ShouldShowGearRewardsOnSeparateLineWhenBossGearRewardIsPresent()
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
                    },
                    new[]
                    {
                        new RunGearReward(GearIds.GatebreakerBlade),
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
                runResult,
                BootstrapWorldTestData.CreateWorldGraph());

            string summaryText = PostRunSummaryTextBuilder.Build(postRunStateController, runResult);

            Assert.That(summaryText, Does.Contain("Boss bonus: Persistent progression material x2"));
            Assert.That(summaryText, Does.Contain("Boss gear: Gatebreaker Blade"));
        }

        [Test]
        public void ShouldKeepCombinedRewardSpikesSeparatedWhenMultipleSpikeTypesArePresent()
        {
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
                runResult,
                BootstrapWorldTestData.CreateWorldGraph());

            string summaryText = PostRunSummaryTextBuilder.Build(postRunStateController, runResult);

            Assert.That(summaryText, Does.Contain("Rewards: Soft currency x1, Region material x1"));
            Assert.That(summaryText, Does.Contain("Clear bonus: Persistent progression material x1"));
            Assert.That(summaryText, Does.Contain("Boss bonus: Persistent progression material x2"));
            Assert.That(summaryText, Does.Contain("Boss gear: Gatebreaker Blade"));
            Assert.That(summaryText, Does.Contain("Unlocks: Forward route opened; Cavern Gate opened"));
            Assert.That(summaryText, Does.Contain("Progress: node +1 this run; tracked total 3 / 3; persistent +0"));
            Assert.That(summaryText, Does.Not.Contain("route unlock"));
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
            Assert.That(summaryText, Does.Contain("Source: Cavern relic caches"));
            Assert.That(summaryText, Does.Contain("Boss bonus: Persistent progression material x3"));
            Assert.That(summaryText, Does.Not.Contain("Clear bonus:"));
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

