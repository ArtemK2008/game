using NUnit.Framework;
using Survivalon.Runtime;

namespace Survivalon.Tests.EditMode
{
    public sealed class RunLifecycleControllerProgressionTests
    {
        [Test]
        public void ShouldGrantNodeProgressWhenSuccessfulCombatRunResolvesAgainstEnemy()
        {
            PersistentWorldState worldState = new PersistentWorldState();
            RunLifecycleController controller = new RunLifecycleController(
                RunLifecycleControllerTestData.CreateCombatNodeState(),
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
            WorldGraph worldGraph = BootstrapWorldTestData.CreateWorldGraph();
            PersistentWorldState worldState = BootstrapWorldTestData.CreateWorldState();
            RunLifecycleController controller = new RunLifecycleController(
                RunLifecycleControllerTestData.CreatePushCombatNodeState(),
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
            WorldGraph worldGraph = BootstrapWorldTestData.CreateWorldGraph();
            PersistentWorldState worldState = BootstrapWorldTestData.CreateWorldState();

            Assert.That(worldState.TryGetNodeState(new NodeId("region_001_node_002"), out PersistentNodeState pushNodeState), Is.True);
            pushNodeState.ApplyUnlockProgress(1);

            RunLifecycleController firstController = new RunLifecycleController(
                RunLifecycleControllerTestData.CreatePushCombatNodeState(),
                worldGraph,
                persistentWorldState: worldState);
            RunLifecycleControllerTestData.RunToPostRun(firstController);

            int nodeStateCountAfterFirstClear = worldState.NodeStates.Count;

            RunLifecycleController replayController = new RunLifecycleController(
                RunLifecycleControllerTestData.CreatePushCombatNodeState(),
                worldGraph,
                persistentWorldState: worldState);
            RunLifecycleControllerTestData.RunToPostRun(replayController);

            Assert.That(firstController.RunResult.DidUnlockRoute, Is.True);
            Assert.That(replayController.RunResult.DidUnlockRoute, Is.False);
            Assert.That(worldState.NodeStates.Count, Is.EqualTo(nodeStateCountAfterFirstClear));
            Assert.That(worldState.TryGetNodeState(new NodeId("region_001_node_003"), out PersistentNodeState gateNodeState), Is.True);
            Assert.That(gateNodeState.State, Is.EqualTo(NodeState.Available));
        }

        [Test]
        public void ShouldKeepFailedTrackedCombatNodeAtZeroProgressAndNotUnlockRouteInCurrentOneVsOneMvp()
        {
            PersistentWorldState worldState = new PersistentWorldState();
            RunLifecycleController controller = new RunLifecycleController(
                RunLifecycleControllerTestData.CreateBossCombatNodeState(),
                persistentWorldState: worldState);

            RunLifecycleControllerTestData.RunToPostRun(controller);

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
                RunLifecycleControllerTestData.CreateBossCombatNodeState(),
                persistentWorldState: worldState);
            RunLifecycleController replayController = new RunLifecycleController(
                RunLifecycleControllerTestData.CreateBossCombatNodeState(),
                persistentWorldState: worldState);

            RunLifecycleControllerTestData.RunToPostRun(firstController);
            RunLifecycleControllerTestData.RunToPostRun(replayController);

            Assert.That(firstController.RunResult.ResolutionState, Is.EqualTo(RunResolutionState.Failed));
            Assert.That(replayController.RunResult.ResolutionState, Is.EqualTo(RunResolutionState.Failed));
            Assert.That(worldState.TryGetNodeState(new NodeId("region_001_node_005"), out PersistentNodeState nodeState), Is.True);
            Assert.That(nodeState.State, Is.EqualTo(NodeState.Available));
            Assert.That(nodeState.UnlockProgress, Is.EqualTo(0));
            Assert.That(nodeState.UnlockThreshold, Is.EqualTo(3));
        }
    }
}
