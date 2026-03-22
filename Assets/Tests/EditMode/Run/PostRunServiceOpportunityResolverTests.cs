using NUnit.Framework;
using Survivalon.Core;
using Survivalon.Run;
using Survivalon.State.Persistence;
using Survivalon.Tests.EditMode.World;
using Survivalon.World;

namespace Survivalon.Tests.EditMode.Run
{
    public sealed class PostRunServiceOpportunityResolverTests
    {
        [Test]
        public void ShouldResolveReadyRefinementIndependentlyFromForwardPushTarget()
        {
            PersistentGameState gameState = BootstrapWorldTestData.CreateGameState();
            gameState.WorldState.SetCurrentNode(BootstrapWorldScenario.ForestPushNodeId);
            gameState.WorldState.SetLastSafeNode(BootstrapWorldScenario.ForestEntryNodeId);
            gameState.ResourceBalances.Add(ResourceCategory.RegionMaterial, 3);

            PostRunServiceOpportunityState serviceOpportunityState =
                new PostRunServiceOpportunityResolver().Resolve(
                    BootstrapWorldTestData.CreateWorldGraph(),
                    gameState.WorldState,
                    gameState.ResourceBalances,
                    gameState.ProgressionState);

            Assert.That(serviceOpportunityState.ServiceHubDisplayName, Is.EqualTo("Cavern Service Hub"));
            Assert.That(
                serviceOpportunityState.OpportunityKind,
                Is.EqualTo(PostRunServiceOpportunityKind.ReadyRefinement));
            Assert.That(serviceOpportunityState.HasOpportunity, Is.True);
        }
    }
}
