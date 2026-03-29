using System;
using System.Collections.Generic;
using Survivalon.Core;
using Survivalon.Data.Rewards;
using Survivalon.World;

namespace Survivalon.State.Persistence
{
    public sealed class OfflineProgressClaimResolver
    {
        private const long SecondsPerHour = 60L * 60L;

        private readonly WorldGraph worldGraph;
        private readonly Func<DateTimeOffset> utcNowProvider;
        private readonly AccountWideProgressionEffectResolver progressionEffectResolver;
        private readonly WorldNodeDisplayNameResolver worldNodeDisplayNameResolver;

        public OfflineProgressClaimResolver(
            WorldGraph worldGraph,
            Func<DateTimeOffset> utcNowProvider = null,
            AccountWideProgressionEffectResolver progressionEffectResolver = null,
            WorldNodeDisplayNameResolver worldNodeDisplayNameResolver = null)
        {
            this.worldGraph = worldGraph ?? throw new ArgumentNullException(nameof(worldGraph));
            this.utcNowProvider = utcNowProvider ?? (() => DateTimeOffset.UtcNow);
            this.progressionEffectResolver =
                progressionEffectResolver ?? new AccountWideProgressionEffectResolver();
            this.worldNodeDisplayNameResolver =
                worldNodeDisplayNameResolver ?? new WorldNodeDisplayNameResolver();
        }

        public static int MaximumClaimableWholeHours => 2;

        public bool TryResolve(PersistentGameState gameState, out OfflineProgressClaimState claimState)
        {
            if (gameState == null)
            {
                throw new ArgumentNullException(nameof(gameState));
            }

            claimState = null;

            if (!CanClaim(gameState, out NodeId resumeNodeId, out int countedWholeHours))
            {
                return false;
            }

            if (!TryResolvePerHourRewardAmount(
                    gameState,
                    resumeNodeId,
                    out int perHourRewardAmount,
                    out string sourceNodeDisplayName))
            {
                return false;
            }

            claimState = new OfflineProgressClaimState(
                ResourceCategory.RegionMaterial,
                countedWholeHours * perHourRewardAmount,
                countedWholeHours,
                sourceNodeDisplayName);
            return true;
        }

        private bool CanClaim(
            PersistentGameState gameState,
            out NodeId resumeNodeId,
            out int countedWholeHours)
        {
            resumeNodeId = default;
            countedWholeHours = 0;

            PersistentOfflineProgressStableSaveAnchorState stableSaveAnchorState =
                gameState.OfflineProgressStableSaveAnchorState;
            if (!stableSaveAnchorState.HasStableSaveAnchor ||
                !stableSaveAnchorState.IsEligibleForOfflineProgress ||
                stableSaveAnchorState.EligibilityKind != OfflineProgressEligibilityKind.FarmReadyWorldNode)
            {
                return false;
            }

            if (!gameState.SafeResumeState.HasSafeResumeTarget ||
                gameState.SafeResumeState.TargetType != SafeResumeTargetType.WorldMap)
            {
                return false;
            }

            long currentUnixTimeSeconds = utcNowProvider().ToUnixTimeSeconds();
            long elapsedUnixTimeSeconds =
                currentUnixTimeSeconds - stableSaveAnchorState.LastStableSaveUnixTimeSeconds;
            if (elapsedUnixTimeSeconds < SecondsPerHour)
            {
                return false;
            }

            countedWholeHours = (int)Math.Min(
                elapsedUnixTimeSeconds / SecondsPerHour,
                MaximumClaimableWholeHours);
            if (countedWholeHours <= 0)
            {
                return false;
            }

            resumeNodeId = gameState.SafeResumeState.ResumeNodeId;
            return true;
        }

        private bool TryResolvePerHourRewardAmount(
            PersistentGameState gameState,
            NodeId resumeNodeId,
            out int perHourRewardAmount,
            out string sourceNodeDisplayName)
        {
            perHourRewardAmount = 0;
            sourceNodeDisplayName = null;

            try
            {
                WorldNode worldNode = worldGraph.GetNode(resumeNodeId);
                WorldRegion worldRegion = worldGraph.GetRegion(worldNode.RegionId);
                if (!RegionMaterialRewardSupportResolver.Supports(worldNode.NodeType, worldRegion.ResourceCategory))
                {
                    return false;
                }

                if (worldNode.RegionMaterialYieldContent == null)
                {
                    return false;
                }

                AccountWideProgressionEffectState progressionEffects =
                    progressionEffectResolver.Resolve(gameState.ProgressionState);
                int regionMaterialYieldBonus = worldNode.RegionMaterialYieldContent.RegionMaterialBonus;
                perHourRewardAmount =
                    RunRewardTuningCatalog.Current.OrdinaryCombatRegionMaterialReward.Amount +
                    progressionEffects.OrdinaryRegionMaterialRewardBonus +
                    regionMaterialYieldBonus;
                if (perHourRewardAmount <= 0)
                {
                    return false;
                }

                sourceNodeDisplayName = worldNodeDisplayNameResolver.Resolve(worldNode);
                return true;
            }
            catch (KeyNotFoundException)
            {
                return false;
            }
        }
    }
}
