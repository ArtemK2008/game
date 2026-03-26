using NUnit.Framework;
using Survivalon.Characters;
using Survivalon.Core;
using Survivalon.Data.Characters;
using Survivalon.Data.Gear;
using Survivalon.Data.Progression;
using Survivalon.Run;
using Survivalon.State.Persistence;
using Survivalon.Tests.EditMode.Run;
using Survivalon.Tests.EditMode.World;

namespace Survivalon.Tests.EditMode.State.Persistence
{
    /// <summary>
    /// Проверяет safe-resume persistence с уже купленными progression upgrades после выноса authored catalog в Data.Progression.
    /// </summary>
    public sealed class SafeResumePersistenceServiceTests
    {
        [Test]
        public void ShouldPersistResolvedWorldLevelContext()
        {
            MemoryPersistentGameStateStorage storage = new MemoryPersistentGameStateStorage();
            SafeResumePersistenceService service = new SafeResumePersistenceService(storage);
            PersistentGameState gameState = CreateGameState("region_002_node_001", "region_001_node_002");
            gameState.ResourceBalances.Add(ResourceCategory.SoftCurrency, 4);

            service.SaveResolvedWorldContext(gameState);

            Assert.That(storage.HasSavedState, Is.True);
            Assert.That(storage.SavedGameState.SafeResumeState.HasSafeResumeTarget, Is.True);
            Assert.That(storage.SavedGameState.SafeResumeState.TargetType, Is.EqualTo(SafeResumeTargetType.WorldMap));
            Assert.That(storage.SavedGameState.SafeResumeState.ResumeNodeId, Is.EqualTo(new NodeId("region_002_node_001")));
            Assert.That(storage.SavedGameState.ResourceBalances.GetAmount(ResourceCategory.SoftCurrency), Is.EqualTo(4));
        }

        [Test]
        public void ShouldLoadLastSavedStateInsteadOfUnsavedRuntimeMutations()
        {
            MemoryPersistentGameStateStorage storage = new MemoryPersistentGameStateStorage();
            SafeResumePersistenceService service = new SafeResumePersistenceService(storage);
            PersistentGameState savedGameState = CreateGameState("region_002_node_001", "region_001_node_002");

            service.SaveResolvedWorldContext(savedGameState);

            savedGameState.WorldState.SetCurrentNode(new NodeId("region_002_node_002"));
            savedGameState.WorldState.SetLastSafeNode(new NodeId("region_002_node_001"));

            PersistentGameState loadedGameState = service.LoadOrCreate(CreateGameState("region_001_node_002", "region_001_node_001"));

            Assert.That(loadedGameState.WorldState.CurrentNodeId, Is.EqualTo(new NodeId("region_002_node_001")));
            Assert.That(loadedGameState.SafeResumeState.ResumeNodeId, Is.EqualTo(new NodeId("region_002_node_001")));
        }

        [Test]
        public void ShouldPersistPurchasedAccountWideUpgradeAndAppliedEffectState()
        {
            MemoryPersistentGameStateStorage storage = new MemoryPersistentGameStateStorage();
            SafeResumePersistenceService service = new SafeResumePersistenceService(storage);
            AccountWideProgressionBoardService boardService = new AccountWideProgressionBoardService();
            AccountWideProgressionEffectResolver effectResolver = new AccountWideProgressionEffectResolver();
            PersistentGameState gameState = CreateGameState("region_002_node_001", "region_001_node_002");
            gameState.ResourceBalances.Add(ResourceCategory.PersistentProgressionMaterial, 4);

            AccountWideUpgradePurchaseStatus healthPurchaseStatus =
                boardService.TryPurchase(gameState, AccountWideUpgradeId.CombatBaselineProject);
            AccountWideUpgradePurchaseStatus pushPurchaseStatus =
                boardService.TryPurchase(gameState, AccountWideUpgradeId.PushOffenseProject);
            AccountWideUpgradePurchaseStatus farmPurchaseStatus =
                boardService.TryPurchase(gameState, AccountWideUpgradeId.FarmYieldProject);
            service.SaveResolvedWorldContext(gameState);

            Assert.That(healthPurchaseStatus, Is.EqualTo(AccountWideUpgradePurchaseStatus.Purchased));
            Assert.That(pushPurchaseStatus, Is.EqualTo(AccountWideUpgradePurchaseStatus.Purchased));
            Assert.That(farmPurchaseStatus, Is.EqualTo(AccountWideUpgradePurchaseStatus.Purchased));
            Assert.That(
                storage.SavedGameState.ProgressionState.TryGetEntry(
                    "account_wide_combat_baseline_project",
                    out ProgressionEntryState healthEntry),
                Is.True);
            Assert.That(healthEntry.IsUnlocked, Is.True);
            Assert.That(healthEntry.CurrentValue, Is.EqualTo(1));
            Assert.That(
                storage.SavedGameState.ProgressionState.TryGetEntry(
                    "account_wide_push_offense_project",
                    out ProgressionEntryState pushEntry),
                Is.True);
            Assert.That(pushEntry.IsUnlocked, Is.True);
            Assert.That(pushEntry.CurrentValue, Is.EqualTo(1));
            Assert.That(
                storage.SavedGameState.ProgressionState.TryGetEntry(
                    "account_wide_farm_yield_project",
                    out ProgressionEntryState farmEntry),
                Is.True);
            Assert.That(farmEntry.IsUnlocked, Is.True);
            Assert.That(farmEntry.CurrentValue, Is.EqualTo(1));

            AccountWideProgressionEffectState effects = effectResolver.Resolve(storage.SavedGameState.ProgressionState);
            Assert.That(effects.PlayerMaxHealthBonus, Is.EqualTo(10));
            Assert.That(effects.PlayerAttackPowerBonus, Is.EqualTo(4));
            Assert.That(effects.OrdinaryRegionMaterialRewardBonus, Is.EqualTo(1));
        }

        [Test]
        public void ShouldPersistPlayableCharacterStateInResolvedWorldSnapshot()
        {
            MemoryPersistentGameStateStorage storage = new MemoryPersistentGameStateStorage();
            SafeResumePersistenceService service = new SafeResumePersistenceService(storage);
            PersistentGameState gameState = CreateGameState("region_002_node_001", "region_001_node_002");
            gameState.EnsureOwnedGearId(GearIds.TrainingBlade);
            gameState.EnsureOwnedGearId(GearIds.GuardCharm);
            gameState.AddCharacterState(new PersistentCharacterState(
                "character_vanguard",
                isUnlocked: true,
                isSelectable: true,
                isActive: false,
                progressionRank: 2,
                skillPackageId: PlayableCharacterSkillPackageIds.VanguardDefault,
                loadoutState: new PersistentLoadoutState(
                    equippedGearStates: new[]
                    {
                        new EquippedGearState(GearIds.TrainingBlade, GearCategory.PrimaryCombat),
                        new EquippedGearState(GearIds.GuardCharm, GearCategory.SecondarySupport),
                    })));
            gameState.AddCharacterState(new PersistentCharacterState(
                "character_striker",
                isUnlocked: true,
                isSelectable: true,
                isActive: true,
                progressionRank: 1,
                skillPackageId: PlayableCharacterSkillPackageIds.StrikerDefault));

            service.SaveResolvedWorldContext(gameState);

            Assert.That(storage.SavedGameState.CharacterStates, Has.Count.EqualTo(2));
            Assert.That(storage.SavedGameState.OwnedGearIds, Has.Count.EqualTo(2));
            Assert.That(storage.SavedGameState.OwnedGearIds, Does.Contain(GearIds.TrainingBlade));
            Assert.That(storage.SavedGameState.OwnedGearIds, Does.Contain(GearIds.GuardCharm));
            Assert.That(
                storage.SavedGameState.TryGetCharacterState("character_vanguard", out PersistentCharacterState characterState),
                Is.True);
            Assert.That(characterState.IsUnlocked, Is.True);
            Assert.That(characterState.IsSelectable, Is.True);
            Assert.That(characterState.IsActive, Is.False);
            Assert.That(characterState.ProgressionRank, Is.EqualTo(2));
            Assert.That(characterState.SkillPackageId, Is.EqualTo(PlayableCharacterSkillPackageIds.VanguardDefault));
            Assert.That(characterState.LoadoutState.EquippedGearStates, Has.Count.EqualTo(2));
            Assert.That(characterState.LoadoutState.EquippedGearStates, Has.Some.Matches<EquippedGearState>(state =>
                state.GearId == GearIds.TrainingBlade &&
                state.GearCategory == GearCategory.PrimaryCombat));
            Assert.That(characterState.LoadoutState.EquippedGearStates, Has.Some.Matches<EquippedGearState>(state =>
                state.GearId == GearIds.GuardCharm &&
                state.GearCategory == GearCategory.SecondarySupport));
            Assert.That(
                storage.SavedGameState.TryGetCharacterState("character_striker", out PersistentCharacterState strikerState),
                Is.True);
            Assert.That(strikerState.IsUnlocked, Is.True);
            Assert.That(strikerState.IsSelectable, Is.True);
            Assert.That(strikerState.IsActive, Is.True);
            Assert.That(strikerState.ProgressionRank, Is.EqualTo(1));
            Assert.That(strikerState.SkillPackageId, Is.EqualTo(PlayableCharacterSkillPackageIds.StrikerDefault));
        }

        [Test]
        public void ShouldKeepRunOnlySkillUpgradeChoiceTemporaryAfterSaveLoad()
        {
            MemoryPersistentGameStateStorage storage = new MemoryPersistentGameStateStorage();
            SafeResumePersistenceService service = new SafeResumePersistenceService(storage);
            PlayableCharacterSelectionService selectionService = new PlayableCharacterSelectionService();
            PersistentGameState gameState = BootstrapWorldTestData.CreateGameState();

            Assert.That(selectionService.TrySelectCharacter(gameState, "character_striker"), Is.True);

            service.SaveResolvedWorldContext(gameState);

            PersistentGameState loadedGameState = service.LoadOrCreate(BootstrapWorldTestData.CreateGameState());
            RunLifecycleController controller = new RunLifecycleController(
                RunLifecycleControllerTestData.CreateCombatNodeState(),
                persistentContext: RunPersistentContext.FromGameState(loadedGameState));

            Assert.That(controller.RunTimeSkillUpgradeOptions.Count, Is.EqualTo(2));
            Assert.That(controller.RequiresRunTimeSkillUpgradeChoice, Is.True);
            Assert.That(controller.TryStartAutomaticFlow(), Is.False);
        }

        private static PersistentGameState CreateGameState(string currentNodeIdValue, string lastSafeNodeIdValue)
        {
            PersistentGameState gameState = new PersistentGameState();
            gameState.WorldState.SetCurrentNode(new NodeId(currentNodeIdValue));
            gameState.WorldState.SetLastSafeNode(new NodeId(lastSafeNodeIdValue));
            gameState.WorldState.ReplaceReachableNodes(new[] { new NodeId(lastSafeNodeIdValue) });
            return gameState;
        }

        private sealed class MemoryPersistentGameStateStorage : IPersistentGameStateStorage
        {
            public PersistentGameState SavedGameState { get; private set; }

            public bool HasSavedState => SavedGameState != null;

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

