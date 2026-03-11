using System;
using UnityEngine;

namespace Survivalon.Runtime
{
    [Serializable]
    public sealed class NodeConnectionData
    {
        [SerializeField]
        private NodeDefinition targetNode;

        public NodeDefinition TargetNode => targetNode;
    }
}
