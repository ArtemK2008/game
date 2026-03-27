using NUnit.Framework;
using Survivalon.Core;
using Survivalon.State.Persistence;
using Survivalon.World;
using Survivalon.Tests.EditMode.State.Persistence;

namespace Survivalon.Tests.EditMode.World
{
    public sealed class WorldNodeFarmReadinessResolverTests
    {
        [Test]
        public void ShouldMarkCompletedCombatContentAsFarmReady()
        {
            WorldNodeFarmReadinessResolver resolver = new WorldNodeFarmReadinessResolver();
            WorldGraph worldGraph = WorldFlowTestData.CreateFarmAccessGraph();
            PersistentWorldState clearedWorldState = WorldFlowTestData.CreateFarmAccessWorldState();
            PersistentWorldState masteredWorldState = PersistentStateTestData.CreateWorldState(
                new NodeId("node_current"),
                new[] { new NodeId("node_current") },
                PersistentStateTestData.CreateNodeState(new NodeId("node_cleared_farm"), 3, NodeState.Mastered, 3));

            Assert.That(
                resolver.IsFarmReady(worldGraph, clearedWorldState, new NodeId("node_cleared_farm")),
                Is.True);
            Assert.That(
                resolver.IsFarmReady(worldGraph, masteredWorldState, new NodeId("node_cleared_farm")),
                Is.True);
        }

        [Test]
        public void ShouldNotMarkUnclearedCombatContentAsFarmReady()
        {
            WorldNodeFarmReadinessResolver resolver = new WorldNodeFarmReadinessResolver();
            WorldGraph worldGraph = WorldFlowTestData.CreateFarmAccessGraph();
            PersistentWorldState worldState = WorldFlowTestData.CreateFarmAccessWorldState();

            Assert.That(
                resolver.IsFarmReady(worldGraph, worldState, new NodeId("node_reachable")),
                Is.False);
        }

        [Test]
        public void ShouldNotMarkBossOrServiceContentAsFarmReady()
        {
            WorldNodeFarmReadinessResolver resolver = new WorldNodeFarmReadinessResolver();
            RegionId regionId = new RegionId("region_test");
            NodeId serviceNodeId = new NodeId("service_node");
            NodeId bossNodeId = new NodeId("boss_node");
            WorldGraph worldGraph = new WorldGraph(
                new[]
                {
                    new WorldRegion(
                        regionId,
                        0,
                        serviceNodeId,
                        new[] { serviceNodeId, bossNodeId },
                        ResourceCategory.RegionMaterial,
                        "test")
                },
                new[]
                {
                    new WorldNode(serviceNodeId, regionId, NodeType.ServiceOrProgression, NodeState.Available),
                    new WorldNode(bossNodeId, regionId, NodeType.BossOrGate, NodeState.Available)
                },
                new WorldNodeConnection[0]);
            PersistentWorldState worldState = PersistentStateTestData.CreateWorldState(
                serviceNodeId,
                new[] { serviceNodeId },
                PersistentStateTestData.CreateNodeState(serviceNodeId, 1, NodeState.Cleared, 1),
                PersistentStateTestData.CreateNodeState(bossNodeId, 1, NodeState.Cleared, 1));

            Assert.That(resolver.IsFarmReady(worldGraph, worldState, serviceNodeId), Is.False);
            Assert.That(resolver.IsFarmReady(worldGraph, worldState, bossNodeId), Is.False);
        }
    }
}
