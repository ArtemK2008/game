using System;

namespace Survivalon.Run
{
    public sealed class PostRunServiceOpportunityState
    {
        public PostRunServiceOpportunityState(
            string serviceHubDisplayName = null,
            PostRunServiceOpportunityKind opportunityKind = PostRunServiceOpportunityKind.None)
        {
            if (opportunityKind != PostRunServiceOpportunityKind.None &&
                string.IsNullOrWhiteSpace(serviceHubDisplayName))
            {
                throw new ArgumentException(
                    "Service hub display name cannot be null or whitespace when a service opportunity exists.",
                    nameof(serviceHubDisplayName));
            }

            ServiceHubDisplayName = serviceHubDisplayName;
            OpportunityKind = opportunityKind;
        }

        public string ServiceHubDisplayName { get; }

        public PostRunServiceOpportunityKind OpportunityKind { get; }

        public bool HasOpportunity =>
            !string.IsNullOrWhiteSpace(ServiceHubDisplayName) &&
            OpportunityKind != PostRunServiceOpportunityKind.None;
    }
}
