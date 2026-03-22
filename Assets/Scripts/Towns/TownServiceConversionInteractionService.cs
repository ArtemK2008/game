using System;
using Survivalon.Data.Towns;
using Survivalon.State.Persistence;

namespace Survivalon.Towns
{
    public sealed class TownServiceConversionInteractionService
    {
        private readonly SafeResumePersistenceService persistenceService;

        public TownServiceConversionInteractionService(SafeResumePersistenceService persistenceService = null)
        {
            this.persistenceService = persistenceService;
        }

        public bool TryConvert(PersistentGameState gameState, TownServiceConversionId conversionId)
        {
            if (gameState == null)
            {
                throw new ArgumentNullException(nameof(gameState));
            }

            TownServiceConversionDefinition conversionDefinition = TownServiceConversionCatalog.Get(conversionId);
            if (!gameState.ResourceBalances.TrySpend(
                    conversionDefinition.InputResourceCategory,
                    conversionDefinition.InputAmount))
            {
                return false;
            }

            gameState.ResourceBalances.Add(
                conversionDefinition.OutputResourceCategory,
                conversionDefinition.OutputAmount);

            if (persistenceService != null)
            {
                persistenceService.SaveResolvedWorldContext(gameState);
            }

            return true;
        }
    }
}
