using Survivalon.Core;
using Survivalon.Data.Towns;
using Survivalon.State.Persistence;
using Survivalon.World;

namespace Survivalon.Run
{
    public sealed class PostRunServiceOpportunityResolver
    {
        private readonly WorldNodeAccessResolver worldNodeAccessResolver;
        private readonly WorldNodeDisplayNameResolver worldNodeDisplayNameResolver;

        public PostRunServiceOpportunityResolver(
            WorldNodeAccessResolver worldNodeAccessResolver = null,
            WorldNodeDisplayNameResolver worldNodeDisplayNameResolver = null)
        {
            this.worldNodeAccessResolver = worldNodeAccessResolver ?? new WorldNodeAccessResolver();
            this.worldNodeDisplayNameResolver = worldNodeDisplayNameResolver ?? new WorldNodeDisplayNameResolver();
        }

        public PostRunServiceOpportunityState Resolve(
            WorldGraph worldGraph,
            PersistentWorldState persistentWorldState,
            ResourceBalancesState resourceBalancesState,
            PersistentProgressionState persistentProgressionState)
        {
            string serviceHubDisplayName = ResolveServiceHubDisplayName(worldGraph, persistentWorldState);
            if (string.IsNullOrWhiteSpace(serviceHubDisplayName) ||
                resourceBalancesState == null ||
                persistentProgressionState == null)
            {
                return new PostRunServiceOpportunityState(serviceHubDisplayName);
            }

            bool hasAffordableProject = HasAffordableProject(resourceBalancesState, persistentProgressionState);
            bool hasReadyRefinement = HasReadyRefinement(resourceBalancesState);

            if (hasAffordableProject && hasReadyRefinement)
            {
                return new PostRunServiceOpportunityState(
                    serviceHubDisplayName,
                    PostRunServiceOpportunityKind.AffordableProjectAndReadyRefinement);
            }

            if (hasAffordableProject)
            {
                return new PostRunServiceOpportunityState(
                    serviceHubDisplayName,
                    PostRunServiceOpportunityKind.AffordableProject);
            }

            return hasReadyRefinement
                ? new PostRunServiceOpportunityState(
                    serviceHubDisplayName,
                    PostRunServiceOpportunityKind.ReadyRefinement)
                : new PostRunServiceOpportunityState(serviceHubDisplayName);
        }

        private string ResolveServiceHubDisplayName(WorldGraph worldGraph, PersistentWorldState persistentWorldState)
        {
            if (worldGraph == null || persistentWorldState == null || !HasWorldAnchor(persistentWorldState))
            {
                return null;
            }

            var enterableNodes = worldNodeAccessResolver.GetEnterableNodes(worldGraph, persistentWorldState);
            for (int index = 0; index < enterableNodes.Count; index++)
            {
                WorldNode node = enterableNodes[index];
                if (node.NodeType == NodeType.ServiceOrProgression && node.TownServiceContext != null)
                {
                    return worldNodeDisplayNameResolver.Resolve(node);
                }
            }

            return null;
        }

        private static bool HasAffordableProject(
            ResourceBalancesState resourceBalancesState,
            PersistentProgressionState persistentProgressionState)
        {
            for (int index = 0; index < AccountWideProgressionUpgradeCatalog.All.Count; index++)
            {
                AccountWideProgressionUpgradeDefinition upgradeDefinition =
                    AccountWideProgressionUpgradeCatalog.All[index];

                if (IsPurchased(persistentProgressionState, upgradeDefinition.ProgressionId))
                {
                    continue;
                }

                if (resourceBalancesState.GetAmount(upgradeDefinition.CostResourceCategory) >= upgradeDefinition.CostAmount)
                {
                    return true;
                }
            }

            return false;
        }

        private static bool HasReadyRefinement(ResourceBalancesState resourceBalancesState)
        {
            TownServiceConversionDefinition refinementDefinition =
                TownServiceConversionCatalog.Get(TownServiceConversionId.RegionMaterialRefinement);
            return resourceBalancesState.GetAmount(refinementDefinition.InputResourceCategory) >=
                refinementDefinition.InputAmount;
        }

        private static bool IsPurchased(PersistentProgressionState persistentProgressionState, string progressionId)
        {
            return persistentProgressionState.TryGetEntry(progressionId, out ProgressionEntryState entry) &&
                entry.IsUnlocked &&
                entry.CurrentValue > 0;
        }

        private static bool HasWorldAnchor(PersistentWorldState persistentWorldState)
        {
            return persistentWorldState.HasCurrentNode || persistentWorldState.HasLastSafeNode;
        }
    }
}
