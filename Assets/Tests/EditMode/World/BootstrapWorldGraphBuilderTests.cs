using System.Linq;
using NUnit.Framework;
using Survivalon.Core;
using Survivalon.Data.Combat;
using Survivalon.World;

namespace Survivalon.Tests.EditMode.World
{
    public sealed class BootstrapWorldGraphBuilderTests
    {
        [Test]
        public void ShouldCreateExpectedBootstrapGraphTopology()
        {
            WorldGraph worldGraph = new BootstrapWorldGraphBuilder().Create();

            Assert.That(worldGraph.Regions.Count, Is.EqualTo(2));
            Assert.That(worldGraph.Nodes.Count, Is.EqualTo(6));
            Assert.That(worldGraph.Connections.Count, Is.EqualTo(5));

            AssertNode(worldGraph, BootstrapWorldScenario.ForestEntryNodeId, BootstrapWorldScenario.ForestRegionId, NodeType.Combat, NodeState.Cleared);
            AssertNode(worldGraph, BootstrapWorldScenario.ForestPushNodeId, BootstrapWorldScenario.ForestRegionId, NodeType.Combat, NodeState.InProgress);
            AssertNode(worldGraph, BootstrapWorldScenario.ForestGateNodeId, BootstrapWorldScenario.ForestRegionId, NodeType.BossOrGate, NodeState.Locked);
            AssertNode(worldGraph, BootstrapWorldScenario.ForestFarmNodeId, BootstrapWorldScenario.ForestRegionId, NodeType.Combat, NodeState.Available);
            AssertNode(worldGraph, BootstrapWorldScenario.CavernServiceNodeId, BootstrapWorldScenario.CavernRegionId, NodeType.ServiceOrProgression, NodeState.Available);
            AssertNode(worldGraph, BootstrapWorldScenario.CavernGateNodeId, BootstrapWorldScenario.CavernRegionId, NodeType.BossOrGate, NodeState.Locked);
            Assert.That(
                worldGraph.GetNode(BootstrapWorldScenario.ForestEntryNodeId).CombatEncounter,
                Is.SameAs(CombatStandardEncounterCatalog.EnemyUnitEncounter));
            Assert.That(
                worldGraph.GetNode(BootstrapWorldScenario.ForestPushNodeId).CombatEncounter,
                Is.SameAs(CombatStandardEncounterCatalog.BulwarkRaiderEncounter));
            Assert.That(
                worldGraph.GetNode(BootstrapWorldScenario.ForestGateNodeId).CombatEncounter,
                Is.SameAs(CombatBossEncounterCatalog.GateBossEncounter));
            Assert.That(
                worldGraph.GetNode(BootstrapWorldScenario.ForestFarmNodeId).CombatEncounter,
                Is.SameAs(CombatStandardEncounterCatalog.EnemyUnitEncounter));
            Assert.That(
                worldGraph.GetNode(BootstrapWorldScenario.CavernServiceNodeId).CombatEncounter,
                Is.Null);
            Assert.That(
                worldGraph.GetNode(BootstrapWorldScenario.CavernGateNodeId).CombatEncounter,
                Is.SameAs(CombatBossEncounterCatalog.GateBossEncounter));
            Assert.That(
                worldGraph.GetNode(BootstrapWorldScenario.ForestGateNodeId).CombatEncounter.EncounterType,
                Is.EqualTo(CombatEncounterType.Boss));
            Assert.That(
                worldGraph.GetNode(BootstrapWorldScenario.ForestGateNodeId).CombatEncounter.PrimaryEnemyProfile.HostileEntityType,
                Is.EqualTo(CombatHostileEntityType.Boss));

            WorldRegion forestRegion = worldGraph.Regions.Single(region => region.RegionId == BootstrapWorldScenario.ForestRegionId);
            WorldRegion cavernRegion = worldGraph.Regions.Single(region => region.RegionId == BootstrapWorldScenario.CavernRegionId);

            Assert.That(forestRegion.ProgressionOrder, Is.EqualTo(0));
            Assert.That(forestRegion.EntryNodeId, Is.EqualTo(BootstrapWorldScenario.ForestEntryNodeId));
            Assert.That(forestRegion.ResourceCategory, Is.EqualTo(ResourceCategory.RegionMaterial));
            Assert.That(forestRegion.DifficultyBand, Is.EqualTo("frontier"));
            Assert.That(forestRegion.NodeIds, Is.EqualTo(new[]
            {
                BootstrapWorldScenario.ForestEntryNodeId,
                BootstrapWorldScenario.ForestPushNodeId,
                BootstrapWorldScenario.ForestGateNodeId,
                BootstrapWorldScenario.ForestFarmNodeId,
            }));

            Assert.That(cavernRegion.ProgressionOrder, Is.EqualTo(1));
            Assert.That(cavernRegion.EntryNodeId, Is.EqualTo(BootstrapWorldScenario.CavernServiceNodeId));
            Assert.That(cavernRegion.ResourceCategory, Is.EqualTo(ResourceCategory.PersistentProgressionMaterial));
            Assert.That(cavernRegion.DifficultyBand, Is.EqualTo("depths"));
            Assert.That(cavernRegion.NodeIds, Is.EqualTo(new[]
            {
                BootstrapWorldScenario.CavernServiceNodeId,
                BootstrapWorldScenario.CavernGateNodeId,
            }));

            AssertConnection(worldGraph, BootstrapWorldScenario.ForestEntryNodeId, BootstrapWorldScenario.ForestPushNodeId);
            AssertConnection(worldGraph, BootstrapWorldScenario.ForestPushNodeId, BootstrapWorldScenario.ForestGateNodeId);
            AssertConnection(worldGraph, BootstrapWorldScenario.ForestPushNodeId, BootstrapWorldScenario.ForestFarmNodeId);
            AssertConnection(worldGraph, BootstrapWorldScenario.ForestPushNodeId, BootstrapWorldScenario.CavernServiceNodeId);
            AssertConnection(worldGraph, BootstrapWorldScenario.CavernServiceNodeId, BootstrapWorldScenario.CavernGateNodeId);
        }

        private static void AssertNode(
            WorldGraph worldGraph,
            NodeId nodeId,
            RegionId regionId,
            NodeType nodeType,
            NodeState nodeState)
        {
            WorldNode node = worldGraph.GetNode(nodeId);
            Assert.That(node.RegionId, Is.EqualTo(regionId));
            Assert.That(node.NodeType, Is.EqualTo(nodeType));
            Assert.That(node.State, Is.EqualTo(nodeState));
        }

        private static void AssertConnection(WorldGraph worldGraph, NodeId sourceNodeId, NodeId targetNodeId)
        {
            Assert.That(
                worldGraph.GetOutboundConnections(sourceNodeId).Any(connection => connection.TargetNodeId == targetNodeId),
                Is.True);
        }
    }
}

