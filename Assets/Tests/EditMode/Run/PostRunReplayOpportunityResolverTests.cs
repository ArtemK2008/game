using NUnit.Framework;
using Survivalon.Run;
using Survivalon.Tests.EditMode.World;
using Survivalon.World;

namespace Survivalon.Tests.EditMode.Run
{
    public sealed class PostRunReplayOpportunityResolverTests
    {
        [Test]
        public void ShouldResolveContinueNodeProgressWhenTrackedNodeStillNeedsProgress()
        {
            PostRunStateController postRunStateController = new PostRunStateController(
                NodePlaceholderTestData.CreatePushCombatPlaceholderState(),
                new RunResult(
                    BootstrapWorldScenario.ForestPushNodeId,
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
                        canStopSession: true)));

            PostRunReplayReasonKind replayReasonKind =
                new PostRunReplayOpportunityResolver().Resolve(postRunStateController);

            Assert.That(replayReasonKind, Is.EqualTo(PostRunReplayReasonKind.ContinueNodeProgress));
        }

        [Test]
        public void ShouldResolveRetryAttemptWhenRunFails()
        {
            PostRunStateController postRunStateController = new PostRunStateController(
                NodePlaceholderTestData.CreateForestGateBossPlaceholderState(),
                new RunResult(
                    BootstrapWorldScenario.ForestGateNodeId,
                    RunResolutionState.Failed,
                    RunRewardPayload.Empty,
                    0,
                    0,
                    3,
                    0,
                    false,
                    new RunNextActionContext(
                        canReplayNode: true,
                        canChooseAnotherNode: true,
                        canStopSession: true)));

            PostRunReplayReasonKind replayReasonKind =
                new PostRunReplayOpportunityResolver().Resolve(postRunStateController);

            Assert.That(replayReasonKind, Is.EqualTo(PostRunReplayReasonKind.RetryAttempt));
        }
    }
}
