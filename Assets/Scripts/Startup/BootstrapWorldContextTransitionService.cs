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
            RecordAndPersist(gameState, sessionContext, contextNodeId);
            return StartupEntryTarget.WorldViewPlaceholder;
        }

        public StartupEntryTarget PrepareStopSession(
            PersistentGameState gameState,
            SessionContextState sessionContext,
            NodeId contextNodeId)
        {
            RecordAndPersist(gameState, sessionContext, contextNodeId);
            return StartupEntryTarget.MainMenuPlaceholder;
        }

        private void RecordAndPersist(
            PersistentGameState gameState,
            SessionContextState sessionContext,
            NodeId contextNodeId)
        {
            if (gameState == null)
            {
                throw new ArgumentNullException(nameof(gameState));
            }

            if (sessionContext == null)
            {
                throw new ArgumentNullException(nameof(sessionContext));
            }

            sessionContext.RecordReturnedToWorldContext(contextNodeId);
            persistenceService.SaveResolvedWorldContext(gameState);
        }
    }
}

