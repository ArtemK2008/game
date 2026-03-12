using System;
using System.Collections.Generic;

namespace Survivalon.Runtime
{
    public sealed class RunRewardPayload
    {
        private static readonly RunRewardPayload EmptyInstance = new RunRewardPayload(Array.Empty<RunResourceReward>());
        private readonly List<RunResourceReward> resourceRewards;

        public RunRewardPayload(IEnumerable<RunResourceReward> resourceRewards)
        {
            if (resourceRewards == null)
            {
                throw new ArgumentNullException(nameof(resourceRewards));
            }

            this.resourceRewards = new List<RunResourceReward>(resourceRewards);
        }

        public static RunRewardPayload Empty => EmptyInstance;

        public IReadOnlyList<RunResourceReward> ResourceRewards => resourceRewards;

        public bool HasRewards => resourceRewards.Count > 0;
    }
}
