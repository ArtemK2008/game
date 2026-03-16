using System;
using System.Collections.Generic;
using NUnit.Framework;
using Survivalon.Runtime;
using Survivalon.Runtime.Combat;
using Survivalon.Runtime.Core;
using Survivalon.Runtime.Run;
using Survivalon.Runtime.State;
using Survivalon.Runtime.State.Persistence;
using Survivalon.Runtime.World;
using Survivalon.Tests.EditMode.State.Persistence;

namespace Survivalon.Tests.EditMode.World
{
    public sealed class WorldGraphValidationTests
    {
        [Test]
        public void ShouldThrowWhenDuplicateRegionIdsAreProvided()
        {
            WorldNode node = CreateNode("region_001_node_001", "region_001", NodeState.Available);
            WorldRegion firstRegion = CreateRegion("region_001", 0, node.NodeId);
            WorldRegion duplicateRegion = CreateRegion("region_001", 1, node.NodeId);

            ArgumentException exception = Assert.Throws<ArgumentException>(
                () => new WorldGraph(
                    new[] { firstRegion, duplicateRegion },
                    new[] { node },
                    Array.Empty<WorldNodeConnection>()));

            Assert.That(exception.Message, Does.Contain("Duplicate region id"));
        }

        [Test]
        public void ShouldThrowWhenDuplicateNodeIdsAreProvided()
        {
            WorldNode firstNode = CreateNode("region_001_node_001", "region_001", NodeState.Available);
            WorldNode duplicateNode = CreateNode("region_001_node_001", "region_001", NodeState.Cleared);
            WorldRegion region = CreateRegion("region_001", 0, firstNode.NodeId);

            ArgumentException exception = Assert.Throws<ArgumentException>(
                () => new WorldGraph(
                    new[] { region },
                    new[] { firstNode, duplicateNode },
                    Array.Empty<WorldNodeConnection>()));

            Assert.That(exception.Message, Does.Contain("Duplicate node id"));
        }

        [Test]
        public void ShouldThrowWhenConnectionSourceNodeIsMissing()
        {
            WorldNode targetNode = CreateNode("region_001_node_001", "region_001", NodeState.Available);
            WorldRegion region = CreateRegion("region_001", 0, targetNode.NodeId);

            ArgumentException exception = Assert.Throws<ArgumentException>(
                () => new WorldGraph(
                    new[] { region },
                    new[] { targetNode },
                    new[]
                    {
                        new WorldNodeConnection(new NodeId("region_001_node_999"), targetNode.NodeId),
                    }));

            Assert.That(exception.Message, Does.Contain("Connection source node"));
        }

        [Test]
        public void ShouldThrowWhenConnectionTargetNodeIsMissing()
        {
            WorldNode sourceNode = CreateNode("region_001_node_001", "region_001", NodeState.Available);
            WorldRegion region = CreateRegion("region_001", 0, sourceNode.NodeId);

            ArgumentException exception = Assert.Throws<ArgumentException>(
                () => new WorldGraph(
                    new[] { region },
                    new[] { sourceNode },
                    new[]
                    {
                        new WorldNodeConnection(sourceNode.NodeId, new NodeId("region_001_node_999")),
                    }));

            Assert.That(exception.Message, Does.Contain("Connection target node"));
        }

        [Test]
        public void ShouldThrowWhenRegionReferencesNodeFromDifferentRegion()
        {
            WorldNode foreignNode = CreateNode("region_002_node_001", "region_002", NodeState.Available);
            WorldRegion incorrectRegion = CreateRegion(
                "region_001",
                0,
                foreignNode.NodeId,
                foreignNode.NodeId);
            WorldRegion foreignRegion = CreateRegion("region_002", 1, foreignNode.NodeId);

            ArgumentException exception = Assert.Throws<ArgumentException>(
                () => new WorldGraph(
                    new[] { incorrectRegion, foreignRegion },
                    new[] { foreignNode },
                    Array.Empty<WorldNodeConnection>()));

            Assert.That(exception.Message, Does.Contain("belongs to region"));
        }

        [Test]
        public void ShouldThrowWhenNodeIsNotListedInItsRegion()
        {
            WorldNode node = CreateNode("region_001_node_001", "region_001", NodeState.Available);
            WorldNode listedNode = CreateNode("region_001_node_002", "region_001", NodeState.Available);
            WorldRegion region = CreateRegion(
                "region_001",
                0,
                listedNode.NodeId,
                listedNode.NodeId);

            ArgumentException exception = Assert.Throws<ArgumentException>(
                () => new WorldGraph(
                    new[] { region },
                    new[] { node, listedNode },
                    Array.Empty<WorldNodeConnection>()));

            Assert.That(exception.Message, Does.Contain("is not listed in region"));
        }

        [Test]
        public void ShouldThrowWhenNodeReferencesMissingRegion()
        {
            WorldNode missingRegionNode = CreateNode("region_002_node_001", "region_002", NodeState.Available);
            WorldNode listedNode = CreateNode("region_001_node_001", "region_001", NodeState.Available);
            WorldRegion existingRegion = CreateRegion(
                "region_001",
                0,
                listedNode.NodeId,
                listedNode.NodeId);

            ArgumentException exception = Assert.Throws<ArgumentException>(
                () => new WorldGraph(
                    new[] { existingRegion },
                    new[] { listedNode, missingRegionNode },
                    Array.Empty<WorldNodeConnection>()));

            Assert.That(exception.Message, Does.Contain("references missing region"));
        }

        private static WorldNode CreateNode(string nodeIdValue, string regionIdValue, NodeState state)
        {
            return new WorldNode(
                new NodeId(nodeIdValue),
                new RegionId(regionIdValue),
                NodeType.Combat,
                state);
        }

        private static WorldRegion CreateRegion(
            string regionIdValue,
            int progressionOrder,
            NodeId entryNodeId,
            params NodeId[] nodeIds)
        {
            IReadOnlyList<NodeId> effectiveNodeIds = nodeIds.Length == 0
                ? new[] { entryNodeId }
                : nodeIds;

            return new WorldRegion(
                new RegionId(regionIdValue),
                progressionOrder,
                entryNodeId,
                effectiveNodeIds,
                ResourceCategory.RegionMaterial,
                $"band_{progressionOrder}");
        }
    }
}
