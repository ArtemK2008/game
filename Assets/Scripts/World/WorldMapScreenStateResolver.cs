using System;
using UnityEngine;
using Survivalon.Core;

namespace Survivalon.World
{
    public static class WorldMapScreenStateResolver
    {
        public static WorldMapScreenButtonState ResolveEntryButtonState(
            bool hasNodeEntryHandler,
            bool hasSelectedNode,
            string selectedNodeDisplayName,
            bool hasQuickRepeatNode,
            string quickRepeatNodeDisplayName,
            NodeType quickRepeatNodeType)
        {
            bool canEnterSelection = hasNodeEntryHandler && hasSelectedNode;
            bool canQuickRepeatNode = hasNodeEntryHandler && !hasSelectedNode && hasQuickRepeatNode;

            return new WorldMapScreenButtonState(
                canEnterSelection
                    ? string.IsNullOrWhiteSpace(selectedNodeDisplayName)
                        ? "Enter selected node"
                        : $"Enter {selectedNodeDisplayName}"
                    : canQuickRepeatNode
                        ? BuildQuickRepeatButtonLabel(quickRepeatNodeDisplayName, quickRepeatNodeType)
                    : "Select a reachable node to enter",
                canEnterSelection || canQuickRepeatNode);
        }

        private static string BuildQuickRepeatButtonLabel(string quickRepeatNodeDisplayName, NodeType quickRepeatNodeType)
        {
            if (string.IsNullOrWhiteSpace(quickRepeatNodeDisplayName))
            {
                throw new ArgumentException(
                    "Quick-repeat node display name cannot be null or whitespace.",
                    nameof(quickRepeatNodeDisplayName));
            }

            switch (quickRepeatNodeType)
            {
                case NodeType.Combat:
                case NodeType.BossOrGate:
                    return $"Replay {quickRepeatNodeDisplayName}";
                case NodeType.ServiceOrProgression:
                    return $"Return to {quickRepeatNodeDisplayName}";
                default:
                    throw new ArgumentOutOfRangeException(
                        nameof(quickRepeatNodeType),
                        quickRepeatNodeType,
                        "Unknown quick-repeat node type.");
            }
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

