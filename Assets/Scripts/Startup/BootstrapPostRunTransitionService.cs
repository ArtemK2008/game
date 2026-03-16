using System;
using Survivalon.Runtime;
using Survivalon.Runtime.Run;
using Survivalon.Runtime.State;
using Survivalon.Runtime.State.Persistence;
using Survivalon.Runtime.World;
using Survivalon.Runtime.Core;

namespace Survivalon.Runtime.Startup
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
