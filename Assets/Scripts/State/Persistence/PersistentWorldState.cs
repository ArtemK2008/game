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

        public void SetCurrentNode(NodeId nodeId)
        {
            currentNodeIdValue = nodeId.Value;
        }

        public void SetLastSafeNode(NodeId nodeId)
        {
            lastSafeNodeIdValue = nodeId.Value;
        }
    }
}
