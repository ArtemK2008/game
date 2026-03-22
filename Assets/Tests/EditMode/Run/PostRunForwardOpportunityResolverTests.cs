using NUnit.Framework;
using Survivalon.Core;
using Survivalon.Run;
using Survivalon.State.Persistence;
using Survivalon.Tests.EditMode.World;
using Survivalon.World;

namespace Survivalon.Tests.EditMode.Run
{
    public sealed class PostRunForwardOpportunityResolverTests
    {
        [Test]
        public void ShouldResolveNewlyUnlockedBossGateAsPushTarget()
        {
            PostRunForwardOpportunityState forwardOpportunityState =
                new PostRunForwardOpportunityResolver().Resolve(
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
                        BossProgressionGateUnlockResult.CreateUnlocked(BootstrapWorldScenario.CavernGateNodeId)),
                    BootstrapWorldTestData.CreateWorldGraph(),
                    persistentWorldState: null);

            Assert.That(forwardOpportunityState.TargetDisplayName, Is.EqualTo("Cavern Gate"));
            Assert.That(
                forwardOpportunityState.OpportunityKind,
                Is.EqualTo(PostRunForwardOpportunityKind.NewlyUnlockedPushTarget));
        }

        [Test]
        public void ShouldKeepServiceHubOutOfForwardPushTargetSelection()
        {
            PersistentGameState gameState = BootstrapWorldTestData.CreateGameState();
            gameState.WorldState.SetCurrentNode(BootstrapWorldScenario.ForestPushNodeId);
            gameState.WorldState.SetLastSafeNode(BootstrapWorldScenario.ForestEntryNodeId);

            PostRunForwardOpportunityState forwardOpportunityState =
                new PostRunForwardOpportunityResolver().Resolve(
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
                            canStopSession: true)),
                    BootstrapWorldTestData.CreateWorldGraph(),
                    gameState.WorldState);

            Assert.That(forwardOpportunityState.TargetDisplayName, Is.EqualTo("Forest Farm"));
            Assert.That(
                forwardOpportunityState.OpportunityKind,
                Is.EqualTo(PostRunForwardOpportunityKind.AvailablePushTarget));
        }
    }
}
