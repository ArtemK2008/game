using System;

namespace Survivalon.Runtime
{
    public sealed class WorldNodeConnection
    {
        public WorldNodeConnection(NodeId sourceNodeId, NodeId targetNodeId)
        {
            if (sourceNodeId == targetNodeId)
            {
                throw new ArgumentException("World node connections must link two distinct nodes.");
            }

            SourceNodeId = sourceNodeId;
            TargetNodeId = targetNodeId;
        }

        public NodeId SourceNodeId { get; }

        public NodeId TargetNodeId { get; }
    }
}
