using System;
using NUnit.Framework;
using Survivalon.Core;
using Survivalon.Data.Progression;
using Survivalon.State.Persistence;
using Survivalon.Tests.EditMode.World;
using Survivalon.World;

namespace Survivalon.Tests.EditMode.State.Persistence
{
    public sealed class OfflineProgressClaimResolverTests
    {
        [Test]
        public void ShouldResolveOfflineClaimForEligibleFarmReadyWorldContext()
        {
            DateTimeOffset now = new DateTimeOffset(2026, 3, 29, 12, 0, 0, TimeSpan.Zero);
            OfflineProgressClaimResolver resolver = new OfflineProgressClaimResolver(
                BootstrapWorldTestData.CreateWorldGraph(),
                () => now);
            PersistentGameState gameState = CreateEligibleFarmReadySavedGameState(
                BootstrapWorldScenario.ForestFarmNodeId,
                now.AddHours(-2));

            bool resolved = resolver.TryResolve(gameState, out OfflineProgressClaimState claimState);

            Assert.That(resolved, Is.True);
            Assert.That(claimState, Is.Not.Null);
            Assert.That(claimState.ResourceCategory, Is.EqualTo(ResourceCategory.RegionMaterial));
            Assert.That(claimState.Amount, Is.EqualTo(4));
            Assert.That(claimState.CountedWholeHours, Is.EqualTo(2));
            Assert.That(claimState.SourceNodeDisplayName, Is.EqualTo("Forest Farm"));
        }

        [Test]
        public void ShouldIncludeFarmYieldUpgradeAndHourCapInResolvedClaim()
        {
            DateTimeOffset now = new DateTimeOffset(2026, 3, 29, 12, 0, 0, TimeSpan.Zero);
            OfflineProgressClaimResolver resolver = new OfflineProgressClaimResolver(
                BootstrapWorldTestData.CreateWorldGraph(),
                () => now);
            PersistentGameState gameState = CreateEligibleFarmReadySavedGameState(
                BootstrapWorldScenario.ForestFarmNodeId,
                now.AddHours(-12));
            gameState.ResourceBalances.Add(ResourceCategory.PersistentProgressionMaterial, 1);
            AccountWideProgressionBoardService boardService = new AccountWideProgressionBoardService();

            Assert.That(
                boardService.TryPurchase(gameState, AccountWideUpgradeId.FarmYieldProject),
                Is.EqualTo(AccountWideUpgradePurchaseStatus.Purchased));

            bool resolved = resolver.TryResolve(gameState, out OfflineProgressClaimState claimState);

            Assert.That(resolved, Is.True);
            Assert.That(claimState.Amount, Is.EqualTo(6));
            Assert.That(claimState.CountedWholeHours, Is.EqualTo(OfflineProgressClaimResolver.MaximumClaimableWholeHours));
        }

        [Test]
        public void ShouldNotResolveClaimWhenElapsedTimeIsBelowOneWholeHour()
        {
            DateTimeOffset now = new DateTimeOffset(2026, 3, 29, 12, 0, 0, TimeSpan.Zero);
            OfflineProgressClaimResolver resolver = new OfflineProgressClaimResolver(
                BootstrapWorldTestData.CreateWorldGraph(),
                () => now);
            PersistentGameState gameState = CreateEligibleFarmReadySavedGameState(
                BootstrapWorldScenario.ForestFarmNodeId,
                now.AddMinutes(-59));

            Assert.That(resolver.TryResolve(gameState, out OfflineProgressClaimState claimState), Is.False);
            Assert.That(claimState, Is.Null);
        }

        [Test]
        public void ShouldFailClosedWhenEligibleSavedNodeDoesNotSupportRegionMaterialOutput()
        {
            DateTimeOffset now = new DateTimeOffset(2026, 3, 29, 12, 0, 0, TimeSpan.Zero);
            OfflineProgressClaimResolver resolver = new OfflineProgressClaimResolver(
                BootstrapWorldTestData.CreateWorldGraph(),
                () => now);
            PersistentGameState gameState = CreateEligibleFarmReadySavedGameState(
                BootstrapWorldScenario.CavernPushNodeId,
                now.AddHours(-2));

            Assert.That(resolver.TryResolve(gameState, out OfflineProgressClaimState claimState), Is.False);
            Assert.That(claimState, Is.Null);
        }

        [Test]
        public void ShouldFailClosedWhenSavedWorldNodeHasNoExplicitFarmYieldContent()
        {
            DateTimeOffset now = new DateTimeOffset(2026, 3, 29, 12, 0, 0, TimeSpan.Zero);
            OfflineProgressClaimResolver resolver = new OfflineProgressClaimResolver(
                BootstrapWorldTestData.CreateWorldGraph(),
                () => now);
            PersistentGameState gameState = CreateEligibleFarmReadySavedGameState(
                BootstrapWorldScenario.ForestPushNodeId,
                now.AddHours(-2));

            Assert.That(resolver.TryResolve(gameState, out OfflineProgressClaimState claimState), Is.False);
            Assert.That(claimState, Is.Null);
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
    }
}
