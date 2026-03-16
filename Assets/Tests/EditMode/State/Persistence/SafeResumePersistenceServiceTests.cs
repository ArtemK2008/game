using NUnit.Framework;
using Survivalon.Core;
using Survivalon.State.Persistence;

namespace Survivalon.Tests.EditMode.State.Persistence
{
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
            gameState.AddCharacterState(new PersistentCharacterState(
                "character_vanguard",
                isUnlocked: true,
                isSelectable: true,
                isActive: true,
                progressionRank: 2,
                skillPackageId: "skill_package_vanguard_default"));

            service.SaveResolvedWorldContext(gameState);

            Assert.That(storage.SavedGameState.CharacterStates, Has.Count.EqualTo(1));
            Assert.That(
                storage.SavedGameState.TryGetCharacterState("character_vanguard", out PersistentCharacterState characterState),
                Is.True);
            Assert.That(characterState.IsUnlocked, Is.True);
            Assert.That(characterState.IsSelectable, Is.True);
            Assert.That(characterState.IsActive, Is.True);
            Assert.That(characterState.ProgressionRank, Is.EqualTo(2));
            Assert.That(characterState.SkillPackageId, Is.EqualTo("skill_package_vanguard_default"));
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

