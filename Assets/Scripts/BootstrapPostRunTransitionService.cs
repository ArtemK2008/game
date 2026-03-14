using System;

namespace Survivalon.Runtime
{
    public sealed class BootstrapPostRunTransitionService
    {
        private readonly SafeResumePersistenceService persistenceService;

        public BootstrapPostRunTransitionService(SafeResumePersistenceService persistenceService)
        {
            this.persistenceService = persistenceService ?? throw new ArgumentNullException(nameof(persistenceService));
        }

        public StartupEntryTarget PrepareReturnToWorld(
            PersistentGameState gameState,
            SessionContextState sessionContext,
            RunResult runResult)
        {
            RecordAndPersist(gameState, sessionContext, runResult);
            return StartupEntryTarget.WorldViewPlaceholder;
        }

        public StartupEntryTarget PrepareStopSession(
            PersistentGameState gameState,
            SessionContextState sessionContext,
            RunResult runResult)
        {
            RecordAndPersist(gameState, sessionContext, runResult);
            return StartupEntryTarget.MainMenuPlaceholder;
        }

        private void RecordAndPersist(
            PersistentGameState gameState,
            SessionContextState sessionContext,
            RunResult runResult)
        {
            if (gameState == null)
            {
                throw new ArgumentNullException(nameof(gameState));
            }

            if (sessionContext == null)
            {
                throw new ArgumentNullException(nameof(sessionContext));
            }

            if (runResult == null)
            {
                throw new ArgumentNullException(nameof(runResult));
            }

            sessionContext.RecordRunReturned(runResult.NodeId);
            persistenceService.SaveResolvedWorldContext(gameState);
        }
    }
}
