using System.Collections.Generic;
using NUnit.Framework;
using UnityEngine;
using Survivalon.Characters;
using Survivalon.Core;
using Survivalon.Data.Characters;
using Survivalon.Data.Gear;
using Survivalon.World;

namespace Survivalon.Tests.EditMode.World
{
    /// <summary>
    /// Проверяет player-facing presentation текста карты мира после выноса runtime character options в Characters.
    /// </summary>
    public sealed class WorldMapScreenPresentationTests
    {
        [Test]
        public void BuildSummaryText_ShouldRejectMissingWorldStateSummary()
        {
            Assert.That(
                () => WorldMapScreenTextBuilder.BuildSummaryText(
                    null,
                    selectedNodeDisplayName: null),
                Throws.ArgumentNullException.With.Property("ParamName").EqualTo("worldStateSummary"));
        }

        [Test]
        public void BuildSummaryText_ShouldMatchExistingWorldMapSummary()
        {
            string summaryText = WorldMapScreenTextBuilder.BuildSummaryText(
                CreateWorldStateSummary(
                    "Verdant Frontier",
                    CreateNodeReference("region_001_node_002", "Raider Trail"),
                    NodeState.InProgress,
                    selectableDestinationCount: 4,
                    new[]
                    {
                        CreateNodeReference("region_002_node_001", "Cavern Service Hub"),
                        CreateNodeReference("region_001_node_004", "Forest Farm"),
                        CreateNodeReference("region_001_node_006", "Raider Holdout"),
                    },
                    new[]
                    {
                        CreateNodeReference("region_001_node_001", "Frontier Entry"),
                    },
                    System.Array.Empty<WorldMapNodeReferenceDisplayState>(),
                    new[]
                    {
                        CreateNodeReference("region_001_node_003", "Frontier Gate"),
                    }),
                "Forest Farm");

            Assert.That(summaryText, Is.EqualTo(
                "Location: Verdant Frontier\n" +
                "Current: Raider Trail (In progress) | Selected: Forest Farm\n" +
                "Paths now: 4 enterable | 3 forward | 1 backtrack | 0 replayable | 1 blocked\n" +
                "Forward: Cavern Service Hub, Forest Farm, Raider Holdout\n" +
                "Backtrack: Frontier Entry | Replayable: none\n" +
                "Blocked: Frontier Gate\n" +
                "Node states: Available = enterable | InProgress = started | Cleared = replayable | Locked = blocked"));
        }

        [Test]
        public void BuildSummaryText_ShouldShowFallbackLabelsWithoutSelectionOrSessionContext()
        {
            string summaryText = WorldMapScreenTextBuilder.BuildSummaryText(
                CreateWorldStateSummary(
                    "Verdant Frontier",
                    CreateNodeReference("region_001_node_002", "Raider Trail"),
                    NodeState.InProgress,
                    selectableDestinationCount: 1,
                    System.Array.Empty<WorldMapNodeReferenceDisplayState>(),
                    new[]
                    {
                        CreateNodeReference("region_001_node_001", "Frontier Entry"),
                    },
                    System.Array.Empty<WorldMapNodeReferenceDisplayState>(),
                    System.Array.Empty<WorldMapNodeReferenceDisplayState>()),
                selectedNodeDisplayName: null);

            Assert.That(summaryText, Does.Contain("Location: Verdant Frontier"));
            Assert.That(summaryText, Does.Contain("Current: Raider Trail (In progress) | Selected: none"));
            Assert.That(summaryText, Does.Contain("Paths now: 1 enterable | 0 forward | 1 backtrack | 0 replayable | 0 blocked"));
            Assert.That(summaryText, Does.Contain("Forward: none"));
            Assert.That(summaryText, Does.Contain("Backtrack: Frontier Entry | Replayable: none"));
            Assert.That(summaryText, Does.Contain("Blocked: none"));
            Assert.That(summaryText, Does.Contain("Node states: Available = enterable"));
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
                    "Cavern Service Hub",
                    WorldMapPathRole.ForwardRoute));

            Assert.That(labelText, Is.EqualTo(
                "Cavern Service Hub\n" +
                "Echo Caverns\n" +
                "Path: Forward route | Type: Service hub | State: Available\n" +
                "Status: Selectable"));
        }

        [Test]
        public void BuildNodeLabel_ShouldShowFarmReadyMarkerForCompletedCombatContent()
        {
            string labelText = WorldMapScreenTextBuilder.BuildNodeLabel(
                new WorldMapNodeOption(
                    new NodeId("region_001_node_004"),
                    new RegionId("region_001"),
                    NodeType.Combat,
                    NodeState.Cleared,
                    isSelectable: true,
                    isCurrentContext: false,
                    isSelected: false,
                    "Verdant Frontier",
                    "Forest Farm",
                    WorldMapPathRole.ReplayableFarmNode,
                    isFarmReady: true));

            Assert.That(labelText, Is.EqualTo(
                "Forest Farm\n" +
                "Verdant Frontier\n" +
                "Path: Replayable farm node | Type: Combat | State: Cleared | Farm-ready\n" +
                "Status: Selectable"));
        }

        [Test]
        public void BuildNodeLabel_ShouldShowEliteChallengeMarkerForOptionalChallengeContent()
        {
            string labelText = WorldMapScreenTextBuilder.BuildNodeLabel(
                new WorldMapNodeOption(
                    BootstrapWorldScenario.ForestEliteNodeId,
                    BootstrapWorldScenario.ForestRegionId,
                    NodeType.Combat,
                    NodeState.Available,
                    isSelectable: true,
                    isCurrentContext: false,
                    isSelected: false,
                    "Verdant Frontier",
                    "Raider Holdout",
                    WorldMapPathRole.ForwardRoute,
                    optionalChallengeDisplayName: "Elite challenge"));

            Assert.That(labelText, Is.EqualTo(
                "Raider Holdout\n" +
                "Verdant Frontier\n" +
                "Path: Forward route | Type: Combat | State: Available | Elite challenge\n" +
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
                selectedNodeDisplayName: "Forest Farm",
                hasQuickRepeatNode: false,
                quickRepeatNodeDisplayName: null,
                quickRepeatNodeType: default);
            WorldMapScreenButtonState noSelectionState = WorldMapScreenStateResolver.ResolveEntryButtonState(
                hasNodeEntryHandler: true,
                hasSelectedNode: false,
                selectedNodeDisplayName: null,
                hasQuickRepeatNode: false,
                quickRepeatNodeDisplayName: null,
                quickRepeatNodeType: default);
            WorldMapScreenButtonState availableState = WorldMapScreenStateResolver.ResolveEntryButtonState(
                hasNodeEntryHandler: true,
                hasSelectedNode: true,
                selectedNodeDisplayName: "Forest Farm",
                hasQuickRepeatNode: true,
                quickRepeatNodeDisplayName: "Raider Trail",
                quickRepeatNodeType: NodeType.Combat);
            WorldMapScreenButtonState quickReplayState = WorldMapScreenStateResolver.ResolveEntryButtonState(
                hasNodeEntryHandler: true,
                hasSelectedNode: false,
                selectedNodeDisplayName: null,
                hasQuickRepeatNode: true,
                quickRepeatNodeDisplayName: "Forest Farm",
                quickRepeatNodeType: NodeType.Combat);
            WorldMapScreenButtonState quickReturnState = WorldMapScreenStateResolver.ResolveEntryButtonState(
                hasNodeEntryHandler: true,
                hasSelectedNode: false,
                selectedNodeDisplayName: null,
                hasQuickRepeatNode: true,
                quickRepeatNodeDisplayName: "Cavern Service Hub",
                quickRepeatNodeType: NodeType.ServiceOrProgression);

            AssertButtonState(unavailableState, "Select a reachable node to enter", false);
            AssertButtonState(noSelectionState, "Select a reachable node to enter", false);
            AssertButtonState(availableState, "Enter Forest Farm", true);
            AssertButtonState(quickReplayState, "Replay Forest Farm", true);
            AssertButtonState(quickReturnState, "Return to Cavern Service Hub", true);
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
                "Frontier Test Node",
                isCurrentContext
                    ? WorldMapPathRole.CurrentContext
                    : isSelectable
                        ? WorldMapPathRole.ForwardRoute
                        : WorldMapPathRole.BlockedPath);
        }

        private static WorldMapWorldStateSummary CreateWorldStateSummary(
            string locationDisplayName,
            WorldMapNodeReferenceDisplayState currentNode,
            NodeState currentNodeState,
            int selectableDestinationCount,
            IReadOnlyList<WorldMapNodeReferenceDisplayState> forwardRouteNodes,
            IReadOnlyList<WorldMapNodeReferenceDisplayState> backtrackRouteNodes,
            IReadOnlyList<WorldMapNodeReferenceDisplayState> replayableFarmNodes,
            IReadOnlyList<WorldMapNodeReferenceDisplayState> blockedLinkedNodes)
        {
            return new WorldMapWorldStateSummary(
                locationDisplayName,
                currentNode,
                currentNodeState,
                selectableDestinationCount,
                forwardRouteNodes,
                backtrackRouteNodes,
                replayableFarmNodes,
                blockedLinkedNodes);
        }

        private static WorldMapNodeReferenceDisplayState CreateNodeReference(
            string nodeIdValue,
            string displayName)
        {
            return new WorldMapNodeReferenceDisplayState(new NodeId(nodeIdValue), displayName);
        }
    }
}

