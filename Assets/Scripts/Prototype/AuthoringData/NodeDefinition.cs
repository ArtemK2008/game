using System.Collections.Generic;
using UnityEngine;
using Survivalon.Core;

namespace Survivalon.Prototype.AuthoringData
{
    /// <summary>
    /// Хранит dormant prototype-описание узла мира для неиспользуемого authoring slice.
    /// </summary>
    [CreateAssetMenu(
        fileName = "NodeDefinition",
        menuName = "Survivalon/Prototype/Authoring/Node Definition")]
    public sealed class NodeDefinition : ScriptableObject
    {
        [SerializeField]
        private string nodeIdValue = string.Empty;

        [SerializeField]
        private RegionDefinition region;

        [SerializeField]
        private NodeType nodeType = NodeType.Combat;

        [SerializeField]
        private NodeState defaultState = NodeState.Locked;

        [SerializeField]
        private List<NodeConnectionData> outboundConnections = new List<NodeConnectionData>();

        [SerializeField]
        private List<string> unlockPrerequisiteIds = new List<string>();

        public NodeId NodeId => new NodeId(nodeIdValue);

        public RegionDefinition Region => region;

        public NodeType NodeType => nodeType;

        public NodeState DefaultState => defaultState;

        public IReadOnlyList<NodeConnectionData> OutboundConnections => outboundConnections;

        public IReadOnlyList<string> UnlockPrerequisiteIds => unlockPrerequisiteIds;
    }
}

