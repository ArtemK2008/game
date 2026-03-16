using System;

namespace Survivalon.Runtime.State.Persistence
{
    public enum AccountWideUpgradePurchaseStatus
    {
        Purchased = 0,
        AlreadyPurchased = 1,
        InsufficientResources = 2,
    }

    public sealed class AccountWideProgressionBoardService
    {
        public bool HasRequiredResources(PersistentGameState gameState, AccountWideUpgradeId upgradeId)
        {
            if (gameState == null)
            {
                throw new ArgumentNullException(nameof(gameState));
            }

            AccountWideProgressionUpgradeDefinition upgradeDefinition =
                AccountWideProgressionUpgradeCatalog.Get(upgradeId);
            return gameState.ResourceBalances.GetAmount(upgradeDefinition.CostResourceCategory) >= upgradeDefinition.CostAmount;
        }

        public bool CanPurchase(PersistentGameState gameState, AccountWideUpgradeId upgradeId)
        {
            if (gameState == null)
            {
                throw new ArgumentNullException(nameof(gameState));
            }

            return !IsPurchased(gameState, upgradeId) &&
                HasRequiredResources(gameState, upgradeId);
        }

        public bool IsPurchased(PersistentGameState gameState, AccountWideUpgradeId upgradeId)
        {
            if (gameState == null)
            {
                throw new ArgumentNullException(nameof(gameState));
            }

            AccountWideProgressionUpgradeDefinition upgradeDefinition =
                AccountWideProgressionUpgradeCatalog.Get(upgradeId);

            return gameState.ProgressionState.TryGetEntry(upgradeDefinition.ProgressionId, out ProgressionEntryState entry) &&
                entry.IsUnlocked &&
                entry.CurrentValue > 0;
        }

        public AccountWideUpgradePurchaseStatus TryPurchase(PersistentGameState gameState, AccountWideUpgradeId upgradeId)
        {
            if (gameState == null)
            {
                throw new ArgumentNullException(nameof(gameState));
            }

            if (IsPurchased(gameState, upgradeId))
            {
                return AccountWideUpgradePurchaseStatus.AlreadyPurchased;
            }

            AccountWideProgressionUpgradeDefinition upgradeDefinition =
                AccountWideProgressionUpgradeCatalog.Get(upgradeId);

            if (!gameState.ResourceBalances.TrySpend(
                upgradeDefinition.CostResourceCategory,
                upgradeDefinition.CostAmount))
            {
                return AccountWideUpgradePurchaseStatus.InsufficientResources;
            }

            ProgressionEntryState progressionEntry = gameState.ProgressionState.GetOrAddEntry(
                upgradeDefinition.ProgressionId,
                upgradeDefinition.LayerType);
            progressionEntry.Unlock();
            progressionEntry.IncreaseValue(1);
            return AccountWideUpgradePurchaseStatus.Purchased;
        }
    }
}
