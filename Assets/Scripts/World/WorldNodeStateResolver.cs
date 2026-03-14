using System;

namespace Survivalon.Runtime
{
    public sealed class WorldNodeStateResolver
    {
        public NodeState ResolveNodeState(WorldGraph worldGraph, PersistentWorldState worldState, NodeId nodeId)
        {
            if (worldGraph == null)
            {
                throw new ArgumentNullException(nameof(worldGraph));
            }

            if (worldState == null)
            {
                throw new ArgumentNullException(nameof(worldState));
            }

            return worldState.TryGetNodeState(nodeId, out PersistentNodeState nodeState)
                ? nodeState.State
                : worldGraph.GetNode(nodeId).State;
        }
    }
}
