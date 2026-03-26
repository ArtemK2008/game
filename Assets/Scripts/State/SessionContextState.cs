using System;
using Survivalon.Core;
using Survivalon.State.Persistence;

namespace Survivalon.State
{
    public sealed class SessionContextState
    {
        private string recentNodeIdValue = string.Empty;
        private string recentPushTargetNodeIdValue = string.Empty;
        private string lastSelectedNodeIdValue = string.Empty;
        private string returnToWorldReentryNodeIdValue = string.Empty;

        public bool HasRecentNode => !string.IsNullOrWhiteSpace(recentNodeIdValue);

        public NodeId RecentNodeId => new NodeId(recentNodeIdValue);

        public bool HasRecentPushTarget => !string.IsNullOrWhiteSpace(recentPushTargetNodeIdValue);

        public NodeId RecentPushTargetNodeId => new NodeId(recentPushTargetNodeIdValue);

        public bool HasLastSelectedNode => !string.IsNullOrWhiteSpace(lastSelectedNodeIdValue);

        public NodeId LastSelectedNodeId => new NodeId(lastSelectedNodeIdValue);

        public bool HasReturnToWorldReentryOffer => !string.IsNullOrWhiteSpace(returnToWorldReentryNodeIdValue);

        public NodeId ReturnToWorldReentryNodeId => new NodeId(returnToWorldReentryNodeIdValue);

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
            ConsumeReturnToWorldReentryOffer();
        }

        public void RecordReturnedToWorldContext(NodeId nodeId)
        {
            recentNodeIdValue = nodeId.Value;
        }

        public void OfferReturnToWorldReentry(NodeId nodeId)
        {
            recentNodeIdValue = nodeId.Value;
            returnToWorldReentryNodeIdValue = nodeId.Value;
        }

        public void ConsumeReturnToWorldReentryOffer()
        {
            returnToWorldReentryNodeIdValue = string.Empty;
        }
    }
}

