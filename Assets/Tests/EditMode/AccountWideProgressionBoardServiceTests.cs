using NUnit.Framework;
using Survivalon.Runtime;

namespace Survivalon.Tests.EditMode
{
    public sealed class AccountWideProgressionBoardServiceTests
    {
        [Test]
        public void ShouldReportRequiredResourcesAvailableWhenPersistentProgressionMaterialBalanceMeetsCost()
        {
            PersistentGameState gameState = new PersistentGameState();
            AccountWideProgressionBoardService service = new AccountWideProgressionBoardService();
            gameState.ResourceBalances.Add(ResourceCategory.PersistentProgressionMaterial, 1);

            bool hasRequiredResources = service.HasRequiredResources(
                gameState,
                AccountWideUpgradeId.CombatBaselineProject);

            Assert.That(hasRequiredResources, Is.True);
        }

        [Test]
        public void ShouldReportUpgradeBuyableWhenResourcesAreAvailableAndUpgradeIsNotPurchased()
        {
            PersistentGameState gameState = new PersistentGameState();
            AccountWideProgressionBoardService service = new AccountWideProgressionBoardService();
            gameState.ResourceBalances.Add(ResourceCategory.PersistentProgressionMaterial, 1);

            bool canPurchase = service.CanPurchase(gameState, AccountWideUpgradeId.CombatBaselineProject);

            Assert.That(canPurchase, Is.True);
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
            Assert.That(appliedEffects.PlayerAttackPowerBonus, Is.EqualTo(0));
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
        public void ShouldReportUpgradeNotBuyableWhenPersistentProgressionMaterialIsInsufficient()
        {
            PersistentGameState gameState = new PersistentGameState();
            AccountWideProgressionBoardService service = new AccountWideProgressionBoardService();

            bool hasRequiredResources = service.HasRequiredResources(
                gameState,
                AccountWideUpgradeId.CombatBaselineProject);
            bool canPurchase = service.CanPurchase(
                gameState,
                AccountWideUpgradeId.CombatBaselineProject);

            Assert.That(hasRequiredResources, Is.False);
            Assert.That(canPurchase, Is.False);
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

        [Test]
        public void ShouldReportAlreadyPurchasedUpgradeAsNotBuyableEvenWhenResourcesRemain()
        {
            PersistentGameState gameState = new PersistentGameState();
            AccountWideProgressionBoardService service = new AccountWideProgressionBoardService();
            gameState.ResourceBalances.Add(ResourceCategory.PersistentProgressionMaterial, 2);

            service.TryPurchase(gameState, AccountWideUpgradeId.CombatBaselineProject);

            bool hasRequiredResources = service.HasRequiredResources(
                gameState,
                AccountWideUpgradeId.CombatBaselineProject);
            bool canPurchase = service.CanPurchase(
                gameState,
                AccountWideUpgradeId.CombatBaselineProject);

            Assert.That(hasRequiredResources, Is.True);
            Assert.That(canPurchase, Is.False);
        }

        [Test]
        public void ShouldReportPushOffenseProjectBuyableWhenTwoMaterialsAreAvailable()
        {
            PersistentGameState gameState = new PersistentGameState();
            AccountWideProgressionBoardService service = new AccountWideProgressionBoardService();
            gameState.ResourceBalances.Add(ResourceCategory.PersistentProgressionMaterial, 2);

            bool hasRequiredResources = service.HasRequiredResources(gameState, AccountWideUpgradeId.PushOffenseProject);
            bool canPurchase = service.CanPurchase(gameState, AccountWideUpgradeId.PushOffenseProject);

            Assert.That(hasRequiredResources, Is.True);
            Assert.That(canPurchase, Is.True);
        }

        [Test]
        public void ShouldRejectPushOffenseProjectWhenOnlyOneMaterialIsAvailable()
        {
            PersistentGameState gameState = new PersistentGameState();
            AccountWideProgressionBoardService service = new AccountWideProgressionBoardService();
            gameState.ResourceBalances.Add(ResourceCategory.PersistentProgressionMaterial, 1);

            bool hasRequiredResources = service.HasRequiredResources(gameState, AccountWideUpgradeId.PushOffenseProject);
            bool canPurchase = service.CanPurchase(gameState, AccountWideUpgradeId.PushOffenseProject);

            Assert.That(hasRequiredResources, Is.False);
            Assert.That(canPurchase, Is.False);
        }

        [Test]
        public void ShouldPurchasePushOffenseProjectAndApplyAttackPowerBonus()
        {
            PersistentGameState gameState = new PersistentGameState();
            AccountWideProgressionBoardService service = new AccountWideProgressionBoardService();
            AccountWideProgressionEffectResolver effectResolver = new AccountWideProgressionEffectResolver();
            AccountWideProgressionUpgradeDefinition upgradeDefinition =
                AccountWideProgressionUpgradeCatalog.Get(AccountWideUpgradeId.PushOffenseProject);
            gameState.ResourceBalances.Add(ResourceCategory.PersistentProgressionMaterial, upgradeDefinition.CostAmount);

            AccountWideUpgradePurchaseStatus purchaseStatus =
                service.TryPurchase(gameState, AccountWideUpgradeId.PushOffenseProject);
            AccountWideProgressionEffectState appliedEffects =
                effectResolver.Resolve(gameState.ProgressionState);

            Assert.That(purchaseStatus, Is.EqualTo(AccountWideUpgradePurchaseStatus.Purchased));
            Assert.That(gameState.ResourceBalances.GetAmount(ResourceCategory.PersistentProgressionMaterial), Is.EqualTo(0));
            Assert.That(
                gameState.ProgressionState.TryGetEntry(upgradeDefinition.ProgressionId, out ProgressionEntryState purchasedEntry),
                Is.True);
            Assert.That(purchasedEntry.LayerType, Is.EqualTo(ProgressionLayerType.AccountWide));
            Assert.That(purchasedEntry.IsUnlocked, Is.True);
            Assert.That(purchasedEntry.CurrentValue, Is.EqualTo(1));
            Assert.That(appliedEffects.PlayerMaxHealthBonus, Is.EqualTo(0));
            Assert.That(appliedEffects.PlayerAttackPowerBonus, Is.EqualTo(4));
        }
    }
}
