using NUnit.Framework;
using Survivalon.Core;
using Survivalon.Data.Characters;
using Survivalon.Data.Gear;
using Survivalon.Startup;
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
            Assert.That(startupState.GameState.CharacterStates, Has.Count.EqualTo(2));
            AssertCharacterState(
                startupState.GameState,
                "character_vanguard",
                true,
                PlayableCharacterSkillPackageIds.VanguardDefault);
            AssertCharacterState(
                startupState.GameState,
                "character_striker",
                false,
                PlayableCharacterSkillPackageIds.StrikerDefault);
            Assert.That(startupState.GameState.OwnedGearIds, Has.Count.EqualTo(1));
            Assert.That(startupState.GameState.OwnedGearIds[0], Is.EqualTo(GearIds.TrainingBlade));
        }

        [Test]
        public void ShouldUsePersistedGameStateAndPreserveExistingValidSelectionWhenStorageHasSavedState()
        {
            MemoryPersistentGameStateStorage storage = new MemoryPersistentGameStateStorage();
            PersistentGameState persistedGameState = new PersistentGameState();
            persistedGameState.WorldState.SetCurrentNode(new NodeId("region_002_node_001"));
            persistedGameState.WorldState.SetLastSafeNode(new NodeId("region_001_node_001"));
            persistedGameState.SafeResumeState.MarkWorldMap(new NodeId("region_002_node_001"));
            persistedGameState.AddCharacterState(new PersistentCharacterState(
                "character_striker",
                isUnlocked: true,
                isSelectable: true,
                isActive: true,
                skillPackageId: PlayableCharacterSkillPackageIds.StrikerDefault));
            storage.Seed(persistedGameState);

            SafeResumePersistenceService persistenceService = new SafeResumePersistenceService(storage);
            BootstrapStartupStateFactory stateFactory = new BootstrapStartupStateFactory(persistenceService);

            BootstrapStartupState startupState = stateFactory.Create(new BootstrapWorldMapFactory());

            Assert.That(startupState.GameState, Is.SameAs(persistedGameState));
            Assert.That(startupState.EntryTarget, Is.EqualTo(StartupEntryTarget.WorldViewPlaceholder));
            Assert.That(startupState.SessionContext.HasRecentNode, Is.True);
            Assert.That(startupState.SessionContext.RecentNodeId, Is.EqualTo(new NodeId("region_002_node_001")));
            Assert.That(startupState.GameState.CharacterStates, Has.Count.EqualTo(2));
            AssertCharacterState(
                startupState.GameState,
                "character_vanguard",
                false,
                PlayableCharacterSkillPackageIds.VanguardDefault);
            AssertCharacterState(
                startupState.GameState,
                "character_striker",
                true,
                PlayableCharacterSkillPackageIds.StrikerDefault);
            Assert.That(startupState.GameState.OwnedGearIds, Does.Contain(GearIds.TrainingBlade));
        }

        [Test]
        public void ShouldNormalizePlayableCharacterSelectionWhenPersistedStateHasNoActiveSelectableCharacter()
        {
            MemoryPersistentGameStateStorage storage = new MemoryPersistentGameStateStorage();
            PersistentGameState persistedGameState = new PersistentGameState();
            persistedGameState.WorldState.SetCurrentNode(new NodeId("region_002_node_001"));
            persistedGameState.WorldState.SetLastSafeNode(new NodeId("region_001_node_001"));
            persistedGameState.SafeResumeState.MarkWorldMap(new NodeId("region_002_node_001"));
            persistedGameState.AddCharacterState(new PersistentCharacterState(
                "character_vanguard",
                isUnlocked: true,
                isSelectable: true,
                isActive: false,
                skillPackageId: PlayableCharacterSkillPackageIds.VanguardDefault));
            persistedGameState.AddCharacterState(new PersistentCharacterState(
                "character_striker",
                isUnlocked: true,
                isSelectable: true,
                isActive: false,
                skillPackageId: PlayableCharacterSkillPackageIds.StrikerDefault));
            storage.Seed(persistedGameState);

            SafeResumePersistenceService persistenceService = new SafeResumePersistenceService(storage);
            BootstrapStartupStateFactory stateFactory = new BootstrapStartupStateFactory(persistenceService);

            BootstrapStartupState startupState = stateFactory.Create(new BootstrapWorldMapFactory());

            Assert.That(startupState.GameState, Is.SameAs(persistedGameState));
            Assert.That(startupState.GameState.CharacterStates, Has.Count.EqualTo(2));
            AssertCharacterState(
                startupState.GameState,
                "character_vanguard",
                true,
                PlayableCharacterSkillPackageIds.VanguardDefault);
            AssertCharacterState(
                startupState.GameState,
                "character_striker",
                false,
                PlayableCharacterSkillPackageIds.StrikerDefault);
        }

        private static void AssertCharacterState(
            PersistentGameState gameState,
            string characterId,
            bool isActive,
            string expectedSkillPackageId)
        {
            Assert.That(gameState.TryGetCharacterState(characterId, out PersistentCharacterState characterState), Is.True);
            Assert.That(characterState.IsUnlocked, Is.True);
            Assert.That(characterState.IsSelectable, Is.True);
            Assert.That(characterState.IsActive, Is.EqualTo(isActive));
            Assert.That(characterState.SkillPackageId, Is.EqualTo(expectedSkillPackageId));
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
