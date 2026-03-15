using NUnit.Framework;
using Survivalon.Runtime;

namespace Survivalon.Tests.EditMode
{
    public sealed class AccountWideProgressionBoardServiceTests
    {
        [Test]
        public void ShouldReportUpgradeAffordableWhenPersistentProgressionMaterialBalanceMeetsCost()
        {
            PersistentGameState gameState = new PersistentGameState();
            AccountWideProgressionBoardService service = new AccountWideProgressionBoardService();
            gameState.ResourceBalances.Add(ResourceCategory.PersistentProgressionMaterial, 1);

            bool canAfford = service.CanAfford(gameState, AccountWideUpgradeId.CombatBaselineProject);

            Assert.That(canAfford, Is.True);
        }

        [Test]
        public void ShouldPurchaseAccountWideUpgradeAndSpendPersistentProgressionMaterial()
        {
            PersistentGameState gameState = new PersistentGameState();
            AccountWideProgressionBoardService service = new AccountWideProgressionBoardService();
            AccountWideProgressionEffectResolver effectResolver = new AccountWideProgressionEffectResolver();
            gameState.ResourceBalances.Add(ResourceCategory.PersistentProgressionMaterial, 1);

            AccountWideUpgradePurchaseStatus purchaseStatus =
                service.TryPurchase(gameState, AccountWideUpgradeId.CombatBaselineProject);
            AccountWideProgressionEffectState appliedEffects =
                effectResolver.Resolve(gameState.ProgressionState);

            Assert.That(purchaseStatus, Is.EqualTo(AccountWideUpgradePurchaseStatus.Purchased));
            Assert.That(gameState.ResourceBalances.GetAmount(ResourceCategory.PersistentProgressionMaterial), Is.EqualTo(0));
            Assert.That(
                gameState.ProgressionState.TryGetEntry(
                    "account_wide_combat_baseline_project",
                    out ProgressionEntryState purchasedEntry),
                Is.True);
            Assert.That(purchasedEntry.LayerType, Is.EqualTo(ProgressionLayerType.AccountWide));
            Assert.That(purchasedEntry.IsUnlocked, Is.True);
            Assert.That(purchasedEntry.CurrentValue, Is.EqualTo(1));
            Assert.That(appliedEffects.PlayerMaxHealthBonus, Is.EqualTo(10));
        }

        [Test]
        public void ShouldRejectPurchaseWhenPersistentProgressionMaterialIsInsufficient()
        {
            PersistentGameState gameState = new PersistentGameState();
            AccountWideProgressionBoardService service = new AccountWideProgressionBoardService();

            AccountWideUpgradePurchaseStatus purchaseStatus =
                service.TryPurchase(gameState, AccountWideUpgradeId.CombatBaselineProject);

            Assert.That(purchaseStatus, Is.EqualTo(AccountWideUpgradePurchaseStatus.InsufficientResources));
            Assert.That(gameState.ResourceBalances.GetAmount(ResourceCategory.PersistentProgressionMaterial), Is.EqualTo(0));
            Assert.That(
                gameState.ProgressionState.TryGetEntry(
                    "account_wide_combat_baseline_project",
                    out _),
                Is.False);
        }

        [Test]
        public void ShouldRejectRepurchasingAlreadyPurchasedAccountWideUpgrade()
        {
            PersistentGameState gameState = new PersistentGameState();
            AccountWideProgressionBoardService service = new AccountWideProgressionBoardService();
            gameState.ResourceBalances.Add(ResourceCategory.PersistentProgressionMaterial, 2);

            AccountWideUpgradePurchaseStatus firstPurchaseStatus =
                service.TryPurchase(gameState, AccountWideUpgradeId.CombatBaselineProject);
            AccountWideUpgradePurchaseStatus secondPurchaseStatus =
                service.TryPurchase(gameState, AccountWideUpgradeId.CombatBaselineProject);

            Assert.That(firstPurchaseStatus, Is.EqualTo(AccountWideUpgradePurchaseStatus.Purchased));
            Assert.That(secondPurchaseStatus, Is.EqualTo(AccountWideUpgradePurchaseStatus.AlreadyPurchased));
            Assert.That(gameState.ResourceBalances.GetAmount(ResourceCategory.PersistentProgressionMaterial), Is.EqualTo(1));
        }
    }
}
