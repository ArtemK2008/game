using System;
using System.Collections.Generic;
using Survivalon.Core;
using Survivalon.Data.Characters;
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

        public static string BuildCharacterSelectionText(IReadOnlyList<PlayableCharacterSelectionOption> selectionOptions)
        {
            if (selectionOptions == null)
            {
                throw new ArgumentNullException(nameof(selectionOptions));
            }

            string selectedCharacterLabel = "none";
            for (int index = 0; index < selectionOptions.Count; index++)
            {
                if (selectionOptions[index].IsSelected)
                {
                    selectedCharacterLabel = selectionOptions[index].DisplayName;
                    break;
                }
            }

            return
                $"Selected character: {selectedCharacterLabel}\n" +
                $"Available characters: {selectionOptions.Count}";
        }

        public static string BuildSkillPackageAssignmentText(
            string selectedCharacterDisplayName,
            IReadOnlyList<PlayableCharacterSkillPackageOption> skillPackageOptions)
        {
            if (selectedCharacterDisplayName == null)
            {
                throw new ArgumentNullException(nameof(selectedCharacterDisplayName));
            }

            if (skillPackageOptions == null)
            {
                throw new ArgumentNullException(nameof(skillPackageOptions));
            }

            string assignedPackageLabel = "none";
            string assignedPackageSummary = "No valid package assigned.";
            for (int index = 0; index < skillPackageOptions.Count; index++)
            {
                if (skillPackageOptions[index].IsAssigned)
                {
                    assignedPackageLabel = skillPackageOptions[index].DisplayName;
                    assignedPackageSummary = skillPackageOptions[index].Summary;
                    break;
                }
            }

            return
                $"Selected character package: {selectedCharacterDisplayName}\n" +
                $"Assigned package: {assignedPackageLabel}\n" +
                $"Package effect: {assignedPackageSummary}\n" +
                $"Available packages: {skillPackageOptions.Count}";
        }

        public static string BuildCharacterButtonLabel(PlayableCharacterSelectionOption selectionOption)
        {
            if (selectionOption == null)
            {
                throw new ArgumentNullException(nameof(selectionOption));
            }

            return selectionOption.IsSelected
                ? $"Selected: {selectionOption.DisplayName}"
                : $"Select: {selectionOption.DisplayName}";
        }

        public static string BuildSkillPackageButtonLabel(PlayableCharacterSkillPackageOption skillPackageOption)
        {
            if (skillPackageOption == null)
            {
                throw new ArgumentNullException(nameof(skillPackageOption));
            }

            return skillPackageOption.IsAssigned
                ? $"Assigned: {skillPackageOption.DisplayName}"
                : $"Assign: {skillPackageOption.DisplayName}";
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

