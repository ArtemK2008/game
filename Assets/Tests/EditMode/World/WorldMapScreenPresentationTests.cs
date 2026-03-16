using NUnit.Framework;
using Survivalon.Runtime;
using UnityEngine;
using Survivalon.Runtime.Combat;
using Survivalon.Runtime.Core;
using Survivalon.Runtime.Run;
using Survivalon.Runtime.State;
using Survivalon.Runtime.State.Persistence;
using Survivalon.Runtime.World;
using Survivalon.Tests.EditMode.State.Persistence;

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
