using System.Collections.Generic;
using Survivalon.Runtime;
using Survivalon.Runtime.Core;
using Survivalon.Runtime.Data.Characters;
using Survivalon.Runtime.Data.Combat;
using Survivalon.Runtime.Data.Gear;
using Survivalon.Runtime.State.Persistence;
using Survivalon.Runtime.World;
using Survivalon.Tests.EditMode.World;

namespace Survivalon.Tests.EditMode.State.Persistence
{
    public static class PersistentStateTestData
    {
        public static PersistentNodeState CreateNodeState(
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

        public static PersistentWorldState CreateWorldState(
            NodeId currentNodeId,
            IEnumerable<NodeId> reachableNodeIds,
            params PersistentNodeState[] nodeStates)
        {
            PersistentWorldState worldState = new PersistentWorldState();
            worldState.SetCurrentNode(currentNodeId);
            worldState.ReplaceReachableNodes(reachableNodeIds);

            if (nodeStates.Length > 0)
            {
                worldState.ReplaceNodeStates(nodeStates);
            }

            return worldState;
        }
    }
}
