using NUnit.Framework;
using Survivalon.Runtime;

namespace Survivalon.Tests.EditMode
{
    public sealed class NodePlaceholderScreenPresentationTests
    {
        [Test]
        public void BuildSummaryText_ShouldMatchExistingRunStartSummary()
        {
            string summaryText = NodePlaceholderScreenTextBuilder.BuildSummaryText(
                CreateServicePlaceholderState(),
                RunLifecycleState.RunStart);

            Assert.That(summaryText, Is.EqualTo(
                "Region: region_002\n" +
                "Type: ServiceOrProgression\n" +
                "Node state: Available\n" +
                "Lifecycle: RunStart\n" +
                "Entered from: region_001_node_002"));
        }

        [Test]
        public void BuildSummaryText_ShouldIncludeResolutionOnlyDuringResolvedState()
        {
            RunResult runResult = CreateSucceededRunResult();

            string resolvedSummary = NodePlaceholderScreenTextBuilder.BuildSummaryText(
                CreateServicePlaceholderState(),
                RunLifecycleState.RunResolved,
                runResult);
            string postRunSummary = NodePlaceholderScreenTextBuilder.BuildSummaryText(
                CreateServicePlaceholderState(),
                RunLifecycleState.PostRun,
                runResult);

            Assert.That(resolvedSummary, Does.Contain("Resolution: Succeeded"));
            Assert.That(postRunSummary, Does.Not.Contain("Resolution: Succeeded"));
        }

        [Test]
        public void BuildStatusText_ShouldMatchNonCombatLifecycleTexts()
        {
            NodePlaceholderState serviceState = CreateServicePlaceholderState();

            Assert.That(
                NodePlaceholderScreenTextBuilder.BuildStatusText(serviceState, RunLifecycleState.RunStart),
                Is.EqualTo("Run shell initialized. Start the placeholder run when ready."));
            Assert.That(
                NodePlaceholderScreenTextBuilder.BuildStatusText(serviceState, RunLifecycleState.RunActive),
                Is.EqualTo("Run is active. Resolve the placeholder run to produce a run result."));
            Assert.That(
                NodePlaceholderScreenTextBuilder.BuildStatusText(serviceState, RunLifecycleState.RunResolved),
                Is.EqualTo("Run resolved. Open the post-run state to review next actions."));
            Assert.That(
                NodePlaceholderScreenTextBuilder.BuildStatusText(serviceState, RunLifecycleState.PostRun),
                Is.EqualTo("Post-run summary is active. Replay, return to the world map, or stop the session."));
        }

        [Test]
        public void BuildStatusText_ShouldMatchCombatLifecycleTexts()
        {
            NodePlaceholderState combatState = CreateCombatPlaceholderState();
            CombatEncounterState resolvedEncounterState = CreateResolvedCombatEncounterState();

            Assert.That(
                NodePlaceholderScreenTextBuilder.BuildStatusText(combatState, RunLifecycleState.RunStart),
                Is.EqualTo("Combat shell initialized. Preparing automatic combat startup."));
            Assert.That(
                NodePlaceholderScreenTextBuilder.BuildStatusText(
                    combatState,
                    RunLifecycleState.RunActive,
                    CreateActiveCombatEncounterState()),
                Is.EqualTo("Combat shell active. Enemy hostility and player attacks resolve automatically until one side is defeated."));
            Assert.That(
                NodePlaceholderScreenTextBuilder.BuildStatusText(
                    combatState,
                    RunLifecycleState.RunResolved,
                    resolvedEncounterState),
                Is.EqualTo("Combat shell resolved. Winner: Player. Preparing post-run summary."));
        }

        [Test]
        public void ResolveAdvanceButtonState_ShouldMatchLifecycleAndCombatState()
        {
            NodePlaceholderState serviceState = CreateServicePlaceholderState();
            NodePlaceholderState combatState = CreateCombatPlaceholderState();

            AssertButtonState(
                NodePlaceholderScreenStateResolver.ResolveAdvanceButtonState(serviceState, RunLifecycleState.RunStart, false),
                "Start Placeholder Run",
                true);
            AssertButtonState(
                NodePlaceholderScreenStateResolver.ResolveAdvanceButtonState(combatState, RunLifecycleState.RunStart, false),
                "Combat Auto-Starting",
                false);
            AssertButtonState(
                NodePlaceholderScreenStateResolver.ResolveAdvanceButtonState(serviceState, RunLifecycleState.RunActive, false),
                "Resolve Placeholder Run",
                true);
            AssertButtonState(
                NodePlaceholderScreenStateResolver.ResolveAdvanceButtonState(combatState, RunLifecycleState.RunActive, true),
                "Combat Auto-Running",
                false);
            AssertButtonState(
                NodePlaceholderScreenStateResolver.ResolveAdvanceButtonState(serviceState, RunLifecycleState.RunResolved, false),
                "Enter Post-Run State",
                true);
            AssertButtonState(
                NodePlaceholderScreenStateResolver.ResolveAdvanceButtonState(combatState, RunLifecycleState.RunResolved, true),
                "Preparing Post-Run",
                false);
            AssertButtonState(
                NodePlaceholderScreenStateResolver.ResolveAdvanceButtonState(combatState, RunLifecycleState.PostRun, true),
                "Run Lifecycle Complete",
                false);
        }

        [Test]
        public void ResolvePostRunPanelState_ShouldMatchExistingActionAvailability()
        {
            PostRunStateController postRunStateController = new PostRunStateController(
                CreateCombatPlaceholderState(),
                CreateSucceededRunResult());

            NodePlaceholderScreenPostRunPanelState hiddenState =
                NodePlaceholderScreenStateResolver.ResolvePostRunPanelState(
                    RunLifecycleState.RunActive,
                    null,
                    false);
            NodePlaceholderScreenPostRunPanelState visibleState =
                NodePlaceholderScreenStateResolver.ResolvePostRunPanelState(
                    RunLifecycleState.PostRun,
                    postRunStateController,
                    true);
            NodePlaceholderScreenPostRunPanelState noStopHandlerState =
                NodePlaceholderScreenStateResolver.ResolvePostRunPanelState(
                    RunLifecycleState.PostRun,
                    postRunStateController,
                    false);

            Assert.That(hiddenState.IsVisible, Is.False);
            AssertButtonState(hiddenState.ReplayButton, "Replay Node", false);
            AssertButtonState(hiddenState.ReturnToWorldButton, "Return To World Map", false);
            AssertButtonState(hiddenState.StopSessionButton, "Stop Session", false);

            Assert.That(visibleState.IsVisible, Is.True);
            AssertButtonState(visibleState.ReplayButton, "Replay Node", true);
            AssertButtonState(visibleState.ReturnToWorldButton, "Return To World Map", true);
            AssertButtonState(visibleState.StopSessionButton, "Stop Session", true);

            Assert.That(noStopHandlerState.IsVisible, Is.True);
            AssertButtonState(noStopHandlerState.StopSessionButton, "Stop Session Unavailable", false);
        }

        [Test]
        public void ShouldShowCombatShell_ShouldOnlyBeTrueForActiveOrResolvedCombatEncounter()
        {
            Assert.That(NodePlaceholderScreenStateResolver.ShouldShowCombatShell(RunLifecycleState.RunStart, true), Is.False);
            Assert.That(NodePlaceholderScreenStateResolver.ShouldShowCombatShell(RunLifecycleState.RunActive, false), Is.False);
            Assert.That(NodePlaceholderScreenStateResolver.ShouldShowCombatShell(RunLifecycleState.RunActive, true), Is.True);
            Assert.That(NodePlaceholderScreenStateResolver.ShouldShowCombatShell(RunLifecycleState.RunResolved, true), Is.True);
            Assert.That(NodePlaceholderScreenStateResolver.ShouldShowCombatShell(RunLifecycleState.PostRun, true), Is.False);
        }

        [Test]
        public void BuildPostRunSummaryText_ShouldMatchExistingSummaryFormatting()
        {
            RunResult runResult = CreateSucceededRunResult();
            PostRunStateController postRunStateController = new PostRunStateController(
                CreateCombatPlaceholderState(),
                runResult);

            string summaryText = NodePlaceholderScreenTextBuilder.BuildPostRunSummaryText(
                postRunStateController,
                runResult);

            Assert.That(summaryText, Is.EqualTo(
                "Run finished.\n" +
                "Node: region_001_node_004\n" +
                "Resolution: Succeeded\n" +
                "Node progress total: 2 / 3\n" +
                "Rewards: None\n" +
                "Node progress delta: 2\n" +
                "Persistent progression delta: 0\n" +
                "Route unlock changed: Yes\n" +
                "Next actions:\n" +
                "- Replay: Yes\n" +
                "- Return to world: Yes\n" +
                "- Stop: Yes"));
        }

        private static void AssertButtonState(NodePlaceholderScreenButtonState buttonState, string expectedLabel, bool expectedInteractable)
        {
            Assert.That(buttonState.Label, Is.EqualTo(expectedLabel));
            Assert.That(buttonState.IsInteractable, Is.EqualTo(expectedInteractable));
        }

        private static NodePlaceholderState CreateServicePlaceholderState()
        {
            return new NodePlaceholderState(
                new NodeId("region_002_node_001"),
                new RegionId("region_002"),
                NodeType.ServiceOrProgression,
                NodeState.Available,
                new NodeId("region_001_node_002"));
        }

        private static NodePlaceholderState CreateCombatPlaceholderState()
        {
            return new NodePlaceholderState(
                new NodeId("region_001_node_004"),
                new RegionId("region_001"),
                NodeType.Combat,
                NodeState.Available,
                new NodeId("region_001_node_002"));
        }

        private static CombatEncounterState CreateActiveCombatEncounterState()
        {
            CombatShellContextFactory combatShellContextFactory = new CombatShellContextFactory();
            CombatShellContext combatContext = combatShellContextFactory.Create(CreateCombatPlaceholderState());
            return new CombatEncounterState(combatContext);
        }

        private static CombatEncounterState CreateResolvedCombatEncounterState()
        {
            CombatEncounterState encounterState = CreateActiveCombatEncounterState();
            encounterState.Resolve(CombatEncounterOutcome.PlayerVictory);
            return encounterState;
        }

        private static RunResult CreateSucceededRunResult()
        {
            return new RunResult(
                new NodeId("region_001_node_004"),
                RunResolutionState.Succeeded,
                RunRewardPayload.Empty,
                2,
                2,
                3,
                0,
                true,
                new RunNextActionContext(
                    canReplayNode: true,
                    canChooseAnotherNode: true,
                    canStopSession: true));
        }
    }
}
