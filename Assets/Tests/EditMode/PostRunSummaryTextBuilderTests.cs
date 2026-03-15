using NUnit.Framework;
using Survivalon.Runtime;

namespace Survivalon.Tests.EditMode
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
                    }),
                1,
                1,
                3,
                0,
                true,
                new RunNextActionContext(
                    canReplayNode: true,
                    canChooseAnotherNode: true,
                    canStopSession: true));
            PostRunStateController postRunStateController = new PostRunStateController(
                NodePlaceholderTestData.CreateCombatPlaceholderState(),
                runResult);

            string summaryText = PostRunSummaryTextBuilder.Build(postRunStateController, runResult);

            Assert.That(summaryText, Is.EqualTo(
                "Run finished.\n" +
                "Node: region_001_node_004\n" +
                "Resolution: Succeeded\n" +
                "Rewards gained: Soft currency x1, Region material x1\n" +
                "Progress changes: node +1 (1 / 3), persistent +0, route unlock Yes\n" +
                "Next actions:\n" +
                "- Replay: Yes\n" +
                "- Return to world: Yes\n" +
                "- Stop: Yes"));
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
            Assert.That(summaryText, Does.Contain("Progress changes: node not tracked, persistent +0, route unlock No"));
            Assert.That(summaryText, Does.Contain("- Stop: No"));
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
