using System;
using Survivalon.Core;

namespace Survivalon.World
{
    public sealed class WorldNodeDisplayNameResolver
    {
        public string Resolve(WorldNode worldNode)
        {
            if (worldNode == null)
            {
                throw new ArgumentNullException(nameof(worldNode));
            }

            return string.IsNullOrWhiteSpace(worldNode.DisplayName)
                ? worldNode.NodeId.Value
                : worldNode.DisplayName;
        }

        public string Resolve(WorldGraph worldGraph, NodeId nodeId)
        {
            if (worldGraph == null)
            {
                throw new ArgumentNullException(nameof(worldGraph));
            }

            return Resolve(worldGraph.GetNode(nodeId));
        }

        public string Resolve(NodePlaceholderState placeholderState)
        {
            if (placeholderState == null)
            {
                throw new ArgumentNullException(nameof(placeholderState));
            }

            return string.IsNullOrWhiteSpace(placeholderState.NodeDisplayName)
                ? placeholderState.NodeId.Value
                : placeholderState.NodeDisplayName;
        }
    }
}
