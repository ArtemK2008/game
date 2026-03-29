using System;

namespace Survivalon.State.Persistence
{
    public sealed class OfflineProgressClaimService
    {
        private readonly SafeResumePersistenceService persistenceService;

        public OfflineProgressClaimService(SafeResumePersistenceService persistenceService)
        {
            this.persistenceService = persistenceService ?? throw new ArgumentNullException(nameof(persistenceService));
        }

        public void Claim(PersistentGameState gameState, OfflineProgressClaimState claimState)
        {
            if (gameState == null)
            {
                throw new ArgumentNullException(nameof(gameState));
            }

            if (claimState == null)
            {
                throw new ArgumentNullException(nameof(claimState));
            }

            if (!gameState.SafeResumeState.HasSafeResumeTarget)
            {
                throw new InvalidOperationException(
                    "Offline claim requires a persisted safe resume target.");
            }

            gameState.ResourceBalances.Add(claimState.ResourceCategory, claimState.Amount);
            persistenceService.SaveResolvedContext(gameState, gameState.SafeResumeState.TargetType);
        }
    }
}
