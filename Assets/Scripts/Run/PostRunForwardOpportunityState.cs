using System;

namespace Survivalon.Run
{
    public sealed class PostRunForwardOpportunityState
    {
        public PostRunForwardOpportunityState(
            string targetDisplayName = null,
            PostRunForwardOpportunityKind opportunityKind = PostRunForwardOpportunityKind.None)
        {
            if (opportunityKind != PostRunForwardOpportunityKind.None &&
                string.IsNullOrWhiteSpace(targetDisplayName))
            {
                throw new ArgumentException(
                    "Forward target display name cannot be null or whitespace when a forward opportunity exists.",
                    nameof(targetDisplayName));
            }

            TargetDisplayName = targetDisplayName;
            OpportunityKind = opportunityKind;
        }

        public string TargetDisplayName { get; }

        public PostRunForwardOpportunityKind OpportunityKind { get; }

        public bool HasOpportunity =>
            OpportunityKind != PostRunForwardOpportunityKind.None &&
            !string.IsNullOrWhiteSpace(TargetDisplayName);
    }
}
