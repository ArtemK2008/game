using NUnit.Framework;
using Survivalon.Runtime;
using Survivalon.Runtime.Startup;
using Survivalon.Runtime.Core;
using Survivalon.Runtime.Run;
using Survivalon.Runtime.State;
using Survivalon.Runtime.State.Persistence;
using Survivalon.Runtime.World;
using Survivalon.Tests.EditMode.World;

namespace Survivalon.Tests.EditMode.Startup
{
    public sealed class BootstrapPostRunTransitionServiceTests
    {
        [Test]
        public void ShouldPrepareReturnToWorldByRecordingSessionAndSavingResolvedWorldContext()
        {
            PersistentGameState gameState = new PersistentGameState();
            gameState.WorldState.SetCurrentNode(new NodeId("region_002_node_001"));
            SessionContextState sessionContext = new SessionContextState();
            MemoryPersistentGameStateStorage storage = new MemoryPersistentGameStateStorage();
            BootstrapPostRunTransitionService transitionService = new BootstrapPostRunTransitionService(
                new SafeResumePersistenceService(storage));

            StartupEntryTarget entryTarget = transitionService.PrepareReturnToWorld(
                gameState,
                sessionContext,
                CreateRunResult(new NodeId("region_002_node_001")));

            Assert.That(entryTarget, Is.EqualTo(StartupEntryTarget.WorldViewPlaceholder));
            Assert.That(sessionContext.HasRecentNode, Is.True);
            Assert.That(sessionContext.RecentNodeId, Is.EqualTo(new NodeId("region_002_node_001")));
            Assert.That(storage.SavedGameState, Is.Not.Null);
            Assert.That(storage.SavedGameState.SafeResumeState.HasSafeResumeTarget, Is.True);
            Assert.That(storage.SavedGameState.SafeResumeState.TargetType, Is.EqualTo(SafeResumeTargetType.WorldMap));
            Assert.That(storage.SavedGameState.SafeResumeState.ResumeNodeId, Is.EqualTo(new NodeId("region_002_node_001")));
        }

        [Test]
        public void ShouldPrepareStopSessionByRecordingSessionAndSavingResolvedWorldContext()
        {
            PersistentGameState gameState = new PersistentGameState();
            gameState.WorldState.SetCurrentNode(new NodeId("region_001_node_004"));
            SessionContextState sessionContext = new SessionContextState();
            MemoryPersistentGameStateStorage storage = new MemoryPersistentGameStateStorage();
            BootstrapPostRunTransitionService transitionService = new BootstrapPostRunTransitionService(
                new SafeResumePersistenceService(storage));

            StartupEntryTarget entryTarget = transitionService.PrepareStopSession(
                gameState,
                sessionContext,
                CreateRunResult(new NodeId("region_001_node_004")));

            Assert.That(entryTarget, Is.EqualTo(StartupEntryTarget.MainMenuPlaceholder));
            Assert.That(sessionContext.HasRecentNode, Is.True);
            Assert.That(sessionContext.RecentNodeId, Is.EqualTo(new NodeId("region_001_node_004")));
            Assert.That(storage.SavedGameState, Is.Not.Null);
            Assert.That(storage.SavedGameState.SafeResumeState.HasSafeResumeTarget, Is.True);
            Assert.That(storage.SavedGameState.SafeResumeState.ResumeNodeId, Is.EqualTo(new NodeId("region_001_node_004")));
        }

        private static RunResult CreateRunResult(NodeId nodeId)
        {
            return new RunResult(
                nodeId,
                RunResolutionState.Succeeded,
                RunRewardPayload.Empty,
                0,
                0,
                0,
                0,
                false,
                new RunNextActionContext(
                    canReplayNode: true,
                    canChooseAnotherNode: true,
                    canStopSession: true));
        }

        private sealed class MemoryPersistentGameStateStorage : IPersistentGameStateStorage
        {
            public PersistentGameState SavedGameState { get; private set; }

            public void Save(PersistentGameState gameState)
            {
                SavedGameState = gameState;
            }

            public bool TryLoad(out PersistentGameState gameState)
            {
                gameState = SavedGameState;
                return gameState != null;
            }
        }
    }
}
