using System;
using System.Collections.Generic;
using Survivalon.Core;
using Survivalon.Data.Characters;
using Survivalon.Data.Gear;

namespace Survivalon.World
{
    public static class WorldMapScreenTextBuilder
    {
        public static string BuildSummaryText(
            WorldMapWorldStateSummary worldStateSummary,
            string selectedNodeDisplayName)
        {
            if (worldStateSummary == null)
            {
                throw new ArgumentNullException(nameof(worldStateSummary));
            }

            string selectedNodeLabel = string.IsNullOrWhiteSpace(selectedNodeDisplayName)
                ? "none"
                : selectedNodeDisplayName;

            return
                $"Location: {worldStateSummary.CurrentLocationDisplayName}\n" +
                $"Current: {worldStateSummary.CurrentNode.DisplayName} ({BuildNodeStateDisplayName(worldStateSummary.CurrentNodeState)}) | Selected: {selectedNodeLabel}\n" +
                $"Paths now: {worldStateSummary.SelectableDestinationCount} enterable | {worldStateSummary.ForwardRouteNodes.Count} forward | {worldStateSummary.BacktrackRouteNodes.Count} backtrack | {worldStateSummary.ReplayableFarmNodes.Count} replayable | {worldStateSummary.BlockedLinkedNodes.Count} blocked\n" +
                $"Forward: {BuildNodeListLabel(worldStateSummary.ForwardRouteNodes)}\n" +
                $"Backtrack: {BuildNodeListLabel(worldStateSummary.BacktrackRouteNodes)} | Replayable: {BuildNodeListLabel(worldStateSummary.ReplayableFarmNodes)}\n" +
                $"Blocked: {BuildNodeListLabel(worldStateSummary.BlockedLinkedNodes)}\n" +
                "Node states: Available = enterable | InProgress = started | Cleared = replayable | Locked = blocked";
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

        public static string BuildAssignmentText(
            string selectedCharacterDisplayName,
            IReadOnlyList<PlayableCharacterSkillPackageOption> skillPackageOptions,
            IReadOnlyList<PlayableCharacterGearAssignmentOption> gearAssignmentOptions)
        {
            if (selectedCharacterDisplayName == null)
            {
                throw new ArgumentNullException(nameof(selectedCharacterDisplayName));
            }

            if (skillPackageOptions == null)
            {
                throw new ArgumentNullException(nameof(skillPackageOptions));
            }

            if (gearAssignmentOptions == null)
            {
                throw new ArgumentNullException(nameof(gearAssignmentOptions));
            }

            string assignedPackageLabel = "none";
            for (int index = 0; index < skillPackageOptions.Count; index++)
            {
                if (skillPackageOptions[index].IsAssigned)
                {
                    assignedPackageLabel = skillPackageOptions[index].DisplayName;
                    break;
                }
            }

            string primaryGearLabel = ResolveEquippedGearDisplayName(
                gearAssignmentOptions,
                GearCategory.PrimaryCombat);
            string supportGearLabel = ResolveEquippedGearDisplayName(
                gearAssignmentOptions,
                GearCategory.SecondarySupport);

            return
                $"Selected character build: {selectedCharacterDisplayName}\n" +
                $"Assigned package: {assignedPackageLabel}\n" +
                $"Primary gear: {primaryGearLabel} | Support gear: {supportGearLabel}\n" +
                $"Available packages: {skillPackageOptions.Count} | Owned primary gear: {CountOptionsForCategory(gearAssignmentOptions, GearCategory.PrimaryCombat)} | Owned support gear: {CountOptionsForCategory(gearAssignmentOptions, GearCategory.SecondarySupport)}";
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

        public static string BuildGearAssignmentButtonLabel(PlayableCharacterGearAssignmentOption gearAssignmentOption)
        {
            if (gearAssignmentOption == null)
            {
                throw new ArgumentNullException(nameof(gearAssignmentOption));
            }

            return gearAssignmentOption.IsEquipped
                ? $"Unequip: {gearAssignmentOption.DisplayName}"
                : $"Equip: {gearAssignmentOption.DisplayName}";
        }

        public static string BuildNodeLabel(WorldMapNodeOption nodeOption)
        {
            if (nodeOption == null)
            {
                throw new ArgumentNullException(nameof(nodeOption));
            }

            return
                $"{nodeOption.NodeDisplayName}\n" +
                $"{nodeOption.LocationDisplayName}\n" +
                $"Path: {BuildPathRoleLabel(nodeOption.PathRole)} | Type: {BuildNodeTypeDisplayName(nodeOption.NodeType)} | State: {BuildNodeStateDisplayName(nodeOption.NodeState)}\n" +
                $"Status: {BuildAvailabilityLabel(nodeOption)}";
        }

        private static string BuildPathRoleLabel(WorldMapPathRole pathRole)
        {
            switch (pathRole)
            {
                case WorldMapPathRole.CurrentContext:
                    return "Current anchor";
                case WorldMapPathRole.ForwardRoute:
                    return "Forward route";
                case WorldMapPathRole.BacktrackRoute:
                    return "Backtrack route";
                case WorldMapPathRole.ReplayableFarmNode:
                    return "Replayable farm node";
                case WorldMapPathRole.BlockedPath:
                    return "Blocked path";
                default:
                    throw new ArgumentOutOfRangeException(nameof(pathRole), pathRole, "Unknown world map path role.");
            }
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

        private static string BuildNodeTypeDisplayName(NodeType nodeType)
        {
            switch (nodeType)
            {
                case NodeType.Combat:
                    return "Combat";
                case NodeType.BossOrGate:
                    return "Boss gate";
                case NodeType.ServiceOrProgression:
                    return "Service hub";
                default:
                    throw new ArgumentOutOfRangeException(nameof(nodeType), nodeType, "Unknown node type.");
            }
        }

        private static string BuildNodeStateDisplayName(NodeState nodeState)
        {
            switch (nodeState)
            {
                case NodeState.Available:
                    return "Available";
                case NodeState.InProgress:
                    return "In progress";
                case NodeState.Cleared:
                    return "Cleared";
                case NodeState.Locked:
                    return "Locked";
                default:
                    throw new ArgumentOutOfRangeException(nameof(nodeState), nodeState, "Unknown node state.");
            }
        }

        private static int CountOptionsForCategory(
            IReadOnlyList<PlayableCharacterGearAssignmentOption> gearAssignmentOptions,
            GearCategory gearCategory)
        {
            int count = 0;
            for (int index = 0; index < gearAssignmentOptions.Count; index++)
            {
                if (gearAssignmentOptions[index].GearCategory == gearCategory)
                {
                    count++;
                }
            }

            return count;
        }

        private static string ResolveEquippedGearDisplayName(
            IReadOnlyList<PlayableCharacterGearAssignmentOption> gearAssignmentOptions,
            GearCategory gearCategory)
        {
            for (int index = 0; index < gearAssignmentOptions.Count; index++)
            {
                PlayableCharacterGearAssignmentOption gearAssignmentOption = gearAssignmentOptions[index];
                if (gearAssignmentOption.GearCategory == gearCategory && gearAssignmentOption.IsEquipped)
                {
                    return gearAssignmentOption.DisplayName;
                }
            }

            return "none";
        }

        private static string BuildNodeListLabel(IReadOnlyList<WorldMapNodeReferenceDisplayState> nodes)
        {
            if (nodes == null)
            {
                throw new ArgumentNullException(nameof(nodes));
            }

            if (nodes.Count == 0)
            {
                return "none";
            }

            string[] labels = new string[nodes.Count];
            for (int index = 0; index < nodes.Count; index++)
            {
                labels[index] = nodes[index].DisplayName;
            }

            return string.Join(", ", labels);
        }
    }
}

