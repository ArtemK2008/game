using System;
using UnityEngine;
using Survivalon.Runtime.Core;

namespace Survivalon.Runtime.World
{
    public static class WorldMapScreenStateResolver
    {
        public static WorldMapScreenButtonState ResolveEntryButtonState(
            bool hasNodeEntryHandler,
            bool hasSelectedNode,
            NodeId selectedNodeId)
        {
            bool canEnterSelection = hasNodeEntryHandler && hasSelectedNode;

            return new WorldMapScreenButtonState(
                canEnterSelection
                    ? $"Enter {selectedNodeId.Value}"
                    : "Select a reachable node to enter",
                canEnterSelection);
        }

        public static Color ResolveNodeColor(WorldMapNodeOption nodeOption)
        {
            if (nodeOption == null)
            {
                throw new ArgumentNullException(nameof(nodeOption));
            }

            if (nodeOption.IsSelected)
            {
                return new Color(0.77f, 0.62f, 0.20f, 1f);
            }

            if (nodeOption.IsCurrentContext)
            {
                return new Color(0.18f, 0.39f, 0.70f, 1f);
            }

            if (nodeOption.IsSelectable)
            {
                return new Color(0.18f, 0.50f, 0.24f, 1f);
            }

            if (nodeOption.NodeState == NodeState.Locked)
            {
                return new Color(0.24f, 0.24f, 0.27f, 1f);
            }

            return new Color(0.34f, 0.34f, 0.38f, 1f);
        }
    }

    public readonly struct WorldMapScreenButtonState
    {
        public WorldMapScreenButtonState(string label, bool isInteractable)
        {
            if (string.IsNullOrWhiteSpace(label))
            {
                throw new ArgumentException("Button label cannot be null or whitespace.", nameof(label));
            }

            Label = label;
            IsInteractable = isInteractable;
        }

        public string Label { get; }

        public bool IsInteractable { get; }
    }
}
