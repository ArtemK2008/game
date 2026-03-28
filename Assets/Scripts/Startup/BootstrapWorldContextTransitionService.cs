using System;
using Survivalon.State;
using Survivalon.State.Persistence;
using Survivalon.Core;

namespace Survivalon.Startup
{
    public sealed class BootstrapWorldContextTransitionService
    {
        private readonly SafeResumePersistenceService persistenceService;

        public BootstrapWorldContextTransitionService(SafeResumePersistenceService persistenceService)
        {
            this.persistenceService = persistenceService ?? throw new ArgumentNullException(nameof(persistenceService));
        }

        public StartupEntryTarget PrepareReturnToWorld(
            PersistentGameState gameState,
            SessionContextState sessionContext,
            NodeId contextNodeId)
        {
            RecordAndPersist(
                gameState,
                sessionContext,
                contextNodeId,
                offerReturnToWorldReentry: true,
                safeResumeTargetType: SafeResumeTargetType.WorldMap);
            return StartupEntryTarget.WorldViewPlaceholder;
        }

        public StartupEntryTarget PrepareStopSession(
            PersistentGameState gameState,
            SessionContextState sessionContext,
            NodeId contextNodeId)
        {
            RecordAndPersist(
                gameState,
                sessionContext,
                contextNodeId,
                offerReturnToWorldReentry: false,
                safeResumeTargetType: SafeResumeTargetType.WorldMap);
            return StartupEntryTarget.MainMenuPlaceholder;
        }

        public StartupEntryTarget PrepareStopSessionFromTownService(
            PersistentGameState gameState,
            SessionContextState sessionContext,
            NodeId contextNodeId)
        {
            RecordAndPersist(
                gameState,
                sessionContext,
                contextNodeId,
                offerReturnToWorldReentry: false,
                safeResumeTargetType: SafeResumeTargetType.TownService);
            return StartupEntryTarget.MainMenuPlaceholder;
        }

        public void PersistResolvedPostRunBoundary(PersistentGameState gameState)
        {
            if (gameState == null)
            {
                throw new ArgumentNullException(nameof(gameState));
            }

            persistenceService.SaveResolvedWorldContext(gameState);
        }

        private void RecordAndPersist(
            PersistentGameState gameState,
            SessionContextState sessionContext,
            NodeId contextNodeId,
            bool offerReturnToWorldReentry,
            SafeResumeTargetType safeResumeTargetType)
        {
            if (gameState == null)
            {
                throw new ArgumentNullException(nameof(gameState));
            }

            if (sessionContext == null)
            {
                throw new ArgumentNullException(nameof(sessionContext));
            }

            if (offerReturnToWorldReentry)
            {
                sessionContext.OfferReturnToWorldReentry(contextNodeId);
            }
            else
            {
                sessionContext.RecordReturnedToWorldContext(contextNodeId);
                sessionContext.ConsumeReturnToWorldReentryOffer();
            }

            persistenceService.SaveResolvedContext(gameState, safeResumeTargetType);
        }
    }
}

