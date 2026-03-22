using System;
using UnityEngine;

namespace Survivalon.Prototype.AuthoringData
{
    /// <summary>
    /// Хранит dormant prototype-связь узлов для неиспользуемого authoring slice.
    /// </summary>
    [Serializable]
    public sealed class NodeConnectionData
    {
        [SerializeField]
        private NodeDefinition targetNode;

        public NodeDefinition TargetNode => targetNode;
    }
}

