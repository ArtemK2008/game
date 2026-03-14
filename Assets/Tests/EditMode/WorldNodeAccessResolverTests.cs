using System.Collections.Generic;
using System.Linq;
using NUnit.Framework;
using Survivalon.Runtime;

namespace Survivalon.Tests.EditMode
{
    public sealed class WorldNodeAccessResolverTests
    {
        [Test]
        public void ShouldIncludeClearedNodeForFarmAccessEvenWhenPathRulesWouldNotReachIt()
        {
            WorldGraph worldGraph = WorldFlowTestData.CreateFarmAccessGraph(includeLockedConnection: true);
            PersistentWorldState worldState = WorldFlowTestData.CreateFarmAccessWorldState();
            WorldNodeAccessResolver resolver = new WorldNodeAccessResolver();

            IReadOnlyList<NodeId> enterableNodeIds = resolver.GetEnterableNodes(worldGraph, worldState)
                .Select(node => node.NodeId)
                .ToArray();

            CollectionAssert.AreEquivalent(
                new[]
                {
                    new NodeId("node_reachable"),
                    new NodeId("node_cleared_farm"),
                },
                enterableNodeIds);
        }

        [Test]
        public void ShouldNotIncludeUnclearedOrLockedNodeWhenOnlyFarmAccessWouldMakeItReachable()
        {
            WorldGraph worldGraph = WorldFlowTestData.CreateFarmAccessGraph(includeLockedConnection: true);
            PersistentWorldState worldState = WorldFlowTestData.CreateFarmAccessWorldState();
            WorldNodeAccessResolver resolver = new WorldNodeAccessResolver();

            IReadOnlyList<NodeId> enterableNodeIds = resolver.GetEnterableNodes(worldGraph, worldState)
                .Select(node => node.NodeId)
                .ToArray();

            Assert.That(enterableNodeIds, Has.No.Member(new NodeId("node_unreachable_available")));
            Assert.That(enterableNodeIds, Has.No.Member(new NodeId("node_locked")));
        }

        [Test]
        public void ShouldRejectLockedPathCandidateFromDirectEnterableLookup()
        {
            WorldGraph worldGraph = WorldFlowTestData.CreateFarmAccessGraph(includeLockedConnection: true);
            PersistentWorldState worldState = WorldFlowTestData.CreateFarmAccessWorldState();
            WorldNodeAccessResolver resolver = new WorldNodeAccessResolver();

            bool foundLockedNode = resolver.TryGetEnterableNode(worldGraph, worldState, new NodeId("node_locked"), out WorldNode lockedNode);

            Assert.That(foundLockedNode, Is.False);
            Assert.That(lockedNode, Is.Null);
        }

        [Test]
        public void ShouldReturnOnlyForwardNodesThatAreActuallyEnterable()
        {
            WorldGraph worldGraph = WorldFlowTestData.CreateFarmAccessGraph(includeLockedConnection: true);
            PersistentWorldState worldState = WorldFlowTestData.CreateFarmAccessWorldState();
            WorldNodeAccessResolver resolver = new WorldNodeAccessResolver();

            IReadOnlyList<NodeId> enterableNodeIds = resolver.GetForwardEnterableNodes(worldGraph, worldState)
                .Select(node => node.NodeId)
                .ToArray();

            CollectionAssert.AreEquivalent(
                new[]
                {
                    new NodeId("node_reachable"),
                },
                enterableNodeIds);
        }

    }
}
