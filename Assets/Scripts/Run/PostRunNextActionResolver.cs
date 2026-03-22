using System;
using System.Collections.Generic;
using Survivalon.Core;
using Survivalon.Data.Towns;
using Survivalon.State.Persistence;
using Survivalon.World;

namespace Survivalon.Run
{
    public sealed class PostRunNextActionResolver
    {
        private readonly WorldNodeAccessResolver worldNodeAccessResolver;
        private readonly WorldNodeDisplayNameResolver worldNodeDisplayNameResolver;

        public PostRunNextActionResolver(
            WorldNodeAccessResolver worldNodeAccessResolver = null,
            WorldNodeDisplayNameResolver worldNodeDisplayNameResolver = null)
        {
            this.worldNodeAccessResolver = worldNodeAccessResolver ?? new WorldNodeAccessResolver();
            this.worldNodeDisplayNameResolver = worldNodeDisplayNameResolver ?? new WorldNodeDisplayNameResolver();
        }

        public PostRunNextActionState Resolve(
            PostRunStateController postRunStateController,
            WorldGraph worldGraph,
            PersistentWorldState persistentWorldState,
            ResourceBalancesState resourceBalancesState,
            PersistentProgressionState persistentProgressionState)
        {
            if (postRunStateController == null)
            {
                throw new ArgumentNullException(nameof(postRunStateController));
            }

            RunResult runResult = postRunStateController.RunResult;
            string forwardTargetDisplayName = ResolveForwardTargetDisplayName(
                runResult,
                worldGraph,
                persistentWorldState);
            string serviceHubDisplayName = ResolveServiceHubDisplayName(worldGraph, persistentWorldState);
            PostRunServiceOpportunityKind serviceOpportunityKind = ResolveServiceOpportunityKind(
                serviceHubDisplayName,
                resourceBalancesState,
                persistentProgressionState);
            PostRunReplayReasonKind replayReasonKind = ResolveReplayReason(postRunStateController);

            return new PostRunNextActionState(
                postRunStateController.NodeContext.NodeDisplayName,
                postRunStateController.CanReplayNode,
                postRunStateController.CanReturnToWorld,
                postRunStateController.CanStopSession,
                ResolveRecommendedActionKind(
                    postRunStateController,
                    forwardTargetDisplayName,
                    serviceOpportunityKind),
                replayReasonKind,
                forwardTargetDisplayName,
                serviceHubDisplayName,
                serviceOpportunityKind);
        }

        private string ResolveForwardTargetDisplayName(
            RunResult runResult,
            WorldGraph worldGraph,
            PersistentWorldState persistentWorldState)
        {
            if (runResult == null || worldGraph == null)
            {
                return null;
            }

            if (runResult.HasBossProgressionGateUnlock &&
                runResult.BossProgressionGateUnlock.TryGetUnlockedNodeId(out NodeId unlockedNodeId))
            {
                return worldNodeDisplayNameResolver.Resolve(worldGraph, unlockedNodeId);
            }

            if (persistentWorldState == null)
            {
                return null;
            }

            if (!HasWorldAnchor(persistentWorldState))
            {
                return null;
            }

            IReadOnlyList<WorldNode> forwardNodes =
                worldNodeAccessResolver.GetForwardEnterableNodes(worldGraph, persistentWorldState);

            WorldNode preferredNode = null;
            int preferredNodeScore = int.MinValue;
            for (int index = 0; index < forwardNodes.Count; index++)
            {
                WorldNode candidate = forwardNodes[index];
                int score = ScoreForwardTarget(candidate);
                if (score <= preferredNodeScore)
                {
                    continue;
                }

                preferredNode = candidate;
                preferredNodeScore = score;
            }

            return preferredNode == null
                ? null
                : worldNodeDisplayNameResolver.Resolve(preferredNode);
        }

        private string ResolveServiceHubDisplayName(WorldGraph worldGraph, PersistentWorldState persistentWorldState)
        {
            if (worldGraph == null || persistentWorldState == null)
            {
                return null;
            }

            if (!HasWorldAnchor(persistentWorldState))
            {
                return null;
            }

            IReadOnlyList<WorldNode> enterableNodes =
                worldNodeAccessResolver.GetEnterableNodes(worldGraph, persistentWorldState);
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

        private static PostRunServiceOpportunityKind ResolveServiceOpportunityKind(
            string serviceHubDisplayName,
            ResourceBalancesState resourceBalancesState,
            PersistentProgressionState persistentProgressionState)
        {
            if (string.IsNullOrWhiteSpace(serviceHubDisplayName) ||
                resourceBalancesState == null ||
                persistentProgressionState == null)
            {
                return PostRunServiceOpportunityKind.None;
            }

            bool hasAffordableProject = HasAffordableProject(resourceBalancesState, persistentProgressionState);
            bool hasReadyRefinement = HasReadyRefinement(resourceBalancesState);

            if (hasAffordableProject && hasReadyRefinement)
            {
                return PostRunServiceOpportunityKind.AffordableProjectAndReadyRefinement;
            }

            if (hasAffordableProject)
            {
                return PostRunServiceOpportunityKind.AffordableProject;
            }

            return hasReadyRefinement
                ? PostRunServiceOpportunityKind.ReadyRefinement
                : PostRunServiceOpportunityKind.None;
        }

        private static PostRunReplayReasonKind ResolveReplayReason(PostRunStateController postRunStateController)
        {
            if (postRunStateController == null)
            {
                throw new ArgumentNullException(nameof(postRunStateController));
            }

            if (!postRunStateController.CanReplayNode)
            {
                return PostRunReplayReasonKind.None;
            }

            RunResult runResult = postRunStateController.RunResult;
            if (runResult.ResolutionState != RunResolutionState.Succeeded)
            {
                return PostRunReplayReasonKind.RetryAttempt;
            }

            if (runResult.HasTrackedNodeProgress &&
                runResult.NodeProgressValue < runResult.NodeProgressThreshold)
            {
                return PostRunReplayReasonKind.ContinueNodeProgress;
            }

            if (HasRegionMaterialReward(runResult.RewardPayload))
            {
                return PostRunReplayReasonKind.FarmRegionMaterial;
            }

            if (runResult.RewardPayload.HasRewards)
            {
                return PostRunReplayReasonKind.FarmRewards;
            }

            return PostRunReplayReasonKind.RetryAttempt;
        }

        private static PostRunRecommendedActionKind ResolveRecommendedActionKind(
            PostRunStateController postRunStateController,
            string forwardTargetDisplayName,
            PostRunServiceOpportunityKind serviceOpportunityKind)
        {
            if (postRunStateController == null)
            {
                throw new ArgumentNullException(nameof(postRunStateController));
            }

            RunResult runResult = postRunStateController.RunResult;
            bool hasForwardPushOpportunity = !string.IsNullOrWhiteSpace(forwardTargetDisplayName);
            bool hasServiceOpportunity = serviceOpportunityKind != PostRunServiceOpportunityKind.None;

            if (postRunStateController.CanReturnToWorld &&
                hasForwardPushOpportunity &&
                (runResult.DidUnlockRoute || runResult.HasBossProgressionGateUnlock))
            {
                return PostRunRecommendedActionKind.ReturnToWorldPush;
            }

            if (postRunStateController.CanReturnToWorld && hasServiceOpportunity)
            {
                return PostRunRecommendedActionKind.ReturnToWorldService;
            }

            if (postRunStateController.CanReplayNode &&
                ResolveReplayReason(postRunStateController) != PostRunReplayReasonKind.None)
            {
                return PostRunRecommendedActionKind.Replay;
            }

            if (postRunStateController.CanReturnToWorld && hasForwardPushOpportunity)
            {
                return PostRunRecommendedActionKind.ReturnToWorldPush;
            }

            return PostRunRecommendedActionKind.Stop;
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

        private static bool HasRegionMaterialReward(RunRewardPayload rewardPayload)
        {
            if (rewardPayload == null)
            {
                return false;
            }

            foreach (RunMaterialReward reward in rewardPayload.MaterialRewards)
            {
                if (reward.ResourceCategory == ResourceCategory.RegionMaterial && reward.Amount > 0)
                {
                    return true;
                }
            }

            return false;
        }

        private static int ScoreForwardTarget(WorldNode candidate)
        {
            if (candidate == null)
            {
                return int.MinValue;
            }

            if (candidate.NodeType == NodeType.BossOrGate)
            {
                return 400;
            }

            if (candidate.NodeType == NodeType.ServiceOrProgression)
            {
                return 100;
            }

            if (candidate.RegionMaterialYieldContent != null)
            {
                return 200;
            }

            return 300;
        }

        private static bool HasWorldAnchor(PersistentWorldState persistentWorldState)
        {
            return persistentWorldState != null &&
                (persistentWorldState.HasCurrentNode || persistentWorldState.HasLastSafeNode);
        }
    }
}
