using System;
using Survivalon.Data.Progression;
using Survivalon.State.Persistence;

namespace Survivalon.Towns
{
    /// <summary>
    /// Оркестрирует покупку account-wide progression upgrades из town/service shell.
    /// </summary>
    public sealed class TownServiceProgressionInteractionService
    {
        private readonly AccountWideProgressionBoardService progressionBoardService;
        private readonly SafeResumePersistenceService persistenceService;

        public TownServiceProgressionInteractionService(
            SafeResumePersistenceService persistenceService = null,
            AccountWideProgressionBoardService progressionBoardService = null)
        {
            this.persistenceService = persistenceService;
            this.progressionBoardService = progressionBoardService ?? new AccountWideProgressionBoardService();
        }

        public AccountWideUpgradePurchaseStatus TryPurchase(PersistentGameState gameState, AccountWideUpgradeId upgradeId)
        {
            if (gameState == null)
            {
                throw new ArgumentNullException(nameof(gameState));
            }

            AccountWideUpgradePurchaseStatus purchaseStatus =
                progressionBoardService.TryPurchase(gameState, upgradeId);

            if (purchaseStatus == AccountWideUpgradePurchaseStatus.Purchased && persistenceService != null)
            {
                persistenceService.SaveResolvedWorldContext(gameState);
            }

            return purchaseStatus;
        }
    }
}
