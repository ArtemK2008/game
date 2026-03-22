using NUnit.Framework;
using Survivalon.Combat;
using Survivalon.Core;
using Survivalon.Data.Combat;
using Survivalon.Data.World;
using Survivalon.Run;
using Survivalon.World;

namespace Survivalon.Tests.EditMode.World
{
    public sealed class NodePlaceholderScreenPresentationTests
    {
        [Test]
        public void BuildSummaryText_ShouldRejectMissingPlaceholderState()
        {
            Assert.That(
                () => NodePlaceholderScreenTextBuilder.BuildSummaryText(null, RunLifecycleState.RunStart),
                Throws.ArgumentNullException.With.Property("ParamName").EqualTo("placeholderState"));
        }

        [Test]
        public void BuildSummaryText_ShouldMatchExistingRunStartSummary()
        {
            string summaryText = NodePlaceholderScreenTextBuilder.BuildSummaryText(
                NodePlaceholderTestData.CreateServicePlaceholderState(),
                RunLifecycleState.RunStart);

            Assert.That(summaryText, Is.EqualTo(
                "Location: Echo Caverns\n" +
                "Node: Cavern Service Hub\n" +
                "Region: region_002\n" +
                "Reward focus: Persistent progression gains\n" +
                "Enemy emphasis: Gate guardians\n" +
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
                NodePlaceholderTestData.CreateServicePlaceholderState(),
                RunLifecycleState.RunResolved,
                runResult);
            string postRunSummary = NodePlaceholderScreenTextBuilder.BuildSummaryText(
                NodePlaceholderTestData.CreateServicePlaceholderState(),
                RunLifecycleState.PostRun,
                runResult);

            Assert.That(resolvedSummary, Does.Contain("Resolution: Succeeded"));
            Assert.That(postRunSummary, Does.Not.Contain("Resolution: Succeeded"));
        }

        [Test]
        public void BuildSummaryText_ShouldShowFrontierFarmRevisitValueWhenPresent()
        {
            string summaryText = NodePlaceholderScreenTextBuilder.BuildSummaryText(
                NodePlaceholderTestData.CreateFrontierFarmPlaceholderState(),
                RunLifecycleState.RunStart);

            Assert.That(summaryText, Does.Contain("Location: Verdant Frontier"));
            Assert.That(summaryText, Does.Contain("Node: Forest Farm"));
            Assert.That(summaryText, Does.Contain("Reward focus: Region material farming"));
            Assert.That(summaryText, Does.Contain("Revisit value: Region material yield +1"));
        }

        [Test]
        public void BuildSummaryText_ShouldHideRevisitValueWhenPlaceholderDoesNotSupportRegionMaterialRewards()
        {
            NodePlaceholderState invalidPlaceholderState = new NodePlaceholderState(
                new NodeId("region_002_node_099"),
                BootstrapWorldScenario.CavernRegionId,
                NodeType.Combat,
                NodeState.Available,
                BootstrapWorldScenario.CavernServiceNodeId,
                CombatStandardEncounterCatalog.EnemyUnitEncounter,
                locationIdentity: LocationIdentityCatalog.EchoCaverns,
                regionMaterialYieldContent: new RegionMaterialYieldContentDefinition(1),
                supportsRegionMaterialRewards: false);

            string summaryText = NodePlaceholderScreenTextBuilder.BuildSummaryText(
                invalidPlaceholderState,
                RunLifecycleState.RunStart);

            Assert.That(summaryText, Does.Contain("Location: Echo Caverns"));
            Assert.That(summaryText, Does.Not.Contain("Revisit value:"));
        }

        [Test]
        public void BuildTitleText_ShouldUseFriendlyNodeDisplayName()
        {
            Assert.That(
                NodePlaceholderScreenTextBuilder.BuildTitleText(NodePlaceholderTestData.CreateFrontierFarmPlaceholderState()),
                Is.EqualTo("Forest Farm"));
        }

        [Test]
        public void BuildCombatContextSummaryText_ShouldStayCompactForCombatNodes()
        {
            string summaryText = NodePlaceholderScreenTextBuilder.BuildCombatContextSummaryText(
                NodePlaceholderTestData.CreateCombatPlaceholderState());

            Assert.That(summaryText, Is.EqualTo(
                "Location: Verdant Frontier\n" +
                "Encounter: Combat"));
        }

        [Test]
        public void BuildStatusText_ShouldMatchNonCombatLifecycleTexts()
        {
            NodePlaceholderState serviceState = NodePlaceholderTestData.CreateServicePlaceholderState();

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
            NodePlaceholderState combatState = NodePlaceholderTestData.CreateCombatPlaceholderState();
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
        public void BuildStatusText_ShouldShowRunTimeSkillChoicePromptWhenChoiceIsPending()
        {
            string statusText = NodePlaceholderScreenTextBuilder.BuildStatusText(
                NodePlaceholderTestData.CreateCombatPlaceholderState(),
                RunLifecycleState.RunStart,
                combatEncounterState: null,
                requiresRunTimeSkillUpgradeChoice: true);

            Assert.That(
                statusText,
                Is.EqualTo("Combat shell initialized. Choose a run-only skill upgrade to start automatic combat."));
        }

        [Test]
        public void BuildStatusText_ShouldFallbackToGenericResolvedTextWhenCombatEncounterIsMissing()
        {
            string statusText = NodePlaceholderScreenTextBuilder.BuildStatusText(
                NodePlaceholderTestData.CreateCombatPlaceholderState(),
                RunLifecycleState.RunResolved,
                combatEncounterState: null);

            Assert.That(statusText, Is.EqualTo("Run resolved. Open the post-run state to review next actions."));
        }

        [Test]
        public void ResolveAdvanceButtonState_ShouldMatchLifecycleAndCombatState()
        {
            NodePlaceholderState serviceState = NodePlaceholderTestData.CreateServicePlaceholderState();
            NodePlaceholderState combatState = NodePlaceholderTestData.CreateCombatPlaceholderState();

            AssertButtonState(
                NodePlaceholderScreenStateResolver.ResolveAdvanceButtonState(
                    serviceState,
                    RunLifecycleState.RunStart,
                    hasPendingRunTimeSkillUpgradeChoice: false,
                    hasCombatEncounterState: false),
                "Start Placeholder Run",
                true);
            AssertButtonState(
                NodePlaceholderScreenStateResolver.ResolveAdvanceButtonState(
                    combatState,
                    RunLifecycleState.RunStart,
                    hasPendingRunTimeSkillUpgradeChoice: false,
                    hasCombatEncounterState: false),
                "Combat Auto-Starting",
                false);
            AssertButtonState(
                NodePlaceholderScreenStateResolver.ResolveAdvanceButtonState(
                    serviceState,
                    RunLifecycleState.RunActive,
                    hasPendingRunTimeSkillUpgradeChoice: false,
                    hasCombatEncounterState: false),
                "Resolve Placeholder Run",
                true);
            AssertButtonState(
                NodePlaceholderScreenStateResolver.ResolveAdvanceButtonState(
                    combatState,
                    RunLifecycleState.RunActive,
                    hasPendingRunTimeSkillUpgradeChoice: false,
                    hasCombatEncounterState: true),
                "Combat Auto-Running",
                false);
            AssertButtonState(
                NodePlaceholderScreenStateResolver.ResolveAdvanceButtonState(
                    serviceState,
                    RunLifecycleState.RunResolved,
                    hasPendingRunTimeSkillUpgradeChoice: false,
                    hasCombatEncounterState: false),
                "Enter Post-Run State",
                true);
            AssertButtonState(
                NodePlaceholderScreenStateResolver.ResolveAdvanceButtonState(
                    combatState,
                    RunLifecycleState.RunResolved,
                    hasPendingRunTimeSkillUpgradeChoice: false,
                    hasCombatEncounterState: true),
                "Preparing Post-Run",
                false);
            AssertButtonState(
                NodePlaceholderScreenStateResolver.ResolveAdvanceButtonState(
                    combatState,
                    RunLifecycleState.PostRun,
                    hasPendingRunTimeSkillUpgradeChoice: false,
                    hasCombatEncounterState: true),
                "Run Lifecycle Complete",
                false);
            AssertButtonState(
                NodePlaceholderScreenStateResolver.ResolveAdvanceButtonState(
                    combatState,
                    RunLifecycleState.RunStart,
                    hasPendingRunTimeSkillUpgradeChoice: true,
                    hasCombatEncounterState: false),
                "Choose Run Upgrade",
                false);
        }

        [Test]
        public void ResolvePostRunPanelState_ShouldMatchExistingActionAvailability()
        {
            PostRunStateController postRunStateController = new PostRunStateController(
                NodePlaceholderTestData.CreateCombatPlaceholderState(),
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
        public void ShouldUseCompactCombatHeader_ShouldMatchCombatCapability()
        {
            Assert.That(
                NodePlaceholderScreenStateResolver.ShouldUseCompactCombatHeader(
                    NodePlaceholderTestData.CreateCombatPlaceholderState()),
                Is.True);
            Assert.That(
                NodePlaceholderScreenStateResolver.ShouldUseCompactCombatHeader(
                    NodePlaceholderTestData.CreateServicePlaceholderState()),
                Is.False);
        }

        [Test]
        public void PostRunSummaryTextBuilder_ShouldMatchExistingSummaryFormatting()
        {
            RunResult runResult = CreateSucceededRunResult();
            PostRunStateController postRunStateController = new PostRunStateController(
                NodePlaceholderTestData.CreateCombatPlaceholderState(),
                runResult);

            string summaryText = PostRunSummaryTextBuilder.Build(
                postRunStateController,
                runResult);

            Assert.That(summaryText, Is.EqualTo(
                "Run finished.\n" +
                "Location: Verdant Frontier\n" +
                "Node: region_001_node_004\n" +
                "Resolution: Succeeded\n" +
                "Rewards gained: Soft currency x1, Region material x1\n" +
                "Reward source: Frontier salvage\n" +
                "Milestone rewards: Persistent progression material x1\n" +
                "Progress changes: node +1 this run; tracked total 3 / 3; persistent +0; route unlock Yes\n" +
                "Next actions:\n" +
                "- Replay: Yes\n" +
                "- Return to world: Yes\n" +
                "- Stop: Yes"));
        }

        [Test]
        public void PostRunSummaryTextBuilder_ShouldIncludeCurrencyAndMaterialRewardsWhenPresent()
        {
            RunResult runResult = new RunResult(
                new NodeId("region_001_node_004"),
                RunResolutionState.Succeeded,
                new RunRewardPayload(
                    new[]
                    {
                        new RunCurrencyReward(ResourceCategory.SoftCurrency, 1),
                    },
                    new[]
                    {
                        new RunMaterialReward(ResourceCategory.RegionMaterial, 1),
                    }),
                1,
                1,
                3,
                0,
                false,
                new RunNextActionContext(
                    canReplayNode: true,
                    canChooseAnotherNode: true,
                    canStopSession: true));
            PostRunStateController postRunStateController = new PostRunStateController(
                NodePlaceholderTestData.CreateCombatPlaceholderState(),
                runResult);

            string summaryText = PostRunSummaryTextBuilder.Build(
                postRunStateController,
                runResult);

            Assert.That(summaryText, Does.Contain("Rewards gained: Soft currency x1, Region material x1"));
            Assert.That(summaryText, Does.Contain("Reward source: Frontier salvage"));
            Assert.That(summaryText, Does.Not.Contain("Milestone rewards:"));
            Assert.That(summaryText, Does.Contain("Progress changes: node +1 this run; tracked total 1 / 3; persistent +0; route unlock No"));
        }

        [Test]
        public void PostRunSummaryTextBuilder_ShouldShowUntrackedProgressWhenThresholdIsZero()
        {
            RunResult runResult = new RunResult(
                new NodeId("region_002_node_001"),
                RunResolutionState.Failed,
                RunRewardPayload.Empty,
                0,
                0,
                0,
                0,
                false,
                new RunNextActionContext(
                    canReplayNode: true,
                    canChooseAnotherNode: true,
                    canStopSession: false));
            PostRunStateController postRunStateController = new PostRunStateController(
                NodePlaceholderTestData.CreateServicePlaceholderState(),
                runResult);

            string summaryText = PostRunSummaryTextBuilder.Build(
                postRunStateController,
                runResult);

            Assert.That(summaryText, Does.Contain("Progress changes: node not tracked; persistent +0; route unlock No"));
            Assert.That(summaryText, Does.Contain("Resolution: Failed"));
            Assert.That(summaryText, Does.Contain("- Stop: No"));
        }

        [Test]
        public void ResolveAdvanceButtonState_ShouldRejectMissingPlaceholderState()
        {
            Assert.That(
                () => NodePlaceholderScreenStateResolver.ResolveAdvanceButtonState(
                    null,
                    RunLifecycleState.RunStart,
                    hasPendingRunTimeSkillUpgradeChoice: false,
                    hasCombatEncounterState: false),
                Throws.ArgumentNullException.With.Property("ParamName").EqualTo("placeholderState"));
        }

        private static void AssertButtonState(NodePlaceholderScreenButtonState buttonState, string expectedLabel, bool expectedInteractable)
        {
            Assert.That(buttonState.Label, Is.EqualTo(expectedLabel));
            Assert.That(buttonState.IsInteractable, Is.EqualTo(expectedInteractable));
        }

        private static CombatEncounterState CreateActiveCombatEncounterState()
        {
            CombatShellContextFactory combatShellContextFactory = new CombatShellContextFactory();
            CombatShellContext combatContext = combatShellContextFactory.Create(
                NodePlaceholderTestData.CreateCombatPlaceholderState(),
                null,
                null,
                default);
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
                new RunRewardPayload(
                    new[]
                    {
                        new RunCurrencyReward(ResourceCategory.SoftCurrency, 1),
                    },
                    new[]
                    {
                        new RunMaterialReward(ResourceCategory.RegionMaterial, 1),
                    },
                    System.Array.Empty<RunCurrencyReward>(),
                    new[]
                    {
                        new RunMaterialReward(ResourceCategory.PersistentProgressionMaterial, 1),
                    }),
                1,
                3,
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

