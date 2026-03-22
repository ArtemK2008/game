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

            string unlockSummary = service.TryUnlockProgressionGate(
                NodePlaceholderTestData.CreateForestGateBossPlaceholderState(),
                didDefeatBoss: true,
                worldGraph,
                worldState);

            Assert.That(unlockSummary, Is.EqualTo("Cavern gate opened"));
            Assert.That(worldState.TryGetNodeState(BootstrapWorldScenario.CavernGateNodeId, out PersistentNodeState unlockedNodeState), Is.True);
            Assert.That(unlockedNodeState.State, Is.EqualTo(NodeState.Available));
        }

        [Test]
        public void ShouldLeaveOrdinaryCombatNodesOutOfBossGateUnlockRule()
        {
            WorldGraph worldGraph = BootstrapWorldTestData.CreateWorldGraph();
            PersistentWorldState worldState = BootstrapWorldTestData.CreateWorldState();
            BossProgressionGateUnlockService service = new BossProgressionGateUnlockService();

            string unlockSummary = service.TryUnlockProgressionGate(
                NodePlaceholderTestData.CreateCombatPlaceholderState(),
                didDefeatBoss: true,
                worldGraph,
                worldState);

            Assert.That(unlockSummary, Is.Empty);
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
    }
}
