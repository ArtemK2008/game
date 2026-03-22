using System;
using System.Collections.Generic;
using Survivalon.Core;
using Survivalon.Data.Characters;
using Survivalon.Data.Gear;
using Survivalon.State;

namespace Survivalon.World
{
    public static class WorldMapScreenTextBuilder
    {
        public static string BuildSummaryText(
            WorldMapWorldStateSummary worldStateSummary,
            bool hasSelectedNode,
            NodeId selectedNodeId,
            SessionContextState sessionContext)
        {
            if (worldStateSummary == null)
            {
                throw new ArgumentNullException(nameof(worldStateSummary));
            }

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

            return
                $"Location: {worldStateSummary.CurrentLocationDisplayName} | Region: {worldStateSummary.CurrentRegionId.Value}\n" +
                $"Current node: {worldStateSummary.CurrentNodeId.Value} ({worldStateSummary.CurrentNodeState}) | Selected: {selectedNodeLabel}\n" +
                $"Reachable destinations: {worldStateSummary.SelectableDestinationCount} ({worldStateSummary.ForwardRouteNodeIds.Count} forward / {worldStateSummary.BacktrackOrFarmNodeIds.Count} backtrack-farm)\n" +
                $"Forward routes: {BuildNodeListLabel(worldStateSummary.ForwardRouteNodeIds)}\n" +
                $"Backtrack / farm: {BuildNodeListLabel(worldStateSummary.BacktrackOrFarmNodeIds)}\n" +
                $"Blocked links: {BuildNodeListLabel(worldStateSummary.BlockedLinkedNodeIds)}\n" +
                $"Recent: {recentNodeLabel} | Push target: {recentPushTargetLabel} | Last selected: {lastSelectedNodeLabel}\n" +
                "State legend: Available = enterable | InProgress = started | Cleared = replayable | Locked = blocked\n" +
                "Status legend: Current = active anchor | Selectable = can enter now | Known = visible but not enterable\n" +
                "Select a reachable node, then confirm entry to start the current node flow.";
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
                $"{nodeOption.LocationDisplayName} / {nodeOption.NodeId.Value}\n" +
                $"Path: {BuildPathRoleLabel(nodeOption.PathRole)} | Type: {nodeOption.NodeType} | State: {nodeOption.NodeState}\n" +
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
                case WorldMapPathRole.BacktrackOrFarmRoute:
                    return "Backtrack / farm route";
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

        private static string BuildNodeListLabel(IReadOnlyList<NodeId> nodeIds)
        {
            if (nodeIds == null)
            {
                throw new ArgumentNullException(nameof(nodeIds));
            }

            if (nodeIds.Count == 0)
            {
                return "none";
            }

            string[] labels = new string[nodeIds.Count];
            for (int index = 0; index < nodeIds.Count; index++)
            {
                labels[index] = nodeIds[index].Value;
            }

            return string.Join(", ", labels);
        }
    }
}

