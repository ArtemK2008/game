using System;
using UnityEngine;

namespace Survivalon.Runtime.Data.World
{
    [Serializable]
    public sealed class NodeConnectionData
    {
        [SerializeField]
        private NodeDefinition targetNode;

        public NodeDefinition TargetNode => targetNode;
    }
}
