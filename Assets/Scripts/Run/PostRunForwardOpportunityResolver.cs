using System;
using System.Collections.Generic;
using Survivalon.Core;
using Survivalon.State.Persistence;
using Survivalon.World;

namespace Survivalon.Run
{
    public sealed class PostRunForwardOpportunityResolver
    {
        private readonly WorldNodeAccessResolver worldNodeAccessResolver;
        private readonly WorldNodeDisplayNameResolver worldNodeDisplayNameResolver;

        public PostRunForwardOpportunityResolver(
            WorldNodeAccessResolver worldNodeAccessResolver = null,
            WorldNodeDisplayNameResolver worldNodeDisplayNameResolver = null)
        {
            this.worldNodeAccessResolver = worldNodeAccessResolver ?? new WorldNodeAccessResolver();
            this.worldNodeDisplayNameResolver = worldNodeDisplayNameResolver ?? new WorldNodeDisplayNameResolver();
        }

        public PostRunForwardOpportunityState Resolve(
            RunResult runResult,
            WorldGraph worldGraph,
            PersistentWorldState persistentWorldState)
        {
            if (runResult == null || worldGraph == null)
            {
                return new PostRunForwardOpportunityState();
            }

            if (runResult.HasBossProgressionGateUnlock &&
                runResult.BossProgressionGateUnlock.TryGetUnlockedNodeId(out NodeId unlockedNodeId))
            {
                return new PostRunForwardOpportunityState(
                    worldNodeDisplayNameResolver.Resolve(worldGraph, unlockedNodeId),
                    PostRunForwardOpportunityKind.NewlyUnlockedPushTarget);
            }

            if (persistentWorldState == null || !HasWorldAnchor(persistentWorldState))
            {
                return new PostRunForwardOpportunityState();
            }

            IReadOnlyList<WorldNode> forwardNodes =
                worldNodeAccessResolver.GetForwardEnterableNodes(worldGraph, persistentWorldState);

            WorldNode preferredNode = null;
            int preferredNodeScore = int.MinValue;
            for (int index = 0; index < forwardNodes.Count; index++)
            {
                WorldNode candidate = forwardNodes[index];
                if (!IsPushTarget(candidate))
                {
                    continue;
                }

                int score = ScorePushTarget(candidate);
                if (score <= preferredNodeScore)
                {
                    continue;
                }

                preferredNode = candidate;
                preferredNodeScore = score;
            }

            return preferredNode == null
                ? new PostRunForwardOpportunityState()
                : new PostRunForwardOpportunityState(
                    worldNodeDisplayNameResolver.Resolve(preferredNode),
                    PostRunForwardOpportunityKind.AvailablePushTarget);
        }

        private static bool IsPushTarget(WorldNode candidate)
        {
            return candidate != null &&
                candidate.NodeType != NodeType.ServiceOrProgression;
        }

        private static int ScorePushTarget(WorldNode candidate)
        {
            if (candidate == null)
            {
                return int.MinValue;
            }

            if (candidate.NodeType == NodeType.BossOrGate)
            {
                return 400;
            }

            if (candidate.RegionMaterialYieldContent != null)
            {
                return 200;
            }

            return 300;
        }

        private static bool HasWorldAnchor(PersistentWorldState persistentWorldState)
        {
            return persistentWorldState.HasCurrentNode || persistentWorldState.HasLastSafeNode;
        }
    }
}
