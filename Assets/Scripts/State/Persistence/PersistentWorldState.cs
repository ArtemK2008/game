using System;
using System.Collections.Generic;
using UnityEngine;

namespace Survivalon.Runtime
{
    [Serializable]
    public sealed class PersistentWorldState
    {
        [SerializeField]
        private string currentNodeIdValue = string.Empty;

        [SerializeField]
        private string lastSafeNodeIdValue = string.Empty;

        [SerializeField]
        private List<string> reachableNodeIdValues = new List<string>();

        [SerializeField]
        private List<string> unlockedRegionIdValues = new List<string>();

        [SerializeField]
        private List<PersistentNodeState> nodeStates = new List<PersistentNodeState>();

        public bool HasCurrentNode => !string.IsNullOrWhiteSpace(currentNodeIdValue);

        public NodeId CurrentNodeId => new NodeId(currentNodeIdValue);

        public bool HasLastSafeNode => !string.IsNullOrWhiteSpace(lastSafeNodeIdValue);

        public NodeId LastSafeNodeId => new NodeId(lastSafeNodeIdValue);

        public IReadOnlyList<string> ReachableNodeIdValues => reachableNodeIdValues;

        public IReadOnlyList<string> UnlockedRegionIdValues => unlockedRegionIdValues;

        public IReadOnlyList<PersistentNodeState> NodeStates => nodeStates;

        public bool TryGetNodeState(NodeId nodeId, out PersistentNodeState nodeState)
        {
            for (int index = 0; index < nodeStates.Count; index++)
            {
                if (nodeStates[index].NodeId == nodeId)
                {
                    nodeState = nodeStates[index];
                    return true;
                }
            }

            nodeState = null;
            return false;
        }

        public void SetCurrentNode(NodeId nodeId)
        {
            currentNodeIdValue = nodeId.Value;
        }

        public void SetLastSafeNode(NodeId nodeId)
        {
            lastSafeNodeIdValue = nodeId.Value;
        }

        public void ReplaceReachableNodes(IEnumerable<NodeId> nodeIds)
        {
            if (nodeIds == null)
            {
                throw new ArgumentNullException(nameof(nodeIds));
            }

            reachableNodeIdValues.Clear();
            HashSet<string> uniqueNodeIdValues = new HashSet<string>(StringComparer.Ordinal);

            foreach (NodeId nodeId in nodeIds)
            {
                if (uniqueNodeIdValues.Add(nodeId.Value))
                {
                    reachableNodeIdValues.Add(nodeId.Value);
                }
            }
        }

        public void ReplaceNodeStates(IEnumerable<PersistentNodeState> replacementNodeStates)
        {
            if (replacementNodeStates == null)
            {
                throw new ArgumentNullException(nameof(replacementNodeStates));
            }

            nodeStates.Clear();
            foreach (PersistentNodeState nodeState in replacementNodeStates)
            {
                if (nodeState == null)
                {
                    throw new ArgumentException("Replacement node states cannot contain null entries.", nameof(replacementNodeStates));
                }

                nodeStates.Add(nodeState);
            }
        }

        public PersistentNodeState GetOrAddNodeState(
            NodeId nodeId,
            int unlockThreshold,
            NodeState initialState = NodeState.Available,
            int initialProgress = 0)
        {
            if (TryGetNodeState(nodeId, out PersistentNodeState existingNodeState))
            {
                return existingNodeState;
            }

            if (unlockThreshold < 0)
            {
                throw new ArgumentOutOfRangeException(nameof(unlockThreshold), "Unlock threshold cannot be negative.");
            }

            if (initialProgress < 0 || initialProgress > unlockThreshold)
            {
                throw new ArgumentOutOfRangeException(nameof(initialProgress), "Initial progress must be between zero and the unlock threshold.");
            }

            NodeState normalizedState = initialState == NodeState.Locked
                ? NodeState.Locked
                : NodeState.Available;
            PersistentNodeState nodeState = new PersistentNodeState(nodeId, unlockThreshold, normalizedState);

            if (normalizedState != NodeState.Locked && initialProgress > 0)
            {
                nodeState.ApplyUnlockProgress(initialProgress);
            }

            if (initialState == NodeState.Mastered)
            {
                nodeState.MarkMastered();
            }

            nodeStates.Add(nodeState);
            return nodeState;
        }
    }
}
