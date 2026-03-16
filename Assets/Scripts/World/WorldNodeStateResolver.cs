using System;
using Survivalon.Runtime.Combat;
using Survivalon.Runtime.Core;
using Survivalon.Runtime.Run;
using Survivalon.Runtime.State;
using Survivalon.Runtime.State.Persistence;

namespace Survivalon.Runtime.World
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
