using System;
using UnityEngine;
using Survivalon.Core;

namespace Survivalon.State.Persistence
{
    public sealed class SafeResumePersistenceService
    {
        private readonly IPersistentGameStateStorage storage;
        private readonly Func<DateTimeOffset> utcNowProvider;
        private readonly OfflineProgressEligibilityResolver offlineProgressEligibilityResolver;

        public SafeResumePersistenceService(
            IPersistentGameStateStorage storage,
            Func<DateTimeOffset> utcNowProvider = null,
            OfflineProgressEligibilityResolver offlineProgressEligibilityResolver = null)
        {
            this.storage = storage ?? throw new ArgumentNullException(nameof(storage));
            this.utcNowProvider = utcNowProvider ?? (() => DateTimeOffset.UtcNow);
            this.offlineProgressEligibilityResolver = offlineProgressEligibilityResolver;
        }

        public PersistentGameState LoadOrCreate(PersistentGameState fallbackState)
        {
            if (fallbackState == null)
            {
                throw new ArgumentNullException(nameof(fallbackState));
            }

            if (TryLoadExisting(out PersistentGameState persistedState))
            {
                return persistedState;
            }

            return CloneGameState(fallbackState);
        }

        public bool TryLoadExisting(out PersistentGameState gameState)
        {
            if (!storage.TryLoad(out PersistentGameState persistedState))
            {
                gameState = null;
                return false;
            }

            gameState = CloneGameState(persistedState);
            return true;
        }

        public void SaveResolvedWorldContext(PersistentGameState gameState)
        {
            SaveResolvedContext(gameState, SafeResumeTargetType.WorldMap);
        }

        public void SaveResolvedTownServiceContext(PersistentGameState gameState)
        {
            SaveResolvedContext(gameState, SafeResumeTargetType.TownService);
        }

        public void SaveResolvedContext(
            PersistentGameState gameState,
            SafeResumeTargetType targetType)
        {
            SaveResolvedSafeContext(gameState, targetType);
        }

        private static NodeId ResolveResumeNodeId(PersistentWorldState worldState)
        {
            if (worldState == null)
            {
                throw new ArgumentNullException(nameof(worldState));
            }

            if (worldState.HasCurrentNode)
            {
                return worldState.CurrentNodeId;
            }

            if (worldState.HasLastSafeNode)
            {
                return worldState.LastSafeNodeId;
            }

            throw new InvalidOperationException(
                "Safe resume persistence requires a current node or last safe node.");
        }

        private void SaveResolvedSafeContext(
            PersistentGameState gameState,
            SafeResumeTargetType targetType)
        {
            if (gameState == null)
            {
                throw new ArgumentNullException(nameof(gameState));
            }

            PersistentGameState snapshot = CloneGameState(gameState);
            NodeId resumeNodeId = ResolveResumeNodeId(snapshot.WorldState);
            switch (targetType)
            {
                case SafeResumeTargetType.WorldMap:
                    snapshot.SafeResumeState.MarkWorldMap(resumeNodeId);
                    break;
                case SafeResumeTargetType.TownService:
                    snapshot.SafeResumeState.MarkTownService(resumeNodeId);
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(targetType), targetType, "Unsupported safe resume target type.");
            }

            OfflineProgressEligibilityKind eligibilityKind = offlineProgressEligibilityResolver?.Resolve(
                snapshot.WorldState,
                targetType,
                resumeNodeId) ?? OfflineProgressEligibilityKind.None;
            snapshot.OfflineProgressStableSaveAnchorState.StampStableSaveAnchor(
                utcNowProvider().ToUnixTimeSeconds(),
                eligibilityKind);
            storage.Save(snapshot);
        }

        private static PersistentGameState CloneGameState(PersistentGameState gameState)
        {
            string json = JsonUtility.ToJson(gameState);
            PersistentGameState clonedGameState = JsonUtility.FromJson<PersistentGameState>(json);
            if (clonedGameState == null)
            {
                throw new InvalidOperationException("Persistent game state could not be cloned for safe persistence.");
            }

            return clonedGameState;
        }
    }
}

