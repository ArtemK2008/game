using System.Collections.Generic;
using Survivalon.Runtime.Combat;
using Survivalon.Runtime.Core;
using Survivalon.Runtime.Run;
using Survivalon.Runtime.State;
using Survivalon.Runtime.State.Persistence;

namespace Survivalon.Runtime.World
{
    public sealed class BootstrapWorldGraphBuilder
    {
        public WorldGraph Create()
        {
            List<WorldNode> nodes = new List<WorldNode>
            {
                new WorldNode(BootstrapWorldScenario.ForestEntryNodeId, BootstrapWorldScenario.ForestRegionId, NodeType.Combat, NodeState.Cleared),
                new WorldNode(BootstrapWorldScenario.ForestPushNodeId, BootstrapWorldScenario.ForestRegionId, NodeType.Combat, NodeState.InProgress),
                new WorldNode(BootstrapWorldScenario.ForestGateNodeId, BootstrapWorldScenario.ForestRegionId, NodeType.BossOrGate, NodeState.Locked),
                new WorldNode(BootstrapWorldScenario.ForestFarmNodeId, BootstrapWorldScenario.ForestRegionId, NodeType.Combat, NodeState.Available),
                new WorldNode(BootstrapWorldScenario.CavernServiceNodeId, BootstrapWorldScenario.CavernRegionId, NodeType.ServiceOrProgression, NodeState.Available),
                new WorldNode(BootstrapWorldScenario.CavernGateNodeId, BootstrapWorldScenario.CavernRegionId, NodeType.BossOrGate, NodeState.Locked),
            };

            List<WorldRegion> regions = new List<WorldRegion>
            {
                new WorldRegion(
                    BootstrapWorldScenario.ForestRegionId,
                    0,
                    BootstrapWorldScenario.ForestEntryNodeId,
                    new[]
                    {
                        BootstrapWorldScenario.ForestEntryNodeId,
                        BootstrapWorldScenario.ForestPushNodeId,
                        BootstrapWorldScenario.ForestGateNodeId,
                        BootstrapWorldScenario.ForestFarmNodeId,
                    },
                    ResourceCategory.RegionMaterial,
                    "frontier"),
                new WorldRegion(
                    BootstrapWorldScenario.CavernRegionId,
                    1,
                    BootstrapWorldScenario.CavernServiceNodeId,
                    new[]
                    {
                        BootstrapWorldScenario.CavernServiceNodeId,
                        BootstrapWorldScenario.CavernGateNodeId,
                    },
                    ResourceCategory.PersistentProgressionMaterial,
                    "depths"),
            };

            List<WorldNodeConnection> connections = new List<WorldNodeConnection>
            {
                new WorldNodeConnection(BootstrapWorldScenario.ForestEntryNodeId, BootstrapWorldScenario.ForestPushNodeId),
                new WorldNodeConnection(BootstrapWorldScenario.ForestPushNodeId, BootstrapWorldScenario.ForestGateNodeId),
                new WorldNodeConnection(BootstrapWorldScenario.ForestPushNodeId, BootstrapWorldScenario.ForestFarmNodeId),
                new WorldNodeConnection(BootstrapWorldScenario.ForestPushNodeId, BootstrapWorldScenario.CavernServiceNodeId),
                new WorldNodeConnection(BootstrapWorldScenario.CavernServiceNodeId, BootstrapWorldScenario.CavernGateNodeId),
            };

            return new WorldGraph(regions, nodes, connections);
        }
    }
}
