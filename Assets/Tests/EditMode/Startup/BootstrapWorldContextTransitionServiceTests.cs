using NUnit.Framework;
using Survivalon.Startup;
using Survivalon.Core;
using Survivalon.State;
using Survivalon.State.Persistence;

namespace Survivalon.Tests.EditMode.Startup
{
    public sealed class BootstrapWorldContextTransitionServiceTests
    {
        [Test]
        public void ShouldPrepareReturnToWorldByRecordingSessionAndSavingResolvedWorldContext()
        {
            PersistentGameState gameState = new PersistentGameState();
            gameState.WorldState.SetCurrentNode(new NodeId("region_002_node_001"));
            SessionContextState sessionContext = new SessionContextState();
            MemoryPersistentGameStateStorage storage = new MemoryPersistentGameStateStorage();
            BootstrapWorldContextTransitionService transitionService = new BootstrapWorldContextTransitionService(
                new SafeResumePersistenceService(storage));

            StartupEntryTarget entryTarget = transitionService.PrepareReturnToWorld(
                gameState,
                sessionContext,
                new NodeId("region_002_node_001"));

            Assert.That(entryTarget, Is.EqualTo(StartupEntryTarget.WorldViewPlaceholder));
            Assert.That(sessionContext.HasRecentNode, Is.True);
            Assert.That(sessionContext.RecentNodeId, Is.EqualTo(new NodeId("region_002_node_001")));
            Assert.That(sessionContext.HasReturnToWorldReentryOffer, Is.True);
            Assert.That(sessionContext.ReturnToWorldReentryNodeId, Is.EqualTo(new NodeId("region_002_node_001")));
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
            BootstrapWorldContextTransitionService transitionService = new BootstrapWorldContextTransitionService(
                new SafeResumePersistenceService(storage));

            StartupEntryTarget entryTarget = transitionService.PrepareStopSession(
                gameState,
                sessionContext,
                new NodeId("region_001_node_004"));

            Assert.That(entryTarget, Is.EqualTo(StartupEntryTarget.MainMenuPlaceholder));
            Assert.That(sessionContext.HasRecentNode, Is.True);
            Assert.That(sessionContext.RecentNodeId, Is.EqualTo(new NodeId("region_001_node_004")));
            Assert.That(sessionContext.HasReturnToWorldReentryOffer, Is.False);
            Assert.That(storage.SavedGameState, Is.Not.Null);
            Assert.That(storage.SavedGameState.SafeResumeState.HasSafeResumeTarget, Is.True);
            Assert.That(storage.SavedGameState.SafeResumeState.ResumeNodeId, Is.EqualTo(new NodeId("region_001_node_004")));
        }

        [Test]
        public void ShouldPersistResolvedPostRunBoundaryWithoutChangingEntryTargetFlow()
        {
            PersistentGameState gameState = new PersistentGameState();
            gameState.WorldState.SetCurrentNode(new NodeId("region_001_node_004"));
            gameState.WorldState.SetLastSafeNode(new NodeId("region_001_node_002"));
            gameState.ResourceBalances.Add(ResourceCategory.SoftCurrency, 2);
            MemoryPersistentGameStateStorage storage = new MemoryPersistentGameStateStorage();
            BootstrapWorldContextTransitionService transitionService = new BootstrapWorldContextTransitionService(
                new SafeResumePersistenceService(storage));

            transitionService.PersistResolvedPostRunBoundary(gameState);

            Assert.That(storage.SavedGameState, Is.Not.Null);
            Assert.That(storage.SavedGameState.SafeResumeState.HasSafeResumeTarget, Is.True);
            Assert.That(storage.SavedGameState.SafeResumeState.TargetType, Is.EqualTo(SafeResumeTargetType.WorldMap));
            Assert.That(storage.SavedGameState.SafeResumeState.ResumeNodeId, Is.EqualTo(new NodeId("region_001_node_004")));
            Assert.That(storage.SavedGameState.ResourceBalances.GetAmount(ResourceCategory.SoftCurrency), Is.EqualTo(2));
        }

        [Test]
        public void ShouldPrepareTownServiceStopSessionByRecordingSessionAndSavingResolvedTownContext()
        {
            PersistentGameState gameState = new PersistentGameState();
            gameState.WorldState.SetCurrentNode(new NodeId("region_002_node_001"));
            SessionContextState sessionContext = new SessionContextState();
            MemoryPersistentGameStateStorage storage = new MemoryPersistentGameStateStorage();
            BootstrapWorldContextTransitionService transitionService = new BootstrapWorldContextTransitionService(
                new SafeResumePersistenceService(storage));

            StartupEntryTarget entryTarget = transitionService.PrepareStopSessionFromTownService(
                gameState,
                sessionContext,
                new NodeId("region_002_node_001"));

            Assert.That(entryTarget, Is.EqualTo(StartupEntryTarget.MainMenuPlaceholder));
            Assert.That(sessionContext.HasRecentNode, Is.True);
            Assert.That(sessionContext.RecentNodeId, Is.EqualTo(new NodeId("region_002_node_001")));
            Assert.That(sessionContext.HasReturnToWorldReentryOffer, Is.False);
            Assert.That(storage.SavedGameState, Is.Not.Null);
            Assert.That(storage.SavedGameState.SafeResumeState.HasSafeResumeTarget, Is.True);
            Assert.That(storage.SavedGameState.SafeResumeState.TargetType, Is.EqualTo(SafeResumeTargetType.TownService));
            Assert.That(storage.SavedGameState.SafeResumeState.ResumeNodeId, Is.EqualTo(new NodeId("region_002_node_001")));
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

