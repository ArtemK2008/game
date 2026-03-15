using NUnit.Framework;
using Survivalon.Runtime;

namespace Survivalon.Tests.EditMode
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
            gameState.ResourceBalances.Add(ResourceCategory.PersistentProgressionMaterial, 1);

            AccountWideUpgradePurchaseStatus purchaseStatus =
                boardService.TryPurchase(gameState, AccountWideUpgradeId.CombatBaselineProject);
            service.SaveResolvedWorldContext(gameState);

            Assert.That(purchaseStatus, Is.EqualTo(AccountWideUpgradePurchaseStatus.Purchased));
            Assert.That(
                storage.SavedGameState.ProgressionState.TryGetEntry(
                    "account_wide_combat_baseline_project",
                    out ProgressionEntryState savedEntry),
                Is.True);
            Assert.That(savedEntry.IsUnlocked, Is.True);
            Assert.That(savedEntry.CurrentValue, Is.EqualTo(1));
            Assert.That(
                effectResolver.Resolve(storage.SavedGameState.ProgressionState).PlayerMaxHealthBonus,
                Is.EqualTo(10));
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
