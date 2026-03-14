using NUnit.Framework;
using Survivalon.Runtime;

namespace Survivalon.Tests.EditMode
{
    public sealed class RunLifecycleControllerTests
    {
        [Test]
        public void ShouldStartInRunStartStateWithoutRunResult()
        {
            RunLifecycleController controller = CreateController();

            Assert.That(controller.CurrentState, Is.EqualTo(RunLifecycleState.RunStart));
            Assert.That(controller.HasRunResult, Is.False);
        }

        [Test]
        public void ShouldAdvanceThroughRunLifecycleAndProduceRunResult()
        {
            RunLifecycleController controller = CreateController();

            bool enteredActive = controller.TryEnterActiveState();
            bool resolved = controller.TryResolveRun(RunResolutionState.Succeeded);
            bool enteredPostRun = controller.TryEnterPostRunState();

            Assert.That(enteredActive, Is.True);
            Assert.That(resolved, Is.True);
            Assert.That(enteredPostRun, Is.True);
            Assert.That(controller.CurrentState, Is.EqualTo(RunLifecycleState.PostRun));
            Assert.That(controller.HasRunResult, Is.True);
            Assert.That(controller.RunResult.NodeId, Is.EqualTo(new NodeId("region_002_node_001")));
            Assert.That(controller.RunResult.ResolutionState, Is.EqualTo(RunResolutionState.Succeeded));
            Assert.That(controller.RunResult.RewardPayload, Is.Not.Null);
            Assert.That(controller.RunResult.RewardPayload.HasRewards, Is.False);
            Assert.That(controller.RunResult.NextActionContext.CanReplayNode, Is.True);
            Assert.That(controller.RunResult.NextActionContext.CanChooseAnotherNode, Is.True);
            Assert.That(controller.RunResult.NextActionContext.CanStopSession, Is.True);
        }

        [Test]
        public void ShouldIgnoreElapsedTimeForNonCombatRun()
        {
            RunLifecycleController controller = CreateController();

            Assert.That(controller.TryEnterActiveState(), Is.True);

            bool advanced = controller.TryAdvanceTime(1f);

            Assert.That(advanced, Is.False);
            Assert.That(controller.CurrentState, Is.EqualTo(RunLifecycleState.RunActive));
            Assert.That(controller.HasRunResult, Is.False);
        }

        [Test]
        public void ShouldCreateCombatShellContextWhenCombatRunEntersActiveState()
        {
            RunLifecycleController controller = new RunLifecycleController(CreateCombatNodeState());

            bool enteredActive = controller.TryEnterActiveState();

            Assert.That(enteredActive, Is.True);
            Assert.That(controller.HasCombatContext, Is.True);
            Assert.That(controller.HasCombatEncounterState, Is.True);
            Assert.That(controller.CombatContext.NodeId, Is.EqualTo(new NodeId("region_001_node_004")));
            Assert.That(controller.CombatContext.PlayerEntity.EntityId, Is.EqualTo(new CombatEntityId("player_main")));
            Assert.That(controller.CombatContext.PlayerEntity.DisplayName, Is.EqualTo("Player Unit"));
            Assert.That(controller.CombatContext.PlayerEntity.Side, Is.EqualTo(CombatSide.Player));
            Assert.That(controller.CombatContext.PlayerEntity.BaseStats.MaxHealth, Is.EqualTo(120f));
            Assert.That(controller.CombatContext.PlayerEntity.BaseStats.AttackPower, Is.EqualTo(14f));
            Assert.That(controller.CombatContext.PlayerEntity.BaseStats.AttackRate, Is.EqualTo(1.2f));
            Assert.That(controller.CombatContext.PlayerEntity.BaseStats.Defense, Is.EqualTo(12f));
            Assert.That(controller.CombatContext.PlayerEntity.IsAlive, Is.True);
            Assert.That(controller.CombatContext.PlayerEntity.IsActive, Is.True);
            Assert.That(controller.CombatContext.EnemyEntity.EntityId, Is.EqualTo(new CombatEntityId("region_001_node_004_enemy_001")));
            Assert.That(controller.CombatContext.EnemyEntity.DisplayName, Is.EqualTo("Enemy Unit"));
            Assert.That(controller.CombatContext.EnemyEntity.Side, Is.EqualTo(CombatSide.Enemy));
            Assert.That(controller.CombatContext.EnemyEntity.BaseStats.MaxHealth, Is.EqualTo(75f));
            Assert.That(controller.CombatContext.EnemyEntity.BaseStats.AttackPower, Is.EqualTo(8f));
            Assert.That(controller.CombatContext.EnemyEntity.BaseStats.AttackRate, Is.EqualTo(0.9f));
            Assert.That(controller.CombatContext.EnemyEntity.BaseStats.Defense, Is.EqualTo(4f));
            Assert.That(controller.CombatContext.EnemyEntity.IsAlive, Is.True);
            Assert.That(controller.CombatContext.EnemyEntity.IsActive, Is.True);
            Assert.That(controller.CombatEncounterState.PlayerEntity.CurrentHealth, Is.EqualTo(120f));
            Assert.That(controller.CombatEncounterState.EnemyEntity.CurrentHealth, Is.EqualTo(75f));
        }

        [Test]
        public void ShouldAdvanceCombatUntilCombatRunResolves()
        {
            RunLifecycleController controller = new RunLifecycleController(CreateCombatNodeState());

            Assert.That(controller.TryEnterActiveState(), Is.True);

            for (int index = 0; index < 5 && controller.CurrentState == RunLifecycleState.RunActive; index++)
            {
                controller.TryAdvanceCombat(1f);
            }

            Assert.That(controller.CurrentState, Is.EqualTo(RunLifecycleState.RunResolved));
            Assert.That(controller.HasRunResult, Is.True);
            Assert.That(controller.RunResult.ResolutionState, Is.EqualTo(RunResolutionState.Succeeded));
            Assert.That(controller.CombatEncounterState.IsResolved, Is.True);
            Assert.That(controller.CombatEncounterState.Outcome, Is.EqualTo(CombatEncounterOutcome.PlayerVictory));
            Assert.That(controller.CombatEncounterState.EnemyEntity.IsAlive, Is.False);
            Assert.That(controller.CombatEncounterState.EnemyEntity.IsActive, Is.False);
            Assert.That(controller.CombatEncounterState.HasActiveEnemy, Is.False);
            Assert.That(controller.CombatEncounterState.ActiveEnemyCount, Is.EqualTo(0));
            Assert.That(controller.CombatEncounterState.EnemyEntity.CurrentHealth, Is.EqualTo(0f));
            Assert.That(controller.CombatEncounterState.ElapsedCombatSeconds, Is.EqualTo(5f).Within(0.001f));
        }

        [Test]
        public void ShouldGrantNodeProgressWhenSuccessfulCombatRunResolvesAgainstEnemy()
        {
            PersistentWorldState worldState = new PersistentWorldState();
            RunLifecycleController controller = new RunLifecycleController(
                CreateCombatNodeState(),
                persistentWorldState: worldState);

            Assert.That(controller.TryStartAutomaticFlow(), Is.True);

            for (int index = 0; index < 24 && controller.CurrentState != RunLifecycleState.PostRun; index++)
            {
                controller.TryAdvanceAutomaticTime(0.25f);
            }

            Assert.That(controller.CurrentState, Is.EqualTo(RunLifecycleState.PostRun));
            Assert.That(controller.RunResult.NodeProgressDelta, Is.EqualTo(1));
            Assert.That(controller.RunResult.NodeProgressValue, Is.EqualTo(1));
            Assert.That(controller.RunResult.NodeProgressThreshold, Is.EqualTo(3));
            Assert.That(controller.RunResult.DidUnlockRoute, Is.False);
            Assert.That(worldState.TryGetNodeState(new NodeId("region_001_node_004"), out PersistentNodeState nodeState), Is.True);
            Assert.That(nodeState.UnlockProgress, Is.EqualTo(1));
            Assert.That(nodeState.UnlockThreshold, Is.EqualTo(3));
            Assert.That(nodeState.State, Is.EqualTo(NodeState.InProgress));
        }

        [Test]
        public void ShouldUnlockNextConnectedNodeWhenTrackedNodeClears()
        {
            BootstrapWorldMapFactory factory = new BootstrapWorldMapFactory();
            WorldGraph worldGraph = factory.CreateWorldGraph();
            PersistentWorldState worldState = factory.CreateGameState().WorldState;
            RunLifecycleController controller = new RunLifecycleController(
                CreatePushCombatNodeState(),
                worldGraph,
                persistentWorldState: worldState);

            Assert.That(worldState.TryGetNodeState(new NodeId("region_001_node_002"), out PersistentNodeState pushNodeState), Is.True);
            pushNodeState.ApplyUnlockProgress(1);
            Assert.That(pushNodeState.State, Is.EqualTo(NodeState.InProgress));
            Assert.That(pushNodeState.UnlockProgress, Is.EqualTo(2));
            Assert.That(controller.TryStartAutomaticFlow(), Is.True);

            for (int index = 0; index < 24 && controller.CurrentState != RunLifecycleState.PostRun; index++)
            {
                controller.TryAdvanceAutomaticTime(0.25f);
            }

            Assert.That(controller.CurrentState, Is.EqualTo(RunLifecycleState.PostRun));
            Assert.That(controller.RunResult.ResolutionState, Is.EqualTo(RunResolutionState.Succeeded));
            Assert.That(controller.RunResult.DidUnlockRoute, Is.True);
            Assert.That(worldState.TryGetNodeState(new NodeId("region_001_node_002"), out pushNodeState), Is.True);
            Assert.That(pushNodeState.State, Is.EqualTo(NodeState.Cleared));
            Assert.That(pushNodeState.UnlockProgress, Is.EqualTo(3));
            Assert.That(worldState.TryGetNodeState(new NodeId("region_001_node_003"), out PersistentNodeState gateNodeState), Is.True);
            Assert.That(gateNodeState.State, Is.EqualTo(NodeState.Available));
        }

        [Test]
        public void ShouldNotUnlockConnectedNodeAgainWhenClearedNodeIsReplayed()
        {
            BootstrapWorldMapFactory factory = new BootstrapWorldMapFactory();
            WorldGraph worldGraph = factory.CreateWorldGraph();
            PersistentWorldState worldState = factory.CreateGameState().WorldState;

            Assert.That(worldState.TryGetNodeState(new NodeId("region_001_node_002"), out PersistentNodeState pushNodeState), Is.True);
            pushNodeState.ApplyUnlockProgress(1);

            RunLifecycleController firstController = new RunLifecycleController(
                CreatePushCombatNodeState(),
                worldGraph,
                persistentWorldState: worldState);
            RunToPostRun(firstController);

            int nodeStateCountAfterFirstClear = worldState.NodeStates.Count;

            RunLifecycleController replayController = new RunLifecycleController(
                CreatePushCombatNodeState(),
                worldGraph,
                persistentWorldState: worldState);
            RunToPostRun(replayController);

            Assert.That(firstController.RunResult.DidUnlockRoute, Is.True);
            Assert.That(replayController.RunResult.DidUnlockRoute, Is.False);
            Assert.That(worldState.NodeStates.Count, Is.EqualTo(nodeStateCountAfterFirstClear));
            Assert.That(worldState.TryGetNodeState(new NodeId("region_001_node_003"), out PersistentNodeState gateNodeState), Is.True);
            Assert.That(gateNodeState.State, Is.EqualTo(NodeState.Available));
        }

        [Test]
        public void ShouldStartAutomaticCombatFlowWithoutAdvancingTime()
        {
            RunLifecycleController controller = new RunLifecycleController(CreateCombatNodeState());

            Assert.That(controller.TryStartAutomaticFlow(), Is.True);
            Assert.That(controller.CurrentState, Is.EqualTo(RunLifecycleState.RunActive));
            Assert.That(controller.HasRunResult, Is.False);
            Assert.That(controller.CombatEncounterState.ElapsedCombatSeconds, Is.EqualTo(0f).Within(0.001f));
        }

        [Test]
        public void ShouldIgnoreZeroElapsedAutomaticTimeProgression()
        {
            RunLifecycleController controller = new RunLifecycleController(CreateCombatNodeState());

            controller.TryStartAutomaticFlow();

            bool changed = controller.TryAdvanceAutomaticTime(0f);

            Assert.That(changed, Is.False);
            Assert.That(controller.CurrentState, Is.EqualTo(RunLifecycleState.RunActive));
            Assert.That(controller.CombatEncounterState.ElapsedCombatSeconds, Is.EqualTo(0f).Within(0.001f));
        }

        [Test]
        public void ShouldAdvanceAutomaticCombatFlowFromNodeEntryToPostRun()
        {
            PersistentWorldState worldState = new PersistentWorldState();
            RunLifecycleController controller = new RunLifecycleController(
                CreateCombatNodeState(),
                persistentWorldState: worldState);

            Assert.That(controller.TryStartAutomaticFlow(), Is.True);
            Assert.That(controller.CurrentState, Is.EqualTo(RunLifecycleState.RunActive));

            for (int index = 0; index < 24 && controller.CurrentState != RunLifecycleState.PostRun; index++)
            {
                controller.TryAdvanceAutomaticTime(0.25f);
            }

            Assert.That(controller.CurrentState, Is.EqualTo(RunLifecycleState.PostRun));
            Assert.That(controller.HasRunResult, Is.True);
            Assert.That(controller.RunResult.ResolutionState, Is.EqualTo(RunResolutionState.Succeeded));
            Assert.That(controller.CombatEncounterState.Outcome, Is.EqualTo(CombatEncounterOutcome.PlayerVictory));
            Assert.That(controller.RunResult.NodeProgressDelta, Is.EqualTo(1));
            Assert.That(controller.RunResult.NodeProgressValue, Is.EqualTo(1));
            Assert.That(controller.RunResult.NodeProgressThreshold, Is.EqualTo(3));
        }

        [Test]
        public void ShouldResolveCombatRunAsFailedWhenPlayerIsDefeated()
        {
            RunLifecycleController controller = new RunLifecycleController(CreateBossCombatNodeState());

            Assert.That(controller.TryEnterActiveState(), Is.True);

            for (int index = 0; index < 12 && controller.CurrentState == RunLifecycleState.RunActive; index++)
            {
                controller.TryAdvanceCombat(1f);
            }

            Assert.That(controller.CurrentState, Is.EqualTo(RunLifecycleState.RunResolved));
            Assert.That(controller.HasRunResult, Is.True);
            Assert.That(controller.RunResult.ResolutionState, Is.EqualTo(RunResolutionState.Failed));
            Assert.That(controller.CombatEncounterState.IsResolved, Is.True);
            Assert.That(controller.CombatEncounterState.Outcome, Is.EqualTo(CombatEncounterOutcome.EnemyVictory));
            Assert.That(controller.CombatEncounterState.PlayerEntity.IsAlive, Is.False);
            Assert.That(controller.CombatEncounterState.PlayerEntity.IsActive, Is.False);
            Assert.That(controller.CombatEncounterState.HasActivePlayer, Is.False);
            Assert.That(controller.CombatEncounterState.ActivePlayerCount, Is.EqualTo(0));
        }

        [Test]
        public void ShouldAdvanceAutomaticHostileCombatFlowToFailedPostRun()
        {
            PersistentWorldState worldState = new PersistentWorldState();
            RunLifecycleController controller = new RunLifecycleController(
                CreateBossCombatNodeState(),
                persistentWorldState: worldState);

            Assert.That(controller.TryStartAutomaticFlow(), Is.True);
            Assert.That(controller.CurrentState, Is.EqualTo(RunLifecycleState.RunActive));

            for (int index = 0; index < 64 && controller.CurrentState != RunLifecycleState.PostRun; index++)
            {
                controller.TryAdvanceAutomaticTime(0.25f);
            }

            Assert.That(controller.CurrentState, Is.EqualTo(RunLifecycleState.PostRun));
            Assert.That(controller.HasRunResult, Is.True);
            Assert.That(controller.RunResult.ResolutionState, Is.EqualTo(RunResolutionState.Failed));
            Assert.That(controller.CombatEncounterState.Outcome, Is.EqualTo(CombatEncounterOutcome.EnemyVictory));
            Assert.That(controller.RunResult.NodeProgressDelta, Is.EqualTo(0));
            Assert.That(controller.RunResult.NodeProgressValue, Is.EqualTo(0));
            Assert.That(controller.RunResult.NodeProgressThreshold, Is.EqualTo(3));
        }

        [Test]
        public void ShouldKeepFailedTrackedCombatNodeAtZeroProgressAndNotUnlockRouteInCurrentOneVsOneMvp()
        {
            PersistentWorldState worldState = new PersistentWorldState();
            RunLifecycleController controller = new RunLifecycleController(
                CreateBossCombatNodeState(),
                persistentWorldState: worldState);

            RunToPostRun(controller);

            Assert.That(controller.RunResult.ResolutionState, Is.EqualTo(RunResolutionState.Failed));
            Assert.That(controller.RunResult.NodeProgressDelta, Is.EqualTo(0));
            Assert.That(controller.RunResult.NodeProgressValue, Is.EqualTo(0));
            Assert.That(controller.RunResult.NodeProgressThreshold, Is.EqualTo(3));
            Assert.That(controller.RunResult.DidUnlockRoute, Is.False);
            Assert.That(worldState.TryGetNodeState(new NodeId("region_001_node_005"), out PersistentNodeState nodeState), Is.True);
            Assert.That(nodeState.State, Is.EqualTo(NodeState.Available));
            Assert.That(nodeState.UnlockProgress, Is.EqualTo(0));
            Assert.That(nodeState.UnlockThreshold, Is.EqualTo(3));
        }

        [Test]
        public void ShouldKeepFailedTrackedCombatReplayAtZeroProgressInCurrentOneVsOneMvp()
        {
            PersistentWorldState worldState = new PersistentWorldState();
            RunLifecycleController firstController = new RunLifecycleController(
                CreateBossCombatNodeState(),
                persistentWorldState: worldState);
            RunLifecycleController replayController = new RunLifecycleController(
                CreateBossCombatNodeState(),
                persistentWorldState: worldState);

            RunToPostRun(firstController);
            RunToPostRun(replayController);

            Assert.That(firstController.RunResult.ResolutionState, Is.EqualTo(RunResolutionState.Failed));
            Assert.That(replayController.RunResult.ResolutionState, Is.EqualTo(RunResolutionState.Failed));
            Assert.That(worldState.TryGetNodeState(new NodeId("region_001_node_005"), out PersistentNodeState nodeState), Is.True);
            Assert.That(nodeState.State, Is.EqualTo(NodeState.Available));
            Assert.That(nodeState.UnlockProgress, Is.EqualTo(0));
            Assert.That(nodeState.UnlockThreshold, Is.EqualTo(3));
        }

        [Test]
        public void ShouldStopAutomaticCombatFlowAfterPostRunIsReached()
        {
            RunLifecycleController controller = new RunLifecycleController(CreateCombatNodeState());

            controller.TryStartAutomaticFlow();
            for (int index = 0; index < 24 && controller.CurrentState != RunLifecycleState.PostRun; index++)
            {
                controller.TryAdvanceAutomaticTime(0.25f);
            }

            float resolvedElapsedSeconds = controller.CombatEncounterState.ElapsedCombatSeconds;
            bool advancedAfterPostRun = controller.TryAdvanceAutomaticTime(1f);

            Assert.That(controller.CurrentState, Is.EqualTo(RunLifecycleState.PostRun));
            Assert.That(advancedAfterPostRun, Is.False);
            Assert.That(controller.CombatEncounterState.ElapsedCombatSeconds, Is.EqualTo(resolvedElapsedSeconds).Within(0.001f));
        }

        [Test]
        public void ShouldRejectManualCombatResolutionBeforeCombatOutcomeIsReached()
        {
            RunLifecycleController controller = new RunLifecycleController(CreateCombatNodeState());

            Assert.That(controller.TryEnterActiveState(), Is.True);

            bool resolved = controller.TryResolveRun(RunResolutionState.Succeeded);

            Assert.That(resolved, Is.False);
            Assert.That(controller.CurrentState, Is.EqualTo(RunLifecycleState.RunActive));
            Assert.That(controller.HasRunResult, Is.False);
        }

        [Test]
        public void ShouldEnterPostRunStateOnlyAfterResolvedRun()
        {
            RunLifecycleController controller = CreateController();

            controller.TryEnterActiveState();
            controller.TryResolveRun(RunResolutionState.Succeeded);

            bool enteredPostRun = controller.TryEnterPostRunState();

            Assert.That(enteredPostRun, Is.True);
            Assert.That(controller.CurrentState, Is.EqualTo(RunLifecycleState.PostRun));
        }

        [Test]
        public void ShouldRejectOutOfOrderLifecycleTransitions()
        {
            RunLifecycleController controller = CreateController();

            bool resolvedBeforeActive = controller.TryResolveRun(RunResolutionState.Succeeded);
            bool enteredPostRunBeforeResolved = controller.TryEnterPostRunState();

            Assert.That(resolvedBeforeActive, Is.False);
            Assert.That(enteredPostRunBeforeResolved, Is.False);
            Assert.That(controller.CurrentState, Is.EqualTo(RunLifecycleState.RunStart));
        }

        private static RunLifecycleController CreateController()
        {
            return new RunLifecycleController(new NodePlaceholderState(
                new NodeId("region_002_node_001"),
                new RegionId("region_002"),
                NodeType.ServiceOrProgression,
                NodeState.Available,
                new NodeId("region_001_node_002")));
        }

        private static NodePlaceholderState CreateCombatNodeState()
        {
            return new NodePlaceholderState(
                new NodeId("region_001_node_004"),
                new RegionId("region_001"),
                NodeType.Combat,
                NodeState.Available,
                new NodeId("region_001_node_002"));
        }

        private static NodePlaceholderState CreateBossCombatNodeState()
        {
            return new NodePlaceholderState(
                new NodeId("region_001_node_005"),
                new RegionId("region_001"),
                NodeType.BossOrGate,
                NodeState.Available,
                new NodeId("region_001_node_004"));
        }

        private static NodePlaceholderState CreatePushCombatNodeState()
        {
            return new NodePlaceholderState(
                new NodeId("region_001_node_002"),
                new RegionId("region_001"),
                NodeType.Combat,
                NodeState.InProgress,
                new NodeId("region_001_node_001"));
        }

        private static void RunToPostRun(RunLifecycleController controller, int maxStepCount = 128)
        {
            Assert.That(controller.TryStartAutomaticFlow(), Is.True);

            for (int index = 0; index < maxStepCount && controller.CurrentState != RunLifecycleState.PostRun; index++)
            {
                controller.TryAdvanceAutomaticTime(0.25f);
            }

            Assert.That(controller.CurrentState, Is.EqualTo(RunLifecycleState.PostRun));
        }
    }
}
