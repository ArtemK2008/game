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
            WorldGraph worldGraph = CreateFarmAccessGraph();
            PersistentWorldState worldState = CreateFarmAccessWorldState();
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
            WorldGraph worldGraph = CreateFarmAccessGraph();
            PersistentWorldState worldState = CreateFarmAccessWorldState();
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
            WorldGraph worldGraph = CreateFarmAccessGraph();
            PersistentWorldState worldState = CreateFarmAccessWorldState();
            WorldNodeAccessResolver resolver = new WorldNodeAccessResolver();

            bool foundLockedNode = resolver.TryGetEnterableNode(worldGraph, worldState, new NodeId("node_locked"), out WorldNode lockedNode);

            Assert.That(foundLockedNode, Is.False);
            Assert.That(lockedNode, Is.Null);
        }

        [Test]
        public void ShouldReturnOnlyForwardNodesThatAreActuallyEnterable()
        {
            WorldGraph worldGraph = CreateFarmAccessGraph();
            PersistentWorldState worldState = CreateFarmAccessWorldState();
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

        private static WorldGraph CreateFarmAccessGraph()
        {
            RegionId regionId = new RegionId("region_001");
            WorldNode currentNode = new WorldNode(new NodeId("node_current"), regionId, NodeType.ServiceOrProgression, NodeState.Available);
            WorldNode reachableNode = new WorldNode(new NodeId("node_reachable"), regionId, NodeType.Combat, NodeState.Available);
            WorldNode clearedFarmNode = new WorldNode(new NodeId("node_cleared_farm"), regionId, NodeType.Combat, NodeState.Available);
            WorldNode unreachableAvailableNode = new WorldNode(new NodeId("node_unreachable_available"), regionId, NodeType.Combat, NodeState.Available);
            WorldNode lockedNode = new WorldNode(new NodeId("node_locked"), regionId, NodeType.BossOrGate, NodeState.Locked);

            return new WorldGraph(
                new[]
                {
                    new WorldRegion(
                        regionId,
                        0,
                        currentNode.NodeId,
                        new[]
                        {
                            currentNode.NodeId,
                            reachableNode.NodeId,
                            clearedFarmNode.NodeId,
                            unreachableAvailableNode.NodeId,
                            lockedNode.NodeId,
                        },
                        ResourceCategory.RegionMaterial,
                        "farm_access"),
                },
                new[]
                {
                    currentNode,
                    reachableNode,
                    clearedFarmNode,
                    unreachableAvailableNode,
                    lockedNode,
                },
                new[]
                {
                    new WorldNodeConnection(currentNode.NodeId, reachableNode.NodeId),
                    new WorldNodeConnection(currentNode.NodeId, lockedNode.NodeId),
                });
        }

        private static PersistentWorldState CreateFarmAccessWorldState()
        {
            PersistentWorldState worldState = new PersistentWorldState();
            worldState.SetCurrentNode(new NodeId("node_current"));
            worldState.ReplaceReachableNodes(new[] { new NodeId("node_current") });
            worldState.ReplaceNodeStates(new[]
            {
                CreateNodeState(new NodeId("node_cleared_farm"), 3, NodeState.Cleared, 3),
                CreateNodeState(new NodeId("node_locked"), 3, NodeState.Locked, 0),
            });
            return worldState;
        }

        private static PersistentNodeState CreateNodeState(
            NodeId nodeId,
            int unlockThreshold,
            NodeState nodeState,
            int unlockProgress)
        {
            NodeState initialState = nodeState == NodeState.Locked
                ? NodeState.Locked
                : NodeState.Available;
            PersistentNodeState persistentNodeState = new PersistentNodeState(nodeId, unlockThreshold, initialState);

            if (initialState != NodeState.Locked && unlockProgress > 0)
            {
                persistentNodeState.ApplyUnlockProgress(unlockProgress);
            }

            return persistentNodeState;
        }
    }
}
