using NUnit.Framework;
using Survivalon.Core;
using Survivalon.State.Persistence;
using Survivalon.Tests.EditMode.World;
using Survivalon.World;

namespace Survivalon.Tests.EditMode.State.Persistence
{
    public sealed class OfflineProgressEligibilityResolverTests
    {
        [Test]
        public void ShouldMarkFarmReadyWorldMapContextAsEligible()
        {
            OfflineProgressEligibilityResolver resolver = new OfflineProgressEligibilityResolver(
                WorldFlowTestData.CreateFarmAccessGraph());

            OfflineProgressEligibilityKind eligibilityKind = resolver.Resolve(
                WorldFlowTestData.CreateFarmAccessWorldState(),
                SafeResumeTargetType.WorldMap,
                new NodeId("node_cleared_farm"));

            Assert.That(eligibilityKind, Is.EqualTo(OfflineProgressEligibilityKind.FarmReadyWorldNode));
        }

        [Test]
        public void ShouldKeepClearedWorldMapContextWithoutExplicitFarmYieldIneligible()
        {
            OfflineProgressEligibilityResolver resolver = new OfflineProgressEligibilityResolver(
                BootstrapWorldTestData.CreateWorldGraph());
            PersistentWorldState worldState = BootstrapWorldTestData.CreateWorldState();
            worldState.SetCurrentNode(BootstrapWorldScenario.ForestPushNodeId);
            worldState.SetLastSafeNode(BootstrapWorldScenario.ForestPushNodeId);
            worldState.ReplaceReachableNodes(new[] { BootstrapWorldScenario.ForestPushNodeId });
            Assert.That(
                worldState.TryGetNodeState(BootstrapWorldScenario.ForestPushNodeId, out PersistentNodeState nodeState),
                Is.True);
            nodeState.ApplyUnlockProgress(nodeState.UnlockThreshold);

            OfflineProgressEligibilityKind eligibilityKind = resolver.Resolve(
                worldState,
                SafeResumeTargetType.WorldMap,
                BootstrapWorldScenario.ForestPushNodeId);

            Assert.That(eligibilityKind, Is.EqualTo(OfflineProgressEligibilityKind.None));
        }

        [Test]
        public void ShouldKeepTownServiceContextIneligible()
        {
            OfflineProgressEligibilityResolver resolver = new OfflineProgressEligibilityResolver(
                WorldFlowTestData.CreateFarmAccessGraph());

            OfflineProgressEligibilityKind eligibilityKind = resolver.Resolve(
                WorldFlowTestData.CreateFarmAccessWorldState(),
                SafeResumeTargetType.TownService,
                new NodeId("node_cleared_farm"));

            Assert.That(eligibilityKind, Is.EqualTo(OfflineProgressEligibilityKind.None));
        }

        [Test]
        public void ShouldFailClosedWhenResumeNodeIsUnknown()
        {
            OfflineProgressEligibilityResolver resolver = new OfflineProgressEligibilityResolver(
                WorldFlowTestData.CreateFarmAccessGraph());

            OfflineProgressEligibilityKind eligibilityKind = resolver.Resolve(
                WorldFlowTestData.CreateFarmAccessWorldState(),
                SafeResumeTargetType.WorldMap,
                new NodeId("node_missing"));

            Assert.That(eligibilityKind, Is.EqualTo(OfflineProgressEligibilityKind.None));
        }
    }
}
