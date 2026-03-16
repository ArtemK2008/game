using NUnit.Framework;
using Survivalon.Startup;
using Survivalon.Core;
using Survivalon.State.Persistence;
using Survivalon.World;

namespace Survivalon.Tests.EditMode.Startup
{
    public sealed class BootstrapStartupStateFactoryTests
    {
        [Test]
        public void ShouldCreateWorldEntryStateFromFallbackGameState()
        {
            MemoryPersistentGameStateStorage storage = new MemoryPersistentGameStateStorage();
            SafeResumePersistenceService persistenceService = new SafeResumePersistenceService(storage);
            BootstrapWorldMapFactory worldMapFactory = new BootstrapWorldMapFactory();
            BootstrapStartupStateFactory stateFactory = new BootstrapStartupStateFactory(persistenceService);

            BootstrapStartupState startupState = stateFactory.Create(worldMapFactory);

            Assert.That(startupState.WorldGraph, Is.Not.Null);
            Assert.That(startupState.GameState, Is.Not.Null);
            Assert.That(startupState.NodeEntryFlowController, Is.Not.Null);
            Assert.That(startupState.SessionContext, Is.Not.Null);
            Assert.That(startupState.EntryTarget, Is.EqualTo(StartupEntryTarget.WorldViewPlaceholder));
            Assert.That(startupState.GameState.WorldState.HasCurrentNode, Is.True);
            Assert.That(startupState.SessionContext.HasRecentNode, Is.True);
            Assert.That(startupState.SessionContext.RecentNodeId, Is.EqualTo(startupState.GameState.WorldState.CurrentNodeId));
            Assert.That(startupState.GameState.CharacterStates, Has.Count.EqualTo(1));
            Assert.That(startupState.GameState.CharacterStates[0].CharacterId, Is.EqualTo("character_vanguard"));
            Assert.That(startupState.GameState.CharacterStates[0].IsUnlocked, Is.True);
            Assert.That(startupState.GameState.CharacterStates[0].IsSelectable, Is.True);
            Assert.That(startupState.GameState.CharacterStates[0].IsActive, Is.True);
        }

        [Test]
        public void ShouldUsePersistedGameStateWhenStorageHasSavedState()
        {
            MemoryPersistentGameStateStorage storage = new MemoryPersistentGameStateStorage();
            PersistentGameState persistedGameState = new PersistentGameState();
            persistedGameState.WorldState.SetCurrentNode(new NodeId("region_002_node_001"));
            persistedGameState.WorldState.SetLastSafeNode(new NodeId("region_001_node_001"));
            persistedGameState.SafeResumeState.MarkWorldMap(new NodeId("region_002_node_001"));
            storage.Seed(persistedGameState);

            SafeResumePersistenceService persistenceService = new SafeResumePersistenceService(storage);
            BootstrapStartupStateFactory stateFactory = new BootstrapStartupStateFactory(persistenceService);

            BootstrapStartupState startupState = stateFactory.Create(new BootstrapWorldMapFactory());

            Assert.That(startupState.GameState, Is.SameAs(persistedGameState));
            Assert.That(startupState.EntryTarget, Is.EqualTo(StartupEntryTarget.WorldViewPlaceholder));
            Assert.That(startupState.SessionContext.HasRecentNode, Is.True);
            Assert.That(startupState.SessionContext.RecentNodeId, Is.EqualTo(new NodeId("region_002_node_001")));
            Assert.That(startupState.GameState.CharacterStates, Has.Count.EqualTo(1));
            Assert.That(startupState.GameState.CharacterStates[0].CharacterId, Is.EqualTo("character_vanguard"));
            Assert.That(startupState.GameState.CharacterStates[0].IsActive, Is.True);
        }

        private sealed class MemoryPersistentGameStateStorage : IPersistentGameStateStorage
        {
            private PersistentGameState savedGameState;

            public void Seed(PersistentGameState gameState)
            {
                savedGameState = gameState;
            }

            public void Save(PersistentGameState gameState)
            {
                savedGameState = gameState;
            }

            public bool TryLoad(out PersistentGameState gameState)
            {
                gameState = savedGameState;
                return gameState != null;
            }
        }
    }
}

