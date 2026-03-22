using NUnit.Framework;
using Survivalon.Core;
using Survivalon.Run;
using Survivalon.State.Persistence;
using Survivalon.Tests.EditMode.World;
using Survivalon.World;

namespace Survivalon.Tests.EditMode.Run
{
    public sealed class PostRunNextActionResolverTests
    {
        [Test]
        public void ShouldRecommendReplayWhenTrackedNodeStillNeedsProgress()
        {
            PostRunStateController postRunStateController = CreateController(
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

            PostRunNextActionState nextActionState = new PostRunNextActionResolver().Resolve(
                postRunStateController,
                worldGraph: null,
                persistentWorldState: null,
                resourceBalancesState: null,
                persistentProgressionState: null);

            Assert.That(nextActionState.RecommendedActionKind, Is.EqualTo(PostRunRecommendedActionKind.Replay));
            Assert.That(nextActionState.ReplayReasonKind, Is.EqualTo(PostRunReplayReasonKind.ContinueNodeProgress));
            Assert.That(nextActionState.HasForwardPushOpportunity, Is.False);
            Assert.That(nextActionState.HasServiceOpportunity, Is.False);
        }

        [Test]
        public void ShouldRecommendPushWhenBossGateUnlockOpensForwardTarget()
        {
            WorldGraph worldGraph = BootstrapWorldTestData.CreateWorldGraph();
            PostRunStateController postRunStateController = CreateController(
                NodePlaceholderTestData.CreateForestGateBossPlaceholderState(),
                new RunResult(
                    BootstrapWorldScenario.ForestGateNodeId,
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
                        canStopSession: true),
                    BossProgressionGateUnlockResult.CreateUnlocked(BootstrapWorldScenario.CavernGateNodeId)));

            PostRunNextActionState nextActionState = new PostRunNextActionResolver().Resolve(
                postRunStateController,
                worldGraph,
                persistentWorldState: null,
                resourceBalancesState: null,
                persistentProgressionState: null);

            Assert.That(nextActionState.RecommendedActionKind, Is.EqualTo(PostRunRecommendedActionKind.ReturnToWorldPush));
            Assert.That(nextActionState.ForwardTargetDisplayName, Is.EqualTo("Cavern Gate"));
        }

        [Test]
        public void ShouldRecommendServiceHubWhenRefinementIsReady()
        {
            PersistentGameState gameState = BootstrapWorldTestData.CreateGameState();
            WorldGraph worldGraph = BootstrapWorldTestData.CreateWorldGraph();
            gameState.WorldState.SetCurrentNode(BootstrapWorldScenario.ForestPushNodeId);
            gameState.WorldState.SetLastSafeNode(BootstrapWorldScenario.ForestEntryNodeId);
            gameState.ResourceBalances.Add(ResourceCategory.RegionMaterial, 3);

            PostRunStateController postRunStateController = CreateController(
                NodePlaceholderTestData.CreatePushCombatPlaceholderState(),
                new RunResult(
                    BootstrapWorldScenario.ForestPushNodeId,
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
                        canStopSession: true)));

            PostRunNextActionState nextActionState = new PostRunNextActionResolver().Resolve(
                postRunStateController,
                worldGraph,
                gameState.WorldState,
                gameState.ResourceBalances,
                gameState.ProgressionState);

            Assert.That(nextActionState.RecommendedActionKind, Is.EqualTo(PostRunRecommendedActionKind.ReturnToWorldService));
            Assert.That(nextActionState.ServiceHubDisplayName, Is.EqualTo("Cavern Service Hub"));
            Assert.That(nextActionState.ServiceOpportunityKind, Is.EqualTo(PostRunServiceOpportunityKind.ReadyRefinement));
            Assert.That(nextActionState.ForwardTargetDisplayName, Is.EqualTo("Forest Farm"));
        }

        [Test]
        public void ShouldRefreshRecommendationWhenPersistentStateChanges()
        {
            PersistentGameState gameState = BootstrapWorldTestData.CreateGameState();
            WorldGraph worldGraph = BootstrapWorldTestData.CreateWorldGraph();
            gameState.WorldState.SetCurrentNode(BootstrapWorldScenario.ForestPushNodeId);
            gameState.WorldState.SetLastSafeNode(BootstrapWorldScenario.ForestEntryNodeId);

            PostRunStateController postRunStateController = CreateController(
                NodePlaceholderTestData.CreatePushCombatPlaceholderState(),
                new RunResult(
                    BootstrapWorldScenario.ForestPushNodeId,
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
                        canStopSession: true)));
            PostRunNextActionResolver resolver = new PostRunNextActionResolver();

            PostRunNextActionState baselineState = resolver.Resolve(
                postRunStateController,
                worldGraph,
                gameState.WorldState,
                gameState.ResourceBalances,
                gameState.ProgressionState);

            gameState.ResourceBalances.Add(ResourceCategory.RegionMaterial, 3);

            PostRunNextActionState updatedState = resolver.Resolve(
                postRunStateController,
                worldGraph,
                gameState.WorldState,
                gameState.ResourceBalances,
                gameState.ProgressionState);

            Assert.That(baselineState.RecommendedActionKind, Is.EqualTo(PostRunRecommendedActionKind.Replay));
            Assert.That(baselineState.HasServiceOpportunity, Is.False);
            Assert.That(updatedState.RecommendedActionKind, Is.EqualTo(PostRunRecommendedActionKind.ReturnToWorldService));
            Assert.That(updatedState.ServiceOpportunityKind, Is.EqualTo(PostRunServiceOpportunityKind.ReadyRefinement));
        }

        private static PostRunStateController CreateController(
            NodePlaceholderState placeholderState,
            RunResult runResult)
        {
            return new PostRunStateController(placeholderState, runResult);
        }
    }
}
