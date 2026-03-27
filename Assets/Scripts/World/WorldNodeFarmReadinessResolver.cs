using System;
using Survivalon.Core;
using Survivalon.State.Persistence;

namespace Survivalon.World
{
    public sealed class WorldNodeFarmReadinessResolver
    {
        private readonly WorldNodeStateResolver worldNodeStateResolver;

        public WorldNodeFarmReadinessResolver(WorldNodeStateResolver worldNodeStateResolver = null)
        {
            this.worldNodeStateResolver = worldNodeStateResolver ?? new WorldNodeStateResolver();
        }

        public bool IsFarmReady(WorldGraph worldGraph, PersistentWorldState worldState, NodeId nodeId)
        {
            if (worldGraph == null)
            {
                throw new ArgumentNullException(nameof(worldGraph));
            }

            if (worldState == null)
            {
                throw new ArgumentNullException(nameof(worldState));
            }

            WorldNode node = worldGraph.GetNode(nodeId);
            if (node.NodeType != NodeType.Combat)
            {
                return false;
            }

            NodeState nodeState = worldNodeStateResolver.ResolveNodeState(worldGraph, worldState, nodeId);
            return nodeState == NodeState.Cleared || nodeState == NodeState.Mastered;
        }
    }
}
