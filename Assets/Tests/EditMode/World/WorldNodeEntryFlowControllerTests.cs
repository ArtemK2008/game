using NUnit.Framework;
using Survivalon.Core;
using Survivalon.Data.Combat;
using Survivalon.Data.World;
using Survivalon.State.Persistence;
using Survivalon.World;
using Survivalon.Tests.EditMode.State.Persistence;

namespace Survivalon.Tests.EditMode.World
{
    public sealed class WorldNodeEntryFlowControllerTests
    {
        [Test]
        public void ShouldRejectMissingWorldGraphDuringConstruction()
        {
            Assert.That(
                () => new WorldNodeEntryFlowController(null, new PersistentWorldState()),
                Throws.ArgumentNullException.With.Property("ParamName").EqualTo("worldGraph"));
        }

        [Test]
        public void ShouldRejectMissingWorldStateDuringConstruction()
        {
            Assert.That(
                () => new WorldNodeEntryFlowController(BootstrapWorldTestData.CreateWorldGraph(), null),
                Throws.ArgumentNullException.With.Property("ParamName").EqualTo("worldState"));
        }

        [Test]
        public void ShouldEnterReachableNodeAndUpdateWorldContext()
        {
            WorldGraph worldGraph = BootstrapWorldTestData.CreateWorldGraph();
            PersistentWorldState worldState = BootstrapWorldTestData.CreateWorldState();
            WorldNodeEntryFlowController controller = new WorldNodeEntryFlowController(worldGraph, worldState);
            NodeId selectedNodeId = new NodeId("region_002_node_001");

            bool entered = controller.TryEnterNode(selectedNodeId, out NodePlaceholderState placeholderState);

            Assert.That(entered, Is.True);
            Assert.That(placeholderState, Is.Not.Null);
            Assert.That(placeholderState.NodeId, Is.EqualTo(selectedNodeId));
            Assert.That(placeholderState.NodeDisplayName, Is.EqualTo("Cavern Service Hub"));
            Assert.That(placeholderState.LocationIdentity.DisplayName, Is.EqualTo("Echo Caverns"));
            Assert.That(placeholderState.LocationIdentity.EnemyEmphasisDisplayName, Is.EqualTo("Gate guardians"));
            Assert.That(placeholderState.LocationIdentity.IsFallbackIdentity, Is.False);
            Assert.That(placeholderState.OriginNodeId, Is.EqualTo(new NodeId("region_001_node_002")));
            Assert.That(worldState.CurrentNodeId, Is.EqualTo(selectedNodeId));
            Assert.That(worldState.LastSafeNodeId, Is.EqualTo(new NodeId("region_001_node_002")));
            Assert.That(worldState.ReachableNodeIdValues, Does.Contain("region_001_node_001"));
            Assert.That(worldState.ReachableNodeIdValues, Does.Contain("region_001_node_002"));
        }

        [Test]
        public void ShouldCarryBootstrapCombatEncounterContentIntoPlaceholderState()
        {
            WorldGraph worldGraph = BootstrapWorldTestData.CreateWorldGraph();
            PersistentWorldState worldState = BootstrapWorldTestData.CreateWorldState();
            WorldNodeEntryFlowController controller = new WorldNodeEntryFlowController(worldGraph, worldState);

            bool entered = controller.TryEnterNode(BootstrapWorldScenario.ForestFarmNodeId, out NodePlaceholderState placeholderState);

            Assert.That(entered, Is.True);
            Assert.That(placeholderState, Is.Not.Null);
            Assert.That(placeholderState.NodeDisplayName, Is.EqualTo("Forest Farm"));
            Assert.That(placeholderState.CombatEncounter, Is.SameAs(CombatStandardEncounterCatalog.EnemyUnitEncounter));
            Assert.That(placeholderState.LocationIdentity, Is.SameAs(LocationIdentityCatalog.VerdantFrontier));
            Assert.That(placeholderState.LocationIdentity.EnemyEmphasisDisplayName, Is.EqualTo("Frontier raiders"));
            Assert.That(placeholderState.LocationIdentity.IsFallbackIdentity, Is.False);
            Assert.That(placeholderState.RegionMaterialYieldContent, Is.Not.Null);
            Assert.That(placeholderState.RegionMaterialYieldContent.RegionMaterialBonus, Is.EqualTo(1));
            Assert.That(placeholderState.SupportsRegionMaterialRewards, Is.True);
            Assert.That(
                placeholderState.CombatEncounter.PrimaryEnemyProfile,
                Is.SameAs(CombatStandardEnemyProfileCatalog.EnemyUnit));
        }

        [Test]
        public void ShouldCarryBootstrapBossEncounterContentIntoPlaceholderState()
        {
            WorldGraph worldGraph = BootstrapWorldTestData.CreateWorldGraph();
            PersistentWorldState worldState = BootstrapWorldTestData.CreateWorldState();
            WorldNodeEntryFlowController controller = new WorldNodeEntryFlowController(worldGraph, worldState);

            worldState.ReplaceNodeStates(new[]
            {
                PersistentStateTestData.CreateNodeState(
                    BootstrapWorldScenario.ForestGateNodeId,
                    unlockThreshold: 3,
                    nodeState: NodeState.Available,
                    unlockProgress: 0),
            });

            bool entered = controller.TryEnterNode(BootstrapWorldScenario.ForestGateNodeId, out NodePlaceholderState placeholderState);

            Assert.That(entered, Is.True);
            Assert.That(placeholderState, Is.Not.Null);
            Assert.That(placeholderState.NodeDisplayName, Is.EqualTo("Frontier Gate"));
            Assert.That(placeholderState.CombatEncounter, Is.SameAs(CombatBossEncounterCatalog.GateBossEncounter));
            Assert.That(placeholderState.CombatEncounter.EncounterType, Is.EqualTo(CombatEncounterType.Boss));
            Assert.That(
                placeholderState.CombatEncounter.PrimaryEnemyProfile,
                Is.SameAs(CombatBossProfileCatalog.GateBoss));
            Assert.That(placeholderState.BossProgressionGate, Is.Not.Null);
            Assert.That(placeholderState.LocationIdentity, Is.SameAs(LocationIdentityCatalog.VerdantFrontier));
            Assert.That(placeholderState.LocationIdentity.EnemyEmphasisDisplayName, Is.EqualTo("Frontier raiders"));
            Assert.That(placeholderState.LocationIdentity.IsFallbackIdentity, Is.False);
            Assert.That(placeholderState.BossRewardContent, Is.Null);
            Assert.That(placeholderState.RegionMaterialYieldContent, Is.Null);
            Assert.That(placeholderState.SupportsRegionMaterialRewards, Is.False);
            Assert.That(
                placeholderState.BossProgressionGate.UnlockedNodeId,
                Is.EqualTo(BootstrapWorldScenario.CavernGateNodeId));
        }

        [Test]
        public void ShouldCarryBootstrapCavernBossRewardContentIntoPlaceholderState()
        {
            WorldGraph worldGraph = BootstrapWorldTestData.CreateWorldGraph();
            PersistentWorldState worldState = BootstrapWorldTestData.CreateWorldState();
            WorldNodeEntryFlowController controller = new WorldNodeEntryFlowController(worldGraph, worldState);

            worldState.ReplaceNodeStates(new[]
            {
                PersistentStateTestData.CreateNodeState(
                    BootstrapWorldScenario.CavernGateNodeId,
                    unlockThreshold: 3,
                    nodeState: NodeState.Available,
                    unlockProgress: 0),
            });
            worldState.SetCurrentNode(BootstrapWorldScenario.CavernServiceNodeId);
            worldState.SetLastSafeNode(BootstrapWorldScenario.ForestPushNodeId);
            worldState.ReplaceReachableNodes(new[]
            {
                BootstrapWorldScenario.CavernServiceNodeId,
            });

            bool entered = controller.TryEnterNode(BootstrapWorldScenario.CavernGateNodeId, out NodePlaceholderState placeholderState);

            Assert.That(entered, Is.True);
            Assert.That(placeholderState, Is.Not.Null);
            Assert.That(placeholderState.NodeDisplayName, Is.EqualTo("Cavern Gate"));
            Assert.That(placeholderState.LocationIdentity, Is.SameAs(LocationIdentityCatalog.EchoCaverns));
            Assert.That(placeholderState.LocationIdentity.IsFallbackIdentity, Is.False);
            Assert.That(placeholderState.BossRewardContent, Is.Not.Null);
            Assert.That(placeholderState.BossRewardContent.PersistentProgressionMaterialBonus, Is.EqualTo(1));
        }

        [Test]
        public void ShouldCarryBootstrapTownServiceContextIntoPlaceholderState()
        {
            WorldGraph worldGraph = BootstrapWorldTestData.CreateWorldGraph();
            PersistentWorldState worldState = BootstrapWorldTestData.CreateWorldState();
            WorldNodeEntryFlowController controller = new WorldNodeEntryFlowController(worldGraph, worldState);

            bool entered = controller.TryEnterNode(BootstrapWorldScenario.CavernServiceNodeId, out NodePlaceholderState placeholderState);

            Assert.That(entered, Is.True);
            Assert.That(placeholderState, Is.Not.Null);
            Assert.That(placeholderState.CombatEncounter, Is.Null);
            Assert.That(placeholderState.BossProgressionGate, Is.Null);
            Assert.That(placeholderState.TownServiceContext, Is.Not.Null);
            Assert.That(placeholderState.TownServiceContext.ContextId, Is.EqualTo("town_service_cavern_hub"));
            Assert.That(placeholderState.TownServiceContext.DisplayName, Is.EqualTo("Cavern Service Hub"));
            Assert.That(placeholderState.LocationIdentity, Is.SameAs(LocationIdentityCatalog.EchoCaverns));
            Assert.That(placeholderState.LocationIdentity.EnemyEmphasisDisplayName, Is.EqualTo("Gate guardians"));
            Assert.That(placeholderState.LocationIdentity.IsFallbackIdentity, Is.False);
            Assert.That(placeholderState.BossRewardContent, Is.Null);
            Assert.That(placeholderState.RegionMaterialYieldContent, Is.Null);
            Assert.That(placeholderState.SupportsRegionMaterialRewards, Is.False);
        }

        [Test]
        public void ShouldRejectEnteringLockedOrUnreachableNode()
        {
            WorldGraph worldGraph = BootstrapWorldTestData.CreateWorldGraph();
            PersistentWorldState worldState = BootstrapWorldTestData.CreateWorldState();
            WorldNodeEntryFlowController controller = new WorldNodeEntryFlowController(worldGraph, worldState);

            bool enteredLockedNode = controller.TryEnterNode(new NodeId("region_001_node_003"), out NodePlaceholderState lockedNodeState);

            Assert.That(enteredLockedNode, Is.False);
            Assert.That(lockedNodeState, Is.Null);
            Assert.That(worldState.CurrentNodeId, Is.EqualTo(new NodeId("region_001_node_002")));
            Assert.That(worldState.LastSafeNodeId, Is.EqualTo(new NodeId("region_001_node_001")));
        }

        [Test]
        public void ShouldAllowReenteringCurrentContextNodeForLowFrictionReplay()
        {
            WorldGraph worldGraph = BootstrapWorldTestData.CreateWorldGraph();
            PersistentWorldState worldState = BootstrapWorldTestData.CreateWorldState();
            WorldNodeEntryFlowController controller = new WorldNodeEntryFlowController(worldGraph, worldState);
            NodeId currentNodeId = new NodeId("region_001_node_002");

            bool entered = controller.TryEnterNode(currentNodeId, out NodePlaceholderState placeholderState);

            Assert.That(entered, Is.True);
            Assert.That(placeholderState, Is.Not.Null);
            Assert.That(placeholderState.NodeId, Is.EqualTo(currentNodeId));
            Assert.That(placeholderState.OriginNodeId, Is.EqualTo(currentNodeId));
            Assert.That(worldState.CurrentNodeId, Is.EqualTo(currentNodeId));
            Assert.That(worldState.LastSafeNodeId, Is.EqualTo(currentNodeId));
        }

        [Test]
        public void ShouldAllowEnteringReachableClearedNodeWithoutRegressingPersistentState()
        {
            WorldGraph worldGraph = BootstrapWorldTestData.CreateWorldGraph();
            PersistentWorldState worldState = BootstrapWorldTestData.CreateWorldState();
            WorldNodeEntryFlowController controller = new WorldNodeEntryFlowController(worldGraph, worldState);
            NodeId clearedNodeId = new NodeId("region_001_node_001");

            Assert.That(worldState.TryGetNodeState(clearedNodeId, out PersistentNodeState clearedNodeStateBeforeEntry), Is.True);
            Assert.That(clearedNodeStateBeforeEntry.State, Is.EqualTo(NodeState.Cleared));
            Assert.That(clearedNodeStateBeforeEntry.UnlockProgress, Is.EqualTo(clearedNodeStateBeforeEntry.UnlockThreshold));

            bool entered = controller.TryEnterNode(clearedNodeId, out NodePlaceholderState placeholderState);

            Assert.That(entered, Is.True);
            Assert.That(placeholderState, Is.Not.Null);
            Assert.That(placeholderState.NodeId, Is.EqualTo(clearedNodeId));
            Assert.That(placeholderState.NodeState, Is.EqualTo(NodeState.Cleared));
            Assert.That(worldState.CurrentNodeId, Is.EqualTo(clearedNodeId));
            Assert.That(worldState.LastSafeNodeId, Is.EqualTo(new NodeId("region_001_node_002")));
            Assert.That(worldState.TryGetNodeState(clearedNodeId, out PersistentNodeState clearedNodeStateAfterEntry), Is.True);
            Assert.That(clearedNodeStateAfterEntry.State, Is.EqualTo(NodeState.Cleared));
            Assert.That(clearedNodeStateAfterEntry.UnlockProgress, Is.EqualTo(clearedNodeStateAfterEntry.UnlockThreshold));
        }

        [Test]
        public void ShouldAllowEnteringPersistentlyClearedNodeEvenWhenItIsNotPathReachable()
        {
            WorldGraph worldGraph = WorldFlowTestData.CreateFarmAccessGraph();
            PersistentWorldState worldState = WorldFlowTestData.CreateFarmAccessWorldState();
            WorldNodeEntryFlowController controller = new WorldNodeEntryFlowController(worldGraph, worldState);
            NodeId clearedFarmNodeId = new NodeId("node_cleared_farm");

            bool entered = controller.TryEnterNode(clearedFarmNodeId, out NodePlaceholderState placeholderState);

            Assert.That(entered, Is.True);
            Assert.That(placeholderState, Is.Not.Null);
            Assert.That(placeholderState.NodeId, Is.EqualTo(clearedFarmNodeId));
            Assert.That(placeholderState.NodeState, Is.EqualTo(NodeState.Cleared));
            Assert.That(worldState.CurrentNodeId, Is.EqualTo(clearedFarmNodeId));
            Assert.That(worldState.LastSafeNodeId, Is.EqualTo(new NodeId("node_current")));
            Assert.That(worldState.TryGetNodeState(clearedFarmNodeId, out PersistentNodeState clearedNodeState), Is.True);
            Assert.That(clearedNodeState.State, Is.EqualTo(NodeState.Cleared));
            Assert.That(clearedNodeState.UnlockProgress, Is.EqualTo(clearedNodeState.UnlockThreshold));
        }

        [Test]
        public void ShouldRejectUnavailableNodeWhenItIsNotReachableAndNotPersistentlyCleared()
        {
            WorldGraph worldGraph = WorldFlowTestData.CreateFarmAccessGraph();
            PersistentWorldState worldState = WorldFlowTestData.CreateFarmAccessWorldState();
            WorldNodeEntryFlowController controller = new WorldNodeEntryFlowController(worldGraph, worldState);

            bool enteredUnreachableAvailableNode = controller.TryEnterNode(new NodeId("node_unreachable_available"), out NodePlaceholderState unavailableNodeState);
            bool enteredLockedNode = controller.TryEnterNode(new NodeId("node_locked"), out NodePlaceholderState lockedNodeState);

            Assert.That(enteredUnreachableAvailableNode, Is.False);
            Assert.That(enteredLockedNode, Is.False);
            Assert.That(unavailableNodeState, Is.Null);
            Assert.That(lockedNodeState, Is.Null);
            Assert.That(worldState.CurrentNodeId, Is.EqualTo(new NodeId("node_current")));
        }

        [Test]
        public void ShouldUseLastSafeNodeAsOriginWhenCurrentNodeIsMissing()
        {
            WorldGraph worldGraph = WorldFlowTestData.CreateFarmAccessGraph();
            PersistentWorldState worldState = new PersistentWorldState();
            WorldNodeEntryFlowController controller = new WorldNodeEntryFlowController(worldGraph, worldState);
            NodeId lastSafeNodeId = new NodeId("node_current");

            worldState.SetLastSafeNode(lastSafeNodeId);
            worldState.ReplaceReachableNodes(new[] { lastSafeNodeId });
            worldState.ReplaceNodeStates(new[]
            {
                PersistentStateTestData.CreateNodeState(new NodeId("node_cleared_farm"), 3, NodeState.Cleared, 3),
            });

            bool entered = controller.TryEnterNode(new NodeId("node_cleared_farm"), out NodePlaceholderState placeholderState);

            Assert.That(entered, Is.True);
            Assert.That(placeholderState.OriginNodeId, Is.EqualTo(lastSafeNodeId));
            Assert.That(worldState.LastSafeNodeId, Is.EqualTo(lastSafeNodeId));
        }

        [Test]
        public void ShouldRejectEntryWhenCurrentAndLastSafeContextAreMissing()
        {
            WorldGraph worldGraph = WorldFlowTestData.CreateFarmAccessGraph();
            PersistentWorldState worldState = new PersistentWorldState();
            WorldNodeEntryFlowController controller = new WorldNodeEntryFlowController(worldGraph, worldState);

            worldState.ReplaceNodeStates(new[]
            {
                PersistentStateTestData.CreateNodeState(new NodeId("node_cleared_farm"), 3, NodeState.Cleared, 3),
            });

            Assert.That(
                () => controller.TryEnterNode(new NodeId("node_cleared_farm"), out _),
                Throws.InvalidOperationException.With.Message.Contains("current node or last safe node"));
        }
    }
}

