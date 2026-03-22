using System.Collections.Generic;
using NUnit.Framework;
using UnityEngine;
using Survivalon.Core;
using Survivalon.Data.Characters;
using Survivalon.Data.Gear;
using Survivalon.State;
using Survivalon.World;

namespace Survivalon.Tests.EditMode.World
{
    public sealed class WorldMapScreenPresentationTests
    {
        [Test]
        public void BuildSummaryText_ShouldRejectMissingWorldStateSummary()
        {
            Assert.That(
                () => WorldMapScreenTextBuilder.BuildSummaryText(
                    null,
                    hasSelectedNode: false,
                    selectedNodeId: default,
                    sessionContext: null),
                Throws.ArgumentNullException.With.Property("ParamName").EqualTo("worldStateSummary"));
        }

        [Test]
        public void BuildSummaryText_ShouldMatchExistingWorldMapSummary()
        {
            SessionContextState sessionContext = new SessionContextState();
            sessionContext.RecordSelection(new NodeId("region_001_node_004"), isForwardSelectable: true);
            sessionContext.RecordReturnedToWorldContext(new NodeId("region_001_node_002"));

            string summaryText = WorldMapScreenTextBuilder.BuildSummaryText(
                CreateWorldStateSummary(
                    "Verdant Frontier",
                    "region_001",
                    "region_001_node_002",
                    NodeState.InProgress,
                    selectableDestinationCount: 3,
                    new[]
                    {
                        new NodeId("region_001_node_004"),
                        new NodeId("region_002_node_001"),
                    },
                    new[]
                    {
                        new NodeId("region_001_node_001"),
                    },
                    new[]
                    {
                        new NodeId("region_001_node_003"),
                    }),
                hasSelectedNode: true,
                selectedNodeId: new NodeId("region_001_node_004"),
                sessionContext: sessionContext);

            Assert.That(summaryText, Is.EqualTo(
                "Location: Verdant Frontier | Region: region_001\n" +
                "Current node: region_001_node_002 (InProgress) | Selected: region_001_node_004\n" +
                "Reachable destinations: 3 (2 forward / 1 backtrack-farm)\n" +
                "Forward routes: region_001_node_004, region_002_node_001\n" +
                "Backtrack / farm: region_001_node_001\n" +
                "Blocked links: region_001_node_003\n" +
                "Recent: region_001_node_002 | Push target: region_001_node_004 | Last selected: region_001_node_004\n" +
                "State legend: Available = enterable | InProgress = started | Cleared = replayable | Locked = blocked\n" +
                "Status legend: Current = active anchor | Selectable = can enter now | Known = visible but not enterable\n" +
                "Select a reachable node, then confirm entry to start the current node flow."));
        }

        [Test]
        public void BuildSummaryText_ShouldShowFallbackLabelsWithoutSelectionOrSessionContext()
        {
            string summaryText = WorldMapScreenTextBuilder.BuildSummaryText(
                CreateWorldStateSummary(
                    "Verdant Frontier",
                    "region_001",
                    "region_001_node_002",
                    NodeState.InProgress,
                    selectableDestinationCount: 1,
                    System.Array.Empty<NodeId>(),
                    new[]
                    {
                        new NodeId("region_001_node_001"),
                    },
                    System.Array.Empty<NodeId>()),
                hasSelectedNode: false,
                selectedNodeId: default,
                sessionContext: null);

            Assert.That(summaryText, Does.Contain("Location: Verdant Frontier | Region: region_001"));
            Assert.That(summaryText, Does.Contain("Current node: region_001_node_002 (InProgress) | Selected: none"));
            Assert.That(summaryText, Does.Contain("Reachable destinations: 1 (0 forward / 1 backtrack-farm)"));
            Assert.That(summaryText, Does.Contain("Forward routes: none"));
            Assert.That(summaryText, Does.Contain("Backtrack / farm: region_001_node_001"));
            Assert.That(summaryText, Does.Contain("Blocked links: none"));
            Assert.That(summaryText, Does.Contain("Recent: none | Push target: none | Last selected: none"));
            Assert.That(summaryText, Does.Contain("State legend: Available = enterable"));
            Assert.That(summaryText, Does.Contain("Status legend: Current = active anchor"));
        }

        [Test]
        public void BuildCharacterSelectionText_ShouldShowCurrentSelectionAndAvailableCount()
        {
            string selectionText = WorldMapScreenTextBuilder.BuildCharacterSelectionText(
                new[]
                {
                    new PlayableCharacterSelectionOption("character_vanguard", "Vanguard", isSelected: false),
                    new PlayableCharacterSelectionOption("character_striker", "Striker", isSelected: true),
                });

            Assert.That(selectionText, Is.EqualTo(
                "Selected character: Striker\n" +
                "Available characters: 2"));
        }

        [Test]
        public void BuildCharacterButtonLabel_ShouldShowSelectedFormatting()
        {
            string selectedLabel = WorldMapScreenTextBuilder.BuildCharacterButtonLabel(
                new PlayableCharacterSelectionOption("character_vanguard", "Vanguard", isSelected: true));
            string unselectedLabel = WorldMapScreenTextBuilder.BuildCharacterButtonLabel(
                new PlayableCharacterSelectionOption("character_vanguard", "Vanguard", isSelected: false));

            Assert.That(selectedLabel, Is.EqualTo("Selected: Vanguard"));
            Assert.That(unselectedLabel, Is.EqualTo("Select: Vanguard"));
        }

        [Test]
        public void BuildAssignmentText_ShouldShowAssignedPackageAndSummary()
        {
            string assignmentText = WorldMapScreenTextBuilder.BuildAssignmentText(
                "Vanguard",
                new[]
                {
                    new PlayableCharacterSkillPackageOption(
                        "character_vanguard",
                        PlayableCharacterSkillPackageIds.VanguardDefault,
                        "Standard Guard",
                        "No passive or active skill.",
                        isAssigned: false),
                    new PlayableCharacterSkillPackageOption(
                        "character_vanguard",
                        PlayableCharacterSkillPackageIds.VanguardBurstDrill,
                        "Burst Drill",
                        "Adds Burst Strike.",
                        isAssigned: true),
                },
                new[]
                {
                    new PlayableCharacterGearAssignmentOption(
                        "character_vanguard",
                        GearIds.TrainingBlade,
                        "Training Blade",
                        GearCategory.PrimaryCombat,
                        isEquipped: true),
                    new PlayableCharacterGearAssignmentOption(
                        "character_vanguard",
                        GearIds.GuardCharm,
                        "Guard Charm",
                        GearCategory.SecondarySupport,
                        isEquipped: false),
                });

            Assert.That(assignmentText, Is.EqualTo(
                "Selected character build: Vanguard\n" +
                "Assigned package: Burst Drill\n" +
                "Primary gear: Training Blade | Support gear: none\n" +
                "Available packages: 2 | Owned primary gear: 1 | Owned support gear: 1"));
        }

        [Test]
        public void BuildSkillPackageButtonLabel_ShouldShowAssignedFormatting()
        {
            string assignedLabel = WorldMapScreenTextBuilder.BuildSkillPackageButtonLabel(
                new PlayableCharacterSkillPackageOption(
                    "character_vanguard",
                    PlayableCharacterSkillPackageIds.VanguardDefault,
                    "Standard Guard",
                    "No passive or active skill.",
                    isAssigned: true));
            string unassignedLabel = WorldMapScreenTextBuilder.BuildSkillPackageButtonLabel(
                new PlayableCharacterSkillPackageOption(
                    "character_vanguard",
                    PlayableCharacterSkillPackageIds.VanguardBurstDrill,
                    "Burst Drill",
                    "Adds Burst Strike.",
                    isAssigned: false));

            Assert.That(assignedLabel, Is.EqualTo("Assigned: Standard Guard"));
            Assert.That(unassignedLabel, Is.EqualTo("Assign: Burst Drill"));
        }

        [Test]
        public void BuildGearAssignmentButtonLabel_ShouldShowEquipAndUnequipFormatting()
        {
            string equippedLabel = WorldMapScreenTextBuilder.BuildGearAssignmentButtonLabel(
                new PlayableCharacterGearAssignmentOption(
                    "character_vanguard",
                    GearIds.TrainingBlade,
                    "Training Blade",
                    GearCategory.PrimaryCombat,
                    isEquipped: true));
            string unequippedLabel = WorldMapScreenTextBuilder.BuildGearAssignmentButtonLabel(
                new PlayableCharacterGearAssignmentOption(
                    "character_vanguard",
                    GearIds.TrainingBlade,
                    "Training Blade",
                    GearCategory.PrimaryCombat,
                    isEquipped: false));

            Assert.That(equippedLabel, Is.EqualTo("Unequip: Training Blade"));
            Assert.That(unequippedLabel, Is.EqualTo("Equip: Training Blade"));
        }

        [Test]
        public void BuildNodeLabel_ShouldMatchExistingFormatting()
        {
            string labelText = WorldMapScreenTextBuilder.BuildNodeLabel(
                new WorldMapNodeOption(
                    new NodeId("region_002_node_001"),
                    new RegionId("region_002"),
                    NodeType.ServiceOrProgression,
                    NodeState.Available,
                    isSelectable: true,
                    isCurrentContext: false,
                    isSelected: false,
                    "Echo Caverns",
                    WorldMapPathRole.ForwardRoute));

            Assert.That(labelText, Is.EqualTo(
                "Echo Caverns / region_002_node_001\n" +
                "Path: Forward route | Type: ServiceOrProgression | State: Available\n" +
                "Status: Selectable"));
        }

        [Test]
        public void BuildNodeLabel_ShouldRejectMissingNodeOption()
        {
            Assert.That(
                () => WorldMapScreenTextBuilder.BuildNodeLabel(null),
                Throws.ArgumentNullException.With.Property("ParamName").EqualTo("nodeOption"));
        }

        [Test]
        public void BuildCharacterSelectionText_ShouldRejectMissingSelectionOptions()
        {
            Assert.That(
                () => WorldMapScreenTextBuilder.BuildCharacterSelectionText(null),
                Throws.ArgumentNullException.With.Property("ParamName").EqualTo("selectionOptions"));
        }

        [Test]
        public void BuildCharacterButtonLabel_ShouldRejectMissingSelectionOption()
        {
            Assert.That(
                () => WorldMapScreenTextBuilder.BuildCharacterButtonLabel(null),
                Throws.ArgumentNullException.With.Property("ParamName").EqualTo("selectionOption"));
        }

        [Test]
        public void BuildAssignmentText_ShouldRejectMissingSkillPackageOptions()
        {
            Assert.That(
                () => WorldMapScreenTextBuilder.BuildAssignmentText("Vanguard", null, System.Array.Empty<PlayableCharacterGearAssignmentOption>()),
                Throws.ArgumentNullException.With.Property("ParamName").EqualTo("skillPackageOptions"));
        }

        [Test]
        public void BuildAssignmentText_ShouldRejectMissingGearAssignmentOptions()
        {
            Assert.That(
                () => WorldMapScreenTextBuilder.BuildAssignmentText("Vanguard", System.Array.Empty<PlayableCharacterSkillPackageOption>(), null),
                Throws.ArgumentNullException.With.Property("ParamName").EqualTo("gearAssignmentOptions"));
        }

        [Test]
        public void BuildSkillPackageButtonLabel_ShouldRejectMissingSkillPackageOption()
        {
            Assert.That(
                () => WorldMapScreenTextBuilder.BuildSkillPackageButtonLabel(null),
                Throws.ArgumentNullException.With.Property("ParamName").EqualTo("skillPackageOption"));
        }

        [Test]
        public void BuildGearAssignmentButtonLabel_ShouldRejectMissingGearAssignmentOption()
        {
            Assert.That(
                () => WorldMapScreenTextBuilder.BuildGearAssignmentButtonLabel(null),
                Throws.ArgumentNullException.With.Property("ParamName").EqualTo("gearAssignmentOption"));
        }

        [Test]
        public void ResolveEntryButtonState_ShouldMatchSelectionAndHandlerAvailability()
        {
            WorldMapScreenButtonState unavailableState = WorldMapScreenStateResolver.ResolveEntryButtonState(
                hasNodeEntryHandler: false,
                hasSelectedNode: true,
                selectedNodeId: new NodeId("region_001_node_004"));
            WorldMapScreenButtonState noSelectionState = WorldMapScreenStateResolver.ResolveEntryButtonState(
                hasNodeEntryHandler: true,
                hasSelectedNode: false,
                selectedNodeId: default);
            WorldMapScreenButtonState availableState = WorldMapScreenStateResolver.ResolveEntryButtonState(
                hasNodeEntryHandler: true,
                hasSelectedNode: true,
                selectedNodeId: new NodeId("region_001_node_004"));

            AssertButtonState(unavailableState, "Select a reachable node to enter", false);
            AssertButtonState(noSelectionState, "Select a reachable node to enter", false);
            AssertButtonState(availableState, "Enter region_001_node_004", true);
        }

        [Test]
        public void ResolveNodeColor_ShouldMatchExistingPriorityOrder()
        {
            Assert.That(
                WorldMapScreenStateResolver.ResolveNodeColor(CreateNodeOption("selected", NodeState.Available, true, false, true)),
                Is.EqualTo(new Color(0.77f, 0.62f, 0.20f, 1f)));
            Assert.That(
                WorldMapScreenStateResolver.ResolveNodeColor(CreateNodeOption("current", NodeState.Available, false, true, false)),
                Is.EqualTo(new Color(0.18f, 0.39f, 0.70f, 1f)));
            Assert.That(
                WorldMapScreenStateResolver.ResolveNodeColor(CreateNodeOption("selectable", NodeState.Available, true, false, false)),
                Is.EqualTo(new Color(0.18f, 0.50f, 0.24f, 1f)));
            Assert.That(
                WorldMapScreenStateResolver.ResolveNodeColor(CreateNodeOption("locked", NodeState.Locked, false, false, false)),
                Is.EqualTo(new Color(0.24f, 0.24f, 0.27f, 1f)));
            Assert.That(
                WorldMapScreenStateResolver.ResolveNodeColor(CreateNodeOption("known", NodeState.Cleared, false, false, false)),
                Is.EqualTo(new Color(0.34f, 0.34f, 0.38f, 1f)));
        }

        [Test]
        public void ResolveNodeColor_ShouldRejectMissingNodeOption()
        {
            Assert.That(
                () => WorldMapScreenStateResolver.ResolveNodeColor(null),
                Throws.ArgumentNullException.With.Property("ParamName").EqualTo("nodeOption"));
        }

        private static void AssertButtonState(WorldMapScreenButtonState buttonState, string expectedLabel, bool expectedInteractable)
        {
            Assert.That(buttonState.Label, Is.EqualTo(expectedLabel));
            Assert.That(buttonState.IsInteractable, Is.EqualTo(expectedInteractable));
        }

        private static WorldMapNodeOption CreateNodeOption(
            string nodeIdValue,
            NodeState nodeState,
            bool isSelectable,
            bool isCurrentContext,
            bool isSelected)
        {
            return new WorldMapNodeOption(
                new NodeId(nodeIdValue),
                new RegionId("region_001"),
                NodeType.Combat,
                nodeState,
                isSelectable,
                isCurrentContext,
                isSelected,
                "Verdant Frontier",
                isCurrentContext
                    ? WorldMapPathRole.CurrentContext
                    : isSelectable
                        ? WorldMapPathRole.ForwardRoute
                        : WorldMapPathRole.BlockedPath);
        }

        private static WorldMapWorldStateSummary CreateWorldStateSummary(
            string locationDisplayName,
            string regionIdValue,
            string currentNodeIdValue,
            NodeState currentNodeState,
            int selectableDestinationCount,
            IReadOnlyList<NodeId> forwardRouteNodeIds,
            IReadOnlyList<NodeId> backtrackOrFarmNodeIds,
            IReadOnlyList<NodeId> blockedLinkedNodeIds)
        {
            return new WorldMapWorldStateSummary(
                locationDisplayName,
                new RegionId(regionIdValue),
                new NodeId(currentNodeIdValue),
                currentNodeState,
                selectableDestinationCount,
                forwardRouteNodeIds,
                backtrackOrFarmNodeIds,
                blockedLinkedNodeIds);
        }
    }
}

