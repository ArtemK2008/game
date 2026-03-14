using NUnit.Framework;
using Survivalon.Runtime;

namespace Survivalon.Tests.EditMode
{
    public sealed class WorldNodeStateResolverTests
    {
        [Test]
        public void ShouldPreferPersistentNodeStateOverGraphNodeState()
        {
            WorldGraph worldGraph = CreateGraph(NodeState.Locked);
            PersistentWorldState worldState = new PersistentWorldState();
            WorldNodeStateResolver resolver = new WorldNodeStateResolver();
            NodeId nodeId = new NodeId("node_001");

            worldState.GetOrAddNodeState(nodeId, 0, NodeState.Locked).MarkAvailable();

            NodeState resolvedState = resolver.ResolveNodeState(worldGraph, worldState, nodeId);

            Assert.That(resolvedState, Is.EqualTo(NodeState.Available));
        }

        [Test]
        public void ShouldFallbackToGraphNodeStateWhenPersistentStateIsMissing()
        {
            WorldGraph worldGraph = CreateGraph(NodeState.Cleared);
            PersistentWorldState worldState = new PersistentWorldState();
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
