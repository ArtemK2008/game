using NUnit.Framework;
using Survivalon.Core;
using Survivalon.State.Persistence;
using Survivalon.World;
using Survivalon.Tests.EditMode.State.Persistence;

namespace Survivalon.Tests.EditMode.World
{
    public sealed class WorldNodeStateResolverTests
    {
        [Test]
        public void ShouldPreferPersistentNodeStateOverGraphNodeState()
        {
            WorldGraph worldGraph = CreateGraph(NodeState.Locked);
            WorldNodeStateResolver resolver = new WorldNodeStateResolver();
            NodeId nodeId = new NodeId("node_001");
            PersistentWorldState worldState = PersistentStateTestData.CreateWorldState(
                nodeId,
                new NodeId[0],
                PersistentStateTestData.CreateNodeState(nodeId, 0, NodeState.Available, 0));

            NodeState resolvedState = resolver.ResolveNodeState(worldGraph, worldState, nodeId);

            Assert.That(resolvedState, Is.EqualTo(NodeState.Available));
        }

        [Test]
        public void ShouldFallbackToGraphNodeStateWhenPersistentStateIsMissing()
        {
            WorldGraph worldGraph = CreateGraph(NodeState.Cleared);
            PersistentWorldState worldState = PersistentStateTestData.CreateWorldState(
                new NodeId("node_001"),
                new NodeId[0]);
            WorldNodeStateResolver resolver = new WorldNodeStateResolver();

            NodeState resolvedState = resolver.ResolveNodeState(worldGraph, worldState, new NodeId("node_001"));

            Assert.That(resolvedState, Is.EqualTo(NodeState.Cleared));
        }

        private static WorldGraph CreateGraph(NodeState nodeState)
        {
            RegionId regionId = new RegionId("region_001");
            WorldNode node = new WorldNode(new NodeId("node_001"), regionId, NodeType.Combat, nodeState);

            return new WorldGraph(
                new[]
                {
                    new WorldRegion(
                        regionId,
                        0,
                        node.NodeId,
                        new[] { node.NodeId },
                        ResourceCategory.RegionMaterial,
                        "resolver_test"),
                },
                new[] { node },
                new WorldNodeConnection[0]);
        }
    }
}

