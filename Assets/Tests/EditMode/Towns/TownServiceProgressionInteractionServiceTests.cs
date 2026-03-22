using NUnit.Framework;
using Survivalon.Core;
using Survivalon.Data.Progression;
using Survivalon.State.Persistence;
using Survivalon.Tests.EditMode.World;
using Survivalon.Towns;

namespace Survivalon.Tests.EditMode.Towns
{
    /// <summary>
    /// Проверяет town/service purchase flow после выноса static progression content в Data.Progression.
    /// </summary>
    public sealed class TownServiceProgressionInteractionServiceTests
    {
        [Test]
        public void ShouldPurchaseAffordableUpgradeAndPersistUpdatedProgressionState()
        {
            MemoryPersistentGameStateStorage storage = new MemoryPersistentGameStateStorage();
            SafeResumePersistenceService persistenceService = new SafeResumePersistenceService(storage);
            TownServiceProgressionInteractionService interactionService =
                new TownServiceProgressionInteractionService(persistenceService);
            AccountWideProgressionEffectResolver effectResolver = new AccountWideProgressionEffectResolver();
            PersistentGameState gameState = BootstrapWorldTestData.CreateGameState();
            gameState.ResourceBalances.Add(ResourceCategory.PersistentProgressionMaterial, 1);

            AccountWideUpgradePurchaseStatus purchaseStatus =
                interactionService.TryPurchase(gameState, AccountWideUpgradeId.CombatBaselineProject);

            Assert.That(purchaseStatus, Is.EqualTo(AccountWideUpgradePurchaseStatus.Purchased));
            Assert.That(gameState.ResourceBalances.GetAmount(ResourceCategory.PersistentProgressionMaterial), Is.EqualTo(0));
            Assert.That(storage.SavedGameState, Is.Not.Null);
            Assert.That(
                storage.SavedGameState.ProgressionState.TryGetEntry(
                    "account_wide_combat_baseline_project",
                    out ProgressionEntryState progressionEntry),
                Is.True);
            Assert.That(progressionEntry.IsUnlocked, Is.True);
            Assert.That(progressionEntry.CurrentValue, Is.EqualTo(1));

            AccountWideProgressionEffectState progressionEffects =
                effectResolver.Resolve(storage.SavedGameState.ProgressionState);
            Assert.That(progressionEffects.PlayerMaxHealthBonus, Is.EqualTo(10));
        }

        [Test]
        public void ShouldNotPersistWhenUpgradePurchaseIsRejected()
        {
            MemoryPersistentGameStateStorage storage = new MemoryPersistentGameStateStorage();
            SafeResumePersistenceService persistenceService = new SafeResumePersistenceService(storage);
            TownServiceProgressionInteractionService interactionService =
                new TownServiceProgressionInteractionService(persistenceService);
            PersistentGameState gameState = BootstrapWorldTestData.CreateGameState();

            AccountWideUpgradePurchaseStatus purchaseStatus =
                interactionService.TryPurchase(gameState, AccountWideUpgradeId.PushOffenseProject);

            Assert.That(purchaseStatus, Is.EqualTo(AccountWideUpgradePurchaseStatus.InsufficientResources));
            Assert.That(storage.SavedGameState, Is.Null);
        }

        [Test]
        public void ShouldPurchaseBossSalvageProjectAndPersistFutureBossRewardEffect()
        {
            MemoryPersistentGameStateStorage storage = new MemoryPersistentGameStateStorage();
            SafeResumePersistenceService persistenceService = new SafeResumePersistenceService(storage);
            TownServiceProgressionInteractionService interactionService =
                new TownServiceProgressionInteractionService(persistenceService);
            AccountWideProgressionEffectResolver effectResolver = new AccountWideProgressionEffectResolver();
            PersistentGameState gameState = BootstrapWorldTestData.CreateGameState();
            gameState.ResourceBalances.Add(ResourceCategory.PersistentProgressionMaterial, 2);

            AccountWideUpgradePurchaseStatus purchaseStatus =
                interactionService.TryPurchase(gameState, AccountWideUpgradeId.BossSalvageProject);

            Assert.That(purchaseStatus, Is.EqualTo(AccountWideUpgradePurchaseStatus.Purchased));
            Assert.That(gameState.ResourceBalances.GetAmount(ResourceCategory.PersistentProgressionMaterial), Is.EqualTo(0));
            Assert.That(storage.SavedGameState, Is.Not.Null);
            Assert.That(
                storage.SavedGameState.ProgressionState.TryGetEntry(
                    "account_wide_boss_salvage_project",
                    out ProgressionEntryState progressionEntry),
                Is.True);
            Assert.That(progressionEntry.IsUnlocked, Is.True);
            Assert.That(progressionEntry.CurrentValue, Is.EqualTo(1));

            AccountWideProgressionEffectState progressionEffects =
                effectResolver.Resolve(storage.SavedGameState.ProgressionState);
            Assert.That(progressionEffects.BossProgressionMaterialRewardBonus, Is.EqualTo(1));
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
