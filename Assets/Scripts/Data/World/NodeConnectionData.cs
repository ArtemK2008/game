using System;
using UnityEngine;

namespace Survivalon.Data.World
{
    [Serializable]
    public sealed class NodeConnectionData
    {
        [SerializeField]
        private NodeDefinition targetNode;

        public NodeDefinition TargetNode => targetNode;
    }
}

