using NUnit.Framework;
using Survivalon.Core;
using Survivalon.Data.Characters;
using Survivalon.Data.Gear;
using Survivalon.State.Persistence;
using Survivalon.Tests.EditMode.World;
using Survivalon.Towns;

namespace Survivalon.Tests.EditMode.Towns
{
    public sealed class TownServiceScreenStateResolverTests
    {
        [Test]
        public void ShouldResolveProgressionHubAndBuildPreparationFromPersistentState()
        {
            PersistentGameState gameState = BootstrapWorldTestData.CreateGameState();
            gameState.ResourceBalances.Add(ResourceCategory.PersistentProgressionMaterial, 2);

            PlayableCharacterSkillPackageAssignmentService skillPackageAssignmentService =
                new PlayableCharacterSkillPackageAssignmentService();
            PlayableCharacterGearAssignmentService gearAssignmentService =
                new PlayableCharacterGearAssignmentService();
            AccountWideProgressionBoardService progressionBoardService = new AccountWideProgressionBoardService();

            Assert.That(
                skillPackageAssignmentService.TryAssignSelectedCharacterSkillPackage(
                    gameState,
                    PlayableCharacterSkillPackageIds.VanguardBurstDrill),
                Is.True);
            Assert.That(
                gearAssignmentService.TryAssignSelectedCharacterGear(gameState, GearIds.TrainingBlade),
                Is.True);
            Assert.That(
                gearAssignmentService.TryAssignSelectedCharacterGear(gameState, GearIds.GuardCharm),
                Is.True);
            Assert.That(
                progressionBoardService.TryPurchase(
                    gameState,
                    AccountWideUpgradeId.CombatBaselineProject),
                Is.EqualTo(AccountWideUpgradePurchaseStatus.Purchased));

            TownServiceScreenStateResolver resolver = new TownServiceScreenStateResolver();

            TownServiceScreenState screenState = resolver.Resolve(
                NodePlaceholderTestData.CreateTownServicePlaceholderState(),
                gameState);

            Assert.That(screenState.ServiceContext.DisplayName, Is.EqualTo("Cavern Service Hub"));
            Assert.That(screenState.PersistentProgressionMaterialAmount, Is.EqualTo(1));
            Assert.That(screenState.SelectedCharacterDisplayName, Is.EqualTo("Vanguard"));
            Assert.That(screenState.AssignedSkillPackageDisplayName, Is.EqualTo("Burst Drill"));
            Assert.That(screenState.PrimaryGearDisplayName, Is.EqualTo("Training Blade"));
            Assert.That(screenState.SupportGearDisplayName, Is.EqualTo("Guard Charm"));
            Assert.That(screenState.ProgressionOptions.Count, Is.EqualTo(4));
            Assert.That(screenState.ProgressionOptions[0].UpgradeId, Is.EqualTo(AccountWideUpgradeId.CombatBaselineProject));
            Assert.That(screenState.ProgressionOptions[0].UpgradeDisplayName, Is.EqualTo("Combat Baseline Project"));
            Assert.That(screenState.ProgressionOptions[0].IsPurchased, Is.True);
            Assert.That(screenState.ProgressionOptions[0].IsAffordable, Is.False);
            Assert.That(screenState.ProgressionOptions[1].UpgradeId, Is.EqualTo(AccountWideUpgradeId.PushOffenseProject));
            Assert.That(screenState.ProgressionOptions[1].UpgradeDisplayName, Is.EqualTo("Push Offense Project"));
            Assert.That(screenState.ProgressionOptions[1].IsPurchased, Is.False);
            Assert.That(screenState.ProgressionOptions[1].IsAffordable, Is.False);
            Assert.That(screenState.ProgressionOptions[2].UpgradeId, Is.EqualTo(AccountWideUpgradeId.FarmYieldProject));
            Assert.That(screenState.ProgressionOptions[2].UpgradeDisplayName, Is.EqualTo("Farm Yield Project"));
            Assert.That(screenState.ProgressionOptions[2].IsPurchased, Is.False);
            Assert.That(screenState.ProgressionOptions[2].IsAffordable, Is.True);
            Assert.That(screenState.ProgressionOptions[3].UpgradeId, Is.EqualTo(AccountWideUpgradeId.BossSalvageProject));
            Assert.That(screenState.ProgressionOptions[3].UpgradeDisplayName, Is.EqualTo("Boss Salvage Project"));
            Assert.That(screenState.ProgressionOptions[3].IsPurchased, Is.False);
            Assert.That(screenState.ProgressionOptions[3].IsAffordable, Is.False);
            Assert.That(screenState.SkillPackageOptions.Count, Is.EqualTo(2));
            Assert.That(screenState.SkillPackageOptions[0].SkillPackageId, Is.EqualTo(PlayableCharacterSkillPackageIds.VanguardDefault));
            Assert.That(screenState.SkillPackageOptions[0].IsAssigned, Is.False);
            Assert.That(screenState.SkillPackageOptions[1].SkillPackageId, Is.EqualTo(PlayableCharacterSkillPackageIds.VanguardBurstDrill));
            Assert.That(screenState.SkillPackageOptions[1].IsAssigned, Is.True);
            Assert.That(screenState.GearAssignmentOptions.Count, Is.EqualTo(2));
            Assert.That(screenState.GearAssignmentOptions[0].GearId, Is.EqualTo(GearIds.TrainingBlade));
            Assert.That(screenState.GearAssignmentOptions[0].IsEquipped, Is.True);
            Assert.That(screenState.GearAssignmentOptions[1].GearId, Is.EqualTo(GearIds.GuardCharm));
            Assert.That(screenState.GearAssignmentOptions[1].IsEquipped, Is.True);
        }

        [Test]
        public void ShouldRejectServiceStateResolutionWithoutTownServiceContext()
        {
            TownServiceScreenStateResolver resolver = new TownServiceScreenStateResolver();

            Assert.That(
                () => resolver.Resolve(
                    NodePlaceholderTestData.CreateServicePlaceholderState(),
                    BootstrapWorldTestData.CreateGameState()),
                Throws.ArgumentException.With.Message.Contains("town service context"));
        }
    }
}
