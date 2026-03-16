using System;
using UnityEngine;
using Survivalon.Core;

namespace Survivalon.State.Persistence
{
    public sealed class SafeResumePersistenceService
    {
        private readonly IPersistentGameStateStorage storage;

        public SafeResumePersistenceService(IPersistentGameStateStorage storage)
        {
            this.storage = storage ?? throw new ArgumentNullException(nameof(storage));
        }

        public PersistentGameState LoadOrCreate(PersistentGameState fallbackState)
        {
            if (fallbackState == null)
            {
                throw new ArgumentNullException(nameof(fallbackState));
            }

            if (storage.TryLoad(out PersistentGameState persistedState))
            {
                return persistedState;
            }

            return CloneGameState(fallbackState);
        }

        public void SaveResolvedWorldContext(PersistentGameState gameState)
        {
            if (gameState == null)
            {
                throw new ArgumentNullException(nameof(gameState));
            }

            PersistentGameState snapshot = CloneGameState(gameState);
            NodeId resumeNodeId = ResolveResumeNodeId(snapshot.WorldState);
            snapshot.SafeResumeState.MarkWorldMap(resumeNodeId);
            storage.Save(snapshot);
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

