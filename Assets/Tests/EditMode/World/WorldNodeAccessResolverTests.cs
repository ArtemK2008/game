using System.Collections.Generic;
using System.Linq;
using NUnit.Framework;
using Survivalon.Core;
using Survivalon.State.Persistence;
using Survivalon.World;

namespace Survivalon.Tests.EditMode.World
{
    public sealed class WorldNodeAccessResolverTests
    {
        [Test]
        public void ShouldRejectMissingWorldGraphForEnterableNodeQueries()
        {
            WorldNodeAccessResolver resolver = new WorldNodeAccessResolver();

            Assert.That(
                () => resolver.GetEnterableNodes(null, new PersistentWorldState()),
                Throws.ArgumentNullException.With.Property("ParamName").EqualTo("worldGraph"));
        }

        [Test]
        public void ShouldRejectMissingWorldStateForForwardEnterableQueries()
        {
            WorldNodeAccessResolver resolver = new WorldNodeAccessResolver();

            Assert.That(
                () => resolver.GetForwardEnterableNodes(BootstrapWorldTestData.CreateWorldGraph(), null),
                Throws.ArgumentNullException.With.Property("ParamName").EqualTo("worldState"));
        }

        [Test]
        public void ShouldRejectMissingWorldGraphForDirectEnterableLookup()
        {
            WorldNodeAccessResolver resolver = new WorldNodeAccessResolver();

            Assert.That(
                () => resolver.TryGetEnterableNode(null, new PersistentWorldState(), new NodeId("node_001"), out _),
                Throws.ArgumentNullException.With.Property("ParamName").EqualTo("worldGraph"));
        }

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

        [Test]
        public void ShouldNotDuplicateClearedNodeWhenItIsPathEnterableAndFarmAccessible()
        {
            WorldGraph worldGraph = BootstrapWorldTestData.CreateWorldGraph();
            PersistentWorldState worldState = BootstrapWorldTestData.CreateWorldState();
            WorldNodeAccessResolver resolver = new WorldNodeAccessResolver();

            IReadOnlyList<NodeId> enterableNodeIds = resolver.GetEnterableNodes(worldGraph, worldState)
                .Select(node => node.NodeId)
                .ToArray();

            Assert.That(enterableNodeIds.Count(nodeId => nodeId == new NodeId("region_001_node_001")), Is.EqualTo(1));
        }
    }
}

