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
                return new Color(0.10f, 0.62f, 0.86f, 1f);
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

        public static Color ResolveNodeIconTint(WorldMapNodeOption nodeOption)
        {
            if (nodeOption == null)
            {
                throw new ArgumentNullException(nameof(nodeOption));
            }

            if (nodeOption.NodeState == NodeState.Locked)
            {
                return new Color(0.42f, 0.44f, 0.48f, 0.52f);
            }

            if (IsReplayableNode(nodeOption))
            {
                return new Color(0.92f, 0.94f, 0.95f, 0.90f);
            }

            if (nodeOption.IsSelected || nodeOption.IsCurrentContext || nodeOption.IsSelectable)
            {
                return Color.white;
            }

            return new Color(0.84f, 0.87f, 0.90f, 0.78f);
        }

        public static bool TryResolveNodeStateMarkerStyle(
            WorldMapNodeOption nodeOption,
            out WorldMapNodeStateMarkerStyle markerStyle)
        {
            if (nodeOption == null)
            {
                throw new ArgumentNullException(nameof(nodeOption));
            }

            if (nodeOption.IsSelected)
            {
                markerStyle = new WorldMapNodeStateMarkerStyle(
                    new Color(0.92f, 0.74f, 0.24f, 0.96f),
                    size: 110f);
                return true;
            }

            if (nodeOption.IsCurrentContext)
            {
                markerStyle = new WorldMapNodeStateMarkerStyle(
                    new Color(0.18f, 0.82f, 1f, 0.90f),
                    size: 96f);
                return true;
            }

            if (nodeOption.NodeState == NodeState.Locked)
            {
                markerStyle = default;
                return false;
            }

            if (IsReplayableNode(nodeOption))
            {
                markerStyle = new WorldMapNodeStateMarkerStyle(
                    new Color(0.34f, 0.72f, 0.80f, 0.62f),
                    size: 78f);
                return true;
            }

            if (nodeOption.IsSelectable)
            {
                markerStyle = new WorldMapNodeStateMarkerStyle(
                    new Color(0.48f, 0.90f, 0.40f, 0.76f),
                    size: 84f);
                return true;
            }

            markerStyle = default;
            return false;
        }

        private static bool IsReplayableNode(WorldMapNodeOption nodeOption)
        {
            return nodeOption.NodeState == NodeState.Cleared ||
                nodeOption.IsFarmReady ||
                nodeOption.PathRole == WorldMapPathRole.ReplayableFarmNode;
        }
    }

    public readonly struct WorldMapNodeStateMarkerStyle
    {
        public WorldMapNodeStateMarkerStyle(Color color, float size)
        {
            Color = color;
            Size = size;
        }

        public Color Color { get; }

        public float Size { get; }
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

