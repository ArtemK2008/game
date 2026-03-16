using System.Collections.Generic;
using Survivalon.Runtime.Core;
using Survivalon.Runtime.State.Persistence;
using Survivalon.Runtime.World;
using Survivalon.Tests.EditMode.State.Persistence;

namespace Survivalon.Tests.EditMode.World
{
    public static class WorldFlowTestData
    {
        public static WorldGraph CreateFarmAccessGraph(bool includeLockedConnection = false)
        {
            RegionId regionId = new RegionId("region_001");
            WorldNode currentNode = new WorldNode(new NodeId("node_current"), regionId, NodeType.ServiceOrProgression, NodeState.Available);
            WorldNode reachableNode = new WorldNode(new NodeId("node_reachable"), regionId, NodeType.Combat, NodeState.Available);
            WorldNode clearedFarmNode = new WorldNode(new NodeId("node_cleared_farm"), regionId, NodeType.Combat, NodeState.Available);
            WorldNode unreachableAvailableNode = new WorldNode(new NodeId("node_unreachable_available"), regionId, NodeType.Combat, NodeState.Available);
            WorldNode lockedNode = new WorldNode(new NodeId("node_locked"), regionId, NodeType.BossOrGate, NodeState.Locked);

            List<WorldNodeConnection> connections = new List<WorldNodeConnection>
            {
                new WorldNodeConnection(currentNode.NodeId, reachableNode.NodeId),
            };

            if (includeLockedConnection)
            {
                connections.Add(new WorldNodeConnection(currentNode.NodeId, lockedNode.NodeId));
            }

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
                connections);
        }

        public static PersistentWorldState CreateFarmAccessWorldState()
        {
            return PersistentStateTestData.CreateWorldState(
                new NodeId("node_current"),
                new[] { new NodeId("node_current") },
                PersistentStateTestData.CreateNodeState(new NodeId("node_cleared_farm"), 3, NodeState.Cleared, 3),
                PersistentStateTestData.CreateNodeState(new NodeId("node_locked"), 3, NodeState.Locked, 0));
        }
    }
}
