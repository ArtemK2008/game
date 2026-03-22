using System.Linq;
using NUnit.Framework;
using Survivalon.Core;
using Survivalon.State.Persistence;
using Survivalon.World;

namespace Survivalon.Tests.EditMode.World
{
    public sealed class BossProgressionGateUnlockServiceTests
    {
        [Test]
        public void ShouldUnlockConfiguredTargetNodeWhenBossDefeatSucceeds()
        {
            WorldGraph worldGraph = BootstrapWorldTestData.CreateWorldGraph();
            PersistentWorldState worldState = BootstrapWorldTestData.CreateWorldState();
            BossProgressionGateUnlockService service = new BossProgressionGateUnlockService();

            BossProgressionGateUnlockResult unlockResult = service.TryUnlockProgressionGate(
                NodePlaceholderTestData.CreateForestGateBossPlaceholderState(),
                didDefeatBoss: true,
                worldGraph,
                worldState);

            Assert.That(unlockResult.DidUnlock, Is.True);
            Assert.That(unlockResult.TryGetUnlockedNodeId(out NodeId unlockedNodeId), Is.True);
            Assert.That(unlockedNodeId, Is.EqualTo(BootstrapWorldScenario.CavernGateNodeId));
            Assert.That(worldState.TryGetNodeState(BootstrapWorldScenario.CavernGateNodeId, out PersistentNodeState unlockedNodeState), Is.True);
            Assert.That(unlockedNodeState.State, Is.EqualTo(NodeState.Available));
        }

        [Test]
        public void ShouldLeaveOrdinaryCombatNodesOutOfBossGateUnlockRule()
        {
            WorldGraph worldGraph = BootstrapWorldTestData.CreateWorldGraph();
            PersistentWorldState worldState = BootstrapWorldTestData.CreateWorldState();
            BossProgressionGateUnlockService service = new BossProgressionGateUnlockService();

            BossProgressionGateUnlockResult unlockResult = service.TryUnlockProgressionGate(
                NodePlaceholderTestData.CreateCombatPlaceholderState(),
                didDefeatBoss: true,
                worldGraph,
                worldState);

            Assert.That(unlockResult.DidUnlock, Is.False);
            Assert.That(unlockResult.TryGetUnlockedNodeId(out _), Is.False);
            Assert.That(worldState.TryGetNodeState(BootstrapWorldScenario.CavernGateNodeId, out PersistentNodeState cavernGateNodeState), Is.True);
            Assert.That(cavernGateNodeState.State, Is.EqualTo(NodeState.Locked));
        }

        [Test]
        public void ShouldMakeUnlockedGateReachableFromForestGateWorldContext()
        {
            WorldGraph worldGraph = BootstrapWorldTestData.CreateWorldGraph();
            PersistentWorldState worldState = BootstrapWorldTestData.CreateWorldState();
            BossProgressionGateUnlockService service = new BossProgressionGateUnlockService();
            NodeReachabilityResolver nodeReachabilityResolver = new NodeReachabilityResolver();

            worldState.SetCurrentNode(BootstrapWorldScenario.ForestGateNodeId);
            worldState.SetLastSafeNode(BootstrapWorldScenario.ForestPushNodeId);
            worldState.ReplaceReachableNodes(new[]
            {
                BootstrapWorldScenario.ForestPushNodeId,
                BootstrapWorldScenario.ForestFarmNodeId,
                BootstrapWorldScenario.CavernServiceNodeId,
            });

            service.TryUnlockProgressionGate(
                NodePlaceholderTestData.CreateForestGateBossPlaceholderState(),
                didDefeatBoss: true,
                worldGraph,
                worldState);

            NodeId[] reachableNodeIds = nodeReachabilityResolver
                .GetReachableNodes(worldGraph, worldState)
                .Select(node => node.NodeId)
                .ToArray();

            Assert.That(reachableNodeIds, Has.Member(BootstrapWorldScenario.CavernGateNodeId));
        }

        [Test]
        public void ShouldRepresentBossGateUnlockAsStructuredDomainData()
        {
            Assert.That(typeof(BossProgressionGateDefinition).GetProperty("UnlockedNodeId"), Is.Not.Null);
            Assert.That(typeof(BossProgressionGateDefinition).GetProperty("UnlockSummaryText"), Is.Null);
            Assert.That(
                typeof(BossProgressionGateUnlockService)
                    .GetMethod(nameof(BossProgressionGateUnlockService.TryUnlockProgressionGate))
                    .ReturnType,
                Is.EqualTo(typeof(BossProgressionGateUnlockResult)));
        }
    }
}
