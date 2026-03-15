using System;

namespace Survivalon.Runtime
{
    public sealed class RunRewardResolutionService
    {
        private static readonly RunRewardPayload SuccessfulCombatRewardPayload = new RunRewardPayload(
            new[]
            {
                new RunCurrencyReward(ResourceCategory.SoftCurrency, 1),
            },
            Array.Empty<RunMaterialReward>());

        public RunRewardPayload Resolve(
            NodePlaceholderState nodeContext,
            RunResolutionState resolutionState)
        {
            if (nodeContext == null)
            {
                throw new ArgumentNullException(nameof(nodeContext));
            }

            return resolutionState == RunResolutionState.Succeeded && nodeContext.UsesCombatShell
                ? SuccessfulCombatRewardPayload
                : RunRewardPayload.Empty;
        }
    }
}
