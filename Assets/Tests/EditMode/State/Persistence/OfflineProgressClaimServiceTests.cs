using System;
using NUnit.Framework;
using Survivalon.Core;
using Survivalon.State.Persistence;
using Survivalon.Tests.EditMode.World;
using Survivalon.World;

namespace Survivalon.Tests.EditMode.State.Persistence
{
    public sealed class OfflineProgressClaimServiceTests
    {
        [Test]
        public void ShouldGrantOfflineClaimPersistImmediatelyAndPreventImmediateDoubleClaim()
        {
            DateTimeOffset savedTime = new DateTimeOffset(2026, 3, 29, 10, 0, 0, TimeSpan.Zero);
            DateTimeOffset claimTime = new DateTimeOffset(2026, 3, 29, 12, 0, 0, TimeSpan.Zero);
            MemoryPersistentGameStateStorage storage = new MemoryPersistentGameStateStorage();
            SafeResumePersistenceService persistenceService = new SafeResumePersistenceService(
                storage,
                () => claimTime,
                new OfflineProgressEligibilityResolver(BootstrapWorldTestData.CreateWorldGraph()));
            OfflineProgressClaimResolver claimResolver = new OfflineProgressClaimResolver(
                BootstrapWorldTestData.CreateWorldGraph(),
                () => claimTime);
            OfflineProgressClaimService claimService = new OfflineProgressClaimService(persistenceService);
            PersistentGameState gameState = CreateEligibleFarmReadySavedGameState(
                BootstrapWorldScenario.ForestFarmNodeId,
                savedTime);

            Assert.That(claimResolver.TryResolve(gameState, out OfflineProgressClaimState claimState), Is.True);

            claimService.Claim(gameState, claimState);

            Assert.That(storage.HasSavedState, Is.True);
            Assert.That(storage.SavedGameState.ResourceBalances.GetAmount(ResourceCategory.RegionMaterial), Is.EqualTo(4));
            Assert.That(
                storage.SavedGameState.OfflineProgressStableSaveAnchorState.LastStableSaveUnixTimeSeconds,
                Is.EqualTo(claimTime.ToUnixTimeSeconds()));
            Assert.That(
                storage.SavedGameState.OfflineProgressStableSaveAnchorState.EligibilityKind,
                Is.EqualTo(OfflineProgressEligibilityKind.FarmReadyWorldNode));

            OfflineProgressClaimResolver secondResolver = new OfflineProgressClaimResolver(
                BootstrapWorldTestData.CreateWorldGraph(),
                () => claimTime);
            Assert.That(secondResolver.TryResolve(storage.SavedGameState, out OfflineProgressClaimState secondClaimState), Is.False);
            Assert.That(secondClaimState, Is.Null);
        }

        private static PersistentGameState CreateEligibleFarmReadySavedGameState(
            NodeId nodeId,
            DateTimeOffset stableSaveTime)
        {
            PersistentGameState gameState = BootstrapWorldTestData.CreateGameState();
            gameState.WorldState.SetCurrentNode(nodeId);
            gameState.WorldState.SetLastSafeNode(nodeId);
            gameState.WorldState.ReplaceReachableNodes(new[] { nodeId });

            Assert.That(gameState.WorldState.TryGetNodeState(nodeId, out PersistentNodeState nodeState), Is.True);
            if (!nodeState.IsCompleted)
            {
                nodeState.ApplyUnlockProgress(nodeState.UnlockThreshold);
            }

            gameState.SafeResumeState.MarkWorldMap(nodeId);
            gameState.OfflineProgressStableSaveAnchorState.StampStableSaveAnchor(
                stableSaveTime.ToUnixTimeSeconds(),
                OfflineProgressEligibilityKind.FarmReadyWorldNode);
            return gameState;
        }

        private sealed class MemoryPersistentGameStateStorage : IPersistentGameStateStorage
        {
            public PersistentGameState SavedGameState { get; private set; }

            public bool HasSavedState => SavedGameState != null;

            public void Save(PersistentGameState gameState)
            {
                string json = UnityEngine.JsonUtility.ToJson(gameState);
                SavedGameState = UnityEngine.JsonUtility.FromJson<PersistentGameState>(json);
                Assert.That(SavedGameState, Is.Not.Null);
            }

            public bool TryLoad(out PersistentGameState gameState)
            {
                gameState = SavedGameState;
                return gameState != null;
            }
        }
    }
}
