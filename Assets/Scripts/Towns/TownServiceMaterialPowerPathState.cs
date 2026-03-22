using System;
using System.Collections.Generic;

namespace Survivalon.Towns
{
    public sealed class TownServiceMaterialPowerPathState
    {
        public TownServiceMaterialPowerPathState(
            int readyRefinementCount,
            int regionMaterialTowardsNextRefinementAmount,
            int refinementInputRequirement,
            int persistentProgressionMaterialAfterRefinementPath,
            IReadOnlyList<string> alreadyAffordableProjectDisplayNames,
            IReadOnlyList<string> newProjectTargetDisplayNames)
        {
            if (readyRefinementCount < 0)
            {
                throw new ArgumentOutOfRangeException(
                    nameof(readyRefinementCount),
                    readyRefinementCount,
                    "Ready refinement count cannot be negative.");
            }

            if (refinementInputRequirement <= 0)
            {
                throw new ArgumentOutOfRangeException(
                    nameof(refinementInputRequirement),
                    refinementInputRequirement,
                    "Refinement input requirement must be positive.");
            }

            if (persistentProgressionMaterialAfterRefinementPath < 0)
            {
                throw new ArgumentOutOfRangeException(
                    nameof(persistentProgressionMaterialAfterRefinementPath),
                    persistentProgressionMaterialAfterRefinementPath,
                    "Persistent progression material projection cannot be negative.");
            }

            if (regionMaterialTowardsNextRefinementAmount < 0 ||
                regionMaterialTowardsNextRefinementAmount >= refinementInputRequirement)
            {
                throw new ArgumentOutOfRangeException(
                    nameof(regionMaterialTowardsNextRefinementAmount),
                    regionMaterialTowardsNextRefinementAmount,
                    "Region material progress must stay within the next refinement requirement.");
            }

            PersistentProgressionMaterialAmountAfterRefinementPath =
                persistentProgressionMaterialAfterRefinementPath;
            ReadyRefinementCount = readyRefinementCount;
            RegionMaterialTowardsNextRefinementAmount = regionMaterialTowardsNextRefinementAmount;
            RefinementInputRequirement = refinementInputRequirement;
            AlreadyAffordableProjectDisplayNames = alreadyAffordableProjectDisplayNames ??
                throw new ArgumentNullException(nameof(alreadyAffordableProjectDisplayNames));
            NewProjectTargetDisplayNames = newProjectTargetDisplayNames ??
                throw new ArgumentNullException(nameof(newProjectTargetDisplayNames));
        }

        public int ReadyRefinementCount { get; }

        public int RegionMaterialTowardsNextRefinementAmount { get; }

        public int RefinementInputRequirement { get; }

        public int PersistentProgressionMaterialAmountAfterRefinementPath { get; }

        public IReadOnlyList<string> AlreadyAffordableProjectDisplayNames { get; }

        public IReadOnlyList<string> NewProjectTargetDisplayNames { get; }
    }
}
