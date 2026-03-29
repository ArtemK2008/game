using System;
using Survivalon.Data.Towns;
using Survivalon.State.Persistence;

namespace Survivalon.Towns
{
    public sealed class TownServiceConversionInteractionService
    {
        private readonly SafeResumePersistenceService persistenceService;
        private readonly AccountWideProgressionEffectResolver progressionEffectResolver;
        private readonly TownServiceConversionEffectResolver conversionEffectResolver;

        public TownServiceConversionInteractionService(
            SafeResumePersistenceService persistenceService = null,
            AccountWideProgressionEffectResolver progressionEffectResolver = null,
            TownServiceConversionEffectResolver conversionEffectResolver = null)
        {
            this.persistenceService = persistenceService;
            this.progressionEffectResolver =
                progressionEffectResolver ?? new AccountWideProgressionEffectResolver();
            this.conversionEffectResolver =
                conversionEffectResolver ?? new TownServiceConversionEffectResolver();
        }

        public bool TryConvert(PersistentGameState gameState, TownServiceConversionId conversionId)
        {
            if (gameState == null)
            {
                throw new ArgumentNullException(nameof(gameState));
            }

            TownServiceConversionDefinition conversionDefinition = TownServiceConversionCatalog.Get(conversionId);
            AccountWideProgressionEffectState progressionEffects =
                progressionEffectResolver.Resolve(gameState.ProgressionState);
            if (!gameState.ResourceBalances.TrySpend(
                    conversionDefinition.InputResourceCategory,
                    conversionDefinition.InputAmount))
            {
                return false;
            }

            int outputAmount = conversionEffectResolver.ResolveOutputAmount(
                conversionDefinition,
                progressionEffects);
            gameState.ResourceBalances.Add(
                conversionDefinition.OutputResourceCategory,
                outputAmount);

            if (persistenceService != null)
            {
                persistenceService.SaveResolvedTownServiceContext(gameState);
            }

            return true;
        }
    }
}
