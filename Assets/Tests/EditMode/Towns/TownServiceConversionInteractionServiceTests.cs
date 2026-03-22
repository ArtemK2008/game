using NUnit.Framework;
using Survivalon.Core;
using Survivalon.Data.Towns;
using Survivalon.State.Persistence;
using Survivalon.Tests.EditMode.World;
using Survivalon.Towns;

namespace Survivalon.Tests.EditMode.Towns
{
    public sealed class TownServiceConversionInteractionServiceTests
    {
        [Test]
        public void ShouldConvertRegionMaterialIntoPersistentProgressionMaterialAndPersistImmediately()
        {
            MemoryPersistentGameStateStorage storage = new MemoryPersistentGameStateStorage();
            SafeResumePersistenceService persistenceService = new SafeResumePersistenceService(storage);
            TownServiceConversionInteractionService interactionService =
                new TownServiceConversionInteractionService(persistenceService);
            PersistentGameState gameState = BootstrapWorldTestData.CreateGameState();
            gameState.ResourceBalances.Add(ResourceCategory.RegionMaterial, 3);

            bool converted = interactionService.TryConvert(gameState, TownServiceConversionId.RegionMaterialRefinement);

            Assert.That(converted, Is.True);
            Assert.That(gameState.ResourceBalances.GetAmount(ResourceCategory.RegionMaterial), Is.EqualTo(0));
            Assert.That(
                gameState.ResourceBalances.GetAmount(ResourceCategory.PersistentProgressionMaterial),
                Is.EqualTo(1));
            Assert.That(storage.SavedGameState, Is.Not.Null);
            Assert.That(storage.SavedGameState.ResourceBalances.GetAmount(ResourceCategory.RegionMaterial), Is.EqualTo(0));
            Assert.That(
                storage.SavedGameState.ResourceBalances.GetAmount(ResourceCategory.PersistentProgressionMaterial),
                Is.EqualTo(1));
        }

        [Test]
        public void ShouldRejectConversionWhenRegionMaterialIsInsufficient()
        {
            MemoryPersistentGameStateStorage storage = new MemoryPersistentGameStateStorage();
            SafeResumePersistenceService persistenceService = new SafeResumePersistenceService(storage);
            TownServiceConversionInteractionService interactionService =
                new TownServiceConversionInteractionService(persistenceService);
            PersistentGameState gameState = BootstrapWorldTestData.CreateGameState();
            gameState.ResourceBalances.Add(ResourceCategory.RegionMaterial, 2);

            bool converted = interactionService.TryConvert(gameState, TownServiceConversionId.RegionMaterialRefinement);

            Assert.That(converted, Is.False);
            Assert.That(gameState.ResourceBalances.GetAmount(ResourceCategory.RegionMaterial), Is.EqualTo(2));
            Assert.That(
                gameState.ResourceBalances.GetAmount(ResourceCategory.PersistentProgressionMaterial),
                Is.EqualTo(0));
            Assert.That(storage.SavedGameState, Is.Null);
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
