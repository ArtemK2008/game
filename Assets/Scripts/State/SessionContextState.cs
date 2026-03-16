using System;
using Survivalon.Runtime.Core;
using Survivalon.Runtime.State.Persistence;

namespace Survivalon.Runtime.State
{
    public sealed class SessionContextState
    {
        private string recentNodeIdValue = string.Empty;
        private string recentPushTargetNodeIdValue = string.Empty;
        private string lastSelectedNodeIdValue = string.Empty;

        public bool HasRecentNode => !string.IsNullOrWhiteSpace(recentNodeIdValue);

        public NodeId RecentNodeId => new NodeId(recentNodeIdValue);

        public bool HasRecentPushTarget => !string.IsNullOrWhiteSpace(recentPushTargetNodeIdValue);

        public NodeId RecentPushTargetNodeId => new NodeId(recentPushTargetNodeIdValue);

        public bool HasLastSelectedNode => !string.IsNullOrWhiteSpace(lastSelectedNodeIdValue);

        public NodeId LastSelectedNodeId => new NodeId(lastSelectedNodeIdValue);

        public void SeedFromWorldState(PersistentWorldState worldState)
        {
            if (worldState == null)
            {
                throw new ArgumentNullException(nameof(worldState));
            }

            if (HasRecentNode)
            {
                return;
            }

            if (worldState.HasCurrentNode)
            {
                recentNodeIdValue = worldState.CurrentNodeId.Value;
                return;
            }

            if (worldState.HasLastSafeNode)
            {
                recentNodeIdValue = worldState.LastSafeNodeId.Value;
            }
        }

        public void RecordSelection(NodeId nodeId, bool isForwardSelectable)
        {
            lastSelectedNodeIdValue = nodeId.Value;
            if (isForwardSelectable)
            {
                recentPushTargetNodeIdValue = nodeId.Value;
            }
        }

        public void RecordNodeEntry(NodeId nodeId)
        {
            recentNodeIdValue = nodeId.Value;
        }

        public void RecordRunReturned(NodeId nodeId)
        {
            recentNodeIdValue = nodeId.Value;
        }
    }
}
