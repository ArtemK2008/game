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
        public void BuildSummaryText_ShouldRejectMissingNodeOptions()
        {
            Assert.That(
                () => WorldMapScreenTextBuilder.BuildSummaryText(
                    null,
                    hasSelectedNode: false,
                    selectedNodeId: default,
                    sessionContext: null,
                    hasForwardRouteChoice: false,
                    forwardSelectableNodeCount: 0),
                Throws.ArgumentNullException.With.Property("ParamName").EqualTo("nodeOptions"));
        }

        [Test]
        public void BuildSummaryText_ShouldMatchExistingWorldMapSummary()
        {
            SessionContextState sessionContext = new SessionContextState();
            sessionContext.RecordSelection(new NodeId("region_001_node_004"), isForwardSelectable: true);
            sessionContext.RecordRunReturned(new NodeId("region_001_node_002"));

            string summaryText = WorldMapScreenTextBuilder.BuildSummaryText(
                new[]
                {
                    CreateNodeOption("region_001_node_002", NodeState.InProgress, isSelectable: false, isCurrentContext: true, isSelected: false),
                    CreateNodeOption("region_001_node_004", NodeState.Available, isSelectable: true, isCurrentContext: false, isSelected: true),
                    CreateNodeOption("region_002_node_001", NodeState.Available, isSelectable: true, isCurrentContext: false, isSelected: false),
                },
                hasSelectedNode: true,
                selectedNodeId: new NodeId("region_001_node_004"),
                sessionContext: sessionContext,
                hasForwardRouteChoice: true,
                forwardSelectableNodeCount: 2);

            Assert.That(summaryText, Is.EqualTo(
                "Current node: region_001_node_002\n" +
                "Recent node: region_001_node_002\n" +
                "Recent push target: region_001_node_004\n" +
                "Last selected node: region_001_node_004\n" +
                "Selectable destinations: 2\n" +
                "Forward route options: 2 (Branch choice available)\n" +
                "Selected node: region_001_node_004\n" +
                "Select a reachable node, then confirm entry to start the placeholder node flow."));
        }

        [Test]
        public void BuildSummaryText_ShouldShowFallbackLabelsWithoutSelectionOrSessionContext()
        {
            string summaryText = WorldMapScreenTextBuilder.BuildSummaryText(
                new[]
                {
                    CreateNodeOption("region_001_node_002", NodeState.InProgress, isSelectable: false, isCurrentContext: true, isSelected: false),
                },
                hasSelectedNode: false,
                selectedNodeId: default,
                sessionContext: null,
                hasForwardRouteChoice: false,
                forwardSelectableNodeCount: 1);

            Assert.That(summaryText, Does.Contain("Recent node: none"));
            Assert.That(summaryText, Does.Contain("Recent push target: none"));
            Assert.That(summaryText, Does.Contain("Last selected node: none"));
            Assert.That(summaryText, Does.Contain("Selected node: none"));
            Assert.That(summaryText, Does.Contain("Forward route options: 1 (Single forward route)"));
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
        public void BuildSkillPackageAssignmentText_ShouldShowAssignedPackageAndSummary()
        {
            string assignmentText = WorldMapScreenTextBuilder.BuildSkillPackageAssignmentText(
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
                "Training Blade",
                availablePrimaryCombatGearCount: 1);

            Assert.That(assignmentText, Is.EqualTo(
                "Selected character build: Vanguard\n" +
                "Assigned package: Burst Drill\n" +
                "Equipped primary gear: Training Blade\n" +
                "Available packages: 2 | Owned primary gear: 1"));
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
                    isSelected: false));

            Assert.That(labelText, Is.EqualTo(
                "region_002 / region_002_node_001\n" +
                "Type: ServiceOrProgression | State: Available | Selectable"));
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
        public void BuildSkillPackageAssignmentText_ShouldRejectMissingSkillPackageOptions()
        {
            Assert.That(
                () => WorldMapScreenTextBuilder.BuildSkillPackageAssignmentText("Vanguard", null, "none", 0),
                Throws.ArgumentNullException.With.Property("ParamName").EqualTo("skillPackageOptions"));
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
                isSelected);
        }
    }
}

