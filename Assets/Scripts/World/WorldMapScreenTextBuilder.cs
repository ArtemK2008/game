using System;
using System.Collections.Generic;
using Survivalon.Core;
using Survivalon.State;

namespace Survivalon.World
{
    public static class WorldMapScreenTextBuilder
    {
        public static string BuildSummaryText(
            IReadOnlyList<WorldMapNodeOption> nodeOptions,
            bool hasSelectedNode,
            NodeId selectedNodeId,
            SessionContextState sessionContext,
            bool hasForwardRouteChoice,
            int forwardSelectableNodeCount)
        {
            if (nodeOptions == null)
            {
                throw new ArgumentNullException(nameof(nodeOptions));
            }

            string currentNodeLabel = "unknown";
            string selectedNodeLabel = hasSelectedNode ? selectedNodeId.Value : "none";
            string recentNodeLabel = GetSessionNodeLabel(
                sessionContext,
                context => context.HasRecentNode,
                context => context.RecentNodeId);
            string recentPushTargetLabel = GetSessionNodeLabel(
                sessionContext,
                context => context.HasRecentPushTarget,
                context => context.RecentPushTargetNodeId);
            string lastSelectedNodeLabel = GetSessionNodeLabel(
                sessionContext,
                context => context.HasLastSelectedNode,
                context => context.LastSelectedNodeId);
            int selectableCount = 0;

            foreach (WorldMapNodeOption nodeOption in nodeOptions)
            {
                if (nodeOption.IsCurrentContext)
                {
                    currentNodeLabel = nodeOption.NodeId.Value;
                }

                if (nodeOption.IsSelectable)
                {
                    selectableCount++;
                }
            }

            string routeChoiceLabel = hasForwardRouteChoice
                ? "Branch choice available"
                : "Single forward route";

            return
                $"Current node: {currentNodeLabel}\n" +
                $"Recent node: {recentNodeLabel}\n" +
                $"Recent push target: {recentPushTargetLabel}\n" +
                $"Last selected node: {lastSelectedNodeLabel}\n" +
                $"Selectable destinations: {selectableCount}\n" +
                $"Forward route options: {forwardSelectableNodeCount} ({routeChoiceLabel})\n" +
                $"Selected node: {selectedNodeLabel}\n" +
                "Select a reachable node, then confirm entry to start the placeholder node flow.";
        }

        public static string BuildNodeLabel(WorldMapNodeOption nodeOption)
        {
            if (nodeOption == null)
            {
                throw new ArgumentNullException(nameof(nodeOption));
            }

            return
                $"{nodeOption.RegionId.Value} / {nodeOption.NodeId.Value}\n" +
                $"Type: {nodeOption.NodeType} | State: {nodeOption.NodeState} | {BuildAvailabilityLabel(nodeOption)}";
        }

        private static string BuildAvailabilityLabel(WorldMapNodeOption nodeOption)
        {
            if (nodeOption.IsSelected)
            {
                return "Selected";
            }

            if (nodeOption.IsCurrentContext)
            {
                return "Current";
            }

            if (nodeOption.IsSelectable)
            {
                return "Selectable";
            }

            if (nodeOption.NodeState == NodeState.Locked)
            {
                return "Locked";
            }

            return "Known";
        }

        private static string GetSessionNodeLabel(
            SessionContextState sessionContext,
            Func<SessionContextState, bool> hasValue,
            Func<SessionContextState, NodeId> selector)
        {
            if (sessionContext == null || !hasValue(sessionContext))
            {
                return "none";
            }

            return selector(sessionContext).Value;
        }
    }
}

